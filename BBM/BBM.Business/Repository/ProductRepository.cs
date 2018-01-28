using BBM.Business.Model.Entity;
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

        //public shop_sanpham GetSingle(int fooId)
        //{
        //    var query = GetById(fooId);
        //    return query;
        //}
    }
}
