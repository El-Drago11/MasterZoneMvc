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

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessContentAboutAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessContentAboutService businessContentAboutService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessContentAboutAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessContentAboutService = new BusinessContentAboutService(db);
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
        /// To Add Business Dance Banner Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessAbout")]

        public HttpResponseMessage AddUpdateBusinessBannerAbout()
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
                About_VM about_VM = new About_VM();
                about_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);

                // Check and set Title 

                string TitleParam = HttpRequest.Params["Title"];
                about_VM.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                about_VM.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                about_VM.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                about_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssAboutImageFile = files["AboutImage"];
                about_VM.AboutImage = _BusinesssAboutImageFile; // for validation
                string _BusinessAboutImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessAboutImageFileName = ""; // will be used to delete file while updating.



                // Validate infromation passed
                Error_VM error_VM = about_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_BusinesssAboutImageFile != null)
                    {

                        _BusinessAboutImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssAboutImageFile);
                    }

                }



                if (about_VM.Mode == 1)
                {
                    var respGetBusinessAboutDetail = businessContentAboutService.GetBusinessAboutDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessAboutDetail != null && _BusinesssAboutImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessAboutDetail.AboutImage == null)
                        {
                            _PreviousBusinessAboutImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessAboutImageFileNameGenerated = respGetBusinessAboutDetail.AboutImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessAboutImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }



                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentAbout_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentAbout
                {
                    Id = about_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = about_VM.Title,
                    SubTitle = about_VM.SubTitle,
                    Description = about_VM.Description,
                    AboutImage = _BusinessAboutImageFileNameGenerated,
                    Mode = about_VM.Mode
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
                    #region Insert-Update About Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessAboutImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAboutImage), _PreviousBusinessAboutImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAboutImage), _BusinessAboutImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssAboutImageFile, NewImageFileWithPath);


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
        /// To Get Business About Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
         [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAbout")]
        public HttpResponseMessage GetBusinessContentAbout()
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


                AboutDetail_VM resp = new AboutDetail_VM();

                resp = businessContentAboutService.GetBusinessAboutDetail(_BusinessOwnerLoginId);



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
        /// To Get Business About Detail For Dancing 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAboutDetail")]
        public HttpResponseMessage GetBusinessContentAboutDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                


                BusinessAboutDetailViewModel resp = new BusinessAboutDetailViewModel();

                resp = businessContentAboutService.GetBusinessDanceAboutDetail(businessOwnerLoginId);



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

        //////////////////////////////////////To Add About Service Detail //////////////////////////////////////

        /// <summary>
        /// To Add Business  Banner Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessAboutService")]

        public HttpResponseMessage AddUpdateBusinessAboutServiceProfile()
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
                BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                AboutServiceViewModel aboutServiceViewModel = new AboutServiceViewModel();

                // Parse and assign values from HTTP request parameters
                aboutServiceViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                aboutServiceViewModel.AboutServiceTitle = HttpRequest.Params["AboutServiceTitle"];
                aboutServiceViewModel.AboutServiceDescription = HttpRequest.Params["AboutServiceDescription"];
                aboutServiceViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssAboutServiceIconFile = files["AboutServiceIcon"];
                aboutServiceViewModel.AboutServiceIcon = _BusinesssAboutServiceIconFile; // for validation
                string _BusinessAboutServiceIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessAboutServiceIconFileName = ""; // will be used to delete file while updating.

                // Validate information passed
             

                Error_VM error_VM = aboutServiceViewModel.ValidInformation();


                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                if (files.Count > 0)
                {
                    if (_BusinesssAboutServiceIconFile != null)
                    {

                        _BusinessAboutServiceIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssAboutServiceIconFile);
                    }

                }



                if (aboutServiceViewModel.Mode == 2)
                {

                    var respGetBusinessBannerDetail = businessContentAboutService.GetBusinessAboutServiceDetail_Get(_BusinessOwnerLoginId);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessAboutServiceIconFileNameGenerated = respGetBusinessBannerDetail.AboutServiceIcon ?? "";

                    }
                    else
                    {
                        _PreviousBusinessAboutServiceIconFileName = respGetBusinessBannerDetail.AboutServiceIcon ?? "";

                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentAboutService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentAboutService
                {
                    Id = aboutServiceViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                     ProfilePageTypeId = BusinesssProfilePageType.Id,
                    AboutServiceTitle = aboutServiceViewModel.AboutServiceTitle,
                    AboutServiceDescription = aboutServiceViewModel.AboutServiceDescription,
                    AboutServiceIcon = _BusinessAboutServiceIconFileNameGenerated,
                    Mode = aboutServiceViewModel.Mode
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
                    #region Insert-Update About Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessAboutServiceIconFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAboutServiceIcon), _PreviousBusinessAboutServiceIconFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAboutServiceIcon), _BusinessAboutServiceIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssAboutServiceIconFile, NewImageFileWithPath);


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
        /// To Get Business About Service Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAboutService/{id}")]
        public HttpResponseMessage GetBusinessContentAboutService(long Id)
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


                BusinessContentAboutServiceDetail_VM resp = new BusinessContentAboutServiceDetail_VM();

                resp = businessContentAboutService.GetBusinessAboutServiceDetail(Id);



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
        /// Get All Business Content About Serive with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllAboutServiceDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentAboutServiceDataTablePagination()
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

                AboutServiceDetail_Pagination_SQL_Params_VM _Params_VM = new AboutServiceDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentAboutService.GetBusinessContentAboutServiceList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Delete About Service Detail
        /// </summary>
        /// <param name="id">About Service Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteAboutServiceDetail")]
        public HttpResponseMessage DeleteAboutServiceById(long id)
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

                // Delete Content About Service 
                resp = businessContentAboutService.DeleteAboutServiceDetail(id);


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
        /// To Get Business  Content Service About Detail For Sports  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAboutDetail_Get")]
        public HttpResponseMessage GetBusinessContentAboutDetail_Get(long businessOwnerLoginId)
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


                BusinessAboutDetailViewModel resp = new BusinessAboutDetailViewModel();

                resp = businessContentAboutService.GetBusinessDanceAboutDetail(businessOwnerLoginId);
                List<BusinessContentAboutServiceDetail_VM> businessaboutservicedetaillist = businessContentAboutService.GetBusinessAboutServiceDetail_GetList(businessOwnerLoginId);



                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data =  new
                {
                   BusinessAboutDetail = resp,
                   BusinessAboutServiceDetail = businessaboutservicedetaillist,
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
        /// Add-Update Instructor other information (Profile-page)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateInstructorOtherInformation")]

        public HttpResponseMessage AddUpdateInstructorOtherInformation()
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
                InstructorOtherInformationRequest_VM instructorOtherInformationRequest_VM = new InstructorOtherInformationRequest_VM();
                instructorOtherInformationRequest_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                instructorOtherInformationRequest_VM.Title = HttpRequest.Params["Title"];

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                instructorOtherInformationRequest_VM.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                instructorOtherInformationRequest_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                var resp = storedProcedureRepository.SP_InsertUpdateInstructorOtherInformation_Get<SPResponseViewModel>(new SP_InsertUpdateInstructorOtherInformation
                {
                    Id = instructorOtherInformationRequest_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = instructorOtherInformationRequest_VM.Title,
                    Description = instructorOtherInformationRequest_VM.Description,
                    Mode = instructorOtherInformationRequest_VM.Mode
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
        /// To Get Instructor Other Information Detail For instructor  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetInstructorOtherInformation")]
        public HttpResponseMessage GetInstructorOtherInformation()
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


                InstructorOtherInformationDetail_VM resp = new InstructorOtherInformationDetail_VM();

                resp = businessContentAboutService.GetInstructorOtherInformationDetail(_BusinessOwnerLoginId);



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
        /// To Get Instructor Other Information Detail By businessOwnerLoginId (In Visitor Panel)
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]

        [Route("api/Business/GetInstructorOtherInformationDetail")]
        public HttpResponseMessage GetInstructorOtherInformation(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                var validateResponse = ValidateLoggedInUser();


                InstructorOtherInformationDetail_VM resp = new InstructorOtherInformationDetail_VM();

                resp = businessContentAboutService.GetInstructorOtherInformationDetail(businessOwnerLoginId);



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
    }
}