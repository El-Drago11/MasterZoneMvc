namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageBusinessLicenses_Params_VM
    {
        public long Id { get; set; }
        public long LicenseId { get; set; }
        public long CertificateId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
    }
}