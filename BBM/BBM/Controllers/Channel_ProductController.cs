using AutoMapper;
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

namespace BBM.Controllers
{
    public class Channel_ProductController : BaseController
    {
        //
        // GET: /KENH_Product/
        private CRUD _crud;
        private admin_softbbmEntities _context = new admin_softbbmEntities();
        public Channel_ProductController()
        {
            _crud = new CRUD();
        }
        public ActionResult RenderView()
        {
            var a = User.ChannelId;

            return PartialView("~/Views/Shared/Partial/module/Product/_Channel_product.cshtml");
        }

        public JsonResult LoadListChannel_product(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var lstTmp = _context.soft_Channel_Product_Price.Where(o => o.ChannelId.Equals(User.ChannelId)).ToList();

                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    lstTmp = lstTmp.Where(o =>
                       (!string.IsNullOrEmpty(o.shop_sanpham.tensp) && Helpers.convertToUnSign3(o.shop_sanpham.tensp.ToLower()).Contains(pageinfo.keyword))
                     || (!string.IsNullOrEmpty(o.shop_sanpham.Barcode) && Helpers.convertToUnSign3(o.shop_sanpham.Barcode.ToLower()).Contains(pageinfo.keyword))
                     || (!string.IsNullOrEmpty(o.shop_sanpham.masp) && Helpers.convertToUnSign3(o.shop_sanpham.masp.ToLower()).Contains(pageinfo.keyword))
                    ).ToList();
                }
                #endregion
                #region Fillter
                if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
                {
                    foreach (var item in pageinfo.filterby)
                    {
                        switch (item.Name)
                        {
                            case "Catalog":
                                if (item.Values != null && item.Values.Length > 0)
                                    lstTmp = lstTmp.Where(o => item.Values.Contains(o.shop_sanpham.CatalogId.ToString())).ToList();
                                break;
                            case "Suppliers":
                                if (item.Values != null && item.Values.Length > 0)
                                    lstTmp = lstTmp.Where(o => item.Values.Contains(o.shop_sanpham.SuppliersId.ToString())).ToList();
                                break;
                            case "Unit":
                                if (item.Values != null && item.Values.Length > 0)
                                    lstTmp = lstTmp.Where(o => item.Values.Contains(o.shop_sanpham.UnitId.ToString())).ToList();
                                break;
                        }
                    }

                }
                #endregion
                #region Sort
                if (!string.IsNullOrEmpty(pageinfo.sortby))
                {
                    switch (pageinfo.sortby)
                    {
                        case "ProductName":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.shop_sanpham.tensp).ToList();
                            else
                                lstTmp = lstTmp.OrderBy(o => o.shop_sanpham.tensp).ToList();
                            break;
                        case "Code":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.shop_sanpham.masp).ToList();
                            else
                                lstTmp = lstTmp.OrderBy(o => o.shop_sanpham.masp).ToList();
                            break;
                    }

                }
                #endregion
                Channel_Paging<Channel_Product_PriceModel> lstInfo = new Channel_Paging<Channel_Product_PriceModel>();
                if (lstTmp != null && lstTmp.Count > 0)
                {
                    int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                    lstInfo.totalItems = lstTmp.Count();
                    int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                    if (pageinfo.pagesize < lstTmp.Count)
                        if (quantity > 0)
                            lstTmp = lstTmp.GetRange(min, quantity);
                    lstInfo.listTable = Mapper.Map<List<Channel_Product_PriceModel>>(lstTmp);
                    lstInfo.startItem = min;
                }
                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách Kênh - Sản phẩm không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ChangePrice(List<Channel_Product_PriceModel> model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                foreach (var item in model)
                {
                    var data = new soft_Channel_Product_Price
                    {
                        Id = item.Id,
                        Price = item.PriceChange,
                        ChannelId = item.ChannelId,
                    };
                    _crud.Update<soft_Channel_Product_Price>(data, o => o.Price);
                }
                _crud.SaveChanges();
                Messaging.messaging = "Đã thay đổi giá thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Thay đổi giá không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveProduct(int Id)
        {
            var Messaging = new RenderMessaging();
            try
            {
                var hasIts = _context.soft_Channel_Product_Price.Find(Id);
                if (hasIts != null)
                {
                    _context.soft_Channel_Product_Price.Remove(hasIts);
                    _context.SaveChanges();
                    Messaging.messaging = "Đã xóa sản phẩm này ra khỏi Kênh thành công!";

                }
                else
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Không tìm thấy sản phẩm này trong Kênh.";
                }
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Xóa sản phẩm này ra khỏi Kênh không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProductIntoChannel(int ProductId)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId > 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }
                var product = _context.shop_sanpham.Find(ProductId);
                if (product != null)
                {
                    var data = new soft_Channel_Product_Price()
                    {
                        Price = product.PriceInput.HasValue ? product.PriceInput.Value : 0,
                        ProductId = ProductId,
                        ChannelId = User.ChannelId,
                        EmployeeCreate = User.UserId,
                        DateCreate = DateTime.Now
                    };
                    _crud.Add<soft_Channel_Product_Price>(data);

                }
                _crud.SaveChanges();
                Messaging.messaging = "Đã thêm sản phẩm vào kênh thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Thêm sản phẩm vào kênh không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}
