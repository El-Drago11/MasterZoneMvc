using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MainPlanBooking_VM
    {

        public long Id { get; set; }
        public long MainPlanId { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string PlanDurationTypeKey { get; set; }
        public decimal Price { get; set; }
        public decimal CompareAtPrice { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
    }
}