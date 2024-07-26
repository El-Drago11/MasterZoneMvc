using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessNoticeBoard_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string StartDate { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessNoticeBoard_Param_VM()
        {
            StartDate = string.Empty;
            Description = string.Empty ;
        }

    }
}