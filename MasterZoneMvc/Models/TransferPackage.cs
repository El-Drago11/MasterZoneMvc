using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class TransferPackage
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long TransferFromUserloginId { get; set; } //To be Transfer
        public long TransferToUserLoginId { get; set; } //To Whom Transfer
        public long PackageId { get; set; } // To PlanId 
        public long PlanBookingId { get; set; } 
        public string TransferDate { get; set; }
        public string TransferReason { get; set; }
        public string RejectionReason { get; set; }
        public int TransferStatus { get; set; } // 0 Pending, 1 Accepeted or 2 Rejected 
        public int TransferType { get; set; }
        public string TransferCity { get; set; }
        public string TransferState { get; set; }
        public int Limit { get; set; }
        public int Notification { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}