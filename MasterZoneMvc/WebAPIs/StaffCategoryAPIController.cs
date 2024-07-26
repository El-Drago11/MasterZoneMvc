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
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class StaffCategoryAPIController : ApiController
    {

        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private StaffCategoryService staffCategoryService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public StaffCategoryAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            staffCategoryService = new StaffCategoryService(db);
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
        /// Add Update Staff Category For Super Admin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StaffCategory/AddUpdateStaffCategory")]
        public HttpResponseMessage AddUpdateStaffCategory()
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
                RequestStaffCategory_VM requestHPMultipleItem_VM = new RequestStaffCategory_VM();
                requestHPMultipleItem_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestHPMultipleItem_VM.Name = HttpRequest.Params["Name"].Trim();
                requestHPMultipleItem_VM.IsActive = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                requestHPMultipleItem_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = requestHPMultipleItem_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                // Insert-Update Card-Section
                var resp = staffCategoryService.InsertUpdateStaffCategory(new ViewModels.StoredProcedureParams.SP_InsertUpdateStaffCategory_Params_VM()
                {
                    Id = requestHPMultipleItem_VM.Id,
                    Name = requestHPMultipleItem_VM.Name,
                    IsActive = requestHPMultipleItem_VM.IsActive,
                    Mode = requestHPMultipleItem_VM.Mode
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
        [Route("api/StaffCategory/GetAllStaffCategoryDetail")]
        [HttpGet]
        public HttpResponseMessage GetAllStaffCategoryDetail()
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

                List<StaffCategory_VM> ParentBusinessCategory = staffCategoryService.Get_ManageStaffCategoryDetail(_LoginID_Exact);

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
        /// Get Single Staff-Category Detail by id
        /// </summary>
        /// <param name="id">Event-Category Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StaffCategory/GetStaffCategoryTypeById")]
        public HttpResponseMessage GetStaffCategoryTypeById(long id)
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

                var resp = staffCategoryService.GetStaffCategoryById(id);

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
        [Route("api/StaffCategory/DeleteStaffCategory")]
        public HttpResponseMessage DeleteStaffCategory(long id)
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
                var resp = staffCategoryService.DeleteStaffCategory(id);

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
        /// <param name="id">Staff-Category-Type Id</param>
        /// <returns>Returns response +ve for success, else -ve with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/StaffCategory/ChangeStatusStaffCategoryType")]
        public HttpResponseMessage ChangeStatusStaffCategoryType(long id)
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

                var resp = staffCategoryService.ChangeStatusStaffCategoryType(id);

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

