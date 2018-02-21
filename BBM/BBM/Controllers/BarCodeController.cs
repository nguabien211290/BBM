using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class BarCodeController : BaseController
    {
        private CRUD _crud;
        private admin_softbbmEntities _context = new admin_softbbmEntities();
        public BarCodeController()
        {
            _crud = new CRUD();
        }

        public ActionResult RenderView(List<Order_DetialModel> products = null)
        {
            if (products != null)
            {
                var rs = new List<Prodcut_Branches_PriceChannel>();
                foreach (var item in products)
                {
                    if (item.Product.id > 0)
                    {
                        var product = _context.shop_sanpham.Find(item.Product.id);
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

        public ActionResult GetConfig()
        {
            var Messaging = new RenderMessaging();
            try
            {
                var config = _context.soft_Config_PrintTem.FirstOrDefault(o => o.BranchId == User.BranchesId);
                if (config == null)
                    Messaging.Data = new soft_Config_PrintTem();
                else
                    Messaging.Data = config;

                Messaging.isError = false;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Không lưu được cấu hình, vui lòng thử lại.";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveConfig(soft_Config_PrintTem model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                model.BranchId = User.BranchesId;
                if (model.Id <= 0)
                {
                    model.BranchId = User.BranchesId;
                    _crud.Add(model);
                }
                else
                    _crud.Update(model);

                _crud.SaveChanges();
                Messaging.isError = false;
                Messaging.messaging = "Lưu cấu hình thành công.";
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
