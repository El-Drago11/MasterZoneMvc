namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateClassCategoryType_Params_VM
    {
        public long Id { get; set; }
        public long BusinessCategoryId { get; set; }
        public long ParentClassCategoryTypeId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int IsActive { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public string Description { get; set; }

        public SP_InsertUpdateClassCategoryType_Params_VM()
        {
            Name = string.Empty;
            Image = string.Empty;
            Description = string.Empty;
        }
    }
}