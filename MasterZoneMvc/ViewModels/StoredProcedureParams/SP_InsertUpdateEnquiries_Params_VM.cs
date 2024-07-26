using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateEnquiries_Params_VM
    {
        public long Id { get; set; }

        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }

        public string Activity { get; set; }
        public long LevelId { get; set; }
        public long BusinessPlanId { get; set; }
        public long ClassId { get; set; }
        public string StartFromDate { get; set; }
        public string Status { get; set; }

        public long StaffId { get; set; }
        public string FollowUpDate { get; set; }
        public string Notes { get; set; }

        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateEnquiries_Params_VM()
        {
            Name = String.Empty;
            Gender = String.Empty;
            Email = String.Empty;
            DOB = String.Empty;
            PhoneNumber = String.Empty;
            AlternatePhoneNumber = String.Empty;
            Address = String.Empty;
            Activity = String.Empty;
            StartFromDate = String.Empty;
            Status = String.Empty;
            FollowUpDate = String.Empty;
            Notes = String.Empty;

        }
    }
}