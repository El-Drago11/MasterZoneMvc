using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Views
{
    public class TrainingBookingViewModel
    {
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

    public class TrainingBooking_Pagination_BO_VM
    {
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

        public string CreatedOn_FormatDate { get; set; }
        public long StudentUserLoginId { get; set; }
        public string StudentName { get; set; }
        public string BusinessStudentProfileImageWithPath { get; set; }
        public string BusinessStudentProfileImage { get; set; }
        public string StudentProfileImage { get; set; }
        public string StudentProfileImageWithPath { get; set; }
        public int OrderStatus { get; set; }


        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class TrainingBookingDetail_VM : TrainingBookingViewModel
    {
        public string CreatedOn_FormatDate { get; set; }
    }
}