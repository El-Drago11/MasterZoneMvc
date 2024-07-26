namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageItemFeatures_Params_VM
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public long FeatureId { get; set; }
        public int Mode { get; set; }

        public SP_ManageItemFeatures_Params_VM()
        {
            ItemType = string.Empty;
        }
    }
}