using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class About
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string AboutTitle { get; set; }
        public string AboutDescription { get; set; }
        public string OurMissionTitle { get; set; }
        public string OurMissionDescription { get; set; }
        public string Image { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }
}