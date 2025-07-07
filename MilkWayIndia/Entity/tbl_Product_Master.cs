using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Product_Master
    {
        [Key]
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string ProductName { get; set; }
        public string Code { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public int? RewardPoint { get; set; }
        public decimal? Subscription { get; set; }
        public string Detail { get; set; }
        public string Image { get; set; }
        public string YoutubeTitle { get; set; }
        public string YoutubeURL { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDaily { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? Profit { get; set; }
        public int? OrderBy { get; set; }
    }
}