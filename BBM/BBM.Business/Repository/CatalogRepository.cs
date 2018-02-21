using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Repository
{
    public class CatalogRepository : Repository<soft_Catalog>
    {
        public CatalogRepository(admin_softbbmEntities dbContext) : base(dbContext) { }

        public override List<soft_Catalog> SearchBy(PagingInfo pageinfo, out int count, out int min, out double totalMoney, int BranchesId = 0)
        {
            totalMoney = 0;

            var lstTmp = GetAll();

            var isSort = false;
            #region Sort
            if (!string.IsNullOrEmpty(pageinfo.sortby))
            {
                isSort = true;
                switch (pageinfo.sortby)
                {
                    case "Name":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Name);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Name);
                        break;
                }
            }
            #endregion
            #region Search
            if (!string.IsNullOrEmpty(pageinfo.keyword))
            {
                pageinfo.keyword = pageinfo.keyword.ToLower();

                lstTmp = lstTmp.Where(o => !string.IsNullOrEmpty(o.Name) && o.Name.Contains(pageinfo.keyword));
            }
            #endregion

            min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

            count = lstTmp.Count();

            if (!isSort)
                lstTmp = lstTmp.OrderByDescending(o => o.Name);

            var result = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

            return result;
        }
    }
}
