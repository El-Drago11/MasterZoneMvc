using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateMasterProExtraInformation_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Pdf { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateMasterProExtraInformation_Params_VM()
        {
            Title = string.Empty;
            ShortDescription = string.Empty;
            Pdf = string.Empty;
        }
    }
}