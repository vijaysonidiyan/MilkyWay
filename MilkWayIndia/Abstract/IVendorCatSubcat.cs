using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;


namespace MilkWayIndia.Abstract
{
    public interface IVendorCatSubcat
    {
        tbl_Vendor_CatSubcat_Assign UpdateVendorCatSubcatStatus(int ID);
    }
}