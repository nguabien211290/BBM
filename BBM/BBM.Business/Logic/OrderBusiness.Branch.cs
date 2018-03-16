using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public partial class OrderBusiness
    {
        public List<OrderModel> GetOrder_Branches(PagingInfo pageinfo, int BranchesId, out int count, out int min)
        {
            double totalMoney = 0;
            return Mapper.Map<List<OrderModel>>(unitOfWork.OrderBranchesRepository.SearchBy(pageinfo, out count, out min,out totalMoney, BranchesId));
        }
        public async Task<bool> AddOrder_Branches(OrderModel model, UserCurrent User)
        {
            model.TypeOrder = (int)TypeOrder.OrderBranches;
            model.DateCreate = DateTime.Now;
            model.EmployeeCreate = User.UserId;
            model.Status = (int)StatusOrder_Input.Process;

            var objOrder = Mapper.Map<soft_Order>(model);

            unitOfWork.OrderBranchesRepository.Add(objOrder);

            await unitOfWork.SaveChanges();

            return true;
        }

    }
}
