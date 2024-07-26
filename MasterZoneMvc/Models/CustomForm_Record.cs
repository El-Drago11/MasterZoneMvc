using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class CustomForm_Record
    {
        [Key]
        public long Id { get; set; }
        public long FormId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApplicantUserLoginId { get; set; } // To Add UserLoginID to save the detail
        public long FormElementId { get; set; } // To Add the Form Element Id 
        public string FormElementName { get; set; }
        public string FormElementValue { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }

    }
}