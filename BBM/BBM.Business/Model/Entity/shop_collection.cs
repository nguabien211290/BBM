//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BBM.Business.Model.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class shop_collection
    {
        public int id { get; set; }
        public Nullable<int> idloai { get; set; }
        public Nullable<int> iddm { get; set; }
        public Nullable<int> iddmc { get; set; }
        public Nullable<int> idsp { get; set; }
        public string lable { get; set; }
    
        public virtual shop_danhmuccon shop_danhmuccon { get; set; }
        public virtual shop_sanpham shop_sanpham { get; set; }
    }
}
