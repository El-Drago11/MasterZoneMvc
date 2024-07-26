using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class ForgotPassword_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Email { get; set; }
        public int Mode { get; set; }
        public ForgotPassword_Param_VM()
        {
            Email = string.Empty;
        }

    }
}