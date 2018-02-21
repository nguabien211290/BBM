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
        #region Order Input

        List<OrderModel> GetOrder_Input(PagingInfo pageinfo, int BranchesId, out int count, out int min);

        Task<bool> AddOrder_Input(OrderModel model, UserCurrent User, bool isDone = true, int OrderSuppliersId = 0);

        #endregion
        #region Order sale
        List<donhang> GetOrder_Sale(PagingInfo pageinfo, int BranchesId, out int count, out int min);

        Task<OrderModel> CreatOrder_Sale(OrderModel model, bool isDone, UserCurrent User);

        Task<Tuple<bool, string>> UpdateOrder_Sale(OrderModel model, UserCurrent User);

        OrderModel GetInfoOrder(int Id);
        #endregion

        #region Order Braches
        List<OrderModel> GetOrder_Branches(PagingInfo pageinfo, int BranchesId, out int count, out int min);
        Task<bool> AddOrder_Branches(OrderModel model, UserCurrent User);
        #endregion
    }
}
