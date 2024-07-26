using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateClassBooking_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ClassId { get; set; }
        public long UserLoginId { get; set; }
        public string ClassQRCode { get; set; }
        public string ClassStartDate { get; set; }
        public string ClassEndDate { get; set; }
        public int Mode { get; set; }
        public long BatchId { get; set; }

        public SP_InsertUpdateClassBooking_Params_VM()
        {
            Id = 0;
            OrderId = 0;
            ClassId = 0;
            UserLoginId = 0;
            ClassQRCode = "";
            ClassStartDate = "";
            ClassEndDate = "";
            Mode = 0;
        }
    }
}