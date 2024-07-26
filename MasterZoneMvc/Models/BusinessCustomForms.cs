using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessCustomForm
    {
        [Key]

        public long Id { get; set; }
        public long CustomFormId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long TransferById { get; set; }


    }
}