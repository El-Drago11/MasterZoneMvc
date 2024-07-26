namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageGroup_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public int Mode { get; set; }
        public string SearchKeywords { get; set; }
        public string GroupType { get; set; }
        public SP_ManageGroup_Params_VM()
        {
            SearchKeywords = string.Empty;
            GroupType = string.Empty;
        }
    }
}