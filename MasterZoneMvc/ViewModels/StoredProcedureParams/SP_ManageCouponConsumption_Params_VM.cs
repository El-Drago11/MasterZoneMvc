using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageCouponConsumption_Params_VM
    {
        public long Id { get; set; }
        public long ConsumerUserLoginId { get; set; }
        public string CouponCode { get; set; }
        public int Mode { get; set; }

        public SP_ManageCouponConsumption_Params_VM()
        {
            CouponCode = "";
        }
    }
}