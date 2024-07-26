using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateClassicDanceVideo_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Note { get; set; }
        public string VideoImage { get; set; }
        public string VideoLink { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText1 { get; set; }
        public string ButtonLink1 { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateClassicDanceVideo_Param_VM ()
        {
            Title = String.Empty;
            SubTitle = string.Empty;
            Note = String.Empty;
            VideoImage = String.Empty;
            ButtonText = String.Empty;
            ButtonLink = String.Empty;
            VideoLink = String.Empty;
            ButtonLink1 = String.Empty;
            ButtonText1 = string.Empty;
        }
    }
}