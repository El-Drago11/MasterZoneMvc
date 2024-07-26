using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class QueryService
    {
        private MasterZoneDbContext db;

        public QueryService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        
        public JqueryDataTable_Pagination_Response_VM<Query_Pagination_VM> GetBusinessQueriesList_Pagination(NameValueCollection httpRequestParams, ManageQuery_Pagination_SQL_Params_VM _Params_VM)
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
            string[] querySortColumn = {
                            "CreatedOn",
                            "StudentName",
                            "Title",
                            "Description", 
                            "CreatedOn"
                        };

            _Params_VM.JqueryDataTableParams.sortColumn = querySortColumn[_Params_VM.JqueryDataTableParams.sortColumnIndex];

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";

            List<Query_Pagination_VM> lstPaginationRecords = new List<Query_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessOwnerLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstPaginationRecords = db.Database.SqlQuery<Query_Pagination_VM>("exec sp_ManageQuery_Pagination @id,@businessOwnerLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstPaginationRecords.Count > 0 ? lstPaginationRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Query_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Query_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstPaginationRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

    }
}