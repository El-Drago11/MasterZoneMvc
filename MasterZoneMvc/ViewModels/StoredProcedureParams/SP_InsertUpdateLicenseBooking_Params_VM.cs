namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateLicenseBooking_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateLicenseBooking_Params_VM()
        {
            Id = 0;
            OrderId = 0;
        }
    }
}