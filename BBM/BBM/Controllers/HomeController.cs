using Newtonsoft.Json;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using BBM.Business.Models.Module;
using System.Collections.Generic;
using BBM.Infractstructure.Security;

namespace BBM.Controllers
{
    [CustomAuthorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.UserBranchId = User.BranchesId;
            ViewBag.UserName = User.UserName;
            return View();
        }
    }
}
