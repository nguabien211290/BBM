using AutoMapper;
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
    public class SuppliersBusiness : ISuppliersBusiness
    {
        private IUnitOfWork unitOfWork;

        public SuppliersBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<SuppliersModel> GetSuppliers(PagingInfo pageinfo, out int count, out int min)
        {
            double totalMoney = 0;

            var result = unitOfWork.SuppliersRepository.SearchBy(pageinfo, out count, out min, out totalMoney);

            return Mapper.Map<List<SuppliersModel>>(result);
        }
        public async Task<bool> DeleteSuppliers(int id)
        {
            var suppliers = unitOfWork.SuppliersRepository.GetById(id);

            if (suppliers == null)
            {
                return false;
            }

            unitOfWork.SuppliersRepository.Delete(suppliers);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> UpdateSuppliers(SuppliersModel model, int UserId)
        {
            bool isComit = false;
            if (model.SuppliersId <= 0)
            {
                var objSuppliers = Mapper.Map<soft_Suppliers>(model);

                objSuppliers.DateCreate = DateTime.Now;
                objSuppliers.EmployeeCreate = UserId;
                objSuppliers.DateUpdate = null;

                unitOfWork.SuppliersRepository.Add(objSuppliers);

                isComit = true;
            }
            else
            {

                var sup = unitOfWork.SuppliersRepository.GetById(model.SuppliersId);

                if (sup == null)
                    return false;

                sup.DateUpdate = DateTime.Now;
                sup.EmployeeUpdate = UserId;

                Mapper.Map(model, sup);

                unitOfWork.SuppliersRepository.Update(sup, 
                    o => o.Address, 
                    o => o.Email, 
                    o => o.Name, 
                    o => o.Phone, 
                    o => o.EmployeeUpdate,
                    o => o.DateUpdate);

                isComit = true;
            }

            if (isComit)
                await unitOfWork.SaveChanges();

            return true;
        }
    }
}
