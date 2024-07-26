using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessSponsor_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string SponsorTitle { get; set; }
        public string SponsorIcon { get; set; }
        public string SponsorLink { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessSponsor_Params_VM()
            {
            SponsorLink = string.Empty;
            SponsorTitle = string.Empty;
            SponsorIcon = string.Empty;
            }
    }
}