using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using BBM.Business.Models.View;
using BBM.Business.Models.Enum;
using BBM.Infractstructure.Security;
using BBM.Business.Logic;
using System.Threading.Tasks;

namespace BBM.Controllers
{
    public class SuppliersController : BaseController
    {
        private ISuppliersBusiness suppliersBus;
        public SuppliersController(ISuppliersBusiness _suppliersBus)
        {
            suppliersBus = _suppliersBus;
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Suppliers })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Suppliers/Suppliers.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Suppliers })]
        public JsonResult GetSuppliersby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging<Channel_Paging<SuppliersModel>>();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Channel_Paging<SuppliersModel> lstInfo = new Channel_Paging<SuppliersModel>();

                int count, min = 0;

                var rs = suppliersBus.GetSuppliers(pageinfo, out count, out min);

                lstInfo.startItem = min;

                lstInfo.totalItems = count;

                lstInfo.listTable = rs;

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách nhà phân phối không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Suppliers })]
        [HttpPost]
        public async Task<JsonResult> UpdateSuppliers(SuppliersModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.Name)
                    || string.IsNullOrEmpty(model.Phone)
                    || string.IsNullOrEmpty(model.Address)
                    || string.IsNullOrEmpty(model.Email))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ vui lòng thử lại.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await suppliersBus.UpdateSuppliers(model, User.UserId);

                Messaging.messaging = !Messaging.isError ? "Cập nhật nhà phân phối thành công!" : "Cập nhật nhà phân phối không thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật nhà phân phối không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Delete_Suppliers })]
        [HttpPost]
        public async Task<JsonResult> DeleteSuppliers(int id)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                if (id <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ vui lòng thử lại.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await suppliersBus.DeleteSuppliers(id);

                Messaging.messaging = !Messaging.isError ? "Xóa nhà phân phối thành công!" : "Xóa nhà phân phối không thành công, vui lòng thử lại sau!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Nhà phân phối này đang sử dụng không xóa được, vui lòng thử lại!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}