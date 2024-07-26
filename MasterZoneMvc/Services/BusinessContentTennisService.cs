using MasterZoneMvc.DAL;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.ViewModels;
using System.Collections.Specialized;
using System.Data.SqlClient;

namespace MasterZoneMvc.Services
{
    public class BusinessContentTennisService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessContentTennisService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Business Content Tennis Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Tennis Detail</returns>
        public List<BusinessContentTennisDetail_VM> GetBusinessContentTennisDetail_List(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentTennisDetail_Get<BusinessContentTennisDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        /// <summary>
        /// Business Content Tennis Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Tennis Detail</returns>
        public BusinessContentTennisDetail_VM GetBusinessContentTennisDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentTennis_Get<BusinessContentTennisDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        /// <summary>
        /// Business Content Tennis Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Tennis Detail</returns>
        public BusinessContentTennisDetail_VM GetBusinessContentTennisDetail_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentTennis_Get<BusinessContentTennisDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM
            {

                Id = Id,
                Mode = 1


            });

        }


        /// <summary>
        /// To View Tennis Detail
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<TennisDetail_Pagination_VM> GetBusinessContentTennisList_Pagination(NameValueCollection httpRequestParams, TennisDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<TennisDetail_Pagination_VM> lstTennisRecords = new List<TennisDetail_Pagination_VM>();

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

            lstTennisRecords = db.Database.SqlQuery<TennisDetail_Pagination_VM>("exec sp_ManageBusinessContentTennis_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstTennisRecords.Count > 0 ? lstTennisRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<TennisDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<TennisDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstTennisRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Business Content Tennis Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Tennis Detail</returns>
        public SPResponseViewModel DeleteTennisDetail(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentTennis_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentTennis_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }
        /// <summary>
        /// to get business timing list
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<TennisAreaTimeSlot_VM> GetBusinessTennisTiming_List(long BusinessOwnerLoginId,long slotId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessTennisTiming_GetAll<TennisAreaTimeSlot_VM>(new SP_ManageTennisTiming_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                slotId = slotId,
                Mode = 1


            });

        }


        /// <summary>
        /// to get tennis  timing  only list
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<TennisAreaTimeSlot_VM> GetTennisTimingList(long BusinessOwnerLoginId, long slotId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessTennisTiming_GetAll<TennisAreaTimeSlot_VM>(new SP_ManageTennisTiming_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                slotId = slotId,
                Mode = 2


            });

        }

        /// <summary>
        /// to get tennis  timing  only list
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<TennisAreaTimeSlot_VM> GetTennisTimingListForBusiness(long BusinessOwnerLoginId, long slotId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessTennisTiming_GetAll<TennisAreaTimeSlot_VM>(new SP_ManageTennisTiming_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                slotId = slotId,
                Mode = 3


            });

        }
        /// <summary>
        /// to get room details for edit (masterProfile-room)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
         public BusinessContentTennisDetail_VM GetBusinessMasterProfileRoomDetail_ById(long Id)
         {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageMasterProfileRoomDetail_GetbyId<BusinessContentTennisDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM
            {
                Id = Id,
                Mode = 2

            });

         }

        /// <summary>
        /// To View Tennis Detail
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<TennisDetail_Pagination_VM> GetMasterProfileRoomDetailsList_Pagination(NameValueCollection httpRequestParams, TennisDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<TennisDetail_Pagination_VM> lstTennisRecords = new List<TennisDetail_Pagination_VM>();

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

            lstTennisRecords = db.Database.SqlQuery<TennisDetail_Pagination_VM>("exec sp_ManageMasterProfileRoom_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstTennisRecords.Count > 0 ? lstTennisRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<TennisDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<TennisDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstTennisRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Business Content Tennis Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Tennis Detail</returns>
        public SPResponseViewModel DeleteRoomById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateMasterProfileRoomDetails_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentTennis_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }
    } 
}