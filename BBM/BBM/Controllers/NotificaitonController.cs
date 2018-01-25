using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class NotificaitonController : BaseController
    {
        private CRUD _crud;
        private admin_softbbmEntities _context;
        public NotificaitonController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }

        public JsonResult LoadNotification()
        {
            var notificationnotReview = Mapper.Map<List<NotificationModel>>(_context.soft_Notification.Where(o =>
                                        o.IsReview == false && o.Branch.HasValue && o.Branch.Value == User.BranchesId).OrderByDescending(o => o.DateCreate).ToList());
            if (notificationnotReview.Count <= 10)
            {
                var notification = Mapper.Map<List<NotificationModel>>(_context.soft_Notification.Where(o =>
                o.IsReview == true && o.Branch.HasValue && o.Branch.Value == User.BranchesId).OrderByDescending(o => o.DateCreate).Take(10).ToList());
                var rss = notificationnotReview.Concat(notification);
                return Json(rss, JsonRequestBehavior.AllowGet);
            }
            return Json(notificationnotReview, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult IsReview()
        {
            var notification = _context.soft_Notification.Where(o => o.IsReview == false && (o.Branch==User.BranchesId || o.UserId == User.UserId)).ToList();
            bool isUpdate = false;
            foreach (var item in notification)
            {
                item.IsReview = true;
                isUpdate = true;
                _crud.Update<soft_Notification>(item, o => o.IsReview);
            }
            if (isUpdate)
                _crud.SaveChanges();
            
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProcessNotification()
        {
            return Json(JsonRequestBehavior.AllowGet);
        }
        private string _processOrderSupplier()
        {


            //var dates = from a in _context.soft_Channel_Product_SaleTotal
            //            group a by a.Channel_Product_Id into g
            //            select new { Date = g.Key, Count = g.Count(x => x.DateSale), od = g.Sum(x => x.Total_sale) };


            return string.Empty;
        }
    }
}
