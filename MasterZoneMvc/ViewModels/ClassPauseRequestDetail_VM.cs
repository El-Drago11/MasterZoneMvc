using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ClassPauseRequestDetail_VM
    {
        public long Id { get; set; }
        public long ClassBookingId { get; set; }
        public  long UserLoginId {get;set;}
        public long BusinessOwnerLoginId {get;set;}
        public string BusinessReply { get;set;}
        public string PauseStartDate { get;set;}
        public string PauseEndDate { get;set;}
        public int PauseDays { get;set;}
        public string Reason { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.ValidInformationMessage;
            }

            StringBuilder sb = new StringBuilder();
            if(Mode ==1)
            {
                if (String.IsNullOrEmpty(PauseStartDate)) { sb.Append(Resources.VisitorPanel.StartDateRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(PauseEndDate)) { sb.Append(Resources.VisitorPanel.EndDateRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(Reason)) { sb.Append(Resources.VisitorPanel.ReasonRequired); vm.Valid = false; }
            }
            

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }


    public class ClassPauseRequest_Pagination
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ClassBookingId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string StudentName { get; set; }
        public long StudentUserLoginId { get; set; }
        public string StudentProfileImageWithPath { get; set; }
        public string MasterId { get; set; }
        public string ClassName { get; set; }
        public string PauseStartDate { get; set; }
        public string PauseEndDate { get; set; }
        public int PauseDays { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }

    }

    public class ClassPauseRequest_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class ClassBookingDate_VM
    {
        public long ClassPauseRequestId { get; set; }
        public int PauseDays { get; set; }
        public long ClassBookingId { get; set; }
    }


    public class StudentPauseClassRequestDetail_Pagination
    {
        public long Id { get; set; }
        public long ClassBookingId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string ClassName { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string PauseStartDate { get; set; }
        public string PauseEndDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BusinessReply { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }


    }


    public class ClassPauseRequestDetail_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ClassBookingId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessReply { get; set; }
        public string PauseStartDate { get; set; }
        public string PauseEndDate { get; set; }
        public int PauseDays { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
    }
}