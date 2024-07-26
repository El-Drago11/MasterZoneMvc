using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ProfilePageType
    {
        [Key]
        public long Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}