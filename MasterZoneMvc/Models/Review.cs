using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Review
    {
        [Key]
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public int Rating { get; set; }
        public string ReviewBody { get; set; }
        public long ReviewerUserLoginId { get; set; }

        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}