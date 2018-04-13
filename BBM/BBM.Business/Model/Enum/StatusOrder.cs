using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Enum
{
    public enum StatusProductsSwitch
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

    public enum PTTT
    {
        [Display(Name = "Tiền mặt")]
        Cash = 1,
        [Display(Name = "Chuyển khoản")]
        BankTransfer = 2,
        [Display(Name = "Tiền mặt tỉnh")]
        CashOutHCM = 3,
        [Display(Name = "Thanh toán trực tuyến")]
        OnlinePayment = 4,
        [Display(Name = "Bằng thẻ ngân hàng khi nhận hàng")]
        BankCardOnDelivery = 5
    }
}