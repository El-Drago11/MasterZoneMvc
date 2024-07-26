﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Apartment
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string Blocks { get; set; }
        public string Areas { get; set; }

        public int Status { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}