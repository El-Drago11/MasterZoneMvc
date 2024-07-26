using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SP_ManageEventMenu_Params_VM
    {
        public string MenuTag { get; set; }
        public string CategoryKey { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public string ItemType { get; set; }
        public string City { get; set; }
        public int Mode { get; set; }

    }
}