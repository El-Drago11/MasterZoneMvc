using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Staff
    {
        [Key]
        public long Id { get; set; }

        public long UserLoginId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public long StaffCategoryId { get; set; }

        //public decimal MonthlySalary { get; set; }

        public string ProfileImage { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public string MasterId { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal DearnessAllowance { get; set; }
        public string Remarks { get; set; }
        public int IsProfessional { get; set; }
        public string Designation { get; set; }

    }
}