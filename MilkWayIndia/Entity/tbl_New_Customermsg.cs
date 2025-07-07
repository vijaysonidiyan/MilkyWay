using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MilkWayIndia.Entity
{
    public class tbl_New_Customermsg
    {

        [Key]
        public int? Id { get; set; }
        
        public string Message { get; set; }
        public int? StateId { get; set; }
        public string MobileNo { get; set; }
        public string Whatsappno { get; set; }
        public Boolean? IsActive { get; set; }
    }
}