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
using Antlr.Runtime.Misc;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessContentStudioEquipmentAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessContentStudioEquipmentService businessContentStudioEquipmentService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessContentStudioEquipmentAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessContentStudioEquipmentService = new BusinessContentStudioEquipmentService(db);
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
        /// To Add Business  Studio Equipment PPCMeta Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentStudioEquipment_PPCMeta")]

        public HttpResponseMessage AddUpdateBusinessStudioEquipment_PPCMeta()
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
                BusinessContentStudioEqipment_PPCMeta_VM businessContentStudioEqipment_PPCMeta_VM = new BusinessContentStudioEqipment_PPCMeta_VM();
                businessContentStudioEqipment_PPCMeta_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);              
                string titleParam = HttpRequest["Title"];
                businessContentStudioEqipment_PPCMeta_VM.Title = !string.IsNullOrEmpty(titleParam) ? titleParam.Trim() : string.Empty;

                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                businessContentStudioEqipment_PPCMeta_VM.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;


                businessContentStudioEqipment_PPCMeta_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

               

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentStudioEquipment_PPCMeta_Detail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentStudioEquipmentPPCMeta_Param_VM
                {
                    Id = businessContentStudioEqipment_PPCMeta_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessContentStudioEqipment_PPCMeta_VM.Title,
                    SubTitle = businessContentStudioEqipment_PPCMeta_VM.SubTitle,
                    Mode = businessContentStudioEqipment_PPCMeta_VM.Mode
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
        /// To Get Business StudioEquipment Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentStudioEquipment_PPCMetaDetail")]
        public HttpResponseMessage GetBusinessContentStudioEquipmentDetail()
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


                BusinessContentStudioEqipment_PPCMeta_VM resp = new BusinessContentStudioEqipment_PPCMeta_VM();

                resp = businessContentStudioEquipmentService.GetBusinessContentStudioEquipment_PPCMetaDetail(_BusinessOwnerLoginId);



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
        [Route("api/Business/AddUpdateBusinessContentStudioEquipment")]

        public HttpResponseMessage AddUpdateBusinessStudioEquipment()
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
                BusinressContentStudioEquipment_VM businressContentStudioEquipment_VM = new BusinressContentStudioEquipment_VM();
                businressContentStudioEquipment_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businressContentStudioEquipment_VM.EquipmentType = HttpRequest.Params["EquipmentType"].Trim();
                businressContentStudioEquipment_VM.EquipmentValue = HttpRequest.Params["EquipmentValue"].Trim();
                businressContentStudioEquipment_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = businressContentStudioEquipment_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentStudioEquipmentDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentStudioEquipment_Param_VM
                {
                    Id = businressContentStudioEquipment_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    EquipmentType = businressContentStudioEquipment_VM.EquipmentType,
                    EquipmentValue = businressContentStudioEquipment_VM.EquipmentValue,
                    Mode = businressContentStudioEquipment_VM.Mode
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
        /// To Get Business Studio Equipment Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentStudioEquipmentDetail")]
        public HttpResponseMessage GetBusinessContentStudioEquipment_Detail(long Id)
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


                BusinressContentStudioEquipmentDetail_VM resp = new BusinressContentStudioEquipmentDetail_VM();

                resp = businessContentStudioEquipmentService.GetBusinessContentStudioEquipmentDetail(Id);



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
        /// Get All Business Content Studio Equipment with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentStudioEquipmentDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentStudioEquipmentDataTablePagination()
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

                BusinessContentStudioEqipmentDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentStudioEqipmentDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentStudioEquipmentService.GetBusinessContentStudioEquipmentList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Delete StudioEquipment Detail
        /// </summary>
        /// <param name="id">StudioEquipment Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteStudioEquipmentDetail")]
        public HttpResponseMessage DeleteStudioEquipmentById(long id)
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

                // Delete Content Studio Equipment Detail 
                resp = businessContentStudioEquipmentService.DeleteStudioEquipmentDetail(id);


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
        /// To Get Business StudioEquipment Detail (Without Authorization)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentStudioEquipmentDetail_VisitorPanel")]
        public HttpResponseMessage GetBusinessContentStudioEquipmentDetails(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {



                BusinessContentStudioEqipment_PPCMeta_VM resp = new BusinessContentStudioEqipment_PPCMeta_VM();

                resp = businessContentStudioEquipmentService.GetBusinessContentStudioEquipment_PPCMetaDetail(businessOwnerLoginId);

                List<BusinressContentStudioEquipmentDetail_VM>  businessContentStudioEquipmentlst = businessContentStudioEquipmentService.GetBusinessContentStudioEquipment_lstDetail(businessOwnerLoginId);

                foreach (var data in businessContentStudioEquipmentlst)
                {


                    string _eventOptions = (data == null) ? "" : data.EquipmentValue;
                    string[] stringArray = _eventOptions.Split(',');

                    List<BusinressContentStudioEquipmentValueDetail_VM> businessEventCompanyDetailEventOptions = new List<BusinressContentStudioEquipmentValueDetail_VM>();
                    foreach (string item in stringArray)
                    {
                        // Create an instance of BusinessContentEventCompanyDetailEventOptions_VM and set its properties
                        BusinressContentStudioEquipmentValueDetail_VM option = new BusinressContentStudioEquipmentValueDetail_VM
                        {
                            EquipmentValueName = item // Assuming 'Name' is the property in BusinessContentEventCompanyDetailEventOptions_VM
                        };

                        businessEventCompanyDetailEventOptions.Add(option);
                    }
                    data.EquipmentValue = string.Join(",", businessEventCompanyDetailEventOptions.Select(item => item.EquipmentValueName));

                }

                // Now, businessContentStudioEquipmentlst contains the objects created from the split values.

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentStudioEquipment = resp,
                    BusinessContentStudioEquipmentlst = businessContentStudioEquipmentlst,
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








        //////////////////////////////////////////////////// To Add Audio Detail /////////////////////////////////////////////////
        /// <summary>
        /// To Add Business Audio PPCMeta Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentAudio_PPCMeta")]

        public HttpResponseMessage AddUpdateBusinessAudio_PPCMeta()
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
                BusinessContentProtfolio_VM businessContentProtfolio_VM = new BusinessContentProtfolio_VM();
                businessContentProtfolio_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string titleParam = HttpRequest["Title"];
                businessContentProtfolio_VM.Title = !string.IsNullOrEmpty(titleParam) ? titleParam.Trim() : string.Empty;

                // Check and set SubTitle
                string descriptionParam = HttpRequest.Params["Description"];
                businessContentProtfolio_VM.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                businessContentProtfolio_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Validate infromation passed
                Error_VM error_VM = businessContentProtfolio_VM.ValidInformation();



                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                // Get Attatched PortfolioImage Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssPortfolioImageFile = files["PortfolioImage"];
                businessContentProtfolio_VM.PortfolioImage = _BusinesssPortfolioImageFile; // for validation
                string _BusinessPortfolioImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessPortfolioImageFileName = ""; // will be used to delete file while updating.



                HttpPostedFile _BusinesssAudioImageFile = files["AudioImage"];
                businessContentProtfolio_VM.AudioImage = _BusinesssAudioImageFile; // for validation
                string _BusinessAudioImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessAudioImageFileName = ""; // will be used to delete file while updating.

                if (files.Count > 0)
                {
                    if (_BusinesssPortfolioImageFile != null)
                    {

                        _BusinessPortfolioImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssPortfolioImageFile);
                    }
                    if(_BusinesssAudioImageFile != null)
                    {
                        _BusinessAudioImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssAudioImageFile);
                    }

                }


                if (businessContentProtfolio_VM.Mode == 1)
                {
                    var respGetBusinessProtfolioDetail = businessContentStudioEquipmentService.GetBusinessContentAudio_PPCMetaDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessProtfolioDetail != null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessProtfolioDetail.PortfolioImage == null)
                        {
                            _PreviousBusinessPortfolioImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessPortfolioImageFileNameGenerated = respGetBusinessProtfolioDetail.PortfolioImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessPortfolioImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                    if (respGetBusinessProtfolioDetail != null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessProtfolioDetail.AudioImage == null)
                        {
                            _PreviousBusinessAudioImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessAudioImageFileNameGenerated = respGetBusinessProtfolioDetail.AudioImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessAudioImageFileName = ""; // Set it to an empty string or handle it as needed
                    }

                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentAudio_PPCMeta_Detail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentPortfolio_Params_VM
                {
                    Id = businessContentProtfolio_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessContentProtfolio_VM.Title,
                    Description = businessContentProtfolio_VM.Description,
                    PortfolioImage = _BusinessPortfolioImageFileNameGenerated,
                    AudioImage = _BusinessAudioImageFileNameGenerated,
                    Mode = businessContentProtfolio_VM.Mode
                }) ;


                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
               
                if (resp.ret == 1)
                {
                    // Update Class Image.
                    #region Insert-Update Portofolio & Image on Server
                    if (files.Count > 0)
                    {
                        if (_BusinesssPortfolioImageFile != null)
                        {
                            if (!String.IsNullOrEmpty(_PreviousBusinessPortfolioImageFileName))
                            {
                                // remove previous  Portofolio image file
                                string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessPortfolioImage), _PreviousBusinessPortfolioImageFileName);
                                fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                            }

                            // save new image file
                            string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessPortfolioImage), _BusinessPortfolioImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_BusinesssPortfolioImageFile, NewImageFileWithPath);

                        }

                        if (_BusinesssAudioImageFile != null)
                        {
                            if (!String.IsNullOrEmpty(_PreviousBusinessAudioImageFileName))
                            {
                                // remove previous  Portofolio image file
                                string RemoveAudioImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAudioImage), _PreviousBusinessAudioImageFileName);
                                fileHelper.DeleteAttachedFileFromServer(RemoveAudioImageFileWithPath);
                            }

                            // save new image file
                            string NewAudioImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAudioImage), _BusinessAudioImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_BusinesssAudioImageFile, NewAudioImageFileWithPath);

                        }

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
        /// To Get Business Audio Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAudio_PPCMetaDetail")]
        public HttpResponseMessage GetBusinessContentAudioDetail()
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


                BusinessContentProtfolioDetail_VM resp = new BusinessContentProtfolioDetail_VM();

                resp = businessContentStudioEquipmentService.GetBusinessContentAudio_PPCMetaDetail(_BusinessOwnerLoginId);



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


        ////////////////////////////////////////////// Full Crud Detail//////////////////////////////////////////////////

        /// <summary>
        /// To Add Business  Audio  Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentAudio")]

        public HttpResponseMessage AddUpdateBusinessAudioProfile()
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
                BusinessContentAudio_VM businessContentAudio_VM = new BusinessContentAudio_VM();

                // Parse and assign values from HTTP request parameters
                businessContentAudio_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentAudio_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentAudio_VM.ArtistName = HttpRequest.Params["ArtistName"].Trim() ;
                businessContentAudio_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);





                // Get Attatched PortfolioImage Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssAudioFile = files["AudioFile"];
                businessContentAudio_VM.AudioFile = _BusinesssAudioFile; // for validation
                string _BusinessAudioFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessAudioFileName = ""; // will be used to delete file while updating.


                if (files.Count > 0)
                {
                    
                     if(_BusinesssAudioFile != null)
                    {
                        _BusinessAudioFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssAudioFile);
                    }

                }


                if (businessContentAudio_VM.Mode == 2)
                {

                    var respGetAudioImageData = businessContentStudioEquipmentService.BusinessContentAudioDetail_GetById(businessContentAudio_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessAudioFileNameGenerated = respGetAudioImageData.AudioFile ?? "";
                        
                    }
                    else
                    {
                        _PreviousBusinessAudioFileName = respGetAudioImageData.AudioFile ?? "" ;
                       
                    }
                }

   


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentAudioDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentAudio_Params_VM
                {
                    Id = businessContentAudio_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentAudio_VM.Title,
                    ArtistName = businessContentAudio_VM.ArtistName,
                    AudioFile = _BusinessAudioFileNameGenerated,
                    Mode = businessContentAudio_VM.Mode
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
                    #region Insert-Update Portofolio/AudioFiles & Image on Server
                    if (files.Count > 0)
                    {
                        
                        
                        if (_BusinesssAudioFile != null)
                        {
                               if (!String.IsNullOrEmpty(_PreviousBusinessAudioFileName))
                                   {
                                        /////// remove previous Audio  file
                                        string RemoveAudioFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAudioFile), _PreviousBusinessAudioFileName);
                                        fileHelper.DeleteAttachedFileFromServer(RemoveAudioFileWithPath);
                                   }
                                    // save new Audio  file
                                    string NewAudioFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessAudioFile), _BusinessAudioFileNameGenerated);
                                    fileHelper.SaveUploadedFile(_BusinesssAudioFile, NewAudioFileWithPath);

                        }

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
        /// To Get Business Audio   Detail By Id  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAudioDetail")]
        public HttpResponseMessage GetBusinessContentAudio_Detail(long Id)
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


                BusinessContentAudioDetail_VM resp = new BusinessContentAudioDetail_VM();

               

                resp = businessContentStudioEquipmentService.BusinessContentAudioDetail_GetById(Id);

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
        /// Delete Audio  Detail By Id 
        /// </summary>
        /// <param name="id">Audio Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteAudioDetail")]
        public HttpResponseMessage DeleteAudioById(long id)
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

                // Delete Content Audio  Detail 
                resp = businessContentStudioEquipmentService.DeleteAudioDetail(id);


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
        /// Get All Business Content Studio Equipment with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentAudioDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentAudioDataTablePagination()
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

                BusinessContentAudioDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentAudioDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentStudioEquipmentService.GetBusinessContentAudioList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Business Audio Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentAudioDetailForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentAudioPortfolioDetails(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {




                BusinessContentProtfolioDetail_VM resp = new BusinessContentProtfolioDetail_VM();

                resp = businessContentStudioEquipmentService.GetBusinessContentAudio_PPCMetaDetail(businessOwnerLoginId);


                List<BusinessContentAudioDetail_VM> businessContentProtfolioDetaillst = businessContentStudioEquipmentService.GetBusinessContentAudiolst_Get(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data =  new { 

                BusinessContentAudioPPCMetaDetail = resp,
                BusinessContentAudiolstDetail = businessContentProtfolioDetaillst,
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
    }
}