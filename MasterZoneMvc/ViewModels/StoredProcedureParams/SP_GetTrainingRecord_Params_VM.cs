using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_GetTrainingRecord_Params_VM
    {
        public long Id { get; set; }
        public string City { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public int Mode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public SP_GetTrainingRecord_Params_VM()
        {
            City = string.Empty;
        }
    }

    public class SP_GetAllTrainingDetailSearch_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Location { get; set; }
        public string TrainingType { get; set; }
        public string Searchkeyword { get; set; }
        public string SearchBy { get; set; }
        public int Mode { get; set; }

        public SP_GetAllTrainingDetailSearch_Params_VM()
        {

            Searchkeyword = string.Empty;
            SearchBy = string.Empty;
            Location = string.Empty;
            TrainingType = string.Empty;
        }
    }
}