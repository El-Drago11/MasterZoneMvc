namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageApartmentBooking_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public long ApartmentId { get; set; }
        public int Mode { get; set; }

        public SP_ManageApartmentBooking_Params_VM()
        {

        }
    }
}