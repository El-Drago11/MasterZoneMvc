using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class EventCategoryAPIController : ApiController
    {

        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private EventService eventService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public EventCategoryAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            eventService = new EventService(db);
            businessOwnerService = new BusinessOwnerService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
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
        /// Add Update Event Category For Super Admin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/EventCategory/AddUpdateEventCategory")]
        public HttpResponseMessage AddUpdateEventCategory()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestEventCategory_VM requestEventCategory_VM = new RequestEventCategory_VM();
                requestEventCategory_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestEventCategory_VM.CategoryName = HttpRequest.Params["CategoryName"].Trim();
                requestEventCategory_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestEventCategory_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = requestEventCategory_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update Card-Section
                var resp = eventService.InsertUpdateEventCategory(new ViewModels.StoredProcedureParams.SP_InsertUpdateEventCategory_Params_VM()
                {
                    Id = requestEventCategory_VM.Id,
                    CategoryName = requestEventCategory_VM.CategoryName,
                    Status = requestEventCategory_VM.Status,
                    Mode = requestEventCategory_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

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
        /// get all details by list
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/EventCategory/GetAllEventCategoryDetail")]
        [HttpGet]
        public HttpResponseMessage GetAllEventCategoryDetail()
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

                List<EventCategory_VM> ParentBusinessCategory = eventService.GetAllEventCategories();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = ParentBusinessCategory;

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
        /// Get Single Event-Category Detail by id
        /// </summary>
        /// <param name="id">Event-Category Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/EventCategory/GetEventCategoryTypeById")]
        public HttpResponseMessage GetEventCategoryTypeById(long id)
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

                var resp = eventService.GetEventCategoryById(id);

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



        /// <summary>
        /// Delete Event-Category-Type By Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/EventCategory/DeleteEventCategory")]
        public HttpResponseMessage DeleteEventCategory(long id)
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
                var resp = eventService.DeleteEventCategory(id);

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
        /// Change Active-Inactive Status by Id
        /// </summary>
        /// <param name="id">Event-Category-Type Id</param>
        /// <returns>Returns response +ve for success, else -ve with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/EventCategory/ChangeStatusEventCategoryType")]
        public HttpResponseMessage ChangeStatusEventCategoryType(long id)
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

                var resp = eventService.ChangeStatusEventCategoryType(id);

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
        /// Get all active event Category detail by list
        /// </summary>
        /// <returns>Event Category List</returns>
        //[Authorize(Roles = "SuperAdmin,SubAdmin,BusinessAdmin,Staff")]
        [Route("api/EventCategory/GetAllActiveEventCategoryDetail")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveEventCategoryDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _LoginID_Exact = validateResponse.UserLoginId;
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<EventCategory_VM> categoryList = eventService.GetAllEventCategories().Where(e => e.Status == 1).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = categoryList;

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
