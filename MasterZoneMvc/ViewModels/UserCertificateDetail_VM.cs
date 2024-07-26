using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserCertificateDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string CertificateIcon { get; set; }
        public string CertificateImageWithPath { get; set; }
        public string ShortDescription { get; set; }
        public string AdditionalInformation { get; set; }
        public decimal Price { get; set; }
    }
}