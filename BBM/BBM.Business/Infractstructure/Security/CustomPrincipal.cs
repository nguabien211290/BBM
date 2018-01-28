using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Principal;
using BBM.Business.Model.Entity;
using Newtonsoft.Json;
using BBM.Business.Models.Module;

namespace BBM.Business.Infractstructure.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private admin_softbbmEntities _context = new admin_softbbmEntities();
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {

            var employee = _context.sys_Employee.FirstOrDefault(o => o.Id == UserId
            && o.IsDisable.HasValue
            && !o.IsDelete
            && !o.IsDisable.Value);

            if (employee == null || employee.Roles == null) return false;

            var roleObject = JsonConvert.DeserializeObject<List<RolesObjectEmp>>(employee.Roles);


            roleObject.ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.Roles))
                    item.lstRole = item.Roles.Split(',').Select(Int32.Parse).ToList();
            });

            var roleArry = role.Split(',').Select(Int32.Parse).ToList();

            foreach (var item in roleArry)
            {

                var anc = roleObject.FirstOrDefault(o => o.BrandId == BranchesId && o.lstRole != null && o.lstRole.Contains(item));
                if (anc != null)
                    return true;
            }
            return false;
        }

        public CustomPrincipal(string Username)
        {
            this.Identity = new GenericIdentity(Username);
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int BranchesId { get; set; }
        public bool IsPrimary { get; set; }
        public int ChannelId { get; set; }
        public List<Role_Branches> role_branches { get; set; }
    }

    public class CustomPrincipalSerializeModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int BranchesId { get; set; }
        public bool IsPrimary { get; set; }
        public int ChannelId { get; set; }
        public List<Role_Branches> role_branches { get; set; }
    }
    public class Role_Branches
    {
        public int BranchesId { get; set; }
        public List<string> Roles { get; set; }
    }
}