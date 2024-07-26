using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentEvent_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentEvent_Param_VM()
        {
            Description = string.Empty;
        }

    }
}