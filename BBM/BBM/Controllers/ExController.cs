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
using BBM.Business.Repository;
using BBM.Business.Logic;

namespace BBM.Controllers
{
    public class ExController : BaseController
    {

        private IProductRepository _Ipr;
        private IOrderBusiness _IOrderBus;
        public ExController(IProductRepository Ipr, IOrderBusiness IOrderBus)
        {
            _Ipr = Ipr;
            _IOrderBus = IOrderBus;
        }
        //public JsonResult Index(int id)
        //{
        //    //var a = Mapper.Map<ProductSampleModel>(_Ipr.FindBy(o => o.id == id).FirstOrDefault());
        //    //var a = _IOrderBus.GetId(id);
        //    return Json(new { id = a.Id, tensp = a.Code }, JsonRequestBehavior.AllowGet);
        //}
    }
}
