using Newtonsoft.Json;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BBM.Controllers
{
    public class AuthController : Controller
    {
        private admin_softbbmEntities _context;
        public AuthController()
        {
            _context = new admin_softbbmEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Access_Denied()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginViewModel model, string returnUrl = "")
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var user = _context.sys_Employee.Where(u => u.Email == model.Username && u.Pwd == model.Password).FirstOrDefault();
                    var branchsDefault = 0;
                    if (user != null)
                    {
                        if (user.Roles != null)
                        {
                            var roleObject = JsonConvert.DeserializeObject<dynamic>(user.Roles);

                            branchsDefault = roleObject[0].BrandId;
                        }
                        //var role = new List<Role_Branches>();

                        // var lstRole = EnumHelper<RolesEnum>.GetListValueEnum(typeof(RolesEnum));

                        //  foreach (var item in roleObject)
                        //{
                        //var rolesName = new List<string>();
                        //var fooArray = item.Roles.Value.Split(',');

                        //string firstRate = fooArray[0];
                        //if (!string.IsNullOrEmpty(firstRate))
                        //{
                        //    foreach (var ro in fooArray)
                        //    {
                        //        var nameRole = lstRole.FirstOrDefault(o => o.Key == int.Parse(ro));
                        //        rolesName.Add(nameRole.Value);
                        //    }
                        //    role.Add(new Infractstructure.Security.Role_Branches { BranchesId = item.BrandId, Roles = rolesName });

                        //}

                        //}

                        var branchesId = user.Branches_last.HasValue ? user.Branches_last.Value : branchsDefault;
                        var branches = _context.soft_Branches.Find(branchesId);

                        CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                        serializeModel.UserId = user.Id;
                        serializeModel.UserName = user.Name;
                        serializeModel.Email = user.Email;
                        serializeModel.BranchesId = branchesId;
                        serializeModel.IsPrimary = branches.IsPrimary;
                        serializeModel.ChannelId = user.Channel_last.HasValue ? user.Channel_last.Value : branches.soft_Channel.FirstOrDefault().Id;


                        //serializeModel.role_branches = role;

                        string userData = JsonConvert.SerializeObject(serializeModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                 1,
                                 user.Email,
                                 DateTime.Now,
                                 DateTime.Now.AddDays(30),
                                 // DateTime.Now.AddMinutes(15),
                                 false,
                                 userData);

                        string encTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                        //faCookie.Expires = DateTime.Now.AddDays(30);

                        Response.Cookies.Add(faCookie);

                        return RedirectToAction("Index", "Home");

                        //if (roles.Contains("Admin"))
                        //{
                        //    return RedirectToAction("Index", "Admin");
                        //}
                        //else if (roles.Contains("User"))
                        //{
                        //    return RedirectToAction("Index", "User");
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Index", "Home");
                        //}
                    }

                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Đăng nhập thất bại, vui lòng kiểm tra tài khoản.");
            }

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Auth", null);
        }

    }
}
