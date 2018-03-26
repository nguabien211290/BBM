using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Repository
{
    public class ProductRepository : Repository<shop_sanpham>
    {
        public ProductRepository(admin_softbbmEntities dbContext) : base(dbContext) { }

        public List<shop_sanpham> SearchBy1(PagingInfo pageinfo, out int count, out int min, out Dictionary<string, object> values, UserCurrent User)
        {
            values = new Dictionary<string, object>();

            var lstTmp = GetAll();

            #region Fillter
            if (pageinfo.filterby != null && pageinfo.filterby.Count > 0)
            {
                foreach (var item in pageinfo.filterby)
                {
                    var key = 0;

                    if (item.Fiter.Equals("Price") || item.Fiter.Equals("Stock"))
                        key = int.Parse(item.Name);
                    else
                        key = int.Parse(item.Value);

                    switch (item.Fiter)
                    {
                        case "Catalog":
                            lstTmp = lstTmp.Where(o => o.CatalogId > 0 && o.CatalogId.Equals(key));
                            break;
                        case "Suppliers":
                            lstTmp = lstTmp.Where(o => o.SuppliersId > 0 && o.SuppliersId.Equals(key));
                            break;
                        case "Status":
                            lstTmp = lstTmp.Where(o => o.Status > 0 && o.Status.Equals(key));
                            break;
                        case "VAT":
                            lstTmp = lstTmp.Where(o => o.StatusVAT > 0 && o.StatusVAT.Equals(key));
                            break;
                        case "Price":
                        case "Stock":
                            lstTmp = lstTmp.Where(o => item.Ids.Contains(o.id));
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
                            lstTmp = lstTmp.OrderByDescending(o => o.DateCreate);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.DateCreate);
                        break;
                    case "Id":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.id);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.id);
                        break;
                    case "Barcode":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.Barcode);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.Barcode);
                        break;
                    case "PriceBase":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.PriceBase);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.PriceBase);
                        break;
                    case "PriceBase_Old":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.PriceBase_Old);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.PriceBase_Old);
                        break;
                    case "PriceCompare":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.PriceCompare);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.PriceCompare);
                        break;

                    case "PriceInput":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.PriceInput);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.PriceInput);
                        break;
                    case "ProductName":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.tensp);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.tensp);
                        break;
                    case "Code":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.CatalogId);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.CatalogId);
                        break;
                    case "StatusVAT":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.StatusVAT);
                        else
                            lstTmp = lstTmp.OrderBy(o => o.StatusVAT);
                        break;
                    case "Stock_Sum":
                        if (pageinfo.sortbydesc)
                            lstTmp = lstTmp.OrderByDescending(o => o.soft_Branches_Product_Stock.Sum(p => p.Stock_Total));
                        else
                            lstTmp = lstTmp.OrderBy(o => o.soft_Branches_Product_Stock.Sum(p => p.Stock_Total));
                        break;
                }

            }
            #endregion
            #region Search
            if (!string.IsNullOrEmpty(pageinfo.keyword))
            {
                pageinfo.keyword = pageinfo.keyword.ToLower();
                lstTmp = lstTmp.Where(o =>
                 (!string.IsNullOrEmpty(o.tensp) && o.tensp.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.Barcode) && o.Barcode.Contains(pageinfo.keyword))
                 || (!string.IsNullOrEmpty(o.masp) && o.masp.Contains(pageinfo.keyword))
                );
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
