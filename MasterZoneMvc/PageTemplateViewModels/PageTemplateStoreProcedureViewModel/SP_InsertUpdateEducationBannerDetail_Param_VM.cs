using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateEducationBannerDetail_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string BannerType { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string BannerImage { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateEducationBannerDetail_Param_VM()
        {
            BannerType = String.Empty;
            Title = String.Empty;
            SubTitle = String.Empty;
            Description = String.Empty;
            BannerImage = String.Empty;
            ButtonText = string.Empty;
            ButtonLink = string.Empty;
        }
    }
}