using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateCourseBooking_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long CourseId { get; set; }
        public long UserLoginId { get; set; }

        public string CourseStartDate { get; set; }
        public string CourseEndDate { get; set; }
        public int Mode { get; set; }


        public SP_InsertUpdateCourseBooking_Params_VM()
        {
            Id = 0;
            OrderId = 0;
            CourseId = 0;
            UserLoginId = 0;
            CourseStartDate = "";
            CourseEndDate = "";
            Mode = 0;
        }
    }
}