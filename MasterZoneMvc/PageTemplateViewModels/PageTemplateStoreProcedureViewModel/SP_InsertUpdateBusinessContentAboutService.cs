using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentAboutService
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string AboutServiceTitle { get; set; }
        public string AboutServiceDescription { get; set; }
        public string AboutServiceIcon { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateBusinessContentAboutService()
        {
            AboutServiceTitle = String.Empty;
            AboutServiceDescription = String.Empty;
            AboutServiceIcon = String.Empty;
        }
    }
}