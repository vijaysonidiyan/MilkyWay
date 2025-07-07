using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tblMedicines
    {
        [Key]
        public int? ID { get; set; }        
        public int? CustomerId { get; set; }
        public string PhotoPath { get; set; }        
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [NotMapped]
        public string CustomerName { get; set; }
        [NotMapped]
        public string CreateDateStr { get; set; }
    }
}