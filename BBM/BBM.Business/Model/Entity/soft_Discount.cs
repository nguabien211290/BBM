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
    
    public partial class soft_Discount
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public System.DateTime Startdate { get; set; }
        public Nullable<System.DateTime> Enddate { get; set; }
        public bool IsNotExp { get; set; }
        public Nullable<int> Total { get; set; }
        public int Type { get; set; }
        public int Value { get; set; }
        public bool Disable { get; set; }
        public int EmployeeCreate { get; set; }
        public System.DateTime DateCreate { get; set; }
        public Nullable<int> EmployeeUpdate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
    }
}