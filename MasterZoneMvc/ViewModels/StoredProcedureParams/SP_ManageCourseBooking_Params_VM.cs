using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageCourseBooking_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long StudentUserLoginId { get; set; }
        public long CourseId { get; set; }
        public int Mode { get; set; }
        public string JoiningDate { get; set; }

        public SP_ManageCourseBooking_Params_VM()
        {
            JoiningDate = string.Empty;
        }
    }
}