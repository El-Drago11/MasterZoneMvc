using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class TransferPackageService
    {

        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public TransferPackageService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get All Transfer-Package List By Business Owner with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Transfer Package List</returns>
        public JqueryDataTable_Pagination_Response_VM<PackageTransferPagintation_VM> GetAllTransferPackageListByBusinessOwner_Pagination(NameValueCollection httpRequestParams, TransferPackage_Pagination_SQL_Params_VM _Params_VM)
        {

            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";

            long recordsTotal = 0;

            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<PackageTransferPagintation_VM> lstTransferPackageRecords = new List<PackageTransferPagintation_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstTransferPackageRecords = db.Database.SqlQuery<PackageTransferPagintation_VM>("exec sp_TransferPackage_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstTransferPackageRecords.Count > 0 ? lstTransferPackageRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<PackageTransferPagintation_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<PackageTransferPagintation_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstTransferPackageRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get All Transfer-Package List By Student-User with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Transfer-Package List</returns>
        public JqueryDataTable_Pagination_Response_VM<NotificationDetailForStudent_Pagination_VM> GetAllTransferPackageListByStudentUser_Pagination(NameValueCollection httpRequestParams, NotificationDetail_Pagination_SQL_Params_VM _Params_VM)
        {

            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";

            long recordsTotal = 0;

            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<NotificationDetailForStudent_Pagination_VM> lstNotificationlstRecords = new List<NotificationDetailForStudent_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstNotificationlstRecords = db.Database.SqlQuery<NotificationDetailForStudent_Pagination_VM>("exec sp_ManageTransferPackageDetailForUser_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstNotificationlstRecords.Count > 0 ? lstNotificationlstRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<NotificationDetailForStudent_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<NotificationDetailForStudent_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstNotificationlstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get User Transfer-Package Request List Having Transfer-Status 0 or 1 (Pending or accepted)
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<TransferStudentPackage_VM> GetAllPendingOrAcceptedStudentTransferPackageRequestList(long userLoginId)
        {
            SqlParameter[] queryParamsRequestTransfer = new SqlParameter[] {
                            new SqlParameter("id", ""),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerLoginId","0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("mode", "7")
                            };

            var respRequestNotification = db.Database.SqlQuery<TransferStudentPackage_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerLoginId,@userLoginId,@mode", queryParamsRequestTransfer).ToList();
            return respRequestNotification;
        }

        /// <summary>
        /// Get User Transfer-Package Request List Having Transfer-Status 0 (Pending)
        /// </summary>
        /// <param name="userLoginId">Student-User-Login-Id</param>
        /// <returns>List of Requests pending for </returns>
        public List<TransferStudentPackage_VM> GetAllPendingStudentTransferPackageRequestList(long userLoginId)
        {
            SqlParameter[] queryParamsRequestTransfer = new SqlParameter[] {
                            new SqlParameter("id", ""),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerLoginId","0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("mode", "8")
                            };

            var respRequestNotification = db.Database.SqlQuery<TransferStudentPackage_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerLoginId,@userLoginId,@mode", queryParamsRequestTransfer).ToList();
            return respRequestNotification;
        }

        /// <summary>
        /// Get Transfer Package Detail By Id
        /// </summary>
        /// <param name="id">Transfer-Package-Id</param>
        public PackageViewDetail_VM GetTransferPackageDetail(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4"),
                        };

            return db.Database.SqlQuery<PackageViewDetail_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).FirstOrDefault();
        }
    }
}