using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Student
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(UserLogin))]
        public long UserLoginId { get; set; }
        //public UserLogin UserLogin { get; set; }

        public int IsBlocked { get; set; }

        public string ProfileImage { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BlockReason { get; set; }
        public string BusinessStudentProfileImage { get; set; }

        //public ICollection<BusinessReview> BusinessReviews { get; set; }
        //public ICollection<Enquiry> Enquiries { get; set; }
    }
}