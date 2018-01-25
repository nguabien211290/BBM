using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class ChannelModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Channel { get; set; }
        public Nullable<int> BranchesId { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
    public class Channel_Product_PriceModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ChannelId { get; set; }
        public string Channel { get; set; }
        public int Total { get; set; }
        public int PriceChange { get; set; }
        public int Price { get; set; }
        public string Note { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
        public ProductSampleModel shop_sanpham { get; set; }

    }
    public class Channel_Paging<T> where T : class
    {
        public List<Channel_Product_PriceModel> channelproduct { get; set; }
        public List<ProductSampleModel> product { get; set; }
        public List<T> listTable { get; set; }
        public int totalItems { get; set; }
        public int startItem { get; set; }
    }
}