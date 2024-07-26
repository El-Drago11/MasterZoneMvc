using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentMuchMoreService_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Content { get; set; }
        public string ServiceIcon { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentMuchMoreService_Param_VM()
        {
            Content = string.Empty;
            ServiceIcon = string.Empty;
        }
    }
}