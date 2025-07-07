using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
namespace MilkWayIndia.Abstract
{
    //public class IOffer
    //{
    //}

    public interface IOffer
    {
        //tbl_Vendor_CatSubcat_Assign UpdateVendorCatSubcatStatus(int ID);
        tbl_New_Customermsg UpdateVendorCatSubcatStatus(int Id);

        tbl_SectorWiseMsg UpdateSectorMsg(int Id);
    }
}