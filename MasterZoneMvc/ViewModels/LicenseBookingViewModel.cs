using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class LicenseBookingViewModel
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }

        public int LicenseIsPaid { get; set; }
        public decimal LicensePrice { get; set; }
        public string LicenseCommissionType { get; set; }
        public decimal LicenseCommissionValue { get; set; }
        public decimal LicenseGSTPercent { get; set; }
        public string LicenseGSTDescription { get; set; }
        public decimal LicenseMinSellingPrice { get; set; }

        public int Status { get; set; } // Pending, Approved, Declined.
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class LicenseBookingRequest_ForBO_Pagination_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }

        //public int LicenseIsPaid { get; set; }
        public decimal LicensePrice { get; set; }
        //public string LicenseCommissionType { get; set; }
        //public decimal LicenseCommissionValue { get; set; }
        //public decimal LicenseGSTPercent { get; set; }
        //public string LicenseGSTDescription { get; set; }
        //public decimal LicenseMinSellingPrice { get; set; }

        public int Status { get; set; } // Pending, Approved, Declined.
        
        public string CertificateName { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseDescription { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoWithPath { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public string OrderPaymentMethod { get; set; }
        public string StatusText { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }
    
    public class LicenseBookingRequest_ForBO_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }

        public int LicenseIsPaid { get; set; }
        public decimal LicensePrice { get; set; }
        public string LicenseCommissionType { get; set; }
        public decimal LicenseCommissionValue { get; set; }
        public decimal LicenseGSTPercent { get; set; }
        public string LicenseGSTDescription { get; set; }
        public decimal LicenseMinSellingPrice { get; set; }

        public int Status { get; set; } // Pending, Approved, Declined.
        
        public string CertificateName { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseDescription { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoWithPath { get; set; }
        public int LicenseAchievingOrder { get; set; }
        public string LicenseTimePeriod { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public string OrderPaymentMethod { get; set; }
        public int PaymentIsApproved { get; set; }
        public string StatusText { get; set; }
        public string CommissionTypeName { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
    }

    public class LicenseBookingRequest_ForSA_Pagination_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }

        //public int LicenseIsPaid { get; set; }
        public decimal LicensePrice { get; set; }
        //public string LicenseCommissionType { get; set; }
        //public decimal LicenseCommissionValue { get; set; }
        //public decimal LicenseGSTPercent { get; set; }
        //public string LicenseGSTDescription { get; set; }
        //public decimal LicenseMinSellingPrice { get; set; }

        public int Status { get; set; } // Pending, Approved, Declined.

        public string CertificateName { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseDescription { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoWithPath { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public string OrderPaymentMethod { get; set; }
        public long PaymentResponseId { get; set; }
        public int PaymentIsApproved { get; set; }
        public string StatusText { get; set; }
        public string BusinessName { get; set; }
        public string BusinessOwnerFullName { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class LicenseBookingRequest_ForSA_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }

        public int LicenseIsPaid { get; set; }
        public decimal LicensePrice { get; set; }
        public string LicenseCommissionType { get; set; }
        public decimal LicenseCommissionValue { get; set; }
        public decimal LicenseGSTPercent { get; set; }
        public string LicenseGSTDescription { get; set; }
        public decimal LicenseMinSellingPrice { get; set; }

        public int Status { get; set; } // Pending, Approved, Declined.

        public string CertificateName { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseDescription { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoWithPath { get; set; }
        public int LicenseAchievingOrder { get; set; }
        public string LicenseTimePeriod { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public string OrderPaymentMethod { get; set; }
        public long PaymentResponseId { get; set; }
        public int PaymentIsApproved { get; set; }
        public string StatusText { get; set; }
        public string CommissionTypeName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessOwnerFullName { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
    }

    public class BusinessBookedLicense_Pagination_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public long LicenseBookingId { get; set; }
        public int Quantity { get; set; }
        public int QuantityUsed { get; set; }

        public decimal LicensePrice { get; set; }

        public string CertificateName { get; set; }
        public string LicenseTitle { get; set; }
        public string LicenseDescription { get; set; }
        public string LicenseLogo { get; set; }
        public string LicenseLogoWithPath { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class BusinessBookedLicense_VM
    {
        public long Id { get; set; }
        public long LicenseBookingId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long LicenseId { get; set; }
        public int Quantity { get; set; }

        public int QuantityUsed { get; set; }

        public int LicenseIsPaid { get; set; }
        public decimal LicensePrice { get; set; }
        public string LicenseCommissionType { get; set; }
        public decimal LicenseCommissionValue { get; set; }
        public decimal LicenseGSTPercent { get; set; }
        public string LicenseGSTDescription { get; set; }
        public decimal LicenseMinSellingPrice { get; set; }

        public string LicenseTitle { get; set; }
    }
}