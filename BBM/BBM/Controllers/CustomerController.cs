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
    public class CustomerController : BaseController
    {

        private CRUD _crud;
        private admin_softbbmEntities _context;
        private IApiBusiness apiBus;
        private ICustomerBusiness _customerBus;
        public CustomerController(IApiBusiness _apiBus, ICustomerBusiness customerBus)
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
            _customerBus = customerBus;
            apiBus = _apiBus;
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

                Channel_Paging<CustomerModel> lstInfo = new Channel_Paging<CustomerModel>();

                int count, min = 0;

                var rs = _customerBus.GetCustomer(pageinfo, out count, out min);

                lstInfo.startItem = min;

                lstInfo.totalItems = count;

                lstInfo.listTable = rs;

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

        public async Task<JsonResult> SyncCustomer()
        {
            await apiBus.SyncCustomer();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
