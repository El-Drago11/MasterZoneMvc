using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessTiming
    {

        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string DayName { get; set; }
        public int DayValue { get; set; }
        public int IsOpened { get; set; }
        public string OpeningTime_12HoursFormat { get; set; }
        public string OpeningTime_24HoursFormat { get; set; }
        public string ClosingTime_12HoursFormat { get; set; }
        public string ClosingTime_24HoursFormat { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }


        public string OpeningTime2_12HoursFormat { get; set; }
        public string OpeningTime2_24HoursFormat { get; set; }
        public string ClosingTime2_12HoursFormat { get; set; }
        public string ClosingTime2_24HoursFormat { get; set; }

        public string TodayOff { get; set; }
        public string Notes { get; set; }


    }
}