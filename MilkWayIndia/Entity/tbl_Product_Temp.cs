using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Product_Temp
    {
        [Key]
        public int ID { get; set; }
        public int? UploadID { get; set; }
        public int? SortOrder { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal? MRP { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? ProfitL { get; set; }
        public decimal? ProfitP { get; set; }
        public decimal? Profit { get; set; }
        public string Details { get; set; }
        public string YoutubeTitle { get; set; }
        public string YoutubeURL { get; set; }
        public decimal? Subcription { get; set; }
        public bool? Status { get; set; }
        public bool? IsDaily { get; set; }
        public bool? IsUpload { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ErrorMessage { get; set; }

        [ForeignKey("UploadID")]
        public virtual tbl_BulkUpload tbl_BulkUpload { get; set; }
    }
}