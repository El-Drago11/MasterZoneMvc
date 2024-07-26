using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageHomePageFeaturedCardSection_Params_VM
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public int Mode { get; set; }
        public SP_ManageHomePageFeaturedCardSection_Params_VM()
        {
            Type = string.Empty;
        }
    }
}