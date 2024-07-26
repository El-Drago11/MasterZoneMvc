using Google.Apis.Auth.OAuth2;
using GoogleAuthentication.Services;
using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Google.Apis.Requests.BatchRequest;

namespace MasterZoneMvc.WebAPIs
{
    public class UserContentVideoAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;
        private UserContentVedioService userContentVedioService;

        public UserContentVideoAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            storedProcedureRepository = new StoredProcedureRepository(db);
            userContentVedioService = new UserContentVedioService(db);

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


        #region User Vedios ------------------------------------

        /// <summary>
        /// Add User Videos
        /// </summary>
        /// <returns>If Status 1 to add the vedio, then -1 then occur error</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/UserContentVideo/AddUpdateVideos")]
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
                RequestUserVedio_VM requestUserVideo_VM = new RequestUserVedio_VM();
                requestUserVideo_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestUserVideo_VM.VideoTitle = HttpRequest.Params["VideoTitle"].Trim();
                requestUserVideo_VM.VideoLink = HttpRequest.Params["VideoLink"];
                requestUserVideo_VM.VideoDescription = HttpRequest.Params["VideoDescription"].Trim();
                requestUserVideo_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageUserVideoThumbnailImageFile = files["Image"];
                requestUserVideo_VM.VideoThumbnail = _ManageUserVideoThumbnailImageFile; // for validation
                string _ManageUserThumbnailImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousManageUserThumbnailUserFileName = "";

                // Validate infromation passed
                Error_VM error_VM = requestUserVideo_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_ManageUserVideoThumbnailImageFile != null)
                    {
                        _ManageUserThumbnailImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageUserVideoThumbnailImageFile);
                    }
                }

                if (requestUserVideo_VM.Mode == 2)
                {
                    UserContentVideo_VM usercontentvediodetailresponse = userContentVedioService.GetUserContentVedioDetail(requestUserVideo_VM.Id);


                    // if files are not present or files count is 0
                    if (files.Count <= 0)
                    {
                        _ManageUserThumbnailImageFileNameGenerated = usercontentvediodetailresponse.VideoThumbnail;
                    }
                    else
                    {


                        // Also, you may want to store the previous thumbnail file name if you're going to delete it later
                        _PreviousManageUserThumbnailUserFileName = usercontentvediodetailresponse.VideoThumbnail;
                    }
                }



                var resp = storedProcedureRepository.SP_InsertUpdateUserContentVideos<SPResponseViewModel>(new SP_InsertUpdateUserContentVideo_Params_VM
                {

                    Id = requestUserVideo_VM.Id,
                    UserLoginId = _LoginId,
                    VideoTitle = requestUserVideo_VM.VideoTitle,
                    VideoDescription = requestUserVideo_VM.VideoDescription,
                    VideoLink = requestUserVideo_VM.VideoLink,
                    VideoThumbnail = _ManageUserThumbnailImageFileNameGenerated,
                    Mode = requestUserVideo_VM.Mode


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
                        if (!String.IsNullOrEmpty(_PreviousManageUserThumbnailUserFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_UserVideoThumbNailImage), _PreviousManageUserThumbnailUserFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }
                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_UserVideoThumbNailImage), _ManageUserThumbnailImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageUserVideoThumbnailImageFile, FileWithPath);
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
        #endregion

        #region To Get All User Video-----------------------------------
        /// <summary>
        /// To Get All User Video
        /// </summary>
        /// <returns>To Get  All  Inserted User Vedio Detail </returns>

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/UserContentVideo/GetAllUserContentVideoDetail")]
        public HttpResponseMessage GetAllUserContentVideoList()
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

                List<UserContentVideo_VM> usercontentvediodetailresponse = userContentVedioService.GetAllUserContentVedioDetail(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    UserContentVedioDetailList = usercontentvediodetailresponse,
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
        #endregion

        #region To Get  User Video Detail By Id-----------------------------------
        /// <summary>
        /// To Get All User Video
        /// </summary>
        /// <returns>To Get  All  Inserted User Vedio Detail </returns>

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/UserContentVideo/GetUserContentVideoDetailById")]
        public HttpResponseMessage GetAllUserContentVideoList(long Id)
        {

            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (Id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                UserContentVideo_VM usercontentvediodetailresponse = new UserContentVideo_VM();


                usercontentvediodetailresponse = userContentVedioService.GetUserContentVedioDetail(Id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = usercontentvediodetailresponse;

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


        /// <summary>
        /// To Delte the User Vedio 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/UserContentVideo/DeleteUserContentVideoDetail")]
        public HttpResponseMessage UsercontentVideoDelete(long Id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (Id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Content Video 
                resp = userContentVedioService.DeleteUserContentVedio(Id);


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
        ///  Get User Video for View in Video gallery in user panel 
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>

        [Route("api/UserContentVideo/GetVideoListUser")]
        public HttpResponseMessage GetVideoListUser(long lastRecordId, int recordLimit = 3)
        {


            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<UserContentVideo_VM> lst = new List<UserContentVideo_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("lastRecordId", lastRecordId),
                            new SqlParameter("recordLimit", recordLimit),
                            new SqlParameter("searchtitle", ""),
                            new SqlParameter("userLoginId","0" ),
                            new SqlParameter("mode", "3"),
                    };

                lst = db.Database.SqlQuery<UserContentVideo_VM>("exec sp_ManageUserContentVideos @id,@lastRecordId,@recordLimit,@searchtitle,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    UserVideoDetailList = lst,
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
        /// search by Title of Video List
        /// </summary>
        /// <param name="searchtitle"></param>
        /// <returns></returns>

        [Route("api/UserContentVideo/GetVideoListUserByTitle")]
        public HttpResponseMessage GetVideoListUserByTitle(string  searchtitle)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            
            if (searchtitle == null)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.NoDataFound;
                return Request.CreateResponse(apiResponse);
            }

            try
            {
                List<UserContentVideo_VM> lst = new List<UserContentVideo_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("searchtitle", searchtitle),
                            new SqlParameter("userLoginId","0" ),
                            new SqlParameter("mode", "4"),
                };

                lst = db.Database.SqlQuery<UserContentVideo_VM>("exec sp_ManageUserContentVideos @id,@lastRecordId,@recordLimit,@searchtitle,@userLoginId,@mode", queryParams).ToList();

                //if (lst == null || lst.Count == 0)
                //{
                //    apiResponse.status = -500;
                //    apiResponse.message = Resources.VisitorPanel.NoRecordFound;
                //    return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);

                //}

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    UserVideoDetailList = lst,
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