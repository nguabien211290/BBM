using AutoMapper;
using BBM.Business;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace BBM.Controllers
{
    public class PartialController : BaseController
    {
        private admin_softbbmEntities _context;
        private CRUD _crud;
        private ExportExcelBusiness _exportBus;

        public PartialController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
            _exportBus = new ExportExcelBusiness();
        }


        public ActionResult Menu()
        {
            ViewBag.UserName = User.UserName;
            return PartialView("~/Views/Shared/Partial/module/Partial/_menu.cshtml");
        }


        public JsonResult GroupRole()
        {
            var result = new List<GroupRoleModel>();

            var lstEnumRole = (RolesEnum[])Enum.GetValues(typeof(RolesEnum));


            foreach (GroupRolesEnum grouprole in Enum.GetValues(typeof(GroupRolesEnum)))
            {
                var group = new Business.Models.Module.GroupRoleModel
                {
                    Name = EnumHelper<GroupRolesEnum>.GetDisplayValuebyInt((int)grouprole),
                    Roles = new List<RoleEnumModel>(),
                    Id = (int)grouprole
                };

                switch (group.Id)
                {
                    case (int)GroupRolesEnum.Account:
                        var enumAccount = lstEnumRole.Where(o =>
                            o == RolesEnum.Read_Employess
                         || o == RolesEnum.Create_Employess
                         || o == RolesEnum.Update_Employesss
                         || o == RolesEnum.Read_Roles_Employess
                         || o == RolesEnum.Delete_Employess
                         || o == RolesEnum.Update_Roles_Employess).ToList();

                        foreach (var role in enumAccount)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;
                    case (int)GroupRolesEnum.Suppliers:
                        var enumSuppliers = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Suppliers
                        || o == RolesEnum.Delete_Suppliers
                        || o == RolesEnum.Update_Suppliers).ToList();

                        foreach (var role in enumSuppliers)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.Customer:
                        var enumCustomers = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Customer
                        || o == RolesEnum.Create_Customer
                        || o == RolesEnum.Update_Customer).ToList();

                        foreach (var role in enumCustomers)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.Disscount:
                        var enumDisscount = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Disscount
                        || o == RolesEnum.Create_Disscount
                        || o == RolesEnum.Update_Disscount).ToList();

                        foreach (var role in enumDisscount)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.Products:
                        var enumProducts = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Products
                        || o == RolesEnum.Update_Products
                        || o == RolesEnum.Create_Products
                        || o == RolesEnum.Delete_Products
                        || o == RolesEnum.Update_Products_Price

                        || o == RolesEnum.Update_Products_Price_Discount).ToList();

                        foreach (var role in enumProducts)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.OrderSales:
                        var enumOrderSales = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Order_Sales
                        || o == RolesEnum.Create_Order_Sales
                        || o == RolesEnum.Update_Order_Sales).ToList();

                        foreach (var role in enumOrderSales)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.OrderSuppliers:
                        var enumOrderSuppliers = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Order_Suppliers
                        || o == RolesEnum.Create_Order_Suppliers
                        || o == RolesEnum.Update_Order_Suppliers).ToList();

                        foreach (var role in enumOrderSuppliers)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.OrderOutput:
                        var enumOrderOutput = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Order_OutPut
                        || o == RolesEnum.Create_Order_OutPut
                        || o == RolesEnum.Update_Order_OutPut).ToList();

                        foreach (var role in enumOrderOutput)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.OrderInput:
                        var enumOrderInput = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Order_Input
                        || o == RolesEnum.Create_Order_Input
                        || o == RolesEnum.Update_Order_Input).ToList();

                        foreach (var role in enumOrderInput)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;
                    case (int)GroupRolesEnum.Product_Switch:
                        var enumProduct_Switch = lstEnumRole.Where(o =>
                           o == RolesEnum.Create_Product_Switch
                        || o == RolesEnum.Read_Product_Switch).ToList();

                        foreach (var role in enumProduct_Switch)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.Branches:
                        var enumBranches = lstEnumRole.Where(o =>
                           o == RolesEnum.Read_Branches
                        || o == RolesEnum.Update_Branches
                        || o == RolesEnum.Remove_Branches
                        || o == RolesEnum.Create_Branches).ToList();

                        foreach (var role in enumBranches)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;



                    case (int)GroupRolesEnum.Channel:
                        var enumChannel = lstEnumRole.Where(o =>
                           o == RolesEnum.Create_Channel
                        || o == RolesEnum.Remove_Channel
                        || o == RolesEnum.Update_Channel).ToList();

                        foreach (var role in enumChannel)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.Catalog:
                        var enumCatalog = lstEnumRole.Where(o =>
                         o == RolesEnum.Read_Catalog
                        || o == RolesEnum.Remove_Catalog
                        || o == RolesEnum.Update_Catalog).ToList();

                        foreach (var role in enumCatalog)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;

                    case (int)GroupRolesEnum.OrderBranches:
                        var enumOrderBranches = lstEnumRole.Where(o =>
                         o == RolesEnum.Read_Order_Branches
                        || o == RolesEnum.Create_Order_Branches
                        || o == RolesEnum.Update_Order_Branches).ToList();

                        foreach (var role in enumOrderBranches)
                        {
                            group.Roles.Add(new Business.Models.Module.RoleEnumModel
                            {
                                Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                                Id = (int)role,
                                isSelect = false
                            });
                        }
                        break;
                }
                result.Add(group);

            }
            return Json(result, JsonRequestBehavior.AllowGet);


        }


        public JsonResult SetChannel(int ChannelId = 0)
        {
            if (ChannelId > 0)
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {

                    var Channel = _context.soft_Channel.Find(ChannelId);

                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    CustomPrincipalSerializeModel serializeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);

                    var employee = _context.sys_Employee.Find(serializeModel.UserId);


                    serializeModel.ChannelId = ChannelId;
                    serializeModel.BranchesId = Channel.soft_Branches.BranchesId;
                    serializeModel.IsPrimary = Channel.soft_Branches.IsPrimary;
                    string userData = Newtonsoft.Json.JsonConvert.SerializeObject(serializeModel);
                    var newticket = new FormsAuthenticationTicket(
                                              authTicket.Version,
                                              authTicket.Name,
                                              authTicket.IssueDate,
                                              authTicket.Expiration,
                                              true,
                                             userData);

                    authCookie.Value = FormsAuthentication.Encrypt(newticket);
                    HttpContext.Response.Cookies.Set(authCookie);

                    employee.Channel_last = ChannelId;
                    employee.Branches_last = serializeModel.BranchesId;

                    _crud.Update<sys_Employee>(employee, o => o.Channel_last, o => o.Branches_last);
                    _crud.SaveChanges();
                }

            }
            return null;
        }
        
        public void Excel(int type = 0)
        {
            _exportBus.ToExcel(Response, type);
        }
    }
}
