using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class OrderViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ItemId { get; set; } 
        public string ItemType { get; set; } 
        public int OnlinePayment { get; set; }
        public string PaymentMethod { get; set; }

        public long CouponId { get; set; }
        public decimal CouponDiscountValue { get; set; } 
        public decimal TotalDiscount { get; set; } 
        public int IsTaxable { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }

        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long OwnerUserLoginId { get; set; }

        public string CreatedOn_FormatDate { get; set; }
    }

}