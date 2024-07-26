using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Expense
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; } //Staff-UserLoginId       
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string ExpenseDate { get; set; }
        public DateTime ExpenseDate_DateTimeFormat { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; } // 0= Pending, 1= Accepted, 2 = Rejected 

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; } //LoginUserId       
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}