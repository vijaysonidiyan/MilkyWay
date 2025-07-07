using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_ProductVendor_Master
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? VendorId { get; set; }
        public int? OrderBy { get; set; }
        public int? RewardPoint { get; set; }
        public decimal? Price { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? Profit { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDaily { get; set; }
        public bool? IsAlternate { get; set; }
        public bool? IsMultiple { get; set; }
        public bool? IsWeekDay { get; set; }        

    }
}