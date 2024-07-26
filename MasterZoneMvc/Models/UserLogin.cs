using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class UserLogin
    {
        [Key]
        public long Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public int EmailConfirmed { get; set; }

        //[ForeignKey("Role")]
        public long RoleId { get; set; }
        //public virtual Role Role { get; set; }

        //public int IsBlocked { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public long DeletedByLoginId { get; set; }

        public int Status { get; set; }

        //public ICollection<BusinessOwner> BusinessOwners { get; set; }
        //public ICollection<Student> Students { get; set; }
        //public ICollection<SubAdmin> SubAdmins { get; set; }
        //public ICollection<Staff> Staffs { get; set; }

        //public ICollection<GroupMember> GroupMembers { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string FullAddressLocation { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string MasterId { get; set; }

        public string GoogleUserId { get; set; }
        public string FacebookUserId { get; set; }
        public string GoogleAccessToken { get; set; }
        public string FacebookAccessToken { get; set; }

        public int IsCertified { get; set; }
        public int IsMasterCertified { get; set; }

        public string UniqueUserId { get; set; }
        public int Gender { get; set; }

        public string FacebookProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }

        public string ResetPasswordToken { get; set; }
        public string About { get; set; }
        public DateTime DOB { get; set; }

        public string OTP { get; set; }

    }
}