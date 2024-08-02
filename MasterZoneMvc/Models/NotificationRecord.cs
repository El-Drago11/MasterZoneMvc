using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class NotificationRecord
    {
        [Key]
        public long Id { get; set; }
        public string NotificationType { get; set; }
        public long FromUserLoginId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public long ItemId { get; set; } // Tables Primary Key
        public string ItemTable { get; set; } // Table Name
        public int IsNotificationLinkable { get; set; } // Is notification linkable
        public long OrderId { get; set; }
    }
}