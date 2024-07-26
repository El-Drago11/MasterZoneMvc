using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentClassicDance_PPCMeta_Param_VM
    {

        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get;set; }
        public string TechniqueItemList { get; set; }
        public int Mode { get; set; }
        public long SubmittedByLoginId { get; set; }
        public SP_InsertUpdateBusinessContentClassicDance_PPCMeta_Param_VM ()
        {
            Title = string.Empty;
            Description = string.Empty;
            TechniqueItemList = string.Empty;
        }

    }
}