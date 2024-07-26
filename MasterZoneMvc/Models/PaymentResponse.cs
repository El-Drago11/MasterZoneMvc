using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class PaymentResponse
    {
        [Key]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string Provider { get; set; } //ManualPayment,AdminBypass,CCAvenue
        public DateTime DateStamp { get; set; } // Order Date
        public string ResponseStatus { get; set; } // Completed,Pending
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public int Approved { get; set; }
        public string Description { get; set; }
        public string Method { get; set; } // GPay,Paytm,UPI,CreditCard,DebitCard,Other

        public long CreatedByLoginId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}