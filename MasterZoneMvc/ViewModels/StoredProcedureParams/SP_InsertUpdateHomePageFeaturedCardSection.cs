using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateHomePageFeaturedCardSection
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateHomePageFeaturedCardSection()
        {
            Type = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            ButtonLink = string.Empty;
            ButtonText = string.Empty;
            Thumbnail = string.Empty;
            Video = string.Empty;
        }

    }
}