using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateMainPackageBooking_Param_VM
    {

        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PlanId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public int Mode { get; set; }
        public int Status { get; set; }
        

        public SP_InsertUpdateMainPackageBooking_Param_VM()
        {
            Id = 0;
            OrderId = 0;
            PlanId = 0;
            BusinessOwnerLoginId = 0;
            PlanStartDate = "";
            PlanEndDate = "";
            Mode = 0;
        }
    }
}