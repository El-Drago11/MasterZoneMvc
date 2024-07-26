using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageEventCategory_Params_VM
    {
        public long Id { get; set; }
        public int Mode { get; set; }
    }

    public class SP_ManageEventDetails_Params_VM
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public int Mode { get; set; }
        public long UserLoginId { get; set; }
    }
}