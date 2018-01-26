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

namespace BBM.Controllers
{
    public class EmployessController : BaseController
    {
        //
        // GET: /Employess/

        private CRUD _crud;
        private admin_softbbmEntities _context;
        public EmployessController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
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
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var lstTmp = from employ in _context.sys_Employee
                             where employ.IsDelete == false
                             select new EmployeeModel
                             {
                                 Name = employ.Name,
                                 Email = employ.Email,
                                 Titles = employ.Titles,
                                 Phone = employ.Phone,
                                 Address = employ.Address,
                                 Id = employ.Id,
                                 DateCreate = employ.DateCreate,
                                 EmployeeCreate = employ.EmployeeCreate,
                                 Roles = employ.Roles,
                                 IsDisable = employ.IsDisable.HasValue ? employ.IsDisable.Value : false
                             };

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
                        case "Titles":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Titles);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Titles);
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
                        case "Address":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Address);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Address);
                            break;

                    }

                }
                #endregion
                var employess = lstTmp.ToList();

                foreach (var item in employess)
                {
                    if (!string.IsNullOrEmpty(item.Roles))
                    {
                        item.GroupRoles = new List<RolesObject>();

                        var rol = JsonConvert.DeserializeObject<dynamic>(item.Roles);
                        foreach (var o in rol)
                        {
                            var newobj = new RolesObject();
                            newobj.BrandId = o.BrandId;
                            newobj.Roles = new List<int>();


                            var fooArray = o.Roles.Value.Split(',');

                            string firstRate = fooArray[0];
                            if (!string.IsNullOrEmpty(firstRate))
                            {
                                foreach (var ro in fooArray)
                                {
                                    newobj.Roles.Add(int.Parse(ro));
                                }
                                item.GroupRoles.Add(newobj);
                            }
                        }
                    }
                }


                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    var lstcustomer = lstTmp.ToList();
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    employess = employess.Where(o =>
                    (!string.IsNullOrEmpty(o.Name) && Helpers.convertToUnSign3(o.Name.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Phone) && Helpers.convertToUnSign3(o.Phone.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Email) && Helpers.convertToUnSign3(o.Email.ToLower()).Contains(pageinfo.keyword))
                   || (!string.IsNullOrEmpty(o.Address) && Helpers.convertToUnSign3(o.Address.ToLower()).Contains(pageinfo.keyword))).ToList();

                }
                #endregion
                Channel_Paging<EmployeeModel> lstInfo = new Channel_Paging<EmployeeModel>();
                if (employess != null && employess.Count > 0)
                {
                    int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                    lstInfo.totalItems = employess.Count();
                    int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                    if (pageinfo.pagesize < employess.Count)
                        if (quantity > 0)
                            employess = employess.GetRange(min, quantity);
                    lstInfo.startItem = min;
                    lstInfo.listTable = employess;

                }

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
