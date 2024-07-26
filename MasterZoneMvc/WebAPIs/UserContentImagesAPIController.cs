using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
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
    public class UserContentImagesAPIController : ApiController
    {

            private MasterZoneDbContext db;
            private FileHelper fileHelper;
            private StoredProcedureRepository storedProcedureRepository;
            private UserContentImageService userContentImageService;

            public UserContentImagesAPIController()
            {
                db = new MasterZoneDbContext();
                fileHelper = new FileHelper();
                storedProcedureRepository = new StoredProcedureRepository(db);
              userContentImageService = new UserContentImageService(db);

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


        #region User Image ------------------------------------

        /// <summary>
        /// Add User Images
        /// </summary>
        /// <returns>If Status 1 to add the Image, then -1 then occur error</returns>
        [HttpPost]
            [Authorize(Roles = "Student")]
            [Route("api/UserContentImages/AddUpdateImages")]
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
                UserContentImages_VM requestUserImage_VM = new UserContentImages_VM();
                requestUserImage_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestUserImage_VM.ImageTitle = HttpRequest.Params["ImageTitle"].Trim();
                requestUserImage_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                    // Get Attatched Files
                    HttpFileCollection files = HttpRequest.Files;
                    HttpPostedFile _ManageUserImageThumbnailImageFile = files["Image"];
                requestUserImage_VM.ImageThumbnail = _ManageUserImageThumbnailImageFile; // for validation
                    string _ManageUserThumbnailImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousManageUserThumbnailImageFileName = "";

                // Validate infromation passed
                Error_VM error_VM = requestUserImage_VM.ValidInformation();

                    if (!error_VM.Valid)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }


                    if (files.Count > 0)
                    {
                        if (_ManageUserImageThumbnailImageFile != null)
                        {
                            _ManageUserThumbnailImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageUserImageThumbnailImageFile);
                        }
                   
                    }
                //Edit Mode 
                if(requestUserImage_VM.Mode == 2)
                {

                UserContentImagesDetail_VM usercontentimagedetailresponse = new UserContentImagesDetail_VM();


                usercontentimagedetailresponse = userContentImageService.GetUserContentImageDetail(requestUserImage_VM.Id);
                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _ManageUserThumbnailImageFileNameGenerated = usercontentimagedetailresponse.Image;
                    }
                    else
                    {
                        _PreviousManageUserThumbnailImageFileName = usercontentimagedetailresponse.Image;
                    }

                }


                var resp = storedProcedureRepository.SP_InsertUpdateUserContentImage<SPResponseViewModel>(new SP_InsertUpdateUserContentImage_Params_VM
                    {

                        Id = requestUserImage_VM.Id,
                        UserLoginId = _LoginId,
                        ImageTitle = requestUserImage_VM.ImageTitle,
                        ImageThumbnail = _ManageUserThumbnailImageFileNameGenerated,
                        Mode = requestUserImage_VM.Mode


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
                        if (!String.IsNullOrEmpty(_PreviousManageUserThumbnailImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_UserImageThumbNailImage), _PreviousManageUserThumbnailImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }
                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_UserImageThumbNailImage), _ManageUserThumbnailImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_ManageUserImageThumbnailImageFile, FileWithPath);
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
            [Authorize(Roles = "Student")]
            [Route("api/UserContentImages/GetAllUserContentImageDetail")]
            public HttpResponseMessage GetAllUserContentImageList()
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

                    List<UserContentImagesDetail_VM> usercontentimagedetailresponse = userContentImageService.GetAllUserContentImageDetail(_LoginId);

                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = new
                    {
                        UserContentImageDetailList = usercontentimagedetailresponse,
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

        #region To Get  User Image Detail By Id-----------------------------------
        /// <summary>
        /// To Get All User Image
        /// </summary>
        /// <returns>To Get  All  Inserted User Image Detail </returns>

        [HttpGet]
            [Authorize(Roles = "Student")]
            [Route("api/UserContentImages/GetUserContentImageDetailById")]
            public HttpResponseMessage GetAllUserContentImageList(long Id)
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

                UserContentImagesDetail_VM usercontentimagedetailresponse = new UserContentImagesDetail_VM();


                usercontentimagedetailresponse = userContentImageService.GetUserContentImageDetail(Id);

                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = usercontentimagedetailresponse;

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
            /// To Delte the User Image 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [HttpPost]
            [Authorize(Roles = "Student")]
            [Route("api/UserContentImages/DeleteUserContentImageDetail")]
            public HttpResponseMessage UsercontentImageDelete(long Id)
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
                    resp = userContentImageService.DeleteUserContentImage(Id);


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
        }
    }