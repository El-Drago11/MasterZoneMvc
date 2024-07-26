using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    /// <summary>
    /// Features Limit assigned to user due to Certification or package purchase or default login.
    /// </summary>
    public class UserLoginFeatures
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long FeatureId { get; set; }
        public int IsLimited { get; set; }
        public int Limit { get; set; }
        public int AddOnLimit { get; set; }
        public int IsActive { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}