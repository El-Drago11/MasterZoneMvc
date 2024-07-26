using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class DocumentDetails
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerId { get; set; } // Stores Business-Owner-Login-Id
        public string DocumentTitle { get; set; }
        public string DocumentFile { get; set; }
        public string RejectionReason { get; set; }
        public int Status { get; set; } // 0 is pending , 1 is Accepted and 2 is rejected
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}