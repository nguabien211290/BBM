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
        Task CreateBranches(BranchesModel model, UserCurrent User);
    }
}
