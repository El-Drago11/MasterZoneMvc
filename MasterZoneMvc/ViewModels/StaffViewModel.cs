using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StaffViewModel
    {
        public long Id { get; set; }

        public long UserLoginId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public long StaffCategoryId { get; set; }

        //public decimal MonthlySalary { get; set; }

        public string ProfileImage { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal DearnessAllowance { get; set; }
        public string Remarks { get; set; }

    }

    public class RequestStaffViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public long StaffCategoryId { get; set; }

        //public decimal MonthlySalary { get; set; }

        public HttpPostedFile ProfileImage { get; set; }

        public long BusinessOwnerLoginId { get; set; }

        public int Status { get; set; }
        public int Mode { get; set; }

        public string PermissionIds { get; set; }
        public int IsMasterId { get; set; }
        public string MasterId { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal DearnessAllowance { get; set; }
        public string Remarks { get; set; }
        public int IsProfessional { get; set; }
        public string Designation { get; set; }

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
            else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.BusinessPanel.InvalidStatusRequired); vm.Valid = false; }
            else if (StaffCategoryId <= 0) { sb.Append(Resources.BusinessPanel.InvalidStaffCategoryRequired); vm.Valid = false; }
            else if (string.IsNullOrEmpty(Designation)) { sb.AppendLine(Resources.BusinessPanel.DesignationRequired); vm.Valid = false; }
            else if (IsProfessional < 0 || IsProfessional > 1) { sb.Append(Resources.BusinessPanel.IsProfessionalRequired); vm.Valid = false; }
            else if (IsMasterId > 0 && string.IsNullOrEmpty(MasterId)) { sb.Append(Resources.BusinessPanel.ErrorMessageMasterId); vm.Valid = false; }
            else if (IsMasterId > 0 && (!MasterId.Contains("MU_") && !MasterId.Contains("MBI_"))) { sb.Append(Resources.BusinessPanel.ManageStaff_MasterIdNotAllowed_ErrorMessage); vm.Valid = false; }
            else if (ProfileImage != null)
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
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
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

    public class Staff_VM
    {
        public long Id { get; set; }

        public long UserLoginId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public string State { get; set; }

        public long StaffCategoryId { get; set; }

        //public decimal MonthlySalary { get; set; }

        public string ProfileImage { get; set; }

        public long BusinessOwnerLoginId { get; set; }


        public string Email { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string StaffCategoryName { get; set; }

        public int Status { get; set; }
        public string MasterId { get; set; }
        public string UserLoginMasterId { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal DearnessAllowance { get; set; }
        public string Remarks { get; set; }
        public int IsProfessional { get; set; }
        public string Designation { get; set; }
        public string CreatedOnString { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class StaffEdit_VM : Staff_VM
    {
        public string Password { get; set; }
        public List<PermissionHierarchy_VM> Permissions { get; set; }
        public List<PermissionHierarchy_VM> PermissionsHierarchy { get; set; }

    }

    public class StaffAttendance_VM
    {
        public long Id { get; set; }
        public long StaffId { get; set; }
        public string AttendanceDate { get; set; }
        public int AttendanceStatus { get; set; } // Present(1), Absent(2), OnLeave(3)
        public string StaffName { get; set; }
        public string LeaveReason { get; set; }
        public string StaffProfileImageWithPath { get; set; }
        public string MasterId { get; set; } //change into long
        public int IsApproved { get; set; }
        public string StaffAttendanceInTime_24HF { get; set; }
        public string StaffAttendnaceOutTime_24HF { get; set; }
        public string FormattedAttendanceDate { get; set; }

        public string StaffCategoryName { get; set; }


        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpadatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public long UserLoginId { get; set; }
    }

    public class RequestStaffAttendance_VM
    {
        public long staffId { get; set; }
        public int attendanceStatus { get; set; }
        public string leaveReason { get; set; }
        public int isApproved { get; set; }
        public string staffAttendanceInTime24HF { get; set; }
        public string staffAttendanceOutTime24HF { get; set; }
    }
    public class RequestStaffProfile_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public HttpPostedFile ProfileImage { get; set; }
        public int Mode { get; set; }
        public string LandMark { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string About { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }


        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); vm.Valid = false; }
            else if (!EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired + $"({Email})"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LandMark)) { sb.Append(Resources.BusinessPanel.LandmarkNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.BusinessPanel.TrainingAddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(City)) { sb.Append(Resources.BusinessPanel.CityNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Country)) { sb.Append(Resources.BusinessPanel.CountryNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(State)) { sb.Append(Resources.BusinessPanel.StateNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PinCode)) { sb.Append(Resources.BusinessPanel.PinCodeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.BusinessPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

        public Error_VM VaildInformationProfileImage()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }
            StringBuilder sb = new StringBuilder();
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
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
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

    public class StaffProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LandMark { get; set; }
        public string TwitterProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string FacebookProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string About { get; set; }
        public DateTime DOB { get; set; }
        public long UserLoginId { get; set; }

    }

    public class BranchStaffDetail_Pagination
    {
        public long Id { get; set; }
        public long UerLoginId { get; set; }
        public string StaffMasterId { get; set; }
        public string MasterId { get; set; }
        public string StaffName { get; set; }
        public string StaffCategoryName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string PhoneNumber { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }

    public class BranchStaffDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}