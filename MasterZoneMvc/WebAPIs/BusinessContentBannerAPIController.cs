using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessContentBannerAPIController : ApiController
    {
        // GET api/<controller>
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessContentBannerService businessContentBannerService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessContentBannerAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessContentBannerService = new BusinessContentBannerService(db);
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
        /// To Add Business  Banner Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessBanner")]

        public HttpResponseMessage AddUpdateBusinessBannerProfile()
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
                Banner_VM business_VM = new Banner_VM();

                // Parse and assign values from HTTP request parameters
                business_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                business_VM.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                business_VM.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;

                // Check and set ButtonText
                string buttonTextParam = HttpRequest.Params["ButtonText"];
                business_VM.ButtonText = !string.IsNullOrEmpty(buttonTextParam) ? buttonTextParam.Trim() : string.Empty;

                // Check and set ButtonLink
                string buttonLinkParam = HttpRequest.Params["ButtonLink"];
                business_VM.ButtonLink = !string.IsNullOrEmpty(buttonLinkParam) ? buttonLinkParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                business_VM.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                business_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = business_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssBannerImageFile = files["BannerImage"];
                business_VM.BannerImage = _BusinesssBannerImageFile; // for validation
                string _BusinessBannerImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessBannerImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssBannerImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessBannerImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssBannerImageFile);
                    }
                }

                //// If no new image is uploaded, keep the previous image filename
                //if (string.IsNullOrEmpty(_BusinessBannerImageFileNameGenerated))
                //{
                //    _BusinessBannerImageFileNameGenerated = _PreviousBusinessBannerImageFileName;
                //}

                if (business_VM.Mode == 1)
                {
                    var respGetBusinessBannerDetail = businessContentBannerService.GetBusinessBannerDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessBannerDetail != null && _BusinesssBannerImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessBannerDetail.BannerImage == null)
                        {
                            _PreviousBusinessBannerImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessBannerImageFileNameGenerated = respGetBusinessBannerDetail.BannerImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessBannerImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentBanner_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentBanner_Params_VM
                {
                    Id = business_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = business_VM.Title,
                    SubTitle = business_VM.SubTitle,
                    Description = business_VM.Description,
                    ButtonText = business_VM.ButtonText,
                    ButtonLink = business_VM.ButtonLink,
                    BannerImage = _BusinessBannerImageFileNameGenerated,
                    Mode = business_VM.Mode
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
                    #region Insert-Update Banner Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessBannerImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessBannerImage), _PreviousBusinessBannerImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessBannerImage), _BusinessBannerImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssBannerImageFile, NewImageFileWithPath);
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
        /// To Get Banner Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/BannerDetail/Get")]
        public HttpResponseMessage GetBannerDetailById()
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


             
                    BannerDetail_VM businessbannerDetail = businessContentBannerService.GetBusinessBannerDetail(_BusinessOwnerLoginId);
                
              
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessbannerDetail;

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
        [Route("api/Business/GetBusinessbanner")]
        public HttpResponseMessage GetBusinessContentbanner(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                BannerDetail_VM resp = new BannerDetail_VM();

                resp  = businessContentBannerService.GetBusinessBannerDetail(businessOwnerLoginId);



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
        /// Get All Business Content Banner with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllByPagination")]
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

                BannerDetail_Pagination_SQL_Params_VM _Params_VM = new BannerDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentBannerService.GetBusinessContentBannerList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Delete Banner Detail
        /// </summary>
        /// <param name="id">Banner Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteBannerDetail/{id}")]
        public HttpResponseMessage DeleteBannerById(long id)
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

                // Delete Content Banner Detail 
                resp = businessContentBannerService.DeleteBannerDetail(id);


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
        /// To Get Banner Detail By Id(Sports)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBannerDetail/ByGet/{id}")]
        public HttpResponseMessage GetBannerDetailById(long Id)
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



                BannerDetail_VM businessbannerDetail = businessContentBannerService.GetBusinessBannerDetail_ById(Id);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessbannerDetail;

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
        /// To Get Banner Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBannerDetailList")]
        public HttpResponseMessage GetBannerDetailByBusinessOwnerLoginId(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
          
                 List<BannerDetail_VM> businessbannerDetail = businessContentBannerService.GetBusinessBannerDetailList( businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessbannerDetail;

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
        /// To Add Business  Banner Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessSportsBanner")]

        public HttpResponseMessage AddUpdateBusinessSportsBannerProfile()
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
                Banner_VM business_VM = new Banner_VM();

                // Parse and assign values from HTTP request parameters
                business_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                business_VM.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                business_VM.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;

                // Check and set ButtonText
                string buttonTextParam = HttpRequest.Params["ButtonText"];
                business_VM.ButtonText = !string.IsNullOrEmpty(buttonTextParam) ? buttonTextParam.Trim() : string.Empty;

                // Check and set ButtonLink
                string buttonLinkParam = HttpRequest.Params["ButtonLink"];
                business_VM.ButtonLink = !string.IsNullOrEmpty(buttonLinkParam) ? buttonLinkParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                business_VM.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                business_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = business_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssBannerImageFile = files["BannerImage"];
                business_VM.BannerImage = _BusinesssBannerImageFile; // for validation
                string _BusinessBannerImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessBannerImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssBannerImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessBannerImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssBannerImageFile);
                    }
                }

                //// If no new image is uploaded, keep the previous image filename
                //if (string.IsNullOrEmpty(_BusinessBannerImageFileNameGenerated))
                //{
                //    _BusinessBannerImageFileNameGenerated = _PreviousBusinessBannerImageFileName;
                //}

                if (business_VM.Mode == 2)
                {
                    var respGetBusinessBannerDetail = businessContentBannerService.GetBusinessBannerDetail_ById(business_VM.Id);

                    if (respGetBusinessBannerDetail != null && _BusinesssBannerImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessBannerDetail.BannerImage == null)
                        {
                            _PreviousBusinessBannerImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessBannerImageFileNameGenerated = respGetBusinessBannerDetail.BannerImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessBannerImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentSportsBanner_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentBanner_Params_VM
                {
                    Id = business_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = business_VM.Title,
                    SubTitle = business_VM.SubTitle,
                    Description = business_VM.Description,
                    ButtonText = business_VM.ButtonText,
                    ButtonLink = business_VM.ButtonLink,
                    BannerImage = _BusinessBannerImageFileNameGenerated,
                    Mode = business_VM.Mode
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
                    #region Insert-Update Banner Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessBannerImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessBannerImage), _PreviousBusinessBannerImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessBannerImage), _BusinessBannerImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssBannerImageFile, NewImageFileWithPath);
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
        /// To Add/Update Education Banner Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessEducationBannerProfile")]

        public HttpResponseMessage AddUpdateBusinessEducationBannerProfile()
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

                BusinessContentEducationBannerViewModel businessContentEducationBannerDetailViewModel = new BusinessContentEducationBannerViewModel();
                businessContentEducationBannerDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentEducationBannerDetailViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessContentEducationBannerDetailViewModel.Description = HttpRequest.Params["Description"].Trim();
                businessContentEducationBannerDetailViewModel.SubTitle = HttpRequest.Params["SubTitle"].Trim();
                businessContentEducationBannerDetailViewModel.BannerType = HttpRequest.Params["BannerType"].Trim();
                businessContentEducationBannerDetailViewModel.ButtonLink = HttpRequest.Params["ButtonLink"].Trim();
                businessContentEducationBannerDetailViewModel.ButtonText = HttpRequest.Params["ButtonText"].Trim();
                businessContentEducationBannerDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                // Validate information passed
                Error_VM error_VM = businessContentEducationBannerDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssEducationImageFile = files["BannerImage"];
                businessContentEducationBannerDetailViewModel.BannerImage = _BusinesssEducationImageFile; // for validation
                string _BusinessEducationImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessEducationImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssEducationImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessEducationImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssEducationImageFile);
                    }
                }

                if (businessContentEducationBannerDetailViewModel.Mode == 1)
                {
                    var respGetBusinessInstructorImageDetail = businessContentBannerService.GetEducationBannerProfileDetail(businessContentEducationBannerDetailViewModel.Id);

                    if (respGetBusinessInstructorImageDetail != null && _BusinesssEducationImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessInstructorImageDetail.BannerImage == null)
                        {
                            _PreviousBusinessEducationImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessEducationImageFileNameGenerated = respGetBusinessInstructorImageDetail.BannerImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessEducationImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentEducationBannerProfileDetail_Get<SPResponseViewModel>(new SP_InsertUpdateEducationBannerDetail_Param_VM
                {
                    Id = businessContentEducationBannerDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentEducationBannerDetailViewModel.Title,
                    BannerType = businessContentEducationBannerDetailViewModel.BannerType,
                    Description = businessContentEducationBannerDetailViewModel.Description,
                    BannerImage = _BusinessEducationImageFileNameGenerated,
                    SubTitle = businessContentEducationBannerDetailViewModel.SubTitle,
                    ButtonLink = businessContentEducationBannerDetailViewModel.ButtonLink,
                    ButtonText = businessContentEducationBannerDetailViewModel.ButtonText,
                    SubmittedByLoginId = _BusinessOwnerLoginId,
                    Mode = businessContentEducationBannerDetailViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessEducationImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EducationImage), _PreviousBusinessEducationImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EducationImage), _BusinessEducationImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssEducationImageFile, NewImageFileWithPath);
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
        /// To get Education Banner Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEducationBannerDetailById")]
        public HttpResponseMessage GetBusinessContentEducationBannerDetailById(long id)
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



                BusinessEducationBannerDetail_VM resp = new BusinessEducationBannerDetail_VM();
                resp = businessContentBannerService.GetEducationBannerProfileDetail(id);

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
        /// To Get Banner Detail by Pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetEducationBannerProfileDetailListByPagination")]
        public HttpResponseMessage GetEducationBannerProfileDetailListByPagination()
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


                List<BusinessEducationBannerDetail_VM> resp = businessContentBannerService.GetEducationBannerProfileDetail_Get(_BusinessOwnerLoginId);

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
        /// To Get Education Banner Detail For Visitor Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetEducationBannerProfileDetailListFor_VisitorPanel")]
        public HttpResponseMessage GetEducationBannerProfileDetailListFor_VisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessEducationBannerDetail_VM> resp = businessContentBannerService.GetEducationBannerProfileDetail_Get(businessOwnerLoginId);

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
        /// To Delete Education Banner Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteEducationBannerDetailById")]
        public HttpResponseMessage DeleteEducationBannerDetailById(long id)
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
                var resp = businessContentBannerService.DeleteBusinessContentEducationBannerDetail(id);


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

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateClassIntermediates")]
        public HttpResponseMessage AddUpdateClassIntermediates()
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
                RequestClassIntermediatesImage_VM requestBusinessImage_VM = new RequestClassIntermediatesImage_VM();
                requestBusinessImage_VM.Id = Convert.ToInt64(HttpRequest.Params["id"]);
                requestBusinessImage_VM.ImageTitle = HttpRequest.Params["Title"].Trim();
                requestBusinessImage_VM.Description = HttpRequest.Params["Description"].Trim();
                requestBusinessImage_VM.Mode = 1;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessImageFile = files["Image"];
                requestBusinessImage_VM.Image = _ManageBusinessImageFile; // for validation
                string _ManageBusinessImageFileNameGenerated = ""; //will contains generated file name


                // Validate infromation passed
                Error_VM error_VM = requestBusinessImage_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                if (files.Count > 0)
                {
                    if (_ManageBusinessImageFile != null)
                    {
                        _ManageBusinessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessImageFile);
                    }
                }
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",requestBusinessImage_VM.Id),
                            new SqlParameter("businessOwnerLoginId",_BusinessOwnerLoginId),
                            new SqlParameter("imageTitle",requestBusinessImage_VM.ImageTitle),
                            new SqlParameter("description",requestBusinessImage_VM.Description),
                            new SqlParameter("image",_ManageBusinessImageFileNameGenerated),
                            new SqlParameter("submittedByLoginId",_LoginId),
                            new SqlParameter("mode",requestBusinessImage_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateClassIntermediates @id,@businessOwnerLoginId,@imageTitle,@description,@image ,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Group Image.
                    #region Insert-Update Manage Business Image on Server
                    if (files.Count > 0)
                    {
                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassIntermediate), _ManageBusinessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessImageFile, FileWithPath);
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
        /// to get class intermediate details for update 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetClassIntermediatesDetailById")]
        public HttpResponseMessage GetClassIntermediatesDetailById()
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

                ClassIntermediatesDetail_VM classintermediateDetail = businessContentBannerService.GetClassInterMediateDetail(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = classintermediateDetail;

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