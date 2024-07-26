﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessContentVideos_PPCMeta
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string VideoDescription { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
    }
}