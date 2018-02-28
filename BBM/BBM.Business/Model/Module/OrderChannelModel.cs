using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class OrderModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public int TypeOrder { get; set; }
        public int Total { get; set; }
        public bool isChannelOnline { get; set; }
        public int TotalOrther { get; set; }
        public int Id_Customer { get; set; }
        public int Id_From { get; set; }
        public int Id_To { get; set; }

        public string DisscountCode { get; set; }
        public int DisscountValue { get; set; }

        public string Name_From { get; set; }
        public string Name_To { get; set; }

        public bool IsShip { get; set; }

        public int EmployeeShip { get; set; }
        public int EmployeeCreate { get; set; }
        public string EmployeeNameCreate { get; set; }
        public string EmployeeNameShip { get; set; }
        public string ChannelName { get; set; }
        public System.DateTime DateCreate { get; set; }

        public Nullable<int> EmployeeUpdate { get; set; }
        public string EmployeeNameUpdate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }

        public List<Order_DetialModel> Detail { get; set; }
        public CustomerModel Customer { get; set; }

        public string StatusPrint { get; set; }
    }
    public class Order_InputTmpModel
    {
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public double? Price { get; set; }
        public int Status { get; set; }
        public double? Total { get; set; }

        public string SuppliersName { get; set; }
        public int PriceBase { get; set; }
        public int PriceCompare { get; set; }
        public double Stock_Total { get; set; }
    }
    public class Order_DetialModel
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public double Total { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int Status { get; set; }
        public ProductSampleModel Product { get; set; }

        public int PriceCompare { get; set; }
        public List<Order_PriceChannelsModel> PriceChannels { get; set; }
    }

    public class Order_PriceChannelsModel
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string Channel { get; set; }

        public int Price_Discount { get; set; }
        public DateTime StartDate_Discount { get; set; }
        public DateTime Enddate_Discount { get; set; }
    }

    public class OrderChannel_GroupModel
    {
        public List<Order_DetialModel> products { get; set; }
        public SuppliersModel suppliers { get; set; }
    }

}