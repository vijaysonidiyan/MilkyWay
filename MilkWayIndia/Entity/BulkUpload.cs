using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_BulkUpload
    {
        [Key]
        public int? ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int? TotalItem { get; set; }
        public int? UploadItem { get; set; }
        public bool? IsUpload { get; set; }
        public DateTime CreatedOn { get; set; }
                
        public List<tbl_Product_Temp> list { get; set; }
    }
}