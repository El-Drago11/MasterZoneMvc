using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentPlanPPCMeta_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessPlanTitle { get; set; }
        public string BusinessPlanDescription { get; set; }
        public int SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentPlanPPCMeta_Params_VM ()
            {
            BusinessPlanTitle = string.Empty;
            BusinessPlanDescription = string.Empty;
            }

    }
}