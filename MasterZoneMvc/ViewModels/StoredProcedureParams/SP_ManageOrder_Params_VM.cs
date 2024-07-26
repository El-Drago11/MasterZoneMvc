namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageOrder_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public int Mode { get; set; }

        public SP_ManageOrder_Params_VM()
        {
        }
    }
}