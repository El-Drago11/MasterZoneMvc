using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessPlan
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(BusinessOwner))]
        public long BusinessOwnerLoginId { get; set; }
        //public BusinessOwner BusinessOwner { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        //[ForeignKey(nameof(BusinessPlanDurationType))]
        public long BusinessPlanDurationTypeId { get; set; }
        //public BusinessPlanDurationType BusinessPlanDurationType { get; set; }

        public string Description { get; set; }

        public decimal CompareAtPrice { get; set; }

        public int Status { get; set; }
        public int PlanTypeTitle { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public string PlanImage { get; set; }
        public decimal DiscountPercent { get; set; }
        
        public int  PlanType { get; set; }
    }
}