namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageClass_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string dayNames { get; set; }
        public int Mode { get; set; }

        public SP_ManageClass_Params_VM()
        {
            dayNames=string.Empty;
        }
    }
}