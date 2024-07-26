using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class PlanBooking_ViewModel
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PlanId { get; set; }

        public long StudentUserLoginId { get; set; }
        public long userLoginIdForlink { get; set; }
        public long UserLoginId { get; set; }
        public int PlanBookingType { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string PlanPermission { get; set; }
        public string BusinessOwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public string PlanName { get; set; }
        public string BusinessName { get; set; }
        public string PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public decimal PlanCompareAtPrice { get; set; }
        public int PlanDurationTypeId { get; set; }
        public int Repeat_Purchase { get; set; }
        public int? PlanType { get; set; }
        public string PlanDurationTypeName { get; set; }

        public string Startdate_FormatDates { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }

        public string Enddate_FormatDates { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime UpdatedOn { get; set; }

        public int IsTransfered { get; set; }
        public long TransferPackageId { get; set; }

        public string StudentName { get; set; }
        public string PlanImage { get; set; }
        public string PlanImageWithPath { get; set; }
        public string MainPlanImageWithPath { get; set; }
        public string Description { get; set; }
    }


}