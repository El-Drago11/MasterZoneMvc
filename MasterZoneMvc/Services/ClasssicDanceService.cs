using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using System.Collections.Specialized;
using System.Data.SqlClient;

namespace MasterZoneMvc.Services
{
    public class ClasssicDanceService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public ClasssicDanceService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// To Get Classic Detail By BusinessOwnerLoginId
        /// </summary>
        /// <param name="branchBusinessLoginId"></param>
        /// <returns></returns>
        public ClassicDanceTechnique_VM GetClassicDanceTechniquebusinessLoginId(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessClassicDanceTechniqueDetail_Get<ClassicDanceTechnique_VM>(new SP_ManageBusinessClassicDanceTechnique_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }


        /// <summary>
        /// To Get Classic dance Video Detail By BusinessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentClassicDanceVideoDetail_VM GetClassicDanceVideoDetail(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessClassicDanceVideoDetail_Get<BusinessContentClassicDanceVideoDetail_VM>(new SP_ManageBusinessClassicDanceTechnique_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Classic Dance By BusinessOwnerLoginId For Visitor-Panel
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentClassicDanceDetail_ViewModel> GetClassicDanceTechniqueDetail_lst(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessClassicDanceTechniqueDetail_Getlst<BusinessContentClassicDanceDetail_ViewModel>(new SP_ManageBusinessClassicDanceTechnique_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }



        /// <summary>
        /// To Get Busines Classic Dance Technique Detail PPCMeta By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public BusinessContentClassicDanceDetail_ViewModel GetClassicDanceTechniqueById(long Id)
        {
            return storedPorcedureRepository.SP_ManageBusinessClassicDanceTechniqueDetail_Get<BusinessContentClassicDanceDetail_ViewModel>(new SP_ManageBusinessClassicDanceTechnique_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }

        /// <summary>
        /// To Delete Business Content Classic Dance Technique Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBusinessContentClassicTechniqueDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdateBusinesClassicDanceTechniqueDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentClassicDance_PPCMeta_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }
        /// <summary>
        /// To Get Classic Dance Detail 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>

        public JqueryDataTable_Pagination_Response_VM<ClassicDanceDetail_ByPagination> GetBusinessContentClassicDanceList_Pagination(NameValueCollection httpRequestParams, ClassicDanceDetail_Pagination_SQL_Params_VM _Params_VM)
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


            List<ClassicDanceDetail_ByPagination> lstRecords = new List<ClassicDanceDetail_ByPagination>();

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

            lstRecords = db.Database.SqlQuery<ClassicDanceDetail_ByPagination>("exec sp_GetAllBusinesClassicDanceTechnique_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<ClassicDanceDetail_ByPagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<ClassicDanceDetail_ByPagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// To Get Classic Dance Profile Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public ClassicDanceProfileDetail_VM GetClassicDanceProfileDetail(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageClassicDanceProfile_PPCMeta_Get<ClassicDanceProfileDetail_VM>(new SP_ManageClassicDanceProfileDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Classic Dance Profile Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public ClassicDanceProfileDetail_VM GetClassicDanceProfileDetail_Getlst(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageClassicDanceProfile_PPCMeta_Get<ClassicDanceProfileDetail_VM>(new SP_ManageClassicDanceProfileDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 2
            });
        }
    }
}