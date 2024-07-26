using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// When License Request is approved by SuperAdmin then its record and qty. will be added here.
    /// Then this table is used for Further process. To update quantity used by B-Owner when License is given to Student
    /// Above functionality is merged with LicenseBooking. [on 2023-12-08]
    /// Now this is not used and open for modification. or remove this model. not included in DAL.
    /// </summary>
    public class BusinessLicenses
    {
        [Key]
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
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}