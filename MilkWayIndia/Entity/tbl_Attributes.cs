using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Attributes
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class tbl_Vendor_Product_Assign
    {
        public int? Id { get; set; }
        public Boolean IsActive { get; set; }
        
    }

    public class tbl_Product_Attributes
    {
        public int? ID { get; set; }
        public int? ProductID { get; set; }
        public int? VendorProductAssignID { get; set; }
        public int? AttributeID { get; set; }
        public decimal? MRPPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? Profit { get; set; }
        public decimal? SellPrice { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean? IsDeleted { get; set; }


        public decimal? B2BSellPrice { get; set; }

        public decimal? B2BProfit { get; set; }

        public int? VendorCatId { get; set; }
        public int? VendorId { get; set; }
    }
}