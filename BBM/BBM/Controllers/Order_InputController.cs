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
using BBM.Business.Logic;
using BBM.Business.Model.Module;
using BBM.Infractstructure.Security;
using BBM.Business.Repository;
using System.Threading.Tasks;

namespace BBM.Controllers
{
    public class Order_InputController : BaseController
    {
        //
        // GET: /Order_Input/
        private CRUD _crud;
        private admin_softbbmEntities _context;
        private IOrderBusiness _IOrderBus;
        private IUnitOfWork _unitOW;
        public Order_InputController(IOrderBusiness IOrderBus, IUnitOfWork unitOW)
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
            _IOrderBus = IOrderBus;
            _unitOW = unitOW;

        }
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Input })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/OrderInut/_Channel_Order_Input_List.cshtml");
        }


        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Input })]
        public ActionResult RenderViewCreate(int orderSuppliersId = 0, int orderId = 0)
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

                ViewBag.OrderSuppliersId = orderSuppliersId;
            }
            return PartialView("~/Views/Shared/Partial/module/Order/OrderInut/_Channel_Order_Input_Create.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Input })]
        public JsonResult GetOrder_Inside(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                Channel_Paging<OrderModel> lstInfo = new Channel_Paging<OrderModel>();

                int count, min = 0;

                lstInfo.listTable = _IOrderBus.GetOrder_Input(pageinfo, User.BranchesId, out count, out min);

                lstInfo.totalItems = count;

                lstInfo.startItem = min;

                var sys_Employee = _unitOW.EmployeeRepository.GetAll().ToList();

                var soft_Branches = _unitOW.BrachesRepository.GetAll().ToList();

                foreach (var item in lstInfo.listTable)
                {
                    var userCreate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeCreate);
                    item.EmployeeNameCreate = userCreate.Name + "(" + userCreate.Email + ")";

                    if (item.EmployeeUpdate.HasValue)
                    {
                        var userUpdate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeUpdate);
                        item.EmployeeNameUpdate = userUpdate.Name + "(" + userUpdate.Email + ")";
                    }

                    var lstSup = new List<int>();

                    if (item.Id_From > 0)
                    {
                        var branch = soft_Branches.FirstOrDefault(o => o.BranchesId == item.Id_From);
                        if (branch != null)
                            item.Name_From = branch.BranchesName;
                        else
                        {
                            var orderSuppliers = _unitOW.OrderInputRepository.FindBy(o => o.Id == item.Id_From).FirstOrDefault();
                            if (orderSuppliers != null)
                            {
                                item.Name_From = "ĐH NPP";
                                foreach (var pro in item.Detail)
                                {
                                    if (pro.Product != null && pro.Product.soft_Suppliers != null)
                                    {
                                        var checkHas = lstSup.FirstOrDefault(o => o == pro.Product.soft_Suppliers.SuppliersId);
                                        if (checkHas <= 0)
                                        {
                                            item.Name_From += " - " + pro.Product.soft_Suppliers.Name;

                                            lstSup.Add(pro.Product.soft_Suppliers.SuppliersId);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (item.Id_To > 0)
                    {
                        var branch = soft_Branches.FirstOrDefault(o => o.BranchesId == item.Id_To);
                        if (branch != null)
                            item.Name_To = branch.BranchesName;
                    }
                }

                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị đơn hàng nhập không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Input })]
        public async Task<JsonResult> CreateOrder_Inside(OrderModel model, bool isDone = true, int OrderSuppliersId = 0)
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

                if (model.Detail.Count <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var user = Mapper.Map<UserCurrent>(User);

                var isError = await _IOrderBus.AddOrder_Input(model, user, isDone, OrderSuppliersId);

                Messaging.isError = !isError;
                Messaging.messaging = "Đã Nhập hàng thành công!";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Nhập hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Order_Input })]
        [HttpPost]
        public JsonResult UpdateDone_Order_Inside(int id)
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
                    Status = (int)StatusOrder_Input.Done,
                    DateUpdate = DateTime.Now,
                    EmployeeUpdate = User.UserId
                };

                var modelupdate = Mapper.Map<OrderModel>(order);

                if (order.Id_From > 0)
                    UpdateOrderSuppliers(modelupdate, order.Id_From.Value);

                _crud.Update(objOrder, o => o.Status, o => o.EmployeeUpdate, o => o.DateUpdate);

                UpdateStockByBranches(modelupdate);

                _crud.SaveChanges();

            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật đơn hàng nhập không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        private void UpdateStockByBranches(OrderModel order)
        {
            foreach (var item in order.Detail)
            {
                var StockProduct = _context.soft_Branches_Product_Stock.Where(o => o.ProductId == item.ProductId).ToList();

                if (order.TypeOrder == (int)TypeOrder.Input)
                {
                    #region Product
                    var product = _context.shop_sanpham.Find(item.ProductId);
                    if (product != null)
                    {
                        var productObj = new shop_sanpham
                        {
                            id = product.id,
                            PriceBase_Old = product.PriceBase,
                            PriceInput = (int)item.Price,
                            PriceCompare = item.PriceCompare
                        };

                        var stockbySum = StockProduct != null && StockProduct.Count > 0 ? StockProduct.Sum(o => o.Stock_Total) : 0;

                        var pricebase_old = product.PriceBase != null ? product.PriceBase : 0;

                        var chia = (item.Total + stockbySum);

                        productObj.PriceBase = (int)(((stockbySum * pricebase_old) + (item.Total * (int)item.Price)) / (chia != 0 ? chia : 1));

                        _crud.Update(productObj, o => o.PriceBase, o => o.PriceInput, o => o.PriceBase_Old, o => o.PriceCompare);
                        #endregion
                        #region Stcok
                        var stockTo = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == order.Id_To);
                        if (stockTo != null)
                        {

                            var newstock = new soft_Branches_Product_Stock
                            {
                                BranchesId = stockTo.BranchesId,
                                ProductId = stockTo.ProductId,
                                Stock_Total = stockTo.Stock_Total + item.Total,
                                DateCreate = stockTo.DateCreate,
                                EmployeeCreate = stockTo.EmployeeCreate,
                                DateUpdate = DateTime.Now,
                                EmployeeUpdate = User.UserId,
                            };
                            _crud.Update<soft_Branches_Product_Stock>(newstock, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                        }
                        else
                        {
                            var stockNewTo = new soft_Branches_Product_Stock
                            {

                                BranchesId = order.Id_To,
                                ProductId = item.ProductId,
                                Stock_Total = item.Total,
                                DateCreate = DateTime.Now,
                                EmployeeCreate = order.EmployeeCreate
                            };
                            _crud.Add<soft_Branches_Product_Stock>(stockNewTo);
                        }
                        #endregion
                    }
                }

                if (order.TypeOrder == (int)TypeOrder.Output)
                {
                    var stockTo = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == order.Id_To);
                    var stockFrom = StockProduct.FirstOrDefault(o => o.ProductId == item.ProductId && o.BranchesId == order.Id_From);

                    if (stockTo != null)
                    {

                        var newstock = new soft_Branches_Product_Stock
                        {
                            BranchesId = stockTo.BranchesId,
                            ProductId = stockTo.ProductId,
                            Stock_Total = stockTo.Stock_Total + item.Total,
                            DateCreate = stockTo.DateCreate,
                            EmployeeCreate = stockTo.EmployeeCreate,
                            DateUpdate = DateTime.Now,
                            EmployeeUpdate = User.UserId,
                        };
                        _crud.Update<soft_Branches_Product_Stock>(newstock, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                    }
                    else
                    {
                        stockTo = new soft_Branches_Product_Stock
                        {
                            ProductId = item.ProductId,
                            BranchesId = User.BranchesId,
                            DateCreate = DateTime.Now,
                            Stock_Total = item.Total,
                            EmployeeCreate = User.UserId
                        };
                        _crud.Add(stockTo);
                    }

                    if (stockFrom != null)
                    {

                        var newstock = new soft_Branches_Product_Stock
                        {
                            BranchesId = stockFrom.BranchesId,
                            ProductId = stockFrom.ProductId,
                            Stock_Total = stockFrom.Stock_Total - item.Total,
                            DateCreate = stockFrom.DateCreate,
                            EmployeeCreate = stockFrom.EmployeeCreate,
                            DateUpdate = order.DateCreate,
                            EmployeeUpdate = order.EmployeeCreate,
                        };
                        _crud.Update(newstock, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                    }
                    else
                    {
                        var stockNewTo = new soft_Branches_Product_Stock
                        {

                            BranchesId = order.Id_From,
                            ProductId = item.ProductId,
                            Stock_Total = 0 - item.Total,
                            DateCreate = order.DateCreate,
                            EmployeeCreate = order.EmployeeCreate,
                            DateUpdate = null,
                            EmployeeUpdate = null
                        };
                        _crud.Add<soft_Branches_Product_Stock>(stockNewTo);
                    }
                }
            }
        }

        private void UpdateOrderSuppliers(OrderModel model, int OrderSuppliersId)
        {
            var orderSuppliers = _context.soft_Order.FirstOrDefault(o => o.Id.Equals(OrderSuppliersId) && o.TypeOrder == (int)TypeOrder.OrderProduct && o.Status == (int)StatusOrder_Input.Process);

            if (orderSuppliers != null)
            {
                foreach (var item in orderSuppliers.soft_Order_Child)
                {
                    if (item.Status == null || item.Status == (int)StatusOrder_Input.Process)
                    {
                        var hasProduct = model.Detail.FirstOrDefault(o => o.ProductId == item.ProductId.Value);

                        if (hasProduct != null)
                        {
                            if (item.Total <= hasProduct.Total)
                            {
                                item.Status = (int)StatusOrder_Input.Done;
                                var newobj = new soft_Order_Child
                                {
                                    Id = item.Id,
                                    Status = item.Status
                                };
                                _crud.Update<soft_Order_Child>(newobj, o => o.Status);
                            }
                            else
                            {
                                var newobj = new soft_Order_Child
                                {
                                    Id = item.Id,
                                    Total = item.Total - hasProduct.Total
                                };
                                _crud.Update<soft_Order_Child>(newobj, o => o.Total);
                            }
                        }
                    }
                }

                var OrderChildDoneAll = orderSuppliers.soft_Order_Child.FirstOrDefault(o => o.Status == null || o.Status == (int)StatusOrder_Input.Process);
                if (OrderChildDoneAll == null)
                {
                    var newobj = new soft_Order
                    {
                        Id = orderSuppliers.Id,
                        Status = (int)StatusOrder_Input.Done
                    };
                    _crud.Update<soft_Order>(newobj, o => o.Status);
                }
            }

        }

        private void UpdatePriceCompare(OrderModel order)
        {
            foreach (var item in order.Detail)
            {
                var product = _context.shop_sanpham.Find(item.ProductId);
                if (product != null)
                {
                    var newProduct = new shop_sanpham
                    {
                        id = product.id
                    };

                    newProduct.PriceCompare = item.PriceCompare;

                    _crud.Update<shop_sanpham>(newProduct, o => o.PriceCompare);
                }
            }
        }

        private void UpdatePrice_Channel(OrderModel order)
        {
            foreach (var item in order.Detail)
            {
                if (item.PriceChannels != null && item.PriceChannels.Count > 0)
                    foreach (var pricechannel in item.PriceChannels)
                    {
                        var product = _context.shop_sanpham.Find(item.ProductId);
                        if (product != null)
                        {
                            var hasprice = product.soft_Channel_Product_Price.FirstOrDefault(o => o.ChannelId == pricechannel.Id && o.ProductId == item.ProductId);
                            if (hasprice != null)
                            {
                                var obj = new soft_Channel_Product_Price
                                {
                                    Price = pricechannel.Price,
                                    Id = hasprice.Id,
                                    ChannelId = hasprice.ChannelId,
                                    ProductId = item.ProductId,
                                    DateCreate = hasprice.DateCreate,
                                    EmployeeCreate = hasprice.EmployeeCreate,
                                };
                                _crud.Update<soft_Channel_Product_Price>(obj, o => o.Price);
                            }
                        }
                    }
            }

        }
    }
}
