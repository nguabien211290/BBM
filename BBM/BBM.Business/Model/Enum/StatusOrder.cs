using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Enum
{
    public enum StatusOrder_Branches
    {
        [Display(Name = "Đã xuất", ShortName = "Exported")]
        Exported = 1,
    }
    public enum StatusOrder_Switch
    {
        [Display(Name = "Đang xử lý", ShortName = "Process")]
        Process = 1,
        [Display(Name = "Hoàn thành", ShortName = "Done")]
        Done = 2
    }
    public enum StatusOrder_Output
    {
        [Display(Name = "Đang xử lý", ShortName = "Process")]
        Process = 1,
        [Display(Name = "Hoàn thành", ShortName = "Done")]
        Done = 2
    }
    public enum StatusOrder_Input
    {
        [Display(Name = "Đang xử lý", ShortName = "Process")]
        Process = 1,
        [Display(Name = "Hoàn thành", ShortName = "Done")]
        Done = 2
    }
    public enum StatusOrder_Suppliers
    {
        [Display(Name = "Đang xử lý", ShortName = "Process")]
        Process = 1,
        [Display(Name = "Hoàn thành", ShortName = "Done")]
        Done = 2
    }
    public enum StatusOrder_Sale
    {
        [Display(Name = "Chờ xử lý", ShortName = "Process")]
        Process = 1,
        [Display(Name = "Đang giao hàng", ShortName = "Shipped")]
        Shipped = 2,
        [Display(Name = "Hoàn thành", ShortName = "Done")]
        Done = 3,
        [Display(Name = "Hủy", ShortName = "Cancel")]
        Cancel = 4,
        [Display(Name = "Hoãn", ShortName = "Refund")]
        Refund = 5,
        [Display(Name = "Giao hàng thất bại", ShortName = "ShipCancel")]
        ShipCancel = 6            
    }
}