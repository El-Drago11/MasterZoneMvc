using System;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Home Page Multiple Featured Videos Sections. 2 Videos on Home Page
    /// </summary>
    public class HomePageFeaturedVideo
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
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