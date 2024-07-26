using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StudentViewModel
    {

    }

    public class StudentProfileSetting_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MasterId { get; set; }
        public int EmailConfirmed { get; set; }
        public int status { get; set; }
    }

    public class RequestStudentProfile_VM
    {
        //public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public HttpPostedFile ProfileImage { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public int Mode { get; set; }

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
            if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Email) && !EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired); vm.Valid = false; }

            //else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append("PhoneNumber Required!"); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(PhoneNumber) && !PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }

            if (ProfileImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ProfileImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (ProfileImage.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.FileSizeRequired);
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

    public class StudentList_ForBusiness_VM
    {
        public long StudentId { get; set; }
        public long StudentUserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentFullName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public string BusinessStudentProfileImageWithPath { get; set; }
        public int Status { get; set; }

    }

    public class StudentInstructor_Pagination_VM
    {
        public long Id { get; set; }
        public long StudentUserLoginId { get; set; }

        public string StudentName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Email { get; set; }
        public string InstructorName { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

        public long ClassBookingId { get; set; }
        public long InstructorLoginId { get; set; }
        public string StudentImageWithPath { get; set; }
        public string ClassImageWithPath { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string InstructorBusinessLogoWithPath { get; set; }

    }

    public class StudentList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class StudentBasicDetail_VM
    {
        public Int64 Id { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string Email { get; set; }
        public string BusinessStudentProfileImage { get; set; }
        public string BusinessStudentProfileImageWithPath { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public int IsBlocked  { get; set; }
        public int Gender { get; set; }
        public string GenderString { get; set; }
    }

    public class StudentClassForBO_Pagination_VM
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public long StudentUserLoginId { get; set; }

        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ClassMode { get; set; }
        public string ClassDays { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string BatchName { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long OrderId { get; set; }
        public string OrderPaymentMethod { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class StudentPlanForBO_Pagination_VM
    {
        public long Id { get; set; }
        public long PlanId { get; set; }
        public long StudentUserLoginId { get; set; }

        public string PlanImage { get; set; }
        public string PlanImageWithPath { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public decimal PlanPrice { get; set; }
        public int PlanDurationTypeId { get; set; }
        public string PlanDurationTypeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long OrderId { get; set; }
        public string OrderPaymentMethod { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public int IsTransfered { get; set; }
        public long TransferPackageId { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }
    
    public class StudentEventForBO_Pagination_VM
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public long StudentUserLoginId { get; set; }

        public string EventTitle { get; set; }
        public string FeaturedImage{ get; set; }
        public string EventFeaturedImageWithPath { get; set; }
        public string EventShortDescription { get; set; }
        public decimal EventPrice { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long OrderId { get; set; }
        public string OrderPaymentMethod { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class StudentTrainingForBO_Pagination_VM
    {
        public long Id { get; set; }
        public long TrainingId { get; set; }
        public long StudentUserLoginId { get; set; }

        public string TrainingName { get; set; }
        public string TrainingImage { get; set; }
        public string TrainingImageWithPath { get; set; }
        public string TrainingShortDescription { get; set; }
        public decimal Price { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long OrderId { get; set; }
        public string OrderPaymentMethod { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }



    public class BranchStudentDetail_VM
    {
        public long UserLoginId { get; set; }
        public long StudentId { get; set; }
        public string MasterId { get; set; }
        public string StudentName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
        public List<BusinessClassDetailList_VM> ClassNameList { get; set; }
    }

    public class BusinessClassDetailList_VM
    {

        public string ClassName { get; set; }
    }
}