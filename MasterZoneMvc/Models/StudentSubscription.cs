using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class StudentSubscription
    {
        [Key]
        public long Id { get; set; }

        public long PlanId { get; set; }
        public long StudentId { get; set; }

        // plan detials 
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public decimal PlanCompareAtPrice { get; set; }
        public int BusinessPlanDurationTypeId { get; set; }
        public string BusinessPlanDurationTypeName { get; set; }

        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }

        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public long CouponId { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetAmount { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}