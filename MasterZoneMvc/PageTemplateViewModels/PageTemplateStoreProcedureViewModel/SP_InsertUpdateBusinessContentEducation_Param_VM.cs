using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentEducation_Param_VM
    {

        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string University { get; set; }
        public string UniversityLogo { get; set; }
        public string UniversityImage { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentEducation_Param_VM()
        {

            University = string.Empty;
            UniversityLogo = string.Empty;
            UniversityImage = string.Empty;
            Description = string.Empty;

        }

    }
}