using System;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Home Page Featured Class-Category section to show.
    /// </summary>
    public class HomePageClassCategorySection
    {
        public long Id { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }
}