using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CouponConsumptionViewModel
    {
        public long Id { get; set; }
        public long CouponId { get; set; }
        public long ConsumerUserLoginId { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public string CouponCode { get; set; }
        public int IsFixedAmountCoupon { get; set; }
        public decimal CouponDiscountValue { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}