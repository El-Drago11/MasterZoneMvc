namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateApartmentArea_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public string ApartmentImage { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int IsActive { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateApartmentArea_Params_VM()
        {
            Name = string.Empty;
            SubTitle = string.Empty;
            Description = string.Empty;
            ApartmentImage = string.Empty;
        }
    }
}