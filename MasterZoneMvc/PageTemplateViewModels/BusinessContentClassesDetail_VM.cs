using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentClassesDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string BusinessOwnerName { get; set; }
        public string City { get; set; }
        public string ScheduledStartOnTime_24HF  { get; set; }
        public string DayOfWeek { get; set; }
        public string FormattedScheduledOnDateTime { get; set; }
        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ClassMode { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }


    }
}