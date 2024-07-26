using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdatePauseClassRequest_Param_VM
    {
        public long Id { get; set; }
        public long ClassBookingId { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string PauseStartDate { get; set; }
        public string PauseEndDate { get; set; }
        public int PauseDays { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public  long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public string BusinessReply { get; set; }

        public SP_InsertUpdatePauseClassRequest_Param_VM()
        {
            PauseStartDate = string.Empty;
            PauseEndDate = string.Empty;
            Reason = string.Empty;
            Status = 0;
            SubmittedByLoginId = 0;
            Mode = 0;
            BusinessReply = string.Empty;

        }
    }
}