using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_ManageAllClassTimeTableDetail_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public long InstructorLoginId { get; set; }
        public string ClassDays { get; set; }
        public string ClassMode { get; set; }
        public int Mode { get; set; }
        public SP_ManageAllClassTimeTableDetail_Param_VM()
        {
            ClassDays = string.Empty;
            ClassMode = string.Empty; 
        }
    }
}