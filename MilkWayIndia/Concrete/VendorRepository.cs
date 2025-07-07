using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using MilkWayIndia.Abstract;
namespace MilkWayIndia.Concrete
{
    public class VendorRepository : IVendorCatSubcat
    {
        private EFDbContext db = new EFDbContext();

        public tbl_Vendor_CatSubcat_Assign UpdateVendorCatSubcatStatus(int ID)
        {
            var customer = db.tbl_Vendor_CatSubcat_Assign.FirstOrDefault(s => s.ID == ID);
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