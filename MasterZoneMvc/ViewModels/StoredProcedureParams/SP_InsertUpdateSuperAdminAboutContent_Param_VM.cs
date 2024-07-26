using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateSuperAdminAboutContent_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string AboutTitle { get; set; }
        public string AboutDescription { get; set; }
        public string OurMissionTitle { get; set; }
        public string OurMissionDescription { get; set; }
        public string Image { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateSuperAdminAboutContent_Param_VM()
        {
            AboutTitle = String.Empty;
            AboutDescription = String.Empty;
            OurMissionTitle = String.Empty;
            OurMissionDescription = String.Empty;
            Image = String.Empty;
        }
    }
}