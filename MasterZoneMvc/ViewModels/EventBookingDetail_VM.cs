using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EventBookingDetail_VM
    {
        public long EventBookingId { get; set; }
        public long EventId { get; set; }
        public long UserLoginId { get; set; }
        public long OrderId { get; set; }
        public string EventTicketQRCode { get; set; }
        public int CountofTicket { get; set; }
        public string SponsorImageWithPath { get; set; }
        public string AdditionalInformation { get; set; }
        public string SponsorLink { get; set; }
        public string Link { get; set; }
        public string SponsorTitle { get; set; }
        public string EventLocationURL { get; set; }
        public string EventQRCodeWithPath { get; set; }
        public string EventDetailImageWithPath { get; set; }
        public string EventDetailsName { get; set; }
        public string DetailsType { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string LandMark { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogoImageWithPath { get; set; }
        public string Title { get; set; }
        public string StartDateFormat { get; set; }
        public string EndDateFormat { get; set; }
        public string ShortDescription { get; set; }
        public string EventStartTime { get; set; }
        public string EventEndTime { get; set; }
        public string EventImageWithPath { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        // Order 
        public long ItemId { get; set; } // EventID/PackageID/ClassID/CertificateID
        public string ItemType { get; set; } // Event/Package/Class  //PurchaseType
        public int OnlinePayment { get; set; } // Online(1)/Offline(0)
        public string PaymentMethod { get; set; }// Cash/GPay/UPI/CreditCard
        public long CouponId { get; set; }
        public decimal CouponDiscountValue { get; set; } // $5 or %10 
        public decimal TotalDiscount { get; set; }  // Discount Amount $5 or 100 if order amount is 1000. 
        public int IsTaxable { get; set; } // not(0), Yes(1)
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; } // Final Total amount inclusive GST
        public int Status { get; set; } // Paid(1),New,Refunded


        // Payment Response
        public string Provider { get; set; } //ManualPayment,AdminBypass,CCAvenue
        public DateTime DateStamp { get; set; } // Order Date
        public string ResponseStatus { get; set; } // Completed,Pending
        public string TransactionID { get; set; }
        public int Approved { get; set; }

    }
}