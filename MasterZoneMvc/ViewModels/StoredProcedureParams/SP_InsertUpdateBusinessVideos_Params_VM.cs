namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessVideos_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public long BusinessContentVideoCategoryId { get; set; }

        public SP_InsertUpdateBusinessVideos_Params_VM()
        {
            VideoTitle = string.Empty;
            VideoLink = string.Empty;
            Thumbnail = string.Empty;
            Description = string.Empty;
        }
    }
}