using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessExploreDClassicDanceDetail_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessExploreDClassicDanceDetail_Param_VM()
        {
            Title = string.Empty;
            Description = string.Empty;
        }
    }

    public class SP_ManageBusinessExploreClassicDanceDetail_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string ExploreType { get; set; }
        public int Mode { get; set; }
        public SP_ManageBusinessExploreClassicDanceDetail_Param_VM()
        {
            ExploreType = string.Empty;
        }
    }
}