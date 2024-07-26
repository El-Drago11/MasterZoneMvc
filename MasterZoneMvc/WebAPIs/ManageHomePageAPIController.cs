using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models.Enum;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    /// <summary>
    /// This API Controller contains APIs to manage Home page sections. CRUDs
    /// </summary>
    public class ManageHomePageAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private HomePageContentService homePageContentService;

        public ManageHomePageAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            homePageContentService = new HomePageContentService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        #region Manage Home-Page-Featured-Card -------------------------------------------------------------

        /// <summary>
        /// Add or Update Home-Page-Featured-Card Image/Video
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/FeaturedCard/AddUpdate")]
        public HttpResponseMessage AddUpdateHomePageFeaturedCard()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestHomePageFeaturedCardSection_VM requestHPFCardSection_VM = new RequestHomePageFeaturedCardSection_VM();
                requestHPFCardSection_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestHPFCardSection_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestHPFCardSection_VM.Title = HttpRequest.Params["Title"].Trim();
                requestHPFCardSection_VM.Description = HttpRequest.Params["Description"].Trim();
                requestHPFCardSection_VM.ButtonText = HttpRequest.Params["ButtonText"].Trim();
                requestHPFCardSection_VM.ButtonLink = HttpRequest.Params["ButtonLink"].Trim();
                requestHPFCardSection_VM.Video = (!string.IsNullOrEmpty(HttpRequest.Params["Video"])) ? HttpRequest.Params["Video"].Trim() : "";
                //requestHPFCardSection_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestHPFCardSection_VM.Type = HttpRequest.Params["Type"].Trim();
                
                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ThumbnailImageFile = files["Thumbnail"];
                string _ThumbnailImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousThumbnailImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                if(requestHPFCardSection_VM.Type == HomePageFeaturedCardSectionType.SingleImage.ToString())
                {
                    Error_VM error_VM = requestHPFCardSection_VM.ValidInformation_SingleImageCard();
                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }
                else if(requestHPFCardSection_VM.Type == HomePageFeaturedCardSectionType.SingleVideo.ToString())
                {
                    Error_VM error_VM = requestHPFCardSection_VM.ValidInformation_SingleVideoCard();
                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }
                else
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Type Passed!";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                
                if (files.Count > 0)
                {

                    if (_ThumbnailImageFile != null)
                    {
                        _ThumbnailImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ThumbnailImageFile);
                    }
                }

                if (requestHPFCardSection_VM.Mode == 2)
                {
                    var cardItem = homePageContentService.GetHomePageFeaturedCardSectionByType(requestHPFCardSection_VM.Type);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _ThumbnailImageFileNameGenerated = cardItem.Thumbnail;
                    }
                    else
                    {
                        _PreviousThumbnailImageFileName = cardItem.Thumbnail;
                    }
                }

                // Insert-Update Card-Section
                var resp = homePageContentService.InsertUpdateHomePageFeaturedCardSection(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageFeaturedCardSection
                {
                    Id = requestHPFCardSection_VM.Id,
                    Type = requestHPFCardSection_VM.Type,
                    Title = requestHPFCardSection_VM.Title,
                    Description = requestHPFCardSection_VM.Description,
                    Thumbnail = _ThumbnailImageFileNameGenerated,
                    ButtonText = requestHPFCardSection_VM.ButtonText,
                    ButtonLink = requestHPFCardSection_VM.ButtonLink,
                    Video = requestHPFCardSection_VM.Video,
                    Status = requestHPFCardSection_VM.Status,
                    SubmittedByLoginId = _LoginId
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Delete previous and update new image if passed
                    fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_HomePage, _PreviousThumbnailImageFileName, _ThumbnailImageFileNameGenerated, _ThumbnailImageFile);
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
        /// Get Home-Page-Featured-Card Image/Video Detail By Type
        /// </summary>
        /// <param name="type">Home-Page-Featured-Card Type</param>
        /// <returns>Home-Page-Featured-Card Image/Video Detail</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/FeaturedCard/GetByType")]
        public HttpResponseMessage GetHomePageFeaturedCardDetail(string type)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get Card-Section
                var resp = homePageContentService.GetHomePageFeaturedCardSectionByType(type);

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
        /// Get All Home-Page-Featured-Card Image/Video - For Home Page
        /// </summary>
        /// <returns>Home-Page-Featured-Card Image/Video Items</returns>
        [HttpGet]
        [Route("api/ManageHomePage/FeaturedCard/GetAll")]
        public HttpResponseMessage GetHomePageFeaturedCards()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                // Get Image Card-Section
                var respImageCards = homePageContentService.GetHomePageFeaturedCardSectionByType(HomePageFeaturedCardSectionType.SingleImage.ToString());
                // Get Video Card-Section
                var respVideoCards = homePageContentService.GetHomePageFeaturedCardSectionByType(HomePageFeaturedCardSectionType.SingleVideo.ToString());

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    ImageCard = respImageCards.Status == 1 ? respImageCards : null,
                    VideoCard = respVideoCards.Status == 1 ? respVideoCards : null
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

        #endregion -------------------------------------------------------------------------------------------

        #region Manage Home-Page-Multiple-Item -------------------------------------------------------------

        /// <summary>
        /// Add or Update Home-Page-Multiple-Item Image/Video
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/MultipleItem/AddUpdate")]
        public HttpResponseMessage AddUpdateHomePageMultipleItem()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestHomePageMultipleItem_VM requestHPMultipleItem_VM = new RequestHomePageMultipleItem_VM();
                requestHPMultipleItem_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestHPMultipleItem_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestHPMultipleItem_VM.Title = HttpRequest.Params["Title"].Trim();
                requestHPMultipleItem_VM.Description = HttpRequest.Params["Description"].Trim();
                requestHPMultipleItem_VM.Link = HttpRequest.Params["Link"].Trim();
                requestHPMultipleItem_VM.Video = (!string.IsNullOrEmpty(HttpRequest.Params["Video"])) ? HttpRequest.Params["Video"].Trim() : "";
                requestHPMultipleItem_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestHPMultipleItem_VM.Type = HttpRequest.Params["Type"].Trim();

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ThumbnailImageFile = files["Thumbnail"];
                string _ThumbnailImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousThumbnailImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                if (requestHPMultipleItem_VM.Type == HomePageMultipleItemType.MultipleImage.ToString())
                {
                    Error_VM error_VM = requestHPMultipleItem_VM.ValidInformation_MultipleImageItem();
                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }
                else if (requestHPMultipleItem_VM.Type == HomePageMultipleItemType.MultipleVideo.ToString())
                {
                    Error_VM error_VM = requestHPMultipleItem_VM.ValidInformation_MultipleVideoItem();
                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }
                else
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Type Passed!";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {

                    if (_ThumbnailImageFile != null)
                    {
                        _ThumbnailImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ThumbnailImageFile);
                    }
                }

                if (requestHPMultipleItem_VM.Mode == 2)
                {
                    var cardItem = homePageContentService.GetHomePageMultipleItemDataById(requestHPMultipleItem_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _ThumbnailImageFileNameGenerated = cardItem.Thumbnail;
                    }
                    else
                    {
                        _PreviousThumbnailImageFileName = cardItem.Thumbnail;
                    }
                }

                // Insert-Update Card-Section
                var resp = homePageContentService.InsertUpdateHomePageMultipleItem(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageMultipleItem_Params_VM()
                {
                    Id = requestHPMultipleItem_VM.Id,
                    Type = requestHPMultipleItem_VM.Type,
                    Title = requestHPMultipleItem_VM.Title,
                    Description = requestHPMultipleItem_VM.Description,
                    Thumbnail = _ThumbnailImageFileNameGenerated,
                    Link = requestHPMultipleItem_VM.Link,
                    Video = requestHPMultipleItem_VM.Video,
                    Status = requestHPMultipleItem_VM.Status,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestHPMultipleItem_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Delete previous and update new image if passed
                    fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_HomePage, _PreviousThumbnailImageFileName, _ThumbnailImageFileNameGenerated, _ThumbnailImageFile);
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
        /// GetAll Home-Page-Multiple-Item Image/Video List By Type
        /// </summary>
        /// <param name="type">Multiple-Item-Type</param>
        /// <returns>Multiple-Item List</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/MultipleItem/GetAllByType")]
        public HttpResponseMessage GetHomePageMultipleItemList(string type)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get List Multiple-Item by Type
                var resp = homePageContentService.GetAllHomePageMultipleItemsByType(type);

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
        /// Get Home-Page-Multiple-Item Image/Video Detail By Id
        /// </summary>
        /// <param name="id">Multiple-Item-Id</param>
        /// <returns>Single Multiple-Item Detail</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/MultipleItem/GetById")]
        public HttpResponseMessage GetHomePageMultipleItemList(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get List Multiple-Item by Type
                var resp = homePageContentService.GetHomePageMultipleItemDataById(id);

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
        /// Delete Home-Page-Multiple-Item Image/Video Detail By Id
        /// </summary>
        /// <param name="id">Multiple-Item-Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/MultipleItem/DeleteById")]
        public HttpResponseMessage DeleteHomePageMultipleItem(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                var _item = homePageContentService.GetHomePageMultipleItemDataById(id);
                if(_item == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.NotFound;
                    apiResponse.data = null;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get List Multiple-Item by Type
                var resp = homePageContentService.InsertUpdateHomePageMultipleItem(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageMultipleItem_Params_VM()
                {
                    Id = id,
                    Mode = 3
                });

                // delete the file from the server due to hard-delete
                if(resp.ret == 1)
                {
                    string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_HomePage), _item.Thumbnail);
                    fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                }

                apiResponse.status = 1;
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
        /// Get All Home-Page-Featured-Card Image/Video List - For Home Page
        /// </summary>
        /// <returns>Data obeject containing Multiple-Item Image and Video list</returns>
        [HttpGet]
        [Route("api/ManageHomePage/MultipleItem/GetAllForHomePage")]
        public HttpResponseMessage GetHomePageMultipleItemsForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                // Get Image List - Active
                var respImageList = homePageContentService.GetAllHomePageMultipleItemsByType(HomePageMultipleItemType.MultipleImage.ToString()).Where(i => i.Status == 1).ToList();
                // Get Video List - Active
                var respVideoList = homePageContentService.GetAllHomePageMultipleItemsByType(HomePageMultipleItemType.MultipleVideo.ToString()).Where(i => i.Status == 1).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    MultipleImageList = respImageList,
                    MultipleVideoList = respVideoList
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

        #endregion -------------------------------------------------------------------------------------------

        #region Manage Home-Page-Class-Category-Section -------------------------------------------------------------

        /// <summary>
        /// Add or Update Home-Page-Class-Category-Section Data
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/ClassCategorySection/AddUpdate")]
        public HttpResponseMessage AddUpdateHomePageClassCategorySectionData()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestHomePageClassCategorySection_VM requestHPClassCategorySection_VM = new RequestHomePageClassCategorySection_VM();
                requestHPClassCategorySection_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestHPClassCategorySection_VM.ClassCategoryTypeId = Convert.ToInt64(HttpRequest.Params["ClassCategoryTypeId"]);
                requestHPClassCategorySection_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                
                Error_VM error_VM = requestHPClassCategorySection_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update 
                var resp = homePageContentService.InsertUpdateHomePageClassCategorySection(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageClassCategorySection()
                {
                    Id = requestHPClassCategorySection_VM.Id,
                    ClassCategoryTypeId = requestHPClassCategorySection_VM.ClassCategoryTypeId,
                    Status = requestHPClassCategorySection_VM.Status,
                    SubmittedByLoginId = _LoginId,
                    Mode = 1
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
        /// Get Class-Category-Section Detail
        /// </summary>
        /// <param name="type">Home-Page-Featured-Card Type</param>
        /// <returns>Class-Category-Section Detail</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/ClassCategorySection/Get")]
        public HttpResponseMessage GetHomePageClassCategorySectionDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get Card-Section
                var resp = homePageContentService.GetHomePageClassCategorySectionData();

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
        /// Get All Home-Page-Class-Category-Section - For Home Page
        /// </summary>
        /// <returns>Section data</returns>
        [HttpGet]
        [Route("api/ManageHomePage/ClassCategorySection/GetDetailForHomePage")]
        public HttpResponseMessage GetClassCategorySectionDetailForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var homePageClassCategorySectionDetail = homePageContentService.GetHomePageClassCategorySectionData();

                // Get Parent-Category-Detail 
                ClassCategoryTypeService classCategoryTypeService = new ClassCategoryTypeService(db);
                var parentCategoryDetail = classCategoryTypeService.GetClassCategoryTypeById(homePageClassCategorySectionDetail.ClassCategoryTypeId);

                // Get Sub-Categories list - Active
                var subCategories = homePageContentService.GetHomePageClassCategorySection_SubCategoriesList();
                
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    classCategorySection = homePageClassCategorySectionDetail,
                    ParentClassCategoryDetail = parentCategoryDetail,
                    SubCategories = subCategories
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

        #endregion -------------------------------------------------------------------------------------------


        #region Manage Home-Page-Featured-Video -------------------------------------------------------------

        /// <summary>
        /// Add or Update Home-Page-Featured-Video
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/FeaturedVideo/AddUpdate")]
        public HttpResponseMessage AddUpdateHomePageFeaturedVideo()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestHomePageFeaturedVideo_VM requestHPFVideo_VM = new RequestHomePageFeaturedVideo_VM();
                requestHPFVideo_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestHPFVideo_VM.Title = HttpRequest.Params["Title"] ?? "";
                requestHPFVideo_VM.Description = HttpRequest.Params["Description"] ?? "";
                //requestHPFVideo_VM.Video = (!string.IsNullOrEmpty(HttpRequest.Params["Video"])) ? HttpRequest.Params["Video"].Trim() : "";
                requestHPFVideo_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _VideoFile = files["Video"];
                string _VideoFileNameGenerated = ""; //will contains generated file name
                string _PreviousVideoFileName = ""; // will be used to delete file while updating.
                requestHPFVideo_VM.Video = _VideoFile;

                // Validate infromation passed
                Error_VM error_VM = requestHPFVideo_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {

                    if (_VideoFile != null)
                    {
                        _VideoFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_VideoFile);
                    }
                }

                if (requestHPFVideo_VM.Mode == 2)
                {
                    var cardItem = homePageContentService.GetHomePageFeaturedVideoById(requestHPFVideo_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _VideoFileNameGenerated = cardItem.Video;
                    }
                    else
                    {
                        _PreviousVideoFileName = cardItem.Video;
                    }
                }

                // Insert-Update Home-Page-Featured-Video
                var resp = homePageContentService.InsertUpdateHomePageFeaturedVideo(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageFeaturedVideo_Params_VM()
                {
                    Id = requestHPFVideo_VM.Id,
                    Video = _VideoFileNameGenerated,
                    Title = requestHPFVideo_VM.Title,
                    Description = requestHPFVideo_VM.Description,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestHPFVideo_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Delete previous and update new image if passed
                    fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_HomePageVideos, _PreviousVideoFileName, _VideoFileNameGenerated, _VideoFile);
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
        /// Get Home-Page-Featured-Video Detail By Type
        /// </summary>
        /// <returns>Home-Page-Featured-Video Image/Video Detail</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/FeaturedVideo/GetAll")]
        public HttpResponseMessage GetAllHomePageFeaturedVideos()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get All Videos
                var resp = homePageContentService.GetAllHomePageFeaturedVideos();

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
        /// Delete Home-Page-Featured-Video By Id
        /// </summary>
        /// <param name="id">Home-Page-Featured-Video-Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/FeaturedVideo/DeleteById")]
        public HttpResponseMessage DeleteHomePageFeaturedVideo(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                var _item = homePageContentService.GetHomePageFeaturedVideoById(id);
                if (_item == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.NotFound;
                    apiResponse.data = null;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get List Multiple-Item by Type
                var resp = homePageContentService.InsertUpdateHomePageFeaturedVideo(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageFeaturedVideo_Params_VM()
                {
                    Id = id,
                    Mode = 3
                });

                // delete the file from the server due to hard-delete
                if (resp.ret == 1)
                {
                    string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_HomePageVideos), _item.Video);
                    fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                }

                apiResponse.status = 1;
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
        /// Get All Home-Page-Featured-Video - For Home Page
        /// </summary>
        /// <returns>Home-Page-Featured-Videos Items</returns>
        [HttpGet]
        [Route("api/ManageHomePage/FeaturedVideo/GetAllForHomePage")]
        public HttpResponseMessage GetHomePageFeaturedVideosForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
               // Get Videos - Active
                var respVideoCards = homePageContentService.GetAllHomePageFeaturedVideos().Where(v => v.Status == 1).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respVideoCards;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion -------------------------------------------------------------------------------------------


        #region Manage Home-Page-Multiple-Item -------------------------------------------------------------

        /// <summary>
        /// Add or Update Home-Page-Banner-Item Image/Video
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/BannerItem/AddUpdate")]
        public HttpResponseMessage AddUpdateHomePageBannerItem()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestHomePageBannerItem_VM requestHPBannerItem_VM = new RequestHomePageBannerItem_VM();
                requestHPBannerItem_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestHPBannerItem_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestHPBannerItem_VM.Video = (!string.IsNullOrEmpty(HttpRequest.Params["Video"])) ? HttpRequest.Params["Video"].Trim() : "";
                requestHPBannerItem_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestHPBannerItem_VM.Type = HttpRequest.Params["Type"].Trim();
                requestHPBannerItem_VM.Text = HttpRequest.Params["Text"] ?? "";
                requestHPBannerItem_VM.Link = HttpRequest.Params["Link"] ?? "";

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ImageFile = files["Image"];
                string _ImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousImageFileName = ""; // will be used to delete file while updating.
                requestHPBannerItem_VM.Image = _ImageFile;
                // IF Image Type
                if (requestHPBannerItem_VM.Type == HomePageBannerItemType.Image.ToString())
                {
                    // Validate infromation passed
                    Error_VM error_VM = requestHPBannerItem_VM.ValidInformation_ImageItem();
                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    if (files.Count > 0)
                    {

                        if (_ImageFile != null)
                        {
                            _ImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ImageFile);
                        }
                    }

                    if (requestHPBannerItem_VM.Mode == 2)
                    {
                        var cardItem = homePageContentService.GetHomePageBannerItemById(requestHPBannerItem_VM.Id);

                        // if no image update then update name of previous image else need to delete previous image
                        if (files.Count <= 0)
                        {
                            _ImageFileNameGenerated = cardItem.Image;
                        }
                        else
                        {
                            _PreviousImageFileName = cardItem.Image;
                        }
                    }

                    // Insert-Update Banner-Item
                    var resp = homePageContentService.InsertUpdateHomePageBannerItem(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageBannerItem_Params_VM()
                    {
                        Id = requestHPBannerItem_VM.Id,
                        Type = requestHPBannerItem_VM.Type,
                        Image = _ImageFileNameGenerated,
                        Video = requestHPBannerItem_VM.Video,
                        Status = requestHPBannerItem_VM.Status,
                        Text = requestHPBannerItem_VM.Text,
                        Link = requestHPBannerItem_VM.Link,
                        SubmittedByLoginId = _LoginId,
                        Mode = requestHPBannerItem_VM.Mode
                    });


                    if (resp.ret <= 0)
                    {
                        apiResponse.status = resp.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    if (resp.ret == 1)
                    {
                        // Delete previous and update new image if passed
                        fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_HomePage, _PreviousImageFileName, _ImageFileNameGenerated, _ImageFile);
                    }

                    apiResponse.status = 1;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    // Returend back from here

                }
                // IF Video Type
                else if (requestHPBannerItem_VM.Type == HomePageBannerItemType.Video.ToString())
                {
                    // Validate infromation passed
                    Error_VM error_VM = requestHPBannerItem_VM.ValidInformation_VideoItem();
                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    // Insert-Update Banner-Item
                    var resp = homePageContentService.InsertUpdateHomePageBannerItem(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageBannerItem_Params_VM()
                    {
                        Id = requestHPBannerItem_VM.Id,
                        Type = requestHPBannerItem_VM.Type,
                        Image = _ImageFileNameGenerated,
                        Video = requestHPBannerItem_VM.Video,
                        Text = requestHPBannerItem_VM.Text,
                        Link = requestHPBannerItem_VM.Link,
                        Status = requestHPBannerItem_VM.Status,
                        SubmittedByLoginId = _LoginId,
                        Mode = requestHPBannerItem_VM.Mode
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
                    // Returend back from here
                }
                else
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Type Passed!";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
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
        /// Get All Home-Page-Banner-Item Image/Video List
        /// </summary>
        /// <param name="type">Banner-Item-Type</param>
        /// <returns>Banner-Item List</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/BannerItem/GetAll")]
        public HttpResponseMessage GetAllHomePageBannerItemList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get List 
                var resp = homePageContentService.GetAllHomePageBannerItems();

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
        /// Get Home-Page-Banner-Item Image/Video Detail By Id
        /// </summary>
        /// <param name="id">Banner-Item-Id</param>
        /// <returns>Single Banner-Item Detail</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/BannerItem/GetById")]
        public HttpResponseMessage GetHomePageBannerItemById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Get Item By Id
                var resp = homePageContentService.GetHomePageBannerItemById(id);

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
        /// Delete Home-Page-Banner-Item Image/Video Detail By Id
        /// </summary>
        /// <param name="id">Multiple-Item-Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ManageHomePage/BannerItem/DeleteById")]
        public HttpResponseMessage DeleteHomePageBannerItemById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                var _item = homePageContentService.GetHomePageBannerItemById(id);
                if (_item == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.NotFound;
                    apiResponse.data = null;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Delete Banner-Item By Id
                var resp = homePageContentService.InsertUpdateHomePageBannerItem(new ViewModels.StoredProcedureParams.SP_InsertUpdateHomePageBannerItem_Params_VM()
                {
                    Id = id,
                    Mode = 3
                });

                // delete the file from the server due to hard-delete
                if (resp.ret == 1)
                {
                    string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_HomePage), _item.Image);
                    fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                }

                apiResponse.status = 1;
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
        /// Get All Home-Page-Banner-Item Image/Video List - For Home Page
        /// </summary>
        /// <returns>Data obeject containing Banner-Item Image and Video list</returns>
        [HttpGet]
        [Route("api/ManageHomePage/BannerItem/GetAllForHomePage")]
        public HttpResponseMessage GetHomePageBannerItemsForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                // Get List - Active
                var respList = homePageContentService.GetAllHomePageBannerItems().Where(i => i.Status == 1).ToList();
                
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respList;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion -------------------------------------------------------------------------------------------

    }
}