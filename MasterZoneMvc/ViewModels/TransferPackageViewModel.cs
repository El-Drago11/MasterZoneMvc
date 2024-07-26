using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Web;
using System.Windows;

namespace MasterZoneMvc.ViewModels
{
    public class TransferPackageViewModel
    {
    }

    public class TransferPackageStudentList_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string StudentName { get; set; }
        public string BusinessName { get; set; }
        public string StudentProfileImageWithPath { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public string MasterId { get; set; }

    }


    public class TransferPackagePlanBooking_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long PlanId { get; set; }
        public string PlanName { get; set; }
        public string BusinessName { get; set; }

        public decimal PlanPrice { get; set; }
    }
    public class TransferStudentPackage_VM
    {
        public long Id { get; set; }
        public long TransferStudent { get; set; }
        public int TransferType { get; set; }
        public string TransferReason { get; set; }
        public string TransferStartDate { get; set; }
        public string TransferCity { get; set; }
        public string TransferState { get; set; }
        public long PlanId { get; set; }
        public long PlanBookingId { get; set; }
        public long TransferBusinessUserLoginId { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (TransferType <= 0)
            {
                sb.Append(Resources.VisitorPanel.TransferTypeRequired); vm.Valid = false;
            }
            if (TransferType == 2)
            {
                if (String.IsNullOrEmpty(TransferCity)) { sb.Append(Resources.VisitorPanel.TransferCityRequired); vm.Valid = false; }
                if (String.IsNullOrEmpty(TransferState)) { sb.Append(Resources.VisitorPanel.TransferStateRequired); vm.Valid = false; }
                if (TransferBusinessUserLoginId <= 0)
                {
                    sb.Append(Resources.VisitorPanel.TransferBusinessRequired); vm.Valid = false;

                }
            }
            if (TransferType == 1)
            {
                if (TransferStudent <= 0)
                {
                    sb.Append(Resources.VisitorPanel.TransferStudentRequired); vm.Valid = false;

                }
            }

            if (PlanId <= 0)
            {
                sb.Append(Resources.VisitorPanel.TransferPlanPackageRequired); vm.Valid = false;

            }

            if (String.IsNullOrEmpty(TransferReason)) { sb.Append(Resources.VisitorPanel.TransferReasonRequired); vm.Valid = false; }
            if (String.IsNullOrEmpty(TransferStartDate)) { sb.Append(Resources.VisitorPanel.TransferStartDateRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class TransferPackageBusiness_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessName { get; set; }
    }

    public class PackageTransferPagintation_VM
    {
        public long Id { get; set; }
        public string TransferFromStudentName { get; set; }
        public string TransferToStudentName { get; set; }     
        public string TrasferFromStudentProfileImageWithPath { get; set; }     
        public string TrasferToStudentProfileImageWithPath { get; set; }     
        public long TransferFromUserLoginId { get; set; }     
        public long TransferToUserLoginId { get; set; }     
        public string TrasferToBusinessName { get; set; }
        public string TransferToBusinessLogoWithPath { get; set; }     
        public string PlanName { get; set; }
        public string TransferDate { get; set; }
        public int TransferStatus { get; set; }
        public int TransferType { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public long PlanBookingId { get; set; }
    }

    public class TransferPackage_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
    public class PackageViewDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long PlanBookingId { get; set; }
        public string TransferFromStudentName { get; set; }
        public string TransferToStudentName { get; set; }
        public string BusinessName { get; set; }
        public string PlanName { get; set; }
        public string TransferDate { get; set; }
        public int TransferStatus { get; set; }
        public int TransferType { get; set; }
        public string RejectionReason {get; set;}
        public string TransferReason { get; set; }
        public string TrasferFromStudentProfileImageWithPath { get; set; }
        public string TrasferToStudentProfileImageWithPath { get; set; }
        public long TransferFromUserLoginId { get; set; }
        public long TransferToUserLoginId { get; set; }
        public string TrasferToBusinessName { get; set; }
        public string TransferToBusinessLogoWithPath { get; set; }
    }

    public class PackageStatus_VM
    {
        public long Id { get; set; }
        public int TransferStatus { get; set; }
        public string RejectionReason { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (TransferStatus <= 0)
            {
                sb.Append(Resources.BusinessPanel.TransferStatusRequired); vm.Valid = false;
            }

            if(TransferStatus == 2)
            {
                if (String.IsNullOrEmpty(RejectionReason)) { sb.Append(Resources.BusinessPanel.RejectionReasonRequired); vm.Valid = false; }
            }
           
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }
}