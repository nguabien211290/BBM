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
    public class CustomerRepository : Repository<khachhang>
    {
        public CustomerRepository(admin_softbbmEntities dbContext) : base(dbContext) { }

        public override List<khachhang> SearchBy(PagingInfo pageinfo, out int count, out int min, out double totalMoney, int BranchesId = 0)
        {
            var lstTmp = GetAll();
            totalMoney = 0;
            bool isSort = false;

            #region Sort
            if (!string.IsNullOrEmpty(pageinfo.sortby))
            {
                switch (pageinfo.sortby)
                {
                    case "Id":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.MaKH);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.MaKH);
                        break;
                    case "User":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.tendn);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.tendn);
                        break;
                    case "Name":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.hoten);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.hoten);
                        break;
                    case "Email":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.email);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.email);
                        break;
                    case "Phone":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.dienthoai);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.dienthoai);
                        break;
                    case "DistrictId":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.idtp);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.idtp);
                        break;
                    case "ProvinceId":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.idquan);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.idquan);
                        break;
                }

            }
            #endregion

            #region Search
            if (!string.IsNullOrEmpty(pageinfo.keyword))
            {
                pageinfo.keyword = pageinfo.keyword.ToLower();

                lstTmp = lstTmp.Where(o => !string.IsNullOrEmpty(o.hoten) && o.hoten.Contains(pageinfo.keyword)
                 || (!string.IsNullOrEmpty(o.tendn) && o.tendn.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.dienthoai) && o.dienthoai.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.email) && o.email.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.duong) && o.duong.Contains(pageinfo.keyword)));
            }
            #endregion

            min = Helpers.FindMin(pageinfo.pageindex, pageinfo.pagesize);

            count = lstTmp.Count();

            if (!isSort)
                lstTmp = lstTmp.OrderByDescending(o => o.MaKH);

            var result = lstTmp.Skip(min).Take(pageinfo.pagesize).ToList();

            return result;
        }
    }
}
