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
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class ClassicDanceAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessOwnerService businessOwnerService;
        private ClasssicDanceService classsicDanceService;
        private StoredProcedureRepository storedProcedureRepository;

        public ClassicDanceAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessOwnerService = new BusinessOwnerService(db);
            classsicDanceService = new ClasssicDanceService(db);
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
        /// To Add/Update Classic Dance Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/AddUpdateBusinessClassicDanceDetail")]

        public HttpResponseMessage AddUpdateBusinessClassicDanceDetail()
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
                ClassicDanceTechniqueViewModel classicDanceTechniqueViewModel = new ClassicDanceTechniqueViewModel();

                // Parse and assign values from HTTP request parameters
                classicDanceTechniqueViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                classicDanceTechniqueViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                classicDanceTechniqueViewModel.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;


                classicDanceTechniqueViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = classicDanceTechniqueViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssTechniqueImageFile = files["TechniqueImage"];
                classicDanceTechniqueViewModel.TechniqueImage = _BusinesssTechniqueImageFile; // for validation
                string _BusinessTechniqueImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessTechniqueImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssTechniqueImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessTechniqueImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssTechniqueImageFile);
                    }
                }

                

                if (classicDanceTechniqueViewModel.Mode == 1)
                {
                    var respGetBusinessTechniqueDetail = classsicDanceService.GetClassicDanceTechniquebusinessLoginId(_BusinessOwnerLoginId);

                    if (respGetBusinessTechniqueDetail != null && _BusinesssTechniqueImageFile == null) // 
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessTechniqueDetail.TechniqueImage == null)
                        {
                            _PreviousBusinessTechniqueImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessTechniqueImageFileNameGenerated = respGetBusinessTechniqueDetail.TechniqueImage;
                        }
                    }
                    else
                    {
                        
                        _PreviousBusinessTechniqueImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateClassicDanceTechniqueDetail_Get<SPResponseViewModel>(new SP_InsertUpdateClassicDanceTechnique_Param_VM
                {
                    Id = classicDanceTechniqueViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = classicDanceTechniqueViewModel.Title,
                    SubTitle = classicDanceTechniqueViewModel.SubTitle,
                    TechniqueImage = _BusinessTechniqueImageFileNameGenerated,
                    Mode = classicDanceTechniqueViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessTechniqueImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessTechniqueImage), _PreviousBusinessTechniqueImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessTechniqueImage), _BusinessTechniqueImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssTechniqueImageFile, NewImageFileWithPath);
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
        /// To Get Classic Detail by BusinessOwnerLoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/GetClassicDanceDetailForVisitorPanel")]

        public HttpResponseMessage GetClassicDanceDetailForVisitorPanel()
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



                ClassicDanceTechnique_VM classicDanceTechnique_VM = new ClassicDanceTechnique_VM();
                classicDanceTechnique_VM = classsicDanceService.GetClassicDanceTechniquebusinessLoginId(_BusinessOwnerLoginId);

              
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = classicDanceTechnique_VM;



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
        /// To Insert/Update Business Content Classic Dance PPCMeta
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]

        [Route("api/ClassicDance/AddUpdateBusinessContentClassicDancePPCMeta")]
        public HttpResponseMessage AddUpdateBusinessContentClassicDancePPCMeta()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                var HttpRequest = HttpContext.Current.Request;
                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
                long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);



                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                long respId = 0; // Nullable long

                BusinessContentClassicDancePPCMeta_VM businessContentClassicDancePPCMeta_VM = new BusinessContentClassicDancePPCMeta_VM();
                businessContentClassicDancePPCMeta_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentClassicDancePPCMeta_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentClassicDancePPCMeta_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentClassicDancePPCMeta_VM.TechniqueItemList = HttpRequest.Params["TechniqueItemList"].Trim();
                businessContentClassicDancePPCMeta_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                Error_VM error_VM = businessContentClassicDancePPCMeta_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdateBusinesClassicDanceTechniqueDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentClassicDance_PPCMeta_Param_VM
                {
                    Id = businessContentClassicDancePPCMeta_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentClassicDancePPCMeta_VM.Title,
                    Description = businessContentClassicDancePPCMeta_VM.Description,
                    TechniqueItemList = businessContentClassicDancePPCMeta_VM.TechniqueItemList,                
                    Mode = businessContentClassicDancePPCMeta_VM.Mode
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
        /// To Get Busines Content Classic Dance Detail PPCMeta By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/GetBusinessContentClassicDanceDetail_PPCMeta")]

        public HttpResponseMessage GetBusinessContentClassicDanceDetail_PPCMeta(long id)
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



                BusinessContentClassicDanceDetail_ViewModel businessContentClassicDanceDetail_ViewModel = new BusinessContentClassicDanceDetail_ViewModel();
                businessContentClassicDanceDetail_ViewModel = classsicDanceService.GetClassicDanceTechniqueById(id);

               
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessContentClassicDanceDetail_ViewModel;



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
        /// To Delete Business Classic Dance Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/DeleteClassicDanceDetailById")]
        public HttpResponseMessage DeleteBranchDetailById(long id)
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
                var resp = classsicDanceService.DeleteBusinessContentClassicTechniqueDetail(id);


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
        /// To View Business Classic Dance Detail By Pagination 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/GetAllBusinessClassicDanceDetailByPagination")]
        public HttpResponseMessage GetAllBusinessClassicDanceDetailByPagination()
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

                ClassicDanceDetail_Pagination_SQL_Params_VM _Params_VM = new ClassicDanceDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = classsicDanceService.GetBusinessContentClassicDanceList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Classic Dance Detail PPCMeta By BusinessOwnerLoginId For VisitorPanel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ClassicDance/GetBusinessContentClassicDanceDetail_PPCMetaForVisitorPanel")]
        public HttpResponseMessage GetClassicDanceDetailPPCMeta(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
         


                ClassicDanceTechnique_VM classicDanceTechnique_VM = new ClassicDanceTechnique_VM();
                classicDanceTechnique_VM = classsicDanceService.GetClassicDanceTechniquebusinessLoginId(businessOwnerLoginId);


                List<BusinessContentClassicDanceDetail_ViewModel> businessContentClassicDanceDetail = classsicDanceService.GetClassicDanceTechniqueDetail_lst(businessOwnerLoginId);



                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new {

                    ClassicDanceDetail = classicDanceTechnique_VM,
                    ClassicDanceDetailList = businessContentClassicDanceDetail,
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
        /// To Add Classic Dance Video Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/AddUpdateBusinessClassicDanceVideoDetail")]

        public HttpResponseMessage AddUpdateBusinessClassicDanceVideoDetail()
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
                BusinessContentClassicDanceVideoSection_ViewModel businessContentClassicDanceVideoSection_ViewModel = new BusinessContentClassicDanceVideoSection_ViewModel();

                // Parse and assign values from HTTP request parameters
                businessContentClassicDanceVideoSection_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                businessContentClassicDanceVideoSection_ViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                businessContentClassicDanceVideoSection_ViewModel.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;

                string noteParam = HttpRequest.Params["Note"];
                businessContentClassicDanceVideoSection_ViewModel.Note = !string.IsNullOrEmpty(noteParam) ? noteParam.Trim() : string.Empty;

                string videoLinkParam = HttpRequest.Params["VideoLink"];
                businessContentClassicDanceVideoSection_ViewModel.VideoLink = !string.IsNullOrEmpty(videoLinkParam) ? videoLinkParam.Trim() : string.Empty;

                string buttonTextParam = HttpRequest.Params["ButtonText"];
                businessContentClassicDanceVideoSection_ViewModel.ButtonText = !string.IsNullOrEmpty(buttonTextParam) ? buttonTextParam.Trim() : string.Empty;

                string buttonLinkParam = HttpRequest.Params["ButtonLink"];
                businessContentClassicDanceVideoSection_ViewModel.ButtonLink = !string.IsNullOrEmpty(buttonLinkParam) ? buttonLinkParam.Trim() : string.Empty;

                string buttonText1Param = HttpRequest.Params["ButtonText1"];
                businessContentClassicDanceVideoSection_ViewModel.ButtonText1 = !string.IsNullOrEmpty(buttonText1Param) ? buttonText1Param.Trim() : string.Empty;

                string buttonLink1Param = HttpRequest.Params["ButtonLink1"];
                businessContentClassicDanceVideoSection_ViewModel.ButtonLink1 = !string.IsNullOrEmpty(buttonLink1Param) ? buttonLink1Param.Trim() : string.Empty;

               
                businessContentClassicDanceVideoSection_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = businessContentClassicDanceVideoSection_ViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssVideoImageFile = files["VideoImage"];
                businessContentClassicDanceVideoSection_ViewModel.VideoImage = _BusinesssVideoImageFile; // for validation
                string _BusinessVideoImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessVideoImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssVideoImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessVideoImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssVideoImageFile);
                    }
                }



                if (businessContentClassicDanceVideoSection_ViewModel.Mode == 1)
                {
                    var respGetBusinessTechniqueDetail = classsicDanceService.GetClassicDanceVideoDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessTechniqueDetail != null && _BusinesssVideoImageFile == null) // 
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessTechniqueDetail.VideoImage == null)
                        {
                            _PreviousBusinessVideoImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessVideoImageFileNameGenerated = respGetBusinessTechniqueDetail.VideoImage;
                        }
                    }
                    else
                    {

                        _PreviousBusinessVideoImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateClassicDanceVideoDetail_Get<SPResponseViewModel>(new SP_InsertUpdateClassicDanceVideo_Param_VM
                {
                    Id = businessContentClassicDanceVideoSection_ViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentClassicDanceVideoSection_ViewModel.Title,
                    SubTitle = businessContentClassicDanceVideoSection_ViewModel.SubTitle,
                    Note = businessContentClassicDanceVideoSection_ViewModel.Note,
                    VideoLink = businessContentClassicDanceVideoSection_ViewModel.VideoLink,
                    VideoImage = _BusinessVideoImageFileNameGenerated,
                    ButtonText1 = businessContentClassicDanceVideoSection_ViewModel.ButtonText1,
                    ButtonLink1 = businessContentClassicDanceVideoSection_ViewModel.ButtonLink1,
                    ButtonLink = businessContentClassicDanceVideoSection_ViewModel.ButtonLink,
                    ButtonText = businessContentClassicDanceVideoSection_ViewModel.ButtonText,
                    Mode = businessContentClassicDanceVideoSection_ViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessVideoImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceVideoImage), _PreviousBusinessVideoImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceVideoImage), _BusinessVideoImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssVideoImageFile, NewImageFileWithPath);
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
        /// To Get Classic Dance Video detail By BusinessOwnerLloginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/GetBusinessContentClassicDanceVideoDetail")]

        public HttpResponseMessage GetBusinessContentClassicDanceVideoDetail()
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

                BusinessContentClassicDanceVideoDetail_VM resp = new BusinessContentClassicDanceVideoDetail_VM();
                resp = classsicDanceService.GetClassicDanceVideoDetail(_BusinessOwnerLoginId);


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
        /// To Get Classic Dance Video Detail By BusinessOwnerLoginId For Visitor Panel
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
      
        [Route("api/ClassicDance/GetBusinessContentClassicDanceVideoDetailForVisitorPanel")]

        public HttpResponseMessage GetBusinessContentClassicDanceVideoDetailForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                

                BusinessContentClassicDanceVideoDetail_VM resp = new BusinessContentClassicDanceVideoDetail_VM();
                resp = classsicDanceService.GetClassicDanceVideoDetail(businessOwnerLoginId);


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
        /// To Add/Update Classic Dance Profile Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/AddUpdateBusinessClassicDanceProfileDetail")]

        public HttpResponseMessage AddUpdateBusinessClassicDanceProfileDetail()
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
                ClassicDanceProfileDetailViewModel classicDanceProfileDetailViewModel = new ClassicDanceProfileDetailViewModel();

                // Parse and assign values from HTTP request parameters
                classicDanceProfileDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                classicDanceProfileDetailViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                classicDanceProfileDetailViewModel.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;



                classicDanceProfileDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = classicDanceProfileDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssClassicImageFile = files["ClassicImage"];
                classicDanceProfileDetailViewModel.ClassicImage = _BusinesssClassicImageFile; // for validation
                string _BusinessClassicImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessClassicImageFileName = ""; // will be used to delete the file while updating.

                HttpPostedFile _BusinesssImageFile = files["Image"];
                classicDanceProfileDetailViewModel.Image = _BusinesssImageFile; // for validation
                string _BusinessImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessImageFileName = ""; // will be used to delete the file while updating.

                HttpPostedFile _BusinesssScheduleImageFile = files["ScheduleImage"];
                classicDanceProfileDetailViewModel.ScheduleImage = _BusinesssScheduleImageFile; // for validation
                string _BusinessScheduleImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessScheduleImageFileName = ""; // will be used to delete the file while updating.


                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssClassicImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessClassicImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssClassicImageFile);
                    }
                     if (_BusinesssImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssImageFile);
                    }
                     if (_BusinesssScheduleImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessScheduleImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssScheduleImageFile);
                    }
                }



                if (classicDanceProfileDetailViewModel.Mode == 1)
                {
                    var respGetClassicDanceProfileDetail = classsicDanceService.GetClassicDanceProfileDetail(_BusinessOwnerLoginId);
                    if (respGetClassicDanceProfileDetail != null)
                    {
                        if (_BusinesssClassicImageFile == null)
                        {
                            _BusinessClassicImageFileNameGenerated = respGetClassicDanceProfileDetail.ClassicImage ?? "";

                        }
                        else
                        {
                            _PreviousBusinessClassicImageFileName = respGetClassicDanceProfileDetail.ClassicImage ?? "";
                        }

                        if (_BusinesssImageFile == null)
                        {
                            _BusinessImageFileNameGenerated = respGetClassicDanceProfileDetail.Image ?? "";

                        }
                        else
                        {
                            _PreviousBusinessImageFileName = respGetClassicDanceProfileDetail.Image ?? "";
                        }
                        if (_BusinesssScheduleImageFile == null)
                        {
                            _BusinessScheduleImageFileNameGenerated = respGetClassicDanceProfileDetail.ScheduleImage ?? "";

                        }
                        else
                        {
                            _PreviousBusinessScheduleImageFileName = respGetClassicDanceProfileDetail.ScheduleImage ?? "";
                        }
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateClassicDanceProfileDetail_Get<SPResponseViewModel>(new SP_InsertUpdateClassicDanceProfileDetail_Paarm_VM
                {
                    Id = classicDanceProfileDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = classicDanceProfileDetailViewModel.Title,
                    SubTitle = classicDanceProfileDetailViewModel.SubTitle,
                   ScheduleImage = _BusinessScheduleImageFileNameGenerated,
                    ClassicImage = _BusinessClassicImageFileNameGenerated,
                    Image = _BusinessImageFileNameGenerated,
                    Mode = classicDanceProfileDetailViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessClassicImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceProfileImage), _PreviousBusinessClassicImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }
                        if (_BusinesssClassicImageFile != null)
                        {
                            // save the new image file
                            string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceProfileImage), _BusinessClassicImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_BusinesssClassicImageFile, NewImageFileWithPath);
                        }

                        if (!String.IsNullOrEmpty(_PreviousBusinessImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceProfileImage), _PreviousBusinessImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }
                        if (_BusinesssImageFile != null)
                        {
                            // save the new image file
                            string ImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceProfileImage), _BusinessImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_BusinesssImageFile, ImageFileWithPath);
                        }
                        if (!String.IsNullOrEmpty(_PreviousBusinessScheduleImageFileName))
                        {
                            // remove the previous image file
                            string RemoveScheduleImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceProfileImage), _PreviousBusinessScheduleImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveScheduleImageFileWithPath);
                        }
                        if (_BusinesssScheduleImageFile != null)
                        {
                            // save the new image file
                            string ScheduleImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessClassicDanceProfileImage), _BusinessScheduleImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_BusinesssScheduleImageFile, ScheduleImageFileWithPath);
                            #endregion
                        }
                    }
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
        /// To Get Classic Dance Profile Page Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassicDance/BusinessContentClassicDanceProfileDetail")]

        public HttpResponseMessage BusinessContentClassicDanceProfileDetail()
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


                ClassicDanceProfileDetail_VM resp = new ClassicDanceProfileDetail_VM();
                resp = classsicDanceService.GetClassicDanceProfileDetail(_BusinessOwnerLoginId);


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
        /// To Get Classic Dance Schdeule and Banner Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ClassicDance/GetClassicDanceProfileDetailForVisitorPanel")]
        public HttpResponseMessage GetClassicDanceProfileDetailForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {



                ClassicDanceProfileDetail_VM resp = new ClassicDanceProfileDetail_VM();
                resp = classsicDanceService.GetClassicDanceProfileDetail_Getlst(businessOwnerLoginId);


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