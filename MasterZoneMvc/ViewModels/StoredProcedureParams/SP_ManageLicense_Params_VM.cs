namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageLicense_Params_VM
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public int AchievingOrder { get; set; }
        public int Mode { get; set; }
        public SP_ManageLicense_Params_VM()
        {
            
        }
    }
}