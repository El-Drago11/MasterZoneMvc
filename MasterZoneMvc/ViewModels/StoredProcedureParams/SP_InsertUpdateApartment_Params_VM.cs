namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateApartment_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string Blocks { get; set; }
        public string Areas { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateApartment_Params_VM()
        {
            Name = "";
            Blocks = "";
            Areas = "";
        }
    }
}