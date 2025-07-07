using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Paytm_Request
    {
        [Key]
        public int? ID { get; set; }
        public string OrderNo { get; set; }
        public int? CustomerID { get; set; }
        public int? PlanID { get; set; }
        public string SubscriptionId { get; set; }
        public string Signature { get; set; }
        public string TxnToken { get; set; }
        public Boolean? Authenticated { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? PreNotifyDate { get; set; }
        public Boolean? PreNofifyCall { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Response { get; set; }
        public string TransactionID { get; set; }
        public string ReferebceId { get; set; }
        public string PaytmReferenceId { get; set; }
    }
}