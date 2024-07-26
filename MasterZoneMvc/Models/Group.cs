using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Group
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(BusinessOwner))]
        public long BusinessOwnerId { get; set; }
        //public BusinessOwner BusinessOwner { get; set; }

        public string Name { get; set; }
        public string GroupImage { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}