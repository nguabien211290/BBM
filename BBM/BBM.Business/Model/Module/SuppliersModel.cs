using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{

    public class SuppliersModel
    {
        public int SuppliersId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Thue { get; set; }
        public string AccBank { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public int EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}