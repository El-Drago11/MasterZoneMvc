using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class UserEducation
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string SchoolName { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string Endmonth { get; set; }
        public string EndYear { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Grade { get; set; }
       

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}