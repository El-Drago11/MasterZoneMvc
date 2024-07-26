using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class TransferNotificationViewModel
    {
        

    }
    public class TransferNotification_Pagination_VM
    {
        public long Id { get; set; }
        public string BusinessName { get; set; }
        public string NotificationMessage { get; set; }
        public int Status { get; set; }        
        public string CreatedOn_FormatDate { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
    }

    public class TransferNotification_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
    public class NotificationDetail_Pagination_VM
    {
        public long Id { get; set; }
        public string BusinessName { get; set; }
        public string NotificationMessage { get; set; }
        public int Status { get; set; }
       public string Action { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
    }

    public class NotificationDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }


    public class NotificationDetailForStudent_Pagination_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string TransferFromStudentName { get; set; }
        public string TransferToStudentName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string PlanName { get; set; }
        public string TransferDate { get; set; }
        public int TransferStatus { get; set; }
        public int TransferType { get; set; }

        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public string TrasferFromStudentProfileImageWithPath { get; set; }
        public string TrasferToStudentProfileImageWithPath { get; set; }
        public long TransferFromUserLoginId { get; set; }
        public long TransferToUserLoginId { get; set; }
        public string TrasferToBusinessName { get; set; }
        public string TransferToBusinessLogoWithPath { get; set; }
        public long PlanBookingId { get; set; }
    }


}