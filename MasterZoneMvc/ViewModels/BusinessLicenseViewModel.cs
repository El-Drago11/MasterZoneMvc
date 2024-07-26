using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessLicenseViewModel
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
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    
}