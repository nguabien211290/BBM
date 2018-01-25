using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.View
{
    public class PagingInfo
    {
        public string keyword { get; set; }
        public int pageindex { get; set; }
        public int pagesize { get; set; }
        public string sortby { get; set; }
        public bool sortbydesc { get; set; }
        public List<FilterModel> filterby { get; set; }
    }
    public class FilterModel
    {
        public string Fiter { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string[] Values { get; set; }
    }
}