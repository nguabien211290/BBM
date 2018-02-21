using AutoMapper;
using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public class ApiBusiness : IApiBusiness
    {
        private IUnitOfWork unitOfWork;

        public ApiBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task SyncCustomer()
        {
            int totalRecord = await RequestWrapper.SendRequest<int>(BBMApiInfo._totalCustomer, HttpMethod.Get);

            #region link api GET 
            int limit = 20;
            int page = 1;

            int tempLimit = int.Parse((Math.Ceiling((double)(totalRecord / limit))).ToString()) + 1;
            bool isupdate = false;
            #endregion
            while (page <= tempLimit)
            {
                try {

                    var data = await RequestWrapper.SendRequest<List<CustomerAPiModel>>(
                        BBMApiInfo._getlistCustomer + $"?pagesize={limit}&page={page}",
                        HttpMethod.Get);

                    if (data != null)
                    {
                        if (data.Count == 0) break;

                        foreach (var item in data)
                        {
                            var customerExits = unitOfWork.CutomerRepository.FindBy(o => o.MaKH == item.MaKH).FirstOrDefault();
                            if (customerExits == null)
                            {
                                var cus = Mapper.Map<khachhang>(item);

                                unitOfWork.CutomerRepository.Add(cus);

                                isupdate = true;
                            }
                        }
                    }
                    page++;
                }
                catch (Exception ex)
                {

                }
            }

            if (isupdate)
                await unitOfWork.SaveChanges();
        }
    }
}
