namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateLicense_Params_VM
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string Title { get; set; }
        public string CertificateImage { get; set; }
        public string LicenseLogoImage { get; set; }
        public string SignatureImage { get; set; }
        public string Description { get; set; }
        public int IsPaid { get; set; }
        public int Status { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
        public string TimePeriod { get; set; }
        public int LicenseDuration { get; set; }
        public int AchievingOrder { get; set; }
        public string LicensePermissions { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public decimal GSTPercent { get; set; }
        public string GSTDescription { get; set; }
        public decimal MinSellingPrice { get; set; }
        public decimal Price { get; set; }
        public string Signature2Image { get; set; }
        public string Signature3Image { get; set; }
        public string LicenseCertificateHTMLContent { get; set; }
        public int IsLicenseToTeach { get; set; }
        public string LicenseToTeach_Type { get; set; }
        public string LicenseToTeach_DisplayName { get; set; }
        public int MasterPro { get; set; }  

        public SP_InsertUpdateLicense_Params_VM()
        {
            Title = string.Empty;
            CertificateImage = string.Empty;
            LicenseLogoImage = string.Empty;
            SignatureImage = string.Empty;
            Description = string.Empty;
            CommissionType = string.Empty;
            TimePeriod = string.Empty;
            LicensePermissions = string.Empty;
            GSTDescription = string.Empty;
            Signature2Image = string.Empty;
            Signature3Image = string.Empty;
            LicenseCertificateHTMLContent = string.Empty;
        }
    }
}