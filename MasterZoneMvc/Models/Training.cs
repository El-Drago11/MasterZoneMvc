using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Training
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long InstructorUserLoginId { get; set; }
        public string InstructorEmail { get; set; } //InstructorEmail
        public string InstructorMobileNumber { get; set; } //InstructorMobileNumber
        public string InstructorAlternateNumber { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int IsPaid { get; set; }
        public decimal Price { get; set; }
        public string AdditionalPriceInformation { get; set; }

        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }

        public string CenterName { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LocationUrl { get; set; }

        public string MusicType { get; set; }
        public string EnergyLevel { get; set; }
        public string DanceStyle { get; set; }

        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public string TrainingImage { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string Duration { get; set; }
        public string TrainingClassDays { get; set; }
        public int TotalLectures { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSeats { get; set; }
        public int TotalCredits { get; set; }
        public string AdditionalInformation { get; set; }
        public string ExpectationDescription { get; set; }
        public string TrainingRules { get; set; }
        public string BecomeInstructorDescription { get; set; }

        public long LicenseId { get; set; }
        public long LicenseBookingId { get; set; }
        public int ShowOnHomePage { get; set; }

    }
}