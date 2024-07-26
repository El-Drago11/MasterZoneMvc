namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessImages_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateBusinessImages_Params_VM()
        {
            ImageTitle = string.Empty;
            Image = string.Empty;
        }
    }
}