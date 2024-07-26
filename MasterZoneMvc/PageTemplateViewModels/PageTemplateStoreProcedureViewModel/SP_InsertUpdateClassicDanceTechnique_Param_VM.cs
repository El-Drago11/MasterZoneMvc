using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateClassicDanceTechnique_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string TechniqueImage { get; set; }
        public int Mode { get; set; }
        public long SubmittedByLoginId { get; set; }
        public SP_InsertUpdateClassicDanceTechnique_Param_VM()
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            TechniqueImage = string.Empty;
        }
    }
}