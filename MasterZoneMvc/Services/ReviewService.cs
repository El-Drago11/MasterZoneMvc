using MasterZoneMvc.DAL;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
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
    public class ReviewService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public ReviewService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        public JqueryDataTable_Pagination_Response_VM<ReviewPagintation_VM> GetRatingReview_Pagination(NameValueCollection httpRequestParams, Review_Pagination_SQL_Params_VM _Params_VM)
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


            List<ReviewPagintation_VM> lstReviewRecords = new List<ReviewPagintation_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstReviewRecords = db.Database.SqlQuery<ReviewPagintation_VM>("exec sp_ManageReview_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstReviewRecords.Count > 0 ? lstReviewRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<ReviewPagintation_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<ReviewPagintation_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstReviewRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Business  Review Rating Detail  
        /// </summary>
        /// <param name="userLoginId">Enquiry Id</param>
        /// <returns>Business Rating Detail</returns>
        public BusinessReviewDetail GetInstructorReviewDetail(long UserLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageReview<BusinessReviewDetail>(new SP_ManageReview_Params_VM()
            {
                UserLoginId = UserLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Business  Review Rating Detail  
        /// </summary>
        /// <param name="userLoginId">Enquiry Id</param>
        /// <returns>Business Rating Detail</returns>
        public BusinessReviewDetail GetBusinessReviewDetail_Get(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageReview<BusinessReviewDetail>(new SP_ManageReview_Params_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 4
            });
        }







        /// <summary>
        /// Training Rating By UserLoginId
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public List<BusinessTrainingReviewsDetail> GetTrainingRating(long UserLoginId)
        {
            return storedPorcedureRepository.SP_ManageReview_Get<BusinessTrainingReviewsDetail>(new SP_ManageReview_Params_VM
            {

                UserLoginId = UserLoginId,
                Mode = 3


            });

        }

        /// <summary>
        /// Get Training  Review Rating Detail  by Id -For certification 
        /// </summary>
        /// <param name="trainingId">Enquiry Id</param>
        /// <returns>Status 1 To training Instructor Detail, else -ve value with message</returns>
        public BusinessReviewDetail GetBusinessReviewDetail(long UserLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageReview<BusinessReviewDetail>(new SP_ManageReview_Params_VM()
            {
                UserLoginId = UserLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get  dance Rating  Review Rating Detail  
        /// </summary>
        /// <param name=""> </param>
        /// <returns></returns>
        public List<DanceRatingReview_VM> GetRatingReviewDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageReview_Get<DanceRatingReview_VM>(new SP_ManageReview_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 7
            });
        }

        /// <summary>
        /// Business Review Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Review Detail</returns>
        public BusinessContentReviewDetail_VM GetBusinessContentReviewDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentReview_Get<BusinessContentReviewDetail_VM>(new SP_ManageBusinessContentReview_Params_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// To get Business Detail By BusinessOwnerLoginId (User Resume Detail)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<DanceRatingReview_VM> GetBusinessRatingReviewDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageReview_Get<DanceRatingReview_VM>(new SP_ManageReview_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 6
            });
        }


    }
}