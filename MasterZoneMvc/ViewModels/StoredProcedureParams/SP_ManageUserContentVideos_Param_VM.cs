﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageUserContentVideos_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public int Mode { get; set; }
    }
}