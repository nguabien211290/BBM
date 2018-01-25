using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public int Ref { get; set; }
        public string Href { get; set; }
        public int Branch { get; set; }
        public int Channel { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public bool IsReview { get; set; }
        public DateTime DateCreate { get; set; }
    }
}