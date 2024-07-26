namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageApartmentBlock_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApartmentId { get; set; }
        public int Mode { get; set; }

        public SP_ManageApartmentBlock_Params_VM()
        {

        }
    }
}