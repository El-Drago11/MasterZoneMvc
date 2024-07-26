using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateStudent_Params_VM
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public string UniqueUserId { get; set; }
        public int Gender { get; set; }
        public string BlockReason { get; set; }
        public string BusinessStudentProfileImage { get; set; }
        public long StudentUserLoginId { get; set; }

        public string OTP { get; set; }

        public SP_InsertUpdateStudent_Params_VM()
        {
            Email = string.Empty;
            Password = string.Empty;
            PhoneNumber = string.Empty;
            PhoneNumberCountryCode = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            ProfileImage = string.Empty;
            UniqueUserId = string.Empty;
            BlockReason = string.Empty;
            BusinessStudentProfileImage = string.Empty;
            OTP = string.Empty;
        }
    }
}