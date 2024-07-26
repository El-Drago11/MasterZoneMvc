using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CheckoutViewModel
    {
    }

    public class CheckoutItem_Params_VM
    {
        public long itemId { get; set; }
        public int paymentMode { get; set; }
        public int PlanType { get; set; }
        public long couponId { get; set; }
        public decimal totalAmountPaid { get; set; }
        public long StudentLoginId { get; set; }
    }

    public class CheckOutClass_Parama_VM : CheckoutItem_Params_VM
    {
        public string joinClassDate { get; set; }
        public long BatchId { get; set; }
    }
    
    public class CheckoutLicense_Params_VM : CheckoutItem_Params_VM
    {
        public int quantity { get; set; }
    }
    public class CheckOutCourse_Parama_VM : CheckoutItem_Params_VM
    {
        public string joinCourseDate { get; set; }

    }

}