using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CreateClassBooking_VM
    {
        public long UserLoginId { get; set; }
        public long ClassId { get; set; }
        public long CouponId { get; set; }
        public int OnlinePayment { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public string JoinClassDate { get; set; }

        public string TransactionID { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentProvider { get; set; }
        public string PaymentResponseStatus { get; set; }
        public long BatchId { get; set; }
        public string offer_code { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string order_id { get; set; }
        public string cancel_url { get; set; }
        public string redirect_url { get; set; }
        public string merchant_id { get; set; }
    }
}