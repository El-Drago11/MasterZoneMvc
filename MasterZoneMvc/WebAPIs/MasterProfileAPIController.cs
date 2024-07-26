using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
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
using System.Data.SqlClient;

namespace MasterZoneMvc.WebAPIs
{
    public class MasterProfileAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessOwnerService businessOwnerService;
        private MasterProfileService masterProfileService;
        private StoredProcedureRepository storedProcedureRepository;


        public MasterProfileAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessOwnerService = new BusinessOwnerService(db);
            masterProfileService = new MasterProfileService(db);
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
        /// To Add/Update Explore  Detail (Master Profile)
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/AddUpdateExploreMasterProfileDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateExploreMasterProfileDetail_PPCMeta()
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
                BusinessContentExploreClassicDance_ViewModel businessContentExploreClassicDance_ViewModel = new BusinessContentExploreClassicDance_ViewModel();
                businessContentExploreClassicDance_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);

                // Check and set Title 

                string TitleParam = HttpRequest.Params["Title"];
                businessContentExploreClassicDance_ViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                businessContentExploreClassicDance_ViewModel.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                businessContentExploreClassicDance_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files

                // Validate infromation passed
                Error_VM error_VM = businessContentExploreClassicDance_ViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentExploreClassicDanceDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessExploreDClassicDanceDetail_Param_VM
                {
                    Id = businessContentExploreClassicDance_ViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessContentExploreClassicDance_ViewModel.Title,
                    Description = businessContentExploreClassicDance_ViewModel.Description,
                    SubmittedByLoginId = _BusinessOwnerLoginId,
                    Mode = businessContentExploreClassicDance_ViewModel.Mode
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
        /// To Get Explore Master Profile Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetBusinessContentExploreMasterProfileDetail_PPCMeta")]
        public HttpResponseMessage GetBusinessContentExploreMasterProfileDetail_PPCMeta()
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




                BusinessExploreClassicDanceDetail_VM resp = new BusinessExploreClassicDanceDetail_VM();
                resp = masterProfileService.GetExploreClassicDanceDetail_Get(_BusinessOwnerLoginId);

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
        /// To Get Explore Detail For VisitorPanel(Master Profile)
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetBusinessContentExploreMasterProfileDetail_ForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentExploreMasterProfileDetail_ForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {




                BusinessExploreClassicDanceDetail_VM resp = new BusinessExploreClassicDanceDetail_VM();
                resp = masterProfileService.GetExploreClassicDanceDetail_Get(businessOwnerLoginId);

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
        /// To Add/Update Find Master Profile API 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessFindMasterProfile")]

        public HttpResponseMessage AddUpdateBusinessFindMasterProfile()
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

               // var schdeuleLink = "https://localhost:44305/Home/InstructorResearch?businessOwnerLoginId=0";
                var schdeuleLink = "";

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //  BusinesssProfilePageType.Key = "yoga_web";
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                BusinessContentFindMasterProfile_ViewModel businessContentFindMasterProfile_ViewModel = new BusinessContentFindMasterProfile_ViewModel();
                businessContentFindMasterProfile_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentFindMasterProfile_ViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessContentFindMasterProfile_ViewModel.Description = HttpRequest.Params["Description"].Trim();
                businessContentFindMasterProfile_ViewModel.ExploreType = HttpRequest.Params["ExploreType"].Trim();
                businessContentFindMasterProfile_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                // Validate information passed
                Error_VM error_VM = businessContentFindMasterProfile_ViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssFindImageFile = files["Image"];
                businessContentFindMasterProfile_ViewModel.Image = _BusinesssFindImageFile; // for validation
                string _BusinessFindImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessFindImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssFindImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessFindImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssFindImageFile);
                    }
                }

                //// If no new image is uploaded, keep the previous image filename
                //if (string.IsNullOrEmpty(_BusinessBannerImageFileNameGenerated))
                //{
                //    _BusinessBannerImageFileNameGenerated = _PreviousBusinessBannerImageFileName;
                //}

                if (businessContentFindMasterProfile_ViewModel.Mode == 1)
                {
                    var respGetBusinessFindImageDetail = masterProfileService.GetFindMasterProfileDetail_Get(_BusinessOwnerLoginId);

                    if (respGetBusinessFindImageDetail != null && _BusinesssFindImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessFindImageDetail.Image == null)
                        {
                            _PreviousBusinessFindImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessFindImageFileNameGenerated = respGetBusinessFindImageDetail.Image;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessFindImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentFindMasterProfilDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessFindMasterProfile_Param_VM
                {
                    Id = businessContentFindMasterProfile_ViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentFindMasterProfile_ViewModel.Title,
                    ExploreType = businessContentFindMasterProfile_ViewModel.ExploreType,
                    Description = businessContentFindMasterProfile_ViewModel.Description,
                    Image = _BusinessFindImageFileNameGenerated,
                    ScheduleLink = schdeuleLink,
                    Mode = businessContentFindMasterProfile_ViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessFindImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessFindMasterProfileImage), _PreviousBusinessFindImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessFindMasterProfileImage), _BusinessFindImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssFindImageFile, NewImageFileWithPath);
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
        /// To Get Find Master Profile Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetBusinessContentFindMasterProfileDetail")]
        public HttpResponseMessage GetBusinessContentFindMasterProfileDetail_ForVisitorPanel(long id)
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



                BussinessContentFindMasterProfilelDetail_VM resp = new BussinessContentFindMasterProfilelDetail_VM();
                resp = masterProfileService.GetFindMasterProfileDetail_Get(id);

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
        /// To Get Master Profile Detail List In Find Explore Section 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetBusinessContentFindMasterProfileDetail_ByPagination")]
        public HttpResponseMessage GetBusinessContentFindMasterProfileDetail()
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

                List<BussinessContentFindMasterProfilelDetail_VM> resp = masterProfileService.GetFindMasterProfileDetailList_Get(_BusinessOwnerLoginId);


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
        /// To Delete Find Master Profile Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/DeleteFindMasterProfileDetailById")]
        public HttpResponseMessage DeleteFindMasterProfileDetailById(long id)
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
                var resp = masterProfileService.DeleteBusinessContentFindExploreDetail(id);


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
        /// To Get Find Explore Detail for Visitor-Panel (List)
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetBusinessContentFindMasterProfileDetail_ForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentFindMasterProfileDetailList_ForVisitorPanel(string exploreType)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BussinessContentFindMasterProfilelDetail_VM> resp = masterProfileService.Get_FindMasterProfileDetailList(exploreType);


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
        /// To Get All  Class Category  Detail LastRecordId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/MasterProfile/GetClassesCategoryTypeDetailList")]
        public HttpResponseMessage GetClassesCategoryTypeDetailList(long businessOwnerLoginId, long categoryTypeId, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<Class_VM> response = masterProfileService.GetAllClassDetail_lst(businessOwnerLoginId, categoryTypeId, lastRecordId, recordLimit);

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



        /// <summary>
        /// To Show All Classes Category Id 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="categoryTypeId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetShowAllClassesDetail")]
        public HttpResponseMessage GetShowAllClassesDetail(long businessOwnerLoginId, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<Class_VM> response = masterProfileService.GetAllShowClassCategoryDetail_lst(businessOwnerLoginId, lastRecordId, recordLimit);

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


        //Instructor Detail For Baner

        /// <summary>
        /// To Add/Update Instructor/MemberShip Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessInstructorBannerMasterProfile")]

        public HttpResponseMessage AddUpdateBusinessInstructorBannerMasterProfile()
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

                BusinessContentInstructorBannerDetailViewModel businessContentInstructorBannerDetailViewModel = new BusinessContentInstructorBannerDetailViewModel();
                businessContentInstructorBannerDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentInstructorBannerDetailViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessContentInstructorBannerDetailViewModel.Description = HttpRequest.Params["Description"].Trim();
                businessContentInstructorBannerDetailViewModel.SubTitle = HttpRequest.Params["SubTitle"].Trim();
                businessContentInstructorBannerDetailViewModel.BannerType = HttpRequest.Params["BannerType"].Trim();
                businessContentInstructorBannerDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                // Validate information passed
                Error_VM error_VM = businessContentInstructorBannerDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssInstructorImageFile = files["BannerImage"];
                businessContentInstructorBannerDetailViewModel.BannerImage = _BusinesssInstructorImageFile; // for validation
                string _BusinessInstructorImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessInstructorImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssInstructorImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessInstructorImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssInstructorImageFile);
                    }
                }

                if (businessContentInstructorBannerDetailViewModel.Mode == 1)
                {
                    var respGetBusinessInstructorImageDetail = masterProfileService.GetInstructorBannerMasterProfileDetail(businessContentInstructorBannerDetailViewModel.Id);

                    if (respGetBusinessInstructorImageDetail != null && _BusinesssInstructorImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessInstructorImageDetail.BannerImage == null)
                        {
                            _PreviousBusinessInstructorImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessInstructorImageFileNameGenerated = respGetBusinessInstructorImageDetail.BannerImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessInstructorImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentInstructorBannerMasterProfileDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentInstructorBannerDetail_Param_VM
                {
                    Id = businessContentInstructorBannerDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentInstructorBannerDetailViewModel.Title,
                    BannerType = businessContentInstructorBannerDetailViewModel.BannerType,
                    Description = businessContentInstructorBannerDetailViewModel.Description,
                    BannerImage = _BusinessInstructorImageFileNameGenerated,
                    SubTitle = businessContentInstructorBannerDetailViewModel.SubTitle,
                    Mode = businessContentInstructorBannerDetailViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessInstructorImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessMasterProfileInstructorBannerImage), _PreviousBusinessInstructorImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessMasterProfileInstructorBannerImage), _BusinessInstructorImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssInstructorImageFile, NewImageFileWithPath);
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
        /// To Get Find Master Profile Detail  By Id 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetBusinessContentInstructorBannerDetailById")]
        public HttpResponseMessage GetBusinessContentInstructorBannerDetailById(long id)
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



                BusinessContentInstructorBannerDetail_VM resp = new BusinessContentInstructorBannerDetail_VM();
                resp = masterProfileService.GetInstructorBannerMasterProfileDetail(id);

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
        /// To Get Instructor/MemberShip Detail By BusinesOwnerLoginId 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetInstructorBannerMasterProfileDetailListByPagination")]
        public HttpResponseMessage GetInstructorBannerMasterProfileDetailListByPagination()
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


                List<BusinessContentInstructorBannerDetail_VM> resp = masterProfileService.GetInstructorBannerMasterProfileDetail_Get(_BusinessOwnerLoginId);

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
        /// To  get Instructor/MemberShip Detail by Banner Type 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetInstructorBannerMasterProfileDetailListFor_VisitorPanel")]
        public HttpResponseMessage GetInstructorBannerMasterProfileDetailListFor_VisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessContentInstructorBannerDetail_VM> resp = masterProfileService.GetInstructorBannerMasterProfileDetail_Get(businessOwnerLoginId);

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
        /// To Delete Instructor Banner Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/DeleteInstructorBannerDetailForMasterProfileById")]
        public HttpResponseMessage DeleteInstructorBannerDetailForMasterProfileById(long id)
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
                var resp = masterProfileService.DeleteBusinessContentInstructorBannerDetail(id);


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



        //  Master Profile Instructor About Detail 

        /// <summary>
        /// To Add/Update Instructor About  Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/AddUpdateBusinessInstructorAboutDetail")]

        public HttpResponseMessage AddUpdateBusinessInstructorAboutDetail()
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
                InstructorMasterProfileAboutDetailViewModel instructorMasterProfileAboutDetailViewModel = new InstructorMasterProfileAboutDetailViewModel();

                // Parse and assign values from HTTP request parameters
                instructorMasterProfileAboutDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string TitleParam = HttpRequest.Params["Title"];
                instructorMasterProfileAboutDetailViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;
                // Check and set SubTitle
                string subTitleParam = HttpRequest.Params["SubTitle"];
                instructorMasterProfileAboutDetailViewModel.SubTitle = !string.IsNullOrEmpty(subTitleParam) ? subTitleParam.Trim() : string.Empty;
                // Check and set Button Link
                string subButtonLinkParam = HttpRequest.Params["ButtonLink"];
                instructorMasterProfileAboutDetailViewModel.ButtonLink = !string.IsNullOrEmpty(subButtonLinkParam) ? subButtonLinkParam.Trim() : string.Empty;

                // Check and set Button Text
                string subButtonTextParam = HttpRequest.Params["ButtonText"];
                instructorMasterProfileAboutDetailViewModel.ButtonText = !string.IsNullOrEmpty(subButtonTextParam) ? subButtonTextParam.Trim() : string.Empty;

                // Check and set Button Text
                string subDescriptionParam = HttpRequest.Params["Description"];
                instructorMasterProfileAboutDetailViewModel.Description = !string.IsNullOrEmpty(subDescriptionParam) ? subDescriptionParam.Trim() : string.Empty;



                instructorMasterProfileAboutDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate information passed
                Error_VM error_VM = instructorMasterProfileAboutDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssImageFile = files["Image"];
                instructorMasterProfileAboutDetailViewModel.Image = _BusinesssImageFile; // for validation
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



                if (instructorMasterProfileAboutDetailViewModel.Mode == 1)
                {
                    var respGetBusinessInstructorImageDetail = masterProfileService.GetInstructorAboutMasterProfileDetail_Get(_BusinessOwnerLoginId);

                    if (respGetBusinessInstructorImageDetail != null && _BusinesssImageFile == null) // 
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessInstructorImageDetail.Image == null)
                        {
                            _PreviousBusinessImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessImageFileNameGenerated = respGetBusinessInstructorImageDetail.Image;
                        }
                    }
                    else
                    {

                        _PreviousBusinessImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateInstructorAboutMasterProfileDetail_Get<SPResponseViewModel>(new SP_InsertUpdateInstructorAboutMasterProfile_Param_VM
                {
                    Id = instructorMasterProfileAboutDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = instructorMasterProfileAboutDetailViewModel.Title,
                    SubTitle = instructorMasterProfileAboutDetailViewModel.SubTitle,
                    Image = _BusinessImageFileNameGenerated,
                    ButtonLink = instructorMasterProfileAboutDetailViewModel.ButtonLink,
                    ButtonText = instructorMasterProfileAboutDetailViewModel.ButtonText,
                    Description = instructorMasterProfileAboutDetailViewModel.Description,
                    Mode = instructorMasterProfileAboutDetailViewModel.Mode
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
                    #region Insert-Update  Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessInstructorAboutMasterProfileImage), _PreviousBusinessImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessInstructorAboutMasterProfileImage), _BusinessImageFileNameGenerated);
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
        /// To Get Instructor About Detail By LoginId 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetInstructorAboutMasterProfileDetail")]
        public HttpResponseMessage GetInstructorAboutMasterProfileDetail()
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

                InstructorMasterProfileAboutDetail_VM resp = new InstructorMasterProfileAboutDetail_VM();
                resp = masterProfileService.GetInstructorAboutMasterProfileDetail_Get(_BusinessOwnerLoginId);

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
        /// To Get Instructor About Detail By LoginId in Visitor Panel ( Instructor Master Profile)
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetInstructorAboutMasterProfileDetailFor_VisitorPanel")]
        public HttpResponseMessage GetInstructorAboutMasterProfileDetailFor_VisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                InstructorMasterProfileAboutDetail_VM resp = new InstructorMasterProfileAboutDetail_VM();
                resp = masterProfileService.GetInstructorAboutMasterProfileDetail_Get(businessOwnerLoginId);

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
        /// To Add/Update Term Condition Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/AddUpdateTermConditionDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateTermConditionDetail_PPCMeta()
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
                BusinessContentTermConditionViewModel businessContentTermConditionViewModel = new BusinessContentTermConditionViewModel();
                businessContentTermConditionViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);

                // Check and set Title 

                string TitleParam = HttpRequest.Params["Title"];
                businessContentTermConditionViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                businessContentTermConditionViewModel.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                businessContentTermConditionViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files

                // Validate infromation passed
                Error_VM error_VM = businessContentTermConditionViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdateTermConditionDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessTermCondition_Param_VM
                {
                    Id = businessContentTermConditionViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessContentTermConditionViewModel.Title,
                    Description = businessContentTermConditionViewModel.Description,
                    Mode = businessContentTermConditionViewModel.Mode
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
        /// To Get Term Condition Detail by LoginId 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetTermConditionDetail")]
        public HttpResponseMessage GetTermConditionDetail()
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

                BusinessContentTermConditionDetail_VM resp = new BusinessContentTermConditionDetail_VM();
                resp = masterProfileService.GetTermConditionDetail_Get(_BusinessOwnerLoginId);

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
        /// To Get Term Condition Detail for Visitor Panel
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetTermConditionDetailFor_VisitorPanel")]
        public HttpResponseMessage GetTermConditionDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                BusinessContentTermConditionDetail_VM resp = new BusinessContentTermConditionDetail_VM();
                resp = masterProfileService.GetTermConditionDetail_Get(businessOwnerLoginId);

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
        /// To Add/Update MemberShip Package Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/AddUpdateMemberShipPackageDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateMemberShipPackageDetail_PPCMeta()
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
                BusinessMemberShipPackageDetailViewModel businessMemberShipPackageDetailViewModel = new BusinessMemberShipPackageDetailViewModel();
                businessMemberShipPackageDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);

                // Check and set Title 

                string TitleParam = HttpRequest.Params["Title"];
                businessMemberShipPackageDetailViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                businessMemberShipPackageDetailViewModel.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                businessMemberShipPackageDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                businessMemberShipPackageDetailViewModel.PlanTypeId = Convert.ToInt32(HttpRequest.Params["PlanTypeId"]);



                // Get Attatched Files

                // Validate infromation passed
                Error_VM error_VM = businessMemberShipPackageDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdateMemberShipPackageDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessMemberShipPackage_Param_VM
                {
                    Id = businessMemberShipPackageDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessMemberShipPackageDetailViewModel.Title,
                    Description = businessMemberShipPackageDetailViewModel.Description,
                    PlanTypeId = businessMemberShipPackageDetailViewModel.PlanTypeId,
                    Mode = businessMemberShipPackageDetailViewModel.Mode
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
        /// To Get MemberShip Package Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetMemberShipPackageDetail")]
        public HttpResponseMessage GetMemberShipPackageDetail()
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

                BusinessMemberShipPackageDetail_VM resp = new BusinessMemberShipPackageDetail_VM();
                resp = masterProfileService.GetMemberShipPackageDetail_Get(_BusinessOwnerLoginId);

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
        /// To get MemberShip Package Detail By LoginId For Visitor Panel 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
  
        [Route("api/MasterProfile/GetMemberShipPackageDetailFor_VisitorPanel")]
        public HttpResponseMessage GetMemberShipPackageDetailFor_VisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                

                BusinessMemberShipPackageDetail_VM resp = new BusinessMemberShipPackageDetail_VM();
                resp = masterProfileService.GetMemberShipPackageDetail_Get(businessOwnerLoginId);

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
        /// To Add/Update MemberShip Plan Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/AddUpdateMemberShipPlanDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateMemberShipPlanDetail_PPCMeta()
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

                BusinessMemberShipPlanDetailViewModel businessMemberShipPlanDetailViewModel = new BusinessMemberShipPlanDetailViewModel();
                businessMemberShipPlanDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessMemberShipPlanDetailViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessMemberShipPlanDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                // Validate information passed
                Error_VM error_VM = businessMemberShipPlanDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssMemberShipPlanImageFile = files["Image"];
                businessMemberShipPlanDetailViewModel.Image = _BusinesssMemberShipPlanImageFile; // for validation
                string _BusinessMemberShipPlanImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessMemberShipPlanImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssMemberShipPlanImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessMemberShipPlanImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssMemberShipPlanImageFile);
                    }
                }

              


                if (businessMemberShipPlanDetailViewModel.Mode == 2)
                {

                    var respGetMemberShipPlanImageDetail = masterProfileService.GetMemberShipPlanDetail_Get(businessMemberShipPlanDetailViewModel.Id);


                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessMemberShipPlanImageFileNameGenerated = respGetMemberShipPlanImageDetail.Image ?? "";

                    }
                    else
                    {
                        _PreviousBusinessMemberShipPlanImageFileName = respGetMemberShipPlanImageDetail.Image ?? "";

                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateMemberShipPlanDetail_Get<SPResponseViewModel>(new SP_InsertUpdateMemberShipPlan_Param_VM
                {
                    Id = businessMemberShipPlanDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessMemberShipPlanDetailViewModel.Title,
                    Image = _BusinessMemberShipPlanImageFileNameGenerated,
                    Mode = businessMemberShipPlanDetailViewModel.Mode
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
                        if (!String.IsNullOrEmpty(_PreviousBusinessMemberShipPlanImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessMemberShipPlanImage), _PreviousBusinessMemberShipPlanImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessMemberShipPlanImage), _BusinessMemberShipPlanImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssMemberShipPlanImageFile, NewImageFileWithPath);
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
        /// To Get MemberShip Plan Detail By id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetMemberShipPlanDetail_PPCMeta")]
        public HttpResponseMessage GetMemberShipPlanDetail_PPCMeta(long id)
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

                BusinessMemberShipPlanDetail_VM resp = new BusinessMemberShipPlanDetail_VM();
                resp = masterProfileService.GetMemberShipPlanDetail_Get(id);

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
        /// To Delete the Member Ship Plan Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/DeleteMemberShipPlanDetailById")]
        public HttpResponseMessage DeleteMemberShipPlanDetailById(long id)
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
                var resp = masterProfileService.DeleteBusinessContentMemberShipPlanDetail(id);


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
        /// To Get MemberShip Plan Detail List ( Using By Pagination) 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetMemberShipPlanDetail_ByPagination")]
        public HttpResponseMessage GetMemberShipPlanDetail_ByPagination()
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

               // BusinessMemberShipPlanDetail_VM resp = new BusinessMemberShipPlanDetail_VM();
                List<BusinessMemberShipPlanDetail_VM> resp = masterProfileService.GetMemberShipPlanDetailList_Get(_BusinessOwnerLoginId);

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
        /// To Get All Membership Plan Detail List For visitor Panel 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetMemberShipPlanDetailFor_VisitorPanel")]
        public HttpResponseMessage GetMemberShipPlanDetailFor_VisitorPanel( long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {



                List<BusinessMemberShipPlanDetail_VM> resp = masterProfileService.GetMemberShipPlanDetailList_Get(businessOwnerLoginId);

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
        /// To Get All Instructor Detail List By BusinessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetClassInstructorDetailList")]
        public HttpResponseMessage GetClassInstructorDetailList(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<InstructorList_VM> resp = masterProfileService.GetClassInstructorDetailList_Get(businessOwnerLoginId);

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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/MasterProfile/GetAdvanceMemberShipPackageDetail")]
        public HttpResponseMessage GetAdvanceMemberShipPackageDetail()
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

                BusinessMemberShipPackageDetail_VM resp = new BusinessMemberShipPackageDetail_VM();
                resp = masterProfileService.GetAdvanceMemberShipPackageDetail_Get(_BusinessOwnerLoginId);

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
        /// to get advance details MemberShip
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MasterProfile/GetAdvanceMemberShipPackageDetailFor_VisitorPanel")]
        public HttpResponseMessage GetAdvanceMemberShipPackageDetailFor_VisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessMemberShipPackageDetail_VM resp = new BusinessMemberShipPackageDetail_VM();
                resp = masterProfileService.GetAdvanceMemberShipPackage_Get(businessOwnerLoginId);
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