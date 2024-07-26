using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class TrainingBooking
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long TrainingId { get; set; }
        public long OrderId { get; set; }
        public string TrainingName { get; set; }
        public decimal Price { get; set; }
        public decimal PlanCompareAtPrice { get; set; }
        public int IsCompleted { get; set; }
        public string Duration { get; set; }
        public string TrainingClassDays { get; set; }
        public int TotalLectures { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSeats { get; set; }
        public int TotalCredits { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public long LicenseId { get; set; }
        public long LicenseBookingId { get; set; }

    }
}