using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Logic;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class NotificaitonController : BaseController
    {
        private INotificaitonBusiness INotificaitonBus;
        public NotificaitonController(INotificaitonBusiness _INotificaitonBusiness)
        {
            INotificaitonBus = _INotificaitonBusiness;
        }
        [HttpGet]
        public JsonResult LoadNotification()
        {
            var rs = INotificaitonBus.LoadNotification(User.BranchesId);

            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> IsReview()
        {
            var rs = await INotificaitonBus.IsReview(User.BranchesId, User.UserId);

            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}
