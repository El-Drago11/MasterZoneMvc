using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ClassCategoryType
    {
        [Key]
        public long Id { get; set; }
        public long ParentClassCategoryTypeId { get; set; }
        public long BusinessCategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public string Description { get; set; }
        public int ShowOnHomePage { get; set; }

    }
}