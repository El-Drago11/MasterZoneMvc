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
    public class SubAdminService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public SubAdminService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// To Get SubAdmin Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SubAdminEdit_VM GetSubAdminDetailById(long id)
        {
            SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<SubAdminEdit_VM>("exec sp_ManageSubAdmin @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();
            return resp;
        }

        public JqueryDataTable_Pagination_Response_VM<SubAdmin_Pagination_VM> GetSubAdminList_Pagination(NameValueCollection httpRequestParams, SubAdmin_Pagination_SQL_Params_VM _Params_VM)
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

            List<SubAdmin_Pagination_VM> lstSubAdminRecords = new List<SubAdmin_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstSubAdminRecords = db.Database.SqlQuery<SubAdmin_Pagination_VM>("exec sp_ManageSubAdmin_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstSubAdminRecords.Count > 0 ? lstSubAdminRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<SubAdmin_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<SubAdmin_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstSubAdminRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// To Get SubAdmin Profile Detail By UserLoginId 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public SubAdminProfileDetail_VM GetSubAdminDetail(long UserLoginId)
        {
            SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", UserLoginId),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<SubAdminProfileDetail_VM>("exec sp_ManageSubAdminProfile @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();
            return resp;
        }
    }
}