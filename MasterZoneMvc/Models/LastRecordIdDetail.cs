using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class LastRecordIdDetail
    {
        [Key]
        public long Id { get; set; }
        public string Key { get; set; }
        public string Prefix { get; set; }
        public int Value { get; set; }
    }
}