using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Infractstructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public DiscountController()
        {
            _crud = new CRUD();
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Disscount })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Discount/Discount.cshtml");
        }

        public JsonResult GetDiscountby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var lstTmp = from disc in _context.soft_Discount orderby disc.Id descending select disc;

                var discounts = Mapper.Map<List<DisscountModel>>(lstTmp.ToList());

                Channel_Paging<DisscountModel> lstInfo = new Channel_Paging<DisscountModel>();
                if (discounts != null && discounts.Count > 0)
                {
                    int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                    lstInfo.totalItems = discounts.Count();
                    int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                    if (pageinfo.pagesize < discounts.Count)
                        if (quantity > 0)
                            discounts = discounts.GetRange(min, quantity);
                    lstInfo.listTable = discounts;
                    lstInfo.startItem = min;
                }

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
        public JsonResult DisableDiscount(DisscountModel model)
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
                var discount = _context.soft_Discount.Find(model.Id);
                discount.Disable = true;
                discount.DateUpdate = DateTime.Now;
                discount.EmployeeUpdate = User.UserId;


                _crud.Update<soft_Discount>(discount, o => o.Disable,
                    o => o.DateUpdate, o => o.EmployeeUpdate);

                _crud.SaveChanges();

                Messaging.isError = false;
                Messaging.messaging = "Cập nhật khuyễn mãi thành công!";
                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách khuyến mãi không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Disscount })]
        [HttpPost]
        public JsonResult CreateDiscount(DisscountModel model)
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

                var date = DateTime.Now;


                var discount = Mapper.Map<soft_Discount>(model);

                discount.Startdate = new DateTime(discount.Startdate.Year, discount.Startdate.Month, discount.Startdate.Day, 0, 0, 0, 0);

                if (discount.Enddate.HasValue)
                    discount.Enddate = new DateTime(discount.Enddate.Value.Year, discount.Enddate.Value.Month, discount.Enddate.Value.Day, 23, 59, 59, 999);

                var lstDisscount = _context.soft_Discount.Select(o => o.Code).ToList();
                discount.Code = Helpers.GenerateToken(10, lstDisscount);
                discount.EmployeeCreate = User.UserId;
                discount.DateCreate = DateTime.Now;
                discount.DateUpdate = null;
                if (discount.IsNotExp)
                    discount.Enddate = null;
                _crud.Add<soft_Discount>(discount);
                _crud.SaveChanges();

                Messaging.isError = false;
                Messaging.messaging = "Thêm khuyến mãi thành công!";
                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo khuyến mãi không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}
