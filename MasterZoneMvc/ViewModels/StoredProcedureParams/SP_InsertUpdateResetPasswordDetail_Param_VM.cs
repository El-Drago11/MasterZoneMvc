using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateResetPasswordDetail_Param_VM
    {

        public long Id { get; set; }
        public string ResetPasswordToken { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateResetPasswordDetail_Param_VM()
        {
            ResetPasswordToken = string.Empty;
        }
    }
}