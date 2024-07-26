using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateInstructorOtherInformation
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateInstructorOtherInformation()
        {
            Title = string.Empty;
            Description = string.Empty;

        }
    }
}