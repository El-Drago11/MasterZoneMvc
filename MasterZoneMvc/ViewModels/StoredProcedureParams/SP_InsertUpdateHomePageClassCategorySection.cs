namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateHomePageClassCategorySection
    {
        public long Id { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateHomePageClassCategorySection()
        {
            
        }

    }
}