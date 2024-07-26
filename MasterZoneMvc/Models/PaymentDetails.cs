using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class PaymentDetails
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string PaymentModeType { get; set; }
        public string PaymentModeDetail { get; set; }
    }
}