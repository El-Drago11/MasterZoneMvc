namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageFieldTypeCatalog_Params_VM
    {
        public long Id { get; set; }
        public long PanelTypeId { get; set; }
        public string KeyName { get; set; }
        public int Mode { get; set; }

        public SP_ManageFieldTypeCatalog_Params_VM()
        {
            KeyName = string.Empty;
        }
    }
}