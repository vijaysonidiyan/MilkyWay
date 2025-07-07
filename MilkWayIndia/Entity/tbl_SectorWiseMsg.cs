using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MilkWayIndia.Entity
{
    public class tbl_SectorWiseMsg
    {
        [Key]
        public int? Id { get; set; }

        public string Message { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public int? SectorId { get; set; }
        public DateTime? Updatedon { get; set; }
        public Boolean? IsActive { get; set; }
    }
}