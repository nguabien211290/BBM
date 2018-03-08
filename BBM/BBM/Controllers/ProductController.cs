using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Models.View;
using System;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Models.Enum;
using System.Data;
using BBM.Infractstructure.Security;
using BBM.Business.Logic;
using BBM.Business.Model.Module;
using BBM.Business.Repository;

namespace BBM.Controllers
{
    public class ProductController : BaseController
    {
        private CRUD _crud;
        private admin_softbbmEntities _context;
        private IOrderBusiness _IOrderBus;
        private IUnitOfWork _unitOW;
        public ProductController(IOrderBusiness IOrderBus, IUnitOfWork unitOW)
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
            _IOrderBus = IOrderBus;
            _unitOW = unitOW;
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Products })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Product/Product.cshtml");
        }
        public JsonResult GetProductbyId(int Id)
        {
            var Messaging = new RenderMessaging();
            try
            {
                var product = _context.shop_sanpham.Find(Id);
                if (product == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Sản phẩm không tồn tại.";
                }
                Messaging.Data = Mapper.Map<ProductSampleModel>(product);
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị sản phẩm không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                }

                //  var lstTmp = from product in _context.shop_sanpham select product;


                var lstTmp = _context.shop_sanpham.Select(o => new ProductSampleModel
                {
                    Barcode = o.Barcode,
                    tensp = o.tensp,
                    masp = o.masp,
                    CatalogId = o.CatalogId.HasValue ? o.CatalogId.Value : 0,
                    SuppliersId = o.SuppliersId.HasValue ? o.SuppliersId.Value : 0,
                    id = o.id,
                    Img = o.shop_image.FirstOrDefault().url,
                    PriceBase = o.PriceBase.HasValue ? o.PriceBase.Value : 0,
                    PriceCompare = o.PriceCompare.HasValue ? o.PriceCompare.Value : 0,
                    PriceInput = o.PriceInput.HasValue ? o.PriceInput.Value : 0,
                    PriceBase_Old = o.PriceBase_Old.HasValue ? o.PriceBase_Old.Value : 0,
                    Status = o.Status.HasValue ? o.Status.Value : 0,
                    StatusVAT = o.StatusVAT.HasValue ? o.StatusVAT.Value : 0,
                    DateCreate = o.DateCreate,
                    //PriceWholesale = o.PriceWholesale.HasValue ? o.PriceWholesale.Value : 0,
                    Stock_Sum = o.soft_Branches_Product_Stock.Any() ? o.soft_Branches_Product_Stock.Sum(p => p.Stock_Total) : 0
                });

                // IQueryable<shop_sanpham> lstTmp = _context.shop_sanpham;

                #region Fillter
                if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
                {
                    foreach (var item in pageinfo.filterby)
                    {
                        var key = 0;
                        if (item.Fiter.Equals("Price") || item.Fiter.Equals("Stock"))
                            key = int.Parse(item.Name);
                        else
                            key = int.Parse(item.Value);

                        switch (item.Fiter)
                        {
                            case "Catalog":
                                lstTmp = lstTmp.Where(o => o.CatalogId > 0 && o.CatalogId.Equals(key));
                                break;
                            case "Suppliers":
                                lstTmp = lstTmp.Where(o => o.SuppliersId > 0 && o.SuppliersId.Equals(key));
                                break;
                            case "Status":
                                lstTmp = lstTmp.Where(o => o.Status > 0 && o.Status.Equals(key));
                                break;
                            case "VAT":
                                lstTmp = lstTmp.Where(o => o.StatusVAT > 0 && o.StatusVAT.Equals(key));
                                break;
                            case "Price":
                                if (item.Value == "Equals")
                                {
                                    var soft_Prices = _context.soft_Channel_Product_Price.Where(o => o.Price == key
                                    && o.ChannelId == User.ChannelId).Select(o => o.ProductId);

                                    lstTmp = lstTmp.Where(o => soft_Prices.Contains(o.id));
                                }
                                if (item.Value == "LessThan")
                                {
                                    var soft_Prices = _context.soft_Channel_Product_Price.Where(o => o.Price < key
                                    && o.ChannelId == User.ChannelId).Select(o => o.ProductId);

                                    lstTmp = lstTmp.Where(o => soft_Prices.Contains(o.id));
                                }
                                if (item.Value == "MoreThan")
                                {
                                    var soft_Prices = _context.soft_Channel_Product_Price.Where(o => o.Price > key
                                    && o.ChannelId == User.ChannelId).Select(o => o.ProductId);

                                    lstTmp = lstTmp.Where(o => soft_Prices.Contains(o.id));
                                }
                                break;
                            case "Stock":
                                var branchesId = int.Parse(item.Value2.ToString());
                                if (branchesId > 0)
                                {
                                    if (item.Value == "Equals")
                                    {
                                        var soft_Stock = _context.soft_Branches_Product_Stock.Where(o => o.Stock_Total == key
                                        && o.BranchesId == branchesId).Select(o => o.ProductId);

                                        if (key == 0)
                                        {
                                            var soft_StockNull = _context.soft_Branches_Product_Stock.Where(o => o.BranchesId == User.BranchesId).Select(o => o.ProductId);

                                            lstTmp = lstTmp.Where(o => soft_Stock.Contains(o.id) || !soft_StockNull.Contains(o.id));
                                        }
                                        else
                                        {
                                            lstTmp = lstTmp.Where(o => soft_Stock.Contains(o.id));
                                        }

                                    }
                                    if (item.Value == "LessThan")
                                    {
                                        var soft_Stock = _context.soft_Branches_Product_Stock.Where(o => o.Stock_Total < key
                                       && o.BranchesId == branchesId).Select(o => o.ProductId);

                                        lstTmp = lstTmp.Where(o => soft_Stock.Contains(o.id));
                                    }
                                    if (item.Value == "MoreThan")
                                    {
                                        var soft_Stock = _context.soft_Branches_Product_Stock.Where(o => o.Stock_Total > key
                                      && o.BranchesId == branchesId).Select(o => o.ProductId);

                                        lstTmp = lstTmp.Where(o => soft_Stock.Contains(o.id));
                                    }
                                }
                                else
                                {
                                    var sum_Stock = _context.soft_Branches_Product_Stock.GroupBy(o => o.ProductId).Select(o => new
                                    {
                                        productid = o.Key,
                                        total = o.Sum(i => i.Stock_Total)
                                    });

                                    if (item.Value == "MoreThan")
                                    {
                                        var MoreThan = sum_Stock.Where(o => o.total > key).Select(o => o.productid);

                                        lstTmp = lstTmp.Where(o => MoreThan.Contains(o.id));
                                    }
                                    if (item.Value == "LessThan")
                                    {
                                        var LessThan = sum_Stock.Where(o => o.total < key).Select(o => o.productid);

                                        lstTmp = lstTmp.Where(o => LessThan.Contains(o.id));
                                    }
                                    if (item.Value == "Equals")
                                    {
                                        var Equals = sum_Stock.Where(o => o.total == key).Select(o => o.productid);

                                        lstTmp = lstTmp.Where(o => Equals.Contains(o.id));
                                    }
                                }
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
                        case "DateCreate":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.DateCreate);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.DateCreate);
                            break;
                        case "Id":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.id);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.id);
                            break;
                        case "Barcode":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Barcode);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Barcode);
                            break;
                        case "PriceBase":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.PriceBase);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.PriceBase);
                            break;
                        case "PriceBase_Old":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.PriceBase_Old);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.PriceBase_Old);
                            break;
                        case "PriceCompare":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.PriceCompare);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.PriceCompare);
                            break;

                        case "PriceInput":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.PriceInput);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.PriceInput);
                            break;
                        case "ProductName":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.tensp);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.tensp);
                            break;
                        case "Code":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.CatalogId);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.CatalogId);
                            break;
                        case "StatusVAT":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.StatusVAT);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.StatusVAT);
                            break;
                        case "Stock_Sum":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Stock_Sum);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Stock_Sum);
                            break;
                    }

                }
                #endregion
                var products = new List<ProductSampleModel>();

                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    lstTmp = lstTmp.Where(o =>
                     (!string.IsNullOrEmpty(o.tensp) && o.tensp.Contains(pageinfo.keyword))
                     || (!string.IsNullOrEmpty(o.Barcode) && o.Barcode.Contains(pageinfo.keyword))
                     || (!string.IsNullOrEmpty(o.masp) && o.masp.Contains(pageinfo.keyword))
                    );

                    //lstTmp = lstTmp.Where(o =>
                    //     (!string.IsNullOrEmpty(o.tensp) && Helpers.convertToUnSign3(o.tensp.ToLower()).Contains(pageinfo.keyword))
                    //     || (!string.IsNullOrEmpty(o.Barcode) && Helpers.convertToUnSign3(o.Barcode.ToLower()).Contains(pageinfo.keyword))
                    //     || (!string.IsNullOrEmpty(o.masp) && Helpers.convertToUnSign3(o.masp.ToLower()).Contains(pageinfo.keyword))
                    //    );
                }
                #endregion
                Channel_Paging<Prodcut_Branches_PriceChannel> lstInfo = new Channel_Paging<Prodcut_Branches_PriceChannel>();

                int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                lstInfo.totalItems = lstTmp.Count();
                int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);
                // if (pageinfo.pagesize <= lstTmp.Count())
                if (quantity > 0)
                {
                    if (string.IsNullOrEmpty(pageinfo.sortby))
                        products = Mapper.Map<List<ProductSampleModel>>(lstTmp.OrderBy(o => o.tensp).Skip(min).Take(quantity));
                    else
                        products = Mapper.Map<List<ProductSampleModel>>(lstTmp.Skip(min).Take(quantity));
                }

                // products = products.GetRange(min, quantity);

                lstInfo.listTable = new List<Prodcut_Branches_PriceChannel>();
                lstInfo.startItem = min;

                foreach (var item in products)
                {

                    var stocks = Mapper.Map<List<Product_StockModel>>(_context.soft_Branches_Product_Stock.Where(o => o.ProductId == item.id).ToList());
                    var stock = stocks.FirstOrDefault(o => o.BranchesId == User.BranchesId);
                    if (stock != null)
                        item.Stock_Total = stock.Stock_Total;

                    item.Stock_Sum = stocks.Sum(o => o.Stock_Total);

                    var productInfo = new Prodcut_Branches_PriceChannel
                    {
                        //product_price = price ?? price,
                        product_stock = stock ?? stock,
                        product = item,
                        product_stocks = stocks
                    };

                    var prices = _context.soft_Channel_Product_Price.Where(o => o.ProductId == item.id).ToList();

                    if (prices != null && prices.Count > 0)
                    {
                        var PriceMainStore = prices.FirstOrDefault(o => o.soft_Channel.Type == (int)TypeChannel.IsMainStore);
                        if (PriceMainStore != null)
                            item.PriceMainStore = PriceMainStore.Price;

                        var PriceSi = prices.FirstOrDefault(o => o.soft_Channel.Type == (int)TypeChannel.IsChannelWholesale);
                        if (PriceSi != null)
                            item.PriceWholesale = PriceSi.Price;


                        var price = Mapper.Map<Product_PriceModel>(prices.FirstOrDefault(o => o.ChannelId == User.ChannelId));
                        productInfo.product_price = price;
                    }

                    lstInfo.listTable.Add(productInfo);
                }

                Messaging.Data = lstInfo;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách sản phẩm không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Products_Price })]
        [HttpPost]
        public JsonResult ChangePrice(List<Product_PriceModel> model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                var lstChangePrice = new List<int>();

                foreach (var item in model)
                {

                    var pricechannel = _context.soft_Channel_Product_Price.FirstOrDefault(o => o.ProductId == item.ProductId && o.ChannelId == User.ChannelId);
                    if (pricechannel != null)
                    {
                        if (pricechannel.Price != item.Price)
                        {
                            lstChangePrice.Add(item.ProductId);

                             var data = new soft_Channel_Product_Price
                            {
                                Id = pricechannel.Id,
                                Price = item.PriceChange,
                                DateUpdate = DateTime.Now,
                                EmployeeUpdate = User.UserId,
                            };

                            _crud.Update<soft_Channel_Product_Price>(data, o => o.Price, o => o.DateUpdate, o => o.EmployeeUpdate);
                        }
                    }
                    else
                    {
                        lstChangePrice.Add(item.ProductId);

                        var data = new soft_Channel_Product_Price
                        {
                            Price = item.PriceChange,
                            ChannelId = User.ChannelId,
                            ProductId = item.ProductId,
                            DateCreate = DateTime.Now,
                            EmployeeCreate = User.UserId

                        };
                        _crud.Add<soft_Channel_Product_Price>(data);
                    }
                }

                _crud.SaveChanges();

                foreach(var item in lstChangePrice)
                {
                    var user = Mapper.Map<UserCurrent>(User);
                    var product = _unitOW.ProductRepository.FindBy(o => o.id == item).FirstOrDefault();
                    _IOrderBus.UpdatePriceWholesale(product, user, true);
                }

                Messaging.messaging = "Đã thay đổi giá thành công!";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Thay đổi giá không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Products })]
        public JsonResult UpdateProduct(ProductSampleModel model)
        {
            var Messaging = new RenderMessaging();
            Messaging.messaging = "Cập nhật thành công.";
            try
            {
                if (User == null || User.UserId < 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var productObj = _context.shop_sanpham.Find(model.id);

                if (productObj != null)
                {

                    model.masp = model.masp.Replace(" ", "");

                    var msError = Validate_Product(model);
                    if (msError != null)
                        return Json(msError, JsonRequestBehavior.AllowGet);

                    var data = new shop_sanpham
                    {
                        id = model.id,
                        tensp = model.tensp,
                        masp = model.masp,
                        Barcode = model.Barcode,
                        PriceBase_Old = model.PriceBase_Old,
                        PriceBase = model.PriceBase,
                        PriceCompare = model.PriceCompare,
                        CatalogId = model.CatalogId <= 0 ? (int?)null : model.CatalogId,
                        SuppliersId = model.SuppliersId <= 0 ? (int?)null : model.SuppliersId,
                        Status = model.Status,
                        Note = model.Note,
                        StatusVAT = model.StatusVAT
                    };

                    _crud.Update<shop_sanpham>(data, o => o.SuppliersId,
                        o => o.StatusVAT,
                        o => o.tensp,
                        o => o.PriceBase,
                        o => o.PriceBase_Old,
                        o => o.CatalogId, o => o.PriceCompare,
                        o => o.Barcode, o => o.masp,
                        o => o.Status, o => o.Note);

                    _crud.SaveChanges();

                    if (productObj.PriceBase != model.PriceBase)
                    {
                        var user = Mapper.Map<UserCurrent>(User);
                        productObj = _context.shop_sanpham.Find(model.id);
                        _IOrderBus.UpdatePriceWholesale(productObj, user, true);
                    }

                    Messaging.isError = false;
                    Messaging.messaging = "Cập nhật sản phẩm thành công!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Sản phẩm không tồn tại!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật sản phẩm không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Products })]
        public JsonResult CreateProduct(ProductSampleModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.UserId < 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                model.masp = model.masp.Replace(" ", "");

                var msError = Validate_Product(model);
                if (msError != null)
                    return Json(msError, JsonRequestBehavior.AllowGet);

                var data = new shop_sanpham
                {
                    tensp = model.tensp,
                    masp = model.masp,
                    Barcode = model.Barcode,
                    PriceCompare = 0,
                    PriceBase = 0,
                    PriceBase_Old = 0,
                    PriceInput = 0,
                    hide = false,
                    Status = model.Status,
                    Note = model.Note,
                    DateCreate = DateTime.Now
                };

                _crud.Add<shop_sanpham>(data);

                _crud.SaveChanges();

                Messaging.isError = false;
                Messaging.messaging = "Thêm sản phẩm thành công.";

            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo sản phẩm không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Delete_Products })]
        public JsonResult DeleteProduct(long id)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.UserId < 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (id <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var product = _context.shop_sanpham.Find(id);

                if (product == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Sản phẩm này đã được xóa.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                _context.shop_sanpham.Remove(product);

                _context.SaveChanges();

                Messaging.isError = false;
                Messaging.messaging = "Xóa sản phẩm thành công.";

            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Sản phẩm này hiện đang sử dụng không xóa được, vui lòng kiểm tra các ràng buộc!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetPriceByChannel(int ProductId)
        {
            var Messaging = new RenderMessaging();
            try
            {
                var rs = 0;
                var priceChannel = _context.soft_Channel_Product_Price.FirstOrDefault(o => o.ProductId == ProductId && o.ChannelId == User.ChannelId);
                if (priceChannel != null)
                    rs = priceChannel.Price;
                Messaging.Data = rs;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị giá theo kênh không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Research(string keyword)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    //  var lstTmp = from product in _context.shop_sanpham select product;

                    var products = _context.shop_sanpham.Select(product => new ProductSampleModel
                    {
                        id = product.id,
                        masp = product.masp,
                        Barcode = product.Barcode,
                        tensp = product.tensp,

                        SuppliersName = product.soft_Suppliers.Name,
                        Stock_Total = product.soft_Branches_Product_Stock.FirstOrDefault(o => o.BranchesId == User.BranchesId && o.ProductId == product.id) != null ? product.soft_Branches_Product_Stock.FirstOrDefault(o => o.BranchesId == User.BranchesId && o.ProductId == product.id).Stock_Total : 0,

                        PriceCompare = product.PriceCompare.HasValue ? product.PriceCompare.Value : 0,
                        PriceBase = product.PriceBase.HasValue ? product.PriceBase.Value : 0,
                        PriceBase_Old = product.PriceBase_Old.HasValue ? product.PriceBase_Old.Value : 0,
                        PriceInput = product.PriceInput.HasValue ? product.PriceInput.Value : 0,
                        PriceChannel = product.soft_Channel_Product_Price.FirstOrDefault(o => o.ChannelId == User.ChannelId) != null ? product.soft_Channel_Product_Price.FirstOrDefault(o => o.ChannelId == User.ChannelId).Price : 0,




                        // Mapper.Map<List<Channel_Product_PriceModel>>(product.soft_Channel_Product_Price.Where(o => o.ProductId == product.id).Select(o => o.Id).ToList())
                    });



                    //var products = lstTmp.ToList();// Mapper.Map<List<ProductSampleModel>>(lstTmp.ToList());
                    #region Search
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        keyword = keyword.ToLower();
                        products = products.Where(o =>
                           (!string.IsNullOrEmpty(o.tensp) && o.tensp.ToLower().Contains(keyword))
                         || (!string.IsNullOrEmpty(o.Barcode) && o.Barcode.ToLower().Contains(keyword))
                         || (!string.IsNullOrEmpty(o.masp) && o.masp.ToLower().Contains(keyword))
                        );
                    }


                    #endregion
                    var result = products.ToList();

                    foreach (var item in result)
                    {
                        var channelprice = _context.soft_Channel_Product_Price.Where(o => o.ProductId == item.id).Select(a => new Order_PriceChannelsModel
                        {

                            Id = a.ChannelId,
                            Price = a.Price,
                            Channel = a.soft_Channel.Channel,
                            Enddate_Discount = a.Enddate_Discount.HasValue ? a.Enddate_Discount.Value : default(DateTime),
                            StartDate_Discount = a.StartDate_Discount.HasValue ? a.StartDate_Discount.Value : default(DateTime),
                            Price_Discount = a.Price_Discount.HasValue ? a.Price_Discount.Value : 0
                        }).ToList();

                        item.PriceChannels = channelprice;
                    }

                    Messaging.Data = result;
                }

            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tìm kiếm sản phẩm có lỗi!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        private RenderMessaging Validate_Product(ProductSampleModel model)
        {
            var Messaging = new RenderMessaging();
            if (model.masp.Length >= 9)
            {
                Messaging.isError = true;
                Messaging.messaging = "Code không được lớn 10 ký tự!";
                return Messaging;
            }
            if (string.IsNullOrEmpty(model.tensp))
            {
                Messaging.isError = true;
                Messaging.messaging = "Tên sản phẩm không được trống.";
                return Messaging;
            }
            if (model.id <= 0)
            {
                var productexits = _context.shop_sanpham.FirstOrDefault(o => o.masp.Equals(model.masp));
                if (productexits != null)
                {

                    Messaging.isError = true;
                    Messaging.messaging = "Mã sản phẩm đã tồn tại.";
                    return Messaging;
                }
            }
            return null;
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Products_Price_Discount })]
        [HttpPost]
        public JsonResult ChangePrice_Discount(List<Product_PriceModel> model)
        {
            var Messaging = new RenderMessaging();
            try
            {

                foreach (var item in model)
                {
                    foreach (var channel in item.Channels)
                    {
                        var newobj = item;
                        newobj.ChannelId = channel.Id;
                        var errr = UpdatePrice_Discount(newobj);
                        if (!string.IsNullOrEmpty(errr))
                        {
                            {
                                Messaging.isError = true;
                                Messaging.messaging = errr;
                                return Json(Messaging, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                _crud.SaveChanges();
                Messaging.messaging = "Đã thay đổi giá khuyến mãi thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Thay đổi giá khuyến mãi không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        private string UpdatePrice_Discount(Product_PriceModel item)
        {

            if (item.PriceChange <= 0)
            {
                return "Giá giảm không hợp lệ!";
            }

            if (item.ChannelId <= 0)
            {
                return "Vui lòng chọn Kênh muốn giảm giá.";
            }

            if (item.StartDate_Discount > item.Enddate_Discount
               || item.StartDate_Discount < DateTime.Now
               )
            {
                return "Ngày áp dụng không hợp lệ!";
            }


            item.StartDate_Discount = new DateTime(item.StartDate_Discount.Year, item.StartDate_Discount.Month, item.StartDate_Discount.Day, 0, 0, 0, 0);
            item.Enddate_Discount = new DateTime(item.Enddate_Discount.Year, item.Enddate_Discount.Month, item.Enddate_Discount.Day, 23, 59, 59, 999);

            var pricechannel = _context.soft_Channel_Product_Price.FirstOrDefault(o => o.ProductId == item.ProductId && o.ChannelId == item.ChannelId);
            if (pricechannel != null)
            {
                var data = new soft_Channel_Product_Price
                {
                    Id = pricechannel.Id,
                    Price_Discount = item.PriceChange,
                    StartDate_Discount = item.StartDate_Discount,
                    Enddate_Discount = item.Enddate_Discount,
                    DateUpdate = DateTime.Now,
                    EmployeeUpdate = User.UserId,
                };


                _crud.Update<soft_Channel_Product_Price>(data, o => o.Price_Discount,
                    o => o.StartDate_Discount,
                    o => o.Enddate_Discount,
                    o => o.DateUpdate,
                    o => o.EmployeeUpdate);
            }
            else
            {
                var data = new soft_Channel_Product_Price
                {
                    //Price = item.PriceChange,
                    ChannelId = item.ChannelId,
                    ProductId = item.ProductId,

                    Price_Discount = item.PriceChange,
                    StartDate_Discount = item.StartDate_Discount,
                    Enddate_Discount = item.Enddate_Discount,


                    DateCreate = DateTime.Now,
                    EmployeeCreate = User.UserId

                };
                _crud.Add<soft_Channel_Product_Price>(data);
            }
            return string.Empty;
        }

    }
}