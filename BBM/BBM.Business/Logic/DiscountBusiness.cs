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
    public class DiscountBusiness : IDiscountBusiness
    {
        private IUnitOfWork unitOfWork;

        public DiscountBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<DisscountModel> GetDisscount(PagingInfo pageinfo, out int count, out int min)
        {
            double totalMoney = 0;
            var result = unitOfWork.DisscountRepository.SearchBy(pageinfo, out count, out min, out totalMoney);
            return Mapper.Map<List<DisscountModel>>(result);
        }

        public async Task<bool> DisableDiscount(DisscountModel model, int UserId)
        {

            var discount = unitOfWork.DisscountRepository.FindBy(o => o.Id == model.Id && !o.Disable).FirstOrDefault();

            if (discount == null)
                return false;

            discount.Disable = true;
            discount.DateUpdate = DateTime.Now;
            discount.EmployeeUpdate = UserId;

            unitOfWork.DisscountRepository.Update(discount);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> CreateDiscount(DisscountModel model, int UserId)
        {
            var date = DateTime.Now;

            var discount = Mapper.Map<soft_Discount>(model);

            discount.Startdate = new DateTime(discount.Startdate.Year, discount.Startdate.Month, discount.Startdate.Day, 0, 0, 0, 0);

            if (discount.Enddate.HasValue)
                discount.Enddate = new DateTime(discount.Enddate.Value.Year, discount.Enddate.Value.Month, discount.Enddate.Value.Day, 23, 59, 59, 999);

            var lstDisscount = unitOfWork.DisscountRepository.GetAll().Select(o => o.Code).ToList();

            discount.Code = Helpers.GenerateToken(10, lstDisscount);

            discount.EmployeeCreate = UserId;

            discount.DateCreate = DateTime.Now;

            discount.DateUpdate = null;

            if (discount.IsNotExp)
                discount.Enddate = null;

            unitOfWork.DisscountRepository.Add(discount);

            await unitOfWork.SaveChanges();

            return true;
        }
    }
}
