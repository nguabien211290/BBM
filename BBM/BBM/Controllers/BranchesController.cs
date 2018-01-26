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

namespace BBM.Controllers
{
    public class BranchesController : BaseController
    {
        //
        // GET: /Branches/
        private CRUD _crud;
        private admin_softbbmEntities _context = new admin_softbbmEntities();
        public BranchesController()
        {
            _crud = new CRUD();
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Branches/Branches.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Branches })]
        public ActionResult GetBranches()
        {
            var Messaging = new RenderMessaging();

            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }
                var result = _context.soft_Branches.ToList();
                var lst = Mapper.Map<List<BranchesModel>>(result);
                Messaging.Data = lst;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách Kho có lỗi!";
            }

            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Branches })]
        [HttpPost]
        public JsonResult UpdateBranches(BranchesModel model)
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
                    if (string.IsNullOrEmpty(model.BranchesName))
                    {
                        Messaging.isError = true;
                        Messaging.messaging = "Tên Kho không hợp lệ";
                        return Json(Messaging, JsonRequestBehavior.AllowGet);
                    }


                    var objBraches = Mapper.Map<soft_Branches>(model);
                    objBraches.DateUpdate = null;
                    objBraches.DateCreate = DateTime.Now;
                    objBraches.EmployeeCreate = User.UserId;
                    objBraches.Type = 1;
                    _crud.Add<soft_Branches>(objBraches);
                    _crud.SaveChanges();


                    Messaging.messaging = "Tạo kho thành công!";

                }
                else
                {

                    if (string.IsNullOrEmpty(model.BranchesName)
                        || string.IsNullOrEmpty(model.Address)
                        || string.IsNullOrEmpty(model.Phone))
                    {
                        Messaging.isError = true;
                        Messaging.messaging = "Thông tin Kho không hợp lệ";
                        return Json(Messaging, JsonRequestBehavior.AllowGet);
                    }


                    var objBraches = Mapper.Map<soft_Branches>(model);
                    objBraches.DateUpdate = DateTime.Now;
                    objBraches.EmployeeUpdate = User.UserId;

                    _crud.Update(objBraches, o => o.BranchesName,
                        o => o.Code,
                        o => o.IsPrimary,
                        o => o.Phone, o => o.Address,
                        o => o.DateUpdate, o => o.EmployeeUpdate);
                    _crud.SaveChanges();


                    Messaging.messaging = "Cập nhật kho thành công!";
                }

            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật kho không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Branches })]
        [HttpPost]
        public JsonResult CreateBranches(BranchesModel model)
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

                if (model.BranchesId <= 0
                   || string.IsNullOrEmpty(model.BranchesName)
                   || string.IsNullOrEmpty(model.Address)
                   || string.IsNullOrEmpty(model.Phone)
                   || model.soft_Channel.Count <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kho không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var channelvalidate = model.soft_Channel.FirstOrDefault(o => o.Channel == string.Empty || o.Code == string.Empty);

                if (channelvalidate != null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kênh không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var objBraches = Mapper.Map<soft_Branches>(model);

                objBraches.DateCreate = DateTime.Now;
                objBraches.EmployeeCreate = User.UserId;

                _crud.Add<soft_Branches>(objBraches);

                foreach (var item in model.soft_Channel)
                {
                    var objChannel = Mapper.Map<soft_Channel>(item);

                    objChannel.DateCreate = DateTime.Now;
                    objChannel.EmployeeCreate = User.UserId;

                    _crud.Add<soft_Channel>(objChannel);
                }

                _crud.SaveChanges();

                Messaging.messaging = "Tạo kho thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo kho không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Channel })]
        [HttpPost]
        public JsonResult Update_Channel(ChannelModel model)
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


                var channel = _context.soft_Channel.Find(model.Id);
                if (channel == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kênh không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }


                var objChannel = Mapper.Map<soft_Channel>(model);

                if (model.Id > 0)
                {
                    objChannel.DateUpdate = DateTime.Now;
                    objChannel.EmployeeUpdate = User.UserId;
                    objChannel.Code = model.Code;
                    objChannel.Channel = model.Channel;
                    objChannel.Note = model.Note;
                    objChannel.Code = model.Code;
                    objChannel.DateCreate = DateTime.Now;
                    _crud.Update<soft_Channel>(objChannel, o => o.Code, o => o.Channel, o => o.Note, o => o.DateUpdate, o => o.EmployeeUpdate);
                    _crud.SaveChanges();
                    Messaging.messaging = "Cập nhật kênh bán hàng thành công!";
                }
                else
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin kênh không hợp lệ!";
                }

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
        public JsonResult RemoveChannel(ChannelModel model)
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
                var channel = _context.soft_Channel.Find(model.Id);
                if (channel == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kênh không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                _context.soft_Channel.Remove(channel);

                _context.SaveChanges();

                Messaging.messaging = "Xóa kênh thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Kênh không xóa được vui lòng kiểm tra lại.";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Channel })]
        [HttpPost]
        public JsonResult Create_Channel(ChannelModel model)
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

                var objChannel = Mapper.Map<soft_Channel>(model);


                objChannel.DateCreate = DateTime.Now;
                objChannel.EmployeeCreate = User.UserId;
                objChannel.DateUpdate = null;



                _crud.Add<soft_Channel>(objChannel);
                _crud.SaveChanges();

                Messaging.messaging = "Tạo kênh bán hàng thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo kênh bán hàng không thành công!";
            }

            return Json(Messaging, JsonRequestBehavior.AllowGet);

        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Remove_Branches })]
        [HttpPost]
        public JsonResult RemoveBranchs(BranchesModel model)
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
                var braches = _context.soft_Branches.Find(model.BranchesId);
                if (braches == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Thông tin Kho không hợp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                _context.soft_Branches.Remove(braches);

                _context.SaveChanges();

                Messaging.messaging = "Xóa Kho thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Kho không xóa được vui lòng kiểm tra lại.";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

    }
}
