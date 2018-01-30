using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace BBM.Controllers
{
    public class PartialController : BaseController
    {
        private admin_softbbmEntities _context;
        private CRUD _crud;

        public PartialController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }


        public ActionResult _navbar()
        {
            ViewBag.UserName = User.UserName;
            return PartialView("~/Views/Shared/Partial/module/Partial/_navbar.cshtml");
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
                    //case (int)GroupRolesEnum.Administrator:
                    //var enumAdmin = lstEnumRole.Where(o =>
                    //    o == RolesEnum.Administrator).ToList();

                    //foreach (var role in enumAdmin)
                    //{
                    //    group.Roles.Add(new Models.Module.RoleEnumModel
                    //    {
                    //        Name = EnumHelper<RolesEnum>.GetDisplayValuebyInt((int)role),
                    //        Id = (int)role,
                    //        isSelect = false
                    //    });
                    //}
                    //break;
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

        public JsonResult GetProductby(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                }

                var lstTmp = _context.shop_sanpham.Select(o => new ProductSampleModel
                {
                    Barcode = o.Barcode,
                    tensp = o.tensp,
                    masp = o.masp,
                    CatalogId = o.CatalogId.HasValue ? o.CatalogId.Value : 0,
                    SuppliersId = o.SuppliersId.HasValue ? o.SuppliersId.Value : 0,
                    id = o.id,
                    Img = o.shop_image.FirstOrDefault().url
                });


                // IQueryable<shop_sanpham> lstTmp = _context.shop_sanpham;

                #region Fillter
                if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
                {
                    foreach (var item in pageinfo.filterby)
                    {
                        var key = int.Parse(item.Value);
                        if (key != 0)
                        {
                            switch (item.Fiter)
                            {
                                case "Catalog":
                                    lstTmp = lstTmp.Where(o => o.CatalogId > 0 && o.CatalogId.Equals(key));
                                    break;
                                case "Suppliers":
                                    lstTmp = lstTmp.Where(o => o.SuppliersId > 0 && o.SuppliersId.Equals(key));
                                    break;
                                case "Unit":
                                    lstTmp = lstTmp.Where(o => o.UnitId > 0 && o.UnitId.Equals(key));
                                    break;
                                case "Order_Channel":
                                    var id_form = int.Parse(item.Value);

                                    var branches = _context.soft_Branches.Find(int.Parse(item.Value));

                                    if (branches == null)
                                        break;

                                    var lstchannl = branches.soft_Channel.Select(o => o.Id).ToList();

                                    var order = _context.soft_Order_Child.Where(o =>
                                        (o.soft_Order.DateCreate >= item.StartDate
                                        || o.soft_Order.DateCreate <= item.EndDate)
                                        && o.soft_Order.Id_From.HasValue
                                        && lstchannl.Contains(o.soft_Order.Id_From.Value)
                                        && o.soft_Order.TypeOrder == (int)TypeOrder.Sale).ToList();
                                    //&& o.soft_Order.Id_From.Value == id_form

                                    if (order.Count > 0)
                                    {
                                        var lstId = order.Select(o => o.ProductId).ToList();

                                        lstTmp = lstTmp.Where(o => lstId.Contains(o.id));
                                    }
                                    else
                                    {
                                        Channel_Paging<ProductSampleModel> lstInfo1 = new Channel_Paging<ProductSampleModel>();
                                        lstInfo1.listTable = new List<ProductSampleModel>();
                                        Messaging.Data = lstInfo1;
                                        return Json(Messaging, JsonRequestBehavior.AllowGet);
                                    }
                                    break;
                            }
                        }

                    }
                }
                #endregion
                #region Sort
                if (!string.IsNullOrEmpty(pageinfo.sortby))
                {
                    switch (pageinfo.sortby)
                    {
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
                        case "ProductName":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.tensp);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.tensp);
                            break;
                        case "Code":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.masp);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.masp);
                            break;
                    }

                }
                #endregion
                var products = new List<ProductSampleModel>();

                #region Search
                if (!string.IsNullOrEmpty(pageinfo.keyword))
                {
                    pageinfo.keyword = pageinfo.keyword.ToLower();
                    var querySearch = pageinfo.keyword.Split(' ');
                    foreach (var str in querySearch)
                    {
                        var strtmp = str.ToLower();
                        if (!string.IsNullOrWhiteSpace(strtmp))
                        {
                            lstTmp = lstTmp.Where(o =>
                                     (!string.IsNullOrEmpty(o.tensp) && o.tensp.ToLower().Contains(strtmp))
                                     || (!string.IsNullOrEmpty(o.Barcode) && o.Barcode.ToLower().Contains(strtmp))
                                     || (!string.IsNullOrEmpty(o.masp) && o.masp.ToLower().Contains(strtmp)));
                        }
                    }
                }
                #endregion

                Channel_Paging<ProductSampleModel> lstInfo = new Channel_Paging<ProductSampleModel>();

                int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                lstInfo.totalItems = lstTmp.Count();
                int quantity = Helpers.GetQuantity(lstInfo.totalItems, pageinfo.pageindex, pageinfo.pagesize);

                if (quantity > 0)
                {
                    if (string.IsNullOrEmpty(pageinfo.sortby))
                        products = lstTmp.OrderByDescending(o => o.id).Skip(min).Take(quantity).ToList();// Mapper.Map<List<ProductSampleModel>>(lstTmp.OrderByDescending(o => o.id).Skip(min).Take(quantity).ToList());
                    else
                        products = lstTmp.Skip(min).Take(quantity).ToList();// Mapper.Map<List<ProductSampleModel>>(lstTmp.Skip(min).Take(quantity));
                }


                foreach (var item in products)
                {
                    var stock = _context.soft_Branches_Product_Stock.FirstOrDefault(o => o.ProductId == item.id && o.BranchesId == User.BranchesId);
                    if (stock != null)
                        item.Stock_Total = stock.Stock_Total;

                    if (!string.IsNullOrEmpty(pageinfo.keyword))
                    {
                        var querySearch = pageinfo.keyword.Split(' ');
                        foreach (var str in querySearch)
                        {
                            var queryName = item.tensp.ToLower();
                            if (queryName.Contains(str.ToLower()))
                                item.tensp = item.tensp.Replace(pageinfo.keyword, "<strong>" + pageinfo.keyword + "</strong>");
                            if (!string.IsNullOrEmpty(item.masp))
                            {
                                var queryCode = item.masp.ToLower();
                                if (queryCode.Contains(str.ToLower()))
                                    item.masp = item.masp.Replace(pageinfo.keyword, "<strong>" + pageinfo.keyword + "</strong>");
                            }
                            if (!string.IsNullOrEmpty(item.Barcode))
                            {
                                var queryBarcode = item.Barcode.ToLower();
                                if (queryBarcode.Contains(str.ToLower()))
                                    item.Barcode = item.Barcode.Replace(pageinfo.keyword, "<strong>" + pageinfo.keyword + "</strong>");
                            }
                        }

                    }
                }
                // products = products.GetRange(min, quantity);

                lstInfo.startItem = min;
                lstInfo.listTable = products;

                Messaging.Data = lstInfo;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tìm kiếm sản phẩm có lỗi!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductbyId(int productId)
        {
            var note = string.Empty;
            var Messaging = new RenderMessaging();

            var product = _context.shop_sanpham.Find(productId);

            if (product == null)
            {
                Messaging.Data = new { result = "Sản phẩm không tồn tại." };
                Messaging.isError = true;
                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }

            var orderProduct = _context.soft_Order_Child.Where(o => o.ProductId == productId).ToList();

            var orderedSup = orderProduct.FirstOrDefault(o =>
                                                        o.soft_Order.TypeOrder == (int)TypeOrder.OrderProduct
                                                        && o.Status == (int)StatusOrder_Suppliers.Process);
            if (orderedSup != null)
            {
                var productcheck = _context.shop_sanpham.Find(orderedSup.ProductId);
                if (productcheck != null)
                    note += " Đơn hàng số " + orderedSup.OrderId + " đã đặt sản phẩm " + productcheck.tensp + " ngày " + orderedSup.soft_Order.DateCreate + ".";
            }

            var price = Mapper.Map<Product_PriceModel>(_context.soft_Channel_Product_Price.FirstOrDefault(o => o.ProductId == product.id && o.ChannelId == User.ChannelId));

            var stocks = Mapper.Map<List<Product_StockModel>>(_context.soft_Branches_Product_Stock.Where(o => o.ProductId == product.id).ToList());
            var stock = stocks.FirstOrDefault(o => o.BranchesId == User.BranchesId);

            var productInfo = new Prodcut_Branches_PriceChannel
            {
                product_price = price ?? price,
                product_stock = stock ?? stock,
                product = Mapper.Map<ProductSampleModel>(product),
                product_stocks = stocks
            };
            if (product.soft_Suppliers != null)
                productInfo.product.SuppliersName = product.soft_Suppliers.Name;
            if (product.soft_Branches_Product_Stock != null)
            {
                var stockbyBranches = product.soft_Branches_Product_Stock.FirstOrDefault(o => o.BranchesId == User.BranchesId);
                if (stockbyBranches != null)
                    productInfo.product.Stock_Total = stockbyBranches.Stock_Total;
            }
            Messaging.Data = new { result = productInfo, note = note };
            Messaging.isError = false;
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
                    keyword = keyword.ToLower();
                    var products = _context.shop_sanpham
                        .Where(o =>
                           (!string.IsNullOrEmpty(o.tensp) && o.tensp.ToLower().Contains(keyword))
                         || (!string.IsNullOrEmpty(o.Barcode) && o.Barcode.ToLower().Contains(keyword))
                         || (!string.IsNullOrEmpty(o.masp) && o.masp.ToLower().Contains(keyword))
                        )
                        .Select(product => new ProductSampleModel
                        {
                            id = product.id,
                            masp = product.masp,
                            Barcode = product.Barcode,
                            tensp = product.tensp,
                        });

                    var data = products.ToList();

                    foreach (var item in data)
                    {
                        if (!string.IsNullOrEmpty(item.masp) && !string.IsNullOrEmpty(item.tensp))
                        {
                            var queryName = item.tensp.ToLower();
                            if (queryName.Contains(keyword.ToLower()))
                                item.tensp = queryName.Replace(keyword, "<strong>" + keyword + "</strong> ");

                            var queryCode = item.masp.ToLower();
                            if (queryCode.Contains(keyword.ToLower()))
                                item.masp = queryCode.Replace(keyword, "<strong>" + keyword + "</strong> ");
                        }
                    }

                    Messaging.Data = data;
                }

            }
            catch(Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tìm kiếm sản phẩm có lỗi!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductbyIdForOrder(int productId)
        {
            var note = string.Empty;
            var Messaging = new RenderMessaging();

            var product = _context.shop_sanpham.Select(o => new ProductSampleModel
            {
                Barcode = o.Barcode,
                tensp = o.tensp,
                masp = o.masp,
                id = o.id,

            }).FirstOrDefault(o => o.id == productId);

            if (product == null)
            {
                Messaging.Data = new { result = "Sản phẩm không tồn tại." };
                Messaging.isError = true;
                return Json(Messaging, JsonRequestBehavior.AllowGet);
            }


            var price = _context.soft_Channel_Product_Price.FirstOrDefault(o =>
            o.ProductId == product.id
            && o.ChannelId == User.ChannelId);

            product.PriceChannel = 0;

            if (price != null)
            {
                if (price.StartDate_Discount.HasValue
                    && price.Enddate_Discount.HasValue
                    && price.Price_Discount > 0)
                {
                    var dateNow = DateTime.Now;

                    if (dateNow >= price.StartDate_Discount && dateNow < price.Enddate_Discount)
                    {
                        product.PriceChannel = price.Price_Discount.Value;
                    }
                    else
                    {
                        if (price.Price > 0)
                            product.PriceChannel = price.Price;
                    }
                }
                else
                {
                    if (price.Price > 0)
                        product.PriceChannel = price.Price;
                }
            }

            //if (price != null && price.Price > 0)
            //    product.PriceChannel = price.Price;
            //else
            //    product.PriceChannel = 0;

            var productInfo = new Prodcut_Branches_PriceChannel
            {
                product = product
            };

            Messaging.Data = new { result = productInfo };
            Messaging.isError = false;
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}
