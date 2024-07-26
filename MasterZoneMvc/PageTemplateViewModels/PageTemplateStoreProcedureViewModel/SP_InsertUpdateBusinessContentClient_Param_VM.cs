using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentClient_Param_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientImage { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentClient_Param_VM()
        {
            Name = string.Empty;
            Description = String.Empty;
            ClientImage = String.Empty;
        }
    }
}