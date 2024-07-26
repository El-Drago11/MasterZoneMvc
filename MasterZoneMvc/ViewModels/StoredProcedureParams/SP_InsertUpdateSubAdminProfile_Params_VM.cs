using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateSubAdminProfile_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateSubAdminProfile_Params_VM()
        {
            FirstName = String.Empty;
            LastName = string.Empty;
            Email = String.Empty;
            Password = String.Empty;
            PhoneNumber = String.Empty;
            ProfileImage = String.Empty;
        }
    }
}