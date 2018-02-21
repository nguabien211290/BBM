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
}