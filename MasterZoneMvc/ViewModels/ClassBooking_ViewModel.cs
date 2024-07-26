using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ClassBooking_ViewModel
    {
        
            public long Id { get; set; }
            public long OrderId { get; set; }
            public long ClassId { get; set; }
            public long StudentUserLoginId { get; set; }
            public string ClassQRCode { get; set; }
            public long userLoginId { get; set; }
            public string BusinessOwnerName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string PhoneNumber_CountryCode { get; set; }
            public string ClassQRCodeWithPath { get; set; }
            public string GroupName { get; set; }
            public string BatchStartTime { get; set; }
            public string BatchEndTime { get; set; }
            public string BatchName { get; set; }
           public string StudentName { get; set; }
            public string ClassImageWithPath { get; set; }
          public string ClassCategoryImageWithPath { get; set; }
            public string CreatedOn { get; set; }
         
            public string CreatedOnString { get; set; }
            public decimal Price { get; set; }
            public string Name { get; set; }
            public string BusinessName { get; set; }
            public string Description { get; set; }
            public string ClassPriceType { get; set; }
            public string Startdate_FormatDates { get; set; }
            public string Enddate_FormatDates { get; set; }
            public long BatchId { get; set; }
            public int Repeat_Purchase { get; set; }
        
    }
}