using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web;
using MasterZoneMvc.Common.Helpers;
using System.Data.SqlClient;

namespace MasterZoneMvc.WebAPIs
{
    public class StudentUserAccountAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private StudentService studentService;
   
        public StudentUserAccountAPIController()
        {
            db = new MasterZoneDbContext();
            studentService = new StudentService(db);        

        }

        /// <summary>
        /// Validate Logged-in user. 
        /// </summary>
        /// <returns></returns>
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All User Account with Pagination For the SuperAdmin-Panel [Admin]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StudentUserAccount/GetAllByPagination")]

        public HttpResponseMessage GetAllUserDetailDataTablePagination()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                StudentUser_Pagination_SQL_Params_VM _Params_VM = new StudentUser_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = studentService.GetStudentUserList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = Resources.BusinessPanel.Success,
                    draw = paginationResponse.draw,
                    data = paginationResponse.data,
                    recordsTotal = paginationResponse.recordsTotal,
                    recordsFiltered = paginationResponse.recordsFiltered
                };

                return Request.CreateResponse(HttpStatusCode.OK, objResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        /// <summary>
        /// Get Student Detail by Student Ids
        /// </summary>
        /// <param name="id">Student Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StudentUser/GetUserDetailById")]
        public HttpResponseMessage GetStudentDetailById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;



                StudentUserDetail_VM studentResponse = new StudentUserDetail_VM();

                studentResponse = studentService.GetStudentUserById(id);

                List<StudentCourseDetail_VM>  studentCourseDetailResponse = studentService.GetStudentCourseDetailById(id);
                List<UserImageDetail_VM> userImageDetailResponse = studentService.GetAllUserContentImagesById(id);
                List<UserPlanDetail_VM> userPlanDetailResponse = studentService.GetAllUserPlanDetailById(id);
                List<UserTrainingsDetail_VM> userTrainingsDetailResponse = studentService.GetAllUserTrainingDetailById(id);
                List<UserContentVedio_VM> userVedioDetailResponse = studentService.GetAllUserContentVedioDetailById(id);
                List<UserCertificateDetail_VM> userCertificateDetailResponse = studentService.GetAllUserCertificateDetailById(id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                     StudentDetail = studentResponse,
                     StudentCourseDetailResponse = studentCourseDetailResponse,
                     UserImageDetailResponse = userImageDetailResponse,
                     UserPlanDetailResponse = userPlanDetailResponse,
                     UserTrainingDetailResponse = userTrainingsDetailResponse,
                     UserVedioDetailResponse = userVedioDetailResponse,
                    UserCertificateDetailResponse = userCertificateDetailResponse
                };
                
                
                ;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Block Student-User
        /// </summary>
        /// <param name="id">Student-Id</param>
        /// <returns>Success or Error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StudentUser/Block")]
        public HttpResponseMessage ChangeStatusStudentUser(long id, string blockReason)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                if(string.IsNullOrEmpty(blockReason))
                {
                    blockReason = string.Empty;
                }

                // Block student 
                resp = studentService.BlockStudent(id, blockReason);

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Unblock Student by student Id
        /// </summary>
        /// <param name="id">Student Id</param>
        /// <returns>Returns success or error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StudentUser/Unblock/{id}")]
        public HttpResponseMessage UnBlockStudentUser(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                // UnBlock student 
                resp = studentService.UnBlockStudent(id);

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// Delete the User-Content-Image By Super-Admin
        /// </summary>
        /// <param name="id">Image-Id</param>
        /// <param name="UserLoginId">Student-User-Login-Id</param>
        /// <returns>Response with status 1 if deleted else -ve value with message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StudentUser/DeleteUserImageBySuperAdmin")]
        public HttpResponseMessage DeleteUserContentImageBySuperAdmin(long id, long UserLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete User-Content-Image
                resp = studentService.DeleteUserContentImage(id, UserLoginId);

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Delete the User-Content-Video By Super-Admin
        /// </summary>
        /// <param name="id">Video-Id</param>
        /// <param name="UserLoginId">Student-User-Login-Id</param>
        /// <returns>Response with status 1 if deleted else -ve value with message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StudentUser/DeleteUserVideoBySuperAdmin")]
        public HttpResponseMessage DeleteUserContentVideoBySuperAdmin(long id, long UserLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete User-Content-Video
                resp = studentService.DeleteUserContentVedio(id, UserLoginId);

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

    }
}