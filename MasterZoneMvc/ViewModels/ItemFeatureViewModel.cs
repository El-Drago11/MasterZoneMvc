using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ItemFeatureViewModel
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; } // ItemFeatureType ( for MainPlan, Certificate).
        public long FeatureId { get; set; }
        public int IsLimited { get; set; }
        public int Limit { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class RequestItemFeaturePermission_VM
    {
        public long FeatureId { get; set; }
        public int IsLimited { get; set; }
        public int Limit { get; set; }
    }
}