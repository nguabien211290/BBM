using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public partial class OrderBusiness
    {
        public List<OrderModel> GetOrder_Input(PagingInfo pageinfo, int BranchesId, out int count, out int min)
        {
            double totalMoney = 0;
            return Mapper.Map<List<OrderModel>>(unitOfWork.OrderInputRepository.SearchBy(pageinfo, out count, out min, out totalMoney, BranchesId));
        }
        public async Task<bool> AddOrder_Input(OrderModel model, UserCurrent User, bool isDone = true, int OrderSuppliersId = 0)
        {

            model.Id_To = User.BranchesId;
            model.TypeOrder = (int)TypeOrder.Input;
            model.DateCreate = DateTime.Now;
            model.EmployeeCreate = User.UserId;
            if (OrderSuppliersId > 0)
                model.Id_From = OrderSuppliersId;
            model.Status = isDone ? (int)StatusOrder_Input.Done : (int)StatusOrder_Input.Process;

            var objOrder = Mapper.Map<soft_Order>(model);

            unitOfWork.OrderInputRepository.Add(objOrder);

            if (OrderSuppliersId > 0 && isDone)
                UpdateOrderSuppliers(model, OrderSuppliersId);

            if (isDone)
            {
                UpdateStockByBranches(model, User);
            }
            else
            {
                UpdatePriceCompare(model);
            }

            //UpdatePrice_Channel(model, User);

            await unitOfWork.SaveChanges();

            return true;
        }

        private void UpdateOrderSuppliers(OrderModel model, int OrderSuppliersId)
        {
            var orderSuppliers = unitOfWork.OrderInputRepository.FindBy(o => o.Id.Equals(OrderSuppliersId)
                                                && o.TypeOrder == (int)TypeOrder.OrderProduct
                                                && o.Status == (int)StatusOrder_Input.Process).FirstOrDefault();

            if (orderSuppliers != null)
            {
                foreach (var item in orderSuppliers.soft_Order_Child)
                {
                    if (item.Status == null || item.Status == (int)StatusOrder_Input.Process)
                    {
                        var hasProduct = model.Detail.FirstOrDefault(o => o.ProductId == item.ProductId.Value);

                        if (hasProduct != null)
                        {
                            if (item.Total <= hasProduct.Total)
                            {
                                item.Status = (int)StatusOrder_Input.Done;
                                unitOfWork.OrderChildRepository.Update(item, o => o.Status);
                            }
                            else
                            {
                                item.Total = item.Total - hasProduct.Total;
                                unitOfWork.OrderChildRepository.Update(item, o => o.Total);
                            }
                        }
                    }
                }

                var OrderChildDoneAll = orderSuppliers.soft_Order_Child.FirstOrDefault(o => o.Status == null
                                                    || o.Status == (int)StatusOrder_Input.Process);
                if (OrderChildDoneAll == null)
                {
                    orderSuppliers.Status = (int)StatusOrder_Input.Done;
                    unitOfWork.OrderInputRepository.Update(orderSuppliers, o => o.Status);
                }
            }
        }

        private void UpdatePriceCompare(OrderModel order)
        {
            foreach (var item in order.Detail)
            {
                var product = unitOfWork.ProductRepository.FindBy(o => o.id == item.ProductId).FirstOrDefault();

                if (product != null)
                {
                    product.PriceCompare = item.PriceCompare;

                    unitOfWork.ProductRepository.Update(product, o => o.PriceCompare);
                }
            }
        }

        private void UpdatePrice_Channel(OrderModel order, UserCurrent User)
        {
            foreach (var item in order.Detail)
            {

                var product = unitOfWork.ProductRepository.FindBy(o => o.id == item.ProductId).FirstOrDefault();

                if (product != null)
                {
                    var hasprice = product.soft_Channel_Product_Price.FirstOrDefault(o => o.ChannelId == User.ChannelId && o.ProductId == item.ProductId);

                    if (hasprice != null && hasprice.Price != item.Price)
                    {
                        hasprice.Price = item.Price;

                        unitOfWork.ChanelPriceRepository.Update(hasprice, o => o.Price);
                    }
                    else
                    {
                        unitOfWork.ChanelPriceRepository.Add(new soft_Channel_Product_Price
                        {
                            ChannelId = User.ChannelId,
                            Price = item.Price,
                            ProductId = item.ProductId,
                            DateCreate = DateTime.UtcNow,
                            EmployeeCreate = User.UserId
                        });
                    }
                }
            }

        }
    }
}
