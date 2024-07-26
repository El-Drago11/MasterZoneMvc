using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateUserExperience_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string EndMonth { get; set; }
        public string EndYear { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public  SP_InsertUpdateUserExperience_Param_VM()
        {
            Title = string.Empty;
            CompanyName = string.Empty;
            StartMonth = string.Empty;
            EndMonth = string.Empty;
            StartDate = string.Empty;
            EndDate = string.Empty;
            StartYear = string.Empty;
            EndYear = string.Empty;
            Description = string.Empty;
        }
    }
}