using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class PauseClassAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private PauseClassService pauseClassService;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;

        public PauseClassAPIController()
        {
            db = new MasterZoneDbContext();
            pauseClassService = new PauseClassService(db);
            fileHelper = new FileHelper();
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);

            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// To Add Class Pause Request
        /// </summary>
        /// <param name="classBookingId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Class/AddPauseRequest")]
        public HttpResponseMessage AddPauseClassFeature(long classBookingId)
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
                ClassPauseRequestDetail_VM requestClassPauseRequest_VM = new ClassPauseRequestDetail_VM();
                requestClassPauseRequest_VM.Id = Convert.ToInt32(HttpRequest.Params["Id"]);
                string PauseStartDateParam = HttpRequest.Params["PauseStartDate"];
                requestClassPauseRequest_VM.PauseStartDate = !string.IsNullOrEmpty(PauseStartDateParam) ? PauseStartDateParam.Trim() : string.Empty;
                string PauseEndDateParam = HttpRequest.Params["PauseEndDate"];
                requestClassPauseRequest_VM.PauseEndDate = !string.IsNullOrEmpty(PauseEndDateParam) ? PauseEndDateParam.Trim() : string.Empty;
                string PauseDaysParam = HttpRequest.Params["PauseEndDate"];
                requestClassPauseRequest_VM.PauseDays = Convert.ToInt32(HttpRequest.Params["PauseDays"]);
                string BusinessReplyParam = HttpRequest.Params["BusinessReply"];
                requestClassPauseRequest_VM.BusinessReply = !string.IsNullOrEmpty(BusinessReplyParam) ? BusinessReplyParam.Trim() : string.Empty;
                string ReasonParam = HttpRequest.Params["Reason"];
                requestClassPauseRequest_VM.Reason = !string.IsNullOrEmpty(ReasonParam) ? ReasonParam.Trim() : string.Empty;
                requestClassPauseRequest_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestClassPauseRequest_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);


                // Validate infromation passed
                Error_VM error_VM = requestClassPauseRequest_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                var resp = storedProcedureRepository.SP_InsertUpdatePauseClassRequest_Get<SPResponseViewModel>(new SP_InsertUpdatePauseClassRequest_Param_VM
                {
                    Id = requestClassPauseRequest_VM.Id,
                    UserLoginId = _LoginId,
                    ClassBookingId = classBookingId,
                    PauseStartDate = requestClassPauseRequest_VM.PauseStartDate,
                    PauseEndDate = requestClassPauseRequest_VM.PauseEndDate,
                    Reason = requestClassPauseRequest_VM.Reason,
                    PauseDays = requestClassPauseRequest_VM.PauseDays,
                    SubmittedByLoginId = _LoginId,
                    Status = requestClassPauseRequest_VM.Status,
                    BusinessReply = requestClassPauseRequest_VM.BusinessReply,
                    Mode = requestClassPauseRequest_VM.Mode
                });


                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                // send success response
                apiResponse.status = 1;
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
        /// To Update Action on  Class Pause Request from Business-Panel
        /// </summary>
        /// <param name="classBookingId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/PauseClass/PauseRequestUpdateAction")]
        public HttpResponseMessage PauseRequestUpdateAction()
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
                ClassPauseRequestDetail_VM requestClassPauseRequest_VM = new ClassPauseRequestDetail_VM();
                requestClassPauseRequest_VM.Id = Convert.ToInt32(HttpRequest.Params["Id"]);
                string PauseStartDateParam = HttpRequest.Params["PauseStartDate"];
                requestClassPauseRequest_VM.PauseStartDate = !string.IsNullOrEmpty(PauseStartDateParam) ? PauseStartDateParam.Trim() : string.Empty;
                string PauseEndDateParam = HttpRequest.Params["PauseEndDate"];
                requestClassPauseRequest_VM.PauseEndDate = !string.IsNullOrEmpty(PauseEndDateParam) ? PauseEndDateParam.Trim() : string.Empty;
                string PauseDaysParam = HttpRequest.Params["PauseEndDate"];
                requestClassPauseRequest_VM.PauseDays = Convert.ToInt32(HttpRequest.Params["PauseDays"]);
                string BusinessReplyParam = HttpRequest.Params["BusinessReply"];
                requestClassPauseRequest_VM.BusinessReply = !string.IsNullOrEmpty(BusinessReplyParam) ? BusinessReplyParam.Trim() : string.Empty;
                string ReasonParam = HttpRequest.Params["Reason"];
                requestClassPauseRequest_VM.Reason = !string.IsNullOrEmpty(ReasonParam) ? ReasonParam.Trim() : string.Empty;
                requestClassPauseRequest_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestClassPauseRequest_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);


                // Validate infromation passed
                Error_VM error_VM = requestClassPauseRequest_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var resp = storedProcedureRepository.SP_InsertUpdatePauseClassRequest_Get<SPResponseViewModel>(new SP_InsertUpdatePauseClassRequest_Param_VM
                        {
                            Id = requestClassPauseRequest_VM.Id,
                            UserLoginId = _LoginId,
                            BusinessOwnerLoginId = _BusinessOwnerLoginId,
                            SubmittedByLoginId = _LoginId,
                            Status = requestClassPauseRequest_VM.Status,
                            BusinessReply = requestClassPauseRequest_VM.BusinessReply,
                            Mode = requestClassPauseRequest_VM.Mode
                        });

                        apiResponse.status = resp.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);

                        if (resp.ret <= 0)
                        {
                            transaction.Rollback();
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                        else
                        {
                            // if accepted then extend the dates of class-booking
                            if (requestClassPauseRequest_VM.Status == 2)
                            {
                                var respUpdateDate = storedProcedureRepository.SP_InsertUpdatePauseClassRequest_Get<SPResponseViewModel>(new SP_InsertUpdatePauseClassRequest_Param_VM
                                {
                                    Id = requestClassPauseRequest_VM.Id,
                                    UserLoginId = _LoginId,
                                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                                    ClassBookingId = requestClassPauseRequest_VM.ClassBookingId,
                                    PauseDays = requestClassPauseRequest_VM.PauseDays,
                                    SubmittedByLoginId = _LoginId,
                                    Mode = 3
                                });

                                if (respUpdateDate.ret <= 0)
                                {
                                    transaction.Rollback();

                                    apiResponse.status = resp.ret;
                                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                }
                            }
                        }

                        db.SaveChanges(); // Save changes to the database

                        transaction.Commit(); // Commit the transaction if everything is successful
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions and perform error handling or logging
                        transaction.Rollback(); // Roll back the transaction
                        apiResponse.status = -100;
                        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                    }
                }

                // send response
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
        /// To Add the Pause CLass Request Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllPauseClassRequestDetailByPagination")]
        public HttpResponseMessage GetAllBusinessPauseClassDataTablePagination()
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

                ClassPauseRequest_Pagination_SQL_Params_VM _Params_VM = new ClassPauseRequest_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = pauseClassService.GetPauseClassList_Pagination(HttpRequestParams, _Params_VM);

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

        ///// <summary>
        ///// To Update EndDate In ClassBookingId by BusinessOwnerLoginId
        ///// </summary>
        ///// <param name="classBookingDate_VM"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        //[Route("api/Class/UpdateClassBookingDate")]
        //public HttpResponseMessage AddPauseClassBookingDate(ClassBookingDate_VM classBookingDate_VM)
        //{
        //    ApiResponse_VM apiResponse = new ApiResponse_VM();

        //    try
        //    {
        //        var validateResponse = ValidateLoggedInUser();
        //        if (validateResponse.ApiResponse_VM.status < 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
        //        }

        //        long _LoginId = validateResponse.UserLoginId;
        //        long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

        //        ////--Create object of HttpRequest
        //        //var HttpRequest = HttpContext.Current.Request;
        //        //ClassPauseRequestDetail_VM requestClassPauseRequest_VM = new ClassPauseRequestDetail_VM();
        //        //requestClassPauseRequest_VM.Id = Convert.ToInt32(HttpRequest.Params["Id"]);
        //        //string PauseStartDateParam = HttpRequest.Params["PauseStartDate"];
        //        //requestClassPauseRequest_VM.PauseStartDate = !string.IsNullOrEmpty(PauseStartDateParam) ? PauseStartDateParam.Trim() : string.Empty;
        //        //string PauseEndDateParam = HttpRequest.Params["PauseEndDate"];
        //        //requestClassPauseRequest_VM.PauseEndDate = !string.IsNullOrEmpty(PauseEndDateParam) ? PauseEndDateParam.Trim() : string.Empty;
        //        //string PauseDaysParam = HttpRequest.Params["PauseEndDate"];
        //        //requestClassPauseRequest_VM.PauseDays = Convert.ToInt32(HttpRequest.Params["PauseDays"]);
        //        //string BusinessReplyParam = HttpRequest.Params["BusinessReply"];
        //        //requestClassPauseRequest_VM.BusinessReply = !string.IsNullOrEmpty(BusinessReplyParam) ? BusinessReplyParam.Trim() : string.Empty;
        //        //string ReasonParam = HttpRequest.Params["Reason"];
        //        //requestClassPauseRequest_VM.Reason = !string.IsNullOrEmpty(ReasonParam) ? ReasonParam.Trim() : string.Empty;
        //        //requestClassPauseRequest_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
        //        //requestClassPauseRequest_VM.Mode = 3;   


        //        //// Validate infromation passed
        //        //Error_VM error_VM = requestClassPauseRequest_VM.ValidInformation();

        //        //if (!error_VM.Valid)
        //        //{
        //        //    apiResponse.status = -1;
        //        //    apiResponse.message = error_VM.Message;
        //        //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //        //}


        //        var resp = storedProcedureRepository.SP_InsertUpdatePauseClassRequest_Get<SPResponseViewModel>(new SP_InsertUpdatePauseClassRequest_Param_VM
        //        {
        //            Id = classBookingDate_VM.ClassPauseRequestId,
        //            UserLoginId = _LoginId,
        //            ClassBookingId = classBookingDate_VM.ClassBookingId,
        //            PauseDays = classBookingDate_VM.PauseDays,
        //            SubmittedByLoginId = _LoginId,
        //            Mode = 3
        //        });

        //        if (resp.ret <= 0)
        //        {
        //            apiResponse.status = resp.ret;
        //            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
        //            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //        }

        //        // send success response
        //        apiResponse.status = 1;
        //        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
        //        apiResponse.data = new { };
        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.status = -500;
        //        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}


        /// <summary>
        /// To View All Pause Class Request Detail In Visitor-Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PauseClass/GetAllPauseClassRequestsByPagination")]
        public HttpResponseMessage GetAllPauseClassRequestsByPagination()
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

                ClassPauseRequest_Pagination_SQL_Params_VM _Params_VM = new ClassPauseRequest_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = pauseClassService.GetStudentPauseClassRequestViewList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Delete The Pause Class Detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PauseClass/DeletePauseClassDetailById")]
        public HttpResponseMessage DeletePauseClassDetailById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;



                // Delete By Id
                var resp = pauseClassService.DeletePauseClassDetail(id);


                apiResponse.status = 1;
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
        /// To Get ClassPauseDetail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Business/GetClassPauseRequestDetailById")]
        public HttpResponseMessage GetClassPauseRequestDetailById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                ClassPauseRequestDetail_ViewModel resp = new ClassPauseRequestDetail_ViewModel();

                resp = pauseClassService.GetClassPauseRequestDetailById(id);

               
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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