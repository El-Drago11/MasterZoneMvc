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
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.Services
{
    public class BusinessContentBannerService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public BusinessContentBannerService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }
        /// <summary>
        /// Business Banner Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Banner Detail</returns>
        public BannerDetail_VM GetBusinessBannerDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentBanner_Get<BannerDetail_VM>(new SP_ManageBusinessContentBanner_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Business Banner Detail By Id
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Banner Detail</returns>
        public BannerDetail_VM GetBusinessBannerDetail_ById(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentBanner_Get<BannerDetail_VM>(new SP_ManageBusinessContentBanner_Params_VM
            {

                Id = Id,
                Mode = 2


            });

        }

        public JqueryDataTable_Pagination_Response_VM<BannerDetail_Pagination_VM> GetBusinessContentBannerList_Pagination(NameValueCollection httpRequestParams, BannerDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BannerDetail_Pagination_VM> lstBannerRecords = new List<BannerDetail_Pagination_VM>();

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

            lstBannerRecords = db.Database.SqlQuery<BannerDetail_Pagination_VM>("exec sp_ManageBusinessContentProfilePage_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstBannerRecords.Count > 0 ? lstBannerRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BannerDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BannerDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstBannerRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Business Content Banner Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Banner Detail</returns>
        public SPResponseViewModel DeleteBannerDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateBusinessContentSportsBanner_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentBanner_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }

        /// <summary>
        /// Business Banner Detail (List)
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Banner Detail</returns>
        public List<BannerDetail_VM> GetBusinessBannerDetailList(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentBannerList_Get<BannerDetail_VM>(new SP_ManageBusinessContentBanner_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }



        /// <summary>
        /// To Get Education banner detail by Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public BusinessEducationBannerDetail_VM GetEducationBannerProfileDetail(long Id)
        {
            return storedPorcedureRepository.SP_ManageBusinessEducationBannerDetail_Get<BusinessEducationBannerDetail_VM>(new SP_ManageBusinessContentBannerDetail_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }



        /// <summary>
        /// To Get  Education/Course Banner Detail List 
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessEducationBannerDetail_VM> GetEducationBannerProfileDetail_Get(long BusinessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessEducationBannerDetailList_Get<BusinessEducationBannerDetail_VM>(new SP_ManageBusinessContentBannerDetail_Param_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }




        /// <summary>
        /// To Delete Education Banner Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBusinessContentEducationBannerDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdateBusinessContentEducationBannerProfileDetail_Get<SPResponseViewModel>(new SP_InsertUpdateEducationBannerDetail_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }
        /// <summary>
        /// to get class intermediate for edit
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public ClassIntermediatesDetail_VM GetClassInterMediateDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageClassInterMediate_Get<ClassIntermediatesDetail_VM>(new SP_ManageClassIntermediatesDetail_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

    }
}