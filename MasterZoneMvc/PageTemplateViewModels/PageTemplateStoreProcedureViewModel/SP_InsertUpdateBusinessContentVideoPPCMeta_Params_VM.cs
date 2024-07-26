using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentVideoPPCMeta_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string VideoDescription { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentVideoPPCMeta_Params_VM()
        {
            Title = string.Empty;
            VideoDescription = string.Empty;
        }
            

          
    }
}