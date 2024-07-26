namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageMainPlanBooking_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long MainPlanId { get; set; }
        public int Mode { get; set; }
    }
}