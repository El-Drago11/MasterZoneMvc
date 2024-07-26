using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class MainPlan
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string PlanDurationTypeKey { get; set; }
        public decimal Price { get; set; }
        public decimal CompareAtPrice { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public string PlanImage { get; set; }
        public int Status { get; set; }
        public int PlanType { get; set; }
        public string PlanPermission { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public int ShowOnHomePage { get; set; }
    }
}