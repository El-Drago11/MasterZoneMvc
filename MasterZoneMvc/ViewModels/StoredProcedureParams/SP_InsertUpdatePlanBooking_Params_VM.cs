using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdatePlanBooking_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PlanId { get; set; }
        public long StudentUserLoginId { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public int Mode { get; set; }
        public int PlanBookingType { get; set; }
        public int Repeat_Purchase { get; set; }

        public SP_InsertUpdatePlanBooking_Params_VM()
        {
            Id = 0;
            OrderId = 0;
            PlanId = 0;
            StudentUserLoginId = 0;
            PlanStartDate = "";
            PlanEndDate = "";
            Mode = 0;
            PlanBookingType = 0;
            Repeat_Purchase = 0;
        }
    }
}