using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageMenu_Params_VM
    {
        public long Id { get; set; }
        public long ParentMenuId { get; set; }
        public string Tag { get; set; }
        public int Mode { get; set; }

        public SP_ManageMenu_Params_VM()
        {
            Tag = "";
        }
    }
}