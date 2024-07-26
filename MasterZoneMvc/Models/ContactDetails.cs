using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ContactDetails
    {
        [Key]
        public long Id { get; set; }
        public string Email { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContactTitle { get; set; }
        public string ContactDescription { get; set; }
    }
}