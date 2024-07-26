using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessCategory
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public long ParentBusinessCategoryId { get; set; }

        public string CategoryImage { get; set; }
        public string CategoryKey { get; set; }

        public int ProfilePageTypeId { get; set; }

        public string MenuTag { get; set; }
    }
}