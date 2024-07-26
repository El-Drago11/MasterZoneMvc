using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Order
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ItemId { get; set; } // EventID/PackageID/ClassID/CertificateID
        public string ItemType { get; set; } // Event/Package/Class  //PurchaseType
        public int OnlinePayment { get; set; } // Online(1)/Offline(0)
        //public int Paid { get; set; }
        public string PaymentMethod { get; set; }// Cash/GPay/UPI/CreditCard

        public long CouponId { get; set; }
        public decimal CouponDiscountValue { get; set; } // $5 or %10 
        public decimal TotalDiscount { get; set; }  // Discount Amount $5 or 100 if order amount is 1000. 
        public int IsTaxable { get; set; } // not(0), Yes(1)
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; } // Final Total amount inclusive GST

        public int Status { get; set; } // Paid(1),Pending,Refunded
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long OwnerUserLoginId { get; set; }

    }
}