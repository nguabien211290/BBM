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
    public class ReportController : Controller
    {

        private IUnitOfWork _unitOW;
        public ReportController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.report_sales })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Report/Report.cshtml");
        }
    }
}