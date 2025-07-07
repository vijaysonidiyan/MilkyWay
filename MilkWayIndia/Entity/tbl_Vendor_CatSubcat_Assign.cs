using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MilkWayIndia.Entity
{
    public class tbl_Vendor_CatSubcat_Assign
    {
        [Key]
        public int? ID { get; set; }
        public int? VendorId { get; set; }
        public string VendorCatName { get; set; }
        public int? ParentCat { get; set; }
        public int? Subcat { get; set; }
        public Boolean? IsActive { get; set; }
        
        
        
    }
}