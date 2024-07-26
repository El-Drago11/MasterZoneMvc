using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class TrainingBooking_ViewModel
    {
        
            public long Id { get; set; }
            public long UserLoginId { get; set; }
            public long TrainingId { get; set; }
            public long OrderId { get; set; }
            public long userLoginIdForlink { get; set; }
            public string TrainingName { get; set; }
            public string Email { get; set; }
            public string BusinessOwnerName { get; set; }
           public string PhoneNumber { get; set; }
           public string PhoneNumber_CountryCode { get; set; }
           public string BusinessName { get; set; }
            public string CreatedOnString { get; set; }
            public decimal Price { get; set; }
            public decimal PlanCompareAtPrice { get; set; }
            public int IsCompleted { get; set; }
            public string Duration { get; set; }
            public string TrainingClassDays { get; set; }
            public int TotalLectures { get; set; }
            public int TotalClasses { get; set; }
            public int TotalSeats { get; set; }
            public int TotalCredits { get; set; }
            public string Startdate_FormatDates { get; set; }
            public DateTime StartDate_DateTimeFormat { get; set; }
            public string Enddate_FormatDates { get; set; }
            public DateTime EndDate_DateTimeFormat { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime UpdatedOn { get; set; }

            public long LicenseId { get; set; }
            public long LicenseBookingId { get; set; }
            public string TrainingImageWithPath { get; set; }
            public string TrainingImage { get; set; }
            public string StudentName { get; set; }


    }
}