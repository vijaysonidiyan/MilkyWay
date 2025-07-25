﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Advertisement
    {
        [Key]
        public int? ID { get; set; }
        public int? AdsType { get; set; }
        public string Title { get; set; }
        public string WebsiteLink { get; set; }
        public string AppLink { get; set; }
        public string Description { get; set; }
        public string Mobile { get; set; }
        public string PhotoPath { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public int? ClickCount { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class tbl_Cust_Advertisement
    {
        [Key]
        public int? ID { get; set; }
        public int? CustomerID { get; set; }
        public int? AdvertisementID { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class AdvertisementModel
    {
        public int? ID { get; set; }
        public int? AdsType { get; set; }
        public string Title { get; set; }
        public string WebsiteLink { get; set; }
        public string AppLink { get; set; }
        public string Description { get; set; }
        public string Mobile { get; set; }
        public string PhotoPath { get; set; }
        public string StartDate { get; set; }
        public string ExpiredDate { get; set; }
    }
}