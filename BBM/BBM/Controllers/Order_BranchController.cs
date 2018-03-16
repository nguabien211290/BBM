using AutoMapper;
using BBM.Business.Logic;
using BBM.Business.Model.Module;
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
    public class Order_BranchController : BaseController
    {
        private IOrderBusiness _IOrderBus;
        private IUnitOfWork _unitOW;
        public Order_BranchController(IOrderBusiness IOrderBus, IUnitOfWork unitOW)
        {
            _IOrderBus = IOrderBus;
            _unitOW = unitOW;

        }
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Branches })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/OrderBranch/_Branch_Order_Output_List.cshtml");
        }


        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Branches })]
        public ActionResult RenderViewCreate()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/OrderBranch/_Branch_Order_Output_Create.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Branches })]
        public JsonResult GetOrder_Branches(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Channel_Paging<OrderModel> lstInfo = new Channel_Paging<OrderModel>();

                int count, min = 0;

                lstInfo.listTable = _IOrderBus.GetOrder_Branches(pageinfo, User.BranchesId, out count, out min);

                lstInfo.totalItems = count;

                lstInfo.startItem = min;

                var sys_Employee = _unitOW.EmployeeRepository.GetAll().ToList();

                var soft_Branches = _unitOW.BrachesRepository.GetAll().ToList();

                foreach (var item in lstInfo.listTable)
                {
                    var userCreate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeCreate);
                    item.EmployeeNameCreate = userCreate.Name + "(" + userCreate.Email + ")";

                    if (item.EmployeeUpdate.HasValue)
                    {
                        var userUpdate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeUpdate);
                        item.EmployeeNameUpdate = userUpdate.Name + "(" + userUpdate.Email + ")";
                    }

                    if (item.Id_From > 0)
                    {
                        var branch = soft_Branches.FirstOrDefault(o => o.BranchesId == item.Id_From);
                        if (branch != null)
                            item.Name_From = branch.BranchesName;
                    }

                    if (item.Id_To > 0)
                    {
                        var branch = soft_Branches.FirstOrDefault(o => o.BranchesId == item.Id_To);
                        if (branch != null)
                            item.Name_To = branch.BranchesName;
                    }
                }

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị đơn hàng nhập không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Input })]
        public async Task<JsonResult> CreateOrder_Branches(OrderModel model)
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

                if (model.Id_To <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng chọn Kho bạn xuất đến.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                
                if (model.Id_To == User.BranchesId)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Kho đặt hàng không hơp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (model.Detail.Count <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var user = Mapper.Map<UserCurrent>(User);

                var isError = await _IOrderBus.AddOrder_Branches(model, user);

                Messaging.isError = !isError;
                Messaging.messaging = "Đã đặt hàng nội bộ thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Đặt hàng nội bộ không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }


    }
}