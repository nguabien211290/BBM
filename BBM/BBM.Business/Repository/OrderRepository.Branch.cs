using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Enum;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Repository
{
    public class OrderBranchRepository : Repository<soft_Order>
    {
        public OrderBranchRepository(admin_softbbmEntities dbContext) : base(dbContext) { }

        public override List<soft_Order> SearchBy(PagingInfo pageinfo, out int count, out int min, out double totalMoney, int BranchesId = 0)
        {
            totalMoney = 0;

            var lstTmp = FindBy(o => (o.TypeOrder == (int)TypeOrder.OrderBranches));

            #region Fillter
            if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
            {
                foreach (var item in pageinfo.filterby)
                {
                    var key = 0;
                    switch (item.Fiter)
                    {
                        case "Status":
                            key = int.Parse(item.Value);
                            lstTmp = lstTmp.Where(o => o.Status > 0 && o.Status == key);
                            break;
                        case "Brach_From":
                            key = int.Parse(item.Value);
                            lstTmp = lstTmp.Where(o => o.Id_From > 0 && o.Id_From == key);
                            break;
                        case "Brach_To":
                            key = int.Parse(item.Value);
                            lstTmp = lstTmp.Where(o => o.Id_To > 0 && o.Id_To == key);
                            break;
                        case "Time_To_From":
                            var StartDate = new DateTime(item.StartDate.Year, item.StartDate.Month, item.StartDate.Day, 0, 0, 0, 0); //item.StartDate.Date;
                            var EndDate = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 59, 999);// item.EndDate.Date.AddDays(1);
                            lstTmp = lstTmp.Where(o => o.DateCreate >= StartDate && o.DateCreate <= EndDate);
                            break;
                    }
                }
            }
            #endregion
            bool isSort = false;
            #region Sort
            if (!string.IsNullOrEmpty(pageinfo.sortby))
            {
                isSort = true;
                switch (pageinfo.sortby)
                {
                    case "Brach_To":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Id_To);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Id_To);
                        break;

                    case "Brach_From":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Id_From);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Id_From);
                        break;

                    case "DateCreate":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.DateCreate);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.DateCreate);
                        break;
                    case "Total":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Total);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Total);
                        break;
                    case "Status":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Status);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Status);
                        break;
                    case "Id":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Id);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Id);
                        break;
                }

            }
            #endregion

            min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

            count = lstTmp.Count();

            if (!isSort)
                lstTmp = lstTmp.OrderByDescending(o => o.DateCreate);

            var result = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

            return result;
        }
    }
}
