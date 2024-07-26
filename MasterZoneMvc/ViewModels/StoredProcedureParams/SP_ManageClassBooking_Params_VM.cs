namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageClassBooking_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ClassId { get; set; }
        public long BatchId { get; set; }
        public int Mode { get; set; }
        public string JoiningDate { get; set; }

        public SP_ManageClassBooking_Params_VM()
        {
            JoiningDate = string.Empty;
        }

    }
}