using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
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
using System.IO;
using MasterZoneMvc.Common.ValidationHelpers;
using Org.BouncyCastle.Utilities;
using System.Data.SqlClient;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessContentTennisAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessContentTennisService businessContentTennisService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessContentTennisAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessContentTennisService = new BusinessContentTennisService(db);
            businessOwnerService = new BusinessOwnerService(db);
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
        /// To Add Business  Content Tennis Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessTennis")]

        public HttpResponseMessage AddUpdateBusinessTennisProfile()
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
    
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
               
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentTennis_VM businessContentTennis_VM = new BusinessContentTennis_VM();

                // Parse and assign values from HTTP request parameters
                businessContentTennis_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentTennis_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentTennis_VM.SubTitle = HttpRequest.Params["SubTitle"].Trim();
                businessContentTennis_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentTennis_VM.BasicPrice = Convert.ToDecimal(HttpRequest.Params["BasicPrice"]);
                businessContentTennis_VM.CommercialPrice = Convert.ToDecimal(HttpRequest.Params["CommercialPrice"]);
                businessContentTennis_VM.OtherPrice = Convert.ToDecimal(HttpRequest.Params["OtherPrice"]);
                businessContentTennis_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssTennisImageFile = files["TennisImage"];
                businessContentTennis_VM.TennisImage = _BusinesssTennisImageFile; // for validation
                string _BusinessTennisImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessTennisImageFileName = ""; // will be used to delete file while updating.

                // Validate information passed
                Error_VM error_VM = businessContentTennis_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

               

                if (files.Count > 0)
                {
                    if (_BusinesssTennisImageFile != null)
                    {

                        _BusinessTennisImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssTennisImageFile);
                    }

                }


                if (businessContentTennis_VM.Mode == 2)
                {

                    var respGetBusinessBannerDetail = businessContentTennisService.GetBusinessContentTennisDetail(_BusinessOwnerLoginId);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessTennisImageFileNameGenerated = respGetBusinessBannerDetail.TennisImage ?? "";

                    }
                    else
                    {
                        _PreviousBusinessTennisImageFileName = respGetBusinessBannerDetail.TennisImage ?? "";

                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentTennis_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentTennis_Param_VM
                {
                    Id = businessContentTennis_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentTennis_VM.Title,
                    SubTitle = businessContentTennis_VM.SubTitle,
                    Description = businessContentTennis_VM.Description,
                    TennisImage = _BusinessTennisImageFileNameGenerated,
                    BasicPrice = businessContentTennis_VM.BasicPrice,
                    CommercialPrice = businessContentTennis_VM.CommercialPrice,
                    OtherPrice = businessContentTennis_VM.OtherPrice,
                    Mode = businessContentTennis_VM.Mode
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
                    #region Insert-Update Tennis Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessTennisImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessTennisImage), _PreviousBusinessTennisImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessTennisImage), _BusinessTennisImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssTennisImageFile, NewImageFileWithPath);


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
        /// To Get Business Tennis Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentTennisDetailById/{id}")]
        public HttpResponseMessage GetBusinessContentTennis(long Id)
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


                BusinessContentTennisDetail_VM resp = new BusinessContentTennisDetail_VM();

                resp = businessContentTennisService.GetBusinessContentTennisDetail_ById(Id);



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
        /// To Get Business Tennis Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentTennis")]
        public HttpResponseMessage GetBusinessContentTennisDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                 List<BusinessContentTennisDetail_VM> businessContentTennislst = businessContentTennisService.GetBusinessContentTennisDetail_List(businessOwnerLoginId);

                BusinessContentTennisDetail_VM businessContentTennisDetail = new BusinessContentTennisDetail_VM();

                businessContentTennisDetail = businessContentTennisService.GetBusinessContentTennisDetail(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data =  new {
                    BusinessContentTennisList= businessContentTennislst,
                    BusinessContentTennisDetail = businessContentTennisDetail,

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
        /// Get All Business Content Tennis with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllTennisDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentDataTablePagination()
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

                TennisDetail_Pagination_SQL_Params_VM _Params_VM = new TennisDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentTennisService.GetBusinessContentTennisList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Delete Tennis Detail
        /// </summary>
        /// <param name="id">Tennis Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteTennisDetail")]
        public HttpResponseMessage DeleteTennisById(long id)
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

                // Delete Content Tennis 
                resp = businessContentTennisService.DeleteTennisDetail(id);


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
        /// Add/Update Tennis  Time Slot
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateTennisTimeSlot")]


        public HttpResponseMessage AddUpdateTennisTimeSlot()
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

                // Retrieve time values as a single string from the request parameters
                string timeValuesString = HttpRequest.Params["Time"];

                // Check if timeValuesString is not null or empty
                if (!string.IsNullOrEmpty(timeValuesString))
                {
                                                       
                    string[] timeList = timeValuesString
                        .Trim('[', ']') // Remove square brackets
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) // Split by commas
                        .Select(time => time.Trim('"')) // Remove double quotes from each time value
                        .ToArray();
                    SPResponseViewModel resp1 = null;

                    foreach (string time in timeList)
                    {
                        // Create a new instance of TennisAreaTimeSlotViewModel for each time value
                        TennisAreaTimeSlotViewModel tennisAreaTimeSlotViewModel = new TennisAreaTimeSlotViewModel();

                        // Parse and assign values from HTTP request parameters
                        tennisAreaTimeSlotViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                        tennisAreaTimeSlotViewModel.SlotId = Convert.ToInt64(HttpRequest.Params["SlotId"]);
                        tennisAreaTimeSlotViewModel.Time = time;
                        tennisAreaTimeSlotViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                        // Validate information passed
                        Error_VM error_VM = tennisAreaTimeSlotViewModel.ValidInformation();

                        if (!error_VM.Valid)
                        {
                            apiResponse.status = -1;
                            apiResponse.message = error_VM.Message;
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        // Insert the time value into the database
                        SPResponseViewModel resp = storedProcedureRepository.SP_InsertUpdateTennisTimeSlot_Get<SPResponseViewModel>(new SP_InsertUpdateTennisAreaTimeSlot_Param_VM
                        {
                            Id = tennisAreaTimeSlotViewModel.Id,
                            UserLoginId = _BusinessOwnerLoginId,
                            SlotId = tennisAreaTimeSlotViewModel.SlotId,
                            Time = tennisAreaTimeSlotViewModel.Time,
                            SubmittedByLoginId = _BusinessOwnerLoginId,
                            Mode = tennisAreaTimeSlotViewModel.Mode
                        });

                        if (resp.ret <= 0)
                        {
                            apiResponse.status = resp.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                    }

                    // If all time values were successfully inserted, return success response
                    apiResponse.status = 1;
                    apiResponse.message ="Success";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    // Handle the case when times string is null or empty
                    apiResponse.status = -1;
                    apiResponse.message = "No time values provided";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// to get business tennis timing
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBusinessTennisTimingDetail")]
        public HttpResponseMessage GetBusinessTennisTimingDetail(long businessOwnerLoginId,long slotId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<TennisAreaTimeSlot_VM> tennisTiminglst = businessContentTennisService.GetBusinessTennisTiming_List(businessOwnerLoginId, slotId);
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    TennisTimingSlotList = tennisTiminglst,
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
        /// to get business tennis timing
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBookingTimeList")]
        public HttpResponseMessage GetBookingTimeList(long businessOwnerLoginId, long slotId, string BookDate)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<TennisAreaTimeSlot_VM> _BookingTimeList = businessContentTennisService.GetTennisTimingList(businessOwnerLoginId, slotId, BookDate);
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BookingTimeList = _BookingTimeList
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
        /// to get timing for business
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessTennisTimingDetailForBusiness")]
        public HttpResponseMessage GetBusinessTennisTimingDetailForBusiness(long slotId)
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

                List<TennisAreaTimeSlot_VM> tennistimelist = businessContentTennisService.GetTennisTimingListForBusiness(_LoginId,slotId);
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    Tennistimelist = tennistimelist

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
        /// To Add Business  Content Tennis Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateMasterProfileRoom")]

        public HttpResponseMessage AddUpdateMasterProfileRoom()
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

                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessMasterProfileRoomDetails_VM businessContentTennis_VM = new BusinessMasterProfileRoomDetails_VM();

                // Parse and assign values from HTTP request parameters
                businessContentTennis_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentTennis_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentTennis_VM.SubTitle = HttpRequest.Params["SubTitle"].Trim();
                businessContentTennis_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentTennis_VM.Price = Convert.ToInt32(HttpRequest.Params["Price"]);
                businessContentTennis_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssTennisImageFile = files["TennisImage"];
                businessContentTennis_VM.TennisImage = _BusinesssTennisImageFile; // for validation
                string _BusinessTennisImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessTennisImageFileName = ""; // will be used to delete file while updating.

                // Validate information passed
                Error_VM error_VM = businessContentTennis_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                if (files.Count > 0)
                {
                    if (_BusinesssTennisImageFile != null)
                    {

                        _BusinessTennisImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssTennisImageFile);
                    }

                }


                if (businessContentTennis_VM.Mode == 2)
                {

                    var respGetBusinessBannerDetail = businessContentTennisService.GetBusinessContentTennisDetail(_BusinessOwnerLoginId);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessTennisImageFileNameGenerated = respGetBusinessBannerDetail.TennisImage ?? "";

                    }
                    else
                    {
                        _PreviousBusinessTennisImageFileName = respGetBusinessBannerDetail.TennisImage ?? "";

                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateMasterProfileRoomDetails_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentTennis_Param_VM
                {
                    Id = businessContentTennis_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentTennis_VM.Title,
                    SubTitle = businessContentTennis_VM.SubTitle,
                    Description = businessContentTennis_VM.Description,
                    TennisImage = _BusinessTennisImageFileNameGenerated,
                    Price = businessContentTennis_VM.Price,
                    Mode = businessContentTennis_VM.Mode
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
                    #region Insert-Update Tennis Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessTennisImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessTennisImage), _PreviousBusinessTennisImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessTennisImage), _BusinessTennisImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssTennisImageFile, NewImageFileWithPath);


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
        /// To Get Business Tennis Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetMasterProfileRoomDetailsForEdit/{id}")]
        public HttpResponseMessage GetMasterProfileRoomDetailsForEdit(long Id)
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


                BusinessContentTennisDetail_VM resp = new BusinessContentTennisDetail_VM();

                resp = businessContentTennisService.GetBusinessMasterProfileRoomDetail_ById(Id);



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
        /// Get All Business Content Tennis with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllMasterProfileDataTablePagination")]
        public HttpResponseMessage GetAllMasterProfileDataTablePagination()
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

                TennisDetail_Pagination_SQL_Params_VM _Params_VM = new TennisDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentTennisService.GetMasterProfileRoomDetailsList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Delete Tennis Detail
        /// </summary>
        /// <param name="id">Tennis Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteRoomById")]
        public HttpResponseMessage DeleteRoomById(long id)
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

                // Delete Content Tennis 
                resp = businessContentTennisService.DeleteRoomById(id);


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
        /// Delete Apartment-Area By Area-Id
        /// </summary>
        /// <param name="areaId">Area Id</param>
        /// <returns>Status 1 if deleted else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteApartmentAreaTime")]
        public HttpResponseMessage DeleteApartmentAreaTime(long id)
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

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Delete Apartment-Area by Id
                SPResponseViewModel respDeleteMenu = businessContentTennisService.DeleteApartmentAreaTimeById(id, _BusinessOwnerLoginId);

                apiResponse.status = respDeleteMenu.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(respDeleteMenu.resourceFileName, respDeleteMenu.resourceKey);
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