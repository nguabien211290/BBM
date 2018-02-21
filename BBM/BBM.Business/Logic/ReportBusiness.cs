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
    public class ReportBusiness : IReportBusiness
    {
        private IUnitOfWork unitOfWork;

        public ReportBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<OrderModel> Report_Order(PagingInfo pageinfo, out int count, out int min)
        {
            double totalMoney = 0;
            var result = unitOfWork.CatalogRepository.SearchBy(pageinfo, out count, out min,out totalMoney);
            return Mapper.Map<List<OrderModel>>(result);
        }
    }
}
