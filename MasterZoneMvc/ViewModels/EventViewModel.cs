using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EventViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int IsPaid { get; set; }
        public decimal Price { get; set; }
        public int TotalJoined { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public string AboutEvent { get; set; }
        public string AdditionalInformation { get; set; }
        public string FeaturedImage { get; set; }
        public string TicketInformation { get; set; }
        public int Walkings { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class AddUpdateEvent_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int IsPaid { get; set; }
        public int TotalJoined { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public string AboutEvent { get; set; }
        public string AdditionalInformation { get; set; }
        public HttpPostedFile FeaturedImage { get; set; }
        public string TicketInformation { get; set; }
        public decimal Price { get; set; }
        public int Walkings { get; set; }
        public int Mode { get; set; }
        public string LandMark { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public long EventCategoryId { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = "Invalid Data!";
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Title)) { sb.Append("Event Title Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartDate)) { sb.Append("Event Start Date Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndDate)) { sb.Append("Event End Date Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartTime_24HF)) { sb.Append("Event Start Time Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndTime_24HF)) { sb.Append("Event End Required!"); vm.Valid = false; }
            //else if (Int32.IsNullOrEmpty(IsPaid)) { sb.Append("Group Description Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EventLocationURL)) { sb.Append("Evevnt Location URL Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ShortDescription)) { sb.Append("Evevnt Short Description Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AboutEvent)) { sb.Append("Event About Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AdditionalInformation)) { sb.Append("Event Additional Information  Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(TicketInformation)) { sb.Append("Event Ticket Information Required!"); vm.Valid = false; }
            // else if (String.IsNullOrEmpty(Walkings)) { sb.Append("Group Description Required!"); vm.Valid = false; }
            else if (IsPaid == 1 && Price <= 0) { sb.Append("Event Price Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LandMark)) { sb.Append(Resources.BusinessPanel.LandmarkNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.BusinessPanel.TrainingAddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(City)) { sb.Append(Resources.BusinessPanel.CityNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Country)) { sb.Append(Resources.BusinessPanel.CountryNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(State)) { sb.Append(Resources.BusinessPanel.StateNameRequired); vm.Valid = false; }
            else if (EventCategoryId <= 0) { sb.Append(Resources.BusinessPanel.Event_EventCategoryRequired); vm.Valid = false; }
            else if (FeaturedImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(FeaturedImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (FeaturedImage.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append("Image size too large. Please select a smaller image. (upto 5 MB)");
                }

                vm.Valid = isValidImage;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
    public class Event_Pagination_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public string StartDateTime_Format { get; set; }
        public DateTime EndDateTime { get; set; }
        public string EndDateTime_Format { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string FeaturedImage { get; set; }
        public string EventFeaturedImageWithPath { get; set; }
        public int IsPaid { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public int EventArchived { get; set; }

    }

    public class Event_Pagination_VM_SuperAdminPanel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public string StartDateTime_Format { get; set; }
        public DateTime EndDateTime { get; set; }
        public string EndDateTime_Format { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string FeaturedImage { get; set; }
        public string EventFeaturedImageWithPath { get; set; }
        public int IsPaid { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public int EventArchived { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public int ShowOnHomePage { get; set; }

    }

    public class EventList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 UserLoginId { get; set; }
        public long CreatedByLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
    public class Event_VM
    {
        public long Id { get; set; }
        public long EventCategoryId { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public int IsPaid { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string FeaturedImage { get; set; }
        public string EventImageWithPath { get; set; }
        public string AdditionalInformation { get; set; }
        public string AboutEvent { get; set; }
        public string TicketInformation { get; set; }
        public int Walkings { get; set; }
        public int TotalJoined { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LandMark { get; set; }
    }
    public class UpComingEvent_Vm : Event_VM
    {
        public string HostName { get; set; }
    }
    public class EventCreatorName_VM
    {
        public string EventCreatorName { get; set; }
        public string CreatedImage { get; set; }
    }

    public class EventListByMenuTag_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string FeaturedImage { get; set; }
        public string EventFeaturedImageWithPath { get; set; }
        public long UserLoginId { get; set; }
        public int StartDay { get; set; }
        public string StartMonth { get; set; }
        public string ShortDescription { get; set; }
        public string BusinessName { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string TicketInformation { get; set; }
        public string StartYear { get; set; }
    }

    public class EventList_VM
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string EventTicketQRCode { get; set; }
        public decimal Price { get; set; }
        public string FeaturedImage { get; set; }
        public string EventImageWithPath { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string StartMonth { get; set; }
        public string StartDay { get; set; }
        public int StartYear { get; set; }
        public string Address { get; set; }
        public string MenuTag { get; set; }
    }

    public class EventBranch_Pagination_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string PaymentStatus { get; set; }
        public string IsPaid { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string FeaturedImage { get; set; }
        public string EventFeaturedImageWithPath { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public string EndTime { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }

    }

    public class BranchEventDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}