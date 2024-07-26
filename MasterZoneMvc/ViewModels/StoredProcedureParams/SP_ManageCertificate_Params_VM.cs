namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageCertificate_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public string Searchtext { get; set; }
        public SP_ManageCertificate_Params_VM()
        {
            Searchtext = string.Empty;
        }
    }
}