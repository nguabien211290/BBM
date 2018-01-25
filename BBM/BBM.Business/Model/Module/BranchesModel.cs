using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class BranchesModel
    {
        public int BranchesId { get; set; }
        public string Code { get; set; }
        public string BranchesName { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsPrimary { get; set; }
        public bool Disabled { get; set; }
        public List<ChannelModel> soft_Channel { get; set; }

    }
}