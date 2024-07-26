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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class SuperAdminPlanAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private MainPlanService mainPlanService;
        public SuperAdminPlanAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            mainPlanService = new MainPlanService(db);
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
        /// Add Update Main Plan
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminPlan/AddUpdate")]
        public HttpResponseMessage AddUpdateSuperAdminPlan()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestMainPlan_VM requestMainPlan_VM = new RequestMainPlan_VM();
                requestMainPlan_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestMainPlan_VM.PlanDurationTypeKey = HttpRequest.Params["PlanDurationTypeKey"];
                requestMainPlan_VM.planType = Convert.ToInt32(HttpRequest.Params["planType"].Trim());
                requestMainPlan_VM.Name = HttpRequest.Params["Name"].Trim();
                requestMainPlan_VM.Description = HttpUtility.UrlDecode(HttpRequest.Form.Get("Description"));
                requestMainPlan_VM.Price = Convert.ToDecimal(HttpRequest.Params["Price"]);
                requestMainPlan_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestMainPlan_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestMainPlan_VM.CompareAtPrice = (!String.IsNullOrEmpty(HttpRequest.Params["CompareAtPrice"])) ? Convert.ToDecimal(HttpRequest.Params["CompareAtPrice"]) : 0;
                requestMainPlan_VM.Discount = (!String.IsNullOrEmpty(HttpRequest.Params["Discount"])) ? Convert.ToDecimal(HttpRequest.Params["Discount"]) : 0;
                requestMainPlan_VM.PlanPermission = HttpRequest.Params["PermissionIds"];

                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _PlanImageFile = files["PackageImage"];
                requestMainPlan_VM.PlanImage = _PlanImageFile; // for validation
                string _PlanImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousPlanImageFileName = ""; // will be used to delete file while updating.

                long SuperAdminLoginId = 1;
                long _LoginID_Exact = validateResponse.UserLoginId;

                // Validate infromation passed
                Error_VM error_VM = requestMainPlan_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_PlanImageFile != null)
                    {
                        _PlanImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_PlanImageFile);
                    }
                }

                // if Edit Mode
                if (requestMainPlan_VM.Mode == 2)
                {

                    MainPlan_VM mainPlan_VM = new MainPlan_VM();

                    //Get Event By Id
                    mainPlan_VM = mainPlanService.GetMainPlanById(requestMainPlan_VM.Id);
                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _PlanImageFileNameGenerated = mainPlan_VM.PlanImage;
                    }
                    else
                    {
                        _PreviousPlanImageFileName = mainPlan_VM.PlanImage;
                    }
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestMainPlan_VM.Id),
                            new SqlParameter("superAdminLoginId",SuperAdminLoginId),
                            new SqlParameter("planDurationTypeKey",  requestMainPlan_VM.PlanDurationTypeKey),
                            new SqlParameter("planType",  requestMainPlan_VM.planType),
                            new SqlParameter("name", requestMainPlan_VM.Name),
                            new SqlParameter("description", requestMainPlan_VM.Description),
                            new SqlParameter("price", requestMainPlan_VM.Price),
                            new SqlParameter("status", requestMainPlan_VM.Status),
                            new SqlParameter("compareAtPrice", requestMainPlan_VM.CompareAtPrice),
                            new SqlParameter("discount",requestMainPlan_VM.Discount),
                            new SqlParameter("planPermission",requestMainPlan_VM.PlanPermission),
                            new SqlParameter("planImage", _PlanImageFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginID_Exact),
                            new SqlParameter("mode", requestMainPlan_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateMainPlan @id,@superAdminLoginId,@planDurationTypeKey,@planType,@name,@description,@price,@status,@compareAtPrice,@discount,@planPermission,@planImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Event Image.
                    #region Insert-Update Event Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousPlanImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MainPlanImage), _PreviousPlanImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MainPlanImage), _PlanImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_PlanImageFile, FileWithPath);
                    }
                    #endregion
                }
                #region Insert-Update User Permissions
                SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                 new SqlParameter("userLoginId", resp.Id),
                 new SqlParameter("permissionIds", requestMainPlan_VM.PlanPermission),
                 new SqlParameter("mode", "1")
                };

                var respPermissions = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateUserPermissions @userLoginId,@permissionIds,@mode", queryParamsPermissions).FirstOrDefault();

                if (respPermissions.ret <= 0)
                {
                    apiResponse.status = respPermissions.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                #endregion


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
        /// Get All List Main Plan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminPlan/GetAllList")]
        public HttpResponseMessage GetAllListSuperAdminPlan()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                List<MainPlan_VM> mainPlan = new List<MainPlan_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("userLoginId", "1"),
                            new SqlParameter("mode", "1")
                            };

                mainPlan = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "succes";
                apiResponse.data = mainPlan;
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
        /// Get Data Main Plan By Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminPlan/GetById")]
        public HttpResponseMessage GetByIdSuperAdminPlan(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                MainPlan_VM mainPlan = new MainPlan_VM();

                mainPlan = mainPlanService.GetMainPlanById(id);

                string[] stringArray = mainPlan.PlanPermission.Split(',');
                List<string> list = new List<string>(stringArray);

                // Create a new list to hold Permission_VM objects
                List<PermissionListMainPaln_VM> permissionsList = new List<PermissionListMainPaln_VM>();

                // Iterate over the list of strings
                foreach (string item in list)
                {
                    // Create a new Permission_VM object and assign the string value
                    PermissionListMainPaln_VM permission = new PermissionListMainPaln_VM { MainPlanPermissions = item };

                    // Add the permission object to the permissionsList
                    permissionsList.Add(permission);
                }

                // Assign the permissionsList to resp.Permissions
                mainPlan.MainPlanPermissionsList = permissionsList;

                apiResponse.status = 1;
                apiResponse.message = "succes";
                apiResponse.data = mainPlan;
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
        /// Delete Main Plan By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminPlan/DeleteById")]
        public HttpResponseMessage DeleteByIdSuperAdminPlan(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long SuperAdminLoginId = 1;
                long _LoginID_Exact = validateResponse.UserLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("superAdminLoginId",SuperAdminLoginId),
                            new SqlParameter("planDurationTypeKey",  ""),
                            new SqlParameter("planType",  "0"),
                            new SqlParameter("name", ""),
                            new SqlParameter("description", ""),
                            new SqlParameter("price", "0"),
                            new SqlParameter("status", "0"),
                            new SqlParameter("compareAtPrice", "0"),
                            new SqlParameter("discount","0"),
                            new SqlParameter("planPermission",""),
                            new SqlParameter("planImage",  ""),
                            new SqlParameter("submittedByLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateMainPlan @id,@superAdminLoginId,@planDurationTypeKey,@planType,@name,@description,@price,@status,@compareAtPrice,@discount,@planPermission,@planImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
        /// To change the Super-Admin-Plan(MainPlan) Home Visibility Status - [Super-Admin-Panel]
        /// </summary>
        /// <param name="id">MainPlan-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminPlan/ToggleHomePageVisibilityStatus/{id}")]
        public HttpResponseMessage ToggleHomePageVisibilityStatus(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    apiResponse.data = new { };

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("superAdminLoginId","0"),
                            new SqlParameter("planDurationTypeKey",  ""),
                            new SqlParameter("planType",  "0"),
                            new SqlParameter("name", ""),
                            new SqlParameter("description", ""),
                            new SqlParameter("price", "0"),
                            new SqlParameter("status", "0"),
                            new SqlParameter("compareAtPrice", "0"),
                            new SqlParameter("discount","0"),
                            new SqlParameter("planPermission",""),
                            new SqlParameter("planImage",  ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "4")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateMainPlan @id,@superAdminLoginId,@planDurationTypeKey,@planType,@name,@description,@price,@status,@compareAtPrice,@discount,@planPermission,@planImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();


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
        /// Get All Active Main-Plan List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SuperAdminPlan/GetAllActivePlanList")]
        public HttpResponseMessage GetAllActiveMainPlanList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
               
                List<MainPlan_VM> mainPlan = new List<MainPlan_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("userLoginId", "1"),
                            new SqlParameter("mode", "3")
                            };

                mainPlan = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "succes";
                apiResponse.data = mainPlan;
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
        /// Get All Active Main-Plan List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SuperAdminPlan/GetAllActiveMainPlanListForHomePage")]
        public HttpResponseMessage GetAllActiveMainPlanListForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
               
                List<MainPlan_VM> mainPlan = new List<MainPlan_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("userLoginId", "1"),
                            new SqlParameter("mode", "5")
                            };

                mainPlan = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "succes";
                apiResponse.data = mainPlan;
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