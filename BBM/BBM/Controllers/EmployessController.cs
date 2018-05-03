using AutoMapper;
using Newtonsoft.Json;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBM.Infractstructure.Security;
using BBM.Business.Logic;

namespace BBM.Controllers
{
    public class EmployessController : BaseController
    {
        //
        // GET: /Employess/

        private CRUD _crud;
        private admin_softbbmEntities _context;

        private IEmployessBusiness _IEmployessBusiness;
        public EmployessController(IEmployessBusiness IEmployessBusiness)
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
            _IEmployessBusiness = IEmployessBusiness;
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Employess })]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Employess/Employess.cshtml");
        }

        public ActionResult Render_info_View()
        {
            return PartialView("~/Views/Shared/Partial/module/Employess/Employess_info.cshtml");
        }

        public JsonResult LoadEmployessInfo()
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var employ = _context.sys_Employee.Find(User.UserId);

                if (employ == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var result = Mapper.Map<EmployeeModel>(employ);

                Messaging.Data = result;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị nhân viên không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Employess })]
        public JsonResult GetEmployessby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging<Channel_Paging<EmployeeModel>> ();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Channel_Paging<EmployeeModel> lstInfo = new Channel_Paging<EmployeeModel>();

                int count, min = 0;

                var rs = _IEmployessBusiness.GetEmployessby(pageinfo, out count, out min);

                lstInfo.startItem = min;

                lstInfo.totalItems = count;

                lstInfo.listTable = rs;

                Messaging.Data = lstInfo;
                
                Messaging.Data = lstInfo;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách nhân viên không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Employesss, RolesEnum.Update_Roles_Employess })]
        [HttpPost]
        public JsonResult UpdateEmployess(EmployeeModel model)
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

                if (model.Id <= 0 && (string.IsNullOrEmpty(model.Pwd) || model.Pwd.Length <= 5))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Mật khẩu không hợp lệ.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Email))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Email không được rỗng!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }


                var objEmployee = Mapper.Map<sys_Employee>(model);
                if (model.Id <= 0)
                {
                    objEmployee.DateCreate = DateTime.Now;
                    objEmployee.EmployeeCreate = User.UserId;
                    objEmployee.DateUpdate = null;
                    _crud.Add<sys_Employee>(objEmployee);
                }
                else
                {
                    var employee = _context.sys_Employee.Find(model.Id);
                    objEmployee.DateCreate = employee.DateCreate;
                    objEmployee.DateUpdate = DateTime.Now;
                    objEmployee.EmployeeUpdate = User.UserId;
                    //objEmployee.IsDisable = model.IsDisable;

                    objEmployee.Pwd = !string.IsNullOrEmpty(model.Pwd) ? model.Pwd : employee.Pwd;

                    _crud.Update<sys_Employee>(objEmployee, o => o.Titles, o => o.IsDisable, o => o.Pwd, o => o.Address, o => o.Email, o => o.Name, o => o.Phone, o => o.Phone, o => o.DateUpdate, o => o.EmployeeUpdate, o => o.Roles, o => o.Titles);
                }

                _crud.SaveChanges();
                Messaging.messaging = "Cập nhật nhân viên thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật nhân viên không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateMyInfo(EmployeeModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {

                var employee = _context.sys_Employee.Find(User.UserId);

                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (model.isUpdatePwd && (!employee.Pwd.Equals(model.Pwd)))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Mật khẩu xác nhận không đúng.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (model.isUpdatePwd && (string.IsNullOrEmpty(model.Pwd) || string.IsNullOrEmpty(model.PwdNew) || model.PwdNew.Length <= 5))
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Mật khẩu không hợp lệ.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }


                var objEmployee = Mapper.Map<sys_Employee>(model);

                objEmployee.Email = employee.Email;

                objEmployee.Pwd = model.isUpdatePwd ? model.PwdNew : employee.Pwd;

                objEmployee.DateCreate = employee.DateCreate;
                objEmployee.EmployeeCreate = employee.EmployeeCreate;

                objEmployee.DateUpdate = DateTime.Now;
                objEmployee.EmployeeUpdate = User.UserId;

                _crud.Update<sys_Employee>(objEmployee, o => o.Address, o => o.Name, o => o.Phone, o => o.DateUpdate, o => o.EmployeeUpdate, o => o.Pwd);


                _crud.SaveChanges();
                Messaging.messaging = "Cập nhật thông tin thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật thông tin cá nhân không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmloyeeTitle(Employee_TitleModel model)
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

                var emp = Mapper.Map<soft_Employee_Title>(model);
                _crud.Add<soft_Employee_Title>(emp);
                _crud.SaveChanges();
                Messaging.messaging = "Cập nhật Chức danh thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật chức danh không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Delete_Employess })]
        [HttpPost]
        public JsonResult DeleteEmployess(int id)
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

                var employee = _context.sys_Employee.Find(id);
                employee.IsDelete = true;

                _crud.Update<sys_Employee>(employee, o => o.IsDelete);

                _crud.SaveChanges();
                Messaging.messaging = "Xóa User thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Xóa nhân viên không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadEmployessTitles()
        {
            var rs = Mapper.Map<List<Employee_TitleModel>>(_context.soft_Employee_Title.ToList());
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

    }
}
