using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Customer_Master
    {
        [Key]
        public int? ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int? SectorId { get; set; }
        public int? BuildingId { get; set; }
        public string Photo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public decimal? Credit { get; set; }
        public string ReferralCode { get; set; }
        public Nullable<int> ReferralID { get; set; }
        public int? FlatId { get; set; }
        public DateTime? SubnFromDate { get; set; }
        public DateTime? SubnToDate { get; set; }
        public string fcm_token { get; set; }
        public int? RewardPoint { get; set; }        
        public int? OrderBy { get; set; }
        public Boolean? IsActive { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string CustomerType { get; set; }
    }

    public class tbl_Cusomter_Order_Track
    {
        [Key]
        public int? ID { get; set; }
        public int? CustomerID { get; set; }
        public int? ProductID { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Qty { get; set; }
        public DateTime? NextOrderDate { get; set; }
        public Boolean? IsActive { get; set; }
        public string OrderFlag { get; set; }
        public int? AttributeId { get; set; }
    }

    public class SP_CustomerSortOrderUpdate
    {
        [Key]
        public int CustomerId { get; set; }
        public int SectorId { get; set; }        
    }
}