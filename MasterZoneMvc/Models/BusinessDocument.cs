using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessDocument
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(BusinessOwner))]
        public long BusinessOwnerId { get; set; }
        //public BusinessOwner BusinessOwner { get; set; }

        //[ForeignKey(nameof(BusinessDocumentType))]
        public long BusinessDocumentTypeId { get; set; }
        //public BusinessDocumentType BusinessDocumentType { get; set; }

        public string DocumentPath { get; set; }
    }
}