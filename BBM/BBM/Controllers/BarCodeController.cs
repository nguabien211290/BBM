using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Logic;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class BarCodeController : BaseController
    {
        private IBarCodeBusiness _IBarCodeBusiness;
        private IUnitOfWork _unitOW;
        public BarCodeController(IBarCodeBusiness IBarCodeBusiness, IUnitOfWork unitOW)
        {
            _IBarCodeBusiness = IBarCodeBusiness;
            _unitOW = unitOW;
        }

        public ActionResult RenderView(List<Order_DetialModel> products = null)
        {
            if (products != null)
            {
                var rs = new List<Prodcut_Branches_PriceChannel>();
                foreach (var item in products)
                {
                    if (item.ProductId > 0)
                    {
                        var product = _unitOW.ProductRepositoryV2.GetById(item.ProductId);
                        if (product != null)
                        {
                            var pro = Mapper.Map<ProductSampleModel>(product);
                            if (product.soft_Channel_Product_Price != null)
                            {
                                var price = product.soft_Channel_Product_Price.FirstOrDefault(o => o.ProductId == product.id && o.ChannelId == User.ChannelId);

                                pro.PriceChannel = price != null ? price.Price : 0;
                            }

                            pro.Stock_Total = item.Total;

                            rs.Add(new Prodcut_Branches_PriceChannel
                            {
                                product = pro
                            });
                        }
                    }
                }
                ViewBag.Products = Newtonsoft.Json.JsonConvert.SerializeObject(rs);
            }
            return PartialView("~/Views/Shared/Partial/module/Barcode/Index.cshtml");
        }

        [HttpGet]
        public ActionResult GetConfig()
        {
            var Messaging = new RenderMessaging<BarcodeModel>();
            try
            {
                var config = _IBarCodeBusiness.GetConfig(User.BranchesId);
                if (config == null)
                    Messaging.Data = new BarcodeModel();
                else
                    Messaging.Data = config;

                Messaging.isError = false;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Không lấy được cấu hình, vui lòng thử lại.";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveConfig(BarcodeModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                model.BranchId = User.BranchesId;

                Messaging.isError = !await _IBarCodeBusiness.SaveConfig(model);

                Messaging.messaging = !Messaging.isError ? "Lưu cấu hình thành công." : "Không lưu được cấu hình, vui lòng thử lại!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Không lưu được cấu hình, vui lòng thử lại!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}
