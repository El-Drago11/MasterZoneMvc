using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class EventBooking
    {
        [Key]
        public long Id { get; set; }
        public long EventId { get; set; }
        public long UserLoginId { get; set; }
        public long OrderId { get; set; }
        public string EventTicketQRCode { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}