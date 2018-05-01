using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IBrachesBusiness
    {
        List<BranchesModel> GetBranches();
        Task<bool> CreateBranches(BranchesModel model, int UserId);
        Task<bool> UpdateBranches(BranchesModel model, int UserId);
        Task<bool> RemoveBranchs(int brachesId);
    }
}
