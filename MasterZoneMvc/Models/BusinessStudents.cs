using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessStudents
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerId { get; set; }
        public long StudentId { get; set; }
    }
}