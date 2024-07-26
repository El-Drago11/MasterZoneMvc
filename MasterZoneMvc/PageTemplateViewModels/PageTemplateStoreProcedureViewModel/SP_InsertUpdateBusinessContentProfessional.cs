using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentProfessional
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string ProfessionalTitle { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentProfessional()
        {
            ProfessionalTitle = string.Empty;
            Description = string.Empty;
        }
    }
}