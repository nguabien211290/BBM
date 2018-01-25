using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Model.Module
{
    public class UserCurrent
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
