using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tbl_Paytm_Request_Details
    {
        public int? ID { get; set; }
        public int? CustomerID { get; set; }
        public int? PaytmRequestID { get; set; }
        public string ReferenceID { get; set; }
        public decimal? Amount { get; set; }
        public string PaytmReferenceID { get; set; }
        public DateTime? NotifyDate { get; set; }
        public string PrenotifyRequest { get; set; }
        public string PrenotifyResponse { get; set; }
        public string RenewalOrderID { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string RenewalRequest { get; set; }
        public string RenewalResponse { get; set; }
        public Boolean IsConfirm { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string RenewalTxnID { get; set; }
        public string BankTxnID { get; set; }
        public DateTime? TxnDate { get; set; }
    }
}