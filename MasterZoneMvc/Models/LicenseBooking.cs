using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Business Owner Books Licenses from SuperAdmin
    /// </summary>
    public class LicenseBooking
    {
        [Key]
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

        public int Status { get; set; } // Pending(1), Approved(2), Declined(3). [enum LicenseBookingStatus]
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}