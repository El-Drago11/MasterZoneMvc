using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.Views;
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
    public class TrainingAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private TrainingService trainingService;
        private FileHelper fileHelper;
        private CertificateService certificateService;
        private ReviewService reviewService;

        public TrainingAPIController()
        {
            db = new MasterZoneDbContext();
            trainingService = new TrainingService(db);
            fileHelper = new FileHelper();
            certificateService = new CertificateService(db);
            reviewService = new ReviewService(db);
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
        /// To Add Training Classes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateTrainingInformation")]
        public HttpResponseMessage AddUpdateBusinessTrainingProfile()
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
                RequestTrainingClass_VM trainingClass_VM = new RequestTrainingClass_VM();
                trainingClass_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                trainingClass_VM.InstructorUserLoginId = Convert.ToInt64(HttpRequest.Params["InstructorUserLoginId"]);
                trainingClass_VM.TrainingName = HttpRequest.Params["TrainingName"].Trim();
                trainingClass_VM.ShortDescription = HttpRequest.Params["ShortDescription"].Trim();
                trainingClass_VM.InstructorEmail = HttpRequest.Params["InstructorEmail"].Trim();
                trainingClass_VM.InstructorMobileNumber = HttpRequest.Params["InstructorMobileNumber"].Trim();
                trainingClass_VM.InstructorAlternateNumber = HttpRequest.Params["InstructorAlternateNumber"].Trim();
                trainingClass_VM.IsPaid = Convert.ToInt32(HttpRequest.Params["IsPaid"]);
                trainingClass_VM.Price = Convert.ToDecimal(HttpRequest.Params["Price"]);
                trainingClass_VM.AdditionalPriceInformation = HttpRequest.Params["AdditionalPriceInformation"].Trim();
                trainingClass_VM.CenterName = HttpRequest.Params["CenterName"].Trim();
                trainingClass_VM.Location = HttpRequest.Params["Location"].Trim();
                trainingClass_VM.Address = HttpRequest.Params["Address"].Trim();
                trainingClass_VM.City = HttpRequest.Params["City"].Trim();
                trainingClass_VM.State = HttpRequest.Params["State"].Trim();
                trainingClass_VM.Country = HttpRequest.Params["Country"].Trim();
                trainingClass_VM.PinCode = HttpRequest.Params["PinCode"].Trim();
                trainingClass_VM.LocationUrl = HttpRequest.Params["LocationUrl"].Trim();
                trainingClass_VM.StartDate = HttpRequest.Params["StartDate"].Trim();
                trainingClass_VM.EndDate = HttpRequest.Params["EndDate"].Trim();
                trainingClass_VM.StartTime_24HF = HttpRequest.Params["StartTime_24HF"].Trim();
                trainingClass_VM.EndTime_24HF = HttpRequest.Params["EndTime_24HF"].Trim();
                //trainingClass_VM.Description = HttpRequest.Params["Description"].Trim();
                trainingClass_VM.Description = HttpUtility.UrlDecode(HttpRequest.Form.Get("Description"));
                trainingClass_VM.MusicType = HttpRequest.Params["MusicType"].Trim();
                trainingClass_VM.EnergyLevel = HttpRequest.Params["EnergyLevel"].Trim();
                trainingClass_VM.DanceStyle = HttpRequest.Params["DanceStyle"].Trim();
                trainingClass_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                trainingClass_VM.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLatitude"]) : 0;
                trainingClass_VM.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLongitude"]) : 0;
                trainingClass_VM.TotalClasses = Convert.ToInt32(HttpRequest.Params["TotalClasses"]);
                trainingClass_VM.Duration = HttpRequest.Params["Duration"];
                trainingClass_VM.TrainingClassDays = HttpRequest.Params["TrainingClassDays"];
                trainingClass_VM.TotalSeats = Convert.ToInt32(HttpRequest.Params["TotalSeats"]);
                trainingClass_VM.TotalCredits = Convert.ToInt32(HttpRequest.Params["TotalCredits"]);
                trainingClass_VM.TotalLectures = Convert.ToInt32(HttpRequest.Params["TotalLectures"]);
                trainingClass_VM.ExpectationDescription = HttpUtility.UrlDecode(HttpRequest.Form.Get("ExpectationDescription"));
                trainingClass_VM.AdditionalInformation = HttpUtility.UrlDecode(HttpRequest.Form.Get("AdditionalInformation"));
                trainingClass_VM.BecomeInstructorDescription = HttpUtility.UrlDecode(HttpRequest.Form.Get("BecomeInstructorDescription"));
                trainingClass_VM.TrainingRules = HttpUtility.UrlDecode(HttpRequest.Form.Get("TrainingRules"));
                trainingClass_VM.LicenseBookingId = Convert.ToInt64(HttpRequest.Params["LicenseBookingId"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _TrainingImageFile = files["TrainingImage"];
                trainingClass_VM.TrainingImage = _TrainingImageFile; // for validation
                string _TrainingImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousTrainingImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = trainingClass_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_TrainingImageFile != null)
                    {
                        _TrainingImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_TrainingImageFile);
                    }
                }

                // Check if License Booking
                LicenseService licenseService = new LicenseService(db);
                var licenseBookingInfo = licenseService.GetLicenseBookingDetailForBOById(trainingClass_VM.LicenseBookingId);
                var licenseAvailableCount = licenseService.GetBookedLicensesAvailableQuantity(_BusinessOwnerLoginId, trainingClass_VM.LicenseBookingId);

                if(licenseBookingInfo == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.LicenseNotFound_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                // Only check while creating new training wheater license quantity is uesd or not. On update it is checked in update-mode
                if(trainingClass_VM.Mode == 1 && licenseAvailableCount <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message =  Resources.BusinessPanel.LicenseQuantityFinished_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                if(trainingClass_VM.Price < licenseBookingInfo.LicenseMinSellingPrice)
                {
                    apiResponse.status = -1;
                    apiResponse.message =  String.Format(Resources.BusinessPanel.TrainingPriceGreaterThenMinLicensePrice_Required, licenseBookingInfo.LicenseMinSellingPrice);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (trainingClass_VM.Mode == 2)
                {

                    var respGetTrainingData = trainingService.GetTrainingDataById(trainingClass_VM.Id);
                    // if already that license in not included and its quantiy is used then return error.
                    if (respGetTrainingData.LicenseBookingId != trainingClass_VM.LicenseBookingId && licenseAvailableCount <= 0)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = Resources.BusinessPanel.LicenseQuantityFinished_ErrorMessage;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _TrainingImageFileNameGenerated = respGetTrainingData.TrainingImage ?? "";
                    }
                    else
                    {
                        _PreviousTrainingImageFileName = respGetTrainingData.TrainingImage ?? "";
                    }
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",trainingClass_VM.Id ),
                            new SqlParameter("instructorUserLoginId", trainingClass_VM.InstructorUserLoginId),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("trainingname", trainingClass_VM.TrainingName),
                            new SqlParameter("shortdescription", trainingClass_VM.ShortDescription),
                            new SqlParameter("instructoremail", trainingClass_VM.InstructorEmail),
                            new SqlParameter("instructorMobileNumber", trainingClass_VM.InstructorMobileNumber),
                            new SqlParameter("instructoralternateNumber", trainingClass_VM.InstructorAlternateNumber),
                            new SqlParameter("address", trainingClass_VM.Address),
                            new SqlParameter("isPaid", trainingClass_VM.IsPaid),
                            new SqlParameter("price ", trainingClass_VM.Price),
                            new SqlParameter("additionalpriceinformation", trainingClass_VM.AdditionalPriceInformation),
                            new SqlParameter("country", trainingClass_VM.Country),
                            new SqlParameter("state", trainingClass_VM.State),
                            new SqlParameter("city", trainingClass_VM.City),
                            new SqlParameter("startDate", trainingClass_VM.StartDate),
                            new SqlParameter("endDate", trainingClass_VM.EndDate),
                            new SqlParameter("startTime_24HF", trainingClass_VM.StartTime_24HF),
                            new SqlParameter("endTime_24HF", trainingClass_VM.EndTime_24HF),
                            new SqlParameter("pinCode", trainingClass_VM.PinCode),
                            new SqlParameter("locationUrl", trainingClass_VM.LocationUrl),
                            new SqlParameter("description", trainingClass_VM.Description),
                            new SqlParameter("musicType", trainingClass_VM.MusicType),
                            new SqlParameter("energyLevel", trainingClass_VM.EnergyLevel),
                            new SqlParameter("danceStyle", trainingClass_VM.DanceStyle),
                            new SqlParameter("location", trainingClass_VM.Location),
                            new SqlParameter("centerName", trainingClass_VM.CenterName),
                            new SqlParameter("submittedByLoginId", _LoginID_Exact),
                            new SqlParameter("mode", trainingClass_VM.Mode),
                            new SqlParameter("trainingImage", _TrainingImageFileNameGenerated),
                            new SqlParameter("latitude",trainingClass_VM.Latitude),
                            new SqlParameter("longitude",trainingClass_VM.Longitude),
                            new SqlParameter("duration", trainingClass_VM.Duration),
                            new SqlParameter("trainingClassDays", trainingClass_VM.TrainingClassDays),
                            new SqlParameter("totalLectures", trainingClass_VM.TotalLectures),
                            new SqlParameter("totalClasses", trainingClass_VM.TotalClasses),
                            new SqlParameter("totalSeats",trainingClass_VM.TotalSeats),
                            new SqlParameter("additionalInformation", trainingClass_VM.AdditionalInformation),
                            new SqlParameter("trainingRules", trainingClass_VM.TrainingRules),
                            new SqlParameter("totalCredits", trainingClass_VM.TotalCredits),
                            new SqlParameter("expectationDescription", trainingClass_VM.ExpectationDescription),
                            new SqlParameter("becomesInstructordescription", trainingClass_VM.BecomeInstructorDescription),
                            new SqlParameter("licenseBookingId", trainingClass_VM.LicenseBookingId),
                            };
                
                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTraining @id,@instructorUserLoginId,@userLoginId,@trainingname,@shortdescription,@instructoremail,@instructorMobileNumber,@instructoralternateNumber,@address,@isPaid,@price,@additionalpriceinformation,@country,@state,@city,@startDate,@endDate,@startTime_24HF,@endTime_24HF,@pinCode,@locationUrl,@description,@musicType,@energyLevel,@danceStyle,@location,@centerName,@submittedByLoginId,@mode,@trainingImage,@latitude,@longitude,@duration,@trainingClassDays,@totalLectures, @totalClasses,@totalSeats,@additionalInformation,@trainingRules,@totalCredits,@expectationDescription,@becomesInstructordescription,@licenseBookingId", queryParams).FirstOrDefault();

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
                        if (!String.IsNullOrEmpty(_PreviousTrainingImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_TrainingImage), _PreviousTrainingImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_TrainingImage), _TrainingImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_TrainingImageFile, FileWithPath);
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
        /// Get All Instructor List linked with the BusinessOnwer
        /// </summary>
        /// <returns>List of All-Business-Instructor</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllInstructor")]
        public HttpResponseMessage GetAllInstructorListByBusiness()
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

                List<InstructorList_VM> lst = new List<InstructorList_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", "0"),
                          new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                          new SqlParameter("mode", "1")
                           };

                var resp = db.Database.SqlQuery<InstructorList_VM>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

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
        /// Get All Training with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllTrainingByPagination")]
        public HttpResponseMessage GetAllTrainingDataTablePagination()
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

                Training_Pagination_SQL_Params_VM _Params_VM = new Training_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = trainingService.GetBusinessInstructorList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Training  Detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/TrainingDetailGetById/{id}")]
        public HttpResponseMessage GetById(int id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Training-Record-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                             new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "2")
                            };

                var response = db.Database.SqlQuery<TrainingClass_VM>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// To delete the Training Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/TrainingDeleteById/{id}")]
        public HttpResponseMessage DeleteDiscountById(long id)
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


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",id ),
                            new SqlParameter("instructorUserLoginId", ""),
                            new SqlParameter("userLoginId", _UserLoginId),
                             new SqlParameter("trainingname", ""),
                            new SqlParameter("shortdescription", ""),
                            new SqlParameter("instructoremail", ""),
                            new SqlParameter("instructorMobileNumber",""),
                            new SqlParameter("instructoralternateNumber", ""),
                            new SqlParameter("address", ""),
                            new SqlParameter("isPaid",""),
                             new SqlParameter("price ","0"),
                            new SqlParameter("additionalpriceinformation", ""),
                            new SqlParameter("country", ""),
                            new SqlParameter("state", ""),
                            new SqlParameter("city",""),
                            new SqlParameter("startDate", ""),
                             new SqlParameter("endDate", ""),
                            new SqlParameter("startTime_24HF",""),
                            new SqlParameter("endTime_24HF", ""),
                            new SqlParameter("pinCode", ""),
                            new SqlParameter("locationUrl", ""),
                            new SqlParameter("description", ""),
                            new SqlParameter("musicType", ""),
                            new SqlParameter("energyLevel", ""),
                            new SqlParameter("danceStyle", ""),
                            new SqlParameter("location", ""),
                            new SqlParameter("centerName", ""),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("trainingImage", ""),
                            new SqlParameter("latitude","0"),
                            new SqlParameter("longitude","0"),
                            new SqlParameter("duration", ""),
                            new SqlParameter("trainingClassDays", ""),
                            new SqlParameter("totalLectures", "0"),
                            new SqlParameter("totalClasses", "0"),
                            new SqlParameter("totalSeats","0"),
                            new SqlParameter("additionalInformation", ""),
                            new SqlParameter("trainingRules", ""),
                            new SqlParameter("totalCredits", "0"),
                            new SqlParameter("expectationDescription", ""),
                            new SqlParameter("becomesInstructordescription", ""),
                            new SqlParameter("businessLicenseId", "0"),

                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTraining @id,@instructorUserLoginId,@userLoginId,@trainingname,@shortdescription,@instructoremail,@instructorMobileNumber,@instructoralternateNumber,@address,@isPaid,@price,@additionalpriceinformation,@country,@state,@city,@startDate,@endDate,@startTime_24HF,@endTime_24HF,@pinCode,@locationUrl,@description,@musicType,@energyLevel,@danceStyle,@location,@centerName,@submittedByLoginId,@mode,@trainingImage,@latitude,@longitude,@duration,@trainingClassDays,@totalLectures, @totalClasses,@totalSeats,@additionalInformation,@trainingRules,@totalCredits,@expectationDescription,@becomesInstructordescription,@businessLicenseId", queryParams).FirstOrDefault();

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
        /// Get All Training with Pagination For the Super-Admin-Panel [Super, SubAdmin]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns>All Trainings created by business owners list with pagination</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Training/GetAllTrainingsByPaginationForSuperAdmin")]
        [HttpPost]
        public HttpResponseMessage GetAllTrainingsByPaginationForSuperAdmin()
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

                Training_Pagination_SQL_Params_VM _Params_VM = new Training_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.LoginId = _LoginId;

                var paginationResponse = trainingService.GetAllTrainingsList_Pagination_ForSuperAdminPanel(HttpRequestParams, _Params_VM);


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
        /// To change the Training Home Visibility Status - [Super-Admin-Panel]
        /// </summary>
        /// <param name="id">Training-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Training/ToggleHomePageVisibilityStatus/{id}")]
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
                            new SqlParameter("id",id ),
                            new SqlParameter("instructorUserLoginId", ""),
                            new SqlParameter("userLoginId", _LoginId),
                             new SqlParameter("trainingname", ""),
                            new SqlParameter("shortdescription", ""),
                            new SqlParameter("instructoremail", ""),
                            new SqlParameter("instructorMobileNumber",""),
                            new SqlParameter("instructoralternateNumber", ""),
                            new SqlParameter("address", ""),
                            new SqlParameter("isPaid",""),
                             new SqlParameter("price ","0"),
                            new SqlParameter("additionalpriceinformation", ""),
                            new SqlParameter("country", ""),
                            new SqlParameter("state", ""),
                            new SqlParameter("city",""),
                            new SqlParameter("startDate", ""),
                             new SqlParameter("endDate", ""),
                            new SqlParameter("startTime_24HF",""),
                            new SqlParameter("endTime_24HF", ""),
                            new SqlParameter("pinCode", ""),
                            new SqlParameter("locationUrl", ""),
                            new SqlParameter("description", ""),
                            new SqlParameter("musicType", ""),
                            new SqlParameter("energyLevel", ""),
                            new SqlParameter("danceStyle", ""),
                            new SqlParameter("location", ""),
                            new SqlParameter("centerName", ""),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode", "4"),
                            new SqlParameter("trainingImage", ""),
                            new SqlParameter("latitude","0"),
                            new SqlParameter("longitude","0"),
                            new SqlParameter("duration", ""),
                            new SqlParameter("trainingClassDays", ""),
                            new SqlParameter("totalLectures", "0"),
                            new SqlParameter("totalClasses", "0"),
                            new SqlParameter("totalSeats","0"),
                            new SqlParameter("additionalInformation", ""),
                            new SqlParameter("trainingRules", ""),
                            new SqlParameter("totalCredits", "0"),
                            new SqlParameter("expectationDescription", ""),
                            new SqlParameter("becomesInstructordescription", ""),
                            new SqlParameter("businessLicenseId", "0"),

                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTraining @id,@instructorUserLoginId,@userLoginId,@trainingname,@shortdescription,@instructoremail,@instructorMobileNumber,@instructoralternateNumber,@address,@isPaid,@price,@additionalpriceinformation,@country,@state,@city,@startDate,@endDate,@startTime_24HF,@endTime_24HF,@pinCode,@locationUrl,@description,@musicType,@energyLevel,@danceStyle,@location,@centerName,@submittedByLoginId,@mode,@trainingImage,@latitude,@longitude,@duration,@trainingClassDays,@totalLectures, @totalClasses,@totalSeats,@additionalInformation,@trainingRules,@totalCredits,@expectationDescription,@becomesInstructordescription,@businessLicenseId", queryParams).FirstOrDefault();


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
        /// Get All Trainings For Home Page display
        /// </summary>
        /// <returns>Trainings List</returns>
        [HttpGet]
        [Route("api/Training/GetAllTrainingsForHomePage")]
        public HttpResponseMessage GetAllCertificatesForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<TrainingList_ForHomePage_VM> response = trainingService.GetAllTrainingListForHomePage();

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
        /// Get All Active Business Trainings users-screen
        /// </summary>
        /// <param name="City">filter by city name</param>
        /// <param name="lastRecordId">Last fetched record Id</param>
        /// <param name="recordLimit">no. of records to fetch</param>
        /// <returns>List of Trainings</returns>
        [HttpGet]
        [Route("api/Training/GetAllBusinessTrainingListForUser")]
        public HttpResponseMessage GetAllTrainingForBusiness(string City, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<TrainingList_ForVisitorPanel_VM> response = trainingService.GetAllTrainingList(City, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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
        [Route("api/Training/GetAllTraining")]
        public HttpResponseMessage GetAllTrainingList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<UserTrainingsDetail_VM> trainingresponse = trainingService.GetAllTraining(lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    TrainingList = trainingresponse,
                };

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
        /// To Get Business Training Detail With Sports 
        /// </summary>
        /// <param name="businessSubCategoryId">Business Sub-Category Id</param>
        /// <returns></returns>
        [HttpGet]
        //  [Authorize(Roles = "Student")]
        [Route("api/Training/TrainingDetailGet")]
        public HttpResponseMessage GetTrainingDetailInstructorResearch(string menuTag, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<TrainingDetail_VisitorPanel_VM> trainingResponseDetail = trainingService.GetTrainingDetailListByMenuTag(menuTag, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = trainingResponseDetail;

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
        /// To Get Instructor Detail In Sports Page For Visitor-Panel  -- REPLACED WITH OTHER API NOT TO USE THIS
        /// </summary>
        /// <param name="businessSubCategoryId">Business Sub-Category Id</param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllInstructorDetail")]
        public HttpResponseMessage GetAllInstructorListDetail(string menuTag, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<InstructorList_VM> allinstructordetailList = trainingService.GetAllInstructorDetailByMenuTag(menuTag, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = allinstructordetailList;

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
        /// To Get Training Detail By Location
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student")]
        [Route("api/Training/GetAllTrainingByLocation")]
        public HttpResponseMessage GetAllTrainingByLocation(string searchkeyword = "", string searchBy = "", long lastRecordId = 0, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<TrainingList_ForVisitorPanel_VM> traininglocationresponse = trainingService.GetTrainingListByLocation(searchkeyword, searchBy, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    TrainingLocationList = traininglocationresponse,
                };

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
        //[Authorize(Roles = "Student")]
        [Route("api/Training/TrainingCertificationDetailGet")]
        public HttpResponseMessage GetTrainingDetailById(int id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}
                if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
                TrainingDetail_VisitorPanel_VM trainingResponse = new TrainingDetail_VisitorPanel_VM();

                trainingResponse = trainingService.GetTrainingDetailsById(id);
                // TrainingDetail_VisitorPanel_VM traininginstructorResponse = new TrainingDetail_VisitorPanel_VM();
                TrainingDetail_VisitorPanel_VM traininginstructorResponse = new TrainingDetail_VisitorPanel_VM();



                traininginstructorResponse = trainingService.GetTrainingInstructorDetailById(id);

                TrainingDescriptionList_VM trainingDescriptionList = new TrainingDescriptionList_VM();
                trainingDescriptionList = trainingService.GetTrainingDescriptionById(id);

                List<BusinessTrainingReviewsDetail> trainingRatingList = reviewService.GetTrainingRating(trainingResponse.UserLoginId);

                traininginstructorResponse.certificateIconDetail = certificateService.GetCertificateIcon(trainingResponse.UserLoginId);

                //List<TrainingCertificateIconDetail> certificateIconDetail =  trainingService.GetCertificateIcon(UserLoginId);

                BusinessReviewDetail businessReviewDetail = new BusinessReviewDetail();
                businessReviewDetail = reviewService.GetBusinessReviewDetail(trainingResponse.UserLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    TrainingDetails = trainingResponse,
                    TrainingInstructorList = traininginstructorResponse,
                    TrainingDescriptionList = trainingDescriptionList,
                    TrainingReviewDetail = trainingRatingList,
                    BusinessReviewDetail = businessReviewDetail,

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
        /// Get Active Training Detail for all users. [public]
        /// </summary>
        /// <param name="trainingId">Training-Id</param>
        /// <returns>Training Detail</returns>
        [HttpGet]
        [Route("api/Training/GetActiveTrainingDetialByIdForUser")]
        public HttpResponseMessage GetActiveTrainingDetialByIdForUser(int trainingId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (trainingId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                TrainingDetail_VisitorPanel_VM resp = new TrainingDetail_VisitorPanel_VM();

                resp = trainingService.GetTrainingDetailsById(trainingId);

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.PlanNotFound_ErrorMessage;
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
        /// Get All Training-Bookings of students with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <param name="trainingId">Training Id</param>
        /// <returns>Training booked by students list</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/GetAllTrainingBookingForBOByPagination")]
        public HttpResponseMessage GetAllTrainingBookingForBOByPagination(long trainingId)
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

                Training_Pagination_SQL_Params_VM _Params_VM = new Training_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Id = trainingId;
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = trainingService.GetTrainingBookingList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get All Training List by Business
        /// </summary>
        /// <returns>Business Trainings list</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/GetAllAvailableTrainingByBusiness")]
        public HttpResponseMessage GetAllAvailableTrainingByBusiness()
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

                List<AvailableTraining_VM> training_VM = new List<AvailableTraining_VM>();
                // Get Available trainings
                training_VM = trainingService.GetAllAvailableTrainingForBooking(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = training_VM;

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
        /// Get Training Booking Detail by Training-Booking-Id
        /// </summary>
        /// <param name="trainingBookingId">Training Booking Id</param>
        /// <returns>Returns Training Booking Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/TrainingBookingDetailById")]
        public HttpResponseMessage GetTrainingBookingDetailById(long trainingBookingId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (trainingBookingId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Training-Booking-Detail
                var trainingBookingDetail = trainingService.GetTrainingBookingDetailById(trainingBookingId);

                // Get Training-Detail
                TrainingDetail_VisitorPanel_VM trainingDetail = new TrainingDetail_VisitorPanel_VM();
                trainingDetail = trainingService.GetTrainingDetailsById(trainingBookingDetail.TrainingId);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(trainingBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    TrainingDetail = trainingDetail,
                    TrainingBookingDetail = trainingBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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
        /// Mark Training Completed of student by Training-Booking-Id
        /// - Marks Complete
        /// - Generates and save Training Certificate
        /// - Due to completion upgrades user account according to certification(Individual account)
        /// </summary>
        /// <param name="trainingBookingId">Training Booking Id</param>
        /// <returns>Returns success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/TrainingBooking/MarkComplete")]
        public HttpResponseMessage MarkTrainingBookingCompleteById(long trainingBookingId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (trainingBookingId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Training-Booking-Detail
                var trainingBookingDetail = trainingService.GetTrainingBookingDetailById(trainingBookingId);

                // Get Training-Detail
                TrainingDetail_VisitorPanel_VM trainingDetail = new TrainingDetail_VisitorPanel_VM();
                trainingDetail = trainingService.GetTrainingDetailsById(trainingBookingDetail.TrainingId);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(trainingBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    TrainingDetail = trainingDetail,
                    TrainingBookingDetail = trainingBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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
        /// To Get Trainings details by search filteration - VISITOR PANEL
        /// </summary>
        /// <returns>Filtered Trainings</returns>
        [HttpPost]
        [Route("api/Training/GetTrainingsBySearchFilter")]
        public HttpResponseMessage GetTrainingsBySearchFilter(SearchFilter_APIParmas_VM filterParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<TrainingDetail_VisitorPanel_VM> trainingResponseDetail = trainingService.GetTrainingsListBySearchFilter(filterParams);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = trainingResponseDetail;

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
        /// To Get Training Detail For Without using Licensed  
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/GetAllTrainingDetailByBusinessForVisitorPanel")]
        public HttpResponseMessage GetAllTrainingDetailByBusiness(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessTrainingDetail_VM> training_VM = new List<BusinessTrainingDetail_VM>();
                // Get Available trainings
                training_VM = trainingService.GetAllTrainingDetailForBusiness(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = training_VM;

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
        /// To get Licensed Certificate Detail for Training List 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/GetAllTrainingLicensedDetailByBusiness")]
        public HttpResponseMessage GetAllTrainingLicensedDetailByBusiness(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessTrainingDetail_VM> training_VM = new List<BusinessTrainingDetail_VM>();
                // Get Available trainings
                training_VM = trainingService.GetAllTrainingLicensedDetailForBusiness(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = training_VM;

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
        /// get training details for my booking 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Training/GetTrainingDetails")]
        public HttpResponseMessage GetTrainingDetails()
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

                // Get Training-Record-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                             new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("mode", "3")
                };

                var response = db.Database.SqlQuery<TrainingBooking_ViewModel>("exec sp_ManageTrainingBooking @id,@userLoginId,@businessOwnerLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// get training details for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Training/GetTrainingBookingDetailsById")]
        public HttpResponseMessage GetTrainingBookingDetailsById(long id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Class-Booking-Detail
                var trainingBookingDetail = trainingService.GetTrainingBookingDetailbyId(id, _LoginID_Exact);

                // Get Class-Detail  
                //TrainingBooking_ViewModel trainingbookingDetail = new TrainingBooking_ViewModel();
                //trainingbookingDetail = trainingService.GetTrainingBookingDetail(trainingBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(trainingBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    // TrainingDetail = trainingbookingDetail,
                    TrainingBookingDetail = trainingBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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
        /// To Get Business Booking Training Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/GetBusinessBookingTrainingDetails")]
        public HttpResponseMessage GetBusinessBookingTrainingDetails()
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

                // Get Training-Record-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                             new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("mode", "5")
                            };

                var response = db.Database.SqlQuery<TrainingBooking_ViewModel>("exec sp_ManageTrainingBooking @id,@userLoginId,@businessOwnerLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// get  business training details for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Training/GetBusinessTrainingBookingDetailsById")]
        public HttpResponseMessage GetBusinessTrainingBookingDetailsById(long id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Class-Booking-Detail
                var trainingBookingDetail = trainingService.GetBusinessTrainingBookingDetailById(id, _LoginID_Exact);

                // Get Class-Detail  
                TrainingBooking_ViewModel trainingbookingDetail = new TrainingBooking_ViewModel();
                trainingbookingDetail = trainingService.GetBusinessTrainingBookingDetail(trainingBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(trainingBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    TrainingDetail = trainingbookingDetail,
                    TrainingBookingDetail = trainingBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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
        /// To Get Training Instructor Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllTrainingInstructorDetail")]
        public HttpResponseMessage GetAllTrainingInstructorDetail()
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

                List<InstructorList_VM> lst = new List<InstructorList_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", "0"),
                          new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                          new SqlParameter("mode", "14")
                           };

                var resp = db.Database.SqlQuery<InstructorList_VM>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

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

    }
}