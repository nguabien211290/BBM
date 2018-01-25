using BBM.Business.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Repository
{
    public interface IProductRepository : IRepository<shop_sanpham>
    {
        shop_sanpham GetSingle(int fooId);
    }
}
