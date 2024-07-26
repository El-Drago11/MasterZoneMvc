using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessOwner_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public int RoleId { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string DOB { get; set; }
        public string RejectionReason { get; set; }
        public long BusinessCategoryId { get; set; }
        public int IsPrimeMember { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string UniqueUserId { get; set; }
        public int Verified { get; set; }

        public SP_InsertUpdateBusinessOwner_Params_VM()
        {
            Email = string.Empty;
            Password = string.Empty;
            PhoneNumber = string.Empty;
            PhoneNumberCountryCode = string.Empty;
            BusinessName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Address = string.Empty;
            DOB = string.Empty;
            RejectionReason = string.Empty;
            UniqueUserId = string.Empty;
        }
    }
}