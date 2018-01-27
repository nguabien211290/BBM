using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public class BrachesBusiness : IBrachesBusiness
    {
        private IUnitOfWork unitOfWork;

        public BrachesBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public List<BranchesModel> GetBranches()
        {
            return Mapper.Map<List<BranchesModel>>(unitOfWork.BrachesRepository.GetAll().ToList());
        }
        public async Task CreateBranches(BranchesModel model, UserCurrent User)
        {
            var objBraches = Mapper.Map<soft_Branches>(model);

            objBraches.DateCreate = DateTime.Now;
            objBraches.EmployeeCreate = User.UserId;

            unitOfWork.BrachesRepository.Add(objBraches);

            foreach (var item in model.soft_Channel)
            {
                var objChannel = Mapper.Map<soft_Channel>(item);

                objChannel.DateCreate = DateTime.Now;
                objChannel.EmployeeCreate = User.UserId;

                unitOfWork.ChannelRepository.Add(objChannel);
            }

            await unitOfWork.SaveChanges();
        }

        public async Task UpdateBranches(BranchesModel model, UserCurrent User)
        {
            var objBraches = Mapper.Map<soft_Branches>(model);
            objBraches.DateUpdate = DateTime.Now;
            objBraches.EmployeeUpdate = User.UserId;

            unitOfWork.BrachesRepository.Update(objBraches, o => o.BranchesName,
                o => o.Code,
                o => o.IsPrimary,
                o => o.Phone, o => o.Address,
                o => o.DateUpdate, o => o.EmployeeUpdate);

            await unitOfWork.SaveChanges();
        }
    }
}
