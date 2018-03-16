using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace BBM.Business.Logic
{
    public partial class OrderBusiness : IOrderBusiness
    {
        private IUnitOfWork unitOfWork;

        public OrderBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        private void UpdateStockByBranches(OrderModel order, UserCurrent User)
        {
            foreach (var item in order.Detail)
            {
                var StockProduct = unitOfWork.BrachesStockRepository.FindBy(o => o.ProductId == item.ProductId).ToList();

                if (order.TypeOrder == (int)TypeOrder.Input)
                {
                    var product = unitOfWork.ProductRepository.FindBy(o => o.id == item.ProductId).FirstOrDefault();

                    var priceBase = product.PriceBase;

                    if (product != null)
                    {
                        #region Product

                        product.PriceInput = (int)item.Price;

                        product.PriceCompare = item.PriceCompare;

                        #region Giá cơ bản

                        product.PriceBase_Old = product.PriceBase;

                        var stockbySum = StockProduct != null && StockProduct.Count > 0 ? StockProduct.Sum(o => o.Stock_Total) : 0;

                        var pricebase_old = product.PriceBase != null ? product.PriceBase : 0;

                        var sumTotal = (item.Total + stockbySum);

                        product.PriceBase = (int)(((stockbySum * pricebase_old) + (item.Total * (int)item.Price)) / (sumTotal != 0 ? sumTotal : 1));

                        unitOfWork.ProductRepository.Update(product, o => o.PriceBase, o => o.PriceInput, o => o.PriceBase_Old, o => o.PriceCompare);
                        #endregion

                        #region Giá sỉ

                        if (priceBase != product.PriceBase)
                            UpdatePriceWholesale(product, User);

                        #endregion

                        #endregion

                        #region Stock
                        var stockTo = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == order.Id_To);
                        if (stockTo != null)
                        {

                            stockTo.Stock_Total = stockTo.Stock_Total + item.Total;
                            stockTo.DateUpdate = DateTime.Now;
                            stockTo.EmployeeUpdate = User.UserId;

                            unitOfWork.BrachesStockRepository.Update(stockTo, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                        }
                        else
                        {
                            var stockNewTo = new soft_Branches_Product_Stock
                            {
                                BranchesId = order.Id_To,
                                ProductId = item.ProductId,
                                Stock_Total = item.Total,
                                DateCreate = DateTime.Now,
                                EmployeeCreate = order.EmployeeCreate
                            };
                            unitOfWork.BrachesStockRepository.Add(stockNewTo);
                        }
                        #endregion
                    }
                }

                if (order.TypeOrder == (int)TypeOrder.Output)
                {
                    #region stockTo
                    var stockTo = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == order.Id_To);

                    if (stockTo != null)
                    {
                        stockTo.Stock_Total = stockTo.Stock_Total + item.Total;
                        stockTo.DateUpdate = DateTime.Now;
                        stockTo.EmployeeUpdate = User.UserId;

                        unitOfWork.BrachesStockRepository.Update(stockTo, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                    }
                    else
                    {
                        stockTo = new soft_Branches_Product_Stock
                        {
                            ProductId = item.ProductId,
                            BranchesId = User.BranchesId,
                            DateCreate = DateTime.Now,
                            Stock_Total = item.Total,
                            EmployeeCreate = User.UserId
                        };
                        unitOfWork.BrachesStockRepository.Add(stockTo);
                    }
                    #endregion

                    #region stockFrom
                    var stockFrom = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == order.Id_From);

                    if (stockFrom != null)
                    {
                        stockTo.Stock_Total = stockFrom.Stock_Total - item.Total;
                        stockTo.DateUpdate = order.DateCreate;
                        stockTo.EmployeeUpdate = order.EmployeeCreate;

                        unitOfWork.BrachesStockRepository.Update(stockTo, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                    }
                    else
                    {
                        var stockNewTo = new soft_Branches_Product_Stock
                        {

                            BranchesId = order.Id_From,
                            ProductId = item.ProductId,
                            Stock_Total = 0 - item.Total,
                            DateCreate = order.DateCreate,
                            EmployeeCreate = order.EmployeeCreate,
                            DateUpdate = null,
                            EmployeeUpdate = null
                        };

                        unitOfWork.BrachesStockRepository.Add(stockNewTo);
                    }

                    #endregion
                }

                if (order.TypeOrder == (int)TypeOrder.Sale)
                {
                    #region Stock
                    var stockBraches = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == User.BranchesId);

                    if (stockBraches != null)
                    {
                        double total_stock = 0;
                        switch (order.Status)
                        {
                            case (int)StatusOrder_Sale.Cancel:
                            case (int)StatusOrder_Sale.Refund:
                            case (int)StatusOrder_Sale.ShipCancel:
                                total_stock = stockBraches.Stock_Total + item.Total;
                                break;
                            case (int)StatusOrder_Sale.Done:
                            case (int)StatusOrder_Sale.Process:
                                total_stock = stockBraches.Stock_Total - item.Total;
                                break;
                        }

                        stockBraches.Stock_Total = total_stock;

                        unitOfWork.BrachesStockRepository.Update(stockBraches, o => o.Stock_Total);
                    }
                    else
                    {
                        double total_stock = 0;
                        switch (order.Status)
                        {
                            case (int)StatusOrder_Sale.Cancel:
                            case (int)StatusOrder_Sale.Refund:
                                total_stock = 0;
                                break;
                            case (int)StatusOrder_Sale.Done:
                            case (int)StatusOrder_Sale.Process:
                                total_stock = 0 - item.Total;
                                break;
                        }

                        var newstock = new soft_Branches_Product_Stock
                        {
                            BranchesId = User.BranchesId,
                            ProductId = item.ProductId,
                            Stock_Total = total_stock,
                            DateCreate = DateTime.Now,
                            EmployeeCreate = User.UserId
                        };

                        unitOfWork.BrachesStockRepository.Add(newstock);
                    }
                    #endregion                 
                }
            }
        }

        public void UpdatePriceWholesale(shop_sanpham product, UserCurrent User, bool isCommit = false)
        {
            #region Giá sỉ

            var channelOnline = unitOfWork.ChannelRepository.FindBy(o => o.Type == (int)TypeChannel.IsChannelOnline && o.Id == User.ChannelId).FirstOrDefault();

            if (channelOnline != null)
            {
                var priceOL = product.soft_Channel_Product_Price.FirstOrDefault(o => o.soft_Channel.Type == (int)TypeChannel.IsChannelOnline);

                int? priceWholesale = 0;

                if (priceOL != null)
                    priceWholesale = (int)((priceOL.Price - product.PriceBase) / 5.3) + product.PriceBase;
                else
                    priceWholesale = (int)((0 - product.PriceBase) / 5.3) + product.PriceBase;

                var priceSi = unitOfWork.ChanelPriceRepository.Get(o => o.ProductId == product.id && o.soft_Channel.Type == (int)TypeChannel.IsChannelWholesale).FirstOrDefault();

                if (priceSi != null)
                {
                    priceSi.Price = (int)priceWholesale;

                    unitOfWork.ChanelPriceRepository.Update(priceSi, o => o.Price);
                }
                else
                {
                    var ChannelSi = unitOfWork.ChannelRepository.Get(o => o.Type == (int)TypeChannel.IsChannelWholesale).FirstOrDefault();

                    unitOfWork.ChanelPriceRepository.Add(new soft_Channel_Product_Price
                    {
                        Price = (int)priceWholesale,
                        ChannelId = ChannelSi.Id,
                        DateCreate = DateTime.Now,
                        EmployeeCreate = User.UserId,
                        ProductId = product.id
                    });
                }
            }

            if (isCommit)
                unitOfWork.SaveChanges();

            #endregion
        }

        public async Task<OrderModel> UpdateStatus(OrderModel model)
        {
            var order = unitOfWork.soft_Order.GetById(model.Id);

            unitOfWork.soft_Order.Update(order, o => model.Status);

            await unitOfWork.SaveChanges();

            return Mapper.Map<OrderModel>(unitOfWork.soft_Order.GetById(model.Id));
        }
    }
}
