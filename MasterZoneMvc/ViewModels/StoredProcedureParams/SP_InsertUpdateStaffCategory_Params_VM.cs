using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateStaffCategory_Params_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateStaffCategory_Params_VM()
        {
            Name = string.Empty;
        }
    }
}