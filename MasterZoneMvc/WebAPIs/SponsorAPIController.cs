using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MasterZoneMvc.Common.ValidationHelpers;
using System.Security.Claims;
using System.IO;
using System.Data.SqlClient;
using MasterZoneMvc.PageTemplateViewModels;

namespace MasterZoneMvc.WebAPIs
{
    public class SponsorAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        private SuperAdminSponsorService superAdminSponsorService;
        private FileHelper fileHelper;
        public SponsorAPIController()
        {
            db = new MasterZoneDbContext();
            storedProcedureRepository = new StoredProcedureRepository(db);
            superAdminSponsorService = new SuperAdminSponsorService(db);
            fileHelper = new FileHelper();
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
        /// To Add/Update SuperAdmin Sponsor Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Sponsor/AddUpdateSuperAdminSponsor")]
        public HttpResponseMessage AddUpdateBusinessSponsorProfile()
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

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                SuperAdminSponsorViewModel businessContentSponsor_VM = new SuperAdminSponsorViewModel();
                businessContentSponsor_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentSponsor_VM.SponsorTitle = HttpRequest.Params["SponsorTitle"].Trim();
                businessContentSponsor_VM.SponsorLink = HttpRequest.Params["SponsorLink"].Trim();
                businessContentSponsor_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssSponsorIconFile = files["SponsorIcon"];
                businessContentSponsor_VM.SponsorIcon = _BusinesssSponsorIconFile; // for validation
                string _BusinessSponsorIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessSponsorIconFileName = ""; // will be used to delete file while updating.




                // Validate infromation passed
                Error_VM error_VM = businessContentSponsor_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {

                    if (_BusinesssSponsorIconFile != null)
                    {
                        _BusinessSponsorIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssSponsorIconFile);
                    }


                }


                if (businessContentSponsor_VM.Mode == 2)
                {

                    var respGetSuperAdminSponsorData = superAdminSponsorService.GetSuperAdminSponsorDetailById(businessContentSponsor_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessSponsorIconFileNameGenerated = respGetSuperAdminSponsorData.SponsorIcon ?? "";

                    }
                    else
                    {
                        _PreviousBusinessSponsorIconFileName = respGetSuperAdminSponsorData.SponsorIcon ?? "";

                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateSuperAdminSponsor_Get<SPResponseViewModel>(new SP_InsertUpdateSuperAdminSponsor_Param_VM
                {
                    Id = businessContentSponsor_VM.Id,
                    CreatedByLoginId = _LoginID_Exact,
                    SponsorTitle = businessContentSponsor_VM.SponsorTitle,
                    SponsorLink = businessContentSponsor_VM.SponsorLink,
                    SponsorIcon = _BusinessSponsorIconFileNameGenerated,
                    Mode = businessContentSponsor_VM.Mode
                });




                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Sponsor Image.
                    #region Insert-Update Sponsor Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessSponsorIconFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminSponsorImage), _PreviousBusinessSponsorIconFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminSponsorImage), _BusinessSponsorIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssSponsorIconFile, NewImageFileWithPath);


                    }

                    #endregion
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
        /// To Get SuperAdmin Sponsor Detail By  Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/GetSuperAdminSponsorDetailById")]
        public HttpResponseMessage GetSuperAdminSponsorDetailById(long Id)
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


                SuperAdminSponsor_VisitorVM resp = new SuperAdminSponsor_VisitorVM();

                resp = superAdminSponsorService.GetSuperAdminSponsorDetailById(Id);


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
        /// To Deleted the SuperAdmin Sponsor Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdmin/Sponsor/DeleteById")]
        public HttpResponseMessage DeleteSponsorById(long Id)
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

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Business Sponsor 
                resp = superAdminSponsorService.DeleteSuperAdminSponsor(Id);


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
        /// Get All Sponsor List for Visitor Panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminSponsor/GetAll")]
        public HttpResponseMessage GetAllSuperAdminSponsorList()
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

                List<SuperAdminSponsor_VisitorVM> eventSponsorList = new List<SuperAdminSponsor_VisitorVM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("mode", "3"),
                };
                eventSponsorList = db.Database.SqlQuery<SuperAdminSponsor_VisitorVM>("exec sp_ManageSuperAdminSponsors @id,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = eventSponsorList;

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
        /// To change the Super-Admin-Sponsor Home Visibility Status
        /// </summary>
        /// <param name="id">SuperAdminSponsor-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdminSponsor/ToggleHomePageVisibilityStatus/{id}")]
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

                var resp = storedProcedureRepository.SP_InsertUpdateSuperAdminSponsor_Get<SPResponseViewModel>(new SP_InsertUpdateSuperAdminSponsor_Param_VM
                {
                    Id = id,
                    Mode = 4
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
        /// Get All Sponsors (SuperAdmin) For Home Page display
        /// </summary>
        /// <returns>Super-Admin Sponsors List</returns>
        [HttpGet]
        [Route("api/SuperAdminSponsor/GetAllSponsorsForHomePage")]
        public HttpResponseMessage GetAllSponsorsForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var response = superAdminSponsorService.GetSponsorListForHomePage();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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