using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Certificate Licences.
    /// </summary>
    public class License
    {
        [Key]
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LicenseLogo { get; set; }
        public string CertificateImage { get; set; }
        public string SignatureImage { get; set; }

        public int IsPaid { get; set; }
        public string CommissionType { get; set; } // Fixed or Percentage
        public decimal CommissionValue { get; set; } // Fixed: 1000.00 , Percentage: 15.00
                                                   //  public int MaxPerUser { get; set; }
        public int AchievingOrder { get; set; }
        public int LicenseDuration { get; set; }
        public string TimePeriod { get; set; }
        public string LicensePermissions { get; set; }

        public int Status { get; set; } // Active or Inactive
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public string Signature2Image { get; set; }
        public string Signature3Image { get; set; }
        public decimal GSTPercent { get; set; }
        public string GSTDescription { get; set; }
        public decimal MinSellingPrice { get; set; }
        public decimal Price { get; set; }
        public string LicenseCertificateHTMLContent { get; set; }
        public int IsLicenseToTeach { get; set; }
        public int MasterPro {  get; set; }
        public string LicenseToTeach_Type { get; set; }
        public string LicenseToTeach_DisplayName { get; set; }
    }
}