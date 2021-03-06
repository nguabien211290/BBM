﻿using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface ICatalogBusiness
    {
        List<CatalogModel> GetCatalog(PagingInfo pageinfo, out int count, out int min);
        Task<bool> DeleteCatalog(int id);
        Task<bool> Updateatalog(CatalogModel model, int UserId);
        Task<bool> CreateCatalog(CatalogModel model, int UserId);
    }
}
