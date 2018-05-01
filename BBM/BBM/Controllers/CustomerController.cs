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
    public class CustomerController : BaseController
    {
        private ICustomerBusiness _customerBus;
        private IUnitOfWork _unitOfWork;
        public CustomerController(IApiBusiness _apiBus, ICustomerBusiness customerBus, IUnitOfWork unitOfWork)
        {
            _customerBus = customerBus;
            _unitOfWork = unitOfWork;
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
        public async Task<JsonResult> UpdateCustomer(CustomerModel model)
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

                Messaging.isError = !await _customerBus.UpdateCustomer(model);

                Messaging.messaging = !Messaging.isError ? "Cập nhật khách hàng thành công!" : "Cập nhật khách hàng không thành công!";
            }
            catch(Exception ex)
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
                var customer = _unitOfWork.CutomerRepository.FindBy(o => o.dienthoai.Contains(keyword)).FirstOrDefault();

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
