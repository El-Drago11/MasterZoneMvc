namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageBatch_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public string MasterId { get; set; }
        public string SearchText { get; set; }

        public SP_ManageBatch_Params_VM()
        {
            MasterId = string.Empty;
            SearchText = string.Empty;
        }
    }
}