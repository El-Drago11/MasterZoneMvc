using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_BusinessContentEventImages_Param_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long EventId { get; set; }
        public string Image { get; set; }
        public int Mode { get; set; }
        public SP_BusinessContentEventImages_Param_VM()
        {
            
            Image = string.Empty;

        }
    }
}