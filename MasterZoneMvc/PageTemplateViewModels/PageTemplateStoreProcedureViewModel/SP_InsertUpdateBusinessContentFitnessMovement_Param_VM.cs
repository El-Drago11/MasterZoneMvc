using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentFitnessMovement_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Requirements { get; set; }
        public string Investment { get; set; }
        public string Inclusions { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentFitnessMovement_Param_VM()
        {
            Title = string.Empty;
            Requirements = string.Empty;
            Investment = string.Empty;
            Inclusions = string.Empty;
            Description = string.Empty;
        }
    }
}