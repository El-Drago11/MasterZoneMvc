namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_GetAllStudentGroupRecord_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public int Mode { get; set; }
    }
}