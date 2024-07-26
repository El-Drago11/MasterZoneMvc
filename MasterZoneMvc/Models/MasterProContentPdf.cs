using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class MasterProContentPdf
    {

        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public string ThumbnailPdf { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
    }
}