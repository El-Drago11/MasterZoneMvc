namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateHomePageBannerItem_Params_VM
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }

        public SP_InsertUpdateHomePageBannerItem_Params_VM()
        {
            Type = string.Empty;
            Image = string.Empty;
            Video = string.Empty;
            Text = string.Empty;
            Link = string.Empty;
        }

    }
}