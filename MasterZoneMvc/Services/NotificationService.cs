using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class NotificationService
    {
        private MasterZoneDbContext db;

        public NotificationService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        public SPResponseViewModel InsertUpdateNotification(SPInsertUpdateNotification_Params_VM notification_Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", notification_Params_VM.Id),
                            new SqlParameter("fromUserLoginId", notification_Params_VM.FromUserLoginId),
                            new SqlParameter("notificationType", notification_Params_VM.NotificationType),
                            new SqlParameter("notificationUsersList", notification_Params_VM.NotificationUsersList),
                            new SqlParameter("notificationTitle", notification_Params_VM.NotificationTitle),
                            new SqlParameter("notificationText", notification_Params_VM.NotificationText),
                            new SqlParameter("submittedByLoginId", notification_Params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", notification_Params_VM.Mode),
                            new SqlParameter("itemId", notification_Params_VM.ItemId),
                            new SqlParameter("itemTable", notification_Params_VM.ItemTable),
                            new SqlParameter("isNotificationLinkable", notification_Params_VM.IsNotificationLinkable),
                            new SqlParameter("OrderId",notification_Params_VM.OrderId)
                            };

            var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateNotification @id,@fromUserLoginId,@notificationType,@notificationUsersList,@notificationTitle,@notificationText,@submittedByLoginId,@mode,@itemId,@itemTable,@isNotificationLinkable,@OrderId", queryParams).FirstOrDefault();

            return resp;
        }

        public JqueryDataTable_Pagination_Response_VM<NotificationRecord_Pagination_VM> GetNotificationsList_Pagination(NameValueCollection httpRequestParams, NotificationRecordList_Pagination_SQL_Params_VM _Params_VM)
        {

            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";
            //_Params_VM.GetExpenditure_Params_VM = JsonConvert.DeserializeObject<GetExpenditure_Params_VM>(HttpContext.Current.Request.Params["_Params"]);

            long recordsTotal = 0;

            // optimized code
            string[] expenditureSortColumn = {
                            "CreatedOn",
                            "NotificationTitle",
                            "NotificationText",
                            "CreatedOn"
                        };

            _Params_VM.JqueryDataTableParams.sortColumn = expenditureSortColumn[_Params_VM.JqueryDataTableParams.sortColumnIndex];

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";

            
            List<NotificationRecord_Pagination_VM> lstNotificationRecords = new List<NotificationRecord_Pagination_VM>();

            //IExpenditureRepository expenditureRepository = new ExpenditureRepository(db);
            //lstExpenditures = expenditureRepository.GetExpendituresList_Pagination(_Params_VM);

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("fromUserLoginId", _Params_VM.LoginId),
                            new SqlParameter("toUserLoginId", _Params_VM.ToUserLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstNotificationRecords = db.Database.SqlQuery<NotificationRecord_Pagination_VM>("exec sp_ManageNotification_Pagination @id,@fromUserLoginId,@toUserLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();


            recordsTotal = lstNotificationRecords.Count > 0 ? lstNotificationRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<NotificationRecord_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<NotificationRecord_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstNotificationRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        //public void GetNotificationRecordDetail(long NotificationRecordId, long FromUserLoginId)
        //{

        //}
    }
}