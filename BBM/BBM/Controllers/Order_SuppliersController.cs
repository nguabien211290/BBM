using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Infractstructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class Order_SuppliersController : BaseController
    {
        private CRUD _crud;
        private admin_softbbmEntities _context;
        public Order_SuppliersController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Suppliers })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/OrderSuppliers/_Channel_Order_Suppliers_List.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Suppliers })]
        public ActionResult RenderViewCreate(int orderId = 0)
        {

            if (orderId > 0)
            {
                var order = _context.soft_Order.FirstOrDefault(o => o.Id == orderId);

                if (order != null)
                {
                    var rs = new List<Order_InputTmpModel>();
                    foreach (var item in order.soft_Order_Child)
                    {
                        if (item.shop_sanpham.id > 0)
                        {
                            var product = _context.shop_sanpham.Find(item.shop_sanpham.id);
                            if (product != null)
                            {
                                var stock = _context.soft_Branches_Product_Stock.FirstOrDefault(o => o.BranchesId == User.BranchesId && o.ProductId == product.id);
                                rs.Add(new Business.Models.Module.Order_InputTmpModel
                                {
                                    Code = item.shop_sanpham.masp,
                                    ProductId = (long)item.shop_sanpham.id,
                                    ProductName = item.shop_sanpham.tensp,
                                    Price = item.Price,
                                    Total = item.Total,
                                    PriceBase = product.PriceBase.HasValue ? product.PriceBase.Value : 0,
                                    SuppliersName = product.soft_Suppliers != null ? product.soft_Suppliers.Name : "",
                                    PriceCompare = product.PriceCompare.HasValue ? product.PriceCompare.Value : 0,
                                    Stock_Total = stock != null ? stock.Stock_Total : 0
                                });
                            }
                        }
                    }
                    ViewBag.Products = Newtonsoft.Json.JsonConvert.SerializeObject(rs);
                }
            }
            ViewBag.UserName = User.UserName;
            return PartialView("~/Views/Shared/Partial/module/Order/OrderSuppliers/_Channel_Order_Suppliers_Create.cshtml");
        }
        public JsonResult GetOrder_Suppliers(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var clientMatters = _context.soft_Order.Where(o =>
                              o.TypeOrder == (int)TypeOrder.OrderProduct && o.Id_From.HasValue && o.Id_From == User.BranchesId)
                              .OrderByDescending(o => o.DateCreate);


                var lstTmp = clientMatters.AsEnumerable().Select(o => new OrderModel
                {
                    Id = o.Id,
                    EmployeeCreate = o.EmployeeCreate,
                    EmployeeUpdate = o.EmployeeUpdate,
                    DateUpdate = o.DateUpdate,
                    DateCreate = o.DateCreate,
                    Id_From = o.Id_From.HasValue ? o.Id_From.Value : 0,
                    Id_To = o.Id_To.HasValue ? o.Id_To.Value : 0,
                    Note = o.Note,
                    Status = o.Status,
                    Total = o.Total.HasValue ? (int)o.Total.Value : 0,
                    Detail = Mapper.Map<List<Order_DetialModel>>(o.soft_Order_Child)
                });

                #region Fillter
                if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
                {
                    foreach (var item in pageinfo.filterby)
                    {
                        var key = 0;
                        switch (item.Fiter)
                        {
                            case "Status":
                                key = int.Parse(item.Value);
                                lstTmp = lstTmp.Where(o => o.Status > 0 && o.Status.Equals(key));
                                break;
                            case "Time_To_From":
                                var StartDate = new DateTime(item.StartDate.Year, item.StartDate.Month, item.StartDate.Day, 0, 0, 0, 0); //item.StartDate.Date;
                                var EndDate = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 59, 999);// item.EndDate.Date.AddDays(1);
                                lstTmp = lstTmp.Where(o => o.DateCreate >= StartDate && o.DateCreate <= EndDate);
                                break;
                            case "Suppliers":
                                key = int.Parse(item.Value);
                                var producIds_suppliers = _context.shop_sanpham.Where(o => o.soft_Suppliers.SuppliersId == key).Select(o => (long)o.id);
                                var orderChild = _context.soft_Order_Child.Where(o => producIds_suppliers.Contains(o.ProductId.Value)).Select(o => o.OrderId);
                                lstTmp = lstTmp.Where(o => orderChild.Contains(o.Id));
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
                        case "ChannelsTo":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Id_To);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Id_To);
                            break;
                        case "ChannelsFrom":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Id_From);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Id_From);
                            break;
                        case "DateCreate":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.DateCreate);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.DateCreate);
                            break;
                        case "Total":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Total);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Total);
                            break;
                        case "Status":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Status);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Status);
                            break;
                        case "Id":
                            if (pageinfo.sortbydesc)
                                lstTmp = lstTmp.OrderByDescending(o => o.Id);
                            else
                                lstTmp = lstTmp.OrderBy(o => o.Id);
                            break;
                    }

                }
                #endregion

                Channel_Paging<OrderModel> lstInfo = new Channel_Paging<OrderModel>();

                lstInfo.totalItems = lstTmp.Count();

                int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                var orderSuppliers = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

                foreach (var item in orderSuppliers)
                {
                    var lstSup = new List<int>();

                    foreach (var pro in item.Detail)
                    {
                        if (pro.Product != null && pro.Product.soft_Suppliers != null)
                        {
                            var checkHas = lstSup.FirstOrDefault(o => o == pro.Product.soft_Suppliers.SuppliersId);
                            if (checkHas <= 0)
                            {
                                item.Name_To += " - " + pro.Product.soft_Suppliers.Name;

                                lstSup.Add(pro.Product.soft_Suppliers.SuppliersId);
                            }
                        }
                    }
                }

                lstInfo.listTable = orderSuppliers;
                lstInfo.startItem = min;

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách phiếu đặt hàng nhà phân phối không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateOrder_Suppliers_Print(OrderModel model)
        {
            var Messaging = new RenderMessaging();
            var result = new List<OrderChannel_GroupModel>();
            try
            {
                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in model.Detail)
                {
                    var product = _context.shop_sanpham.Find(item.ProductId);
                    if (product == null)
                    {
                        Messaging.isError = true;
                        Messaging.messaging = "Sản phẩm không tồn tại, vui lòng thử lại.";
                        return Json(Messaging, JsonRequestBehavior.AllowGet);
                    }
                    item.Product = Mapper.Map<ProductSampleModel>(product);

                }

                var product_suppliers = model.Detail.GroupBy(o => o.Product.SuppliersId).Select(pro => new OrderChannel_GroupModel
                {
                    products = Mapper.Map<List<Order_DetialModel>>(pro.ToList()),
                    suppliers = Mapper.Map<SuppliersModel>(pro.FirstOrDefault().Product.soft_Suppliers)
                }).ToList();

                Messaging.Data = product_suppliers;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo đơn hàng nhà phân phối không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Suppliers })]
        [HttpPost]
        public JsonResult CreateOrder_Suppliers(OrderModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in model.Detail)
                {
                    var product = _context.shop_sanpham.Find(item.ProductId);
                    if (product == null)
                    {
                        Messaging.isError = true;
                        Messaging.messaging = "Sản phẩm không tồn tại, vui lòng thử lại.";
                        return Json(Messaging, JsonRequestBehavior.AllowGet);
                    }
                }

                var objOrder = Mapper.Map<soft_Order>(model);
                objOrder.Status = (int)StatusOrder_Suppliers.Process;
                objOrder.DateCreate = DateTime.Now;
                objOrder.EmployeeCreate = User.UserId;
                objOrder.TypeOrder = (int)TypeOrder.OrderProduct;
                objOrder.Id_From = User.BranchesId;
                objOrder.Id_To = null;
                foreach (var item in objOrder.soft_Order_Child)
                {
                    item.Status = (int)StatusOrder_Suppliers.Process;
                }

                _crud.Add<soft_Order>(objOrder);
                _crud.SaveChanges();
                Messaging.messaging = "Đã tạo phiếu đặt hàng.";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo phiếu đặt hàng nhà phân phối không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Order_Suppliers })]
        [HttpPost]
        public JsonResult UpdateDone_Order_Suppliers(int id)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var order = _context.soft_Order.Find(id);

                if (order == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Không tìm thấy đơn hàng này.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var objOrder = new soft_Order
                {
                    Id = id,
                    Status = (int)StatusOrder_Suppliers.Done,
                    DateUpdate = DateTime.Now,
                    EmployeeUpdate = User.UserId
                };

                _crud.Update<soft_Order>(objOrder, o => o.Status, o => o.EmployeeUpdate, o => o.DateUpdate);

                foreach (var item in order.soft_Order_Child)
                {
                    var objOrderChild = new soft_Order_Child
                    {
                        Id = item.Id,
                        Status = (int)StatusOrder_Suppliers.Done,
                        OrderId = item.OrderId,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Total = item.Total
                    };
                    _crud.Update<soft_Order_Child>(objOrderChild, o => o.Status);
                }
               _crud.SaveChanges();

            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật phiếu đặt hàng nhà phân phối không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}
