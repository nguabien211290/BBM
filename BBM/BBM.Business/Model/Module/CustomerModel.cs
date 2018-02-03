using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Mark { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int DistrictId { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }

        public string NameShip { get; set; }
        public string AddressShip { get; set; }
        public string PhoneShip { get; set; }
        public int DistrictIdShip { get; set; }
        public int ProvinceIdShip { get; set; }

        public List<OrderModel> Orders { get; set; }

    }
    public class CustomerAPiModel
    {
        public int MaKH { get; set; }
        public int? idtp { get; set; }
        public int? idquan { get; set; }
        public string hoten { get; set; }
        public string duong { get; set; }
        public string dienthoai { get; set; }
        public string email { get; set; }
        public string tendn { get; set; }
        public string matkhau { get; set; }
        public string diem { get; set; }
        public bool konhanmail { get; set; }
        public DateTime? ngaydangky { get; set; }
    }
}