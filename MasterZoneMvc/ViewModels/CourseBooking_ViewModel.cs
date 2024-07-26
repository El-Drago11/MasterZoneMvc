using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CourseBooking_ViewModel
    {

        public long Id { get; set; }
        public long OrderId { get; set; }
        public long CourseId { get; set; }
        public long StudentUserLoginId { get; set; }
        public long UserLoginIdforlink { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public string StudentName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessOwnerName { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string CourseImage { get; set; }
        public string CourseImageWithPath { get; set; }
        public string GroupName { get; set; }
        public int Duration { get; set; }
        public string DurationType { get; set; }

        // Course detials 
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoursePriceType { get; set; }
        public string Startdate_FormatDates { get; set; }
        public string Enddate_FormatDates { get; set; }
    }
}