using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Data.SqlClient;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;

namespace MasterZoneMvc.Services
{
    public class UserService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public UserService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// To Get UserContentResume Detail 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public BusinessContentUserResumeDetail_VM GetUserContentResumeDetail(long userLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessContentUserResume<BusinessContentUserResumeDetail_VM>(new SP_ManageBusinessContentUserResume_Param_VM()
            {
                UserLoginId = userLoginId,
                Mode = 1
            });
        }
        /// <summary>
        /// To Get The Staff User Content Resume Detail 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public StaffUserContentUserResumeDetail_ViewModel GetStaffUserContentResumeDetail(long userLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessContentUserResume<StaffUserContentUserResumeDetail_ViewModel>(new SP_ManageBusinessContentUserResume_Param_VM()
            {
                BusinessOwnerLoginId = userLoginId,
                Mode = 2
            });
        }


        /// <summary>
        /// To View Experience Detail For Staff Profile Setting Page 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<UserExperienceDetail_ByPagination> GetExperienceDetailList_Pagination(NameValueCollection httpRequestParams, Experience_Pagination_SQL_Params_VM _Params_VM)
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


            List<UserExperienceDetail_ByPagination> lstRecords = new List<UserExperienceDetail_ByPagination>();

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

            lstRecords = db.Database.SqlQuery<UserExperienceDetail_ByPagination>("exec sp_GetAllBusinesUserExperience_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<UserExperienceDetail_ByPagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<UserExperienceDetail_ByPagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// To Get User Experience Detail By Id 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public UserExperienceDetail_ViewModel GetStaffUserExperienceDetail(long Id)
        {
            return storedProcedureRepository.SP_ManageBusinessUserExperienceDetail<UserExperienceDetail_ViewModel>(new SP_ManageBusinessUserExperienceDetail_Param_VM()
            {
              Id = Id,
                Mode = 1
            });
        }
        /// <summary>
        /// To Delete the User Experience Detail by Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteUserExperienceDetail(long Id)
        {
            return storedProcedureRepository.SP_InsertUpdateBusinessUerExperienceDetail_Get<SPResponseViewModel>(new SP_InsertUpdateUserExperience_Param_VM()
            {
              Id = Id,
                Mode = 3
            });
        }

        /// <summary>
        /// To Delete Education By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteUserEducationDetail(long Id)
        {
            return storedProcedureRepository.SP_InsertUpdateBusinessUerEducationDetail_Get<SPResponseViewModel>(new SP_InsertUpdateUserEducation_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }

        /// <summary>
        /// To get Education Detail By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public UserEducation_ViewModel GetStaffUserEducationDetail(long Id)
        {
            return storedProcedureRepository.SP_ManageBusinessUserEducationDetail<UserEducation_ViewModel>(new SP_ManageBusinessUserEducationDetail_Param_VM()
            {
                Id = Id,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Education View Detail 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<UserEducationDetail_ByPagination> GetEducationDetailList_Pagination(NameValueCollection httpRequestParams, Education_Pagination_SQL_Params_VM _Params_VM)
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


            List<UserEducationDetail_ByPagination> lstRecords = new List<UserEducationDetail_ByPagination>();

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

            lstRecords = db.Database.SqlQuery<UserEducationDetail_ByPagination>("exec sp_GetAllBusinesUserEducation_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<UserEducationDetail_ByPagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<UserEducationDetail_ByPagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }
        /// <summary>
        /// To Get User Education Detail List by UserLoginId 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<UserEducation_ViewModel> GetBusinessUserEducationDetaillst_Get(long userLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessUserEducationDetail_Getlst<UserEducation_ViewModel>(new SP_ManageBusinessUserEducationDetail_Param_VM
            {
                UserLoginId = userLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// To Get User Experience Detail By Id 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<UserExperienceDetail_ViewModel> GetBusinessUserExperienceDetaillst_Get(long userLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessUserExperienceDetail_Getlst<UserExperienceDetail_ViewModel>(new SP_ManageBusinessUserExperienceDetail_Param_VM
            {
                UserLoginId = userLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// Get User(Business or Student) Basic Detail By MasterId 
        /// </summary>
        /// <param name="MasterId">Master-Id</param>
        /// <returns>Business or Student Basic Detail by MasterId if exists</returns>
        public UserBasicDetail_VM GetUserBasicDetailByMasterId(string MasterId)
        {
            return storedProcedureRepository.SP_GetDetailByMasterId_Get<UserBasicDetail_VM>(new SP_GetDetailByMasterId_Params_VM()
            {
                MasterId = MasterId,
                Mode = 2
            });
        }

        /// <summary>
        /// to get experience details of user
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<UserExperienceDetail_ViewModel> GetUserExperienceDetail(long Id)
        {
            return storedProcedureRepository.SP_ManageBusinessUserExperienceDetail_Getlst<UserExperienceDetail_ViewModel>(new SP_ManageBusinessUserExperienceDetail_Param_VM()
            {
                UserLoginId = Id,
                Mode = 3
            });
        }

        /// <summary>
        /// to get education details of user
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<UserEducation_ViewModel> GetBusinessEducationDetaillst_Get(long userLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessUserEducationDetail_Getlst<UserEducation_ViewModel>(new SP_ManageBusinessUserEducationDetail_Param_VM()
            {
                UserLoginId = userLoginId,
                Mode = 3
            });
        }
        /// <summary>
        /// to get room details
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentTennisDetail_VM> GetRoomDetail(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageMasterProfileRoomDetail_Get<BusinessContentTennisDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }
        /// <summary>
        /// to get tennis booking details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<TennisBokingDetail_VM> GetTennisBookingDetailList(long businessOwnerloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentTennisDetail_Get<TennisBokingDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM
            {
                BusinessOwnerLoginId = businessOwnerloginid,
                Mode = 3
            });
        }
    }
}