using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_BusinessContentEventCompany_Param_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string EventOptions { get; set; }
        public int Mode { get; set; }
        public SP_BusinessContentEventCompany_Param_VM ()
        {
            Title = string.Empty;
            Description = string.Empty;
            Image = string.Empty;
            EventOptions = string.Empty;

        }
    }
}