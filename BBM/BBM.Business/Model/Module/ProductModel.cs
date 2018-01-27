using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{

    public class ProductSampleModel
    {
        public int id { get; set; }
        public int SuppliersId { get; set; }
        public int UnitId { get; set; }
        public int CatalogId { get; set; }
        public string Barcode { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
        public string Detail_Info { get; set; }
        public bool Stop_Sale { get; set; }

        public string SuppliersName { get; set; }
        public double Stock_Total { get; set; }
        public double Stock_Sum { get; set; }

        public int PriceBase { get; set; }
        public int PriceBase_Old { get; set; }
        public int PriceInput { get; set; }
        public int PriceCompare { get; set; }
        public int PriceChannel { get; set; }
        public int PriceWholesale { get; set; }

        public int PriceMainStore { get; set; }
        public bool HasInChannel { get; set; }
        public SuppliersModel soft_Suppliers { get; set; }
        public string Img { get; set; }

        public int StatusVAT { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public DateTime? DateCreate { get; set; }

        public List<Order_PriceChannelsModel> PriceChannels { get; set; }
    }

    public class Prodcut_Branches_PriceChannel
    {
        public Product_PriceModel product_price { get; set; }
        public List<Product_StockModel> product_stocks { get; set; }
        public Product_StockModel product_stock { get; set; }
        public ProductSampleModel product { get; set; }
        public int orderedSaleAdvbyChannel { get; set; }
    }

    public class Product_PriceModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ChannelId { get; set; }
        public int Price { get; set; }
        public int PriceChange { get; set; }

        public int Price_Discount { get; set; }
        public DateTime StartDate_Discount { get; set; }
        public DateTime Enddate_Discount { get; set; }

        public string Note { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }

        public List<ChannelModel> Channels { get; set; }
    }

    public class Product_StockModel
    {
        public int BranchesId { get; set; }
        public string BranchesName { get; set; }
        public int ProductId { get; set; }
        public int Stock_Total { get; set; }
        public string Note { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public int EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
    public class Product_Sale_Average
    {
        public int ProductId { get; set; }
        public int Sale_Average { get; set; }
    }
}