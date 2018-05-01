using BBM.Business.Model.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IBarCodeBusiness
    {
        BarcodeModel GetConfig(int BranchesId);
        Task<bool> SaveConfig(BarcodeModel model);
    }
}
