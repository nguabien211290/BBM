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

namespace BBM.Controllers
{
    public class SuppliersController : BaseController
    {
        //
        // GET: /NPP/

        private CRUD _crud;
        private admin_softbbmEntities _context;
        public SuppliersController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Suppliers })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Suppliers/Suppliers.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Suppliers })]
        public JsonResult GetSuppliersby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại !";
                }

                var lstTmp = from supplier in _context.soft_Suppliers select supplier;

                #region Sort
                if (!string.IsNullOrEmpty(pageinfo.sortby))
                {
                    switch (pageinfo.sortby)
                    {
                        case "SuppliersId":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.SuppliersId);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.SuppliersId);
                            break;
                        case "Name":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Name);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Name);
                            break;
                        case "Email":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Email);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Email);
                            break;
                        case "Phone":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Phone);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Phone);
                            break;
                    }

                }
                #endregion

                var suppliers = Mapper.Map<List<SuppliersModel>>(lstTmp.ToList());
                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    var lstcustomer = lstTmp.ToList();
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    suppliers = suppliers.Where(o =>
                   (!string.IsNullOrEmpty(o.Name) && Helpers.convertToUnSign3(o.Name.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Phone) && Helpers.convertToUnSign3(o.Phone.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Email) && Helpers.convertToUnSign3(o.Email.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Address) && Helpers.convertToUnSign3(o.Address.ToLower()).Contains(pageinfo.keyword))).ToList();

                }
                #endregion
                Channel_Paging<SuppliersModel> lstInfo = new Channel_Paging<SuppliersModel>();
                if (suppliers != null && suppliers.Count > 0)
                {
                    int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                    lstInfo.totalItems = suppliers.Count();
                    int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                    if (pageinfo.pagesize < suppliers.Count)
                        if (quantity > 0)
                            suppliers = suppliers.GetRange(min, quantity);
                    lstInfo.startItem = min;
                    lstInfo.listTable = suppliers;

                    //foreach (var item in lstInfo.listTable)
                    //{
                    //    var orders = Mapper.Map<List<OrderModel>>(_context.soft_Order.Where(o => o.TypeOrder == (int)TypeOrder.OrderProduct && o.Id_To == item.SuppliersId).ToList());
                    //    item.Orders = orders;
                    //}
                }

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Do sự cố mạng, vui lòng thử lại !";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Suppliers })]
        [HttpPost]
        public JsonResult UpdateSuppliers(SuppliersModel model)
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

                var objSuppliers = Mapper.Map<soft_Suppliers>(model);

                if (model.SuppliersId <= 0)
                {
                    objSuppliers.DateCreate = DateTime.Now;
                    objSuppliers.EmployeeCreate = User.UserId;
                    objSuppliers.DateUpdate = null;
                    _crud.Add<soft_Suppliers>(objSuppliers);
                }
                else
                {
                    objSuppliers.DateUpdate = DateTime.Now;
                    objSuppliers.EmployeeUpdate = User.UserId;
                    _crud.Update<soft_Suppliers>(objSuppliers, o => o.Address, o => o.Email, o => o.Name, o => o.Phone, o => o.EmployeeUpdate, o => o.DateUpdate);
                }

                _crud.SaveChanges();
                Messaging.messaging = "Cập nhật thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Do sự cố mạng, vui lòng thử lại !";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLstSuppliers()
        {

            var lstTmp = Mapper.Map<List<SuppliersModel>>(_context.soft_Suppliers.OrderBy(o=>o.Name).ToList());
            return Json(lstTmp, JsonRequestBehavior.AllowGet);

        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Delete_Suppliers })]
        [HttpPost]
        public JsonResult DeleteSuppliers(int id)
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
                var Suppliers = _context.soft_Suppliers.Find(id);

                if (Suppliers == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Nhà phân phối này đã được xóa.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                _context.soft_Suppliers.Remove(Suppliers);

                _context.SaveChanges();

                Messaging.messaging = "Xóa nhầ phân phối thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Nhà phân phối này đang sử dụng không xóa được, vui lòng thử lại !";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }      
    }
}