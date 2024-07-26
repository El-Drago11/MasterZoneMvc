using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentStudioEquipment_Param_VM
    {
        public long Id  { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string EquipmentType { get; set; } = "0";
        public string EquipmentValue { get; set; } = "0";
        public int Mode { get; set; }
    }
    public class SP_InsertUpdateBusinessContentStudioEquipmentPPCMeta_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Mode { get; set; }
    }

}