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
    public class BusinessContentAboutService
    {

        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public BusinessContentAboutService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }
        /// <summary>
        /// Business About Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get About Detail</returns>
        public AboutDetail_VM GetBusinessAboutDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAbout_Get<AboutDetail_VM>(new SP_ManageBusinessContentAbout
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Business About Detail For  
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get About Detail</returns>
        public BusinessAboutDetailViewModel GetBusinessDanceAboutDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAbout_Get<BusinessAboutDetailViewModel>(new SP_ManageBusinessContentAbout
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Business About Service Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get About  Service Detail</returns>
        public BusinessContentAboutServiceDetail_VM GetBusinessAboutServiceDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAboutService_Get<BusinessContentAboutServiceDetail_VM>(new SP_ManageBusinessContentAboutService
            {

                Id = Id,
                Mode = 1


            });

        }


        /// <summary>
        /// Business About Service Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get About  Service Detail</returns>
        public BusinessContentAboutServiceDetail_VM GetBusinessAboutServiceDetail_Get(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAboutService_Get<BusinessContentAboutServiceDetail_VM>(new SP_ManageBusinessContentAboutService
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }


        /// <summary>
        /// Business About Service Detail (SportsAboutDetailList)
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get About  Service Detail</returns>
        public List<BusinessContentAboutServiceDetail_VM> GetBusinessAboutServiceDetail_GetList(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentAboutService_GetList<BusinessContentAboutServiceDetail_VM>(new SP_ManageBusinessContentAboutService
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }



        public JqueryDataTable_Pagination_Response_VM<AboutServiceDetail_Pagination_VM> GetBusinessContentAboutServiceList_Pagination(NameValueCollection httpRequestParams, AboutServiceDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<AboutServiceDetail_Pagination_VM> lstAboutRecords = new List<AboutServiceDetail_Pagination_VM>();

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

            lstAboutRecords = db.Database.SqlQuery<AboutServiceDetail_Pagination_VM>("exec sp_ManageBusinessContentAboutService_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstAboutRecords.Count > 0 ? lstAboutRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<AboutServiceDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<AboutServiceDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstAboutRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }



        /// <summary>
        /// Business Content About Service Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content About Service Detail</returns>
        public SPResponseViewModel DeleteAboutServiceDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateBusinessContentAboutService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentAboutService
            {
                Id = Id,
                Mode = 3


            });

        }




        /// <summary>
        ///Instructor Other inforamtion For  instructor
        /// </summary>
        /// <param name=""></param>
        /// <returns>To instructor other information</returns>
        public InstructorOtherInformationDetail_VM GetInstructorOtherInformationDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageInstructorOtherInforamtion_Get<InstructorOtherInformationDetail_VM>(new SP_ManageInstructorOtherInforamtion
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }



    }
}