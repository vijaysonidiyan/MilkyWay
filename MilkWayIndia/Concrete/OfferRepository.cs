using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using MilkWayIndia.Abstract;
namespace MilkWayIndia.Concrete
{
    public class OfferRepository : IOffer
    {
        private EFDbContext db = new EFDbContext();

        public tbl_New_Customermsg UpdateVendorCatSubcatStatus(int Id)
        {
            var customer = db.tbl_New_Customermsg.FirstOrDefault(s => s.Id == Id);
            if (customer != null)
            {
                if (customer.IsActive == true)
                    customer.IsActive = false;
                else
                    customer.IsActive = true;

                db.SaveChanges();
                return customer;
            }
            return null;
        }


        public tbl_SectorWiseMsg UpdateSectorMsg(int Id)
        {
            var customer = db.tbl_SectorWiseMsg.FirstOrDefault(s => s.Id == Id);
            if (customer != null)
            {
                if (customer.IsActive == true)
                    customer.IsActive = false;
                else
                    customer.IsActive = true;

                db.SaveChanges();
                return customer;
            }
            return null;
        }
    }
}