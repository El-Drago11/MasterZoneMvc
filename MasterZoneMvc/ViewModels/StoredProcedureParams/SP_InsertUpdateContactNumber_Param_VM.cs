using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateContactNumber_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Location1 { get; set; }
        public string ContactNumber1 { get; set; }
        public string Location2 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Location3 { get; set; }
        public string ContactNumber3 { get; set; }
        public string Location4 { get; set; }
        public string ContactNumber4 { get; set; }
        public string Location5 { get; set; }
        public string ContactNumber5 { get; set; }
        public string Location6 { get; set; }
        public string ContactNumber6 { get; set; }
        public string Location7 { get; set; }
        public string ContactNumber7 { get; set; }
        public string Location8 { get; set; }
        public string ContactNumber8 { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateContactNumber_Param_VM()
        {

            Location1 = string.Empty;
            ContactNumber1 = string.Empty;
            Location2 = string.Empty;
            ContactNumber2 = string.Empty;
            Location3 = string.Empty;
            ContactNumber3 = string.Empty;
            Location4 = string.Empty;
            ContactNumber4 = string.Empty;
            Location5 = string.Empty;
            ContactNumber5 = string.Empty;
            Location6 = string.Empty;
            ContactNumber7 = string.Empty;
            Location8 = string.Empty;
            ContactNumber8 = string.Empty;
        }

    }
}