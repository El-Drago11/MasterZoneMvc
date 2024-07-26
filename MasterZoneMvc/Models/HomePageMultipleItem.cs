using System;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Home Page Multiple Item sections like Multiple Images, Multiple Videos sections.
    /// </summary>
    public class HomePageMultipleItem
    {
        public long Id { get; set; }
        public string Type { get; set; } // HomePageMultipleItemType => Multiple Image, Multiple Video
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
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