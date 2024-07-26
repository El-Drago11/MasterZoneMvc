using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class NotificationTransfer
    {
        [Key]
        public long Id { get; set; }
        public long TransferRequestId { get; set; } //UserLoginId
        public string TransferSenderId { get; set; } //To selected Business Owner Id
        public string NotificationMessage { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}