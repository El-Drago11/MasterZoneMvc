using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateCustomForm_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long CustomFormElementId { get; set; }
        public string CustomFormElementOptions { get; set; }
        public string CustomFormName { get; set; }
        public int Status { get; set; }
        public long CustomFormId { get; set; }
        public string CustomFormElementName { get; set; }
        public string CustomFormElementType { get; set; }
        public string CustomFormElementValue { get; set; }
        public string CustomFormElementPlaceholder { get; set; }
        public int CustomFormElementStatus { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateCustomForm_Param_VM()
        {
            CustomFormName = string.Empty; Status = 0; SubmittedByLoginId = 0;
            CustomFormElementName = string.Empty;
            CustomFormElementOptions =string.Empty;
            CustomFormElementType = string.Empty;
            CustomFormElementValue = string.Empty;
            CustomFormElementPlaceholder = string.Empty;
            CustomFormElementStatus = 0;
        }
    }
}