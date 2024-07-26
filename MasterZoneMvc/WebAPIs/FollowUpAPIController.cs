using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
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
using static MasterZoneMvc.ViewModels.ExpenseViewModel;

namespace MasterZoneMvc.WebAPIs
{
    public class FollowUpAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FollowUpService followUpService;
        public FollowUpAPIController()
        {
            db = new MasterZoneDbContext();
            followUpService = new FollowUpService(db);

        }
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }


        /// <summary>
        /// Get All Enquiry with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/FollowUp/GetAllByPagination")]

        public HttpResponseMessage GetAllFollowupDataTablePagination()
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

                Enquiry_Pagination_SQL_Params_VM _Params_VM = new Enquiry_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = followUpService.GetFollowupList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Enquiry Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/FollowUp/EnquiryDetailGetById")]
        public HttpResponseMessage GetEnquiryDetailById(long id)
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

                BusinessEnquiryDetail_VM enquiryResponse = new BusinessEnquiryDetail_VM();

                enquiryResponse = followUpService.GetEnquiryDetailById(id, _BusinessOwnerLoginId);
                List<EnquiryFollowCommentList_VM> enquiryFollowCommentList_VM = followUpService.GetCommentsById(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    EnquiryDetail = enquiryResponse,
                    EnquiryComments = enquiryFollowCommentList_VM,
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
        ///  Update Enquiry  Follow up by Student-User
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/FollowUp/UpdateEnquiryfollowsupDetail")]
        public HttpResponseMessage UpdateEnqueries(long Id)
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
                EnquiryFollowUpViewModel enquiryfollowup_VM = new EnquiryFollowUpViewModel();

                enquiryfollowup_VM.Id = Id;
                //enquiryfollowup_VM.StaffId = Convert.ToInt64(HttpRequest.Params["StaffId"]);
                enquiryfollowup_VM.Comments = HttpRequest.Params["Comments"];
                enquiryfollowup_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = enquiryfollowup_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                // Insert-Update enquiry 
                resp = followUpService.UpdateEnquiryFollowsup(new ViewModels.StoredProcedureParams.SP_InsertUpdateEnquiriesFollowsup_Params_VM()
                {
                    Id = enquiryfollowup_VM.Id,
                    EnquiryId = Id,
                    Comments = enquiryfollowup_VM.Comments,
                    FollowedbyLoginId = _LoginId,
                    SubmittedByLoginId = _LoginId,
                    Mode = enquiryfollowup_VM.Mode

                });

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