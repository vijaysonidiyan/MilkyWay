using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;

namespace MilkWayIndia.Concrete
{
    public class SubscriptionRepository : ISubscription
    {
        private EFDbContext db = new EFDbContext();
        public tbl_Sector_Subscription UpdateSubscriptionStatus(int ID)
        {
            var customer = db.tbl_Sector_Subscription.FirstOrDefault(s => s.Id == ID);
            if (customer != null)
            {
                if (customer.IsActive == true)
                    customer.IsActive = false;
                else
                    customer.IsActive = true;
                customer.UpdatedOn = Models.Helper.indianTime;
                db.SaveChanges();
                return customer;
            }
            return null;
        }
    }
}