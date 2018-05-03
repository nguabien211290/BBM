using BBM.Business.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IImportBusiness
    {
        Task<List<shop_sanpham>> ImportData(DataSet data, int UserId);
    }
}
