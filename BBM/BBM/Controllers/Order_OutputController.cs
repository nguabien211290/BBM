using AutoMapper;
using BBM.Business;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Business.Repository;
using BBM.Infractstructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class Order_OutputController : BaseController
    {
        private CRUD _crud;
        private admin_softbbmEntities _context;
        private IUnitOfWork _unitOW;
        public Order_OutputController(IUnitOfWork unitOW)
        {
            _crud = new CRUD();
            _unitOW = unitOW;
            _context = new admin_softbbmEntities();

        }
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_OutPut })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/OrderOutput/_Channel_Order_Output_List.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_OutPut })]
        public ActionResult RenderViewCreate(long orderId = 0)
        {
            if (orderId > 0)
            {
                var order = _context.soft_Order.FirstOrDefault(o => o.Id == orderId);

                if (order != null)
                {
                    var rs = new List<Prodcut_Branches_PriceChannel>();
                    foreach (var item in order.soft_Order_Child)
                    {
                        if (item.shop_sanpham.id > 0)
                        {
                            var product = _context.shop_sanpham.Find(item.shop_sanpham.id);
                            if (product != null)
                            {

                                var stocks = Mapper.Map<List<Product_StockModel>>(_context.soft_Branches_Product_Stock.Where(o => o.ProductId == product.id).ToList());
                                var stock = stocks.FirstOrDefault(o => o.BranchesId == User.BranchesId);

                                var productInfo = new Prodcut_Branches_PriceChannel
                                {
                                    product_stock = stock ?? stock,
                                    product = Mapper.Map<ProductSampleModel>(product),
                                    product_stocks = stocks,
                                    Total = item.Total.HasValue ? item.Total.Value : 1,
                                    OrderFromId = orderId
                                };
                                if (product.soft_Suppliers != null)
                                    productInfo.product.SuppliersName = product.soft_Suppliers.Name;

                                rs.Add(productInfo);
                            }
                        }
                    }
                    ViewBag.Products = Newtonsoft.Json.JsonConvert.SerializeObject(rs);
                }
            }
            return PartialView("~/Views/Shared/Partial/module/Order/OrderOutput/_Channel_Order_Output_Create.cshtml");
        }

        public JsonResult GetOrder_Output(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }
                //var lstTmp = from order in _context.soft_Order where order.TypeOrder == (int)TypeOrder.Output && order.Id_From == User.BranchesId orderby order.DateCreate descending select order;

                var clientMatters = _context.soft_Order.Where(o =>
                              o.TypeOrder == (int)TypeOrder.Output && o.Id_From.HasValue && o.Id_From == User.BranchesId)
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
                            case "Brach_From":
                                key = int.Parse(item.Value);
                                lstTmp = lstTmp.Where(o => o.Id_From > 0 && o.Id_From.Equals(key));
                                break;
                            case "Brach_To":
                                key = int.Parse(item.Value);
                                lstTmp = lstTmp.Where(o => o.Id_To > 0 && o.Id_To.Equals(key));
                                break;
                            case "Time_To_From":
                                var StartDate = new DateTime(item.StartDate.Year, item.StartDate.Month, item.StartDate.Day, 0, 0, 0, 0); //item.StartDate.Date;
                                var EndDate = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 59, 999);// item.EndDate.Date.AddDays(1);
                                lstTmp = lstTmp.Where(o => o.DateCreate >= StartDate && o.DateCreate <= EndDate);
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

                var orderOutput = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

                var soft_Branches = _context.soft_Branches.ToList();

                var sys_Employee = _context.sys_Employee.ToList();

                foreach (var item in orderOutput)
                {
                    if (item.Id_From > 0)
                    {
                        var branch = soft_Branches.FirstOrDefault(o => o.BranchesId == item.Id_From);
                        if (branch != null)
                            item.Name_From = branch.BranchesName;
                    }
                    if (item.Id_To > 0)
                    {
                        var branch = soft_Branches.FirstOrDefault(o => o.BranchesId == item.Id_To);
                        if (branch != null)
                            item.Name_To = branch.BranchesName;
                    }

                    var userCreate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeCreate);
                    item.EmployeeNameCreate = userCreate.Name + "(" + userCreate.Email + ")";

                    if (item.EmployeeUpdate.HasValue)
                    {
                        var userUpdate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeUpdate);
                        item.EmployeeNameUpdate = userUpdate.Name + "(" + userUpdate.Email + ")";
                    }
                }

                lstInfo.listTable = orderOutput;

                lstInfo.startItem = min;

                #region addproduct
                #endregion
                Messaging.Data = lstInfo;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách phiếu xuất không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_OutPut })]
        public async Task<JsonResult> CreateOrder_Output(OrderModel model)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.BranchesId <= 0 || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Phiên đăng nhập đã hết hạn.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                model.Id_From = User.BranchesId;

                if (model.Id_To <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng chọn Kho bạn xuất đến.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                var Braches = _context.soft_Branches.Where(o => o.BranchesId == model.Id_To || o.BranchesId == User.BranchesId).ToList();
                var BrachesTo = Braches.FirstOrDefault(o => o.BranchesId == model.Id_To);
                if (model.Id_To == User.BranchesId || BrachesTo == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Kho xuất không hơp lệ";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                if (model.Detail.Count <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng chọn sản phẩm xuất.";
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

                    var productstock = _context.soft_Branches_Product_Stock.FirstOrDefault(o => o.BranchesId == User.BranchesId && o.ProductId == item.ProductId);

                    if (productstock != null)
                    {
                        if (productstock.Stock_Total < item.Total)
                        {
                            Messaging.isError = true;
                            Messaging.messaging = "Số lượng sản phẩm " + product.tensp + " không đủ để xuất, vui lòng thử lại.";
                            return Json(Messaging, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        Messaging.isError = true;
                        Messaging.messaging = "Số lượng sản phẩm " + product.tensp + " không đủ để xuất, vui lòng thử lại.";
                        return Json(Messaging, JsonRequestBehavior.AllowGet);
                    }
                }

                var objOrder = Mapper.Map<soft_Order>(model);

                objOrder.Code = "PX" + DateTime.Now.ToString("dd/MM/yy").Replace("/", "") + "-" + BrachesTo.Code;
                objOrder.Status = (int)StatusOrder_Output.Process;
                objOrder.DateCreate = DateTime.Now;
                objOrder.EmployeeCreate = User.UserId;
                objOrder.TypeOrder = (int)TypeOrder.Output;
                _crud.Add<soft_Order>(objOrder);
                _crud.SaveChanges();
                objOrder.Code = "PX" + DateTime.Now.ToString("dd/MM/yy").Replace("/", "") + "-" + BrachesTo.Code + "-" + objOrder.Id;

              
                if (model.OrderFromId.HasValue)
                {
                    var orderFromBranches = _unitOW.OrderBranchesRepository.FindBy(o => o.Id == model.OrderFromId.Value && o.TypeOrder == (int)TypeOrder.OrderBranches).FirstOrDefault();

                    if (orderFromBranches != null)
                    {
                        orderFromBranches.Status = (int)StatusOrder_Branches.Exported;
                        _unitOW.OrderBranchesRepository.Update(orderFromBranches, o => o.Status);
                        await _unitOW.SaveChanges();
                    }
                }

                _unitOW.NotificationRepository.Add(new soft_Notification
                {
                    Contents = "Đơn hàng xuất " + objOrder.Code + " từ kho " + Braches.FirstOrDefault(o => o.BranchesId == User.BranchesId).BranchesName + " đến kho " + BrachesTo.BranchesName + " cần được xử lý",
                    Branch = model.Id_To,
                    DateCreate = DateTime.Now,
                    Type = (int)TypeNotification.OrderOut,
                    Href = "/Order_Input/RenderView"
                });

                await _unitOW.SaveChanges();

                Messaging.messaging = "Đã tạo phiếu xuất hàng.";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo phiếu xuất không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TotalSumAdvSalebyChannel(List<int> productIds, int brancheId, int totaldateSum = 10)
        {
            var result = new List<Product_Sale_Average>();

            productIds.ForEach(id =>
            {
                result.Add(new Product_Sale_Average
                {
                    ProductId = id,
                    Sale_Average = Product_Sale_AveragebyChannel(id, brancheId, totaldateSum)
                });
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private double Product_Sale_AveragebyChannel(int productId, int brancheId, int totaldateSum)
        {
            var branches = _context.soft_Branches.Find(brancheId);

            if (branches == null)
                return 0;

            var lstChannel = branches.soft_Channel.Select(o => o.Id).ToList();

            var orderedSale = _context.soft_Order_Child.Where(o => o.ProductId == productId
                                    && o.soft_Order.TypeOrder == (int)TypeOrder.Sale
                                    && o.soft_Order.Id_From.HasValue
                                    && lstChannel.Contains(o.soft_Order.Id_From.Value))
                                    .OrderByDescending(o => o.soft_Order.DateCreate).Take(totaldateSum).ToList();

            if (orderedSale.Count > 0)
                return (orderedSale.Sum(o => o.Total) / orderedSale.Count).Value;

            return 0;
        }
    }
}
