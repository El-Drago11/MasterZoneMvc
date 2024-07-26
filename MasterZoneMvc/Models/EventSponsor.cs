using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class EventSponsor
    {
        [Key]
        public long Id { get; set; }
        public long EventId { get; set; }
        public string SponsorTitle { get; set; }
        public string SponsorIcon { get; set; }
        public string SponsorLink { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
    }
}