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
    
    public partial class admin_role
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public int Role { get; set; }
        public string Note { get; set; }
    }
}