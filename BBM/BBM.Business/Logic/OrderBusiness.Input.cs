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
        public List<OrderModel> GetOrder_Inside(PagingInfo pageinfo, int BranchesId, out int count, out int min)
        {
            return Mapper.Map<List<OrderModel>>(unitOfWork.OrderRepository.SearchBy(pageinfo, BranchesId, out count, out min));
        }
        public bool AddOrder_Input(OrderModel model, UserCurrent User, bool isDone = true, int OrderSuppliersId = 0)
        {
            var objOrder = Mapper.Map<soft_Order>(model);
            if (OrderSuppliersId > 0)
                objOrder.Id_From = OrderSuppliersId;
            objOrder.Id_To = User.BranchesId;
            objOrder.DateCreate = DateTime.Now;
            objOrder.EmployeeCreate = User.UserId;
            objOrder.TypeOrder = (int)TypeOrder.Input;
            objOrder.Status = isDone ? (int)StatusOrder_Input.Done : (int)StatusOrder_Input.Process;

            unitOfWork.OrderRepository.Add(objOrder);

            if (OrderSuppliersId > 0 && isDone)
                UpdateOrderSuppliers(model, OrderSuppliersId);



            model.Id_To = User.BranchesId;
            model.TypeOrder = (int)TypeOrder.Input;

            if (isDone)
            {
                UpdateStockByBranches(model, User);
            }
            else
            {
                UpdatePriceCompare(model);
            }

            UpdatePrice_Channel(model);

            unitOfWork.SaveChanges();

            return true;
        }
        
        private void UpdateOrderSuppliers(OrderModel model, int OrderSuppliersId)
        {
            var orderSuppliers = unitOfWork.OrderRepository.FindBy(o => o.Id.Equals(OrderSuppliersId)
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
                    unitOfWork.OrderRepository.Update(orderSuppliers, o => o.Status);
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

        private void UpdatePrice_Channel(OrderModel order)
        {
            foreach (var item in order.Detail)
            {
                if (item.PriceChannels != null && item.PriceChannels.Count > 0)

                    foreach (var pricechannel in item.PriceChannels)
                    {
                        var product = unitOfWork.ProductRepository.FindBy(o => o.id == item.ProductId).FirstOrDefault();

                        if (product != null)
                        {
                            var hasprice = product.soft_Channel_Product_Price.FirstOrDefault(o => o.ChannelId == pricechannel.Id && o.ProductId == item.ProductId);

                            if (hasprice != null && hasprice.Price != pricechannel.Price)
                            {
                                hasprice.Price = pricechannel.Price;
                                
                                unitOfWork.ChanelPriceRepository.Update(hasprice, o => o.Price);
                            }
                        }
                    }
            }

        }
    }
}
