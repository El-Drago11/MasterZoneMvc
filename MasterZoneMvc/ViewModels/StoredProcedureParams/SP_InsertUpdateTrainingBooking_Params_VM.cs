namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateTrainingBooking_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long TrainingId { get; set; }
        public long StudentUserLoginId { get; set; }
        public string TrainingStartDate { get; set; }
        public string TrainingEndDate { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateTrainingBooking_Params_VM()
        {
            Id = 0;
            OrderId = 0;
            TrainingId = 0;
            StudentUserLoginId = 0;
            TrainingStartDate = "";
            TrainingEndDate = "";
            Mode = 0;
        }
    }
}