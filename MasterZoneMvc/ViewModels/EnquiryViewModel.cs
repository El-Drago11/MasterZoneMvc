using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EnquiryViewModel
    {
        public long Id { get; set; }

        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public DateTime DOB_DateTimeFormat { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }

        public string Activity { get; set; }
        public long LevelId { get; set; }
        public long BusinessPlanId { get; set; }
        public long ClassId { get; set; }
        public string StartFromDate { get; set; }
        public DateTime StartFromDate_DateTimeFormat { get; set; }
        public string Status { get; set; }

        public long StaffId { get; set; }
        public string FollowUpDate { get; set; }
        public DateTime FollowUpDate_DateTimeFormat { get; set; }
        public string Notes { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class Enquiry_Pagination_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long StaffId { get; set; }
        public string StaffName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public int EnquiryStatus { get; set; }
        public string Status { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
    }

    public class RequestEnquiry_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public DateTime DOB_DateTimeFormat { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }

        public string Activity { get; set; }
        public long LevelId { get; set; }
        public long BusinessPlanId { get; set; }
        public long ClassId { get; set; }
        public string StartFromDate { get; set; }
        public DateTime StartFromDate_DateTimeFormat { get; set; }
        public string Status { get; set; }

        //public long StaffId { get; set; }
        //public string FollowUpDate { get; set; }
        //public DateTime FollowUpDate_DateTimeFormat { get; set; }
        public string Notes { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }

            StringBuilder sb = new StringBuilder();

            if (Mode == 2 && Id <= 0) { sb.Append(Resources.ErrorMessage.InvalidIdErrorMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.Enquiry_NameRequird); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Gender)) { sb.Append(Resources.BusinessPanel.Enquiry_GenderRequird); vm.Valid = false; }
            else if (String.IsNullOrEmpty(DOB)) { sb.Append(Resources.BusinessPanel.Enquiry_DOBRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Email) && !EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.BusinessPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(PhoneNumber) && !PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.BusinessPanel.EnterValidPhoneNumber_ErrorMessage); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(AlternatePhoneNumber) && !PhoneNumberValidationHelper.IsValidPhoneNumber(AlternatePhoneNumber)) { sb.Append(Resources.BusinessPanel.EnterValidPhoneNumber_ErrorMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.BusinessPanel.AddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Activity)) { sb.Append(Resources.BusinessPanel.Enquiry_ActivityRequired); vm.Valid = false; }
            else if (LevelId <= 0) { sb.Append(Resources.BusinessPanel.Enquriy_LevelRequired); vm.Valid = false; }
            else if (BusinessPlanId <= 0 && ClassId <= 0) { sb.Append(Resources.BusinessPanel.Enquiry_EnquiredForSelectionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Status)) { sb.Append(Resources.BusinessPanel.Enquiry_StatusRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartFromDate)) { sb.Append(Resources.BusinessPanel.Enquiry_StartFromDateRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(StartFromDate) && Convert.ToDateTime(StartFromDate).Date < DateTime.UtcNow.Date) { sb.Append(Resources.BusinessPanel.Enquiry_StartFromDateCannotBeLessThanTodaysDate_ErrorMessage); vm.Valid = false; }
            //else if (StaffId <= 0) { sb.Append(Resources.BusinessPanel.Enquiry_StaffRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(FollowUpDate)) { sb.Append(Resources.BusinessPanel.Enquiry_FollowUpDateRequired); vm.Valid = false; }
            //else if (!String.IsNullOrEmpty(FollowUpDate) && Convert.ToDateTime(FollowUpDate).Date < DateTime.UtcNow.Date) { sb.Append(Resources.BusinessPanel.Enquiry_FollowUpDateCannotBeLessThanTodaysDate_ErrorMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Notes)) { sb.Append(Resources.BusinessPanel.Enquiry_NotesRequired); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvalidMode); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }

    }

    public class EnquiryDetail_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public int IsReplied { get; set; }
        public string BusinessName { get; set; }
        public string RepliedOn_FormatDate { get; set; }
        public string ReplyBody { get; set; }
        public string Description { get; set; }
    }

    public class EnquiryDescription_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public DateTime DOB_DateTimeFormat { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }

        public string Activity { get; set; }
        public long LevelId { get; set; }
        public long BusinessPlanId { get; set; }
        public long ClassId { get; set; }
        public string StartFromDate { get; set; }
        public DateTime StartFromDate_DateTimeFormat { get; set; }
        public string Status { get; set; }

        public long StaffId { get; set; }
        public string FollowUpDate { get; set; }
        public DateTime FollowUpDate_DateTimeFormat { get; set; }
        public string Notes { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string StaffName { get; set; }
        public string BusinessPlanName { get; set; }
        public string ClassName { get; set; }
        public string LevelTitle { get; set; }
        public int EnquiryStatus { get; set; }
    }

    public class BusinessEnquiryDetail_VM
    {
        public long Id { get; set; }
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }
        public string Activity { get; set; }
        public string StartFromDate { get; set; }
        public string Notes { get; set; }
        public string StartFromDate_DateTimeFormat { get; set; }
        public string Status { get; set; }
        public string FollowUpDate { get; set; }
        public string FollowUpDate_DateTimeFormat { get; set; }
    }
    public class EnquiryStaffListUpdate_VM
    {
        public long Id { get; set; }
        public long StaffId { get; set; }
        public string FollowUpDate { get; set; }
        public int Mode { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }

            StringBuilder sb = new StringBuilder();

            if (StaffId <= 0) { sb.Append(Resources.BusinessPanel.Enquiry_StaffRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(FollowUpDate)) { sb.Append(Resources.BusinessPanel.Enquiry_FollowUpDateRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }
    }

    public class EnquiryFollowCommentList_VM
    {
        public long Id { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
    }
}