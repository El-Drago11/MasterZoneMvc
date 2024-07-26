using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class MasterProResume_PPCMeta
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Age { get; set; }
        public string Nationality { get; set; }
        public string UploadCV { get; set; }
        public string Freelance { get; set; }
        public string Skype { get; set; }
        public string Languages { get; set; }

    }
}