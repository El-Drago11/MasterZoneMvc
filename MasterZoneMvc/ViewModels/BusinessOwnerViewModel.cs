using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessOwnerViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }

        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long BusinessOwnersId { get; set; }
        public string Address { get; set; }
        public string DOB { get; set; }
        public DateTime DOB_DateTime { get; set; }

        public int IsAccountAccepted { get; set; }
        public string RejectionReason { get; set; }
        public long BusinessCategoryId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public int IsPrimeMember { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessLogo { get; set; }
        public string About { get; set; }
    }

    public class RegisterBusinessOwnerViewModel
    {
        public BusinessOwner BusinessOwner { get; set; }
        public UserLogin UserLogin { get; set; }
        public List<BusinessDocument> BusinessDocuments { get; set; }

        public Error_VM ValidInformation()
        {

            Error_VM response = new Error_VM();
            response.Valid = true;

            if (this == null || this.BusinessOwner == null || this.UserLogin == null || this.BusinessDocuments == null)
            {
                response.Valid = false;
                response.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return response;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(this.BusinessOwner.BusinessName)) { sb.Append(Resources.BusinessPanel.BusinessNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.BusinessOwner.FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.BusinessOwner.LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.BusinessOwner.Address)) { sb.Append(Resources.BusinessPanel.AddressRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.BusinessOwner.DOB)) { sb.Append(Resources.BusinessPanel.DateOfBirthRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.UserLogin.Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.UserLogin.Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); response.Valid = false; }
            else if (this.BusinessOwner.BusinessCategoryId <= 0) { sb.Append(Resources.BusinessPanel.BusinessCategoryRequired); response.Valid = false; }
            else if (this.BusinessDocuments.Count() == 0) { sb.Append(Resources.BusinessPanel.BusinessDocumentRequired); response.Valid = false; }

            // if all are valid then return success response
            if (response.Valid == false)
            {
                response.Message = sb.ToString();
            }

            return response;
        }
    }

    public class RequestRegisterBusinessOwner_VM
    {
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public long BusinessCategoryId { get; set; }
        //public List<IFormFile> Files { get; set; }

        public Error_VM ValidInformation()
        {

            Error_VM response = new Error_VM();
            response.Valid = true;

            if (this == null)
            {
                response.Valid = false;
                response.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return response;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(BusinessName)) { sb.Append(Resources.BusinessPanel.BusinessNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.BusinessPanel.AddressRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(DOB)) { sb.Append(Resources.BusinessPanel.DateOfBirthRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); response.Valid = false; }
            else if (BusinessCategoryId <= 0) { sb.Append(Resources.BusinessPanel.BusinessCategoryRequired); response.Valid = false; }
            //if (Files.Count() == 0) { sb.Append("Please attach business documents!"); response.Valid = false; }

            // if all are valid then return success response
            if (response.Valid == false)
            {
                response.Message = sb.ToString();
            }

            return response;
        }
    }

    public class RegisterBusinessOwnerPartial_VM
    {
        public long BusinessCategoryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        public Error_VM ValidInformation()
        {

            Error_VM response = new Error_VM();
            response.Valid = true;

            if (this == null)
            {
                response.Valid = false;
                response.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return response;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(this.FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); response.Valid = false; }
            else if (String.IsNullOrEmpty(this.Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); response.Valid = false; }
            else if (this.BusinessCategoryId <= 0) { sb.Append(Resources.BusinessPanel.BusinessCategoryRequired); response.Valid = false; }

            // if all are valid then return success response
            if (response.Valid == false)
            {
                response.Message = sb.ToString();
            }

            return response;
        }
    }

    public class BusinessOwnerList_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int64 BusinessCategoryId { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessLogo { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public int Status { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessName { get; set; }
        public long UserLoginId { get; set; }
        public int IsFavourite { get; set; }
        public int IsFollower { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public string LinkDinProfileLink { get; set; }
        public string UniqueUserId { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }


    }


    public class BusinessOnwerList_ForStudent_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessOwnerFullName { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string BusinessMasterId { get; set; }
    }


    #region Business Detail VM ---------------------------------------

    public class BusinessDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public long BusinessCategoryId { get; set; }
        public string AccountStatus { get; set; }
        public string About { get; set; }
        public string IsPrimeMemberStatus { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessCategoryName { get; set; }
        public string UniqueUserId { get; set; }
        public string MasterId { get; set; }
    }

    public class DocumentDetail_VM
    {
        public string Id { get; set; }
        public string DocumentFile { get; set; }
        public string FilePath { get; set; }
        public string DocumentTitle { get; set; }
        public int Status { get; set; }
        public string IsAcception { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedOn_FormatDate { get; set; }
    }

    public class BusinessDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessLogo { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int Status { get; set; }
        public string Activity { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string MasterId { get; set; }
        public int ShowOnHomePage { get; set; }
    }


    public class BusinessInstrctor_Pagination_SuperAdminPanel_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessLogo { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int Status { get; set; }
        public string Activity { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string MasterId { get; set; }
        public int ShowOnHomePage { get; set; }
    }

    public class BusinessOwnerList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class BusinessDetailUpdate_VM
    {
        public long Id { get; set; }
        public long BusinessCategoryId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int Mode { get; set; }

        public long ParentBusinessCategoryId { get; set; }
        public long SubBusinessCategoryId { get; set; }
        public int Verified { get; set; }
    }

    #endregion


    public class BusinessOnwerListByMenuTag_VM : AddressLocation_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string OwnerFullName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string ProfileImageWithPath { get; set; }
        public int IsCertified { get; set; }

        public long ProfilePageTypeId { get; set; }
        public string ProfilePageTypeKey { get; set; }
        public string ProfilePageTypeName { get; set; }

        public decimal SpecialDiscount { get; set; }
        public decimal SpecialPrice { get; set; }
        public string SpecialDuration { get; set; }
    }

    public class InstructorListByMenuTag_VM : AddressLocation_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public decimal AverageRating { get; set; }
        public int IsFavourite { get; set; }
        public string About { get; set; }
        public string UniqueUserId { get; set; }
        public int IsCertified { get; set; }
        public List<UserCertificationForProfile_VM> CertificationList { get; set; }

        public long ProfilePageTypeId { get; set; }
        public string ProfilePageTypeKey { get; set; }
        public string ProfilePageTypeName { get; set; }
    }

    public class UserCertificationForProfile_VM
    {
        public long Id { get; set; }
        public string CertificateName { get; set; }
        public string CertificateIcon { get; set; }
        public string CertificateIconWithPath { get; set; }
    }

    public class AddressLocation_VM
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string FullAddressLocation { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Pincode { get; set; }
    }

    public class BusinessProfilePageDetail_ForVisitor_VM : AddressLocation_VM {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long BusinessCategoryId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string About { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public int IsCertified { get; set; }

        public int TotalFollowers { get; set; }
        public int TotalFollowings { get; set; }
        public int TotalFavourites { get; set; }
        public decimal AverageRating { get; set; }
        public int OnlineClassesCount { get; set; }
    }

    public class CoachesInstructorAboutDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string ProfileImageWithPath { get; set; }
    }


    /// <summary>
    /// To Get The Academy List In Business Owner 
    /// </summary>
    public class BusinessOwnerDetailList_VM
    {
        public Int64 Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int64 BusinessCategoryId { get; set; }
        public string Address { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessLogo { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Status { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessName { get; set; }
        public int Verified { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double DistanceInKilometers { get; set; }
        public string MenuTag { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public string UniqueUserId { get; set; }
        public string BusinessOwnerName { get; set; }

        public long TotalRecords { get; set; }
        public int IsFavourite { get; set; }

    }

    public class BusinessContentDistanceParameter_VM
    {
        public string MenuTag { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public long LastRecordedId { get; set; }
        public int RecordLimit { get; set; }
    }

    public class MasterIdDetails_VM
    {
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessUniqueUserId { get; set; }
        public long StudentUserLoginId { get; set; }
        public string StudentUniqueUserId { get; set; }
        public string BusinessMasterId { get; set; }
        public string StudentUserMasterId { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessName { get; set; }

        public List<MasterIdDetails_VM> TemporaryMasterIds { get; set; }
    }

    public class BusinessUserOtherProfileDetail
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string FacebookProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string Summary { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public int Freelance { get; set; }
        public string MasterId { get; set; }
    }

    public class InstructorList_ForHomePage_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Name { get; set; }
        public string BusienssName { get; set; }
        public string FacebookProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public int IsCertified { get; set; }
        public int Verified { get; set; }
        public string VerifiedText { get; set; }
        public decimal AverageRating { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string CoverImage { get; set; }
        public string CoverImageWithPath { get; set; }

    }


    public class BusinessBranchesDetailForVisitorPanel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessName { get; set; }
        public int TotalCount { get; set; }
    }

    public class BusinessListByCategory_HomePage_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessLogo { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int Status { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string CoverImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int ShowOnHomePage { get; set; }
    }
}