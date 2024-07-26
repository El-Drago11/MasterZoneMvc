using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessCourseCategory_Param_Vm
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseCategoryImage { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessCourseCategory_Param_Vm()
        {
            Title = String.Empty;
            Description = String.Empty;
            CourseCategoryImage = String.Empty;
        }

    }
}