using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class TrainingViewModel {
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
    }

    public class TrainingClass_VM
    {
        public long Id { get; set; }
        public long InstructorUserLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public string InstructorEmail { get; set; } //InstructorEmail
        public string InstructorMobileNumber { get; set; } //InstructorMobileNumber
        public string InstructorAlternateNumber { get; set; }
        public int IsPaid { get; set; }
        public decimal Price { get; set; }
        public string AdditionalPriceInformation { get; set; }
        public string CenterName { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LocationUrl { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public string Description { get; set; }
        public string MusicType { get; set; }
        public string EnergyLevel { get; set; }
        public string DanceStyle { get; set; }

        public string TrainingImage { get; set; }
        public string TrainingImageWithPath { get; set; }
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
        public long CertificateId { get; set; }
        public long LicenseId { get; set; }
        public long LicenseBookingId { get; set; }

    }

    public class RequestTrainingClass_VM
    {
        public long Id { get; set; }
        public long InstructorUserLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public string InstructorEmail { get; set; } //InstructorEmail
        public string InstructorMobileNumber { get; set; } //InstructorMobileNumber
        public string InstructorAlternateNumber { get; set; }
        public int IsPaid { get; set; }
        public decimal Price { get; set; }
        public string AdditionalPriceInformation { get; set; }
        public string CenterName { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string LocationUrl { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public string Description { get; set; }
        public string MusicType { get; set; }
        public string EnergyLevel { get; set; }
        public string DanceStyle { get; set; }
        public int Mode { get; set; }

        public HttpPostedFile TrainingImage { get; set; }
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
        public long LicenseBookingId { get; set; }

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
            if (String.IsNullOrEmpty(TrainingName)) { sb.Append(Resources.BusinessPanel.TrainingNameRequired); vm.Valid = false; }
            else if (InstructorUserLoginId < 0) { sb.Append(Resources.BusinessPanel.InstructorNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ShortDescription)) { sb.Append(Resources.BusinessPanel.TrainingDescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(InstructorEmail)) { sb.Append(Resources.BusinessPanel.EmailRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(InstructorMobileNumber)) { sb.Append(Resources.BusinessPanel.TrainingInstructorNumberRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(InstructorAlternateNumber)) { sb.Append(Resources.BusinessPanel.TrainingAlternateNumberRequired); vm.Valid = false; }
            else if (IsPaid == 1 && Price < 0) { sb.Append(Resources.BusinessPanel.TrainingPriceRequired); vm.Valid = false; }
            else if (IsPaid == 1 && String.IsNullOrEmpty(AdditionalPriceInformation)) { sb.Append(Resources.BusinessPanel.TrainingAdditionalPriceRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CenterName)) { sb.Append(Resources.BusinessPanel.CenterNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Location)) { sb.Append(Resources.BusinessPanel.LandmarkNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.BusinessPanel.TrainingAddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(City)) { sb.Append(Resources.BusinessPanel.CityNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Country)) { sb.Append(Resources.BusinessPanel.CountryNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(State)) { sb.Append(Resources.BusinessPanel.StateNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PinCode)) { sb.Append(Resources.BusinessPanel.PinCodeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LocationUrl)) { sb.Append(Resources.BusinessPanel.LocationRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartDate)) { sb.Append(Resources.BusinessPanel.TrainingDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndDate)) { sb.Append(Resources.BusinessPanel.TrainingEndDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartTime_24HF)) { sb.Append(Resources.BusinessPanel.TrainingStartTimeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndTime_24HF)) { sb.Append(Resources.BusinessPanel.TrainingEndTimeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.AboutTrainingRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(MusicType)) { sb.Append(Resources.BusinessPanel.MusicRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EnergyLevel)) { sb.Append(Resources.BusinessPanel.EnergyRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(DanceStyle)) { sb.Append(Resources.BusinessPanel.DanceRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Duration)) { sb.Append(Resources.BusinessPanel.DurationTimeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(TrainingClassDays)) { sb.Append(Resources.BusinessPanel.ClassDaysRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AdditionalInformation)) { sb.Append(Resources.BusinessPanel.AdditionalInformationRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ExpectationDescription)) { sb.Append(Resources.BusinessPanel.ExpectationDescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(TrainingRules)) { sb.Append(Resources.BusinessPanel.TrainingRulesRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(BecomeInstructorDescription)) { sb.Append(Resources.BusinessPanel.InstructorBecomeDescriptionRequired); vm.Valid = false; }
            else if (TotalLectures < 0) { sb.Append(Resources.BusinessPanel.TotalLectureRequired); vm.Valid = false; }
            else if (TotalClasses < 0) { sb.Append(Resources.BusinessPanel.TotalClassRequired); vm.Valid = false; }
            else if (TotalSeats < 0) { sb.Append(Resources.BusinessPanel.TotalSeatsRequired); vm.Valid = false; }
            else if (TotalCredits < 0) { sb.Append(Resources.BusinessPanel.TotalCreditsRequired); vm.Valid = false; }
            else if (LicenseBookingId < 0) { sb.Append(Resources.BusinessPanel.Training_BusinessLicenseRequired); vm.Valid = false; }
            else if (TrainingImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(TrainingImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (TrainingImage.ContentLength > 10 * 1024 * 1024) // 10 MB
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

    public class InstructorList_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long InstructorUserLoginId { get; set; }
        public string InstructorName { get; set; }
        public string Designation { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public string FacebookProfileLink { get; set; }
        public string BusinessCategoryName { get; set; }
        public string ProfileImageWithPath { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; }
        public string MenuTag { get; set; }
        public string CertificateIconWithPath { get; set; }
        public string CertificateIcon { get; set; }
        public string Experience { get; set; }
        public int Privacy_UniqueUserId { get; set; }
        public int Verified { get; set; }
        public string InstructorContentDescription { get; set; }
        public string Name { get; set; }
        public string InstructorImageWithPath { get; set; }
        public string About { get; set; }
        public string Address { get; set; }
        public int? Age { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public int IsFavourite { get; set; }
        public int InstructorIsCertified { get; set; }
        public string UniqueUserId { get; set; }
        public string AboutDetails { get; set; }
        public DateTime DateofBirth { get; set; }
        public int MasterPro { get; set; }
        public List<UserCertificateBasicInfo_VM> Certifications { get; set; }
    }

    public class Training_Pagination_VM
    {
        public long Id { get; set; }

        public string InstructorName { get; set; }
        public string TrainingName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Price { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
        public int BookingsCount { get; set; }

        public long InstructorUserLoginId { get; set; }
        public string TrainingImageWithPath { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string InstructorBusinessLogoWithPath { get; set; }
    }
    
    public class Training_Pagination_VM_SuperAdminPanel
    {
        public long Id { get; set; }

        public string InstructorName { get; set; }
        public string TrainingName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Price { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
        public int BookingsCount { get; set; }
        public int Status { get; set; }

        public long InstructorUserLoginId { get; set; }
        public string TrainingImageWithPath { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string InstructorBusinessLogoWithPath { get; set; }

        public string ShortDescription { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public int ShowOnHomePage { get; set; }
    }

    public class Training_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class TrainingList_ForVisitorPanel_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public string CenterName { get; set; }
        public string Address { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TrainingImage { get; set; }
        public string TrainingImageWithPath { get; set; }
        public string Location { get; set; }
        public string StartMonth { get; set; }
        public int StartYear { get; set; }
        public int StartDay { get; set; }
    }

    public class TrainingList_ForHomePage_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TrainingImage { get; set; }
        public string TrainingImageWithPath { get; set; }
    }

    public class UserTrainingsDetail_VM
    {
        public long Id { get; set; }
        public string InstructorName { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public string InstructorEmail { get; set; } //InstructorEmail
        public string InstructorAlternateNumber { get; set; }
        public string Address { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MusicType { get; set; }
        public string EnergyLevel { get; set; }
        public string DanceStyle { get; set; }
        public string Location { get; set; }
        public string StartMonth { get; set; }
        public int StartYear { get; set; }
        public int StartDay { get; set; }
        public string TrainingImageWithPath { get; set; }
        public long UserLoginId { get; set; }
        public string TrainingImage { get; set; }
    }


    public class TrainingDetail_VisitorPanel_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string TrainingName { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string InstructorName { get; set; }
        public string Location { get; set; }
        public string ProfileImageWithPath { get; set; }
        public int IsCertified { get; set; }
        public string InstructorMobileNumber { get; set; }
        public string IsCertification { get; set; }
        public string StartMonth { get; set; }
        public int StartDay { get; set; }
        public int StartYear { get; set; }
        public string Weeks { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string FormattedStartTime { get; set; }
        public string FormattedEndTime { get; set; }
        public string Address { get; set; }
        public string Duration { get; set; }
        public string TrainingClassDays { get; set; }
        public string TrainingImageWithPath { get; set; }
        public int TotalLectures { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSeats { get; set; }
        public int TotalCredits { get; set; }
        public string MenuTag { get; set; }
        public List<TrainingCertificateIconDetail> certificateIconDetail { get; set; }
    }

    public class TrainingDescriptionList_VM
    {
        public long Id { get; set; }
        public string AdditionalInformation { get; set; }
        public string ExpectationDescription { get; set; }
        public string TrainingRules { get; set; }
        public string BecomeInstructorDescription { get; set; }
    }

    public class BusinessTrainingReviewsDetail
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ReviewerUserLoginId { get; set; }
        public int Rating { get; set; }
        public string StudentName { get; set; }
        public int DaysDifference { get; set; }
        public string ReviewBody { get; set; }

    }
    public class TrainingCertificateIconDetail
    {
        public long Id { get; set; }
        public string CertificateImageWithPath { get; set; }
    }
    public class BusinessReviewDetail
    {
        public long Id { get; set; }
        public string BusinessName { get; set; }
        public long UserLoginId { get; set; }
        public int Rating { get; set; }
        public int IsCertified { get; set; }
        public string ProfileImageWithPath { get; set; }
        public int TotalReviewsEntered { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; }
        public int TotalFiveStarRatings { get; set; }
        public int TotalFourStarRatings { get; set; }
        public int TotalThreeStarRatings { get; set; }
        public int TotalTwoStarRatings { get; set; }
        public int TotalOneStarRatings { get; set; }
        public string PercentageFiveStar { get; set; }
        public string PercentageFourStar { get; set; }
        public string PercentageThreeStar { get; set; }
        public string PercentageTwoStar { get; set; }
        public string PercentageOneStar { get; set; }
        public string OfficialWebSiteUrl { get; set; }

    }

    public class AvailableTraining_VM
    {
        public long Id { get; set; }
        public long InstructorUserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public long EncryptedUserLoginId { get; set; }
        public string TrainingName { get; set; }
        public string BusinessLogoImageWithPath { get; set; }
        public string LicenselogowitPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string Title { get; set; }
        public string TrainingStartTime { get; set; }
        public string TrainingEndTime { get; set; }
        public string TrainingClassDays { get; set; }
        public string InstructorImage { get; set; }
        public string MasterId { get; set; }
        public string BusinessName { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }
        public string InstructorName { get; set; }
        public string ShortDescription { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public string Description { get; set; }

        public string TrainingImage { get; set; }
        public string TrainingImageWithPath { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LocationUrl { get; set; }

    }

}