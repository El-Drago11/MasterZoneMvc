using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Enquiry
    {
        [Key]
        public long Id { get; set; }

        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public DateTime DOB_DateTimeFormat { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }

        public string Activity { get; set; }
        public long LevelId { get; set; }
        public long BusinessPlanId { get; set; }
        public long ClassId { get; set; }
        public string StartFromDate { get; set; }
        public DateTime StartFromDate_DateTimeFormat { get; set; }
        public string Status { get; set; }

        public long StaffId { get; set; }
        public string FollowUpDate { get; set; }
        public DateTime FollowUpDate_DateTimeFormat { get; set; }
        public string Notes { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public int EnquiryStatus { get; set; }
    }
}