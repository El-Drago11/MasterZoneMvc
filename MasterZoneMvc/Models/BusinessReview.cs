using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessReview
    {
        [Key]
        public long Id { get; set; }


        //[ForeignKey(nameof(BusinessOwner))]
        public long BusinessOwnerId { get; set; }
        //public BusinessOwner BusinessOwner { get; set; }


        //[ForeignKey(nameof(Student))]
        public long StudentId { get; set; }
        //public Student Student { get; set; }


        public int Rating { get; set; }
        public string Review { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpadatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}