using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateOrder_Params_VM
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
        public decimal Gst { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public long OwnerUserLoginId { get; set; }


        public SP_InsertUpdateOrder_Params_VM()
        {
            Id = 0;
            UserLoginId = 0;
            ItemType = "";
            OnlinePayment = 0;
            PaymentMethod = "";
            CouponId = 0;
            CouponDiscountValue = 0;
            TotalDiscount = 0;
            IsTaxable = 0;
            Gst = 0;
            TotalAmount = 0;
            Status = 0;
            Mode = 0;
            OwnerUserLoginId = 0;
        }
    }
}