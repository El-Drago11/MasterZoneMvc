namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageTrainingBooking_Params_VM
    {
        public long Id { get; set; }
        public long TrainingId { get; set; }
        public long UserLoginId { get; set; }
        public int Mode { get; set; }

        public SP_ManageTrainingBooking_Params_VM()
        {
        }
    }
}