using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentReview_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Description { get; set; }
        public string ReviewImage { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentReview_Params_VM()
        {
            Description = String.Empty;
            ReviewImage = String.Empty;
        }
    }
}