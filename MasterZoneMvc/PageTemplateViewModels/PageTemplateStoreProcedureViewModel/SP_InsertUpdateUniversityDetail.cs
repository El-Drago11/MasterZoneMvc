using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateUniversityDetail
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string UniversityName { get; set; }
        public string Qualification { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UniversityLogo { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateUniversityDetail ()
        {
            Qualification = string.Empty;
            UniversityName = string.Empty;
            StartDate = string.Empty;
            EndDate = string.Empty;
            UniversityLogo = string.Empty;
        }
    }
}