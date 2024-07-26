namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateApartmentBlock_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateApartmentBlock_Params_VM()
        {
            Name = string.Empty;
        }
    }
}