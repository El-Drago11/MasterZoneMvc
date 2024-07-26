﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageCoupon_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public string CouponCode { get; set; }

        public SP_ManageCoupon_Params_VM()
        {
            CouponCode = "";
        }
    }
}