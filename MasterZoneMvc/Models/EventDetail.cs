using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class EventDetail
    {
        [Key]
        public long Id { get; set; }
        public long EventId { get; set; }
        public string DetailsType { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Link { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
    }
}