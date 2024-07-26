using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Coupon
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
        public int IsFixedAmount { get; set; } // Fixed = 1 , Percentage = 0
        public decimal DiscountValue { get; set; } // Fixed: 1000.00 , Percentage: 15.00
                                                   //  public int MaxPerUser { get; set; }
        public int TotalUsed { get; set; }
        public int DiscountFor { get; set; }// All Student = 1, Selected Student = 2
        public string SelectedStudent { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}