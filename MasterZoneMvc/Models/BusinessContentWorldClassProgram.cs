﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessContentWorldClassProgram
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId    { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Options { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

    }
}