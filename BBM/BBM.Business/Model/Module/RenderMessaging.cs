using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class RenderMessaging
    {
        public bool isError { get; set; }
        public string messaging { get; set; }
        public Object Data { get; set; }
     
    }
    public class RenderMessaging<T>
    {
        public bool isError { get; set; }
        public string messaging { get; set; }
        public T Data { get; set; }

    }
}