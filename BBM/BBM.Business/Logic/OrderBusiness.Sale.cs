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
        public List<donhang> GetOrder_Sale(PagingInfo pageinfo, int BranchesId, out int count, out int min, out double totalMoney)
        {
            var channel = unitOfWork.ChannelRepository.GetById(BranchesId);

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

            UpdateStockByBranches(model, User, true);

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

            #region Customer
            if (order.makh != null)
            {
                var customer = unitOfWork.CutomerRepository.GetById(order.makh);
                if (customer != null)
                {
                    if (order.Status != (int)StatusOrder_Sale.Done
                         && order.Status != (int)StatusOrder_Sale.Cancel
                         && model.Status == (int)StatusOrder_Sale.Cancel)
                    {
                        switch (model.Status)
                        {
                            case (int)StatusOrder_Sale.Cancel:
                                var mark = double.Parse(customer.diem) - (order.tongtien / 1000);
                                customer.diem = mark.ToString();
                                break;
                        }
                    }
                    if (!model.Customer.Address.Equals(customer.duong)
                        || !model.Customer.Name.Equals(customer.hoten))
                    {
                        var cus = new khachhang
                        {
                            MaKH = customer.MaKH,
                            hoten = model.Customer.Name,
                            duong = model.Customer.Address,
                            diem = customer.diem
                        };
                        unitOfWork.CutomerRepository.Update(cus, o => o.hoten, o => o.duong, o => o.diem);
                    }
                }
            }
            #endregion

            #region Order

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

            #endregion

            UpdateStockByBranches(model, User);

            await unitOfWork.SaveChanges();

            return new Tuple<bool, string>(true, string.Empty);
        }

        public OrderModel GetInfoOrder(int Id)
        {
            var rs = unitOfWork.OrderSaleRepository.GetById(Id);

            if (rs == null)
            {
                return null;
            }

            var order = Mapper.Map<OrderModel>(rs);


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
                            it.ProductId = product.id;
                        }
                    }

                }
            }

            return order;
        }

        public async Task<bool> UpdateStatusOrders(List<long> ids, StatusOrder_Sale status, UserCurrent User)
        {
            if (ids.Count < 0)
                return false;

            bool isCommit = false;

            foreach (var id in ids)
            {
                var order = unitOfWork.OrderSaleRepository.GetById(id);
                if (order != null
                    && order.Status != (int)StatusOrder_Sale.Cancel
                    && order.Status != (int)StatusOrder_Sale.Done)
                {

                    order.Status = (int)status;

                    unitOfWork.OrderSaleRepository.Update(order, o => o.Status);

                    var model = Mapper.Map<OrderModel>(order);

                    foreach(var pro in model.Detail)
                    {
                        var thisDetal = order.donhang_ct.FirstOrDefault(o => o.Id == pro.Id);

                        pro.ProductId = thisDetal.shop_bienthe.shop_sanpham.id;
                    }

                    model.TypeOrder = (int)TypeOrder.Sale;

                    UpdateStockByBranches(model, User);

                    isCommit = true;
                }
            }

            if (isCommit)
            {
                await unitOfWork.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
