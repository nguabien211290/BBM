using BBM.Business.Logic;
using BBM.Business.Models.Enum;
using BBM.Business.Repository;
using BBM.Infractstructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class SampleController : Controller
    {

        private IUnitOfWork _unitOW;
        public SampleController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.report_sales })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Sample/Sample.cshtml");
        }
    }
}