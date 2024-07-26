using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class StaffAttendance
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long StaffId { get; set; }
        public int AttendanceStatus { get; set; } // Present(1), Absent(2), OnLeave(3)
        public string AttendanceDate { get; set; }
        public DateTime AttendanceDate_DateTimeFormat { get; set; }
        public int AttendanceMonth { get; set; }
        public int AttendanceYear { get; set; }
        public string LeaveReason { get; set; }
        public string InTime_24HF { get; set; }
        public string OutTime_24HF { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpadatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public int IsApproved { get; set; }
    }
}