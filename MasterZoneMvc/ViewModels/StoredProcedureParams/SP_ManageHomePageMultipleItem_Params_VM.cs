namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageHomePageMultipleItem_Params_VM
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public int Mode { get; set; }

        public SP_ManageHomePageMultipleItem_Params_VM()
        {
            Type = string.Empty;
        }
    }
}