using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateEventCategory_Params_VM
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateEventCategory_Params_VM()
        {
            CategoryName = string.Empty;
        }
    }
}