using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Model.Enum
{
    public enum TypeFromCreate
    {
        [Display(Name = "Website")]
        Website = 1,
        [Display(Name = "Soft")]
        Soft = 2
    }
}
