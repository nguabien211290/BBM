using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public partial class OrderBusiness
    {
        public List<donhang> GetOrder_Sale(PagingInfo pageinfo, int BranchesId, out int count, out int min)
        {
            var channel = unitOfWork.ChannelRepository.GetById(BranchesId);
            double totalMoney = 0;
            return unitOfWork.OrderSaleRepository.SearchBy(pageinfo, out count, out min, out totalMoney, BranchesId);
        }

        public async Task<Tuple<OrderModel, bool>> CreatOrder_Sale(OrderModel model, bool isDone, UserCurrent User)
        {
            #region Customer
            if (model.Customer.Id > 0)
            {
                var customer = unitOfWork.CutomerRepository.GetById(model.Customer.Id);
                if (customer != null)
                {
                    model.Id_Customer = model.Customer.Id;
                    customer.diem = model.Customer.Mark;
                    customer.dienthoai = model.Customer.Phone;
                    customer.hoten = model.Customer.Name;
                    customer.duong = model.Customer.Address;

                    unitOfWork.CutomerRepository.Update(customer, o => o.diem, o => o.hoten, o => o.duong, o => o.dienthoai);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Customer.Phone))
                {

                    if (string.IsNullOrEmpty(model.Customer.Name))
                    {
                        model.Customer.Name = model.Customer.Phone;
                    }
                    if (string.IsNullOrEmpty(model.Customer.Address))
                    {
                        model.Customer.Address = model.Customer.Phone;
                    }
                    if (string.IsNullOrEmpty(model.Customer.User))
                    {
                        model.Customer.User = model.Customer.Phone;
                    }
                    if (string.IsNullOrEmpty(model.Customer.Pwd))
                    {
                        model.Customer.Pwd = model.Customer.Phone;
                    }

                    model.Customer.Email = model.Customer.Phone;

                    var customer = Mapper.Map<khachhang>(model.Customer);

                    unitOfWork.CutomerRepository.Add(customer);

                    model.Id_To = customer.MaKH;
                }

            }

            #endregion

            var channel = unitOfWork.ChannelRepository.GetById(User.ChannelId);

            var objOrder = Mapper.Map<donhang>(model);

            foreach (var item in objOrder.donhang_ct)
            {
                var variant = unitOfWork.VariantRepository.FindBy(o => o.idsp == item.IdPro).FirstOrDefault();

                if (variant == null)
                {
                    var newvariant = new shop_bienthe
                    {
                        idsp = (int)item.IdPro,
                        title = "default"
                    };
                    unitOfWork.VariantRepository.Add(newvariant);
                    await unitOfWork.SaveChanges();
                    item.IdPro = newvariant.id;

                }
                else
                {
                    item.IdPro = variant.id;
                }
            }


            objOrder.Status = isDone ? (int)StatusOrder_Sale.Done : (int)StatusOrder_Sale.Process;

            objOrder.EmployeeCreate = User.UserId;

            objOrder.Code = DateTime.Now.ToString("dd/MM/yy").Replace("/", "") + "-" + channel.Code;

            objOrder.ngaydat = DateTime.Now;

            objOrder.khachhang = null;

            objOrder.StatusPrint = string.Empty;

            #region in don hang

            var isPrint = false;

            if (channel.Type == (int)TypeChannel.IsMainStore)
            {
                isPrint = objOrder.Status == (int)StatusOrder_Sale.Done ? true : false;
            }
            else if (channel.Type == (int)TypeChannel.IsChannelOnline)
            {
                isPrint = objOrder.Status == (int)StatusOrder_Sale.Process ? true : false;
            }

            if (isPrint)
                objOrder.StatusPrint = "<li>" + User.UserName + " đã in (" + DateTime.Now + ")</li>";

            #endregion


            unitOfWork.OrderSaleRepository.Add(objOrder);

            model.TypeOrder = (int)TypeOrder.Sale;

            model.Status = objOrder.Status.Value;

            UpdateStockByBranches(model, User);

            await unitOfWork.SaveChanges();

            var rs = Mapper.Map<OrderModel>(objOrder);

            return new Tuple<OrderModel, bool>(rs, isPrint);
        }

        public async Task<Tuple<bool, string>> UpdateOrder_Sale(OrderModel model, UserCurrent User)
        {
            var error = string.Empty;

            var order = unitOfWork.OrderSaleRepository.GetById(model.Id);

            if (order == null)
            {
                error = "Không tìm thấy đơn hàng này.";
                return new Tuple<bool, string>(false, error);
            }

            if (order.Status == (int)StatusOrder_Sale.Cancel
                || order.Status == (int)StatusOrder_Sale.Done)
            {
                error = "Đơn hàng này đã được xử lý.";
                return new Tuple<bool, string>(false, error);
            }

            #region Order

            foreach (var item in model.Detail)
            {
                var hasthis = order.donhang_ct.FirstOrDefault(o => o.IdPro == item.ProductId);
                if (hasthis == null)
                {
                    var OrderChild = new donhang_ct
                    {
                        Sodh = order.id,
                        Dongia = item.Price,
                        Soluong = item.Total,
                        IdPro = item.ProductId
                    };
                    unitOfWork.OrderSale_DetailRepository.Add(OrderChild);
                }
                else
                {
                    if (hasthis.Soluong != item.Total || hasthis.Dongia != item.Price)
                    {
                        var OrderChild = new donhang_ct
                        {
                            Id = hasthis.Id,
                            Sodh = order.id,
                            Dongia = item.Price,
                            Soluong = item.Total,
                            IdPro = item.ProductId
                        };
                        unitOfWork.OrderSale_DetailRepository.Update(OrderChild, o => o.Soluong, o => o.Dongia);
                    }
                }
            }

            order.ghichu = model.Note;

            order.Status = model.Status;

            order.EmployeeShip = model.EmployeeShip;

            if (model.Status == (int)StatusOrder_Sale.Done
                || (model.Status == (int)StatusOrder_Sale.Shipped && User.IsPrimary)
                )
            {
                if (!string.IsNullOrEmpty(order.StatusPrint))
                    order.StatusPrint = order.StatusPrint + " <li>" + User.UserName + " đã in (" + DateTime.Now + ")</li>";
                else
                    order.StatusPrint = "<li>" + User.UserName + " đã in (" + DateTime.Now + ")</li>";

            }

            unitOfWork.OrderSaleRepository.Update(order, o => o.Status, o => o.ghichu, o => o.EmployeeShip);

            #region Customer
            if (order.makh != null)
            {
                var customer = unitOfWork.CutomerRepository.GetById(order.makh);
                if (customer != null)
                {
                    if (!model.Customer.Address.Equals(customer.duong) || !model.Customer.Name.Equals(customer.hoten))
                    {
                        var cus = new khachhang
                        {
                            MaKH = customer.MaKH,
                            hoten = model.Customer.Name,
                            duong = model.Customer.Address,
                            diem = customer.diem
                        };

                        if ((order.Status != (int)StatusOrder_Sale.Done
                            && model.Status == (int)StatusOrder_Sale.Done)
                               || model.Status == (int)StatusOrder_Sale.Cancel
                               || model.Status == (int)StatusOrder_Sale.ShipCancel
                               || model.Status == (int)StatusOrder_Sale.Refund)
                        {
                            switch (order.Status)
                            {
                                case (int)StatusOrder_Sale.Cancel:
                                case (int)StatusOrder_Sale.Refund:
                                case (int)StatusOrder_Sale.ShipCancel:
                                    var mark = double.Parse(customer.diem) - order.tongtien;
                                    customer.diem = mark.ToString();
                                    break;
                            }
                        }
                        unitOfWork.CutomerRepository.Update(cus, o => o.hoten, o => o.duong, o => o.diem);
                    }
                }
            }
            #endregion

            #endregion

            if ((order.Status != (int)StatusOrder_Sale.Done
                && model.Status == (int)StatusOrder_Sale.Done)
                || model.Status == (int)StatusOrder_Sale.Cancel
                || model.Status == (int)StatusOrder_Sale.Refund
                || model.Status == (int)StatusOrder_Sale.ShipCancel)
                UpdateStockByBranches(model, User);


            await unitOfWork.SaveChanges();

            return new Tuple<bool, string>(true, string.Empty);
        }

        public OrderModel GetInfoOrder(int Id)
        {
            var rs = unitOfWork.OrderSaleRepository.GetById(Id);

            var order = Mapper.Map<OrderModel>(rs);

            if (order == null)
            {
                return null;
            }

            if (rs.Channeld != null)
            {

                var channel = unitOfWork.ChannelRepository.GetById(rs.Channeld);

                order.ChannelName = channel.Channel;

                order.isChannelOnline = channel.Type == (int)TypeChannel.IsChannelOnline ? true : false;

            }
            else
            {
                order.isChannelOnline = true;
                order.ChannelName = "Kênh Online";
            }

            if (order.Detail.Count > 0)
            {
                foreach (var it in order.Detail)
                {
                    if (it.ProductId > 0)
                    {
                        var product = unitOfWork.VariantRepository.FindBy(o => o.id == it.ProductId).Select(o => new ProductSampleModel
                        {
                            id = o.idsp.Value,
                            masp = o.shop_sanpham.masp,
                            tensp = o.shop_sanpham.tensp,
                        }).FirstOrDefault();

                        if (product != null)
                        {
                            it.Product = product;
                        }
                    }

                }
            }

            return order;
        }

        public async Task<bool> CancelOrder(int id)
        {
            var order = unitOfWork.OrderSaleRepository.GetById(id);

            if (order == null)
            {
                return false;
            }

            order.Status = (int)StatusOrder_Sale.Cancel;

            unitOfWork.OrderSaleRepository.Update(order, o => o.Status);

            await unitOfWork.SaveChanges();

            return true;
        }
    }
}
