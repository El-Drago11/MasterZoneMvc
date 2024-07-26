namespace MasterZoneMvc.ViewModels
{
    /// <summary>
    /// Required data for the booking of License (by business)
    /// </summary>
    public class CreateLicenseBooking_VM
    {
        public long UserLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }
        public long CouponId { get; set; }
        public int OnlinePayment { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmountPaid { get; set; }

        public string TransactionID { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentProvider { get; set; }
        public string PaymentResponseStatus { get; set; }
    }
}