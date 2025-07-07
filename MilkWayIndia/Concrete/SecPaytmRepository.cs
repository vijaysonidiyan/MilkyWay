using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Concrete
{
    public class SecPaytmRepository : ISecPaytm
    {
        private EFDbContext db = new EFDbContext();
        private readonly Random _random = new Random();
        public int SavePaytmRequest(tbl_Paytm_Request model)
        {
            try
            {
                db.tbl_Paytm_Request.Add(model);
                db.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return 0;
        }

        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public string GenerateOrderNo()
        {
            var date = DateTime.Now.ToString("yyyyMMddHHmm");
            try
            {                
                var orderNo = RandomNumber(1000, 9999);
                return date + orderNo;               
            }
            catch
            {

            }
            return date;
        }

        public tbl_Paytm_Request GetDetailByPaytmToken(string token)
        {
            var detail = db.tbl_Paytm_Request.FirstOrDefault(s => s.TxnToken == token);
            if (detail != null)
                return detail;
            else
                return null;
        }

        public tbl_Paytm_Request GetDetailByCustomerID(int? CustomerID)
        {
            var detail = db.tbl_Paytm_Request.OrderByDescending(s => s.CreatedDate).FirstOrDefault(s => s.CustomerID == CustomerID && s.Authenticated == true);
            if (detail != null)
            {
                var currentDate = Models.Helper.indianTime;
                if (detail.StartDate < currentDate && detail.EndDate > currentDate)
                    return detail;
            }

            return null;
        }

        public tbl_Paytm_Request UpdatePaytmResponseByOrderID(tbl_Paytm_Request model)
        {
            var update = db.tbl_Paytm_Request.FirstOrDefault(s => s.OrderNo == model.OrderNo && s.UpdatedDate == null);
            if (update != null)
            {
                update.Authenticated = model.Authenticated;
                update.TransactionID = model.TransactionID;
                update.UpdatedDate = model.UpdatedDate;
                db.SaveChanges();                
            }
            return update;
        }

        public tbl_Paytm_Request UpdatePaytmPrenotify(tbl_Paytm_Request model)
        {
            var update = db.tbl_Paytm_Request.FirstOrDefault(s => s.OrderNo == model.OrderNo);
            if (update != null)
            {
                update.PreNotifyDate = model.PreNotifyDate;
                update.ReferebceId = model.ReferebceId;
                update.PaytmReferenceId = model.PaytmReferenceId;
                update.UpdatedDate = model.UpdatedDate;
                db.SaveChanges();
            }
            return update;
        }

        public int InsertPaytmPreNotify(tbl_Paytm_Request_Details model)
        {
            try
            {
                db.tbl_Paytm_Request_Details.Add(model);
                db.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return 0;
        }
    }
}