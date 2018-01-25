using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBM.Business
{
    public class NotificaitonBusiness
    {
        private CRUD _crud;
        private admin_softbbmEntities _context;
        public NotificaitonBusiness()
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
        }

        public void Create(soft_Notification model)//int typeNoti, int brachId, int channelId,int userId, string code)
        {
            _context.soft_Notification.Add(model);
            _context.SaveChanges();

            //var isSubmit = false;
            //switch (typeNoti)
            //{
            //    case (int)TypeNotification.OrderOut:
            //        notifi.Contents = "Đơn hàng nhập "+ code +" cần được xử lý";
            //        notifi.Branch = brachId;
            //        notifi.DateCreate = DateTime.Now;
            //        notifi.Type = (int)TypeNotification.OrderOut;
            //        isSubmit = true;
            //        break;
            //}

            //if (isSubmit)
            //{
            //    _context.soft_Notification.Add(notifi);
            //    _context.SaveChanges();

            //}

           
        }
    }
}