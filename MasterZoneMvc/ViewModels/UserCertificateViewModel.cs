using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserCertificateViewModel
    {
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
    
    public class UserCertificate_VM
    {
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
        public string CertificateFileWithPath { get; set; }
        public long ItemId { get; set; }
        public string ItemTable { get; set; }

    }

    public class UserCertificateBasicInfo_VM {
        public long CertificateId { get; set; }
        public string CertificateName { get; set; }
        public string CertificateIconWithPath{ get; set; }

        public long LicenseId { get; set; }
        public string LicenseName { get; set; }
        public string LicenseLogoWithPath { get; set; }
    }

}