using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class PaymentMode
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public int IsActive { get; set; }
    }
}