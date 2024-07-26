namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageBusinessDistanceLocation_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MenuTag { get; set; }
        public long LastRecordedId { get; set; }
        public int RecordedId { get; set; }
        public int Mode { get; set; }

        public SP_ManageBusinessDistanceLocation_Param_VM()
        {
            Latitude = string.Empty;
            Longitude = string.Empty;
            MenuTag = string.Empty;
        }
    }
}