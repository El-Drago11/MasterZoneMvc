namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateHomePageFeaturedVideo_Params_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateHomePageFeaturedVideo_Params_VM()
        {
            Title = string.Empty;
            Description = string.Empty;
            Thumbnail = string.Empty;
            Video = string.Empty;
        }

    }
}