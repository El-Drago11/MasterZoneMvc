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
    public class BusinessContentStudioEquipmentService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public BusinessContentStudioEquipmentService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Business Content Studio Equipment PPC Meta  Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Studio Equipment PPC Meta Detail</returns>
        public BusinessContentStudioEqipment_PPCMeta_VM GetBusinessContentStudioEquipment_PPCMetaDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentStudioEquipment_PPCMeta_Get<BusinessContentStudioEqipment_PPCMeta_VM>(new SP_ManageBusinessContentStudioEquipment_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }


        /// <summary>
        /// Business Content Studio Equipment   Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Studio Equipment  Detail</returns>
        public BusinressContentStudioEquipmentDetail_VM GetBusinessContentStudioEquipmentDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentStudioEquipment_Get<BusinressContentStudioEquipmentDetail_VM>(new SP_ManageBusinessContentStudioEquipment_Param_VM
            {

                Id = Id,
                Mode = 1


            });

        }

        /// <summary>
        /// Get Business Owners Studio Equipment Detail By Pagination [Jquery Datatable Pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentStudioEqipmentDetail_Pagination_VM> GetBusinessContentStudioEquipmentList_Pagination(NameValueCollection httpRequestParams, BusinessContentStudioEqipmentDetail_Pagination_SQL_Params_VM _Params_VM)
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


            List<BusinessContentStudioEqipmentDetail_Pagination_VM> lstStudioEquipmentRecords = new List<BusinessContentStudioEqipmentDetail_Pagination_VM>();

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

            lstStudioEquipmentRecords = db.Database.SqlQuery<BusinessContentStudioEqipmentDetail_Pagination_VM>("exec sp_ManageBusinessContentStudioEquipment_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudioEquipmentRecords.Count > 0 ? lstStudioEquipmentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentStudioEqipmentDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentStudioEqipmentDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudioEquipmentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Business Content Studio Equipment  Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Banner Detail</returns>
        public SPResponseViewModel DeleteStudioEquipmentDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateBusinessContentStudioEquipmentDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentStudioEquipment_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }
        /// <summary>
        /// Business Content Studio Equipment  Record Detail 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To   Get Studio Equipment Business Content Studio Equipment Detail</returns>
        public List<BusinressContentStudioEquipmentDetail_VM> GetBusinessContentStudioEquipment_lstDetail(long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentStudioEquipmentList_Get<BusinressContentStudioEquipmentDetail_VM>(new SP_ManageBusinessContentStudioEquipment_Param_VM
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 2


            });

        }

        ////////////////////////////////////Audio Detail//////////////////////////////////////////////////////////////////

        /// <summary>
        /// Business Content Audio  PPC Meta  Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Audio  PPC Meta Detail</returns>
        public BusinessContentProtfolioDetail_VM GetBusinessContentAudio_PPCMetaDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAudioDetail_PPCMetaGet<BusinessContentProtfolioDetail_VM>(new SP_ManageBusinessContentPortfolio_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }


        /// <summary>
        /// Business Content Audio   Record Detail  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Get   Business Content Audio Detail</returns>
        public BusinessContentAudioDetail_VM BusinessContentAudioDetail_GetById(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAudioDetail_PPCMetaGet<BusinessContentAudioDetail_VM>(new SP_ManageBusinessContentPortfolio_Param_VM
            {
                Id = Id,
                Mode = 2


            });

        }


        /// <summary>
        /// Business Content Audio   Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Audio Detail</returns>
        public SPResponseViewModel DeleteAudioDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateBusinessContentAudioDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentAudio_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }

        /// <summary>
        /// Get Business Owners Audio  Detail By Pagination [Jquery Datatable Pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentAudioDetail_Pagination_VM> GetBusinessContentAudioList_Pagination(NameValueCollection httpRequestParams, BusinessContentAudioDetail_Pagination_SQL_Params_VM _Params_VM)
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


            List<BusinessContentAudioDetail_Pagination_VM> lstAudioRecords = new List<BusinessContentAudioDetail_Pagination_VM>();

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

            lstAudioRecords = db.Database.SqlQuery<BusinessContentAudioDetail_Pagination_VM>("exec sp_ManageBusinessContentAudio_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstAudioRecords.Count > 0 ? lstAudioRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentAudioDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentAudioDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstAudioRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }



        /// <summary>
        /// Business Content Audio    Record Detail  By businessOwnerLoginId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To   Get Studio Equipment Business Content Audio  Detail</returns>
        public List<BusinessContentAudioDetail_VM> GetBusinessContentAudiolst_Get(long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAudioList_Get<BusinessContentAudioDetail_VM>(new SP_ManageBusinessContentPortfolio_Param_VM
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3


            });

        }
    }
}