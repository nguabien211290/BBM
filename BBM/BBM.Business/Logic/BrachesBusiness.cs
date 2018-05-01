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
        public async Task<bool> CreateBranches(BranchesModel model, int UserId)
        {
            var objBraches = Mapper.Map<soft_Branches>(model);

            objBraches.DateCreate = DateTime.Now;

            objBraches.EmployeeCreate = UserId;

            objBraches.Type = 1;

            unitOfWork.BrachesRepository.Add(objBraches);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> UpdateBranches(BranchesModel model, int UserId)
        {
            if (model.BranchesId <= 0)
                return false;

            var ob = unitOfWork.BrachesRepository.GetById(model.BranchesId);

            if(ob==null)
                return false;


            ob.Code = model.Code;
            ob.IsPrimary = model.IsPrimary;
            ob.BranchesName = model.BranchesName;
            ob.Phone = model.Phone;
            ob.Address = model.Address;
            ob.DateUpdate = DateTime.Now;
            ob.EmployeeUpdate = UserId;

            unitOfWork.BrachesRepository.Update(ob, o => o.BranchesName,
                o => o.Code,
                o => o.IsPrimary,
                o => o.Phone, o => o.Address,
                o => o.DateUpdate, o => o.EmployeeUpdate);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> RemoveBranchs(int brachesId)
        {
            var braches = unitOfWork.BrachesRepository.GetById(brachesId);

            if (braches == null)
                return false;

            if (braches.soft_Channel.Count > 0)
                foreach (var item in braches.soft_Channel.ToList())
                {
                    unitOfWork.ChannelRepository.Delete(item.Id);
                }
            unitOfWork.BrachesRepository.Delete(brachesId);

            await unitOfWork.SaveChanges();

            return true;
        }
    }
}
