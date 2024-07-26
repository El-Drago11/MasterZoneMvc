using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageUserLogin_Params_VM
    {
        public long Id { get; set; }
        public string UniqueUserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Mode { get; set; }

        public SP_ManageUserLogin_Params_VM()
        {
            UniqueUserId = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}