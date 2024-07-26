using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    // Plan Or Package Booking 
    public class PlanBooking
    {
        [Key]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PlanId { get; set; }

        public long StudentUserLoginId { get; set; }

        // plan detials 
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public decimal PlanCompareAtPrice { get; set; }
        public int PlanDurationTypeId { get; set; }
        public string PlanDurationTypeName { get; set; }

        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }

        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public int IsTransfered { get; set; }
        public long TransferPackageId { get; set; }
        public int Repeat_Purchase { get; set; }
    }
}