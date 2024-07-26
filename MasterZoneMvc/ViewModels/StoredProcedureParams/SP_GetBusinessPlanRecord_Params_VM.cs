using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_GetBusinessPlanRecord_Params_VM
    {
        public long Id { get; set; }
        public string City { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public int Mode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public SP_GetBusinessPlanRecord_Params_VM()
        {
            City = string.Empty;
        }
    }
}