using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class NotificationRecordViewModel
    {
        public long Id { get; set; }
        public string NotificationType { get; set; }
        public long FromUserLoginId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class RequestNotificationRecord_VM
    {
        public long Id { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }
        public string NotificationUsersList { get; set; }
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
            if (String.IsNullOrEmpty(NotificationTitle)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(NotificationText)) { sb.Append(Resources.BusinessPanel.NotificationTextRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(NotificationUsersList)) { sb.Append(Resources.BusinessPanel.NotificationRecieptRequired); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvalidMode); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class NotificationRecord_Pagination_VM
    {
        public long Id { get; set; }
        public string NotificationType { get; set; }
        public long FromUserLoginId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string ItemTable { get; set; }
        public long ItemId { get; set; }   
    }

    public class NotificationRecordList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 ToUserLoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class NotificationRecord_Detail_VM
    {
        public long Id { get; set; }
        public string NotificationType { get; set; }
        public long FromUserLoginId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }
        public List<string> NotificationUserIdList { get; set; }
    }


    public class UserNotification_Pagination_VM
    {
        public long Id { get; set; }
        public long NotificationId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public int IsRead { get; set; }
        public long TotalRecords { get; set; }

        public long ItemId { get; set; }
        public string NotificationType { get; set; }
        public string ItemTable { get; set; }
        public int IsNotificationLinkable { get; set; }
    }

    public class RequestNotificationReadUnread_VM
    {
        public long NotificationId { get; set; }
        public int ReadStatus { get; set; }
    }
}