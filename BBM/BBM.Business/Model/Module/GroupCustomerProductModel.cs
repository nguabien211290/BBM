using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class GroupCustomerProductModel
    {
        public int Id { get; set; }
        public int Group_customerId { get; set; }
        public int ProductId { get; set; }
        public int Gia { get; set; }
        public DateTime Date { get; set; }
        public int Chietkhau { get; set; }
    }
    public class GroupCus_ProductViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> BanleCH { get; set; }
        public Nullable<int> Online { get; set; }
        public Nullable<int> Si { get; set; }
        public Nullable<int> Lazada { get; set; }
        public Nullable<int> Shopee { get; set; }
        public Nullable<int> Mymall { get; set; }       
        public Nullable<System.DateTime> Date_Onl { get; set; }
        public Nullable<int> Chietkhau_Onl { get; set; }
        public Nullable<System.DateTime> Date_CH { get; set; }
        public Nullable<int> Chietkhau_CH { get; set; }
        public string masp { get; set; }
        public string tensp { get; set; }
        public Nullable<int> soft_IdGroupProduct { get; set; }
        public Nullable<int> soft_IdNPP { get; set; }
        public Nullable<int> soft_IdKho { get; set; }
        public Nullable<int> soft_IdDvt { get; set; }
        public string soft_Barcode { get; set; }
        public Nullable<int> soft_GiaTC { get; set; }
        public Nullable<int> soft_GiaM { get; set; }

    }
}