using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessOwnerProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string DocumentFileWithPath { get; set; }
        public string About { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LandMark { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentFile { get; set; }
        public string UploadedDocumentsWithPath { get; set; }
        public string BusinessName { get; set; }
        public string Experience { get; set; }
        public int Privacy_UniqueUserId { get; set; }
        public string OfficialWebSiteUrl { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public string ProfilePageTypeKey { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string CoverImage { get; set; }
        public string CoverImageWithPath { get; set; }
        public long BusinessCategoryId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string MasterId { get; set; }
        public int MainPlanId { get; set; }
        public string MainPlanName { get; set; }
        public string MainPlanStartDate { get; set; }
        public string MainPlanEndDate { get; set; }

    }

    public class RequestBusinessProfile_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BusinessName { get; set; }
        public HttpPostedFile ProfileImage { get; set; }
        public HttpPostedFile BusinessLogo { get; set; }
        public string About { get; set; }
        public int Mode { get; set; }
        public string LandMark { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string PhoneNumber { get; set; }
        public string DocumentTitle { get; set; }
        public HttpPostedFile DocumentUploadedFile { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Experience { get; set; }
        public int Privacy_UniqueUserId { get; set; }
        public string OfficialWebSiteUrl { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }


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
            else if (String.IsNullOrEmpty(About)) { sb.Append(Resources.BusinessPanel.AboutRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(BusinessName)) { sb.Append(Resources.BusinessPanel.BusinessNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LandMark)) { sb.Append(Resources.BusinessPanel.LandmarkNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.BusinessPanel.TrainingAddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(City)) { sb.Append(Resources.BusinessPanel.CityNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Country)) { sb.Append(Resources.BusinessPanel.CountryNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(State)) { sb.Append(Resources.BusinessPanel.StateNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PinCode)) { sb.Append(Resources.BusinessPanel.PinCodeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.BusinessPanel.PhoneNumberRequired); vm.Valid = false; }
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
            if (BusinessLogo != null)
            {
                // Validate Uploded Image File
                bool isValidLogo = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(BusinessLogo.ContentType))
                {
                    isValidLogo = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (BusinessLogo.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidLogo = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidLogo;
            }

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
            if(ProfileImage == null)
            {
                vm.Valid = false;
                vm.Message = Resources.BusinessPanel.PleaseSelectAnImage_ErrorMessage;
            }
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

        public Error_VM VaildInformationBusinessLogo()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }
            StringBuilder sb = new StringBuilder();

            if (BusinessLogo == null)
            {
                vm.Valid = false;
                vm.Message = Resources.BusinessPanel.PleaseSelectAnImage_ErrorMessage;
            }
            else if (BusinessLogo != null)
            {
                // Validate Uploded Image File
                bool isValidLogo = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(BusinessLogo.ContentType))
                {
                    isValidLogo = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (BusinessLogo.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidLogo = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidLogo;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

        public Error_VM VaildInformationUploadingBusinessOwnerDocuments()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(DocumentTitle)) { sb.Append(Resources.BusinessPanel.BusinessDocumentRequired); vm.Valid = false; }
            else if (DocumentUploadedFile == null) { sb.Append(Resources.BusinessPanel.BusinessProfileDocumentErrorMessage); vm.Valid = false; }
            else if (DocumentUploadedFile != null && DocumentUploadedFile.ContentLength > 10 * 1024 * 1024)
            {
                sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                vm.Valid = false;
            }
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class BusinessOwnerProfileUploadedDocuments
    {
        public string DocumentTitle { get; set; }
        public string DocumentFile { get; set; }
        public string UploadedDocumentsWithPath { get; set; }
        public string IsAcception { get; set; }
        public int Status { get; set; }
    }
    public class BusinessTimingRequest_VM
    {
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public string OpeningTime2 { get; set; }
        public string ClosingTime2 { get; set; }
        public int IsOpen { get; set; }
        public string DayName { get; set; }
        public string TodayOff { get; set; }
        public string Notes { get; set; }
        public int Mode { get; set; }
    }
    public class BusinessTiming_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string DayName { get; set; }
        public int DayValue { get; set; }
        public int IsOpened { get; set; }
        public string OpeningTime_12HoursFormat { get; set; }
        public string OpeningTime_24HoursFormat { get; set; }
        public string ClosingTime_12HoursFormat { get; set; }
        public string ClosingTime_24HoursFormat { get; set; }
        public string OpeningTime2_12HoursFormat { get; set; }
        public string OpeningTime2_24HoursFormat { get; set; }
        public string ClosingTime2_12HoursFormat { get; set; }
        public string ClosingTime2_24HoursFormat { get; set; }
        public string TodayOff { get; set; }
        public string Notes { get; set; }
    }

    public class BusinessOwnerProfileDetail_VM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string About { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LandMark { get; set; }
        public int IsCertified { get; set; }
    }
}