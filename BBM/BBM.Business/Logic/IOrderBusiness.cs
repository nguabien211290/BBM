﻿using BBM.Business.Model.Entity;
using BBM.Business.Model.Module;
using BBM.Business.Models.Module;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IOrderBusiness
    {
        List<donhang> GetOrder_Sale(PagingInfo pageinfo, int BranchesId, out int count, out int min);

        List<OrderModel> GetOrder_Inside(PagingInfo pageinfo, int BranchesId, out int count, out int min);

        Task<bool> AddOrder_Input(OrderModel model, UserCurrent User, bool isDone = true, int OrderSuppliersId = 0);

        Task<OrderModel> CreatOrder_Sale(OrderModel model, bool isDone, UserCurrent User);

        Task<Tuple<bool, string>> UpdateOrder_Sale(OrderModel model, UserCurrent User);

        OrderModel GetInfoOrder(int Id);
    }
}
