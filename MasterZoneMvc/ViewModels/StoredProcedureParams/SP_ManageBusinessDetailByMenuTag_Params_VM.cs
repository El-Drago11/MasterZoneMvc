namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageBusinessDetailByMenuTag_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MenuTag { get; set; }
        public int Mode { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }


        public SP_ManageBusinessDetailByMenuTag_Params_VM()
        {
            MenuTag = string.Empty;
        }
    }
}