using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessService
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; } // BusinessOwnerLoginId
        public string Title { get; set; }
        public string Icon { get; set; }
        public string FeaturedImage { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int ServiceType { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}