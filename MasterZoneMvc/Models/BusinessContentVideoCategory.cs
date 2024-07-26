using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    public class BusinessContentVideoCategory
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}