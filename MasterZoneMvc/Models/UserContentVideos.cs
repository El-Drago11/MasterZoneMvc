using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class UserContentVideos
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string VideoThumbnail { get; set; }

        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
        public string VideoDescription { get; set; }
    }
}