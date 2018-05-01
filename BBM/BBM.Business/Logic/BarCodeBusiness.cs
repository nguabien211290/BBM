using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public class BarCodeBusiness : IBarCodeBusiness
    {
        private IUnitOfWork unitOfWork;

        public BarCodeBusiness(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public BarcodeModel GetConfig(int BranchesId)
        {
            var config = unitOfWork.Config_PrintTemRepository.FindBy(o => o.BranchId == BranchesId).FirstOrDefault();
            if (config == null)
                return null;
            return Mapper.Map<BarcodeModel>(config);
        }

        public async Task<bool> SaveConfig(BarcodeModel model)
        {
            var rs = Mapper.Map<soft_Config_PrintTem>(model);

            if (!rs.BranchId.HasValue)
                return false;

            if (model.Id <= 0)
            {
                unitOfWork.Config_PrintTemRepository.Add(rs);
            }
            else
                unitOfWork.Config_PrintTemRepository.Update(rs);

            await unitOfWork.SaveChanges();

            return true;
        }

    }
}
