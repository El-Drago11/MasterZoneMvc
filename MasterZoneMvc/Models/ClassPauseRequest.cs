using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ClassPauseRequest
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ClassBookingId { get; set; }
        public string PauseStartDate { get; set; }
        public string PauseEndDate { get; set; }
        public int PauseDays { get; set; }
       
        public string Reason { get; set; }
        public int Status { get; set; } // 1 = pending, 2= Approved , 3 = Rejected
        public string BusinessReply { get; set; }

        // Updated--- 
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

    }
}