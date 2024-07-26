using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessUserContentResume_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Summary { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public int Freelance { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessUserContentResume_Param_VM()
        {
            Summary = string.Empty;
            Languages = string.Empty;
            Skills = string.Empty;
            Freelance = 0;
        }
    }
}