using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Models.Enum;
using BBM.Infractstructure.Security;
using BBM.Business.Logic;
using System.Threading.Tasks;

namespace BBM.Controllers
{
    public class BranchesController : BaseController
    {
        //
        // GET: /Branches/
        private IBrachesBusiness _IBrachesBusiness;
        private IChannelBusiness _IChannelBusiness;
        public BranchesController(IBrachesBusiness IBrachesBusiness, IChannelBusiness IChannelBusiness)
        {
            _IBrachesBusiness = IBrachesBusiness;
            _IChannelBusiness = IChannelBusiness;
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Branches/Branches.cshtml");
        }

        #region Braches

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Branches })]
        public ActionResult GetBranches()
        {
            var Messaging = new RenderMessaging<List<BranchesModel>>();

            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Messaging.Data = _IBrachesBusiness.GetBranches();
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách Kho có lỗi!";
            }

            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Branches })]
        [HttpPost]
        public async Task<JsonResult> CreateBranches(BranchesModel model)
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
                if (string.IsNullOrEmpty(model.BranchesName))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Tên Kho không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IBrachesBusiness.CreateBranches(model, User.UserId);

                Messaging.messaging = !Messaging.isError ? "Tạo kho thành công!" : "Tạo kho không thành công!";

            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo kho không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Branches })]
        [HttpPost]
        public async Task<JsonResult> UpdateBranches(BranchesModel model)
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

                if (string.IsNullOrEmpty(model.BranchesName)
                    || string.IsNullOrEmpty(model.Address)
                    || string.IsNullOrEmpty(model.Phone))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kho không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IBrachesBusiness.UpdateBranches(model, User.UserId);

                Messaging.messaging = !Messaging.isError ? "Cập nhật kho thành công!" : "Cập nhật kho không thành công!";

            }
            catch(Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật kho không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Remove_Branches })]
        [HttpPost]
        public async Task<JsonResult> RemoveBranchs(BranchesModel model)
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
                if (model.BranchesId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kho không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IBrachesBusiness.RemoveBranchs(model.BranchesId);

                Messaging.messaging = Messaging.isError ? "Kho không xóa được vui lòng kiểm tra lại." : "Xóa Kho thành công!";

                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Kho không xóa được vui lòng kiểm tra lại.";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Channel
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Channel })]
        [HttpPost]
        public async Task<JsonResult> Create_Channel(ChannelModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {

                if (string.IsNullOrEmpty(model.Channel)
                   || string.IsNullOrEmpty(model.Code))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kênh không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IChannelBusiness.CreateChannel(model, User.UserId);

                Messaging.messaging = !Messaging.isError ? "Tạo kênh bán hàng thành công!" : "Tạo kênh bán hàng không thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo kênh bán hàng không thành công!";
            }

            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Channel })]
        [HttpPost]
        public async Task<JsonResult> Update_Channel(ChannelModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {

                if (model.Id <= 0
                   || string.IsNullOrEmpty(model.Channel)
                   || string.IsNullOrEmpty(model.Code))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kênh không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IChannelBusiness.UpdateChannel(model, User.UserId);

                Messaging.messaging = !Messaging.isError ? "Cập nhật kênh bán hàng thành công!" : "Cập nhật kênh bán hàng không thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật kênh bán hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Remove_Channel })]
        [HttpPost]
        public async Task<JsonResult> RemoveChannel(ChannelModel model)
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
                if (model.Id <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kênh không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = !await _IChannelBusiness.RemoveChannel(model.Id);

                Messaging.messaging = !Messaging.isError ? "Xóa kênh thành công!" : "Kênh không xóa được vui lòng kiểm tra lại.";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Kênh không xóa được vui lòng kiểm tra lại.";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
