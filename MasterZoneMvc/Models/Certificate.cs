using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Certificate
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string CertificateIcon { get; set; }
        public string ShortDescription { get; set; }
        //public string AdditionalInformation { get; set; }
        //public decimal Price { get; set; }
        //public string CertificatePermissions { get; set; }
        public long ProfilePageTypeId { get; set; }

        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public string CertificateTypeKey { get; set; }
        public int ShowOnHomePage { get; set; }
        public string Link { get; set; }
    }
}