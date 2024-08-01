using MasterZoneMvc.Models;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Common;
using System.Security.Claims;
using System.IO;
using MasterZoneMvc.Services;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Models.Enum;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System.Globalization;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Common.ValidationHelpers;
using static iTextSharp.text.pdf.AcroFields;
using System.Data.Entity;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessAdminAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessCategoryService businessCategoryService;
        private StudentService studentService;
        private CertificateService certificateService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;
        private EventService eventService;
        private UserService userService;
        private MasterProExtraInformationService masterProExtraInformationService;
        private MasterProResumeService masterProResumeService;



        public BusinessAdminAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessCategoryService = new BusinessCategoryService(db);
            studentService = new StudentService(db);
            businessOwnerService = new BusinessOwnerService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
            certificateService = new CertificateService(db);
            eventService = new EventService(db);
            userService = new UserService(db); 
            masterProExtraInformationService = new MasterProExtraInformationService(db);
            masterProResumeService = new MasterProResumeService(db);

        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get Weed Day Integral Value by WeekDay-Name
        /// </summary>
        /// <param name="enumString">Week-Day Name (Sunday,..)</param>
        /// <returns>Corresponding Integral value of the day</returns>
        private int GetWeekDayValue(string enumString)
        {
            WeekDays parsedEnum;
            if (Enum.TryParse(enumString, out parsedEnum))
            {
                return (int)parsedEnum;
            }
            return -1;
        }

        // GET: BusinessAdmin
        public IHttpActionResult Index()
        {
            return Ok();
        }
        // POST api/<LoginController>/Register

        // NOT IN USE
        [HttpPost]
        [Route("RegisterFull")]
        [AllowAnonymous]
        public IHttpActionResult RegisterFull(RequestRegisterBusinessOwner_VM requestRegisterBusinessOwner_VM)
        {
            try
            {
                if (requestRegisterBusinessOwner_VM == null)
                {
                    return Json(new ApiResponse_VM { status = -1, message = "Invalid Data!" });
                }
                else if (!requestRegisterBusinessOwner_VM.ValidInformation().Valid)
                {
                    return Json(new ApiResponse_VM { status = -1, message = requestRegisterBusinessOwner_VM.ValidInformation().Message });
                }

                // Add business service
                RegisterBusinessOwnerViewModel registerBusinessOwnerViewModel = new RegisterBusinessOwnerViewModel();

                registerBusinessOwnerViewModel.UserLogin = new MasterZoneMvc.Models.UserLogin()
                {
                    Email = requestRegisterBusinessOwner_VM.Email,
                    Password = requestRegisterBusinessOwner_VM.Password,
                    PhoneNumber = requestRegisterBusinessOwner_VM.PhoneNumber
                };

                registerBusinessOwnerViewModel.BusinessOwner = new MasterZoneMvc.Models.BusinessOwner()
                {
                    BusinessName = requestRegisterBusinessOwner_VM.BusinessName,
                    FirstName = requestRegisterBusinessOwner_VM.FirstName,
                    LastName = requestRegisterBusinessOwner_VM.LastName,
                    DOB = requestRegisterBusinessOwner_VM.DOB,
                    BusinessCategoryId = requestRegisterBusinessOwner_VM.BusinessCategoryId,
                    Address = requestRegisterBusinessOwner_VM.Address,
                };

                registerBusinessOwnerViewModel.BusinessDocuments = new List<MasterZoneMvc.Models.BusinessDocument>();

                //foreach(var document in requestRegisterBusinessOwner_VM.Files)
                //{
                //    registerBusinessOwnerViewModel.BusinessDocuments.Add(new Masterzone.Models.BusinessDocument()
                //    {
                //        DocumentPath = document.FileName
                //    });
                //}


                ////////////////////Commented Protolabz//////////////////////



                /*ServiceResponse_VM responseRegisterBusiness = _businessOwnerService.RegisterBusinessAdmin(registerBusinessOwnerViewModel);
    */
                ApiResponse_VM response = new ApiResponse_VM();
                //response.status = responseRegisterBusiness.Status;
                //response.message = responseRegisterBusiness.Message;
                return Ok(response);
            }

            catch (Exception ex)
            {
                return Json(new ApiResponse_VM { status = -500, message = Resources.ErrorMessage.InternalServerErrorMessage });
            }
            //return View();
        }


        /// <summary>
        /// Get All Students List linked with the BusinessOnwer
        /// </summary>
        /// <returns>List of All-Business-Students</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllStudents")]
        public HttpResponseMessage GetAllStudentListByBusiness()
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

                //List<UserListStudents_VM> lst = new List<UserListStudents_VM>();
                //SqlParameter[] queryParams = new SqlParameter[] {
                //            new SqlParameter("id", "0"),
                //            new SqlParameter("businessOwnerLoginId", "0"),
                //            new SqlParameter("userLoginId", "0"),
                //            new SqlParameter("mode", "1")
                //            };

                //var resp = db.Database.SqlQuery<UserListStudents_VM>("exec sp_ManageStudent @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

                List<StudentList_ForBusiness_VM> resp = new List<StudentList_ForBusiness_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("studentId", "0"),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "1"),
                            new SqlParameter("searchKeywords", "")
                            };

                resp = db.Database.SqlQuery<StudentList_ForBusiness_VM>("exec sp_ManageBusinessStudents @id,@businessOwnerId,@studentId,@userLoginId,@mode,@searchKeywords", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
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
        /// Get All Searched Students List linked with the BusinessOnwer
        /// </summary>
        /// <param name="searchKeywords">search keywords to search by</param>
        /// <returns>Searched Students List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/SearchStudents")]
        public HttpResponseMessage GetAllSearchedStudentListByBusiness(string searchKeywords)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                // validate and return if not valid
                if (String.IsNullOrEmpty(searchKeywords))
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Please enter search keywords";
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<StudentList_ForBusiness_VM> resp = new List<StudentList_ForBusiness_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("studentId", "0"),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("searchKeywords", searchKeywords)
                            };

                resp = db.Database.SqlQuery<StudentList_ForBusiness_VM>("exec sp_ManageBusinessStudents @id,@businessOwnerId,@studentId,@userLoginId,@mode,@searchKeywords", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
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

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/AddUpdate")]
        public HttpResponseMessage AddUpdateBusinessProfile()
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
                RequestBusinessProfile_VM requestBusinessProfile_VM = new RequestBusinessProfile_VM();
                //updateProfieSetting.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessProfile_VM.Email = HttpRequest.Params["Email"].Trim();
                requestBusinessProfile_VM.FirstName = HttpRequest.Params["FirstName"].Trim();
                requestBusinessProfile_VM.LastName = HttpRequest.Params["LastName"].Trim();
                requestBusinessProfile_VM.About = HttpRequest.Params["About"].Trim();
                requestBusinessProfile_VM.BusinessName = HttpRequest.Params["BusinessName"].Trim();
                requestBusinessProfile_VM.Address = HttpRequest.Params["Address"];
                requestBusinessProfile_VM.City = HttpRequest.Params["City"];
                requestBusinessProfile_VM.State = HttpRequest.Params["State"];
                requestBusinessProfile_VM.Country = HttpRequest.Params["Country"];
                requestBusinessProfile_VM.PhoneNumber = HttpRequest.Params["PhoneNumber"];
                requestBusinessProfile_VM.LandMark = HttpRequest.Params["LandMark"];
                requestBusinessProfile_VM.PinCode = HttpRequest.Params["PinCode"];
                requestBusinessProfile_VM.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLatitude"]) : 0;
                requestBusinessProfile_VM.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLongitude"]) : 0;
                requestBusinessProfile_VM.DocumentTitle = HttpRequest.Params["DocumentTitle"];
                requestBusinessProfile_VM.Mode = 1;
                requestBusinessProfile_VM.Privacy_UniqueUserId = Convert.ToInt32(HttpRequest.Params["Privacy_UniqueUserId"]);
                requestBusinessProfile_VM.Experience = HttpRequest.Params["Experience"];
                requestBusinessProfile_VM.OfficialWebSiteUrl = HttpRequest.Params["OfficialWebSiteUrl"];
                requestBusinessProfile_VM.FacebookProfileLink = HttpRequest.Params["FacebookProfileLink"];
                requestBusinessProfile_VM.InstagramProfileLink = HttpRequest.Params["InstagramProfileLink"];
                requestBusinessProfile_VM.LinkedInProfileLink = HttpRequest.Params["LinkedInProfileLink"];
                requestBusinessProfile_VM.TwitterProfileLink = HttpRequest.Params["TwitterProfileLink"];

                // Get Attatched Files

                HttpFileCollection files = HttpRequest.Files;
                //HttpPostedFile _businessImageFile = files["ProfileImage"]; // change name
                //requestBusinessProfile_VM.ProfileImage = _businessImageFile;

                //HttpPostedFile _businessLogoFile = files["BusinessLogo"]; // change name
                //requestBusinessProfile_VM.BusinessLogo = _businessLogoFile;// for validation

                HttpPostedFile _businessDocumentUploadedFile = files["DocumentUploadedFile"];
                requestBusinessProfile_VM.DocumentUploadedFile = _businessDocumentUploadedFile;

                //string _businessImageFileNameGenerated = ""; //will contains generated file name
                //string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.
                //string _businessLogoFileNameGenerated = "";
                //string _PreviousBusinessLogoImageFileName = ""; // will be used to delete file while updating.
                string _businessDocumentsUploadedFilesNameGenerated = "";

                // Validate infromation passed
                Error_VM error_VM = requestBusinessProfile_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                BusinessOwnerProfileViewModel respProfileData = new BusinessOwnerProfileViewModel();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<BusinessOwnerProfileViewModel>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();

                if (_businessDocumentUploadedFile != null && files.Count > 0)
                {
                    _businessDocumentsUploadedFilesNameGenerated = fileHelper.GenerateFileNameTimeStamp(_businessDocumentUploadedFile);
                }

                if (requestBusinessProfile_VM.DocumentTitle == null)
                {
                    requestBusinessProfile_VM.DocumentTitle = "";
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessProfile_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    FirstName = requestBusinessProfile_VM.FirstName,
                    LastName = requestBusinessProfile_VM.LastName,
                    Email = requestBusinessProfile_VM.Email,
                    About = requestBusinessProfile_VM.About,
                    Address = requestBusinessProfile_VM.Address,
                    Country = requestBusinessProfile_VM.Country,
                    State = requestBusinessProfile_VM.State,
                    City = requestBusinessProfile_VM.City,
                    PinCode = requestBusinessProfile_VM.PinCode,
                    LandMark = requestBusinessProfile_VM.LandMark,
                    Latitude = requestBusinessProfile_VM.Latitude,
                    Longitude = requestBusinessProfile_VM.Longitude,
                    PhoneNumber = requestBusinessProfile_VM.PhoneNumber,
                    DocumentTitle = requestBusinessProfile_VM.DocumentTitle,
                    Mode = requestBusinessProfile_VM.Mode,
                    BusinessName = requestBusinessProfile_VM.BusinessName,
                    Experience = requestBusinessProfile_VM.Experience,
                    Privacy_UniqueUserId = requestBusinessProfile_VM.Privacy_UniqueUserId,
                    OfficialWebSiteUrl = requestBusinessProfile_VM.OfficialWebSiteUrl,
                    FacebookProfileLink = requestBusinessProfile_VM.FacebookProfileLink,
                    LinkedInProfileLink = requestBusinessProfile_VM.LinkedInProfileLink,
                    InstagramProfileLink = requestBusinessProfile_VM.InstagramProfileLink,
                    TwitterProfileLink = requestBusinessProfile_VM.TwitterProfileLink
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                #region Insert-Update [Business Profile Image] or [Business Logo] on Server
                if (files.Count > 0)
                {

                    // TODO: [IC-Manisha] Integrated code(Manisha) only save feature no remove. Verify. on Profile update documents
                    if (_businessDocumentUploadedFile != null)
                    {
                        string fileWithPathDocument = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_Business), _businessDocumentsUploadedFilesNameGenerated);
                        fileHelper.SaveUploadedFile(_businessDocumentUploadedFile, fileWithPathDocument);
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
        /// Add-Update Business profile-image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/AddUpdateProfileImage")]
        public HttpResponseMessage AddUpdateBusinessProfileImage()
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
                RequestBusinessProfile_VM requestBusinessProfile_VM = new RequestBusinessProfile_VM();
                requestBusinessProfile_VM.Mode = 2;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _businessImageFile = files["ProfileImage"]; // change name
                requestBusinessProfile_VM.ProfileImage = _businessImageFile;


                string _businessImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestBusinessProfile_VM.VaildInformationProfileImage();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                BusinessOwnerProfileViewModel respProfileData = new BusinessOwnerProfileViewModel();

                // Get Business Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<BusinessOwnerProfileViewModel>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();


                if (_businessImageFile != null && files.Count > 0)
                {
                    _businessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_businessImageFile);

                    _PreviousProfileImageFileName = respProfileData.ProfileImage;
                }
                else
                {
                    _businessImageFileNameGenerated = respProfileData.ProfileImage;
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessProfile_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    ProfileImage = _businessImageFileNameGenerated,
                    Mode = requestBusinessProfile_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                #region Insert-Update [Business Profile Image] or [Business Logo] on Server
                if (files.Count > 0)
                {
                    // if business Profile Image passed then Add-Update Profile Image
                    if (_businessImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessProfileImage), _businessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_businessImageFile, fileWithPathImage);
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
        /// Add-Update Business profile cover-image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/AddUpdateCoverImage")]
        public HttpResponseMessage AddUpdateBusinessCoverImage()
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
                RequestBusinessProfile_VM requestBusinessProfile_VM = new RequestBusinessProfile_VM();
                requestBusinessProfile_VM.Mode = 5; // for Cover-Image update

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _businessImageFile = files["CoverImage"]; // change name
                requestBusinessProfile_VM.ProfileImage = _businessImageFile;


                string _businessImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousCoverImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestBusinessProfile_VM.VaildInformationProfileImage();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                BusinessOwnerProfileViewModel respProfileData = new BusinessOwnerProfileViewModel();

                // Get Business Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<BusinessOwnerProfileViewModel>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();


                if (_businessImageFile != null && files.Count > 0)
                {
                    _businessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_businessImageFile);

                    _PreviousCoverImageFileName = respProfileData.CoverImage;
                }
                else
                {
                    _businessImageFileNameGenerated = respProfileData.CoverImage;
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessProfile_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    CoverImage = _businessImageFileNameGenerated,
                    Mode = requestBusinessProfile_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                #region Insert-Update [Business Cover Image] or [Business Logo] on Server
                if (files.Count > 0)
                {
                    // if business Profile Image passed then Add-Update Profile Image
                    if (_businessImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousCoverImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessCoverImage), _PreviousCoverImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessCoverImage), _businessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_businessImageFile, fileWithPathImage);
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
        /// Add-Update business logo
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/AddUpdateBusinessLogo")]
        public HttpResponseMessage AddUpdateBusinessLogo()
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
                RequestBusinessProfile_VM requestBusinessProfile_VM = new RequestBusinessProfile_VM();

                requestBusinessProfile_VM.Mode = 3;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _businessLogoFile = files["BusinessLogo"]; // change name
                requestBusinessProfile_VM.BusinessLogo = _businessLogoFile;// for validation

                string _businessLogoFileNameGenerated = "";
                string _PreviousBusinessLogoImageFileName = ""; // will be used to delete file while updating.


                // Validate infromation passed
                Error_VM error_VM = requestBusinessProfile_VM.VaildInformationBusinessLogo();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                BusinessOwnerProfileViewModel respProfileData = new BusinessOwnerProfileViewModel();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<BusinessOwnerProfileViewModel>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();

                if (_businessLogoFile != null && files.Count > 0)
                {
                    _businessLogoFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_businessLogoFile);

                    _PreviousBusinessLogoImageFileName = respProfileData.BusinessLogo;
                }
                else
                {
                    _businessLogoFileNameGenerated = respProfileData.BusinessLogo;
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessProfile_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    BusinessLogo = _businessLogoFileNameGenerated,
                    Mode = requestBusinessProfile_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                #region Insert-Update [Business Profile Image] or [Business Logo] on Server
                if (files.Count > 0)
                {
                    // if business logo passed then Add-Update Logo Image

                    if (_businessLogoFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessLogoImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousBusinessLogoImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathLogo = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessLogo), _businessLogoFileNameGenerated);
                        fileHelper.SaveUploadedFile(_businessLogoFile, fileWithPathLogo);
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

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/Get")]
        public HttpResponseMessage GetBusinessProfileData()
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

                BusinessOwnerProfileViewModel respProfileData = new BusinessOwnerProfileViewModel();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<BusinessOwnerProfileViewModel>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();

                var businessProfilePageTypeDetail = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);

                respProfileData.ProfilePageTypeKey = businessProfilePageTypeDetail.Key;
                respProfileData.BusinessOwnerLoginId = _BusinessOwnerLoginId;

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
        /// Get All Business-Owners List By Category With Pagination
        /// </summary>
        /// <param name="categoryId">Business Category Id</param>
        /// <param name="lastRecordId">Last Fecthed Business-Owner-Id</param>
        /// <returns>List of Business-Owners</returns>
        [HttpGet]
        [Route("api/Business/GetAllByCategory")]
        public HttpResponseMessage GetAllBusinesListByCategory(long categoryId, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var businessCategory = businessCategoryService.GetBusinessCategoryById(categoryId);

                // return if category not found
                if (businessCategory == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidBusinessCategory;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = 0;
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status > 0)
                {
                    _LoginID_Exact = validateResponse.UserLoginId;
                }


                int _Mode = (businessCategory.ParentBusinessCategoryId == 0) ? 1 : 2;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessCategoryId", categoryId),
                            new SqlParameter("lastRecordId", lastRecordId),
                            new SqlParameter("recordLimit", recordLimit),
                            new SqlParameter("mode", _Mode),
                            new SqlParameter("userLoginId", _LoginID_Exact)
                            };

                var resp = db.Database.SqlQuery<BusinessOwnerList_VM>("exec sp_GetBusinessOwnersByCategory @id,@businessCategoryId,@lastRecordId,@recordLimit,@mode,@userLoginId", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
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

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/GetBasicProfileDetail")]
        public HttpResponseMessage GetBasicProfileDetailByBusiness()
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


                // List<BasicProfileDetail_VM> lst = new List<BasicProfileDetail_VM>();
                BasicProfileDetail_VM basicProfileDetail_VM = new BasicProfileDetail_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<BasicProfileDetail_VM>("exec  sp_ManageBusiness @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;
                //}

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
        /// Delete Business Owner By Id
        /// </summary>
        /// <param name="id">Business-Owner-Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Delete/{id}")]
        public HttpResponseMessage DeleteBusinessById(Int64 id)
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
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    Mode = 2
                });

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


        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessOwnerList")]
        public HttpResponseMessage GetAllBusinessOwnerListByBusiness()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                /* if (validateResponse.ApiResponse_VM.status < 0)
                 {
                     return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                 }*/

                long _LoginID_Exact = validateResponse.UserLoginId;

                List<BusinessOwnerList_VM> lst = new List<BusinessOwnerList_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId","0"),
                            new SqlParameter("UserLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                lst = db.Database.SqlQuery<BusinessOwnerList_VM>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = lst;
                //}

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin")]
        //[Route("api/Business/BusinessDetail/{id}")]
        [Route("api/Business/GetBusinessDetailById/{id}")]
        //[Route("api/Business/GetBusinessDetailById")] // changed by meena to this check.
        public HttpResponseMessage GetBusinessDetailByBusinessOwnerId(Int64 id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                /* if (validateResponse.ApiResponse_VM.status < 0)
                 {
                     return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                 }*/

                //long _LoginID_Exact = validateResponse.UserLoginId;

                List<BusinessOwnerList_VM> businessOwnerList = new List<BusinessOwnerList_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId","0"),
                            new SqlParameter("UserLoginId", "0"),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<BusinessOwnerList_VM>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;
                //}

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
        /// Search Business Owners by Name or location
        /// Name: Search keywords and SearchBy required.
        /// 
        /// Location: 
        /// By Current-Location: Latitude & Longitude required and SearchBy: "currentLocation"
        /// By Location-Search: Search-Keywords required and SearchBy: "Location"
        /// </summary>
        /// <param name="BusinessSearchParams"></param>
        /// <returns>Searched List of Business-Owners</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Business/GetAllBySearch")]
        public HttpResponseMessage GetAllBusinesListBySearch(RequestBusinessSearchViewModel BusinessSearchParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // return if invalid information
                Error_VM error_VM = BusinessSearchParams.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                int _Mode = 0;
                int _NearbySearchRadius = 20; // In KiloMeters
                BusinessSearchParams.SearchBy = BusinessSearchParams.SearchBy.ToLower();
                if (BusinessSearchParams.SearchBy == "name")
                    _Mode = 1;
                else if (BusinessSearchParams.SearchBy == "category")
                    _Mode = 2;
                else if (BusinessSearchParams.SearchBy == "location")
                    _Mode = 3;
                else if (BusinessSearchParams.SearchBy == "currentlocation")
                    _Mode = 4;

                if (String.IsNullOrEmpty(BusinessSearchParams.Latitude) || String.IsNullOrEmpty(BusinessSearchParams.Longitude))
                {
                    BusinessSearchParams.Latitude = "0";
                    BusinessSearchParams.Longitude = "0";
                }

                long _LoginID_Exact = 0;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessCategoryId", BusinessSearchParams.BusinessCategoryId),
                            new SqlParameter("searchKeyword", BusinessSearchParams.SearchKeyword),
                            new SqlParameter("latitude", BusinessSearchParams.Latitude),
                            new SqlParameter("longitude", BusinessSearchParams.Longitude),
                            new SqlParameter("radius", _NearbySearchRadius),
                            new SqlParameter("lastRecordId", BusinessSearchParams.LastRecordId),
                            new SqlParameter("recordLimit", BusinessSearchParams.RecordLimit),
                            new SqlParameter("mode", _Mode),
                            new SqlParameter("userLoginId", _LoginID_Exact)
                            };

                var resp = db.Database.SqlQuery<BusinessOwnerList_VM>("exec sp_GetBusinessOwnersBySearch @id,@businessCategoryId,@searchKeyword,@latitude,@longitude,@radius,@lastRecordId,@recordLimit,@mode,@userLoginId", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
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
        /// Search Business Owners by Name or location With User-Favourites [Authorized api]
        /// Name: Search keywords and SearchBy required.
        /// 
        /// Location: 
        /// By Current-Location: Latitude & Longitude required and SearchBy: "currentLocation"
        /// By Location-Search: Search-Keywords required and SearchBy: "Location"
        /// </summary>
        /// <param name="BusinessSearchParams"></param>
        /// <returns>Searched List of Business-Owners</returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Business/GetAllBySearchWithFavourites")]
        public HttpResponseMessage GetAllBusinesListBySearchWithFavourites(RequestBusinessSearchViewModel BusinessSearchParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // return if invalid information
                Error_VM error_VM = BusinessSearchParams.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                int _Mode = 0;
                int _NearbySearchRadius = 20; // In KiloMeters
                BusinessSearchParams.SearchBy = BusinessSearchParams.SearchBy.ToLower();
                if (BusinessSearchParams.SearchBy == "name")
                    _Mode = 1;
                else if (BusinessSearchParams.SearchBy == "category")
                    _Mode = 2;
                else if (BusinessSearchParams.SearchBy == "location")
                    _Mode = 3;
                else if (BusinessSearchParams.SearchBy == "currentlocation")
                    _Mode = 4;

                if (String.IsNullOrEmpty(BusinessSearchParams.Latitude) || String.IsNullOrEmpty(BusinessSearchParams.Longitude))
                {
                    BusinessSearchParams.Latitude = "0";
                    BusinessSearchParams.Longitude = "0";
                }

                long _LoginID_Exact = 0;
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status > 0)
                {
                    _LoginID_Exact = validateResponse.UserLoginId;
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessCategoryId", BusinessSearchParams.BusinessCategoryId),
                            new SqlParameter("searchKeyword", BusinessSearchParams.SearchKeyword),
                            new SqlParameter("latitude", BusinessSearchParams.Latitude),
                            new SqlParameter("longitude", BusinessSearchParams.Longitude),
                            new SqlParameter("radius", _NearbySearchRadius),
                            new SqlParameter("lastRecordId", BusinessSearchParams.LastRecordId),
                            new SqlParameter("recordLimit", BusinessSearchParams.RecordLimit),
                            new SqlParameter("mode", _Mode),
                            new SqlParameter("userLoginId", _LoginID_Exact)
                            };

                var resp = db.Database.SqlQuery<BusinessOwnerList_VM>("exec sp_GetBusinessOwnersBySearch @id,@businessCategoryId,@searchKeyword,@latitude,@longitude,@radius,@lastRecordId,@recordLimit,@mode,@userLoginId", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
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
        /// Get All Students with Instructor and Class By Business [Jquery DataTable Pagination]
        /// </summary>
        /// <returns>
        ///     DataTable Pagination List Response
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin, Staff")]
        [Route("api/Business/GetStudentListWithClassStudent_ByPagination")]

        public HttpResponseMessage GetAllInstructorByStudentForDataTablePagination()
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

                StudentList_Pagination_SQL_Params_VM _Params_VM = new StudentList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = studentService.GetBusinessStudentListWithClassInstructor_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Student Booked/Joined Class List of Business [Jquery DataTable Pagination]
        /// </summary>
        /// <returns>
        ///     DataTable Pagination List Response
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetStudentClassesListForBusiness_ByPagination")]

        public HttpResponseMessage GetStudentClassesListForBusiness_DataTablePagination(long studentLoginId)
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

                StudentList_Pagination_SQL_Params_VM _Params_VM = new StudentList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 2;
                _Params_VM.LoginId = studentLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = studentService.GetBusinessStudentClassesList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Student Booked/Joined Plan List of Business [Jquery DataTable Pagination]
        /// </summary>
        /// <returns>
        ///     DataTable Pagination List Response
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetStudentPlanListForBusiness_ByPagination")]

        public HttpResponseMessage GetStudentPlanListForBusiness_DataTablePagination(long studentLoginId)
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

                StudentList_Pagination_SQL_Params_VM _Params_VM = new StudentList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                //_Params_VM.Mode = 3;
                _Params_VM.LoginId = studentLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = studentService.GetBusinessStudentPlanList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Student Booked/Joined Event List of Business [Jquery DataTable Pagination]
        /// </summary>
        /// <returns>
        ///     DataTable Pagination List Response
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetStudentEventListForBusiness_ByPagination")]
        public HttpResponseMessage GetStudentEventListForBusiness_DataTablePagination(long studentLoginId)
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

                StudentList_Pagination_SQL_Params_VM _Params_VM = new StudentList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                //_Params_VM.Mode = 3;
                _Params_VM.LoginId = studentLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = studentService.GetBusinessStudentEventList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Student Booked/Joined Training List of Business [Jquery DataTable Pagination]
        /// </summary>
        /// <returns>
        ///     DataTable Pagination List Response
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetStudentTrainingListForBusiness_ByPagination")]
        public HttpResponseMessage GetStudentTrainingListForBusiness_DataTablePagination(long studentLoginId)
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

                StudentList_Pagination_SQL_Params_VM _Params_VM = new StudentList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                //_Params_VM.Mode = 3;
                _Params_VM.LoginId = studentLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = studentService.GetBusinessStudentTrainingList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Business Documents by Busienss Login id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/UploadedDocumets")]
        public HttpResponseMessage GetBusinessProfileUploadedDocumets()
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

                List<BusinessOwnerProfileUploadedDocuments> respProfileDocuments = new List<BusinessOwnerProfileUploadedDocuments>();
                // Get Business Uploaded documents
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "2")
                            };

                respProfileDocuments = db.Database.SqlQuery<BusinessOwnerProfileUploadedDocuments>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respProfileDocuments;

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
        /// Add Business Owner documents
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/Profile/AddBusinessOwnerDocuments")]
        public HttpResponseMessage AddBusinessOwnerDocuments()
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
                RequestBusinessProfile_VM requestBusinessProfile_VM = new RequestBusinessProfile_VM();

                requestBusinessProfile_VM.DocumentTitle = HttpRequest.Params["DocumentTitle"];
                requestBusinessProfile_VM.Mode = 4;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _businessDocumentUploadedFile = files["DocumentUploadedFile"];
                requestBusinessProfile_VM.DocumentUploadedFile = _businessDocumentUploadedFile;

                string _businessDocumentsUploadedFilesNameGenerated = "";

                // Validate infromation passed
                Error_VM error_VM = requestBusinessProfile_VM.VaildInformationUploadingBusinessOwnerDocuments();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (_businessDocumentUploadedFile != null && files.Count > 0)
                {
                    _businessDocumentsUploadedFilesNameGenerated = fileHelper.GenerateFileNameTimeStamp(_businessDocumentUploadedFile);
                }


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessProfile_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    DocumentTitle = requestBusinessProfile_VM.DocumentTitle,
                    DocumentUploadedFile = _businessDocumentsUploadedFilesNameGenerated,
                    Mode = requestBusinessProfile_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Documents
                #region Insert-Update [Business-Documents] on Server
                if (files.Count > 0)
                {

                    if (_businessDocumentUploadedFile != null)
                    {
                        string fileWithPathDocument = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_Business), _businessDocumentsUploadedFilesNameGenerated);
                        fileHelper.SaveUploadedFile(_businessDocumentUploadedFile, fileWithPathDocument);
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



        #region Business-Owner-Profile Details APIs for Visitor View -------------------------------------------------

        /// <summary>
        /// [NOT IN USE-- splitted it]
        /// Get Business Profile Details for Visitor-Panel/User - Mobile App 
        /// </summary>
        /// <returns>Business Profile-Page Details Data</returns>
        [HttpGet]
        [Route("api/Business/Profile/DetailsForVisitorView")]
        public HttpResponseMessage GetBusinessProfileDetailForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Busienss-Owner-Details
                var _BusinessBasicProfileDetail = businessOwnerService.GetBusinessOwnerBasicProfilePageForVisitorDataById(businessOwnerLoginId);

                //Get Business-Content-Services
                //var _BusinessContentServices = businessOwnerService.GetBusinessContentServiceListForVisitor(businessOwnerLoginId);
                var _BusinessContentServices = businessOwnerService.GetBusinessServiceDetailList(businessOwnerLoginId);

                //Get Business-Content-Videos
                var _BusinessContentVideos = businessOwnerService.GetAllBusinessContentVideosList(businessOwnerLoginId);

                //Get Business-Active-Plans
                var _BusinessActivePlans = new PlanService(db).GetAllActiveBusinessPlans(businessOwnerLoginId);

                //Get Business-Upcoming-Events
                var _UserCurrentDateTime = DateTime.Now; // IST
                var _UserCurrentTimezoneOffset = "";
                var _BusinessUpcomingEvents = businessOwnerService.GetUpcomingEventsForBusinessProfile(businessOwnerLoginId, _UserCurrentTimezoneOffset, 10);

                // Get Busieness-Active-Classes 
                var _BusinessActiveClasses = businessOwnerService.GetAllActiveBusinessClassesForVisitor(businessOwnerLoginId);

                //Extract Business-Online-Classes
                var _BusinessActiveClasses_OnlineOnly = _BusinessActiveClasses.Where(c => c.ClassMode == "Online").ToList();

                //Extract Business-Offline-Classes
                var _BusinessActiveClasses_OfflineOnly = _BusinessActiveClasses.Where(c => c.ClassMode == "Offline").ToList();

                //Get-Related Business Owners List
                var _RelatedBusinessOwnersList = businessOwnerService.GetRelatedBusiness(businessOwnerLoginId, 6);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    businessBasicProfileDetail = _BusinessBasicProfileDetail,
                    businessContentServices = _BusinessContentServices, // done
                    businessContentVideos = _BusinessContentVideos, //
                    businessActivePlans = _BusinessActivePlans,
                    businessOnlineClasses = _BusinessActiveClasses_OnlineOnly,
                    businessOfflineClasses = _BusinessActiveClasses_OfflineOnly,
                    RelatedBusinessList = _RelatedBusinessOwnersList
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
        /// Get Business Basic-Profile Details for Visitor-Panel/User - Mobile App 
        /// </summary>
        /// <returns>Business Basic-Profile-Page Details Data</returns>
        [HttpGet]
        [Route("api/Business/BasicProfilePageDetailsForVisitorView")]
        public HttpResponseMessage GetBusinessBasicProfileDetailForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Busienss-Owner-Details
                var _BusinessBasicProfileDetail = businessOwnerService.GetBusinessOwnerBasicProfilePageForVisitorDataById(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessBasicProfileDetail;

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
        /// Get Business Content-Services List for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business Content-Services-List Details Data</returns>
        [HttpGet]
        [Route("api/Business/GetContentServicesListForVisitorView")]
        public HttpResponseMessage GetBusinessContentServicesListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Business-Content-Services
                //var _BusinessContentServices = businessOwnerService.GetBusinessContentServiceListForVisitor(businessOwnerLoginId);
                var _BusinessContentServices = businessOwnerService.GetBusinessServiceDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessContentServices;

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
        /// Get Business Content-Videos List for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business Content-Videos-List Details Data</returns>
        [HttpGet]
        [Route("api/Business/GetContentVideosListForVisitorView")]
        public HttpResponseMessage GetBusinessContentVideosListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Business-Content-Videos
                var _BusinessContentVideos = businessOwnerService.GetAllBusinessContentVideosList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessContentVideos;

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
        /// Get Business Active-Plans List for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business Active-Plans-List Data</returns>
        [HttpGet]
        [Route("api/Business/GetActivePlansList")]
        public HttpResponseMessage GetBusinessActivePlansListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Business-Active-Plans
                var _BusinessActivePlans = new PlanService(db).GetAllActiveBusinessPlans(businessOwnerLoginId);
                var _BusinessadvancePlans = new PlanService(db).GetAllAdvanceBusinessPlans(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessActivePlans = _BusinessActivePlans,
                    BusinessadvancePlans = _BusinessadvancePlans
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
        /// Get Business Upcoming-Events List for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business Upcoming-Events-List Data</returns>
        [HttpGet]
        [Route("api/Business/GetUpcomingEventsList")]
        public HttpResponseMessage GetBusinessUpcomingEventsListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Business-Upcoming-Events
                var _UserCurrentDateTime = DateTime.Now; // IST
                var _UserCurrentTimezoneOffset = "";
                var _BusinessUpcomingEvents = businessOwnerService.GetUpcomingEventsForBusinessProfile(businessOwnerLoginId, _UserCurrentTimezoneOffset, 10);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessUpcomingEvents;

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
        /// Get Business Online-Class-List for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business Online-Class-List Data</returns>
        [HttpGet]
        [Route("api/Business/GetActiveOnlineClassList")]
        public HttpResponseMessage GetBusinessOnlineClassListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Busieness-Active-Classes 
                var _BusinessActiveClasses = businessOwnerService.GetAllActiveBusinessClassesForVisitor(businessOwnerLoginId);

                //Extract Business-Online-Classes
                var _BusinessActiveClasses_OnlineOnly = _BusinessActiveClasses.Where(c => c.ClassMode == "Online").ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessActiveClasses_OnlineOnly;

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
        /// Get Business Offline-Class-List for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business Offline-Class-List Data</returns>
        [HttpGet]
        [Route("api/Business/GetActiveOfflineClassList")]
        public HttpResponseMessage GetBusinessOfflineClassListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Busieness-Active-Classes 
                var _BusinessActiveClasses = businessOwnerService.GetAllActiveBusinessClassesForVisitor(businessOwnerLoginId);

                //Extract Business-Offline-Classes
                var _BusinessActiveClasses_OfflineOnly = _BusinessActiveClasses.Where(c => c.ClassMode == "Offline").ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessActiveClasses_OfflineOnly;

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
        /// Get Similar related Business-List by Business-Login-Id for Visitor-Panel/User - Mobile App
        /// </summary>
        /// <returns>Business-List Data</returns>
        [HttpGet]
        [Route("api/Business/GetRelatedBusinessList")]
        public HttpResponseMessage GetRelatedBusinessListForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get-Related Business Owners List
                var _RelatedBusinessOwnersList = businessOwnerService.GetRelatedBusiness(businessOwnerLoginId, 6);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _RelatedBusinessOwnersList;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion -------------------------------------------------------------------------------------

        #region Business Timimg -------------------------------------------------------------------------------
        /// <summary>
        /// Save Business-Timing-Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/Timing/AddUpdate")]
        public HttpResponseMessage AddUpdateBusinessTiming(List<BusinessTimingRequest_VM> requestBusinessTiming_VM)
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


                foreach (var _businessTiming in requestBusinessTiming_VM)
                {
                    DateTime openingTime;
                    DateTime closingTime;
                    DateTime openingTime2;
                    DateTime closingTime2;

                    if (_businessTiming.IsOpen == 1 && (string.IsNullOrEmpty(_businessTiming.OpeningTime) || string.IsNullOrEmpty(_businessTiming.ClosingTime) || string.IsNullOrEmpty(_businessTiming.OpeningTime2) || string.IsNullOrEmpty(_businessTiming.ClosingTime2)))
                    {
                        //--Create response
                        var objResponse = new
                        {
                            status = -1,
                            message = String.Format(Resources.BusinessPanel.BusinessTiming_OpeniningClosingTimeRequired_ErrorMessage, _businessTiming.DayName)
                        };

                        //sending response as OK
                        return Request.CreateResponse(HttpStatusCode.OK, objResponse);
                    }


                    else if (_businessTiming.IsOpen == 1)
                    {
                        openingTime = DateTime.ParseExact(_businessTiming.OpeningTime, "HH:mm", CultureInfo.InvariantCulture);
                        closingTime = DateTime.ParseExact(_businessTiming.ClosingTime, "HH:mm", CultureInfo.InvariantCulture);
                        openingTime2 = DateTime.ParseExact(_businessTiming.OpeningTime2, "HH:mm", CultureInfo.InvariantCulture);
                        closingTime2 = DateTime.ParseExact(_businessTiming.ClosingTime2, "HH:mm", CultureInfo.InvariantCulture);

                        if (closingTime <= openingTime && closingTime2 <= openingTime2)
                        {
                            var objResponse = new
                            {
                                status = -1,
                                message = String.Format(Resources.BusinessPanel.BusinessTiming_ClosingTimingLessThanOpening_ErrorMessage, _businessTiming.DayName)
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, objResponse);
                        }

                    }

                }

                SPResponseViewModel _resp = new SPResponseViewModel();
                foreach (var requestBusienssTiming in requestBusinessTiming_VM)
                {
                    string enumString = requestBusienssTiming.DayName.ToUpper();
                    int DayValue = GetWeekDayValue(enumString);

                    SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("dayName", requestBusienssTiming.DayName),
                                new SqlParameter("dayValue", DayValue),
                                new SqlParameter("isOpened", requestBusienssTiming.IsOpen),
                                new SqlParameter("openingTime_12HoursFormat", requestBusienssTiming.OpeningTime),
                                new SqlParameter("openingTime_24HoursFormat", requestBusienssTiming.OpeningTime),
                                new SqlParameter("closingTime_12HoursFormat", requestBusienssTiming.ClosingTime),
                                new SqlParameter("closingTime_24HoursFormat", requestBusienssTiming.ClosingTime),
                                
                                new SqlParameter("openingTime2_12HoursFormat", requestBusienssTiming.OpeningTime2),
                                new SqlParameter("openingTime2_24HoursFormat", requestBusienssTiming.OpeningTime2),
                                new SqlParameter("closingTime2_12HoursFormat", requestBusienssTiming.ClosingTime2),
                                new SqlParameter("closingTime2_24HoursFormat", requestBusienssTiming.ClosingTime2),
                                new SqlParameter("todayOff", requestBusienssTiming.TodayOff),
                                new SqlParameter("notes", requestBusienssTiming.Notes),
                                new SqlParameter("submittedByLoginId", _LoginID_Exact),
                                new SqlParameter("mode", "1")
                              };

                    _resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessTimings @id,@businessOwnerLoginId,@dayName,@dayValue,@isOpened,@openingTime_12HoursFormat,@openingTime_24HoursFormat,@closingTime_12HoursFormat,@closingTime_24HoursFormat," +
                        "@openingTime2_12HoursFormat,@openingTime2_24HoursFormat,@closingTime2_12HoursFormat,@closingTime2_24HoursFormat,@todayOff,@notes,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                }

                if (_resp.ret <= 0)
                {
                    apiResponse.status = _resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(_resp.resourceFileName, _resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(_resp.resourceFileName, _resp.resourceKey);
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
        /// Get business timings data by Business-Owner-Login-Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/Timing/GetBusinessTimingData")]
        public HttpResponseMessage GetBusinessTimingData()
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

                List<BusinessTiming_VM> respBusinessTimingData = new List<BusinessTiming_VM>();

                // Get Business Business-Timings Data
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "1")
                            };

                respBusinessTimingData = db.Database.SqlQuery<BusinessTiming_VM>("exec sp_ManageBusinessTimings @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    businessTimingData = respBusinessTimingData
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
        /// Get business timings data by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/Timing/GetBusinessTimingDataVisitorPanel")]
        public HttpResponseMessage GetBusinessTimingDataVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessTiming_VM> respBusinessTimingData = new List<BusinessTiming_VM>();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                            new SqlParameter("mode", "1")
                            };

                respBusinessTimingData = db.Database.SqlQuery<BusinessTiming_VM>("exec sp_ManageBusinessTimings @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    businessTimingData = respBusinessTimingData
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

        #endregion -----------------------------------------------------------------------------------

        #region Manage Business For Super-Admin -------------------------------------------------------------


        /// <summary>
        /// Get Business View Detail For SuperAdmin-Panel
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Business Detail with Documents</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, SubAdmin")]
        [Route("api/Business/BusinessDetailById")]
        public HttpResponseMessage BusinessDetailByBusinessOwnerIdForSuperAdmin(int businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                /* if (validateResponse.ApiResponse_VM.status < 0)
                 {
                     return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                 }*/

                long _LoginID_Exact = validateResponse.UserLoginId;

                BusinessDetail_VM businessdetail = new BusinessDetail_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", ""),
                            new SqlParameter("businessOwnerLoginId",businessOwnerLoginId),
                            new SqlParameter("UserLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "5")
                            };

                businessdetail = db.Database.SqlQuery<BusinessDetail_VM>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

                List<DocumentDetail_VM> documentDetail_VM = new List<DocumentDetail_VM>();
                if (businessdetail != null)
                {
                    SqlParameter[] _queryParams = new SqlParameter[] {
                                new SqlParameter("id", ""),
                                new SqlParameter("businessOwnerLoginId",businessOwnerLoginId),
                                new SqlParameter("businessOwnerId",businessdetail.Id),
                                new SqlParameter("UserLoginId", _LoginID_Exact),
                                new SqlParameter("mode", "1")
                                };

                    documentDetail_VM = db.Database.SqlQuery<DocumentDetail_VM>("exec sp_ManageBusinessDocuments @id,@businessOwnerLoginId,@businessOwnerId,@UserLoginId,@mode", _queryParams).ToList();
                }
                List<UserImageDetail_VM> businessImageDetailResponse = businessOwnerService.GetAllBusinessContentImages(businessOwnerLoginId);
                List<BusinessVideoResponse_VM> businessVedioDetailResponse = businessOwnerService.GetAllBusinessContentVideosList(businessOwnerLoginId);
                List<UserTrainingsDetail_VM> businessTrainingsDetailResponse = businessOwnerService.GetBusinessTrainingDetailById(businessOwnerLoginId);
                List<BusinessPlanDetail_VM> businessPlanDetailResponse = businessOwnerService.GetBusinessMainPlanBookingDetailsById(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessDetail = businessdetail,
                    BusinessDocuments = documentDetail_VM,
                    BusinessImageDetailResponse = businessImageDetailResponse,
                    BusinessVedioDetailResponse = businessVedioDetailResponse,
                    BusinessTrainingDetailResponse = businessTrainingsDetailResponse,
                    BusinessPlanDetailResponse = businessPlanDetailResponse

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
        /// To  Register User by SuperAdmin
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, SubAdmin")]
        [Route("api/Business/AddUpdateBySuperAdmin")]
        public HttpResponseMessage BusinessAddUpdateRegister()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                RegisterBusiness_VM registerBusinessAdminViewModel = new RegisterBusiness_VM();
                registerBusinessAdminViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                registerBusinessAdminViewModel.BusinessCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessCategoryId"]);
                registerBusinessAdminViewModel.BusinessSubCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessSubCategoryId"]);
                registerBusinessAdminViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                registerBusinessAdminViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                registerBusinessAdminViewModel.Email = HttpRequest.Params["Email"].Trim();
                registerBusinessAdminViewModel.Password = HttpRequest.Params["Password"].Trim();
                registerBusinessAdminViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                registerBusinessAdminViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                if (registerBusinessAdminViewModel.Mode == 3)
                {
                    registerBusinessAdminViewModel.Verified = Convert.ToInt32(HttpRequest.Params["Verified"]);
                }

                // Validate infromation passed
                Error_VM error_VM = registerBusinessAdminViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    Id = registerBusinessAdminViewModel.Id,
                    Email = registerBusinessAdminViewModel.Email,
                    Password = EDClass.Encrypt(registerBusinessAdminViewModel.Password),
                    PhoneNumber = registerBusinessAdminViewModel.PhoneNumber,
                    PhoneNumberCountryCode = "+91",
                    FirstName = registerBusinessAdminViewModel.FirstName,
                    LastName = registerBusinessAdminViewModel.LastName,
                    BusinessCategoryId = registerBusinessAdminViewModel.BusinessCategoryId,
                    BusinessSubCategoryId = registerBusinessAdminViewModel.BusinessSubCategoryId,
                    Mode = registerBusinessAdminViewModel.Mode,
                    Verified = registerBusinessAdminViewModel.Verified
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // If new account created then send email
                if (registerBusinessAdminViewModel.Mode == 1)
                {
                    EmailSender emailSender = new EmailSender();
                    emailSender.Send(registerBusinessAdminViewModel.FirstName + " " + registerBusinessAdminViewModel.LastName, "Registration successful", registerBusinessAdminViewModel.Email, "You have been successfully registered as business owner with Masterzone. Your MasterID is : " + resp.MasterId, "");
                }

                TokenGenerator tokenGenerator = new TokenGenerator();
                var _JWT_User_Token = tokenGenerator.Create_JWT(resp.Id, "BusinessAdmin", 0);

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { token = _JWT_User_Token };
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
        ///  Get  Business detail by Id For SuperAdmin-Panel
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns Business Detail</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/GetBusinessDetail/{id}")]
        public HttpResponseMessage GetByIdForSuperAdmin(int id)
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
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                long _LoginId = validateResponse.UserLoginId;
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", id),
                            new SqlParameter("userLoginId",_LoginId),
                            new SqlParameter("mode", "6")
                            };

                var resp = db.Database.SqlQuery<BusinessDetailUpdate_VM>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;
                if (resp != null)
                {
                    resp.Password = EDClass.Decrypt(resp.Password);
                    //if (resp.ParentBusinessCategoryId == 0)
                    //{
                    //    resp.ParentBusinessCategoryId = resp.BusinessCategoryId;
                    //    resp.SubBusinessCategoryId = 0;
                    //}
                    //else
                    //{
                    //    resp.ParentBusinessCategoryId = resp.ParentBusinessCategoryId;
                    //    resp.SubBusinessCategoryId = resp.BusinessCategoryId;
                    //}
                }

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
                }

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
        /// Get All ManageBusiness  Pagination For the SuperAdmin-Panel [Admin, Business]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/GetAllBusinessDetail_ByPagination")]

        public HttpResponseMessage GetAllBusinessOwnerForDataTablePagination()
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

                BusinessOwnerList_Pagination_SQL_Params_VM _Params_VM = new BusinessOwnerList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get All Busienss-Instructors Accounts Pagination For the SuperAdmin-Panel [Superadmin, subadmin]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/GetAllInstructorListForSuperAdmin_ByPagination")]

        public HttpResponseMessage GetAllInstructorListForSuperAdmin_ByPagination()
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

                BusinessOwnerList_Pagination_SQL_Params_VM _Params_VM = new BusinessOwnerList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.LoginId = _LoginId;

                var paginationResponse = businessOwnerService.GetInstructorListForSuperAdmin_Pagination(HttpRequestParams, _Params_VM);

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
        /// To change the Business/Instructor Home Visibility Status - [Super-Admin-Panel]
        /// </summary>
        /// <param name="id">Class-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/ToggleHomePageVisibilityStatus/{id}")]
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

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    Mode = 6
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
        /// Get All Instructors For Home Page display
        /// </summary>
        /// <returns>Instructors List</returns>
        [HttpGet]
        [Route("api/Business/GetAllInstructorsForHomePage")]
        public HttpResponseMessage GetAllCertificatesForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var response = businessOwnerService.GetAllInstructorListForHomePage();

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

        /// <summary>
        ///  To delete Manage Business detail by Id {soft delete}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/BusinessDetailDelete/{id}")]
        public HttpResponseMessage DeleteBusinessDetailById(Int64 id)
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
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    Mode = 4
                });

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
        ///  Toggle the status of Business-Account Accept/Reject by SuperAdmin
        /// </summary>
        /// <param name="id">Business-Owner-Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/ToggleBusinessAccountAcceptedStatus/{id}")]
        public HttpResponseMessage ToggleBusinessAccountAcceptedStatus(long id)
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
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    Mode = 7
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
        ///  Toggle the status of Business-Prime-Member status by SuperAdmin
        /// </summary>
        /// <param name="id">Business-Owner-Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/ToggleBusinessPrimeMemberStatus/{id}")]
        public HttpResponseMessage ToggleBusinessPrimeMemberStatus(long id)
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
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    Mode = 8
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
        /// Get All List Main Plan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/GetAllListMainPlan")]
        public HttpResponseMessage GetAllListSuperAdminMainPlan()
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
        /// Delte the Business Content-Image By SuperAdmin
        /// </summary>
        /// <param name="id">Image-Id</param>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>status 1 if deleted, else -ve status with message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/Business/DeleteBusinessContentImageBySuperAdmin")]
        public HttpResponseMessage DetleteBusinessContentImageBySuperAdmin(long id, long UserLoginId)
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

                // Delete Content-Image
                resp = businessOwnerService.DeleteBusinessImage(id, UserLoginId);

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
        /// Delte the Business Content-Video By SuperAdmin
        /// </summary>
        /// <param name="id">Video-Id</param>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>status 1 if deleted, else -ve status with message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/Business/DeleteBusinessContentVideoBySuperAdmin")]
        public HttpResponseMessage DetleteBusinessContentVideoBySuperAdmin(long id, long UserLoginId)
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

                // Delete Content-Video
                resp = businessOwnerService.DeleteBusinessVedio(id, UserLoginId);

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
        /// Search Business Owners by Name or location
        /// Name: Search keywords and SearchBy required.
        /// 
        /// Location: 
        /// By Current-Location: Latitude & Longitude required and SearchBy: "currentLocation"
        /// By Location-Search: Search-Keywords required and SearchBy: "Location"
        /// </summary>
        /// <param name="BusinessSearchParams"></param>
        /// <returns>Searched List of Business-Owners</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin,BusinessAdmin,Staff,Student")]
        [Route("api/Business/GetAllBySearchForSuperAdmin")]
        public HttpResponseMessage GetAllBusinesListBySearchForSuperAdmin(RequestBusinessSearch_ForSuperAdmin_VM BusinessSearchParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                // return if invalid information
                Error_VM error_VM = BusinessSearchParams.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                int _Mode = 1;
                long _LoginID_Exact = validateResponse.UserLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("certificateId", BusinessSearchParams.CertificateId),
                            new SqlParameter("businessCategoryId", "0"),
                            new SqlParameter("searchKeyword", BusinessSearchParams.SearchKeyword),
                            new SqlParameter("lastRecordId", BusinessSearchParams.LastRecordId),
                            new SqlParameter("recordLimit", BusinessSearchParams.RecordLimit),
                            new SqlParameter("mode", _Mode),
                            };

                var resp = db.Database.SqlQuery<BusinessSearch_ForSuperAdmin_VM>("exec sp_GetBusinessOwnersBySearch_SuperAdmin @id,@userLoginId,@certificateId,@businessCategoryId,@searchKeyword,@lastRecordId,@recordLimit,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
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
        /// Assign Or Un-Assign Certificate to business
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Owner Login Id</param>
        /// <param name="certificateId">Certificate Id</param>
        /// <param name="isAssgin">pass 1 for Assign else 0 to unassign</param>
        /// <returns>Assign or unassign Certificate to business</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/AssignOrUnassignBusinessCertificate")]
        public HttpResponseMessage AssignOrUnassignBusinessCertificate(long businessOwnerLoginId, long certificateId, int isAssign)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                // return if invalid information
                if (businessOwnerLoginId <= 0 || certificateId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = new SPResponseViewModel();
                if(isAssign == 1)
                {
                    resp = certificateService.AssignBusinessCertificate(businessOwnerLoginId, certificateId);
                }
                else
                {
                    resp = certificateService.UnAssignBusinessCertificate(businessOwnerLoginId, certificateId);
                }

                apiResponse.status = resp.ret;
                apiResponse.message = resp.responseMessage;
                apiResponse.data = null;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        #endregion -----------------------------------------------------------------------------------------

        /// To check the Profile Page type Record In Filter Id
        [HttpPost]
        //[Authorize(Roles = "Student")]
        [Route("api/Business/GetAllSportsBusinessList")]
        public HttpResponseMessage GetAllBusinessSports(BusinessContentDistanceParameter_VM businessContentDistanceParameter_VM, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                //double customer_Latitude = Convert.ToDouble(businessContentDistanceParameter_VM.Latitude);
                //double customer_Longitude = Convert.ToDouble(businessContentDistanceParameter_VM.Longitude);

                //BusinessOwnerList_VM businessOwnerList_VM = new BusinessOwnerList_VM();
                List<BusinessOwnerDetailList_VM> businessOwnerList_VM = businessOwnerService.Get_BusinessSportsDetailList(businessContentDistanceParameter_VM.MenuTag, businessContentDistanceParameter_VM.Latitude, businessContentDistanceParameter_VM.Longitude, businessContentDistanceParameter_VM.LastRecordedId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessOwnerList_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        /// <summary>
        /// Search Academy (B2B Businesses) with search-filters - FOR VISITOR PANEL
        /// </summary>
        /// <param name="filterParams">passed values for filteration</param>
        /// <returns>Filtered Academies List</returns>
        [HttpPost]
        [Route("api/Business/GetAllAcademiesBySearchFilter")]
        public HttpResponseMessage GetAllAcademiesBySearchFilter(SearchFilter_APIParmas_VM filterParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                //double customer_Latitude = Convert.ToDouble(businessContentDistanceParameter_VM.Latitude);
                //double customer_Longitude = Convert.ToDouble(businessContentDistanceParameter_VM.Longitude);

                //BusinessOwnerList_VM businessOwnerList_VM = new BusinessOwnerList_VM();
                List<BusinessOwnerDetailList_VM> businessOwnerList_VM = businessOwnerService.GetB2BBusinessesListBySearchFilter(filterParams);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessOwnerList_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        /// <summary>
        /// Search Instructors (Individual Businesses) with search-filters - FOR VISITOR Panel
        /// </summary>
        /// <param name="filterParams">contains values for filteration</param>
        /// <returns>Filtered Instructors List</returns>
        [HttpPost]
        [Route("api/Business/GetAllInstructorsBySearchFilter")]
        public HttpResponseMessage GetAllInstructorsBySearchFilter(SearchFilter_APIParmas_VM filterParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<InstructorList_VM> businessOwnerList_VM = businessOwnerService.GetInstructorsListBySearchFilter(filterParams);

                // Get All user certifications of instructor
                foreach(var instructor in businessOwnerList_VM)
                {
                    instructor.Certifications = certificateService.GetAllUserCertificates(instructor.InstructorUserLoginId);
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessOwnerList_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        /// <summary>
        /// To Get Business Instructor Detail For Dance 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllInstructors")]
        public HttpResponseMessage GetAllInstructorList(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<InstructorList_VM> instructorList = businessOwnerService.GetBusinessInstructorList(businessOwnerLoginId, 8);
                InstructorList_VM businessInstructorDetail = new InstructorList_VM();
                businessInstructorDetail = businessOwnerService.GetBusinessInstructorDetail(businessOwnerLoginId);
                ///----- Instructor list for instructor profile
                List<InstructorList_VM> othersInstructorList = businessOwnerService.GetBusinessInstructorList(businessOwnerLoginId, 13);
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    InstructorDetail = instructorList,
                    businessInstructorDetail = businessInstructorDetail,
                    OthersInstructorList = othersInstructorList
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
        /// To Get Business Timing Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Business/GetBusinessContactDetail")]
        public HttpResponseMessage GetBusinessTiming(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessContactDetail_VM businesstimingDetail = new BusinessContactDetail_VM();
                businesstimingDetail = businessOwnerService.GetBusinessTiming(businessOwnerLoginId);

                BusinessContentContactInformation_VM resp = new BusinessContentContactInformation_VM();
                resp = businessOwnerService.GetBusinessContactInformation(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessTimingDetail = businesstimingDetail,
                    BusinessContactInformation = resp
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
        /// Get Business Upcoming-Events List for Visitor-Panel/User
        /// </summary>
        /// <returns>Business Upcoming-Events-List Data</returns>
        [HttpGet]
        [Route("api/Business/GetUpcomingEventsListWithAdditionalData")]
        public HttpResponseMessage GetUpcomingEventsListWithAdditionalDataForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                List<BusinessContentEventOrganisationImagesDetail_VM> businessEventImageDetail = new List<BusinessContentEventOrganisationImagesDetail_VM>();
                //Get Business-Upcoming-Events
                var _UserCurrentDateTime = DateTime.Now; // IST
                var _UserCurrentTimezoneOffset = "";
                var _BusinessUpcomingEvents = businessOwnerService.GetUpcomingEventsForBusinessProfile(businessOwnerLoginId, _UserCurrentTimezoneOffset, 10);

                var eventId = _BusinessUpcomingEvents.FirstOrDefault();
                if (eventId == null)
                {

                }
                else
                {
                    //List<BusinessContentEventOrganisationImagesDetail_VM> businessEventImageDetail = eventService.GetBusinessEventImageDetaillst(businessOwnerLoginId, eventId.Id);
                    businessEventImageDetail = eventService.GetBusinessEventImageDetaillst(businessOwnerLoginId, eventId.Id);
                }


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessUpcomingEvents = _BusinessUpcomingEvents,
                    BusinessEventImageDetail = businessEventImageDetail

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
        /// Get Business Instructor Professional List 
        /// </summary>
        /// <returns>Business Instructor Professional List Data</returns>
        [HttpGet]
        [Route("api/Business/GetInstructorProfessionalList")]
        public HttpResponseMessage GetInstructorProfessionalListDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //Get Business-Professional
                List<InstructorProfessional_VM> instructorProfessionalList = businessOwnerService.GetBusinessInstructorProfessionalList(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = instructorProfessionalList;

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
        /// Add Business Content Videos Detail
        /// </summary>
        /// <returns>If Status 1 to add the vedio, then -1 then occur error</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContentVideo/AddUpdatePCCMeta_Detail")]
        public HttpResponseMessage AddUpdateVideos()
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
                BusinessContentVideos_PPCMetaViewModel businessContentVideos_PPCMetaViewModel = new BusinessContentVideos_PPCMetaViewModel();
                businessContentVideos_PPCMetaViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentVideos_PPCMetaViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessContentVideos_PPCMetaViewModel.VideoDescription = HttpRequest.Params["VideoDescription"].Trim();
                businessContentVideos_PPCMetaViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Validate infromation passed
                Error_VM error_VM = businessContentVideos_PPCMetaViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinesContentVideo<SPResponseViewModel>(new SP_InsertUpdateBusinessContentVideoPPCMeta_Params_VM
                {

                    Id = businessContentVideos_PPCMetaViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Title = businessContentVideos_PPCMetaViewModel.Title,
                    VideoDescription = businessContentVideos_PPCMetaViewModel.VideoDescription,
                    Mode = businessContentVideos_PPCMetaViewModel.Mode


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
        /// To Get Business Content Video Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContentVideo/GetVideosDetail")]
        public HttpResponseMessage GetBusinessContentVideo()
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


                BusinessContentVideos_PPCMetaDetail resp = new BusinessContentVideos_PPCMetaDetail();

                resp = businessOwnerService.GetBusinessContentVideoPPCMetaDetail(_BusinessOwnerLoginId);

                List<BusinessContentVideos_PPCMetaDetail> businessVideoList = businessOwnerService.GetBusinessContentVideoDetailList(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentVideo = resp,
                    BusinessContentVideoList = businessVideoList,
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
        /// To Get Business Content Video Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContentVideo/GetBusinessContentVideos")]
        public HttpResponseMessage GetBusinessContentVideos(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {



                BusinessContentVideos_PPCMetaDetail resp = new BusinessContentVideos_PPCMetaDetail();

                resp = businessOwnerService.GetBusinessContentVideoPPCMetaDetail(businessOwnerLoginId);

                List<BusinessContentVideos_PPCMetaDetail> businessVideoList = businessOwnerService.GetBusinessContentVideoDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentVideo = resp,
                    BusinessContentVideoList = businessVideoList,
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

        ////////////////////////////////////Professional Detail For Pages//////////////////////////
        ///
        /// <summary>
        /// To Add Business  Professional Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessProfessional")]

        public HttpResponseMessage AddUpdateBusinessProfessional()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //  BusinesssProfilePageType.Key = "yoga_web";
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentProfessional_VM businessContentProfessional_VM = new BusinessContentProfessional_VM();

                // Parse and assign values from HTTP request parameters
                businessContentProfessional_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentProfessional_VM.ProfessionalTitle = HttpRequest.Params["ProfessionalTitle"].Trim();
                businessContentProfessional_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentProfessional_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentProfessional_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentProfessional
                {
                    Id = businessContentProfessional_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    ProfessionalTitle = businessContentProfessional_VM.ProfessionalTitle,
                    Description = businessContentProfessional_VM.Description,
                    Mode = businessContentProfessional_VM.Mode
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
        /// To Get Professional Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/ProfessionalDetail/Get")]
        public HttpResponseMessage GetProfessionalDetailById()
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


                //var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //BusinesssProfilePageType.Key = "yoga_web";



                BusinessContentProfessionalDetail_VM businessReviewDetail = businessOwnerService.GetBusinessContentProfessionalDetail(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessReviewDetail;


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
        /// To Get Professional Detail By BusinessOwnerLoginId (Sports)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetProfessionalDetail")]
        public HttpResponseMessage GetProfessionalDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                BusinessContentProfessionalDetail_VM businessReviewDetail = businessOwnerService.GetBusinessContentProfessionalDetail(businessOwnerLoginId);
                //Get Business-Professional
                List<InstructorProfessional_VM> instructorProfessionalList = businessOwnerService.GetBusinessInstructorProfessionalList(businessOwnerLoginId);



                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentProfessionalDetail = businessReviewDetail,
                    BusinessContentProfessionalList = instructorProfessionalList,

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
        /// Get All List Business Content Sponsor by BusinessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/Sponsor/GetAllList")]
        public HttpResponseMessage GetAllBusinessSponsorLists(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _LoginId = validateResponse.UserLoginId;
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                List<BusinessContentSponsor_VM> businessContentSponsorList = businessOwnerService.GetBusinessContentSponsorList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessContentSponsorList;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        ////////////////////////////////////////////// Fitness Movement ///////////////////////////////////////////

        /// <summary>
        /// To Add Business  Fitness Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentFitnessMovement_PPCMeta")]

        public HttpResponseMessage AddUpdateBusinessFitnessMovementProfile()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //  BusinesssProfilePageType.Key = "yoga_web";
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentFitnessMovement_VM businessContentFitnessMovement_VM = new BusinessContentFitnessMovement_VM();

                // Parse and assign values from HTTP request parameters
                businessContentFitnessMovement_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentFitnessMovement_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentFitnessMovement_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentFitnessMovement_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Validate information passed



                Error_VM error_VM = businessContentFitnessMovement_VM.ValidInformation();




                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssFitnessImageFile = files["FitnessImage"];
                businessContentFitnessMovement_VM.FitnessImage = _BusinesssFitnessImageFile; // for validation
                string _BusinessFitnessImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessFitnessImageFileName = ""; // will be used to delete file while updating.


                if (files.Count > 0)
                {
                    if (_BusinesssFitnessImageFile != null)
                    {

                        _BusinessFitnessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssFitnessImageFile);
                    }

                }

                if (businessContentFitnessMovement_VM.Mode == 1)
                {
                    var respGetBusinessFitnessDetail = businessOwnerService.GetBusinessContentFitnessDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessFitnessDetail != null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessFitnessDetail.FitnessImage == null)
                        {
                            _PreviousBusinessFitnessImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _PreviousBusinessFitnessImageFileName = respGetBusinessFitnessDetail.FitnessImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessFitnessImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentFitness_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentFitness_Param_VM
                {
                    Id = businessContentFitnessMovement_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentFitnessMovement_VM.Title,
                    Description = businessContentFitnessMovement_VM.Description,
                    FitnessImage = _BusinessFitnessImageFileNameGenerated,
                    Mode = businessContentFitnessMovement_VM.Mode
                });




                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Class Image.
                    #region Insert-Update Fitness Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessFitnessImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessFitnessImage), _PreviousBusinessFitnessImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessFitnessImage), _BusinessFitnessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssFitnessImageFile, NewImageFileWithPath);


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
        /// To Get Business Content  Fitness Detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentFitnessMovementDetail_PPCMeta")]
        public HttpResponseMessage GetBusinessContentFitnessMovementDetail()
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


                BusinessContentFitnessMovementDetail_VM resp = new BusinessContentFitnessMovementDetail_VM();

                resp = businessOwnerService.GetBusinessContentFitnessDetail(_BusinessOwnerLoginId);



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

        /////////////////////////////////////////////////////////////////////////////To Add Full Fitness Movement Detail/////////////////

        ////////////////////////////////////////////// Fitness Movement ///////////////////////////////////////////

        /// <summary>
        /// To Add Business  Fitness  Movement Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentFitnessMovement")]

        public HttpResponseMessage AddUpdateBusinessFitnessProfile()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //  BusinesssProfilePageType.Key = "yoga_web";
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentFitnessMovementViewModel businessContentFitnessMovementViewModel = new BusinessContentFitnessMovementViewModel();

                // Parse and assign values from HTTP request parameters
                businessContentFitnessMovementViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentFitnessMovementViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessContentFitnessMovementViewModel.Requirements = HttpRequest.Params["Requirements"].Trim();
                businessContentFitnessMovementViewModel.Investment = HttpRequest.Params["Investment"].Trim();
                businessContentFitnessMovementViewModel.Inclusions = HttpRequest.Params["Inclusions"].Trim();
                businessContentFitnessMovementViewModel.Description = HttpRequest.Params["Description"].Trim();
                businessContentFitnessMovementViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Validate information passed


                Error_VM error_VM = businessContentFitnessMovementViewModel.ValidInformation();


                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentFitnessMovement_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentFitnessMovement_Param_VM
                {
                    Id = businessContentFitnessMovementViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentFitnessMovementViewModel.Title,
                    Requirements = businessContentFitnessMovementViewModel.Requirements,
                    Investment = businessContentFitnessMovementViewModel.Investment,
                    Inclusions = businessContentFitnessMovementViewModel.Inclusions,
                    Description = businessContentFitnessMovementViewModel.Description,
                    Mode = businessContentFitnessMovementViewModel.Mode
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
        /// To Get Business Content  Fitness Movement Detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentFitnessMovementDetail/ById/{id}")]
        public HttpResponseMessage GetBusinessContentFitnessMovementDetails(long Id)
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


                BusinessContentFitnessMovementDetailViewModel resp = new BusinessContentFitnessMovementDetailViewModel();

                resp = businessOwnerService.GetBusinessContentFitnessMovementDetail(Id);



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
        /// Delete World Class Program Detail
        /// </summary>
        /// <param name="id">WorldClassProgram Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/DeleteBusinessContentFitnessMovementDetail")]
        public HttpResponseMessage DeleteWorldClassProgramById(long id)
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
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Fitness Movement   Detail 
                resp = businessOwnerService.DeleteFitnessMovementDetail(id);


                //return resp;
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
        /// Get All Business Content Fitness Movement with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentFitnessMovementDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentFitnessMovementDataTablePagination()
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

                BusinessContentFitnessMovementDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentFitnessMovementDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessContentFitnessMovementList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Business Content  Fitness Detail To Show 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //  [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentFitnessMovement_PPCMeta")]
        public HttpResponseMessage GetBusinessContentFitnessMovementsDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {




                BusinessContentFitnessMovementDetail_VM resp = new BusinessContentFitnessMovementDetail_VM();

                resp = businessOwnerService.GetBusinessContentFitnessDetail(businessOwnerLoginId);

                List<BusinessContentFitnessMovementDetailViewModel> businessContentFitnessMovementList = businessOwnerService.GetBusinessContentFitnessMovementDetailList_Get(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentFitnessMovementDetail = resp,
                    BusinessContentFitnessMovementList = businessContentFitnessMovementList
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


        ////////////////////////////////////////////////////////////// Client Detail //////////////////////////////////////////////////////////


        /// <summary>
        /// To Add Business Client Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessClient")]
        public HttpResponseMessage AddUpdateBusinessClientProfile()
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
                BusinessContentClientViewModal businessContentClientViewModal = new BusinessContentClientViewModal();
                businessContentClientViewModal.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentClientViewModal.Name = HttpRequest.Params["Name"].Trim();
                businessContentClientViewModal.Description = HttpRequest.Params["Description"].Trim();
                businessContentClientViewModal.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssClientImageFile = files["ClientImage"];
                businessContentClientViewModal.ClientImage = _BusinesssClientImageFile; // for validation
                string _BusinessClientImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessClientImageFileName = ""; // will be used to delete file while updating.




                // Validate infromation passed
                Error_VM error_VM = businessContentClientViewModal.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {

                    if (_BusinesssClientImageFile != null)
                    {
                        _BusinessClientImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssClientImageFile);
                    }


                }


                if (businessContentClientViewModal.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessClientDetail_ById(businessContentClientViewModal.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessClientImageFileNameGenerated = respGetBusinessServiceData.ClientImage;

                    }
                    else
                    {
                        _PreviousBusinessClientImageFileName = respGetBusinessServiceData.ClientImage;

                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessClient_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentClient_Param_VM
                {
                    Id = businessContentClientViewModal.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Name = businessContentClientViewModal.Name,
                    Description = businessContentClientViewModal.Description,
                    ClientImage = _BusinessClientImageFileNameGenerated,
                    Mode = businessContentClientViewModal.Mode
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
                    #region Insert-Update client Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessClientImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentClientImage), _PreviousBusinessClientImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentClientImage), _BusinessClientImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssClientImageFile, NewImageFileWithPath);


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
        /// To Get Business Client Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessClientDetailById")]
        public HttpResponseMessage GetBusinessClientDetailById(long Id)
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


                BusinessContentClientDetail_VM resp = new BusinessContentClientDetail_VM();

                resp = businessOwnerService.GetBusinessClientDetail_ById(Id);


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
        /// Delete Sponsor by SponsorId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteClientDetailById")]
        public HttpResponseMessage DeleteClientById(long Id)
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

                // Delete Business client 
                resp = businessOwnerService.DeleteBusinessClientById(Id);


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
        /// Get All Business Sponsor with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentClientByPagination")]
        public HttpResponseMessage GetAllBusinessClientDataTablePagination()
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

                BusinessContentClientDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentClientDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessclientList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Business Client Detail By BusinessOwnerLoginId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessClientDetail_ForVisitorPanel")]
        public HttpResponseMessage GetBusinessClientDetails(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessContentClientDetail_VM> resp = new List<BusinessContentClientDetail_VM>();

                resp = businessOwnerService.GetBusinessClientDetail_lst(businessOwnerLoginId);

                var businessContentClientDetailData = businessOwnerService.GetBusinessClientDetail(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentClientDetailList = resp,
                    BusinessContentClientDetail = businessContentClientDetailData,
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

        ///////////////////////////////////////////////////////// To Business Currently Working  Detail ///////////////////////////////
        /// <summary>
        /// To Get Business Currently Working Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Business/GetBusinessWorkingDetail")]
        public HttpResponseMessage GetBusinessWorking(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessCurrentlyWorkDetail_VM> businessCurrentlyWorkDetail_VM = businessOwnerService.Get_BusinessWorkDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessWorkingDetailList = businessCurrentlyWorkDetail_VM,
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
        /// To Get Staff Business Detail By Staff UserLoginId 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Business/GetBusinessStaffWorkingDetail")]
        public HttpResponseMessage GetBusinessUserStaffWorking(long userLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessCurrentlyWorkDetail_VM> businessCurrentlyWorkDetail_VM = businessOwnerService.Get_BusinessUserStaffWorkDetailList(userLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessWorkingDetailList = businessCurrentlyWorkDetail_VM,
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

        #region Manage Business Course images ---------------------------------------------------------------------
        /// <summary>
        /// Add Business Course Image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessCourse_PPCMeta")]
        public HttpResponseMessage AddUpdateCourseImages()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentCourseImages_VM requestBusinessCourseImage_VM = new BusinessContentCourseImages_VM();
                requestBusinessCourseImage_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessCourseImage_VM.Title = HttpRequest.Params["Title"].Trim();
                requestBusinessCourseImage_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessCourseImageFile = files["CourseSignIcon"];
                requestBusinessCourseImage_VM.CourseSignIcon = _ManageBusinessCourseImageFile; // for validation
                string _BusinessCourseImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessCourseImageFileName = ""; // will be used to delete file while updating.




                // Validate infromation passed
                Error_VM error_VM = requestBusinessCourseImage_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {

                    if (_ManageBusinessCourseImageFile != null)
                    {
                        _BusinessCourseImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessCourseImageFile);
                    }


                }


                if (requestBusinessCourseImage_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessCourseImage_ById(requestBusinessCourseImage_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessCourseImageFileNameGenerated = respGetBusinessServiceData.CourseSignIcon;

                    }
                    else
                    {
                        _PreviousBusinessCourseImageFileName = respGetBusinessServiceData.CourseSignIcon;

                    }
                }


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentCourseImages_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentCourseImages_Param_VM
                {
                    Id = requestBusinessCourseImage_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = requestBusinessCourseImage_VM.Title,
                    CourseSignIcon = _BusinessCourseImageFileNameGenerated,
                    Mode = requestBusinessCourseImage_VM.Mode
                });
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Group Image.
                    #region Insert-Update Manage Business Course   Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessCourseImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentCourseImage), _PreviousBusinessCourseImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentCourseImage), _BusinessCourseImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessCourseImageFile, NewImageFileWithPath);


                    }
                    #endregion
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
        /// Get  business Course images by Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentCourse_PPCMeta")]
        public HttpResponseMessage GetBusinessCourseImageById(long Id)
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


                BusinessContentCourseImagesDetails_VM resp = new BusinessContentCourseImagesDetails_VM();

                resp = businessOwnerService.GetBusinessCourseImage_ById(Id);


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
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteCourseDetail")]
        public HttpResponseMessage DeleteCourseImageById(long id)
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


                resp = businessOwnerService.DeleteBusinessCourseImageById(id);
                //if(resp.ret == 1)
                //{
                //    //delete image
                //    string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ManageBusinessImage), _ManageBusinessImageFileNameGenerated);
                //    fileHelper.SaveUploadedFile(_ManageBusinessImageFile, FileWithPath);
                //}
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
        /// Get All Business content course with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentCourseByPagination")]
        public HttpResponseMessage GetAllBusinessCourseDataTablePagination()
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

                BusinessContentCourseDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentCourseDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessCourseList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get business Course Image  by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessCourseDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessCourseImagesVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessContentCourseImagesDetails_VM> businessCourseDetail_VM = businessOwnerService.Get_BusinessCourseDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessCourseDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion ----------------------------------------------------------------------------------------------

        #region Manage Business Curriculum  images ---------------------------------------------------------------------
        /// <summary>
        /// Add Business Course Image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessCurriculum_PPCMeta")]
        public HttpResponseMessage AddUpdateCurriculumImages()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentCurriculum_VM requestBusinessCurriculumImage_VM = new BusinessContentCurriculum_VM();
                requestBusinessCurriculumImage_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessCurriculumImage_VM.Title = HttpRequest.Params["Title"].Trim();
                requestBusinessCurriculumImage_VM.CurriculumOptions = HttpRequest.Params["CurriculumOptions"];
                requestBusinessCurriculumImage_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessCurriculumImageFile = files["CurriculumImage"];
                requestBusinessCurriculumImage_VM.CurriculumImage = _ManageBusinessCurriculumImageFile; // for validation
                string _BusinessCurriculumImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessCurriculumImageFileName = ""; // will be used to delete file while updating.




                // Validate infromation passed
                Error_VM error_VM = requestBusinessCurriculumImage_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {

                    if (_ManageBusinessCurriculumImageFile != null)
                    {
                        _BusinessCurriculumImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessCurriculumImageFile);
                    }


                }


                if (requestBusinessCurriculumImage_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessCurriculumDetail_ById(requestBusinessCurriculumImage_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessCurriculumImageFileNameGenerated = respGetBusinessServiceData.CurriculumImage;

                    }
                    else
                    {
                        _PreviousBusinessCurriculumImageFileName = respGetBusinessServiceData.CurriculumImage;

                    }
                }


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentCurriculum_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentCurriculum_Params_VM
                {
                    Id = requestBusinessCurriculumImage_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = requestBusinessCurriculumImage_VM.Title,
                    CurriculumOptions = requestBusinessCurriculumImage_VM.CurriculumOptions,
                    CurriculumImage = _BusinessCurriculumImageFileNameGenerated,
                    Mode = requestBusinessCurriculumImage_VM.Mode
                });
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Group Image.
                    #region Insert-Update Manage Business Curriculum   Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessCurriculumImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentCurriculumImage), _PreviousBusinessCurriculumImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentCurriculumImage), _BusinessCurriculumImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessCurriculumImageFile, NewImageFileWithPath);


                    }
                    #endregion
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
        /// Get  business Course images by Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentCurriculum_PPCMeta")]
        public HttpResponseMessage GetBusinessCurriculumImageById(long Id)
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


                BusinessContentCurriculumDetail_VM resp = new BusinessContentCurriculumDetail_VM();

                resp = businessOwnerService.GetBusinessCurriculumDetail_ById(Id);


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
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteCurriculumDetail")]
        public HttpResponseMessage DeleteCurriculumImageById(long id)
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


                resp = businessOwnerService.DeleteBusinessCurriculumById(id);

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
        /// Get All Business content course with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentCurriculumByPagination")]
        public HttpResponseMessage GetAllBusinessCurriculumDataTablePagination()
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

                BusinessContentCurriculumDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentCurriculumDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessCurriculumList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get business Curriculum Image  by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessCurriculumDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessCurriculumVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                //BusinessContentCurriculumDetail_VM resp = new BusinessContentCurriculumDetail_VM();

                List<BusinessContentCurriculumDetail_VM> businessCourseDetail_VM = businessOwnerService.Get_BusinessCurriculumDetailList(businessOwnerLoginId);
                //resp = businessOwnerService.GetBusinessCurriculumDetail_ById(businessOwnerLoginId);
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessCourseDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        #endregion ----------------------------------------------------------------------------------------------




        #region Manage Business Education   ---------------------------------------------------------------------
        /// <summary>
        /// Add Business Education Image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/AddUpdateBusinessEducation_PPCMeta")]
        public HttpResponseMessage AddUpdateEducationDetail()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentEducation_VM requestBusinessEducation_VM = new BusinessContentEducation_VM();
                requestBusinessEducation_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessEducation_VM.University = HttpRequest.Params["University"];
                //requestBusinessEducation_VM.StartDate = HttpRequest.Params["StartDate"];
                //requestBusinessEducation_VM.EndDate = HttpRequest.Params["EndDate"];
                requestBusinessEducation_VM.Description = HttpRequest.Params["Description"];
                requestBusinessEducation_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessUniversityImageFile = files["UniversityImage"];
                requestBusinessEducation_VM.UniversityImage = _ManageBusinessUniversityImageFile; // for validation
                string _BusinessUniversityImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessUniversityImageFileName = ""; // will be used to delete file while updating.

                // Get Attatched Files

                HttpPostedFile _ManageBusinessUniversityLogoFile = files["UniversityLogo"];
                requestBusinessEducation_VM.UniversityLogo = _ManageBusinessUniversityLogoFile; // for validation
                string _BusinessUniversityLogoFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessUniversityLogoFileName = ""; // will be used to delete file while updating.


                // Validate infromation passed
                Error_VM error_VM = requestBusinessEducation_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                if (files.Count > 0)
                {

                    if (_ManageBusinessUniversityLogoFile != null)
                    {
                        _BusinessUniversityLogoFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessUniversityLogoFile);
                    }



                    if (_ManageBusinessUniversityImageFile != null)
                    {
                        _BusinessUniversityImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessUniversityImageFile);
                    }


                }




                if (requestBusinessEducation_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessEducationDetail_ById(requestBusinessEducation_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    //if (files.Count <= 0)
                    //{
                    if (_ManageBusinessUniversityLogoFile == null)
                    {
                        _BusinessUniversityLogoFileNameGenerated = respGetBusinessServiceData.UniversityLogo ?? "";

                    }
                    else
                    {
                        _PreviousBusinessUniversityLogoFileName = respGetBusinessServiceData.UniversityLogo ?? "";
                    }
                    if (_ManageBusinessUniversityImageFile == null)
                    {
                        _BusinessUniversityImageFileNameGenerated = respGetBusinessServiceData.UniversityImage ?? "";

                    }
                    else
                    {
                        _PreviousBusinessUniversityImageFileName = respGetBusinessServiceData.UniversityImage ?? "";
                    }

                }

                //}

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentEducation_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentEducation_Param_VM
                {
                    Id = requestBusinessEducation_VM.Id,
                    UserLoginId = _LoginID_Exact,
                    University = requestBusinessEducation_VM.University,
                    UniversityLogo = _BusinessUniversityLogoFileNameGenerated,
                    UniversityImage = _BusinessUniversityImageFileNameGenerated,
                    Description = requestBusinessEducation_VM.Description,
                    Mode = requestBusinessEducation_VM.Mode
                });
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {

                    // Update Group Image.
                    #region Insert-Update Manage Business UniversityLogo    on Server
                    //if (files.Count > 0)
                    //{
                    if (!String.IsNullOrEmpty(_PreviousBusinessUniversityLogoFileName))
                    {
                        // remove previous image file
                        string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentUniversityLogo), _PreviousBusinessUniversityLogoFileName);
                        fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                    }
                    if (_ManageBusinessUniversityLogoFile != null)
                    {
                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentUniversityLogo), _BusinessUniversityLogoFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessUniversityLogoFile, NewImageFileWithPath);

                    }



                    if (!String.IsNullOrEmpty(_PreviousBusinessUniversityImageFileName))
                    {
                        // remove previous image file
                        string RemoveUniversityImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentUniversityImage), _PreviousBusinessUniversityImageFileName);
                        fileHelper.DeleteAttachedFileFromServer(RemoveUniversityImageFileWithPath);
                    }
                    if (_ManageBusinessUniversityImageFile != null)
                    // save new image file
                    {
                        string NewUniversityImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentUniversityImage), _BusinessUniversityImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessUniversityImageFile, NewUniversityImageFileWithPath);
                    }



                    //}
                    #endregion
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
        /// Get  business Education detail by Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentEducation_PPCMeta")]
        public HttpResponseMessage GetBusinessEducationDetailById(long Id)
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


                BusinessContentEducationDetail_VM resp = new BusinessContentEducationDetail_VM();

                resp = businessOwnerService.GetBusinessEducationDetail_ById(Id);


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
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/DeleteEducationDetail")]
        public HttpResponseMessage DeleteEducationDetailById(long id)
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


                resp = businessOwnerService.DeleteBusinessEducationDetailById(id);

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
        /// Get All Business content course with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetAllBusinessContentEducationByPagination")]
        public HttpResponseMessage GetAllBusinessEducationDataTablePagination()
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

                BusinessContentEducationDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentEducationDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessEducationList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get business Education   by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessEducationDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessEducationVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {



                List<BusinessContentEducationDetail_VM> businessEducationDetail_VM = businessOwnerService.Get_BusinessEducationDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessEducationDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        #endregion ----------------------------------------------------------------------------------------------



        #region Manage Business Language icon detail ---------------------------------------------------------------------
        /// <summary>
        /// Add Business Language icon
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/AddUpdateBusinessLanguage_PPCMeta")]
        public HttpResponseMessage AddUpdateLanguageImages()
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




                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessLanguage_VM requestBusinessLanguageImage_VM = new BusinessLanguage_VM();
                requestBusinessLanguageImage_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessLanguageImage_VM.Language = HttpRequest.Params["Language"].Trim();
                requestBusinessLanguageImage_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessLanguageIconFile = files["LanguageIcon"];
                requestBusinessLanguageImage_VM.LanguageIcon = _ManageBusinessLanguageIconFile; // for validation
                string _BusinessLanguageIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessLanguageIconFileName = ""; // will be used to delete file while updating.




                // Validate infromation passed
                Error_VM error_VM = requestBusinessLanguageImage_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {

                    if (_ManageBusinessLanguageIconFile != null)
                    {
                        _BusinessLanguageIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessLanguageIconFile);
                    }


                }


                if (requestBusinessLanguageImage_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessLanguageIcon_ById(requestBusinessLanguageImage_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessLanguageIconFileNameGenerated = respGetBusinessServiceData.LanguageIcon;

                    }
                    else
                    {
                        _PreviousBusinessLanguageIconFileName = respGetBusinessServiceData.LanguageIcon;

                    }
                }


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentLanguage_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentLanguage_Param_VM
                {
                    Id = requestBusinessLanguageImage_VM.Id,
                    UserLoginId = _LoginID_Exact,
                    Language = requestBusinessLanguageImage_VM.Language,
                    LanguageIcon = _BusinessLanguageIconFileNameGenerated,
                    Mode = requestBusinessLanguageImage_VM.Mode
                });
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Group Image.
                    #region Insert-Update Manage BusinessLanguage Icon
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessLanguageIconFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentLanguageIcon), _PreviousBusinessLanguageIconFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentLanguageIcon), _BusinessLanguageIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessLanguageIconFile, NewImageFileWithPath);


                    }
                    #endregion
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
        /// Get  business Course images by Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/GetBusinessContentLanguage_PPCMeta")]
        public HttpResponseMessage GetBusinessLanguageIconById(long Id)
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


                BusinessLanguageDetail_VM resp = new BusinessLanguageDetail_VM();

                resp = businessOwnerService.GetBusinessLanguageIcon_ById(Id);


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
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Business/DeleteLanguageDetail")]
        public HttpResponseMessage DeleteLanguageIconeById(long id)
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


                resp = businessOwnerService.DeleteBusinessLanguageIconById(id);
                //if(resp.ret == 1)
                //{
                //    //delete image
                //    string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ManageBusinessImage), _ManageBusinessImageFileNameGenerated);
                //    fileHelper.SaveUploadedFile(_ManageBusinessImageFile, FileWithPath);
                //}
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
        /// Get All Business content Language with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentLanguageByPagination")]
        public HttpResponseMessage GetAllBusinessLanguageDataTablePagination()
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

                BusinessLanguageDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessLanguageDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessLanguageList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get business language Icon  by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessLanguageDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessLanguageDetailVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessLanguageDetail_VM> businessCourseDetail_VM = businessOwnerService.GetBusinessLanguageIconDetaillst_ById(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessCourseDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Get business language Icon  by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessLanguageDetailSuperAdminPanel")]
        public HttpResponseMessage GetBusinessLanguageDetailVisitorPanel()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessLanguageDetail_VM> businessCourseDetail_VM = businessOwnerService.GetBusinessLanguageIconDetaillst();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessCourseDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,SuperAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessLanguage")]
        public HttpResponseMessage AddUpdateLanguage_Details()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentLanguage_PPCMeta_VM requestBusinessLanguage_VM = new BusinessContentLanguage_PPCMeta_VM();
                requestBusinessLanguage_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string LanguageId = HttpRequest.Params["LanguageId"];
                //requestBusinessLanguage_VM.LanguageId = Convert.ToInt32(HttpRequest.Params["LanguageId"]);
                requestBusinessLanguage_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                List<int> integerList = LanguageId.Split(',')
                                  .Select(int.Parse)
                                  .ToList();

                // Validate infromation passed
                //Error_VM error_VM = requestBusinessLanguage_VM.ValidInformation();

                //if (!error_VM.Valid)
                //{
                //    apiResponse.status = -1;
                //    apiResponse.message = error_VM.Message;
                //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                //}





                SPResponseViewModel resp = new SPResponseViewModel();


                resp = businessOwnerService.DeleteBusinessLanguageBybusinessOwnerLoginId(_BusinessOwnerLoginId);

                //// Call the stored procedure with Mode = 2 to delete a record
                //resp = storedProcedureRepository.SP_InsertUpdateBusinessContentLanguages_Get<SPResponseViewModel>(new SP_InsertUpdateLanguage_Param_VM
                //{
                //    Id = requestBusinessLanguage_VM.Id,
                //    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                //    Mode = 2
                //});



                foreach (int number in integerList)
                {
                    // Call the stored procedure with Mode = 1 to insert a record for each number
                    resp = storedProcedureRepository.SP_InsertUpdateBusinessContentLanguages_Get<SPResponseViewModel>(new SP_InsertUpdateLanguage_Param_VM
                    {
                        Id = requestBusinessLanguage_VM.Id,
                        ProfilePageTypeId = BusinesssProfilePageType.Id,
                        BusinessOwnerLoginId = _BusinessOwnerLoginId,
                        LanguageId = number,
                        Mode = 1
                    });

                    // Handle the result of the insertion if needed
                    if (resp.ret <= 0)
                    {
                        apiResponse.status = resp.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
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


        #region To Get All User Video-----------------------------------
        /// <summary>
        /// To Get All User Video
        /// </summary>
        /// <returns>To Get  All  Inserted User Vedio Detail </returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessLanguageDetail")]
        public HttpResponseMessage GetAllUserContentLanguageList()
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

                List<BusinessContentLanguageDetail_VM> usercontentlanguagedetailresponse = businessOwnerService.GetBusinessLanguageDetaillst(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = usercontentlanguagedetailresponse;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }
        #endregion



        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessUserContentLanguageList")]
        public HttpResponseMessage GetAllBusinessUserContentLanguageList(long businessOwnerLoginId)
        {

            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessContentLanguageDetail_VM> usercontentlanguagedetailresponse = businessOwnerService.GetBusinessLanguageDetaillst(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = usercontentlanguagedetailresponse;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetBusinessUniversityListVisitorPanel")]
        public HttpResponseMessage GetBusinessUniversityVisitorPanel()
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


                List<BusinessContentEducationDetail_VM> businessEducationDetail_VM = businessOwnerService.Get_BusinessUniversityDetailList(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessEducationDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }




        //////////////////////////////////////////////////////////////////University Teacher Side  ADDDDDD ////////////////////////////////////
        ///


        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,SuperAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessEducationTeacherUniversity")]
        public HttpResponseMessage AddUpdateUniversity_Details()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentUniversity_VM requestBusinessUniversity_VM = new BusinessContentUniversity_VM();
                requestBusinessUniversity_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessUniversity_VM.UniversityName = HttpRequest.Params["UniversityName"];
                requestBusinessUniversity_VM.StartDate = HttpRequest.Params["StartDate"];
                requestBusinessUniversity_VM.EndDate = HttpRequest.Params["EndDate"];
                requestBusinessUniversity_VM.Qualification = HttpRequest.Params["Qualification"];
                requestBusinessUniversity_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessUniversityLogoFile = files["UniversityLogo"];
                requestBusinessUniversity_VM.UniversityLogo = _ManageBusinessUniversityLogoFile; // for validation
                string _BusinessUniversityLogoFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessUniversityLogoFileName = ""; // will be used to delete file while updating.


                // Validate infromation passed
                Error_VM error_VM = requestBusinessUniversity_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_ManageBusinessUniversityLogoFile != null)
                    {

                        _BusinessUniversityLogoFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessUniversityLogoFile);
                    }

                }


                if (requestBusinessUniversity_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessEducationTeacherDetail_ById(requestBusinessUniversity_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessUniversityLogoFileNameGenerated = respGetBusinessServiceData.UniversityLogo ?? "";


                    }
                    else
                    {
                        _PreviousBusinessUniversityLogoFileName = respGetBusinessServiceData.UniversityLogo ?? "";


                    }
                }


                // Call the stored procedure with Mode = 1 to insert a record for each number
                var resp = storedProcedureRepository.SP_InsertUpdateBusinessUniversity_Get<SPResponseViewModel>(new SP_InsertUpdateUniversityDetail
                {
                    Id = requestBusinessUniversity_VM.Id,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    StartDate = requestBusinessUniversity_VM.StartDate,
                    EndDate = requestBusinessUniversity_VM.EndDate,
                    Qualification = requestBusinessUniversity_VM.Qualification,
                    UniversityName = requestBusinessUniversity_VM.UniversityName,
                    UniversityLogo = _BusinessUniversityLogoFileNameGenerated,
                    Mode = requestBusinessUniversity_VM.Mode,
                });

                // Handle the result of the insertion if needed
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                if (resp.ret == 1)
                {

                    // Update Group Image.
                    #region Insert-Update Manage Business UniversityLogo    on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessUniversityLogoFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContenTeacherUniversityLogo), _PreviousBusinessUniversityLogoFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContenTeacherUniversityLogo), _BusinessUniversityLogoFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessUniversityLogoFile, NewImageFileWithPath);





                    }
                    #endregion
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
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentEducationTeacher_PPCMeta")]
        public HttpResponseMessage GetBusinessEducationTeacherDetailById(long Id)
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


                BusinessUniversityDetail_VM resp = new BusinessUniversityDetail_VM();

                resp = businessOwnerService.GetBusinessEducationTeacherDetail_ById(Id);


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
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/DeleteEducationTeacherDetail")]
        public HttpResponseMessage DeleteEducationTeacherDetailById(long id)
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


                resp = businessOwnerService.DeleteBusinessUniversityById(id);

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
        /// Get All Business content Teacher University Detail  with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetAllBusinessContentTeacherUniversityByPagination")]
        public HttpResponseMessage GetAllBusinessTeacherUniversityDataTablePagination()
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

                BusinessContentUniversityDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentUniversityDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessTeacherList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get All Teacher Education University Detail For Visitor Panel
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessTeacherUniversityEducationDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessTeacherVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessUniversityDetail_VM> businessEducationTeacherUniversityDetail_VM = businessOwnerService.GetBusinessEducationTeacherDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessEducationTeacherUniversityDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Businesss Side To Get All University Detail By Pagination to show the university detail to get superadmin 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("api/BusinessContent/GetAllBusinessUniversityDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessUniversityDetailVisitorPanel()
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


                List<BusinessContentEducationDetail_VM> businessEducationDetail_VM = businessOwnerService.Get_BusinessEducationDetailList(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessEducationDetail_VM;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }






        /////////////////////////////////////////////////////////////////////////////    Business University Id ///////////////////
        ///



        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,SuperAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessUniversityId")]
        public HttpResponseMessage AddUpdateUniversityId_Details()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentUniversity_ViewModel requestBusinessUniversity_VM = new BusinessContentUniversity_ViewModel();
                requestBusinessUniversity_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string UniversityId = HttpRequest.Params["UniversityId"];
                requestBusinessUniversity_VM.Mode = 1;

                List<int> integerList = UniversityId.Split(',')
                                  .Select(int.Parse)
                                  .ToList();



                SPResponseViewModel resp = new SPResponseViewModel();


                resp = businessOwnerService.DeleteBusinessUniversityBybusinessOwnerLoginId(_BusinessOwnerLoginId);



                foreach (int number in integerList)
                {
                    // Call the stored procedure with Mode = 1 to insert a record for each number
                    resp = storedProcedureRepository.SP_InsertUpdateBusinessUniversityId_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentUniversityDetail_PPCMeta
                    {
                        Id = requestBusinessUniversity_VM.Id,
                        ProfilePageTypeId = BusinesssProfilePageType.Id,
                        BusinessOwnerLoginId = _BusinessOwnerLoginId,
                        UniversityId = number,
                        Mode = 1
                    });

                    // Handle the result of the insertion if needed
                    if (resp.ret <= 0)
                    {
                        apiResponse.status = resp.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
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

        //////////////////////////////////////////////// To Show All University Detail  For Education Page
        ///


        [HttpGet]
        [Route("api/BusinessContent/GetAllBusinessEducationAffilatedUniversityDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessUniversityDetailListVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();


                BusinessContentEducationDetail_VM resp = new BusinessContentEducationDetail_VM();
                resp = businessOwnerService.GetBusinessEducationDetail_BybusinessOwnerLoginId(businessOwnerLoginId);

                List<BusinessContentEducationDetail_VM> businessEducationDetail_VM = businessOwnerService.Get_BusinessEducationDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessContentUniversityDetail = resp,
                    BusinessContentUniversityDetailList = businessEducationDetail_VM
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
        /// To Get Business Content Profile Detail (Visitor-Panel) by Using BusinessOwnerLoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBusinessDetailById")]
        public HttpResponseMessage GetBusinessDetailByBusinessOwnerLoginId(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                BusinessOwnerList_VM resp = new BusinessOwnerList_VM();
                resp = businessOwnerService.SP_ManageBusinessContentProfileDetail_Get(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;
                //}

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet]
        [Route("api/Business/GetInstructorDetail")]
        public HttpResponseMessage GetInstructorDetailByBusinessOwnerLoginId(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                BusinessOwnerList_VM resp = new BusinessOwnerList_VM();
                resp = businessOwnerService.SP_ManageInstructorProfileDetail_Get(businessOwnerLoginId);

                List<BusinessOwnerList_VM> ratingDetail = businessOwnerService.Get_InstructorRatingDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    InstructorDetail = resp,
                    RatingDetail = ratingDetail
                };
                //}

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/RegisterStudent")]
        public HttpResponseMessage RegisterBusinessStudent()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessStudentRegisterViewModel studentregisterViewModel = new BusinessStudentRegisterViewModel();

                studentregisterViewModel.Email = HttpRequest.Params["Email"].Trim();
                studentregisterViewModel.Password = HttpRequest.Params["Password"].Trim();
                studentregisterViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                studentregisterViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                studentregisterViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                studentregisterViewModel.Gender = Convert.ToInt32(HttpRequest.Params["Gender"]);
                studentregisterViewModel.PhoneNumber_CountryCode = HttpRequest.Params["PhoneNumberCountryCode"].Trim();
                studentregisterViewModel.HasMasterId = Convert.ToInt32(HttpRequest.Params["HasMasterId"]);
                studentregisterViewModel.MasterId = HttpRequest.Params["MasterId"].Trim();

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinessStudentProfileImageFile = files["BusinessStudentProfileImage"];
                string _BusinessStudentProfileImageFileNameGenerated = ""; //will contains generated file name
                studentregisterViewModel.BusinessStudentProfileImage = _BusinessStudentProfileImageFile;
                
                // Check and set sent Images
                if (files.Count > 0)
                {
                    if (_BusinessStudentProfileImageFile != null)
                        _BusinessStudentProfileImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinessStudentProfileImageFile);
                }

                long studentUserLoginId = 0;
                // Check and verify MasterId of user and get UserLoginId
                if (studentregisterViewModel.HasMasterId == 1)
                {
                    // only Master-User MasterIds allowed:
                    if(!studentregisterViewModel.MasterId.Contains("MU_"))
                    {
                        apiResponse.status = -1;
                        apiResponse.message = Resources.ErrorMessage.OnlyUserMasterIdIsAllowed;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }


                    var dbUserLogin = db.UserLogins.Where(u => u.MasterId == studentregisterViewModel.MasterId && u.IsDeleted == 0).FirstOrDefault();
                    if (dbUserLogin != null)
                    {
                        studentUserLoginId = dbUserLogin.Id;
                    }
                    else
                    {
                        apiResponse.status = -1;
                        apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }

                // Validate infromation passed
                Error_VM error_VM = studentregisterViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (String.IsNullOrEmpty(studentregisterViewModel.PhoneNumber_CountryCode))
                {
                    studentregisterViewModel.PhoneNumber_CountryCode = "+91";
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                var _IsNewUserCreated = 0;
                // For Email if New Account Created
                List<EmailSender_VM> sendEmailList = new List<EmailSender_VM>();
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if(studentregisterViewModel.HasMasterId <= 0)
                        {
                            // Register Student Account
                            studentService = new StudentService(db);
                            var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                            {
                                Email = studentregisterViewModel.Email,
                                Password = EDClass.Encrypt(studentregisterViewModel.Password),
                                PhoneNumber = studentregisterViewModel.PhoneNumber,
                                PhoneNumberCountryCode = "+91",
                                Gender = studentregisterViewModel.Gender,
                                RoleId = 3,
                                FirstName = studentregisterViewModel.FirstName,
                                LastName = studentregisterViewModel.LastName,
                                BusinessStudentProfileImage = _BusinessStudentProfileImageFileNameGenerated,
                                Mode = 1
                            });

                            if (resp.ret <= 0)
                            {
                                apiResponse.status = resp.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // Save Student-Profile picture if passed
                            if (resp != null && resp.ret == 1 && _BusinessStudentProfileImageFile != null)
                            {
                                fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_StudentProfileImage, "", _BusinessStudentProfileImageFileNameGenerated, _BusinessStudentProfileImageFile);
                            }

                            EmailSender_VM sendEmail_Person = new EmailSender_VM()
                            {
                                ReceiverName = studentregisterViewModel.FirstName + " " + studentregisterViewModel.LastName,
                                Subject = "Registration successful",
                                ToEmail = studentregisterViewModel.Email,
                                MessageBody = $"You have been successfully registered with Masterzone. Your MasterID is: {resp.MasterId} and Your password is {studentregisterViewModel.Password}. Please don't forget to change your password after login."
                            };
                            sendEmailList.Add(sendEmail_Person);

                            _IsNewUserCreated = 1;
                            studentUserLoginId = resp.Id;
                        }
                        else
                        {
                            if(_BusinessStudentProfileImageFile != null)
                            {

                                // update Student Profile Image for business.
                                var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                                {
                                    StudentUserLoginId = studentUserLoginId,
                                    BusinessStudentProfileImage = _BusinessStudentProfileImageFileNameGenerated,
                                    Mode = 5
                                });

                                // Save Student-Profile picture if passed
                                if (resp != null && resp.ret == 1)
                                {
                                    fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_StudentProfileImage, "", _BusinessStudentProfileImageFileNameGenerated, _BusinessStudentProfileImageFile);
                                }

                            }
                        }
                        
                        // Link Student with Business
                        BusinessStudentService businessStudentService = new BusinessStudentService(db);
                        var respBusinessStudent = businessStudentService.AddStudentLinkWithBusinessOwner(_BusinessOwnerLoginId, studentUserLoginId);

                        if(respBusinessStudent.ret <= 0)
                        {
                            apiResponse.status = respBusinessStudent.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(respBusinessStudent.resourceFileName, respBusinessStudent.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        db.SaveChanges(); // Save changes to the database

                        transaction.Commit(); // Commit the transaction if everything is successful

                        EmailSender emailSender = new EmailSender();
                        foreach (var emailData in sendEmailList)
                        {
                            emailSender.Send(emailData.ReceiverName, emailData.Subject, emailData.ToEmail, emailData.MessageBody, "");
                        }

                        apiResponse.status = 1;
                        apiResponse.message = Resources.VisitorPanel.StudentRegistered_SuccessMessage;//ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        apiResponse.data = new {};
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions and perform error handling or logging
                        transaction.Rollback(); // Roll back the transaction
                        apiResponse.status = -100;
                        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                }
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// Get Student Basic Profile Detail by Student-Login-Id
        /// </summary>
        /// <param name="studentLoginId">Student UserLoginId</param>
        /// <returns>Student Basic Profile Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetStudentBasicProfileDetail")]
        public HttpResponseMessage GetStudentBasicProfileDetail(long studentLoginId)
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

                BasicProfileDetail_VM basicProfileDetail_VM = new BasicProfileDetail_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", studentLoginId),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
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
        /// Get Masterzone Id Details by User-Login-Id
        /// </summary>
        /// <param name="userLoginId">Business-Owner/User Login-Id</param>
        /// <returns>Returns Masterzone Id Details</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllMasterIdDetail")]
        public HttpResponseMessage GetAllMasterIdDetail(long userLoginId = 0)
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

                var masterzoneIdDetail = businessOwnerService.GetAllMasterIdDetails(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = masterzoneIdDetail;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        #region Notice Board --------------------------------------------------------------------------
        ////////////////////////////////////////////////////////   Notice Board               /////////////////////////////////////////////////////////////
        ///

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessNoticeBoard")]
        public HttpResponseMessage AddUpdateNoticeeBoard_Details()
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
                BusinessContentNoticeBoard_VM requestBusinessNoticeBoard_VM = new BusinessContentNoticeBoard_VM();
                requestBusinessNoticeBoard_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessNoticeBoard_VM.StartDate = HttpRequest.Params["StartDate"];
                requestBusinessNoticeBoard_VM.Description = HttpRequest.Params["Description"];
                requestBusinessNoticeBoard_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);




                // Validate infromation passed
                Error_VM error_VM = requestBusinessNoticeBoard_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }






                // Call the stored procedure with Mode = 1 to insert a record for each number
                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentNoticeBoard<SPResponseViewModel>(new SP_InsertUpdateBusinessNoticeBoard_Param_VM
                {
                    Id = requestBusinessNoticeBoard_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    StartDate = requestBusinessNoticeBoard_VM.StartDate,
                    Description = requestBusinessNoticeBoard_VM.Description,
                    Mode = requestBusinessNoticeBoard_VM.Mode,
                });

                // Handle the result of the insertion if needed
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
        /// To Get Notice Board Detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentNoticeBoardDetail")]
        public HttpResponseMessage GetBusinessNoticeeBoardDetailById(long Id)
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


                BusinessContentNoticeBoardDetail_VM resp = new BusinessContentNoticeBoardDetail_VM();

                resp = businessOwnerService.GetBusinessNoticeBoardDetail_ById(Id);


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
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/DeleteNoticeBoardDetail")]
        public HttpResponseMessage DeleteNoticeeBoardDetailById(long id)
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


                resp = businessOwnerService.DeleteNoticeBoardById(id);

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


        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentNoticeBoardDetail")]
        public HttpResponseMessage GetBusinessNoticeeBoardDetail()
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


                List<BusinessContentNoticeBoardDetail_VM> resp = businessOwnerService.SP_ManageBusinessNoticeBoard_GetList(_BusinessOwnerLoginId);



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



        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentNoticeBoardDetailList")]
        public HttpResponseMessage GetBusinessNoticeBoardDetaillst(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessContentNoticeBoardDetail_VM> resp = businessOwnerService.SP_ManageBusinessNoticeBoard_GetList(businessOwnerLoginId);

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
        #endregion --------------------------------------------------------------------

        #region Business Course Categories Detail ---------------------------------------------------------
        //////////////////////////////////////////////////////////////   Business Course Categories Detail /////////////////////////////////////////////////

        /// <summary>
        ///  To InsertUpdate CourseCategory
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,BusinessAdmin,Staff")]
        [Route("api/BusinessContent/AddUpdateCategoriesDetail")]
        public HttpResponseMessage AddUpdateCoursecategory_Details()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentCourseCategory_ViewModel requestBusinessCourseCategory_VM = new BusinessContentCourseCategory_ViewModel();
                requestBusinessCourseCategory_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessCourseCategory_VM.Title = HttpRequest.Params["Title"];
                requestBusinessCourseCategory_VM.Description = HttpRequest.Params["Description"];
                requestBusinessCourseCategory_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Call the stored procedure with Mode = 1 to insert a record for each number
                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentCourseCategory<SPResponseViewModel>(new SP_InsertUpdateBusinessCourseCategory_Param_VM
                {
                    Id = requestBusinessCourseCategory_VM.Id,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Title = requestBusinessCourseCategory_VM.Title,
                    Description = requestBusinessCourseCategory_VM.Description,
                    Mode = requestBusinessCourseCategory_VM.Mode,
                });

                // Handle the result of the insertion if needed
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                // send success response
                //apiResponse.status = 1;
                //apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                //apiResponse.data = new { };

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
        /// To Get Business CourseCategory Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetCategoriesDetail")]
        public HttpResponseMessage GetBusinessContentCourseCategory()
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


                BusinessContentCourseCategoryDetail_VM resp = new BusinessContentCourseCategoryDetail_VM();

                resp = businessOwnerService.GetBusinessCourseCategoryDetail_ById(_BusinessOwnerLoginId);



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
        /// To Get Business CourseCategory Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetAllBusinessCategoriesDetailForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentCourseCategoryDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessContentCourseCategoryDetail_VM resp = businessOwnerService.GetBusinessCourseCategoryDetail_ById(businessOwnerLoginId);

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
        #endregion -----------------------------------------------------------------------

        #region Business Content Access Course Detail -------------------------------------------
        //////////////////////////////////////////////////////////     Business Content Access Course Detail ////////////////////////////////////////////



        /// <summary>
        /// To Add Business  Banner Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessAccessCourseDetail")]

        public HttpResponseMessage AddUpdateBusinessAccessCourseProfile()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //  BusinesssProfilePageType.Key = "yoga_web";
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentAccessCourseDetail_ViewModel businessContentAccessCourseDetail_ViewModel = new BusinessContentAccessCourseDetail_ViewModel();

                // Parse and assign values from HTTP request parameters
                businessContentAccessCourseDetail_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                businessContentAccessCourseDetail_ViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                businessContentAccessCourseDetail_ViewModel.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;

                // Check and set ButtonText
                string accessCourseParam = HttpRequest.Params["AccessCourse"];
                businessContentAccessCourseDetail_ViewModel.AccessCourse = !string.IsNullOrEmpty(accessCourseParam) ? accessCourseParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                businessContentAccessCourseDetail_ViewModel.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                businessContentAccessCourseDetail_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = businessContentAccessCourseDetail_ViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssCourseImageFile = files["CourseImage"];
                businessContentAccessCourseDetail_ViewModel.CourseImage = _BusinesssCourseImageFile; // for validation
                string _BusinessAccessCourseImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessAccessCourseImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssCourseImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessAccessCourseImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssCourseImageFile);
                    }
                }


                if (businessContentAccessCourseDetail_ViewModel.Mode == 1)
                {
                    var respGetBusinessAccessCourseDetail = businessOwnerService.GetBusinessAccessCourseDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessAccessCourseDetail != null && _BusinesssCourseImageFile == null) // Check if respGetBusinessAccessCourseDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessAccessCourseDetail.CourseImage == null)
                        {
                            _PreviousBusinessAccessCourseImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessAccessCourseImageFileNameGenerated = respGetBusinessAccessCourseDetail.CourseImage;
                        }
                    }
                    else
                    {

                        _PreviousBusinessAccessCourseImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentAccessCourse_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentAccessCourse_Param_VM
                {
                    Id = businessContentAccessCourseDetail_ViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentAccessCourseDetail_ViewModel.Title,
                    SubTitle = businessContentAccessCourseDetail_ViewModel.SubTitle,
                    Description = businessContentAccessCourseDetail_ViewModel.Description,
                    AccessCourse = businessContentAccessCourseDetail_ViewModel.AccessCourse,
                    CourseImage = _BusinessAccessCourseImageFileNameGenerated,
                    Mode = businessContentAccessCourseDetail_ViewModel.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Class Image.
                    #region Insert-Update Access Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessAccessCourseImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAccessImage), _PreviousBusinessAccessCourseImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAccessImage), _BusinessAccessCourseImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssCourseImageFile, NewImageFileWithPath);
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
        /// To Get Access Course Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AccessCourseDetail/Get")]
        public HttpResponseMessage GetAccessCourseDetail()
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


                BusinessContentAccessCourseDetail_VM businessAccesscourseDetail = businessOwnerService.GetBusinessAccessCourseDetail(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessAccesscourseDetail;

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
        /// To Get Business banner 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //  [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessAccessCourseDetail")]
        public HttpResponseMessage GetBusinessContentAccessCourse(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessContentAccessCourseDetail_VM businessContentAccessCourseDetail = businessOwnerService.GetBusinessAccessCourseDetail(businessOwnerLoginId);



                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessContentAccessCourseDetail;




                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion ------------------------------------------------------------------------


        #region Business-User/User Education & Experience CRUD APIs ------------------------------------
        /// <summary>
        /// To get User Content Resume detail by UserLoginId 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessUserResumeContentDetail")]
        public HttpResponseMessage GetBusinessContentUserResumeDetail()
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


                BusinessContentUserResumeDetail_VM resp = new BusinessContentUserResumeDetail_VM();

                resp = userService.GetUserContentResumeDetail(_LoginID_Exact);



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
        /// To Add Business  Studio Equipment  Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentUserResumeDetail")]

        public HttpResponseMessage AddUpdateBusinessContentUserResume()
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
                UserContentResume_VM userContentResume_VM = new UserContentResume_VM();
                userContentResume_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                userContentResume_VM.Summary = HttpRequest.Params["Summary"].Trim();
                userContentResume_VM.Skills = HttpRequest.Params["Skills"].Trim();
                userContentResume_VM.Languages = HttpRequest.Params["Languages"].Trim();
                userContentResume_VM.Freelance = Convert.ToInt32(HttpRequest.Params["Freelance"]);
                userContentResume_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = userContentResume_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentUserResumeDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessUserContentResume_Param_VM
                {
                    Id = userContentResume_VM.Id,
                    UserLoginId = _LoginID_Exact,
                    Summary = userContentResume_VM.Summary,
                    Skills = userContentResume_VM.Skills,
                    Languages = userContentResume_VM.Languages,
                    SubmittedByLoginId = _LoginID_Exact,
                    Freelance = userContentResume_VM.Freelance,
                    Mode = userContentResume_VM.Mode
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
        /// To Get  Staff User Content Recume Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetStaffUserResumeContentDetail")]
        public HttpResponseMessage GetStaffContentUserResumeDetail(long userLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                StaffUserContentUserResumeDetail_ViewModel resp = new StaffUserContentUserResumeDetail_ViewModel();
                resp = userService.GetStaffUserContentResumeDetail(userLoginId);

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
        /// To Add/ Update Experience Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessUserExperienceDetail")]

        public HttpResponseMessage AddUpdateBusinessUserExperienceDetail()
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
                UserExperienceDetail_VM userExperienceDetail_VM = new UserExperienceDetail_VM();
                userExperienceDetail_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                userExperienceDetail_VM.Title = HttpRequest.Params["Title"].Trim();
                userExperienceDetail_VM.CompanyName = HttpRequest.Params["CompanyName"].Trim();
                userExperienceDetail_VM.StartMonth = HttpRequest.Params["StartMonth"].Trim();
                userExperienceDetail_VM.EndMonth = HttpRequest.Params["EndMonth"].Trim();
                userExperienceDetail_VM.StartYear = HttpRequest.Params["StartYear"].Trim();
                userExperienceDetail_VM.EndYear = HttpRequest.Params["EndYear"].Trim();
                userExperienceDetail_VM.StartDate = HttpRequest.Params["StartDate"].Trim();
                userExperienceDetail_VM.EndDate = HttpRequest.Params["EndDate"].Trim();
                userExperienceDetail_VM.Description = HttpRequest.Params["Description"].Trim();
                userExperienceDetail_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = userExperienceDetail_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessUerExperienceDetail_Get<SPResponseViewModel>(new SP_InsertUpdateUserExperience_Param_VM
                {
                    Id = userExperienceDetail_VM.Id,
                    UserLoginId = _LoginID_Exact,
                    Title = userExperienceDetail_VM.Title,
                    CompanyName = userExperienceDetail_VM.CompanyName,
                    StartMonth = userExperienceDetail_VM.StartMonth,
                    EndMonth = userExperienceDetail_VM.EndMonth,
                    StartYear = userExperienceDetail_VM.StartYear,
                    EndYear = userExperienceDetail_VM.EndYear,
                    StartDate = userExperienceDetail_VM.StartDate,
                    EndDate = userExperienceDetail_VM.EndDate,
                    Description = userExperienceDetail_VM.Description,
                    SubmittedByLoginId = _LoginID_Exact,
                    Mode = userExperienceDetail_VM.Mode
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
        /// To View User experience Detail By Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllUserExperienceByPagination")]
        public HttpResponseMessage GetAllUserExperienceByPagination()
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

                Experience_Pagination_SQL_Params_VM _Params_VM = new Experience_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = userService.GetExperienceDetailList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get User Experience Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetUserExperienceDetailById")]
        public HttpResponseMessage GetUserExperienceDetailById(long id)
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


                UserExperienceDetail_ViewModel resp = new UserExperienceDetail_ViewModel();

                resp = userService.GetStaffUserExperienceDetail(id);



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
        /// To Delete User Experience Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteUserExperienceDetailById")]
        public HttpResponseMessage DeleteUserExperienceDetailById(long id)
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
                var resp = userService.DeleteUserExperienceDetail(id);


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
        /// To Delete Education Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteUserEducationDetailById")]
        public HttpResponseMessage DeleteUserEducationDetailById(long id)
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
                var resp = userService.DeleteUserEducationDetail(id);


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
        /// To Get User Education Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetUserEducationDetailById")]
        public HttpResponseMessage GetUserEducationDetailById(long id)
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


                UserEducation_ViewModel resp = new UserEducation_ViewModel();

                resp = userService.GetStaffUserEducationDetail(id);



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
        /// To Add/Update User Education Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessUserEducationDetail")]

        public HttpResponseMessage AddUpdateBusinessUserEducationDetail()
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
                UserEducation_VM userEducation_VM = new UserEducation_VM();
                userEducation_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                userEducation_VM.SchoolName = HttpRequest.Params["SchoolName"].Trim();
                userEducation_VM.Designation = HttpRequest.Params["Designation"].Trim();
                userEducation_VM.StartMonth = HttpRequest.Params["StartMonth"].Trim();
                userEducation_VM.EndMonth = HttpRequest.Params["EndMonth"].Trim();
                userEducation_VM.StartYear = HttpRequest.Params["StartYear"].Trim();
                userEducation_VM.EndYear = HttpRequest.Params["EndYear"].Trim();
                userEducation_VM.StartDate = HttpRequest.Params["StartDate"].Trim();
                userEducation_VM.EndDate = HttpRequest.Params["EndDate"].Trim();
                userEducation_VM.Description = HttpRequest.Params["Description"].Trim();
                userEducation_VM.Grade = HttpRequest.Params["Grade"].Trim();
                userEducation_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = userEducation_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessUerEducationDetail_Get<SPResponseViewModel>(new SP_InsertUpdateUserEducation_Param_VM
                {
                    Id = userEducation_VM.Id,
                    UserLoginId = _LoginID_Exact,
                    SchoolName = userEducation_VM.SchoolName,
                    Designation = userEducation_VM.Designation,
                    StartMonth = userEducation_VM.StartMonth,
                    EndMonth = userEducation_VM.EndMonth,
                    StartYear = userEducation_VM.StartYear,
                    EndYear = userEducation_VM.EndYear,
                    StartDate = userEducation_VM.StartDate,
                    EndDate = userEducation_VM.EndDate,
                    Description = userEducation_VM.Description,
                    Grade = userEducation_VM.Grade,
                    SubmittedByLoginId = _LoginID_Exact,
                    Mode = userEducation_VM.Mode
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
        /// To Get View User Education Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllUserEducationByPagination")]
        public HttpResponseMessage GetAllUserEducationByPagination()
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

                Education_Pagination_SQL_Params_VM _Params_VM = new Education_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = userService.GetEducationDetailList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get the User Resume Detail For Visitor-Panel
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessUserResumeDetail_ForVisitorPanel")]
        public HttpResponseMessage GetBusinessUserResumeDetail_ForVisitorPanel(long userLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<UserEducation_ViewModel> businessUserEducationlst = userService.GetBusinessUserEducationDetaillst_Get(userLoginId);

                List<UserExperienceDetail_ViewModel> businessUserExperiencelst = userService.GetBusinessUserExperienceDetaillst_Get(userLoginId);




                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessUserEducationlst = businessUserEducationlst,
                    BusinessUserExperiencelst = businessUserExperiencelst,

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
        /// To get Other Profile Detail For Visitor-Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetOtherUserProfileDetail")]
        public HttpResponseMessage GetOtherUserProfileDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessUserOtherProfileDetail businessUserOtherProfileDetail = new BusinessUserOtherProfileDetail();

                businessUserOtherProfileDetail = businessOwnerService.GetUserOtherProfileDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessUserOtherProfileDetail;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion -------------------------------------------------------------------------------------


        #region Master-Pro Basic APIs -----------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/MasterProResumeDetails")]
        public HttpResponseMessage MasterProResumeDetails()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                MasterProResume_PPCMeta_VM businessContentFitnessMovement_VM = new MasterProResume_PPCMeta_VM();

                // Parse and assign values from HTTP request parameters
                businessContentFitnessMovement_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentFitnessMovement_VM.Age = HttpRequest.Params["Age"].Trim();
                businessContentFitnessMovement_VM.Nationality = HttpRequest.Params["Nationality"].Trim();
                businessContentFitnessMovement_VM.Freelance = HttpRequest.Params["Freelance"].Trim();
                businessContentFitnessMovement_VM.Skype = HttpRequest.Params["Skype"].Trim();
                businessContentFitnessMovement_VM.Languages = HttpRequest.Params["Languages"].Trim();
                businessContentFitnessMovement_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssServiceImageFile = files["UploadCV"];
                businessContentFitnessMovement_VM.UploadCV = _BusinesssServiceImageFile; // for validation
                string _BusinessServiceImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessServiceImageFileName = "";

                // Validate information passed
                Error_VM error_VM = businessContentFitnessMovement_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                if (files.Count > 0)
                {
                    if (_BusinesssServiceImageFile != null)
                    {
                        if (_BusinesssServiceImageFile != null)
                        {
                            _BusinessServiceImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssServiceImageFile);
                        }


                    }
                }
                if (businessContentFitnessMovement_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = masterProResumeService.SP_MasterProDetails_GetAll(businessContentFitnessMovement_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessServiceImageFileNameGenerated = respGetBusinessServiceData.UploadCV ?? "";
                    }
                    else
                    {
                        _PreviousBusinessServiceImageFileName = respGetBusinessServiceData.UploadCV ?? "";
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateMasterResumeDetails<SPResponseViewModel>(new SP_InsertUpdateMasterpro_Params_VM()
                {
                    Id = businessContentFitnessMovement_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Age = businessContentFitnessMovement_VM.Age,
                    Nationality = businessContentFitnessMovement_VM.Nationality,
                    UploadCV = _BusinessServiceImageFileNameGenerated,
                    Freelance = businessContentFitnessMovement_VM.Freelance,
                    Skype = businessContentFitnessMovement_VM.Skype,
                    Languages = businessContentFitnessMovement_VM.Languages,
                    Mode = businessContentFitnessMovement_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                if (resp.ret == 1)
                {
                    // Update Class Image.
                    #region Insert-Update Training Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessServiceImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MasterproUploadCV), _PreviousBusinessServiceImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MasterproUploadCV), _BusinessServiceImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssServiceImageFile, NewImageFileWithPath);


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

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetMasteroProDetailByBusinessOwnerLoginId")]
        public HttpResponseMessage GetMasteroProDetailByBusinessOwnerLoginId()
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

                MasterProResume_ViewModel resp = new MasterProResume_ViewModel();
                resp = masterProResumeService.SP_MasterProDetails_Get(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    MasterproDetail = resp,

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
        /// Get Masterpro Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetMasteroProDetailById")]
        public HttpResponseMessage GetMasteroProDetailById(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                MasterProResume_ViewModel resp = new MasterProResume_ViewModel();
                resp = masterProResumeService.SP_MasterProDetails_Get(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    MasterproDetail = resp,
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

        #endregion --------------------------------------------------------------------

        #region Master-Pro Extra Information API ---------------------------------------------
        /// <summary>
        /// MasterPro Extra Information
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllMasterProExtraInformationServiceDataTablePagination")]
        public HttpResponseMessage GetAllMasterProExtraInformationServiceDataTablePagination()
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

                MasterProExtraInformationService_Pagination_SQL_Params_VM _Params_VM = new MasterProExtraInformationService_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.UserLoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = masterProExtraInformationService.GetMasterproExtraDetailsServiceList_Pagination(HttpRequestParams, _Params_VM);

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

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateMasterproExtraInformation")]
        public HttpResponseMessage AddUpdateMasterproExtraInformation()
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
                MasterProExtraInformation_VM extraInfo_VM = new MasterProExtraInformation_VM();
                extraInfo_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                extraInfo_VM.Title = HttpRequest.Params["Title"].Trim();
                extraInfo_VM.ShortDescription = HttpRequest.Params["Description"].Trim();
                extraInfo_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files


                // Validate infromation passed
                //Error_VM error_VM = extraInfo_VM.ValidInformation();

                //if (!error_VM.Valid)
                //{
                //    apiResponse.status = -1;
                //    apiResponse.message = error_VM.Message;
                //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                //}




                var resp = storedProcedureRepository.SP_InsertUpdateMasterProExtraInformation_Get<SPResponseViewModel>(new SP_InsertUpdateMasterProExtraInformation_Params_VM
                {
                    Id = extraInfo_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Title = extraInfo_VM.Title,
                    ShortDescription = extraInfo_VM.ShortDescription,
                    Mode = extraInfo_VM.Mode
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

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetMasterProExtraInformationServiceDetailById")]
        public HttpResponseMessage GetMasterProExtraInformationServiceDetailById(long id)
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


                MasterProExtraInformation_ViewModel resp = new MasterProExtraInformation_ViewModel();

                resp = masterProExtraInformationService.GetMasterProExtraInformationById(id);


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

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteMasterProExtraDetailsServiceById")]
        public HttpResponseMessage DeleteMasterProExtraDetailsServiceById(long id)
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
                long _UserLoginId = validateResponse.BusinessAdminLoginId;


                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Business Service 
                resp = masterProExtraInformationService.DeleteMasterproExtraInformationService(id);


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

        [HttpGet]
        [Route("api/Business/GetAllMasterProExtraInformationServiceDetailById")]
        public HttpResponseMessage GetAllMasterProExtraInformationServiceDetailById(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();


                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                List<MasterProExtraInformation_VM> resp = new List<MasterProExtraInformation_VM>();

                resp = masterProExtraInformationService.GetAllMasterProExtraInformationById(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    MasterproDetail = resp,

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

        #endregion --------------------------------------------------------------------------------

        [HttpGet]
        [Route("api/Business/GetAllExperienceServiceDetailById")]
        public HttpResponseMessage GetAllExperienceServiceDetailById(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<UserExperienceDetail_ViewModel> resp = new List<UserExperienceDetail_ViewModel>();
                resp = userService.GetUserExperienceDetail(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    ExperienceDetail = resp,

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

        [HttpGet]
        [Route("api/Business/GetAllEducationServiceDetailById")]
        public HttpResponseMessage GetAllEducationServiceDetailById(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<UserEducation_ViewModel> resp = new List<UserEducation_ViewModel>();

                resp = userService.GetBusinessEducationDetaillst_Get(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    EducationDetail = resp,

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


        // Contact Information 
        /// <summary>
        /// To Add/Update Contact Detail For VisitorPanel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateContactInformationDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateContactInformationDetail_PPCMeta()
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
                BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentContactInformationViewModel businessContentContactInformationViewModel = new BusinessContentContactInformationViewModel();
                businessContentContactInformationViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);

                // Check and set Title 

                string TitleParam = HttpRequest.Params["Title"];
                businessContentContactInformationViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                businessContentContactInformationViewModel.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                businessContentContactInformationViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files

                // Validate infromation passed
                Error_VM error_VM = businessContentContactInformationViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdatBusinessContactContactInformationDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentContactInformation_Param_VM
                {
                    Id = businessContentContactInformationViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessContentContactInformationViewModel.Title,
                    Description = businessContentContactInformationViewModel.Description,
                    Mode = businessContentContactInformationViewModel.Mode
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
        /// To Get Contact Information Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentContactInformationDetail_PPCMeta")]
        public HttpResponseMessage GetBusinessContentContactInformationDetail_PPCMeta()
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




                BusinessContentContactInformation_VM resp = new BusinessContentContactInformation_VM();
                resp = businessOwnerService.GetBusinessContactInformation(_BusinessOwnerLoginId);

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


        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetSingleInstructorDetail")]
        public HttpResponseMessage GetSingleInstructorDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                InstructorList_VM businessInstructorDetail = new InstructorList_VM();
                businessInstructorDetail = businessOwnerService.GetBusinessSingleInstructorDetail(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {

                    businessInstructorDetail = businessInstructorDetail,

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
        /// To Get Follower and Following Count Detail by businessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetFollowerFollowingDetail")]
        public HttpResponseMessage GetFollowerFollowingDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessFollowerDetail_VM businessInstructorDetail = new BusinessFollowerDetail_VM();
                businessInstructorDetail = businessOwnerService.GetBusinessInstructorFollowingDetail(businessOwnerLoginId);

                BusinessFollowerDetail_VM businessFollowerInstructorDetail = new BusinessFollowerDetail_VM();
                businessFollowerInstructorDetail = businessOwnerService.GetBusinessInstructorFollowerDetail(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {

                    businessInstructorFollowingDetail = businessInstructorDetail,
                    BusinessInstructorFollowerDetail = businessFollowerInstructorDetail,

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



        ///// <summary>
        ///// get all course category list
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        ////[Authorize(Roles = "BusinessAdmin,Staff")]
        //[Route("api/Business/GetAllUserCourseCategoryListDetail")]
        //public HttpResponseMessage GetAllUserCourseCategoryListDetail()
        //{

        //    ApiResponse_VM apiResponse = new ApiResponse_VM();
        //    try
        //    {


        //        List<BusinessContentCourseCategoryDetail_PPCMeta> usercontentcategorydetailresponse = businessCategoryService.GetCourseCategoryDetailList();


        //        apiResponse.status = 1;
        //        apiResponse.message = "success";
        //        apiResponse.data = usercontentcategorydetailresponse;


        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.status = -500;
        //        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //}



        /// <summary>
        /// ttt
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllCourseCategoryDetailList")]
        public HttpResponseMessage GetAllUserCourseCategoryListDetail()
        {

            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessContentCourseCategoryDetail_PPCMeta> usercontentcategorydetailresponse = businessCategoryService.GetCourseCategoryDetailList();


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = usercontentcategorydetailresponse;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }


        /// <summary>
        /// To Get Main Plans Detail  By Id 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetMainPlanDetailById")]
        public HttpResponseMessage GetMainPlanDetailById(long itemId)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",itemId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4")
                            };

                mainPlan = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).FirstOrDefault();

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
        /// to get skills details of other staff
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/Business/GetOtherStaffskillDetail")]
        public HttpResponseMessage GetOtherStaffskillDetail(long userLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessContentUserResumeDetail_VM Userdetails = new BusinessContentUserResumeDetail_VM();

                Userdetails = userService.GetUserContentResumeDetail(userLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = Userdetails;

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
        /// get room detail for visitor panel
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetRoomdetailsForVisitorView")]
        public HttpResponseMessage GetRoomdetailsForVisitorView(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (businessOwnerLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _roomdetails = userService.GetRoomDetail(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _roomdetails;
               

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
        /// To Get tennis room Booking  Detail 
        /// </summary>
        /// <param name="classDays"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetTennisBookingDetail")]
        public HttpResponseMessage GetTennisBookingDetail()
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
                List<TennisBokingDetail_VM> resp = userService.GetTennisBookingDetailList(_LoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Business/DashboardData/Get")]
        public HttpResponseMessage GetAdminDashboardDetails()
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


                var businessProfilePageTypeDetail = businessOwnerService.GetBusinessDetailsDashboard(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessProfilePageTypeDetail;

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