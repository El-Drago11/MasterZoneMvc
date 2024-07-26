using System;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Home Page Banner Item (Image/ Video) on Home Page
    /// </summary>
    public class HomePageBannerItem
    {
        public long Id { get; set; }
        public string Type { get; set; } // Image, Video
        public string Image { get; set; }
        public string Video { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }
}