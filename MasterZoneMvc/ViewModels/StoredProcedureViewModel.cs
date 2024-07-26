using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StoredProcedureViewModel
    {
    }

    public class SPLogin_Params_VM {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Mode { get; set; }

        public SPLogin_Params_VM()
        {
            Email = "";
            Password = "";
        }
    }


    public class SPInsertUpdateNotification_Params_VM
    {
        public long Id { get; set; }
        public string NotificationType { get; set; }
        public long FromUserLoginId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationText { get; set; }
        public string NotificationUsersList { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public long ItemId { get; set; } // Tables Primary Key
        public string ItemTable { get; set; } // Table Name
        public int IsNotificationLinkable { get; set; } // Is notification linkable

        public SPInsertUpdateNotification_Params_VM()
        {
            Id = -1;
            FromUserLoginId = -1;
            NotificationType = "";
            NotificationTitle = "";
            NotificationText = "";
            NotificationUsersList = "";
            Mode = -1;
            ItemTable = string.Empty;
        }
    }

    public class ManageEnquiry_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

}