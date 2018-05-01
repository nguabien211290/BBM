using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface INotificaitonBusiness
    {
        List<NotificationModel> LoadNotification(int BranchesId);
        Task<bool> IsReview(int BranchesId, int UserId);
    }
}
