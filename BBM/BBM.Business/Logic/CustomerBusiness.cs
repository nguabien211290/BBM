using AutoMapper;
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
    public class CustomerBusiness : ICustomerBusiness
    {
        private IUnitOfWork unitOfWork;

        public CustomerBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<CustomerModel> GetCustomer(PagingInfo pageinfo, out int count, out int min)
        {
            double totalMoney = 0;
            var result = unitOfWork.CutomerRepository.SearchBy(pageinfo, out count, out min, out totalMoney);
            return Mapper.Map<List<CustomerModel>>(result);
        }
    }
}
