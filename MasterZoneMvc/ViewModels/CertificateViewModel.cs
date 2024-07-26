using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CertificateViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CertificateIcon { get; set; }
        public string ShortDescription { get; set; }
        public string AdditionalInformation { get; set; }
        public decimal Price { get; set; }
        public string CertificatePermissions { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public string CertificateTypeKey { get; set; }
        public string Link { get; set; }
    }

    public class RequestCertificate_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public HttpPostedFile CertificateIcon { get; set; }
        public string ShortDescription { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string CertificateTypeKey { get; set; }
        public string Link { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.SuperAdminPanel.CertificateNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ShortDescription)) { sb.Append(Resources.SuperAdminPanel.CertificateShortDescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CertificateTypeKey) || CertificateTypeKey.Equals("0")) { sb.Append(Resources.SuperAdminPanel.CertificateTypeRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Link)) { sb.Append(Resources.SuperAdminPanel.CertificateLinkRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Link)) { sb.Append(Resources.SuperAdminPanel.ValidLinkRequired); vm.Valid = false; }
            else if (ProfilePageTypeId <= 0) { sb.Append(Resources.SuperAdminPanel.CertificateProfilePageTypeRequired); vm.Valid = false; }
            else if (CertificateIcon != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CertificateIcon.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (CertificateIcon.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
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


    public class CertificateEdit_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string ShortDescription { get; set; }
        public string CertificateIcon { get; set; }
        public string CertificateIconWithPath { get; set; }
        public long ProfilePageTypeId { get; set; }
        public int Status { get; set; }
        public string CertificateTypeKey { get; set; }
        public string Link { get; set; }
    }

    public class CertificatePagination_VM
    {
        public long Id { get; set; }
        public string CertificateTypeKey { get; set; }
        public string CertificateTypeTextValue { get; set; }
        public string Name { get; set; }
        public string CertificateIcon { get; set; }
        public string CertificateIconWithPath { get; set; }
        public string ShortDescription { get; set; }
        public string ProfilePageTypeName { get; set; }
        public int Status { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

        public int ShowOnHomePage{ get; set; }
        public string Link{ get; set; }
    }

    public class Certificate_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class CertificateListForDropdown_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class CertificateListForHomePage_VM
    {
        public long Id { get; set; }
        public string CertificateIcon { get; set; }
        public string CertificateIconWithPath { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
    
    public class CertificateList_ForBO_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string ShortDescription { get; set; }
        public string CertificateIcon { get; set; }
        public string CertificateIconWithPath { get; set; }
        public int Status { get; set; }
    }

    public class CertificateLicenseListForDropdown_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<LicenseListForDropdown_VM> Licenses { get; set; }
    }

    public class InstructorInfo_VM
    {
        public long InstructorId { get; set; }
        public long UserLoginId { get; set; }
        public string InstructorFullName { get; set; }
        public int InstructorStatus { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string InstructorCategoryName { get; set; }
        public string Designation { get; set; }
        public string CertificateJson { get; set; }
        public List<CertificateDetailsViewModel> CertificateDetails { get; set; }
    }

    public class CertificateDetailsViewModel
    {
        public long CertificateId { get; set; }
        public string CertificateName { get; set; }
        public string ShortDescription { get; set; }
        public string CertificateIconWithPath { get; set; }
        public string JoinDate { get; set; }
        public string CertificateValidityDate { get; set; }
        public List<LicenseDetails_VM> licensedetailJson { get; set; }
    }

    public class LicenseDetails_VM
    {
        public string LicenseToTeach_DisplayName { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseLogoWithPath { get; set; }
        public string LicenseValidityDate { get; set; }
        public int Status { get; set; }
    }
}