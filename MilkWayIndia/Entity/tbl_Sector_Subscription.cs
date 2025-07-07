using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MilkWayIndia.Entity
{
    public class tbl_Sector_Subscription
    {
        [Key]
        public int? Id { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public int? SectorId { get; set; }
        public string SubscriptionAmount { get; set; }
        
        public Boolean? IsActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
        
    }
}