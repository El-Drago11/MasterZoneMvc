using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using MasterZoneMvc.Services;
using System.Data.Entity;
using System.IO;

namespace MasterZoneMvc.WebAPIs
{
    public class SuperAdminAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        private SuperAdminService superAdminService;
        private FileHelper fileHelper;

        public SuperAdminAPIController()
        {
            db = new MasterZoneDbContext();
            storedProcedureRepository = new StoredProcedureRepository(db);
            superAdminService = new SuperAdminService(db);
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
        /// Get All Sponsor List for Visitor Panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SuperAdmin/Sponsor/GetAllForVisitor")]
        public HttpResponseMessage GetAllSuperAdminSponsorList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<SuperAdminSponsor_VisitorVM> eventSponsorList = new List<SuperAdminSponsor_VisitorVM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("mode", "1"),
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
        /// To Update SuperAdmin- Basic Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/Profile/AddUpdate")]
        public HttpResponseMessage AddUpdateSuperAdminProfile()
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
                RequestSuperAdminProfileViewModel requestSuperAdminProfileViewModel = new RequestSuperAdminProfileViewModel();
                requestSuperAdminProfileViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestSuperAdminProfileViewModel.Email = HttpRequest.Params["Email"].Trim();
                requestSuperAdminProfileViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                requestSuperAdminProfileViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                requestSuperAdminProfileViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"];
                requestSuperAdminProfileViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Validate infromation passed
                Error_VM error_VM = requestSuperAdminProfileViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SuperAdminDetail_VM respProfileData = new SuperAdminDetail_VM();
                respProfileData = superAdminService.GetSuperAdminDetail(_LoginID_Exact);


                var resp = storedProcedureRepository.SP_InsertUpdateSuperAdminProfile_Get<SPResponseViewModel>(new SP_InsertUpdateSuperAdminProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    FirstName = requestSuperAdminProfileViewModel.FirstName,
                    LastName = requestSuperAdminProfileViewModel.LastName,
                    Email = requestSuperAdminProfileViewModel.Email,
                    PhoneNumber = requestSuperAdminProfileViewModel.PhoneNumber,
                    Mode = requestSuperAdminProfileViewModel.Mode

                });

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
        ///  To Get SuperAdmin Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/Profile/Get")]
        public HttpResponseMessage GetSuperAdminProfileData()
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

                SuperAdminDetail_VM respProfileData = new SuperAdminDetail_VM();
                respProfileData = superAdminService.GetSuperAdmin_ProfileDetail(_LoginID_Exact);



                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respProfileData;

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
        /// Add-Update SuperAdmin profile-image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/Profile/AddUpdateProfileImage")]
        public HttpResponseMessage AddUpdateSuperAdminProfileImage()
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
                RequestSuperAdminProfileViewModel requestSuperAdminProfileViewModel = new RequestSuperAdminProfileViewModel();
                requestSuperAdminProfileViewModel.Mode = 2;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _superAdminImageFile = files["ProfileImage"]; // change name
                requestSuperAdminProfileViewModel.ProfileImage = _superAdminImageFile;


                string _superAdminImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestSuperAdminProfileViewModel.VaildInformationProfileImage();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SuperAdminDetail_VM respProfileData = new SuperAdminDetail_VM();
                respProfileData = superAdminService.GetSuperAdmin_ProfileDetail(_LoginID_Exact);


                if (_superAdminImageFile != null && files.Count > 0)
                {
                    _superAdminImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_superAdminImageFile);

                    _PreviousProfileImageFileName = respProfileData.ProfileImage;
                }
                else
                {
                    _superAdminImageFileNameGenerated = respProfileData.ProfileImage;
                }

                var resp = storedProcedureRepository.SP_InsertUpdateSuperAdminProfile_Get<SPResponseViewModel>(new SP_InsertUpdateSuperAdminProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    ProfileImage = _superAdminImageFileNameGenerated,
                    Mode = requestSuperAdminProfileViewModel.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update superadmin Image.
                #region Insert-Update [SuperAdmin Profile Image] on Server
                if (files.Count > 0)
                {
                    // if superAdmin Profile Image passed then Add-Update Profile Image
                    if (_superAdminImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminProfileImage), _superAdminImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_superAdminImageFile, fileWithPathImage);
                    }


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
        ///  To Get SuperAdminDashboard Profile LoggedIn User Detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/DashoardProfileDetailGet")]
        public HttpResponseMessage GetSuperAdminDashoardProfileDetail()
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

                SuperAdminDetail_VM respProfileDetail = new SuperAdminDetail_VM();
                respProfileDetail = superAdminService.GetSuperAdminDetail(_LoginID_Exact);



                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respProfileDetail;

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
        ///  To Get SuperAdmin Dashboard  Section Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/DashboardDetailGet")]
        public HttpResponseMessage GetSuperAdminDashboardDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                ////}

                //long _LoginID_Exact = validateResponse.UserLoginId;
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SuperAdminDashboardDetail_VM respDashboardDetail = new SuperAdminDashboardDetail_VM();
                respDashboardDetail = superAdminService.GetSuperAdminDashboardDetail();



                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respDashboardDetail;

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
        /// Add-Update Masterzone Logo by Super-Admin
        /// </summary>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SuperAdmin/UpdateMasterzoneLogo")]
        public HttpResponseMessage UpdateMasterzoneLogoImage()
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

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestSuperAdminProfileViewModel requestSuperAdminProfileViewModel = new RequestSuperAdminProfileViewModel();
                requestSuperAdminProfileViewModel.Mode = 2;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _MasterzoneLogoImageFile = files["Image"];
                requestSuperAdminProfileViewModel.ProfileImage = _MasterzoneLogoImageFile;


                string _superAdminImageFileNameGenerated = "logo.png"; //will contains generated file name
                string _PreviousProfileImageFileName = "logo.png"; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestSuperAdminProfileViewModel.VaildInformationMasterzoneLogoImage();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Masterzone Logo Image.
                #region Insert-Update on Server
                if (files.Count > 0)
                {
                    // if Image passed then Add-Update Image
                    if (_MasterzoneLogoImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MasterzoneLogoImage), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MasterzoneLogoImage), _superAdminImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_MasterzoneLogoImageFile, fileWithPathImage);
                    }

                }
                #endregion

                apiResponse.status = 1;
                apiResponse.message = "Masterzone Logo updated successfully!";
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


