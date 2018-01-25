using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class CatalogModel
    {
        public int Id { get; set; }
        public int RefId { get; set; }
        public List<CatalogModel> catalogChilds { get; set; }
        public string Name { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public int EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}