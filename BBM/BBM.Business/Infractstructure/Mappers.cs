using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Infractstructure.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BBM.Business.Infractstructure
{
    public static class MappersBusiness
    {
        public static void Boot()
        {
            Mapper.Initialize(cfg =>
            {

                #region don hang

                cfg.CreateMap<donhang, OrderModel>()
                       .ForMember(a => a.Note, b => b.MapFrom(c => c.ghichu))
                       .ForMember(a => a.Total, b => b.MapFrom(c => (int)c.tongtien))
                       .ForMember(a => a.DateCreate, b => b.MapFrom(c => c.ngaydat))
                       .ForMember(a => a.Id_From, b => b.MapFrom(c => c.Channeld))
                       .ForMember(a => a.Detail, b => b.ResolveUsing(c =>
                       {
                           var rs = new List<Order_DetialModel>();
                           foreach (var item in c.donhang_ct)
                           {
                               rs.Add(new Order_DetialModel
                               {
                                   Id = item.Id,
                                   OrderId = item.Sodh,
                                   Price = item.Dongia.Value,
                                   Total = item.Soluong,
                                   Discount = item.Discount.HasValue ? item.Discount.Value : 0,
                                   ProductId = (int)item.IdPro
                               });
                           }
                           return rs;
                       }))
                       .ForMember(a => a.Customer, b => b.MapFrom(c => c.khachhang));

                cfg.CreateMap<OrderModel, donhang>()
                 .ForMember(a => a.ghichu, b => b.MapFrom(c => c.Note))
                 .ForMember(a => a.tongtien, b => b.MapFrom(c => (int)c.Total))
                 .ForMember(a => a.ngaydat, b => b.MapFrom(c => c.DateCreate))
                 .ForMember(a => a.donhang_ct, b => b.ResolveUsing(c =>
                 {
                     var rs = new List<donhang_ct>();
                     foreach (var item in c.Detail)
                     {
                         rs.Add(new donhang_ct
                         {
                             Sodh = item.OrderId,
                             Dongia = item.Price,
                             Soluong = item.Total,

                             Discount = item.Discount,
                             IdPro = (long)item.ProductId
                         });
                     }
                     return rs;
                 }))
                 .ForMember(a => a.Channeld, b => b.MapFrom(c => c.Id_From))
                 .ForMember(a => a.makh, b => b.MapFrom(c => c.Id_Customer))
                 .ForMember(a => a.khachhang, b => b.MapFrom(c => c.Customer));

                cfg.CreateMap<donhang_ct, Order_DetialModel>()
                     .ForMember(a => a.Id, b => b.MapFrom(c => c.Id))
                     .ForMember(a => a.OrderId, b => b.MapFrom(c => c.Sodh))
                     .ForMember(a => a.Price, b => b.MapFrom(c => c.Dongia.Value))
                     .ForMember(a => a.Total, b => b.MapFrom(c => c.Soluong))
                     .ForMember(a => a.ProductId, b => b.MapFrom(c => c.IdPro));

                #endregion

                #region khachhang
                cfg.CreateMap<khachhang, CustomerModel>()
                                .ForMember(a => a.Id, b => b.MapFrom(c => c.MaKH))
                                .ForMember(a => a.Code, b => b.MapFrom(c => c.MaKH))
                                .ForMember(a => a.Name, b => b.MapFrom(c => c.hoten))
                                .ForMember(a => a.Address, b => b.MapFrom(c => c.duong))
                                .ForMember(a => a.Phone, b => b.MapFrom(c => c.dienthoai))
                                .ForMember(a => a.DistrictId, b => b.MapFrom(c => c.idquan))
                                .ForMember(a => a.ProvinceId, b => b.MapFrom(c => c.idtp))
                                .ForMember(a => a.Pwd, b => b.MapFrom(c => c.matkhau))
                                .ForMember(a => a.User, b => b.MapFrom(c => c.tendn))
                                .ForMember(a => a.Mark, b => b.MapFrom(c => c.diem));

                cfg.CreateMap<CustomerModel, khachhang>()
                            .ForMember(a => a.MaKH, b => b.MapFrom(c => c.Id))
                            .ForMember(a => a.hoten, b => b.MapFrom(c => c.Name))
                            .ForMember(a => a.duong, b => b.MapFrom(c => c.Address))
                            .ForMember(a => a.dienthoai, b => b.MapFrom(c => c.Phone))
                            .ForMember(a => a.idtp, b => b.MapFrom(c => c.ProvinceId))
                            .ForMember(a => a.idquan, b => b.MapFrom(c => c.DistrictId))
                            .ForMember(a => a.matkhau, b => b.MapFrom(c => c.Pwd))
                            .ForMember(a => a.tendn, b => b.MapFrom(c => c.User))
                            .ForMember(a => a.diem, b => b.MapFrom(c => c.Mark));

                cfg.CreateMap<CustomerAPiModel, khachhang>();
                //.ForMember(a => a.ngaydangky, b => b.ResolveUsing(c =>
                //{
                //    return JsonConvert.DeserializeObject<DateTime>(c.ngaydangky, new IsoDateTimeConverter { DateTimeFormat = "dd/mm/yyyy:hh:mm:ss" });
                //}));



                #endregion

                #region soft_Order
                cfg.CreateMap<soft_Order, OrderModel>()
                   .ForMember(a => a.Detail, b => b.MapFrom(c => c.soft_Order_Child))
                   .ForMember(a => a.Customer, b => b.MapFrom(c => c.khachhang));
                cfg.CreateMap<OrderModel, soft_Order>()
                   .ForMember(a => a.soft_Order_Child, b => b.MapFrom(c => c.Detail));
                cfg.CreateMap<Order_DetialModel, soft_Order_Child>();
                cfg.CreateMap<soft_Order_Child, Order_DetialModel>()
                   .ForMember(a => a.Product, b => b.MapFrom(c => c.shop_sanpham));
                cfg.CreateMap<OrderModel, OrderModel>();
                #endregion

                #region product
                cfg.CreateMap<shop_sanpham, ProductSampleModel>()
                       .ForMember(a => a.Img, b => b.ResolveUsing(c =>
                               {
                                   var img = c.shop_image.FirstOrDefault();
                                   if (img != null)
                                       return img.url;
                                   return null;
                               }))
                       .ForMember(a => a.PriceMainStore, b => b.ResolveUsing(c =>
                               {
                                   var pricechannel = c.soft_Channel_Product_Price.FirstOrDefault(o => o.soft_Channel.Type.Equals((int)TypeChannel.IsMainStore));
                                   if (pricechannel != null)
                                   {
                                       return pricechannel.Price;
                                   }
                                   return 0;
                               }));
                cfg.CreateMap<ProductSampleModel, shop_sanpham>();
                cfg.CreateMap<soft_Channel_Product_Price, Channel_Product_PriceModel>();
                cfg.CreateMap<Channel_Product_PriceModel, soft_Channel_Product_Price>();
                cfg.CreateMap<soft_Channel_Product_Price, Channel_Product_PriceModel>();
                cfg.CreateMap<soft_Channel_Product_Price, Product_PriceModel>();
                cfg.CreateMap<soft_Branches_Product_Stock, Product_StockModel>()
                    .ForMember(a => a.BranchesName, b => b.MapFrom(c => c.soft_Branches.BranchesName));

                #endregion

                cfg.CreateMap<EmployeeModel, sys_Employee>();
                cfg.CreateMap<sys_Employee, EmployeeModel>();
                cfg.CreateMap<SuppliersModel, soft_Suppliers>();
                cfg.CreateMap<soft_Suppliers, SuppliersModel>();
                cfg.CreateMap<soft_Discount, DisscountModel>();
                cfg.CreateMap<DisscountModel, soft_Discount>();
                cfg.CreateMap<soft_Notification, NotificationModel>();
                cfg.CreateMap<NotificationModel, soft_Notification>();
                cfg.CreateMap<EmployeeModel, sys_Employee>();
                cfg.CreateMap<sys_Employee, EmployeeModel>();
                cfg.CreateMap<CatalogModel, soft_Catalog>();
                cfg.CreateMap<soft_Catalog, CatalogModel>();
                cfg.CreateMap<soft_Employee_Title, Employee_TitleModel>();
                cfg.CreateMap<Employee_TitleModel, soft_Employee_Title>();
                cfg.CreateMap<soft_Config, Config>();
                cfg.CreateMap<ChannelModel, soft_Channel>();
                cfg.CreateMap<soft_Channel, ChannelModel>();
                cfg.CreateMap<soft_Branches, BranchesModel>();
                cfg.CreateMap<BranchesModel, soft_Branches>();
                cfg.CreateMap<CustomPrincipal, Config_UserModel>();
            });
        }
    }
}