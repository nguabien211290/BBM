using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Logic;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Business.Repository;
using BBM.Infractstructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class CatalogController : BaseController
    {
        //
        // GET: /NPP/
        

        private ICatalogBusiness catalogBus;
        public CatalogController(ICatalogBusiness _catalogBus, IUnitOfWork _unitOfWork)
        {
            catalogBus = _catalogBus;
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Catalog })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Catalog/Catalog.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Catalog })]
        public JsonResult GetCatalogby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging<Channel_Paging<CatalogModel>>();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Channel_Paging<CatalogModel> lstInfo = new Channel_Paging<CatalogModel>();

                int count, min = 0;

                var rs = catalogBus.GetCatalog(pageinfo, out count, out min);

                lstInfo.startItem = min;

                lstInfo.totalItems = count;

                lstInfo.listTable = rs;

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị Nhóm sản phẩm có lỗi!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Catalog })]
        [HttpPost]
        public async Task<JsonResult> UpdateCatalog(CatalogModel model)
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
                if (string.IsNullOrEmpty(model.Name))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Tên nhóm không được trống.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (model.Id <= 0)
                    await catalogBus.CreateCatalog(model, User.UserId);
                else
                    await catalogBus.Updateatalog(model, User.UserId);

                Messaging.messaging = "Cập nhật nhóm thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật nhóm không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Remove_Catalog })]
        [HttpPost]
        public async Task<JsonResult> DeleteCatalog(int id)
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

                Messaging.isError = !await catalogBus.DeleteCatalog(id);

                Messaging.messaging = !Messaging.isError ? "Xóa Nhóm hàng thành công!" : "Xóa nhóm hàng không thành công, vui lòng thử lại!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Nhóm hàng này đang sử dụng không xóa được, vui lòng thử lại!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }      
    }
}

