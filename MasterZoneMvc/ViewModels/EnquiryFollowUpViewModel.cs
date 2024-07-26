using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EnquiryFollowUpViewModel
    {
        public long Id { get; set; }
        //public long EnquiryId { get; set; }
       // public long Followedby { get; set; }
        public string Comments { get; set; }
        public int Mode { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }
            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrEmpty(Comments)) { sb.Append(Resources.BusinessPanel.Enquiry_FollowUpCommentRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }

    }
    public class EnquiryFollowUp_Pagination
    {
        public long Id { get; set; }
        public string BusinessName { get; set; }
        public string StaffName { get; set; }
        public string StartFromDate { get; set; }
        public string StartFromDate_DateTimeFormat { get; set; }
        public int EnquiryStatus { get; set; }
        public string FollowUpDate { get; set; }
        public string FollowUpDate_DateTimeFormat { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }

    public class Enquiry_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}