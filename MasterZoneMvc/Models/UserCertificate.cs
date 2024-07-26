using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Certificates-License earned by User (Student,Instructor).
    /// </summary>
    public class UserCertificate
    {
        [Key]
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public long UserLoginId { get; set; }
        public long TrainingId { get; set; }
        public long CertificateBookingId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long LicenseId { get; set; }
        public string IssuedCertificateNumber { get; set; }
        public string CertificateFile { get; set; }
        public long ItemId { get; set; }
        public string ItemTable { get; set; }
    }
}