using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessFindMasterProfile_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ExploreType { get; set; }
        public string ScheduleLink { get; set; }
        public string Image { get;set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessFindMasterProfile_Param_VM ()
        {
            Title = string.Empty;
            Description = string.Empty;
            ExploreType = string.Empty;
            ScheduleLink = string.Empty;
            Image = string.Empty;
        }
    }
}