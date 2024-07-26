namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManagePaymentResponse_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int Mode { get; set; }

        public SP_ManagePaymentResponse_Params_VM()
        {
        }
    }
}