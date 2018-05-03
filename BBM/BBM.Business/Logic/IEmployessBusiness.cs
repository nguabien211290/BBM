﻿using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IEmployessBusiness
    {
        List<EmployeeModel> GetEmployessby(PagingInfo pageinfo, out int count, out int min);
    }
}
