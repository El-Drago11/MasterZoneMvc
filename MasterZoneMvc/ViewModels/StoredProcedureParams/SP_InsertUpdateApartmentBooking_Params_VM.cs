namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateApartmentBooking_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        public long ApartmentBlockId { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        public long ApartmentAreaId { get; set; }
        public string Activity { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateApartmentBooking_Params_VM()
        {
            MasterId = string.Empty;
            FlatOrVillaNumber = string.Empty;
            Phase = string.Empty;
            Lane = string.Empty;
            OccupantType = string.Empty;
            Activity = string.Empty;
        }
    }
}