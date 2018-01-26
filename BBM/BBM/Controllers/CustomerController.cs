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
    public class CustomerController : BaseController
    {

        private CRUD _crud;
        private admin_softbbmEntities _context;
        public CustomerController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Customer })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Customer/Customer.cshtml");
        }

        public JsonResult GetCustomerby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var lstTmp = from customers in _context.khachhangs orderby customers.MaKH descending select customers;

                #region Sort
                if (!string.IsNullOrEmpty(pageinfo.sortby))
                {
                    switch (pageinfo.sortby)
                    {
                        case "Id":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.MaKH);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.MaKH);
                            break;
                        case "User":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.tendn);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.tendn);
                            break;
                        case "Name":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.hoten);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.hoten);
                            break;
                        case "Email":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.email);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.email);
                            break;
                        case "Phone":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.dienthoai);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.dienthoai);
                            break;
                        case "DistrictId":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.idtp);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.idtp);
                            break;
                        case "ProvinceId":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.idquan);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.idquan);
                            break;
                    }

                }
                #endregion

                var customer = Mapper.Map<List<CustomerModel>>(lstTmp.ToList());
                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    var lstcustomer = lstTmp.ToList();
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    customer = customer.Where(o =>
                   (!string.IsNullOrEmpty(o.Code) && Helpers.convertToUnSign3(o.Code.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Name) && Helpers.convertToUnSign3(o.Name.ToLower()).Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.User) && Helpers.convertToUnSign3(o.User.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Phone) && Helpers.convertToUnSign3(o.Phone.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Email) && Helpers.convertToUnSign3(o.Email.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Address) && Helpers.convertToUnSign3(o.Address.ToLower()).Contains(pageinfo.keyword))).ToList();

                }
                #endregion
                Channel_Paging<CustomerModel> lstInfo = new Channel_Paging<CustomerModel>();
                if (customer != null && customer.Count > 0)
                {
                    int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                    lstInfo.totalItems = customer.Count();
                    int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                    if (pageinfo.pagesize < customer.Count)
                        if (quantity > 0)
                            customer = customer.GetRange(min, quantity);
                    lstInfo.startItem = min;
                    lstInfo.listTable = customer;

                    foreach (var item in lstInfo.listTable)
                    {
                        var orders = Mapper.Map<List<OrderModel>>(_context.soft_Order.Where(o => o.TypeOrder == (int)TypeOrder.Sale && o.Id_To == item.Id).ToList());
                        item.Orders = orders;
                    }
                }

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách khách hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Customer, RolesEnum.Create_Customer })]
        [HttpPost]
        public JsonResult UpdateCustomer(CustomerModel model)
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
                    || string.IsNullOrEmpty(model.Email)
                    || string.IsNullOrEmpty(model.User))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ vui lòng thử lại.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var objCustomer = Mapper.Map<khachhang>(model);
                if (model.Id <= 0)
                {
                    objCustomer.matkhau = Helpers.GenerateToken(10);
                    _crud.Add<khachhang>(objCustomer);
                }
                else
                {
                    _crud.Update<khachhang>(objCustomer);
                }

                _crud.SaveChanges();
                Messaging.messaging = "Cập nhật khách hàng thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật khách hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Research(string keyword)
        {
            var Messaging = new RenderMessaging();
            try
            {
                var customer = _context.khachhangs.FirstOrDefault(o => o.dienthoai.Contains(keyword));


                if (customer != null)
                    Messaging.Data = Mapper.Map<CustomerModel>(customer);
                else
                    Messaging.Data = null;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tìm kiếm khách hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

    }
}
