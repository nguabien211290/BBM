using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business.Models.Module
{

    public class EmployeeModel
    {
        public int Id { get; set; }
        public int? Titles { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int EmployeeCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public int EmployeeUpdate { get; set; }
        public DateTime DateUpdate { get; set; }
        public string Roles { get; set; }
        public string PwdNew { get; set; }
        public bool isUpdatePwd { get; set; }
        public bool IsDisable { get; set; }
        public List<RolesObject> GroupRoles { get; set; }
    }
    public class GroupRoleModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<RoleEnumModel> Roles { get; set; }
    }
    public class RoleEnumModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool isSelect { get; set; }
    }
    public class RolesObject
    {
        public int BrandId { get; set; }
        public List<int> Roles { get; set; }
    }
    public class RolesObjectEmp
    {
        public int BrandId { get; set; }

        public string Roles { get; set; }
        public List<int> lstRole { get; set; }
    }
    public class Employee_TitleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}