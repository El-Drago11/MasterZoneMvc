using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Contact
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Location1 { get; set; }
        public string ContactNumber1 { get; set; }
        public string Location2 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Location3 { get; set; }
        public string ContactNumber3 { get; set; }
        public string Location4 { get; set; }
        public string ContactNumber4 { get; set; }
        public string Location5 { get; set; }
        public string ContactNumber5 { get; set; }
        public string Location6 { get; set; }
        public string ContactNumber6 { get; set; }
        public string Location7 { get; set; }
        public string ContactNumber7 { get; set; }
        public string Location8 { get; set; }
        public string ContactNumber8 { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }
}