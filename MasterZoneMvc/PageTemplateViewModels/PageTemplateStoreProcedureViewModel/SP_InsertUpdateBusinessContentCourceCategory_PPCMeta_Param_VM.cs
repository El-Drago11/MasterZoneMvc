using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentCourceCategory_PPCMeta_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long CourseCategoryId { get; set; }
        public int Mode { get; set; }
    }
}