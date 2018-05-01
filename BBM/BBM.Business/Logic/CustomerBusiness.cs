using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
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

        public async Task<bool> UpdateCustomer(CustomerModel model)
        {
            bool isComit = false;

            var objCustomer = Mapper.Map<khachhang>(model);

            if (model.Id <= 0)
            {
                objCustomer.matkhau = Helpers.GenerateToken(10);

                unitOfWork.CutomerRepository.Add(objCustomer);

                isComit = true;
            }
            else
            {
                var customer = unitOfWork.CutomerRepository.GetById(model.Id);

                if (customer == null)
                    return false;

                Mapper.Map(model, customer);

                unitOfWork.CutomerRepository.Update(customer);

                isComit = true;
            }

            if (isComit)
                await unitOfWork.SaveChanges();

            return true;
        }
    }
}
