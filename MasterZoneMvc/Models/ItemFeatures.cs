using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Stores Fetaures linked with module like MainPlans(Add On, ..), Certificates, etc. 
    /// </summary>
    public class ItemFeatures
    {
        [Key]
        public long Id { get; set; }
        public long ItemId { get; set; } 
        public string ItemType { get; set; } // ItemFeatureType ( for MainPlan, Certificate).
        public long FeatureId { get; set; }
        public int IsLimited { get; set; }
        public int Limit { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}