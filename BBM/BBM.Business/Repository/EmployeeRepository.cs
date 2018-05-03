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
    public class EmployeeRepository : Repository<sys_Employee>
    {
        public EmployeeRepository(admin_softbbmEntities dbContext) : base(dbContext) { }
        public override List<sys_Employee> SearchBy(PagingInfo pageinfo, out int count, out int min, out double totalMoney, int BranchesId = 0)
        {
            totalMoney = 0;

            var lstTmp = GetAll();

            var isSort = false;

            #region Sort
            if (!string.IsNullOrEmpty(pageinfo.sortby))
            {
                switch (pageinfo.sortby)
                {
                    case "Name":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Name);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Name);
                        break;
                    case "Titles":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Titles);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Titles);
                        break;
                    case "Email":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Email);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Email);
                        break;
                    case "Phone":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Phone);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Phone);
                        break;
                    case "Address":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Address);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Address);
                        break;

                }

            }
            #endregion
            #region Search
            if (!string.IsNullOrEmpty(pageinfo.keyword))
            {
                pageinfo.keyword = pageinfo.keyword.ToLower();

                lstTmp = lstTmp.Where(o => o.Name.Contains(pageinfo.keyword)
                || o.Name.Contains(pageinfo.keyword)
                || o.Phone.Contains(pageinfo.keyword)
                || o.Email.Contains(pageinfo.keyword)
                || o.Address.Contains(pageinfo.keyword)
                );
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
