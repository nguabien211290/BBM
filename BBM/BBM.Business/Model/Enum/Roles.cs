using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Enum
{
    public enum RolesEnum
    {
        //[Display(Name = "Full Quyền")]
        //Administrator = 0,

        [Display(Name = "Danh sách đơn hàng nhập")]
        Read_Order_Input = 1,
        [Display(Name = "Tạo đơn hàng nhập")]
        Create_Order_Input = 2,
        [Display(Name = "Cập nhật đơn hàng nhập")]
        Update_Order_Input = 3,

        [Display(Name = "Danh sách đơn hàng xuất")]
        Read_Order_OutPut = 4,
        [Display(Name = "Tạo đơn hàng xuất")]
        Create_Order_OutPut = 5,
        [Display(Name = "Cập nhật đơn hàng xuất")]
        Update_Order_OutPut = 6,

        [Display(Name = "Danh sách đơn hàng NPP")]
        Read_Order_Suppliers = 7,
        [Display(Name = "Tạo đơn hàng NPP")]
        Create_Order_Suppliers = 8,
        [Display(Name = "Cập nhật đơn hàng NPP")]
        Update_Order_Suppliers = 9,

        [Display(Name = "Danh sách đơn đặt hàng")]
        Read_Order_Sales = 10,
        [Display(Name = "Tạo đơn đặt hàng")]
        Create_Order_Sales = 11,
        [Display(Name = "Cập nhật đơn đặt hàng")]
        Update_Order_Sales = 12,


        [Display(Name = "Danh sách sản phẩm")]
        Read_Products = 13,
        [Display(Name = "Cập nhật sản phẩm")]
        Update_Products = 14,
        [Display(Name = "Tạo sản phẩm")]
        Create_Products = 43,
        [Display(Name = "Xóa sản phẩm")]
        Delete_Products = 44,
        [Display(Name = "Cập nhật giá theo Kênh")]
        Update_Products_Price = 15,
        [Display(Name = "Cập nhật giá theo Khuyễn mãi theo Kênh")]
        Update_Products_Price_Discount = 39,


        [Display(Name = "Danh sách khách hàng")]
        Read_Customer = 16,
        [Display(Name = "Tạo đơn khách hàng")]
        Create_Customer = 17,
        [Display(Name = "Cập nhật giá khách hàng")]
        Update_Customer = 18,

        [Display(Name = "Xem danh sách Nhà phân phối")]
        Read_Suppliers = 19,
        [Display(Name = "Xóa Nhà phân phối")]
        Delete_Suppliers = 20,
        [Display(Name = "Cập nhật Nhà phân phối")]
        Update_Suppliers = 21,

        [Display(Name = "Danh sách Nhân viên")]
        Read_Employess = 22,
        [Display(Name = "Tạo đơn Nhân viên")]
        Create_Employess = 23,
        [Display(Name = "Cập nhật Nhân viên")]
        Update_Employesss = 24,
        [Display(Name = "Xem Phân quyền nhân viên")]
        Read_Roles_Employess = 25,
        [Display(Name = "Phân quyền nhân viên")]
        Update_Roles_Employess = 26,

        [Display(Name = "Danh sách khuyến mãi")]
        Read_Disscount = 27,
        [Display(Name = "Tạo khuyến mãi")]
        Create_Disscount = 28,
        [Display(Name = "Cập nhật khuyến mãi")]
        Update_Disscount = 29,

        [Display(Name = "Danh sách phiếu chuyển")]
        Read_Product_Switch = 30,
        [Display(Name = "Tạo danh sách phiếu chuyển")]
        Create_Product_Switch = 31,


        [Display(Name = "Danh sách chi nhánh")]
        Read_Branches = 32,
        [Display(Name = "Tạo chi nhánh")]
        Create_Branches = 33,
        [Display(Name = "Sửa chi nhánh")]
        Update_Branches = 34,
        [Display(Name = "Xóa chi nhánh")]
        Remove_Branches = 35,


        [Display(Name = "Tạo kênh bán hàng")]
        Create_Channel = 36,
        [Display(Name = "Sửa kênh bán hàng")]
        Update_Channel = 37,
        [Display(Name = "Xóa kênh bán hàng")]
        Remove_Channel = 38,

        [Display(Name = "Xem nhóm hàng hàng")]
        Read_Catalog = 40,
        [Display(Name = "Cập nhật nhóm hàng hóa")]
        Update_Catalog = 41,
        [Display(Name = "Xóa nhóm hàng hóa")]
        Remove_Catalog = 42,
        [Display(Name = "Xóa nhân viên")]
        Delete_Employess = 46,


        [Display(Name = "Danh sách đơn hàng đặt nội bộ")]
        Read_Order_Branches = 47,
        [Display(Name = "Tạo đơn hàng đặt nội bộ")]
        Create_Order_Branches = 48,
        [Display(Name = "Cập nhật đơn hàng đặt nội bộ")]
        Update_Order_Branches = 49,

        [Display(Name = "Doanh số bán hàng")]
        report_sales = 50,
        //50
    }
    public enum GroupRolesEnum
    {
        //[Display(Name = "Tất cả")]
        //Administrator = 1,
        [Display(Name = "Đơn hàng nhập")]
        OrderInput = 2,
        [Display(Name = "Đơn hàng xuất")]
        OrderOutput = 3,
        [Display(Name = "Đơn hàng NPP")]
        OrderSuppliers = 4,
        [Display(Name = "Đơn đặt hàng")]
        OrderSales = 5,
        [Display(Name = "Sản phẩm")]
        Products = 6,
        [Display(Name = "Khuyễn mãi")]
        Disscount = 7,
        [Display(Name = "Khách hàng")]
        Customer = 8,
        [Display(Name = "Nhà phân phối")]
        Suppliers = 9,
        [Display(Name = "Quản lý người dùng")]
        Account = 10,
        [Display(Name = "Phiếu chuyển")]
        Product_Switch = 11,
        [Display(Name = "KHO")]
        Branches = 12,
        [Display(Name = "Kênh bán hàng")]
        Channel = 13,
        [Display(Name = "Nhóm hàng hóa")]
        Catalog = 14,
        [Display(Name = "Đơn đặt hàng Nội bộ")]
        OrderBranches = 15,
        [Display(Name = "Báo cáo")]
        Report = 16,

    }
}