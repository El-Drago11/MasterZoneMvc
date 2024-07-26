namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateUserCertificate_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long LicenseId { get; set; }
        public long CertificateId { get; set; }
        public long ItemId { get; set; }
        public string ItemTable { get; set; }
        public string CertificateNumber { get; set; }
        public string CertificateFile { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateUserCertificate_Params_VM()
        {
            // Make default string value to empty
            ItemTable = string.Empty;
            CertificateNumber = string.Empty;
            CertificateFile = string.Empty;
        }
    }
}