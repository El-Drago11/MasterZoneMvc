namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageClassBatches_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ClassId { get; set; }
        public long BatchId { get; set; }
        public int Mode { get; set; }

        public SP_ManageClassBatches_Params_VM()
        {
            
        }
    }
}