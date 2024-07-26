using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentCurriculum_Params_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CurriculumOptions { get; set; }
        public string CurriculumImage { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentCurriculum_Params_VM()
        {
            Title =  string.Empty;
            CurriculumOptions = string.Empty;
            CurriculumImage = string.Empty;
        }
    }
}