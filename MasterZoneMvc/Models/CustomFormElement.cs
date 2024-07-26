using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class CustomFormElement
    {
        [Key]
        public long Id { get; set; }
        public long CustomFormId { get; set; }
        public string CustomFormElementName { get; set; }
        public string CustomFormElementType { get; set; }
        public string CustomFormElementValue { get; set; }
        public string CustomFormElementPlaceholder { get; set; }
        public int CustomFormElementStatus { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}