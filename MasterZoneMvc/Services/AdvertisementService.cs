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
    public class AdvertisementService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public AdvertisementService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Advertisements List by Pagination For Business or Super-Admin based on the passed Mode.
        /// </summary>
        /// <param name="httpRequestParams">FormBody Parameters</param>
        /// <param name="_Params_VM">Parameters needed in Pagination Stored-Procedure</param>
        /// <returns>Advertisements List as Jquery Datatable Response structure</returns>
        public JqueryDataTable_Pagination_Response_VM<Advertisement_Pagination_VM> GetAdvertisementsList_Pagination(NameValueCollection httpRequestParams, AdvertisementList_Pagination_SQL_Params_VM _Params_VM)
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

            //// optimized code
            //string[] sortColumn = {
            //                "CreatedOn",
            //                "AdvertisementCategory",
            //                "ImageOrientationType",
            //                "CreatedOn"
            //            };

            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns["+ _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"]  : "CreatedOn";
            //_Params_VM.JqueryDataTableParams.sortColumn = sortColumn[_Params_VM.JqueryDataTableParams.sortColumnIndex];

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";

            List<Advertisement_Pagination_VM> lstRecords = new List<Advertisement_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("createdForLoginId", _Params_VM.CreatedForLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstRecords = db.Database.SqlQuery<Advertisement_Pagination_VM>("exec sp_ManageAdvertisement_Pagination @id,@userLoginId,@createdForLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Advertisement_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Advertisement_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Advertisement banner
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns the Table-Row data</returns>
        public Advertisement_VM GetAdvertisementBannerDetail()
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageAdvertisement_Get<Advertisement_VM>(new SP_ManageAdvertisement_Params_VM()
            {
                Mode = 5
            });
        }

        public Advertisement_VM GetAdvertisementDetail(long Id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", Id),
                             new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("UserLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

            var resp = db.Database.SqlQuery<Advertisement_VM>("exec sp_ManageAdvertisement @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Get Advertisement banner
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns the Table-Row data</returns>
        public List<Advertisement_VM> GetAdvertisementDetailslider()
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageAdvertisement_GetAll<Advertisement_VM>(new SP_ManageAdvertisement_Params_VM()
            {
                Mode = 6
            });
        }

    }
}