using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentAccessCourse_Param_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string CourseImage { get; set; }
        public string AccessCourse { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentAccessCourse_Param_VM()
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            Description = string.Empty;
            CourseImage = string.Empty;
            AccessCourse = string.Empty;
        }

    }
}