using AutoMapper;
using BBM.Business.Model.Module;
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
    public class ProductBusiness : IProductBusiness
    {
        private IUnitOfWork unitOfWork;

        public ProductBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        //public List<Prodcut_Branches_PriceChannel> GetProductby(PagingInfo pageinfo, out int count, out int min, UserCurrent User)
        //{
           
        //    if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
        //    {
        //        foreach (var item in pageinfo.filterby)
        //        {
        //            var key = 0;

        //            var ids = new List<int>();

        //            switch (item.Fiter)
        //            {
        //                case "Price":
        //                    key = int.Parse(item.Name);
        //                    if (item.Value == "Equals")
        //                    {
        //                        ids = unitOfWork.ChanelPriceRepository.FindBy(o => o.Price == key
        //                       && o.ChannelId == User.ChannelId).Select(p => p.ProductId).ToList();
        //                    }
        //                    if (item.Value == "LessThan")
        //                    {
        //                        ids = unitOfWork.ChanelPriceRepository.FindBy(o => o.Price < key
        //                        && o.ChannelId == User.ChannelId).Select(p => p.ProductId).ToList();
        //                    }
        //                    if (item.Value == "MoreThan")
        //                    {
        //                        ids = unitOfWork.ChanelPriceRepository.FindBy(o => o.Price > key
        //                         && o.ChannelId == User.ChannelId).Select(p => p.ProductId).ToList();
        //                    }
        //                    break;
        //                //case "Stock":
        //                //    var branchesId = int.Parse(item.Value2.ToString());
        //                //    if (branchesId > 0)
        //                //    {
        //                //        if (item.Value == "Equals")
        //                //        {


        //                //            if (key == 0)
        //                //            {
        //                //                var soft_StockNull = _context.soft_Branches_Product_Stock.Where(o => o.BranchesId == User.BranchesId).Select(o => o.ProductId);

        //                //                lstTmp = lstTmp.Where(o => soft_Stock.Contains(o.id) || !soft_StockNull.Contains(o.id));
        //                //            }
        //                //            else
        //                //            {
        //                //                ids = unitOfWork.BrachesStockRepository.FindBy(o => o.Stock_Total == key
        //                //                        && o.BranchesId == branchesId).Select(p => p.ProductId).ToList();
        //                //            }

        //                //        }
        //                //        if (item.Value == "LessThan")
        //                //        {
        //                //            ids = unitOfWork.BrachesStockRepository.FindBy(o => o.Stock_Total < key
        //                //                       && o.BranchesId == branchesId).Select(p => p.ProductId).ToList();
        //                //        }
        //                //        if (item.Value == "MoreThan")
        //                //        {
        //                //            ids = unitOfWork.BrachesStockRepository.FindBy(o => o.Stock_Total > key
        //                //                       && o.BranchesId == branchesId).Select(p => p.ProductId).ToList();
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        var sum_Stock = unitOfWork.BrachesStockRepository.FindBy(o => o.Stock_Total > key
        //                //                       && o.BranchesId == branchesId).Select(p => p.ProductId).ToList();

        //                //        var sum_Stock = _context.soft_Branches_Product_Stock.GroupBy(o => o.ProductId).Select(o => new
        //                //        {
        //                //            productid = o.Key,
        //                //            total = o.Sum(i => i.Stock_Total)
        //                //        });

        //                //        if (item.Value == "MoreThan")
        //                //        {
        //                //            var MoreThan = sum_Stock.Where(o => o.total > key).Select(o => o.productid);

        //                //            lstTmp = lstTmp.Where(o => MoreThan.Contains(o.id));
        //                //        }
        //                //        if (item.Value == "LessThan")
        //                //        {
        //                //            var LessThan = sum_Stock.Where(o => o.total < key).Select(o => o.productid);

        //                //            lstTmp = lstTmp.Where(o => LessThan.Contains(o.id));
        //                //        }
        //                //        if (item.Value == "Equals")
        //                //        {
        //                //            var Equals = sum_Stock.Where(o => o.total == key).Select(o => o.productid);

        //                //            lstTmp = lstTmp.Where(o => Equals.Contains(o.id));
        //                //        }
        //                //    }
        //                //    break;
        //            }

        //            item.Ids = ids.Cast<long>().ToList();
        //        }
        //    }

        //    var value = new Dictionary<string, object>();
        //    var products = unitOfWork.ProductRepositoryV2.SearchBy1(pageinfo, out count, out min, out value, User);

        //    var result= new List<Prodcut_Branches_PriceChannel>();

        //    foreach (var item in products)
        //    {
        //        var stocks = Mapper.Map<List<Product_StockModel>>(item.soft_Branches_Product_Stock.ToList());
        //        var stock = stocks.FirstOrDefault(o => o.BranchesId == User.BranchesId);


        //        //if (stock != null)
        //        //    item.Stock_Total = stock.Stock_Total;

        //        //item.Stock_Sum = stocks.Sum(o => o.Stock_Total);

        //        //var productInfo = new Prodcut_Branches_PriceChannel
        //        //{
        //        //    //product_price = price ?? price,
        //        //    product_stock = stock ?? stock,
        //        //    product = item,
        //        //    product_stocks = stocks
        //        //};

        //        //var prices = _context.soft_Channel_Product_Price.Where(o => o.ProductId == item.id).ToList();

        //        //if (prices != null && prices.Count > 0)
        //        //{
        //        //    var PriceMainStore = prices.FirstOrDefault(o => o.soft_Channel.Type == (int)TypeChannel.IsMainStore);
        //        //    if (PriceMainStore != null)
        //        //        item.PriceMainStore = PriceMainStore.Price;

        //        //    var PriceSi = prices.FirstOrDefault(o => o.soft_Channel.Type == (int)TypeChannel.IsChannelWholesale);
        //        //    if (PriceSi != null)
        //        //        item.PriceWholesale = PriceSi.Price;


        //        //    var price = Mapper.Map<Product_PriceModel>(prices.FirstOrDefault(o => o.ChannelId == User.ChannelId));
        //        //    productInfo.product_price = price;
        //        //}

        //        //lstInfo.listTable.Add(productInfo);

        //        return null;
        //    }

        //}

    }
}
