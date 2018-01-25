using BBM.Business.Model.Entity;
using BBM.Business.Infractstructure;
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
    public partial class OrderRepository : Repository<soft_Order>
    {
        public OrderRepository(admin_softbbmEntities dbContext) : base(dbContext) { }
    }
}
