using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class QueryAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private QueryService queryService;

        public QueryAPIController()
        {
            db = new MasterZoneDbContext();
            queryService = new QueryService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Queries with Pagination for Business
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Query/Business/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllBusinessQueriesByPagination()
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

                ManageQuery_Pagination_SQL_Params_VM _Params_VM = new ManageQuery_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessOwnerLoginId = _BusinessOwnerLoginId;

                var paginationResponse = queryService.GetBusinessQueriesList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// Add or Update Query by Student-User
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Query/AddUpdateQuery")]
        public HttpResponseMessage AddUpdateEnqueries()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Queries_VM enquiries_VM = new Queries_VM();

                enquiries_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                enquiries_VM.Title = HttpRequest.Params["Title"].Trim();
                enquiries_VM.Description = HttpRequest.Params["Description"].Trim();
                enquiries_VM.BusinessOwnerId = Convert.ToInt64(HttpRequest.Params["BusinessOwnerId"]);
                enquiries_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = enquiries_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", enquiries_VM.Id),
                            new SqlParameter("studentLoginId", _LoginId),
                            new SqlParameter("businessOwnerLoginId",enquiries_VM.BusinessOwnerId),
                            new SqlParameter("title",enquiries_VM.Title),
                            new SqlParameter("description",enquiries_VM.Description),
                            new SqlParameter("replyBody",enquiries_VM.Description),
                            new SqlParameter("submittedByLoginId",_LoginId),
                            new SqlParameter("mode",enquiries_VM.Mode),
                            };

                resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateQueries @id,@studentLoginId,@businessOwnerLoginId,@title,@description,@replyBody,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        //// NOT in use / NOT to use
        //[HttpGet]
        //// [Authorize(Roles = "BusinessAdmin")]
        //[Route("api/Query/Business/Owner/Get")]
        //public HttpResponseMessage GetBusinessOwnerData()
        //{
        //    ApiResponse_VM apiResponse = new ApiResponse_VM();
        //    try
        //    {
        //        var validateResponse = ValidateLoggedInUser();
        //        if (validateResponse.ApiResponse_VM.status < 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
        //        }

        //        long _LoginID_Exact = validateResponse.UserLoginId;
        //        long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

        //        List<BusinessOwnerViewModel> lst = new List<BusinessOwnerViewModel>();
        //        // Get Business Profile Data for Image Names
        //        SqlParameter[] queryParam = new SqlParameter[] {
        //                   new SqlParameter("id", "0"),
        //                    new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
        //                    new SqlParameter("userLoginId", "0"),
        //                    new SqlParameter("mode", "3")
        //                    };

        //        lst = db.Database.SqlQuery<BusinessOwnerViewModel>("exec sp_ManageStudent @id,@businessOwnerLoginId,@userLoginId,@mode", queryParam).ToList();

        //        apiResponse.status = 1;
        //        apiResponse.message = "success";
        //        apiResponse.data = lst;

        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.status = -500;
        //        apiResponse.message = "Internal Server Error!";
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}

        /// <summary>
        /// Get All Queries List Created by Student with Pagination [View-More-Type-Pagination]
        /// </summary>
        /// <param name="lastRecordId">Last Record Fetched Id</param>
        /// <returns>List of Queries</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Query/GetAllByStudent")]
        public HttpResponseMessage GetAllQueriesByStudentPagination(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<QueryDetail_VM> lst = new List<QueryDetail_VM>();
                Query query = new Query();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentId", "0"),
                            new SqlParameter("lastRecordId", lastRecordId),
                            new SqlParameter("recordLimit", recordLimit),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "1")
                            };

                lst = db.Database.SqlQuery<QueryDetail_VM>("exec sp_GetQueriesByStudent_Pagination @id,@studentId,@lastRecordId,@recordLimit,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    enqurylst = lst,
                };

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
        /// Delete Query created by Student-User
        /// </summary>
        /// <param name="id">Query Id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Query/Delete")]
        public HttpResponseMessage DeleteQuery(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", id),
                            new SqlParameter("studentId", ""),
                            new SqlParameter("businessOwnerId",""),
                            new SqlParameter("userLoginId",_LoginId),
                            new SqlParameter("mode","3"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageQueries @id,@studentId,@businessOwnerId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };

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
        /// Get Query Detail by Query Id
        /// </summary>
        /// <param name="id">Query Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student,BusinessAdmin,Staff")]
        [Route("api/Query/GetById")]
        public HttpResponseMessage GetQueryById(long id)
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
                QueryDescription_VM queryResponse = new QueryDescription_VM();
                Query query = new Query();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("studentId", ""),
                            new SqlParameter("businessOwnerId",""),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode","2"),
                            };

                queryResponse = db.Database.SqlQuery<QueryDescription_VM>("exec  sp_ManageQueries @id,@studentId,@businessOwnerId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = queryResponse;

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
        /// Add Reply to Query by Business-User
        /// </summary>
        /// <param name="queryId">Query Id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Query/AddQueryReply")]
        public HttpResponseMessage AddQueryReply()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Queries_VM enquiries_VM = new Queries_VM();

                enquiries_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                enquiries_VM.ReplyBody = HttpRequest.Params["ReplyBody"].Trim();
                enquiries_VM.Mode = 3;//Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = enquiries_VM.ValidReplyInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", enquiries_VM.Id),
                            new SqlParameter("studentLoginId", "0"),
                            new SqlParameter("businessOwnerLoginId",_BusinessOwnerLoginId),
                            new SqlParameter("title",""),
                            new SqlParameter("description",""),
                            new SqlParameter("replyBody",enquiries_VM.ReplyBody),
                            new SqlParameter("submittedByLoginId",_LoginId),
                            new SqlParameter("mode",enquiries_VM.Mode)
                            };

                resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateQueries @id,@studentLoginId,@businessOwnerLoginId,@title,@description,@replyBody,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
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