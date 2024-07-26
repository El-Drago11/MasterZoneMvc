using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.Common.ValidationHelpers;
using System.Security.Claims;
using System.IO;
using static Google.Apis.Requests.BatchRequest;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessContentAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessContentVideoService businessContentVideoService;

        public BusinessContentAPIController()
        {
            db = new MasterZoneDbContext();
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

        #region Business Videos -----------------------------------------------------------------------

        /// <summary>
        /// Add Business Videos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/AddUpdateVideos")]
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
                RequestBusinessVideo_VM requestBusinessVideo_VM = new RequestBusinessVideo_VM();
                requestBusinessVideo_VM.Id = Convert.ToInt64(HttpRequest.Params["id"]);
                requestBusinessVideo_VM.VideoTitle = HttpRequest.Params["Title"].Trim();
                requestBusinessVideo_VM.VideoLink = HttpRequest.Params["Link"];
                requestBusinessVideo_VM.VideoCategoryId = Convert.ToInt64(HttpRequest.Params["VideoCategoryId"]);
                requestBusinessVideo_VM.Description = HttpRequest.Params["Description"].Trim();
                requestBusinessVideo_VM.Mode = 1;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessVideoThumbnailImageFile = files["Image"];
                requestBusinessVideo_VM.VideoThumbnail = _ManageBusinessVideoThumbnailImageFile; // for validation
                string _ManageBusinessThumbnailImageFileNameGenerated = ""; //will contains generated file name

                // Validate infromation passed
                Error_VM error_VM = requestBusinessVideo_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_ManageBusinessVideoThumbnailImageFile != null)
                    {
                        _ManageBusinessThumbnailImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessVideoThumbnailImageFile);
                    }
                }

                // Add Video
                businessContentVideoService = new BusinessContentVideoService(db);
                var resp = businessContentVideoService.InsertBusinessContentVideo(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessVideos_Params_VM()
                {
                    Id = requestBusinessVideo_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    VideoTitle = requestBusinessVideo_VM.VideoTitle,
                    VideoLink = requestBusinessVideo_VM.VideoLink,
                    Description = requestBusinessVideo_VM.Description,
                    Thumbnail = _ManageBusinessThumbnailImageFileNameGenerated,
                    BusinessContentVideoCategoryId = requestBusinessVideo_VM.VideoCategoryId,
                    SubmittedByLoginId = _LoginId,
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
                    #region Insert-Update Manage Business Image on Server
                    if (files.Count > 0)
                    {
                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_VideoThumbNailImage), _ManageBusinessThumbnailImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessVideoThumbnailImageFile, FileWithPath);
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
        /// Get all list business videos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetAllVideosList")]
        public HttpResponseMessage GetAllVideosList()
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

                List<BusinessVideoResponse_VM> businessVideoResponse_VMs = new List<BusinessVideoResponse_VM>();
                
                // Get All Active Videos
                businessContentVideoService = new BusinessContentVideoService(db);
                businessVideoResponse_VMs = businessContentVideoService.GetAllBusinessContentVideos(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessVideoResponse_VMs;

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
        /// Delete Video
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/VideoDeleteById")]
        public HttpResponseMessage DeleteVideoById(long id)
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

                // Delete Video
                businessContentVideoService = new BusinessContentVideoService(db);
                var resp = businessContentVideoService.DeleteBusinessVideo(id, _BusinessOwnerLoginId);
                
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
        /// Get business Videos  by Business-OwnerLogin-Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessVideosVisitorPanel")]
        public HttpResponseMessage GetBusinessVideosVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessVideoResponse_VM> businessVideoResponse_VMs = new List<BusinessVideoResponse_VM>();
                List<BusinessAllClassesDetail> businessImageResponse_VMs = new List<BusinessAllClassesDetail>();
                
                // Get Videos
                businessContentVideoService = new BusinessContentVideoService(db);
                businessVideoResponse_VMs = businessContentVideoService.GetAllBusinessContentVideos(businessOwnerLoginId);
                businessImageResponse_VMs = businessContentVideoService.GetAllBusinessClassDetails(businessOwnerLoginId);

                var BusiessVideoRes_ = businessVideoResponse_VMs.FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessVideosList = businessVideoResponse_VMs,
                    BusinessClassdetailist = businessImageResponse_VMs,
                    BusinessVideo = BusiessVideoRes_
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
        /// Get Business Videos For Home Page- Randomly [for Mobile app visitor panel]
        /// </summary>  
        /// <returns>list of Videos</returns>
        [HttpGet]
        [Route("api/BusinessContent/BusinessVideos/GetRandom")]
        public HttpResponseMessage GetRandomContentVideosForVisitorPanel()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // Get Random
                businessContentVideoService = new BusinessContentVideoService(db);
                var videos = businessContentVideoService.GetRandomBusinessContentVideos();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = videos;

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
        /// Get all active 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetAllActiveVideoCategoriesList")]
        public HttpResponseMessage GetAllActiveVideoCategoriesList()
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

                List<BusinessContentVideoCategory_VM> businessVideoResponse_VMs = new List<BusinessContentVideoCategory_VM>();

                // Get All Active Categories
                businessContentVideoService = new BusinessContentVideoService(db);
                businessVideoResponse_VMs = businessContentVideoService.GetAllActiveBusinessVideoCategories();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessVideoResponse_VMs;

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
        /// Get all list business videos by Category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetAllVideosListByCategory")]
        public HttpResponseMessage GetAllVideosListByCategory()
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

                List<BusinessVideoResponse_VM> businessVideoResponse_VMs = new List<BusinessVideoResponse_VM>();

                // Get All Active Videos
                businessContentVideoService = new BusinessContentVideoService(db);
                businessVideoResponse_VMs = businessContentVideoService.GetAllBusinessContentVideos(_BusinessOwnerLoginId);

                List<BusinessVideoByCategory_VM> videoCategoryListVM = new List<BusinessVideoByCategory_VM>();

                videoCategoryListVM = businessVideoResponse_VMs.GroupBy(v => v.VideoCategoryId).Select(v => new BusinessVideoByCategory_VM() { VideoCategoryId = v.First().VideoCategoryId, VideoCategoryName = v.First().VideoCategoryName }).ToList();

                foreach (var videoCategory in videoCategoryListVM)
                {
                    videoCategory.Videos = businessVideoResponse_VMs.Where(v => v.VideoCategoryId == videoCategory.VideoCategoryId).ToList();
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = videoCategoryListVM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion ----------------------------------------------------------------------------------------

        #region Manage Business Images ---------------------------------------------------------------------
        /// <summary>
        /// Add Business Images
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/AddUpdateImages")]
        public HttpResponseMessage AddUpdateImages()
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
                RequestBusinessImage_VM requestBusinessImage_VM = new RequestBusinessImage_VM();
                requestBusinessImage_VM.Id = Convert.ToInt64(HttpRequest.Params["id"]);
                requestBusinessImage_VM.ImageTitle = HttpRequest.Params["Title"].Trim();
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
                            new SqlParameter("id",  requestBusinessImage_VM.Id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("imageTitle", requestBusinessImage_VM.ImageTitle),
                            new SqlParameter("image",_ManageBusinessImageFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestBusinessImage_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessImages @id,@userLoginId,@imageTitle,@image ,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ManageBusinessImage), _ManageBusinessImageFileNameGenerated);
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
        /// Get all list business images
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetAllImagesList")]
        public HttpResponseMessage GetAllImagesList()
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

                List<BusinessImageResponse_VM> businessImageResponse_VMs = new List<BusinessImageResponse_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("mode", "1"),
                };
                businessImageResponse_VMs = db.Database.SqlQuery<BusinessImageResponse_VM>("exec sp_ManageBusinessImages @id,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessImageResponse_VMs;

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
        /// Delete image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/ImageDeleteById")]
        public HttpResponseMessage DeleteImageById(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("imageTitle", ""),
                            new SqlParameter("image",""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessImages @id,@userLoginId,@imageTitle,@image ,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
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
        /// Get business Image  by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessImagesVisitorPanel")]
        public HttpResponseMessage GetBusinessImagesVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessImageResponse_VM> businessImageResponse_VMs = new List<BusinessImageResponse_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", businessOwnerLoginId),
                                new SqlParameter("mode", "1"),
                };
                businessImageResponse_VMs = db.Database.SqlQuery<BusinessImageResponse_VM>("exec sp_ManageBusinessImages @id,@userLoginId,@mode", queryParams).ToList();

                // BusinessImageResponse_VM businessImageResponse_VM = new BusinessImageResponse_VM();
                var businessImageResponse_VM = businessImageResponse_VMs.FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessImagesResponse = businessImageResponse_VMs,
                    SingleBusinessImageResponse = businessImageResponse_VM
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
        #endregion ----------------------------------------------------------------------------------------------

        /// <summary>
        /// To Get Video Detail Through LastRecorded Id 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/UserContentVideo/GetAllVideoDetail")]
        public HttpResponseMessage GetAllVideoDetail(long businessOwnerLoginId, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessVideoResponse_VM> businessVideoResponse_VMs = new List<BusinessVideoResponse_VM>();
                // Get All Active Videos
                businessContentVideoService = new BusinessContentVideoService(db);
                businessVideoResponse_VMs = businessContentVideoService.GetAllBusinessDetaillst(businessOwnerLoginId, lastRecordId, recordLimit);

                //List<BusinessVideoByCategory_VM> videoCategoryListVM = new List<BusinessVideoByCategory_VM>();

                //videoCategoryListVM = businessVideoResponse_VMs.GroupBy(v => v.VideoCategoryId).Select(v => new BusinessVideoByCategory_VM() { VideoCategoryId = v.First().VideoCategoryId, VideoCategoryName = v.First().VideoCategoryName }).ToList();

                //foreach (var videoCategory in videoCategoryListVM)
                //{
                //    videoCategory.Videos = businessVideoResponse_VMs.Where(v => v.VideoCategoryId == videoCategory.VideoCategoryId).ToList();
                //}

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessVideoResponse_VMs;

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
        /// To Get Business Images Detail By LastRecordedId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessImagesDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessImagesDetailVisitorPanel(long businessOwnerLoginId, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessImageResponse_VM> businessVideoResponse_VMs = new List<BusinessImageResponse_VM>();
                // Get All Images
                businessContentVideoService = new BusinessContentVideoService(db);

                businessVideoResponse_VMs = businessContentVideoService.GetAllBusinessImagesDetaillst(businessOwnerLoginId, lastRecordId, recordLimit);



                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessVideoResponse_VMs;


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
        /// To Get Business User Image/Video Detail By Pagination (PageSize)
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessUserDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessUserDetailVisitorPanel(long businessOwnerLoginId, int pageSize, int pageNumber)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessResponseUserDetail> businessUserResponse_VMs = new List<BusinessResponseUserDetail>();
                // Get All Images/Videos
                businessContentVideoService = new BusinessContentVideoService(db);

                businessUserResponse_VMs = businessContentVideoService.GetAllBusinessUserDetaillst(businessOwnerLoginId, pageSize, pageNumber);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessUserResponse_VMs;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        
        #region Business-Content-PDF -------------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/AddUpdatePdf")]
        public HttpResponseMessage AddUpdatePdf()
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
                RequestMasterProImage_VM requestBusinessImage_VM = new RequestMasterProImage_VM();
                requestBusinessImage_VM.Id = Convert.ToInt64(HttpRequest.Params["id"]);
                requestBusinessImage_VM.ImageTitle = HttpRequest.Params["Title"].Trim();
                requestBusinessImage_VM.Mode = 1;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessImageFile = files["Image"];
                requestBusinessImage_VM.Image = _ManageBusinessImageFile; // for validation
                string _ManageBusinessImageFileNameGenerated = "";

                HttpFileCollection file = HttpRequest.Files;
                HttpPostedFile _ManageBusinessPdfFile = files["ThumbnailPdf"];
                requestBusinessImage_VM.ThumbnailPdf = _ManageBusinessPdfFile; // for validation
                string _ManageBusinessPdfFileNameGenerated = "";//will contains generated file name


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
                    if (_ManageBusinessImageFile != null || _ManageBusinessPdfFile != null)
                    {
                        _ManageBusinessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessImageFile);
                    }
                    if (_ManageBusinessPdfFile != null)
                    {
                        _ManageBusinessPdfFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessPdfFile);
                    }
                }


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  requestBusinessImage_VM.Id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("imageTitle", requestBusinessImage_VM.ImageTitle),
                            new SqlParameter("image",_ManageBusinessImageFileNameGenerated),
                            new SqlParameter("thumbnailpdf",_ManageBusinessPdfFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestBusinessImage_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateMasterProPdf @id,@userLoginId,@imageTitle,@image ,@thumbnailpdf,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MasterProPdf), _ManageBusinessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessImageFile, FileWithPath);


                    }
                    if (file.Count > 0)
                    {
                        // save new file
                        string FileWithPaths = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MasterProThumbnailPdf), _ManageBusinessPdfFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessPdfFile, FileWithPaths);


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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessContent/GetAllMasterProPdfList")]
        public HttpResponseMessage GetAllMasterProPdfList()
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

                List<MasterProImageResponse_VM> businessImageResponse_VMs = new List<MasterProImageResponse_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("mode", "1"),
                };
                businessImageResponse_VMs = db.Database.SqlQuery<MasterProImageResponse_VM>("exec sp_ManageMasterProPdf @id,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessImageResponse_VMs;

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
        [Route("api/BusinessContent/DeleteMasterProPdfById")]
        public HttpResponseMessage DeleteMasterProPdfById(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("imageTitle", ""),
                            new SqlParameter("image",""),
                            new SqlParameter("thumbnailpdf",""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateMasterProPdf @id,@userLoginId,@imageTitle,@image ,@thumbnailpdf,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
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

        [HttpGet]
        [Route("api/BusinessContent/GetAllMasterProPdfListVisitorPanel")]
        public HttpResponseMessage GetAllMasterProPdfListVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<MasterProImageResponse_VM> businessImageResponse_VMs = new List<MasterProImageResponse_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", businessOwnerLoginId),
                                new SqlParameter("mode", "1"),
                };
                businessImageResponse_VMs = db.Database.SqlQuery<MasterProImageResponse_VM>("exec sp_ManageMasterProPdf @id,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    MasterProPdfResponse = businessImageResponse_VMs,

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

        //--------Get All PDF & Video API ------------------------------------------
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessDetailPdfVideoDetails")]
        public HttpResponseMessage GetBusinessDetailPdfVideoDetails(long businessOwnerLoginId, int pageSize, int pageNumber)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<MasterProResponseUserDetail> businessUserVideoResponse_VMs = new List<MasterProResponseUserDetail>();
                List<MasterProResponseUserDetail> businessUserImageResponse_VMs = new List<MasterProResponseUserDetail>();
                // Get All Images/Videos
                businessContentVideoService = new BusinessContentVideoService(db);

                businessUserVideoResponse_VMs = businessContentVideoService.GetAllVideoListByBusiness(businessOwnerLoginId, pageSize, pageNumber);
                businessUserImageResponse_VMs = businessContentVideoService.GetAllPDFListByBusiness(businessOwnerLoginId, pageSize, pageNumber);

                apiResponse.status = 1;
                apiResponse.message = "success";

                apiResponse.data = new
                {
                    VideoDetail = businessUserVideoResponse_VMs,
                    ImageDetail = businessUserImageResponse_VMs,

                };


                // Send additional pagination data if needed
                // apiResponse.pageNumber = updatedPageNumber;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion --------------------------------------------------------------------------
    }
}