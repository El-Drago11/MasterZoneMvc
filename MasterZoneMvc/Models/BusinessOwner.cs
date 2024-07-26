using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessOwner
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(UserLogin))]
        public long UserLoginId { get; set; }
        //public UserLogin UserLogin { get; set; }

        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }
        public string DOB { get; set; }
        public DateTime DOB_DateTime { get; set; }

        public int IsAccountAccepted { get; set; }
        public string RejectionReason { get; set; }


        //[ForeignKey(nameof(BusinessCategory))]
        public long BusinessCategoryId { get; set; }
        //public BusinessCategory BusinessCategory { get; set; }

        public int IsPrimeMember { get; set; }

        public string ProfileImage { get; set; }
        public string BusinessLogo { get; set; }
        public string About { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal SpecialPrice { get; set; }
        public string SpecialDuration { get; set; }
        public int Verified { get; set; } // 1 = Verified , 2 = R, 3= TM
        public string Experience { get; set; }
        public int Privacy_UniqueUserId { get; set; } // 1= Private(visible to only user), 2 = Public (visible to all),3 = Protected(visible to related/linked users)
        public string OfficialWebSiteUrl { get; set; }
        public long StudentUserLoginId { get; set; } // To save the student Login-Id

        public int IsBranch { get; set; } // 0 = Is Main Branch , 1 = Is Sub branch 
        public int ShowOnHomePage { get; set; }
        public string CoverImage { get; set; }

    }
}
