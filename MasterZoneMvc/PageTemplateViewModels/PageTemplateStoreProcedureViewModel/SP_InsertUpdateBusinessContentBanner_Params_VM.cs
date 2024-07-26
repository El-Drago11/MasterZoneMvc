using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentBanner_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public string BannerImage { get; set; }
        public int SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentBanner_Params_VM()
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            Description = string.Empty;
            ButtonText = string.Empty;
            ButtonLink = string.Empty;
            BannerImage = string.Empty;
        }
    }
}