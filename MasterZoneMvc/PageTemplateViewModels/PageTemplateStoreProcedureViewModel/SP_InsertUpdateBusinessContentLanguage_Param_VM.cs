using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentLanguage_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Language { get; set; }
        public string LanguageIcon { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentLanguage_Param_VM()
        {
           
            Language = string.Empty;
            LanguageIcon = string.Empty;
        }
    }
}