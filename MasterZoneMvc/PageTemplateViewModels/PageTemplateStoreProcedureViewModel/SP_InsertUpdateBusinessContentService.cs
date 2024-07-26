using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentService
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ServiceTitle { get; set; }
        public string ShortDescription { get; set; }
        public int SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
    }
}