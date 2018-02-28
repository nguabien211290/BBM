using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Enum
{

    public enum TypeChannel
    {
        [Display(Name = "Kênh bán hàng")]
        IsChannelChannel = 0,
        [Display(Name = "Kênh cửa hàng")]
        IsMainStore = 1,
        [Display(Name = "Kênh online")]
        IsChannelOnline = 2,
        [Display(Name = "Kênh sỉ")]
        IsChannelWholesale = 3,
        [Display(Name = "ONLINE")]
        IsOnline = 99
    }
}