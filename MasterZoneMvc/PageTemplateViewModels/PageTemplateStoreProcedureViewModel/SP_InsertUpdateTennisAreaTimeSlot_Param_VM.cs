using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateTennisAreaTimeSlot_Param_VM
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public long UserLoginId { get; set; }
        public string Time { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateTennisAreaTimeSlot_Param_VM()
        {
            Time = string.Empty;
        }
    }
}