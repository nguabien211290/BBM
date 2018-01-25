using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Enum
{
    public enum StatusProduct
    {
        [Display(Name = "Hoạt động")]
        Work = 1,
        [Display(Name = "Tạm hết hàng")]
        Sold_out = 2,
        [Display(Name = "Không sản xuất nữa")]
        Not_Manufacturing = 3,
        [Display(Name = "Không nhập nữa")]
        Not_Input = 4,
        [Display(Name = "Khác")]
        Orther = 5
    }
    public enum StatusVATProduct
    {
        [Display(Name = "Chọn VAT")]
        NONE = 0,
        [Display(Name = "Có VAT")]
        VAT = 1,
        [Display(Name = "Không VAT")]
        NoneVAT = 2,
        [Display(Name = "Có nhưng tốn")]
        VATBUS = 3,
        [Display(Name = "Xuất thỉnh thoảng")]
        Sometime = 4
    }
    
}