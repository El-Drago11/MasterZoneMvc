using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class FeatureViewModel
    {
        public long Id { get; set; }
        public string KeyName { get; set; }
        public string TextValue { get; set; }
        public int IsActive { get; set; }
        public int IsLimited { get; set; } // This feature is limit based e.g. Manage Student -> Limited => only 5 
        public long PanelTypeId { get; set; } // SuperAdmin, Business etc.
        public string Comments { get; set; }
    }

}