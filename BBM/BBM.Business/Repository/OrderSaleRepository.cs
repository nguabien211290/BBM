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
    public class OrderSaleRepository : Repository<donhang>
    {
        public OrderSaleRepository(admin_softbbmEntities dbContext) : base(dbContext) { }

        public override List<donhang> SearchBy(PagingInfo pageinfo, out int count, out int min, out double totalMoney, int BranchesId = 0)
        {
            var lstTmp = FindBy(o => (o.Channeld.HasValue && o.Channeld == BranchesId));

            totalMoney = 0;

            #region Search
            if (!string.IsNullOrEmpty(pageinfo.keyword))
            {
                int iKey = 0;
                bool tryInt = int.TryParse(pageinfo.keyword, out iKey);
                //if (tryInt)
                //    lstTmp = lstTmp.Where(o => o.id == iKey);

                pageinfo.keyword = pageinfo.keyword.ToLower();
                lstTmp = lstTmp.Where(o =>
                (tryInt == true && o.id == iKey)
                 || (!string.IsNullOrEmpty(o.khachhang.tendn) && o.khachhang.tendn.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.khachhang.dienthoai) && o.khachhang.dienthoai.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.khachhang.hoten) && o.khachhang.hoten.Contains(pageinfo.keyword)));
            }
            #endregion

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
                        case "Time_To_From":
                            var StartDate = new DateTime(item.StartDate.Year, item.StartDate.Month, item.StartDate.Day, 0, 0, 0, 0); //item.StartDate.Date;
                            var EndDate = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 59, 999);// item.EndDate.Date.AddDays(1);
                            lstTmp = lstTmp.Where(o => o.ngaydat >= StartDate && o.ngaydat <= EndDate);
                            break;
                        case "EmployeeCreate":
                            key = int.Parse(item.Value);
                            lstTmp = lstTmp.Where(o => o.EmployeeCreate == key);
                            break;
                        case "EmployeeShip":
                            key = int.Parse(item.Value);
                            lstTmp = lstTmp.Where(o => o.EmployeeShip == key);
                            break;
                    }
                }
            }
            #endregion

            #region Sort
            bool isSort = false;
            if (!string.IsNullOrEmpty(pageinfo.sortby))
            {
                isSort = true;
                switch (pageinfo.sortby)
                {
                    case "DateCreate":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.ngaydat);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.ngaydat);
                        break;
                    case "Total":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.tongtien);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.tongtien);
                        break;
                    case "Status":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Status);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Status);
                        break;
                    case "Id":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.id);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.id);
                        break;
                }

            }
            #endregion

            min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

            count = lstTmp.Count();

            if (!isSort)
                lstTmp = lstTmp.OrderByDescending(o => o.ngaydat);

            var result = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

            totalMoney = (double)result.Sum(o => o.tongtien);

            return result;
        }

    }
}
