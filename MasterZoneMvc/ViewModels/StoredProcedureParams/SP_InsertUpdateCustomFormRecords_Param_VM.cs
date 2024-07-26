using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateCustomFormRecords_Param_VM
    {
        public long Id { get; set; }
        public long FormId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApplicantUserLoginId { get; set; } 
        public long FormElementId { get; set; } 
        public string FormElementName { get; set; }
        public string FormElementValue { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateCustomFormRecords_Param_VM ()
        {
            FormElementName = string.Empty;
            FormElementValue = string.Empty;
        }
    }

}