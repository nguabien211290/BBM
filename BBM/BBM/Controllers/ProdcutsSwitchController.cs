using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Repository;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBM.Infractstructure.Security;

namespace BBM.Controllers
{
    public class ProductsSwitchController : BaseController
    {
        //
        // GET: /Order_Input/
        private CRUD _crud;
        private admin_softbbmEntities _context;
        public ProductsSwitchController()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Product_Switch })]
        public ActionResult RenderView()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/ProductsSwitch/_ProductsSwitch_List.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Product_Switch })]
        public ActionResult RenderViewCreate()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/ProductsSwitch/_ProductsSwitch_Create.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Product_Switch })]
        public JsonResult Get_ProductsSwitch(PagingInfo pageinfo)
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null || User.ChannelId <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại !";
                }

                var clientMatters = _context.soft_Order.Where(o =>
                             o.TypeOrder == (int)TypeOrder.Switch
                             && o.Id_To.HasValue && o.Id_To == User.BranchesId
                             && o.Id_From.HasValue && o.Id_From == User.BranchesId
                             )
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
                        switch (item.Fiter)
                        {
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
                    }

                }
                #endregion

                Channel_Paging<OrderModel> lstInfo = new Channel_Paging<OrderModel>();

                lstInfo.totalItems = lstTmp.Count();

                int min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

                var orderSwitch = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

                lstInfo.listTable = orderSwitch;

                var sys_Employee = _context.sys_Employee.ToList();

                foreach (var item in lstInfo.listTable)
                {
                    var userCreate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeCreate);
                    item.EmployeeNameCreate = userCreate.Name + "(" + userCreate.Email + ")";
                }

                lstInfo.startItem = min;

                Messaging.Data = lstInfo;
            }
            catch(Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Do sự cố mạng, vui lòng thử lại !";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Product_Switch })]
        public JsonResult CreateProductsSwitch(OrderModel model)
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

                var objOrder = Mapper.Map<soft_Order>(model);

                objOrder.Id_From = User.BranchesId;
                objOrder.Id_To = User.BranchesId;
                objOrder.DateCreate = DateTime.Now;
                objOrder.EmployeeCreate = User.UserId;
                objOrder.TypeOrder = (int)TypeOrder.Switch;

                objOrder.Status = (int)StatusProductsSwitch.Done;

                _crud.Add<soft_Order>(objOrder);

                UpdateStockByBranches(objOrder);

                _crud.SaveChanges();
                Messaging.messaging = "Đã tạo phiếu điều chỉnh thành công !";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Do sự cố mạng, vui lòng thử lại !";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        private void UpdateStockByBranches(soft_Order order)
        {
            foreach (var item in order.soft_Order_Child)
            {
                var StockProduct = _context.soft_Branches_Product_Stock.FirstOrDefault(o => o.ProductId == item.ProductId
                && o.BranchesId == User.BranchesId);

                if (StockProduct != null)
                {

                    var newstock = new soft_Branches_Product_Stock
                    {
                        BranchesId = User.BranchesId,
                        ProductId = item.ProductId.Value,
                        //Stock_Total = StockProduct.Stock_Total + item.Total.Value,
                        Stock_Total = item.Total.Value,
                        DateCreate = DateTime.Now,
                        EmployeeCreate = User.UserId,
                        DateUpdate = DateTime.Now,
                        EmployeeUpdate = User.UserId,
                    };

                    _crud.Update(newstock, o => o.Stock_Total, o => o.EmployeeUpdate, o => o.DateUpdate);
                }
                else
                {
                    var stockTo = new soft_Branches_Product_Stock
                    {
                        ProductId = item.ProductId.Value,
                        BranchesId = User.BranchesId,
                        DateCreate = DateTime.Now,
                        Stock_Total = item.Total.Value,
                        EmployeeCreate = User.UserId
                    };
                    _crud.Add(stockTo);
                }

            }
        }
    }
}
