using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tblSliders
    {
        [Key]
        public int? ID { get; set; }
        public int? SortOrder { get; set; }
        public int? CategoryID { get; set; }
        public int? SectorID { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string PhotoPath { get; set; }
        public string Mobile { get; set; }
        public string WebsiteLink { get; set; }
        public string AppLink { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
        [NotMapped]
        public string SectorName { get; set; }
    }

    public class tblSliderSectors
    {
        public int? ID { get; set; }
        public int? SliderID { get; set; }
        public int? SectorID { get; set; }
    }
}