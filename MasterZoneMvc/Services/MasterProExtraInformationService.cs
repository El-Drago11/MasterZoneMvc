using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.DAL;
using System.Collections.Specialized;
using System.Data.SqlClient;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;

namespace MasterZoneMvc.Services
{
    public class MasterProExtraInformationService
    {

        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public MasterProExtraInformationService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        public MasterProExtraInformation_ViewModel GetMasterProExtraInformationById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageMasterProExtraInformation_Get<MasterProExtraInformation_ViewModel>(new SP_ManageMasterProExtraInformation_Params_VM()
            {
                Id = Id,
                Mode = 1
            });
        }

        public List<MasterProExtraInformation_VM> GetAllMasterProExtraInformationById(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageMasterProExtraInformation_GetAll<MasterProExtraInformation_VM>(new SP_ManageMasterProExtraInformation_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 2
            });
        }

        

        public JqueryDataTable_Pagination_Response_VM<MasterProExtraInformationService_Pagination_VM> GetMasterproExtraDetailsServiceList_Pagination(NameValueCollection httpRequestParams, MasterProExtraInformationService_Pagination_SQL_Params_VM _Params_VM)
        {
            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 5 : (Convert.ToInt32(httpRequestParams["length"]));
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

            List<MasterProExtraInformationService_Pagination_VM> lstBusinessServiceRecords = new List<MasterProExtraInformationService_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.UserLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
            };

            lstBusinessServiceRecords = db.Database.SqlQuery<MasterProExtraInformationService_Pagination_VM>("exec sp_ManageMasterProExtraInformationService_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstBusinessServiceRecords.Count > 0 ? lstBusinessServiceRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<MasterProExtraInformationService_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<MasterProExtraInformationService_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstBusinessServiceRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        public SPResponseViewModel DeleteMasterproExtraInformationService(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateMasterProExtraInformation_Get<SPResponseViewModel>(new SP_InsertUpdateMasterProExtraInformation_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }
    }
}