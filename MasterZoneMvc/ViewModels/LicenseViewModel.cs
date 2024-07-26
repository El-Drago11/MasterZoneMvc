using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class LicenseViewModel
    {
    }

    public class RequestLicense_VM
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string Title { get; set; }
        public HttpPostedFile CertificateImage { get; set; }
        public HttpPostedFile SignatureImage { get; set; }
        public HttpPostedFile Signature2Image { get; set; }
        public HttpPostedFile Signature3Image { get; set; }
        public HttpPostedFile LicenseLogoImage { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int IsPaid { get; set; }
        public int AchievingOrder { get; set; }
        public int LicenseDuration { get; set; }
        public string TimePeriod { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
        public string BusinessPanelPermissions { get; set; }
        public List<RequestItemFeaturePermission_VM> FeaturePermissions { get; set; }

        public int Mode { get; set; }
        public decimal Price { get; set; }
        public decimal GSTPercent { get; set; }
        public string GSTDescription { get; set; }
        public decimal MinSellingPrice { get; set; }
        public string LicenseCertificateHTMLContent { get; set; }
        public int IsLicenseToTeach { get; set; }
        public string LicenseToTeach_Type { get; set; }
        public string LicenseToTeach_DisplayName { get; set; }

        public int MasterPro { get; set; }
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.SuperAdminPanel.License_TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.SuperAdminPanel.License_DescriptionRequired); vm.Valid = false; }
            else if (CertificateId <= 0) { sb.Append(Resources.SuperAdminPanel.License_CertificateSelectionRequired); vm.Valid = false; }
            else if (string.IsNullOrEmpty(TimePeriod)) { sb.Append(Resources.SuperAdminPanel.License_TimePeriodRequired); vm.Valid = false; }
            else if (AchievingOrder <= 0) { sb.Append(Resources.SuperAdminPanel.License_AchievingOrderRequired); vm.Valid = false; }
            //else if (Mode == 1 &&  CertificateImage == null) { sb.Append(Resources.SuperAdminPanel.License_CertificateImageRequired); vm.Valid = false; }
            else if (Mode == 1 && LicenseLogoImage == null) { sb.Append(Resources.SuperAdminPanel.License_LicenseLogoImageRequired); vm.Valid = false; }
            else if (Mode == 1 && (SignatureImage == null || Signature2Image == null)) { sb.Append(Resources.SuperAdminPanel.License_SignatureImageRequired); vm.Valid = false; }
            else if (IsPaid == 1 && Price <= 0) { sb.Append(Resources.SuperAdminPanel.License_PriceRequired); vm.Valid = false; }
            else if ((string.IsNullOrEmpty(CommissionType) || CommissionType == "0")) { sb.Append(Resources.SuperAdminPanel.License_CommissionTypeRequired); vm.Valid = false; }
            else if (CommissionValue <= 0) { sb.Append(Resources.SuperAdminPanel.License_CommissionValueGreaterThanZeroRequired); vm.Valid = false; }
            else if (MinSellingPrice < 0) { sb.Append(Resources.SuperAdminPanel.License_MinimunSellingPriceRequired); vm.Valid = false; }
            else if (string.IsNullOrEmpty(LicenseCertificateHTMLContent)) { sb.Append(Resources.SuperAdminPanel.License_LicenseCertificateHTMLContentRequired); vm.Valid = false; }
            else if (IsLicenseToTeach <= 0) { sb.Append(Resources.SuperAdminPanel.LicenseToTeachRequired); vm.Valid = false; }
            else if (MasterPro <= 0) { sb.Append(Resources.SuperAdminPanel.License_MasterProRequired); vm.Valid = false; }
            else if (IsLicenseToTeach == 1 && (string.IsNullOrEmpty(LicenseToTeach_Type) || LicenseToTeach_Type == "0"))
            {
                sb.Append(Resources.SuperAdminPanel.LicenseToTeachDisplayRequired);
                vm.Valid = false;
            }
            else if ((LicenseToTeach_Type == "Custom") && string.IsNullOrEmpty(LicenseToTeach_DisplayName))
            {
                sb.Append(Resources.SuperAdminPanel.LicenseToTeachDisplayNameRequired);
                vm.Valid = false;
            }
            else if (FeaturePermissions.Count() > 0)
            {
                bool flag = true;
                foreach (var feature in FeaturePermissions)
                {
                    if (feature.IsLimited == 1 && feature.Limit <= 0)
                    {
                        sb.Append(Resources.SuperAdminPanel.FeatureLimitRequired);
                        flag = false;
                        break;
                    }
                }
                vm.Valid = flag;
            }

            if (vm.Valid && CertificateImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CertificateImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (CertificateImage.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
                }

                vm.Valid = isValidImage;
            }
            if (vm.Valid && SignatureImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(SignatureImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (SignatureImage.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
                }

                vm.Valid = isValidImage;
            }
            if (vm.Valid && Signature2Image != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Signature2Image.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Signature2Image.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
                }

                vm.Valid = isValidImage;
            }

            if (vm.Valid && Signature3Image != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Signature3Image.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Signature3Image.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
                }

                vm.Valid = isValidImage;
            }
            if (vm.Valid && LicenseLogoImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(LicenseLogoImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (LicenseLogoImage.ContentLength > 2 * 1024 * 1024) // 2 MB
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

    public class LicenseEdit_VM
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CertificateImage { get; set; }
        public string LicenseCertificateImageWithPath { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoImageWithPath { get; set; }
        public string SignatureImage { get; set; }
        public string SignatureImageWithPath { get; set; }
        public string Signature2Image { get; set; }
        public string Signature2ImageWithPath { get; set; }
        public string Signature3Image { get; set; }
        public string Signature3ImageWithPath { get; set; }
        public int Status { get; set; }
        public int IsPaid { get; set; }
        public int AchievingOrder { get; set; }
        public string TimePeriod { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
        public string LicensePermissions { get; set; }
        public List<ItemFeatureViewModel> FeaturePermissions { get; set; }
        public decimal GSTPercent { get; set; }
        public string GSTDescription { get; set; }
        public decimal MinSellingPrice { get; set; }
        public decimal Price { get; set; }
        public string LicenseCertificateHTMLContent { get; set; }
        public int IsLicenseToTeach { get; set; }
        public string LicenseToTeach_Type { get; set; }
        public string LicenseToTeach_DisplayName { get; set; }
        public int MasterPro { get; set; }
    }

    public class License_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public long CertificateId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class LicensePagination_VM
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string CertificateName { get; set; }
        public string Title { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoWithPath { get; set; }
        public string Description { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
        public int IsPaid { get; set; }
        public int AchievingOrder { get; set; }
        public int Status { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

        public string CommissionTypeName { get; set; }
    }

    public class LicenseList_ForBO_VM
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CertificateImage { get; set; }
        public string LicenseCertificateImageWithPath { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoImageWithPath { get; set; }
        public string SignatureImage { get; set; }
        public string SignatureImageWithPath { get; set; }
        public string Signature2Image { get; set; }
        public string Signature2ImageWithPath { get; set; }
        public string Signature3Image { get; set; }
        public string Signature3ImageWithPath { get; set; }
        public int Status { get; set; }
        public int IsPaid { get; set; }
        public int AchievingOrder { get; set; }
        public string TimePeriod { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
        public string LicensePermissions { get; set; }
        public List<ItemFeatureViewModel> FeaturePermissions { get; set; }
        public decimal GSTPercent { get; set; }
        public string GSTDescription { get; set; }
        public decimal MinSellingPrice { get; set; }
        public decimal Price { get; set; }
        public string CommissionTypeName { get; set; }
    }

    public class LicenseToTeachDetail_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IsLicenseToTeach { get; set; }
        public string LicenseToTeach_Type { get; set; }
        public string LicenseToTeach_DisplayName { get; set; }
        public int Mode { get; set; }
    }

    public class LicenseCertificateHTMLContent_VM
    {
        public long LicenseId { get; set; }
        public long CertificateId { get; set; }
        public string UniqueCertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueDate_Format { get; set; }

        public string CertificateLogoPath { get; set; }
        public string CertificateTitle { get; set; }
        public string LicenseLogoPath { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseDescription { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Signature1Path { get; set; }
        public string Signature2Path { get; set; }
        public string Signature3Path { get; set; }
        public string TimePeriod { get; set; }
    }

    public class LicenseListForDropdown_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }
}