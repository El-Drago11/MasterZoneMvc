using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class UserResumeContent
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Summary { get; set; }
        public string Languages { get; set; }
        public  string Skills { get; set; }
        public int Freelance { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }

    }
}