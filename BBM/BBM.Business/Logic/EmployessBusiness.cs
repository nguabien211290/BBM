using AutoMapper;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Business.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public class EmployessBusiness : IEmployessBusiness
    {
        private IUnitOfWork unitOfWork;

        public EmployessBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<EmployeeModel> GetEmployessby(PagingInfo pageinfo, out int count, out int min)
        {
            double totalMoney = 0;
            var result = unitOfWork.EmployeeRepository.SearchBy(pageinfo, out count, out min, out totalMoney);

            var employess = Mapper.Map<List<EmployeeModel>>(result);

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

            return employess;
        }

        //public JsonResult UpdateEmployess(EmployeeModel model)
        //{


        //    var objEmployee = Mapper.Map<sys_Employee>(model);
        //    if (model.Id <= 0)
        //    {
        //        objEmployee.DateCreate = DateTime.Now;
        //        objEmployee.EmployeeCreate = User.UserId;
        //        objEmployee.DateUpdate = null;
        //        _crud.Add<sys_Employee>(objEmployee);
        //    }
        //    else
        //    {
        //        var employee = _context.sys_Employee.Find(model.Id);
        //        objEmployee.DateCreate = employee.DateCreate;
        //        objEmployee.DateUpdate = DateTime.Now;
        //        objEmployee.EmployeeUpdate = User.UserId;
        //        //objEmployee.IsDisable = model.IsDisable;

        //        objEmployee.Pwd = !string.IsNullOrEmpty(model.Pwd) ? model.Pwd : employee.Pwd;

        //        _crud.Update<sys_Employee>(objEmployee, o => o.Titles, o => o.IsDisable, o => o.Pwd, o => o.Address, o => o.Email, o => o.Name, o => o.Phone, o => o.Phone, o => o.DateUpdate, o => o.EmployeeUpdate, o => o.Roles, o => o.Titles);
        //    }

        //    _crud.SaveChanges();
        //}

        //[HttpPost]
        //public JsonResult UpdateMyInfo(EmployeeModel model)
        //{
        //    var Messaging = new RenderMessaging();
        //    try
        //    {

        //        var employee = _context.sys_Employee.Find(User.UserId);

        //        if (User == null)
        //        {
        //            Messaging.isError = true;
        //            Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
        //            return Json(Messaging, JsonRequestBehavior.AllowGet);
        //        }

        //        if (model.isUpdatePwd && (!employee.Pwd.Equals(model.Pwd)))
        //        {
        //            Messaging.isError = true;
        //            Messaging.messaging = "Mật khẩu xác nhận không đúng.";
        //            return Json(Messaging, JsonRequestBehavior.AllowGet);
        //        }

        //        if (model.isUpdatePwd && (string.IsNullOrEmpty(model.Pwd) || string.IsNullOrEmpty(model.PwdNew) || model.PwdNew.Length <= 5))
        //        {
        //            Messaging.isError = true;
        //            Messaging.messaging = "Mật khẩu không hợp lệ.";
        //            return Json(Messaging, JsonRequestBehavior.AllowGet);
        //        }


        //        var objEmployee = Mapper.Map<sys_Employee>(model);

        //        objEmployee.Email = employee.Email;

        //        objEmployee.Pwd = model.isUpdatePwd ? model.PwdNew : employee.Pwd;

        //        objEmployee.DateCreate = employee.DateCreate;
        //        objEmployee.EmployeeCreate = employee.EmployeeCreate;

        //        objEmployee.DateUpdate = DateTime.Now;
        //        objEmployee.EmployeeUpdate = User.UserId;

        //        _crud.Update<sys_Employee>(objEmployee, o => o.Address, o => o.Name, o => o.Phone, o => o.DateUpdate, o => o.EmployeeUpdate, o => o.Pwd);


        //        _crud.SaveChanges();
        //        Messaging.messaging = "Cập nhật thông tin thành công!";
        //    }
        //    catch (Exception ex)
        //    {
        //        Messaging.isError = true;
        //        Messaging.messaging = "Cập nhật thông tin cá nhân không thành công!";
        //    }
        //    return Json(Messaging, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult UpdateEmloyeeTitle(Employee_TitleModel model)
        //{
        //    var Messaging = new RenderMessaging();
        //    try
        //    {

        //        if (User == null)
        //        {
        //            Messaging.isError = true;
        //            Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
        //            return Json(Messaging, JsonRequestBehavior.AllowGet);
        //        }

        //        var emp = Mapper.Map<soft_Employee_Title>(model);
        //        _crud.Add<soft_Employee_Title>(emp);
        //        _crud.SaveChanges();
        //        Messaging.messaging = "Cập nhật Chức danh thành công!";
        //    }
        //    catch (Exception ex)
        //    {
        //        Messaging.isError = true;
        //        Messaging.messaging = "Cập nhật chức danh không thành công!";
        //    }
        //    return Json(Messaging, JsonRequestBehavior.AllowGet);
        //}

        //[CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Delete_Employess })]
        //[HttpPost]
        //public JsonResult DeleteEmployess(int id)
        //{
        //    var Messaging = new RenderMessaging();
        //    try
        //    {
        //        if (User == null)
        //        {
        //            Messaging.isError = true;
        //            Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
        //            return Json(Messaging, JsonRequestBehavior.AllowGet);
        //        }

        //        var employee = _context.sys_Employee.Find(id);
        //        employee.IsDelete = true;

        //        _crud.Update<sys_Employee>(employee, o => o.IsDelete);

        //        _crud.SaveChanges();
        //        Messaging.messaging = "Xóa User thành công!";
        //    }
        //    catch (Exception ex)
        //    {
        //        Messaging.isError = true;
        //        Messaging.messaging = "Xóa nhân viên không thành công!";
        //    }
        //    return Json(Messaging, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult LoadEmployessTitles()
        //{
        //    var rs = Mapper.Map<List<Employee_TitleModel>>(_context.soft_Employee_Title.ToList());
        //    return Json(rs, JsonRequestBehavior.AllowGet);
        //}
    }
}
