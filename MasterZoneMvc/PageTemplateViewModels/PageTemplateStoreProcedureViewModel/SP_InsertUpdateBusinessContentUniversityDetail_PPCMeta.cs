using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentUniversityDetail_PPCMeta
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UniversityId { get; set; }
        public int Mode { get; set; }
    }
}