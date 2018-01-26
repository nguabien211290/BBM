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
    public class CatalogController : BaseController
    {
        //
        // GET: /NPP/

        private CRUD _crud;
        private admin_softbbmEntities _context;
        public CatalogController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Catalog })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Catalog/Catalog.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Catalog })]
        public JsonResult GetCatalogby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var lstTmp = from supplier in _context.soft_Catalog select supplier;

                #region Sort
                if (!string.IsNullOrEmpty(pageinfo.sortby))
                {
                    switch (pageinfo.sortby)
                    {
                        case "Name":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Name);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Name);
                            break;
                    }

                }
                #endregion

                var catalogs = Mapper.Map<List<CatalogModel>>(lstTmp.ToList());
                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    var lstcustomer = lstTmp.ToList();
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    catalogs = catalogs.Where(o =>
                   (!string.IsNullOrEmpty(o.Name) && Helpers.convertToUnSign3(o.Name.ToLower()).Contains(pageinfo.keyword))).ToList();

                }
                #endregion
                Channel_Paging<CatalogModel> lstInfo = new Channel_Paging<CatalogModel>();
                if (catalogs != null && catalogs.Count > 0)
                {
                    int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                    lstInfo.totalItems = catalogs.Count();
                    int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                    if (pageinfo.pagesize < catalogs.Count)
                        if (quantity > 0)
                            catalogs = catalogs.GetRange(min, quantity);
                    lstInfo.startItem = min;
                    lstInfo.listTable = catalogs;
                }

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
        public JsonResult UpdateCatalog(CatalogModel model)
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

                var objCatalog = Mapper.Map<soft_Catalog>(model);

                if (model.Id <= 0)
                {

                    objCatalog.DateCreate = DateTime.Now;
                    objCatalog.EmployeeCreate = User.UserId;
                    objCatalog.DateUpdate = null;
                    _crud.Add<soft_Catalog>(objCatalog);
                }
                else
                {
                    objCatalog.DateUpdate = DateTime.Now;
                    objCatalog.EmployeeUpdate = User.UserId;
                    _crud.Update<soft_Catalog>(objCatalog, o => o.Name, o => o.EmployeeUpdate, o => o.DateUpdate);
                }

                _crud.SaveChanges();
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
        public JsonResult DeleteCatalog(int id)
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
                var catalog = _context.soft_Catalog.Find(id);

                if (catalog == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Nhóm hàng này đã được xóa.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                _context.soft_Catalog.Remove(catalog);

                _context.SaveChanges();

                Messaging.messaging = "Xóa Nhóm hàng thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Nhóm hàng này đang sử dụng không xóa được, vui lòng thử lại!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLstCatalog()
        {
            var lstTmp = Mapper.Map<List<CatalogModel>>(_context.soft_Catalog.OrderBy(o => o.Name).ToList());
            return Json(lstTmp, JsonRequestBehavior.AllowGet);
        }
    }
}

