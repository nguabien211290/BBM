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
    public class CatalogBusiness : ICatalogBusiness
    {
        private IUnitOfWork unitOfWork;

        public CatalogBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<CatalogModel> GetCatalog(PagingInfo pageinfo, out int count, out int min)
        {
            double totalMoney = 0;
            var result = unitOfWork.CatalogRepository.SearchBy(pageinfo, out count, out min, out totalMoney);
            return Mapper.Map<List<CatalogModel>>(result);
        }

        public async Task<bool> DeleteCatalog(int id)
        {
            var catalog = unitOfWork.CatalogRepository.GetById(id);

            if (catalog == null)
            {
                return false;
            }

            unitOfWork.CatalogRepository.Delete(catalog);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> CreateCatalog(CatalogModel model, int UserId)
        {
            var objCatalog = Mapper.Map<soft_Catalog>(model);

            objCatalog.DateCreate = DateTime.Now;
            objCatalog.EmployeeCreate = UserId;
            objCatalog.DateUpdate = null;
            unitOfWork.CatalogRepository.Add(objCatalog);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Updateatalog(CatalogModel model, int UserId)
        {

            var catalog = unitOfWork.CatalogRepository.GetById(model.Id);

            if (catalog == null)
                return false;

            catalog.EmployeeUpdate = UserId;
            catalog.DateUpdate = DateTime.Now;
            catalog.Name = model.Name;
            unitOfWork.CatalogRepository.Add(catalog);
            
            await unitOfWork.SaveChanges();

            return true;
        }
    }
}
