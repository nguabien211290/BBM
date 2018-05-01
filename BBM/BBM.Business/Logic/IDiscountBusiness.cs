using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IDiscountBusiness
    {
        List<DisscountModel> GetDisscount(PagingInfo pageinfo, out int count, out int min);
        Task<bool> DisableDiscount(DisscountModel model, int UserId);
        Task<bool> CreateDiscount(DisscountModel model, int UserId);
    }
}
