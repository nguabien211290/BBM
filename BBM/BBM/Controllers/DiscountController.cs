using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Logic;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Infractstructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class DiscountController : BaseController
    {
        //
        // GET: /Discount/
        private CRUD _crud;
        private admin_softbbmEntities _context = new admin_softbbmEntities();

        private IDiscountBusiness _IDiscountBusiness;
        public DiscountController(IDiscountBusiness IDiscountBusiness)
        {
            _IDiscountBusiness = IDiscountBusiness;
            _crud = new CRUD();
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Disscount })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Discount/Discount.cshtml");
        }

        public JsonResult GetDiscountby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging<Channel_Paging<DisscountModel>>();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Channel_Paging<DisscountModel> lstInfo = new Channel_Paging<DisscountModel>();

                int count, min = 0;

                var rs = _IDiscountBusiness.GetDisscount(pageinfo, out count, out min);

                lstInfo.startItem = min;

                lstInfo.totalItems = count;

                lstInfo.listTable = rs;

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách khuyến mãi không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Disscount })]
        [HttpPost]
        public async Task<JsonResult> DisableDiscount(DisscountModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.UserId < 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IDiscountBusiness.DisableDiscount(model, User.UserId);

                Messaging.messaging = !Messaging.isError ? "Tắt khuyễn mãi thành công!" : "Tắt khuyễn mãi không thành công!";

                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tắt khuyễn mãi không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Disscount })]
        [HttpPost]
        public async Task<JsonResult> CreateDiscount(DisscountModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.UserId < 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.Name) || model.Value <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                if (model.Startdate > model.Enddate)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Ngày khuyến mãi không hợp lệ.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IDiscountBusiness.CreateDiscount(model, User.UserId);

                Messaging.messaging = "Tạo khuyến mãi thành công!";

                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo khuyến mãi không thành công!";
            }

            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}
