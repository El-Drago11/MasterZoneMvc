using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class DanceRatingReview_VM
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string StudentName { get; set; }
        public string StudentProfileImageWithPath { get; set; }
        public string ReviewBody { get; set; }
        public string ReviewImageWith { get; set; }
        public int Status { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRating { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
    }
}