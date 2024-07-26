using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class CouponConsumption
    {
        [Key]
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