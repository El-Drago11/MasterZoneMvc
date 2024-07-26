using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EventBooking_ViewModel
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public long UserLoginId { get; set; }
        public long OrderId { get; set; }
        public string EventTicketQRCode { get; set; }
        public string EventTicketQRCodeWithPath { get; set; }
        public long userLoginIdForlink { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public string Title { get; set; }
        public string BusinessOwnerName { get; set; }
        public decimal Price { get; set; }
        public string StudentName { get; set; }
        public string BusinessName { get; set; }
        public string CreatedOnString { get; set; }
        public string UpdatedOn { get; set; }
        public string Startdate_FormatDates { get; set; }
        public string Enddate_FormatDates { get; set; }
        public string EventImageWithPath { get; set; }
        public string FeaturedImage { get; set; }

    }
}