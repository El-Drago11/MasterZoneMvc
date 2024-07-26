using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateCustomFormElement_Param_VM
    {
        public long Id { get; set; }
        public long CustomFormId { get; set; }
        public string CustomFormElementName { get; set; }
        public string CustomFormElementType { get; set; }
        public string CustomFormElementValue { get; set; }
        public string CustomFormElementPlaceholder { get; set; }
        public int CustomFormElementStatus { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateCustomFormElement_Param_VM()
        {
            CustomFormElementName = string.Empty;
            CustomFormElementType = string.Empty;
            CustomFormElementValue = string.Empty;
            CustomFormElementPlaceholder = string.Empty;
            CustomFormElementStatus = 0;

        }

    }
}