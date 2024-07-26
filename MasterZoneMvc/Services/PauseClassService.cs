using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class PauseClassService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public PauseClassService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// To Show View Pagination For Business Owner To get All Pause class Request Detail
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<ClassPauseRequest_Pagination> GetPauseClassList_Pagination(NameValueCollection httpRequestParams, ClassPauseRequest_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<ClassPauseRequest_Pagination> lstRecords = new List<ClassPauseRequest_Pagination>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<ClassPauseRequest_Pagination>("exec sp_ManageBusinessClassPauseRequest_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<ClassPauseRequest_Pagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<ClassPauseRequest_Pagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// To Get Student Pause Class Detail View Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<StudentPauseClassRequestDetail_Pagination> GetStudentPauseClassRequestViewList_Pagination(NameValueCollection httpRequestParams, ClassPauseRequest_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<StudentPauseClassRequestDetail_Pagination> lstRecords = new List<StudentPauseClassRequestDetail_Pagination>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<StudentPauseClassRequestDetail_Pagination>("exec sp_ManageViewPauseClassRequest_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<StudentPauseClassRequestDetail_Pagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentPauseClassRequestDetail_Pagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }

       /// <summary>
       ///  To Delete ClassPauseRequest Detail 
       /// </summary>
       /// <param name="Id"></param>
       /// <returns></returns>
        public SPResponseViewModel DeletePauseClassDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdatePauseClassRequest_Get<SPResponseViewModel>(new SP_InsertUpdatePauseClassRequest_Param_VM()
            {
                Id = Id,
                Mode = 4
            });
        }
        /// <summary>
        /// To Get Class Pause Detail by Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ClassPauseRequestDetail_ViewModel GetClassPauseRequestDetailById(long Id)
        {
            return storedPorcedureRepository.SP_ManagePauseClassRequest<ClassPauseRequestDetail_ViewModel>(new SP_ManageClassPauseRequestDetail_Param_VM()
            {
                Id = Id,
                Mode = 1
            });
        }
    }
}