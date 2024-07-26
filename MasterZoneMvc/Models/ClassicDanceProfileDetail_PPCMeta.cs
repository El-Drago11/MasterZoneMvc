using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ClassicDanceProfileDetail_PPCMeta
    {
        [Key]
        public long Id { get; set; }
        public  long UserLoginId {get;set;}
        public long ProfilePageTypeId {get;set;}
        public string Title { get;set;}
        public string SubTitle { get;set;}
        public string ClassicImage { get; set; }
        public string Image { get; set; }
        public string ScheduleImage { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }
}