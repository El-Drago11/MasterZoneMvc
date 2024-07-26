using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Notification
    {
        [Key]
        public long Id { get; set; }
        public long NotificationRecordId { get; set; }
        public long UserLoginId { get; set; }
        public int IsRead { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}