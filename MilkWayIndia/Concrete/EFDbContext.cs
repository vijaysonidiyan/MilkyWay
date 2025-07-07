using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using System.Data.Entity;

namespace MilkWayIndia.Concrete
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base("name=MilkWayIndia")
        {
        }
        public DbSet<tbl_BulkUpload> tbl_BulkUpload { get; set; }
        public DbSet<tbl_Product_Temp> tbl_Product_Temp { get; set; }
        public DbSet<tblSliders> tblSliders { get; set; }
        public DbSet<tblSliderSectors> tblSliderSectors { get; set; }
        public DbSet<tblMedicines> tblMedicine { get; set; }
        public DbSet<tbl_Customer_Master> tbl_Customer_Master { get; set; }
        public DbSet<tbl_Product_Master> tbl_Product_Master { get; set; }
        public DbSet<tbl_Paytm_Request> tbl_Paytm_Request { get; set; }
        public DbSet<tbl_Paytm_Request_Details> tbl_Paytm_Request_Details { get; set; }
        public DbSet<tbl_Advertisement> tblAdvertisement { get; set; }
        public DbSet<tbl_Cust_Advertisement> tbl_Cust_Advertisement { get; set; }
        public DbSet<tbl_Cusomter_Order_Track> tbl_Cusomter_Order_Track { get; set; }

        public DbSet<tblBillPayCircle> tblBillPayCircle { get; set; }
        public DbSet<tblBillPayCity> tblBillPayCity { get; set; }
        public DbSet<tblBillPayOperator> tblBillPayOperator { get; set; }
        public DbSet<tblBillPayService> tblBillPayService { get; set; }
        public DbSet<tblBillPayProvider> tblBillPayProvider { get; set; }

        public DbSet<tbl_Attributes> tbl_Attributes { get; set; }
        public DbSet<tbl_Product_Attributes> tbl_Product_Attributes { get; set; }

        public DbSet<tbl_Vendor_CatSubcat_Assign> tbl_Vendor_CatSubcat_Assign { get; set; }

        public DbSet<tbl_New_Customermsg> tbl_New_Customermsg { get; set; }
        public DbSet<tbl_SectorWiseMsg> tbl_SectorWiseMsg { get; set; }

        public DbSet<tbl_Vendor_Product_Assign> tbl_Vendor_Product_Assign { get; set; }

        public DbSet<tbl_Sector_Subscription> tbl_Sector_Subscription { get; set; }
    }
}