using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    public class ApartmentArea
    {
        [Key]
            public long Id { get; set; }
            public long ApartmentId { get; set; }
            public string Name { get; set; }
            public string SubTitle { get; set; }
            public string ApartmentImage { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int IsActive { get; set; }

            public DateTime CreatedOn { get; set; }
            public long CreatedByLoginId { get; set; }
            public DateTime UpdatedOn { get; set; }
            public long UpdatedByLoginId { get; set; }
            public int IsDeleted { get; set; }
            public DateTime DeletedOn { get; set; }
    }
}