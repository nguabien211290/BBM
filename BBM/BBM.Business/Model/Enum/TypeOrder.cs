using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Enum
{
    public enum TypeOrder
    {
        [Display(Name = "Input")]
        Input = 1,
        [Display(Name = "Output")]
        Output = 2,
        [Display(Name = "OrderProduct")]
        OrderProduct = 3,
        [Display(Name = "Sale")]
        Sale = 4,
        [Display(Name = "Switch")]
        Switch = 5,
        [Display(Name = "OrderBranches")]
        OrderBranches = 6
    }
    public enum TypePaymentMethods
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
    public enum TimeShiping
    {
        [Display(Name = "08h – 17h giờ hành chánh")]
        HCM08h17h = 1,
        [Display(Name = "17h – 22h ngoài giờ hành chánh")]
        HCM17h22h = 2,
        [Display(Name = "Ngày chủ nhật")]
        HCMSunday = 3,
        [Display(Name = "Bất kỳ giờ nào trong ngày")]
        HCMAnytime = 4,
        [Display(Name = "Nhanh, chỉ trong 3 tiếng")]
        HCMFast3hours = 5

    }
}