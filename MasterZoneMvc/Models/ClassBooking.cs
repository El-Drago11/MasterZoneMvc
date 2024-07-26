using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ClassBooking
    {
        [Key]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ClassId { get; set; }
        public long StudentUserLoginId { get; set; }
        public string ClassQRCode { get; set; }

        // Class detials 
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClassPriceType { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }

        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public long BatchId { get; set; }
        public int Repeat_Purchase { get; set; }
    }
}