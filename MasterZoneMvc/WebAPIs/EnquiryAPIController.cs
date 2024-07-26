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
    public class EnquiryAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private EnquiryService enquiryService;

        public EnquiryAPIController()
        {
            db = new MasterZoneDbContext();
            enquiryService = new EnquiryService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Enquiries with Pagination for Business
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/Business/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllBusinessEnquiriesByPagination()
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

                ManageEnquiry_Pagination_SQL_Params_VM _Params_VM = new ManageEnquiry_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessOwnerLoginId = _BusinessOwnerLoginId;

                var paginationResponse = enquiryService.GetBusinessEnquiriesList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Add or Update Enquiry by Student-User
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/AddUpdateEnquiry")]
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestEnquiry_VM enquiries_VM = new RequestEnquiry_VM();

                enquiries_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                enquiries_VM.Name = HttpRequest.Params["Name"].Trim();
                enquiries_VM.Gender = HttpRequest.Params["Gender"].Trim();
                enquiries_VM.Email = HttpRequest.Params["Email"].Trim();
                enquiries_VM.DOB = HttpRequest.Params["DOB"].Trim();
                enquiries_VM.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                enquiries_VM.AlternatePhoneNumber = HttpRequest.Params["AlternatePhoneNumber"].Trim();
                enquiries_VM.Address = HttpRequest.Params["Address"].Trim();
                enquiries_VM.Activity = HttpRequest.Params["Activity"].Trim();
                enquiries_VM.LevelId = Convert.ToInt64(HttpRequest.Params["LevelId"]);
                enquiries_VM.ClassId = Convert.ToInt64(HttpRequest.Params["ClassId"]);
                enquiries_VM.BusinessPlanId = Convert.ToInt64(HttpRequest.Params["BusinessPlanId"]);
                //enquiries_VM.StaffId = Convert.ToInt64(HttpRequest.Params["StaffId"]);
                enquiries_VM.Status = HttpRequest.Params["Status"].Trim();
                enquiries_VM.StartFromDate = HttpRequest.Params["StartFromDate"].Trim();
                //enquiries_VM.FollowUpDate = HttpRequest.Params["FollowUpDate"].Trim();
                enquiries_VM.Notes = HttpRequest.Params["Notes"].Trim();
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

                // Insert-Update enquiry 
                resp = enquiryService.InsertUpdateEnquiry(new ViewModels.StoredProcedureParams.SP_InsertUpdateEnquiries_Params_VM()
                {
                    Id = enquiries_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Name = enquiries_VM.Name,
                    Email = enquiries_VM.Email,
                    DOB = enquiries_VM.DOB,
                    Gender = enquiries_VM.Gender,
                    PhoneNumber = enquiries_VM.PhoneNumber,
                    AlternatePhoneNumber = enquiries_VM.AlternatePhoneNumber,
                    Address = enquiries_VM.Address,
                    Activity = enquiries_VM.Activity,
                    BusinessPlanId = enquiries_VM.BusinessPlanId,
                    ClassId = enquiries_VM.ClassId,
                    LevelId = enquiries_VM.LevelId,
                    StartFromDate = enquiries_VM.StartFromDate,
                    //StaffId = enquiries_VM.StaffId,
                    //FollowUpDate = enquiries_VM.FollowUpDate,
                    Notes = enquiries_VM.Notes,
                    SubmittedByLoginId = _LoginId,
                    Status = enquiries_VM.Status,
                    Mode = enquiries_VM.Mode
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

        /// <summary>
        /// Delete Enquiry created by Student-User
        /// </summary>
        /// <param name="id">Enquiry Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/Delete")]
        public HttpResponseMessage DeleteEnquiry(long id)
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

                // Delete Enquiry
                SPResponseViewModel resp = enquiryService.DeleteEnquiry(id, _BusinessOwnerLoginId);

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
        /// Get Enquiry Detail by Enquiry Id
        /// </summary>
        /// <param name="id">Enquiry Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/GetById")]
        public HttpResponseMessage GetEnquiryById(long id)
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
                EnquiryDescription_VM enquiryResponse = new EnquiryDescription_VM();

                enquiryResponse = enquiryService.GetEnquiryById(id, _BusinessOwnerLoginId);
                List<EnquiryFollowCommentList_VM> enquiryFollowCommentList_VM = enquiryService.GetCommentsForBusinessById(id, _BusinessOwnerLoginId);

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
        /// Add or Update Enquiry by Student-User
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/UpdateEnquiryDetail")]
        public HttpResponseMessage UpdateEnqueries()
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
                EnquiryStaffListUpdate_VM enquiriesStaffList_VM = new EnquiryStaffListUpdate_VM();

                enquiriesStaffList_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                enquiriesStaffList_VM.StaffId = Convert.ToInt64(HttpRequest.Params["StaffId"]);
                enquiriesStaffList_VM.FollowUpDate = HttpRequest.Params["FollowUpDate"];
                enquiriesStaffList_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = enquiriesStaffList_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                // Insert-Update enquiry 
                resp = enquiryService.UpdateEnquiry(new ViewModels.StoredProcedureParams.SP_InsertUpdateEnquiries_Params_VM()
                {
                    Id = enquiriesStaffList_VM.Id,
                    StaffId = enquiriesStaffList_VM.StaffId,
                    FollowUpDate = enquiriesStaffList_VM.FollowUpDate,
                    SubmittedByLoginId = _LoginId,
                    Mode = enquiriesStaffList_VM.Mode

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

        /// <summary>
        /// To change the Enquiry status By Business 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeStatusEnquiry(long id)
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

                // Insert-Update enquiry 
                resp = enquiryService.UpdateEnquiryStatus(id);

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
        /// Get Enquiry Details by Id for follow up
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/GetEnquiryInfoById")]
        public HttpResponseMessage GetEnquiryInfoById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<EnquiryDescription_VM> enquiryResponse = enquiryService.GetEnquiryDetailById(id, _BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    EnquiryDetail = enquiryResponse,
                    // EnquiryComments = enquiryFollowCommentList_VM,
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

    }
}