#region About Detail Add/Update in SuperAdmin -----------------------

        /// <summary>
        /// To Add/Update SuperAdmin About Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdmin/AddUpdateAboutDetail")]

        public HttpResponseMessage AddUpdateAboutDetail()
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
                long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                SuperAdminAboutContent_ViewModel superAdminAboutContent_ViewModel = new SuperAdminAboutContent_ViewModel();
                if (superAdminAboutContent_ViewModel.Id == 0)
                {

                    superAdminAboutContent_ViewModel.Id = 0;
                }
                else
                {
                    superAdminAboutContent_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                }

                // Check and set About Title 

                string AboutTitleParam = HttpRequest.Params["AboutTitle"];
                superAdminAboutContent_ViewModel.AboutTitle = !string.IsNullOrEmpty(AboutTitleParam) ? AboutTitleParam.Trim() : string.Empty;

                // Check and set About Description
                string AboutDescriptionParam = HttpRequest.Params["AboutDescription"];
                superAdminAboutContent_ViewModel.AboutDescription = !string.IsNullOrEmpty(AboutDescriptionParam) ? AboutDescriptionParam.Trim() : string.Empty;

                // Check and set  Mission About Title 

                string OurMissionTitleParam = HttpRequest.Params["OurMissionTitle"];
                superAdminAboutContent_ViewModel.OurMissionTitle = !string.IsNullOrEmpty(OurMissionTitleParam) ? OurMissionTitleParam.Trim() : string.Empty;

                // Check and set  Mission About Description
                string MissiondescriptionParam = HttpRequest.Params["OurMissionDescription"];
                superAdminAboutContent_ViewModel.OurMissionDescription = !string.IsNullOrEmpty(MissiondescriptionParam) ? MissiondescriptionParam.Trim() : string.Empty;

                superAdminAboutContent_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files

                // Validate infromation passed
                Error_VM error_VM = superAdminAboutContent_ViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssImageFile = files["Image"];
                superAdminAboutContent_ViewModel.Image = _BusinesssImageFile; // for validation
                string _BusinessImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssImageFile);
                    }
                }

                if (superAdminAboutContent_ViewModel.Mode == 1)
                {
                    var respGetBusinessImageDetail = superAdminService.GetSuperAdminAboutDetail_Get(_LoginID_Exact);

                    if (respGetBusinessImageDetail != null && _BusinesssImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessImageDetail.Image == null)
                        {
                            _PreviousBusinessImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessImageFileNameGenerated = respGetBusinessImageDetail.Image;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }




                var resp = storedProcedureRepository.SP_InsertUpdateSuperAdminAboutDetail_Get<SPResponseViewModel>(new SP_InsertUpdateSuperAdminAboutContent_Param_VM
                {
                    Id = superAdminAboutContent_ViewModel.Id,
                    UserLoginId = _LoginID_Exact,
                    AboutTitle = superAdminAboutContent_ViewModel.AboutTitle,
                    AboutDescription = superAdminAboutContent_ViewModel.AboutDescription,
                    OurMissionTitle = superAdminAboutContent_ViewModel.OurMissionTitle,
                    OurMissionDescription = superAdminAboutContent_ViewModel.OurMissionDescription,
                    Image = _BusinessImageFileNameGenerated,
                    SubmittedByLoginId = _LoginID_Exact,
                    Mode = superAdminAboutContent_ViewModel.Mode
                });




                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Find  Image.
                    #region Insert-Update  Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminAboutImage), _PreviousBusinessImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminAboutImage), _BusinessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssImageFile, NewImageFileWithPath);
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
        /// To Get SuperAdmin About Detail By UserLoginId 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/SuperAdmin/GetSuperAdminAboutDetail")]
        public HttpResponseMessage GetSuperAdminAboutDetail()
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




                SuperAdminAboutDetail_VM resp = new SuperAdminAboutDetail_VM();
                resp = superAdminService.GetSuperAdminAboutDetail_Get(_LoginID_Exact);

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
        /// To Get About Detail For VisitorPanel 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SuperAdmin/GetSuperAdminAboutDetailForVisitorPanel")]
        public HttpResponseMessage GetSuperAdminAboutDetailForVisitorPanel()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                SuperAdminAboutDetail_VM resp = new SuperAdminAboutDetail_VM();
                resp = superAdminService.GetSuperAdminAboutDetailGet();

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

        #endregion ----------------------------------------------------------------------------
    }
}