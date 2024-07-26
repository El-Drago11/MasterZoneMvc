using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Home Page Featured Card section. Image Card, Video Card on Home Page
    /// </summary>
    public class HomePageFeaturedCardSection
    {
        public long Id { get; set; }
        public string Type { get; set; } // SingleImage, SingleVideo
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}