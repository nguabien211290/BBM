using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class DisscountModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public bool IsNotExp { get; set; }
        public int Type { get; set; }
        public int Total { get; set; }
        public int Value { get; set; }
        public bool Disable { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public int EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}