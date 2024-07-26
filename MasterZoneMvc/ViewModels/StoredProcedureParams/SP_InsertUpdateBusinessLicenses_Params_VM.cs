namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessLicenses_Params_VM
    {
        public long Id { get; set; }
        public long LicenseBookingId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }
        public int QuantityUsed { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateBusinessLicenses_Params_VM()
        {

        }
    }
}