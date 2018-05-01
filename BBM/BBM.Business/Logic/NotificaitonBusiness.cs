using AutoMapper;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public partial class NotificaitonBusiness : INotificaitonBusiness
    {
        private IUnitOfWork _unitOW;

        public NotificaitonBusiness(
            IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        public List<NotificationModel> LoadNotification(int BranchesId)
        {
            var notis = _unitOW.NotificationRepository.FindBy(o =>
                                         o.IsReview == false && o.Branch.HasValue && o.Branch.Value == BranchesId).OrderByDescending(o => o.DateCreate).ToList();

            var notificationNotReview = Mapper.Map<List<NotificationModel>>(notis);

            if (notificationNotReview.Count <= 10)
            {
                var notisIsReview = _unitOW.NotificationRepository.FindBy(o =>
                                              o.IsReview == true && o.Branch.HasValue && o.Branch.Value == BranchesId).OrderByDescending(o => o.DateCreate).Take(10).ToList();

                var notificationIsReview = Mapper.Map<List<NotificationModel>>(notisIsReview);

                var rs = notificationNotReview.Concat(notificationIsReview);

                return rs.ToList();
            }

            return notificationNotReview;
        }

        public async Task<bool> IsReview(int BranchesId, int UserId)
        {
            var notification = _unitOW.NotificationRepository.FindBy(o => o.IsReview == false && (o.Branch == BranchesId || o.UserId == UserId)).ToList();

            bool isUpdate = false;

            foreach (var item in notification)
            {
                item.IsReview = true;

                isUpdate = true;

                _unitOW.NotificationRepository.Update(item, o => o.IsReview);
            }

            if (isUpdate)
                await _unitOW.SaveChanges();

            return true;
        }
    }
}
