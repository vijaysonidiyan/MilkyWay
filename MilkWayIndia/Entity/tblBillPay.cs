using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Entity
{
    public class tblBillPayCircle
    {
        [Key]
        public int? ID { get; set; }
        public string Name { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int CircleCode { get; set; }
        public int SortOrder { get; set; }
    }

    public class tblBillPayCity
    {
        [Key]
        public int? ID { get; set; }
        public string Name { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class tblBillPayService
    {
        [Key]
        public int? ID { get; set; }
        public int? SortOrder { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class tblBillPayOperator
    {
        [Key]
        public int? ID { get; set; }
        public string Name { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? OperatorCode { get; set; }

        public string Type { get; set; }
    }

    public class tblBillPayProvider
    {
        [Key]
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? OperatorID { get; set; }
        public int? CircleID { get; set; }
        public int? CityID { get; set; }
        public Boolean? IsPartial { get; set; }
        public string OperatorCode { get; set; }
        public int? ServiceID { get; set; }
        public string NumberTag { get; set; }
        public string FieldTag1 { get; set; }
        public string FieldTag2 { get; set; }
        public string FieldTag3 { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}