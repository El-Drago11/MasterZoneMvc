using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateInstructorAboutMasterProfile_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateInstructorAboutMasterProfile_Param_VM()
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            Description = string.Empty;
            Image = String.Empty;
            ButtonText = String.Empty;
            ButtonLink = String.Empty;
        }
    }
}