using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Batch
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public long GroupId { get; set; }
        public long InstructorLoginId { get; set; }
        public int StudentMaxStrength { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime ScheduledOnDateTime { get; set; }
        public int ClassDurationSeconds { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public int Status { get; set; }
    }
}