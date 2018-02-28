using AutoMapper;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBM.Business.Model.Entity;
using BBM.Business.Infractstructure;
using BBM.Business.Models.View;
using BBM.Business.Models.Enum;
using BBM.Business.Infractstructure.Security;
using BBM.Infractstructure.Security;
using BBM.Business.Logic;
using BBM.Business.Model.Module;
using BBM.Business.Repository;
using System.Threading.Tasks;

namespace BBM.Controllers
{
    public class OrderChannelController : BaseController
    {
        //
        // GET: /OrderChannel/
        private CRUD _crud;
        private admin_softbbmEntities _context;
        private IOrderBusiness _IOrderBus;
        private IUnitOfWork _unitOW;
        public OrderChannelController(IOrderBusiness IOrderBus, IUnitOfWork unitOW)
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

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Sales })]
        public ActionResult RenderViewList()
        {
            return PartialView("~/Views/Shared/Partial/module/Order/OrderChannel/_Channel_Order_List.cshtml");
        }
        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Sales })]
        public ActionResult RenderViewOrder(int OrderId = 0)
        {
            ViewBag.OrderId = OrderId;
            return PartialView("~/Views/Shared/Partial/module/Order/OrderChannel/_OrderSale.cshtml");
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Read_Order_Sales })]
        public JsonResult GetOrder(PagingInfo pageinfo)
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

                var lstOrder = _IOrderBus.GetOrder_Sale(pageinfo, User.ChannelId, out count, out min);

                lstInfo.totalItems = count;

                lstInfo.listTable = Mapper.Map<List<OrderModel>>(lstOrder);

                lstInfo.startItem = min;

                var sys_Employee = _unitOW.EmployeeRepository.GetAll().ToList();

                var soft_Branches = _unitOW.BrachesRepository.GetAll().ToList();

                var soft_Channel = _unitOW.ChannelRepository.GetAll().ToList();

                foreach (var item in lstInfo.listTable)
                {
                    if (item.Id_From > 0)
                    {
                        var channel = soft_Channel.FirstOrDefault(o => o.Id == item.Id_From);
                        if (channel != null)
                            item.Name_From = channel.Channel;
                    }


                    var userCreate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeCreate);

                    if (userCreate != null)
                        item.EmployeeNameCreate = userCreate.Name + " (" + userCreate.Email + ")";

                    if (item.EmployeeShip > 0)
                    {
                        var userShip = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeShip);
                        if (userShip != null)
                            item.EmployeeNameShip = userShip.Name;
                    }

                    if (item.EmployeeUpdate.HasValue)
                    {
                        var userUpdate = sys_Employee.FirstOrDefault(o => o.Id == item.EmployeeUpdate);
                        item.EmployeeNameUpdate = userUpdate.Name + "(" + userUpdate.Email + ")";
                    }
                }

                lstInfo.Total = lstInfo.listTable.Sum(o => o.Total);

                Messaging.Data = lstInfo;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị danh sách đơn hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Update_Order_Sales })]
        [HttpPost]
        public JsonResult GetDisscount(string code)
        {
            var Messaging = new RenderMessaging();

            if (!string.IsNullOrEmpty(code))
            {

                var dateNow = DateTime.Now;

                var discount = _unitOW.DisscountRepository.FindBy(o =>
                o.Code.Equals(code)
                && o.Disable == false
                && dateNow >= o.Startdate
                && (o.IsNotExp == true || dateNow < o.Enddate)).FirstOrDefault();

                if (discount == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng kiểm tra mã khuyến mãi.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Messaging.isError = false;
                    Messaging.Data = new { value = discount.Value, type = discount.Type };
                    return Json(Messaging, JsonRequestBehavior.AllowGet);

                }
            }
            return null;
        }

        [HttpPost]
        public JsonResult RenewInfoOrder()
        {
            var Messaging = new RenderMessaging();
            try
            {
                var Channel = _unitOW.ChannelRepository.GetById(User.ChannelId);
                var CodeOrder = DateTime.Now.ToString("dd/MM/yy").Replace("/", "") + "-" + Channel.Code + "-XXX";
                var EmployeeNameCreate = User.UserId;
                bool isChannelOnline = Channel.Type == (int)TypeChannel.IsChannelOnline ? true : false;

                Messaging.isError = false;
                Messaging.Data = new { CodeOrder = CodeOrder, ChannelName = Channel.Channel, EmployeeNameCreate = EmployeeNameCreate, isChannelOnline = isChannelOnline };
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Khởi tạo đơn hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInfoOrder(int Id)
        {
            var Messaging = new RenderMessaging();
            try
            {
                var order = _IOrderBus.GetInfoOrder(Id);
                if (order == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Đơn hàng này không tồn tại.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }
                Messaging.isError = false;
                Messaging.Data = order;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Hiển thị đơn hàng này không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateStatus_Order_Sale(OrderModel model)
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

                var user = Mapper.Map<UserCurrent>(User);

                var result = await _IOrderBus.UpdateOrder_Sale(model, user);

                if (!result.Item1)
                {
                    Messaging.isError = true;
                    Messaging.messaging = !string.IsNullOrEmpty(result.Item2) ? result.Item2 : "Cập nhât đơn hàng không thành công.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                Messaging.isError = false;
                Messaging.messaging = "Cập nhật đơn hàng thành công.";
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Cập nhật đơn hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(RolesEnums = new RolesEnum[] { RolesEnum.Create_Order_Sales })]
        [HttpPost]
        public async Task<JsonResult> CreatOrderSale(OrderModel model, bool isDone)
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

                var lstItemCart = new List<Order_DetialModel>();
                foreach (var item in model.Detail)
                {
                    if (item.ProductId > 0)
                        lstItemCart.Add(item);
                }

                model.Detail = lstItemCart;
                if (model.Detail == null || model.Detail.Count <= 0)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Không có sản phẩm nào trong đơn hàng.";
                    return Json(Messaging, JsonRequestBehavior.AllowGet);
                }

                model.Id_From = User.ChannelId;

                var user = Mapper.Map<UserCurrent>(User);

                var order = await _IOrderBus.CreatOrder_Sale(model, isDone, user);

                Messaging.Data = new { Code = order.Item1.Code + "-" + order.Item1.Id, isPrint = order.Item2 };
                Messaging.messaging = "Đã tạo đơn hàng thành công.";
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Tạo đơn hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Print_Order_Sale(OrderModel model)
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

                var order = _unitOW.OrderSaleRepository.GetById(model.Id);

                if (!string.IsNullOrEmpty(order.StatusPrint))
                    order.StatusPrint = order.StatusPrint + " <li>" + User.UserName + " đã in (" + DateTime.Now + ")</li>";
                else
                    order.StatusPrint = "<li>" + User.UserName + " đã in (" + DateTime.Now + ")</li>";


                _unitOW.OrderSaleRepository.Update(order, o => o.StatusPrint);

                await _unitOW.SaveChanges();

                Messaging.isError = false;
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "In đơn hàng không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}



