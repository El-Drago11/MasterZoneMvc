using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class SubAdminAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private SubAdminService subService;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;

        public SubAdminAPIController()
        {
            db = new MasterZoneDbContext();
            subService = new SubAdminService(db);
            fileHelper = new FileHelper();
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
        /// Add or Upadate SubAdmin -SuperAdmin Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SubAdmin/AddUpdate")]
        public HttpResponseMessage AddUpdateSubAdmin()
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
                SubAdmin_ViewModel subAdmin_VM = new SubAdmin_ViewModel();
                subAdmin_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                subAdmin_VM.FirstName = HttpRequest.Params["FirstName"].Trim();
                subAdmin_VM.LastName = HttpRequest.Params["LastName"].Trim();
                subAdmin_VM.Email = HttpRequest.Params["Email"].Trim();
                subAdmin_VM.Password = HttpRequest.Params["Password"].Trim();
                subAdmin_VM.Address = HttpRequest.Params["Address"];
                subAdmin_VM.City = HttpRequest.Params["City"];
                subAdmin_VM.Country = HttpRequest.Params["Country"];
                subAdmin_VM.State = HttpRequest.Params["State"];
                subAdmin_VM.LandMark = HttpRequest.Params["LandMark"];
                subAdmin_VM.Pincode = HttpRequest.Params["Pincode"];
                subAdmin_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                subAdmin_VM.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLatitude"]) : 0;
                subAdmin_VM.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLongitude"]) : 0;
                subAdmin_VM.PermissionIds = HttpRequest.Params["PermissionIds"];

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _SubAdminImageFile = files["ProfileImage"];
                string _SubAdminImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousSubAdminImageFileName = ""; // will be used to delete file while updating.



                // Validate infromation passed
                Error_VM error_VM = subAdmin_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {

                    // Validate Uploded Image File
                    bool isValidImage = true;
                    string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                    if (!validImageTypes.Contains(_SubAdminImageFile.ContentType))
                    {
                        isValidImage = false;
                        apiResponse.message = Resources.SuperAdminPanel.ValidImageFile_ErrorMessage;
                    }
                    else if (_SubAdminImageFile.ContentLength > 1024 * 1024) // 1 MB
                    {
                        isValidImage = false;
                        apiResponse.message = String.Format(Resources.SuperAdminPanel.ValidFileSize_ErrorMessage, "1 MB");
                    }

                    if (!isValidImage)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    if (_SubAdminImageFile != null)
                    {
                        _SubAdminImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_SubAdminImageFile);
                    }
                }

                if (subAdmin_VM.Mode == 2)
                {
                    SubAdminEdit_VM businessSubAdmin = subService.GetSubAdminDetailById(subAdmin_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _SubAdminImageFileNameGenerated = businessSubAdmin.ProfileImage;
                    }
                    else
                    {
                        _PreviousSubAdminImageFileName = businessSubAdmin.ProfileImage;
                    }
                }



                SqlParameter[] queryParams = new SqlParameter[]
                   {
                        new SqlParameter("id", subAdmin_VM.Id),
                        new SqlParameter("userLoginId", _LoginId),
                        new SqlParameter("firstName", subAdmin_VM.FirstName),
                        new SqlParameter("lastName", subAdmin_VM.LastName),
                        new SqlParameter("email", subAdmin_VM.Email),
                        new SqlParameter("password", EDClass.Encrypt(subAdmin_VM.Password)),
                        new SqlParameter("country",  subAdmin_VM.Country),
                        new SqlParameter("state",subAdmin_VM.State),
                        new SqlParameter("city",subAdmin_VM.City),
                        new SqlParameter("address",subAdmin_VM.Address),
                        new SqlParameter("profileImage", _SubAdminImageFileNameGenerated),
                        new SqlParameter("pinCode",subAdmin_VM.Pincode),
                        new SqlParameter("landMark",subAdmin_VM.LandMark),
                        new SqlParameter("locationLatitude",subAdmin_VM.Latitude),
                        new SqlParameter("locationLongitude",subAdmin_VM.Longitude),
                        new SqlParameter("submittedByLoginId", "1"),
                        new SqlParameter("mode", subAdmin_VM.Mode)
                };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateSubAdmin @id,@userLoginId,@firstName,@lastName,@email,@password,@country,@state,@city,@address,@profileImage,@pinCode,@landMark,@locationLatitude,@locationLongitude,@submittedbyLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                if (resp.ret == 1)
                {
                    // Update User-Permissions
                    // Insert-Update User Permissions
                    SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                            new SqlParameter("userLoginId",resp.Id),
                            new SqlParameter("permissionIds", subAdmin_VM.PermissionIds),
                            new SqlParameter("mode", "1")
                            };

                    var respPermissions = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateUserPermissions @userLoginId,@permissionIds,@mode", queryParamsPermissions).FirstOrDefault();

                    if (respPermissions.ret <= 0)
                    {
                        apiResponse.status = respPermissions.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        apiResponse.data = new { };
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }
                // Update Category Image.
                if (resp.ret == 1)
                {

                    if (subAdmin_VM.Mode == 1)
                    {
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubAdminImage), _SubAdminImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_SubAdminImageFile, FileWithPath);
                    }
                    else if (subAdmin_VM.Mode == 2 && files.Count > 0)
                    {
                        // remove previous file
                        string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousSubAdminImageFileName);
                        fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubAdminImage), _SubAdminImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_SubAdminImageFile, FileWithPath);
                    }
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
                apiResponse.message = "Internal Server Error!";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// Get SubAdmin Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SubAdmin/GetById")]
        public HttpResponseMessage GetSubAdminById(long id)
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
                

                // Get Class By Id

                SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<SubAdminEdit_VM>("exec sp_ManageSubAdmin @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();

                SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                     new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId",resp.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                resp.Permissions = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManageUserPermissions @id, @userLoginId,@mode", queryParamsPermissions).ToList();
              
                resp.Password = EDClass.Decrypt(resp.Password);

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }

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
        /// Get All SubAdmin with Pagination For the SuperAdmin-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SubAdmin/GetAllByPagination")]

        public HttpResponseMessage GetAllSubAdminDataTablePagination()
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

                SubAdmin_Pagination_SQL_Params_VM _Params_VM = new SubAdmin_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = subService.GetSubAdminList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Delete the SubAdmin Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/SubAdmin/SubAdminDeleteById/{id}")]
        public HttpResponseMessage DeleteSubAdminById(long id)
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
                long _UserLoginId = validateResponse.BusinessAdminLoginId;


                SqlParameter[] queryParams = new SqlParameter[]
                  {
                        new SqlParameter("id",id),
                        new SqlParameter("userLoginId", _LoginId),
                        new SqlParameter("firstName", ""),
                        new SqlParameter("lastName", ""),
                        new SqlParameter("email", ""),
                        new SqlParameter("password", ""),
                        new SqlParameter("country",  ""),
                        new SqlParameter("state",""),
                        new SqlParameter("city",""),
                        new SqlParameter("address",""),
                         new SqlParameter("profileImage", ""),
                        new SqlParameter("pinCode",""),
                        new SqlParameter("landMark",""),
                        new SqlParameter("locationLatitude","0"),
                        new SqlParameter("locationLongitude","0"),
                        new SqlParameter("submittedByLoginId", "1"),
                        new SqlParameter("mode",3)
               };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateSubAdmin @id,@userLoginId,@firstName,@lastName,@email,@password,@country,@state,@city,@address,@profileImage,@pinCode,@landMark,@locationLatitude,@locationLongitude,@submittedbyLoginId,@mode", queryParams).FirstOrDefault();

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
        /// Get SubAdmin Detail By UserloginId 
        /// </summary>
        /// <param name="UserloginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SubAdmin")]
        [Route("api/SubAdmin/GetSubAdminDetail")]
        public HttpResponseMessage GetSubAdminDetailById()
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


                // Get SuperAdmin By Id

                SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", ""),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<SubAdminDetailByUser_VM>("exec sp_ManageSubAdmin @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }

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
        /// To Update SubAdmin- Basic Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SubAdmin")]
        [Route("api/SubAdmin/SubAdminProfile/AddUpdate")]
        public HttpResponseMessage AddUpdateSubAdminDetail()
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

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                SubAdminDetailViewModel subAdminDetailViewModel = new SubAdminDetailViewModel();
                subAdminDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                subAdminDetailViewModel.Email = HttpRequest.Params["Email"].Trim();
                subAdminDetailViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                subAdminDetailViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                subAdminDetailViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                subAdminDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Validate infromation passed
                Error_VM error_VM = subAdminDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SubAdminProfileDetail_VM respProfileData = new SubAdminProfileDetail_VM();
                respProfileData = subService.GetSubAdminDetail(_LoginID_Exact);


                var resp = storedProcedureRepository.SP_InsertUpdateSubAdminProfile_Get<SPResponseViewModel>(new SP_InsertUpdateSubAdminProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    FirstName = subAdminDetailViewModel.FirstName,
                    LastName = subAdminDetailViewModel.LastName,
                    Email = subAdminDetailViewModel.Email,
                    PhoneNumber = subAdminDetailViewModel.PhoneNumber,
                    Mode = subAdminDetailViewModel.Mode

                });

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
        ///  To Get SubAdmin Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SubAdmin")]
        [Route("api/SubAdmin/Profile/Get")]
        public HttpResponseMessage GetSubAdminProfileData()
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

                SubAdminProfileDetail_VM respProfileData = new SubAdminProfileDetail_VM();
                respProfileData = subService.GetSubAdminDetail(_LoginID_Exact);



                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respProfileData;

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
        /// Add-Update SubAdmin profile-image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SubAdmin")]
        [Route("api/SubAdmin/Profile/AddUpdateProfileImage")]
        public HttpResponseMessage AddUpdateSubAdminProfileImage()
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

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                SubAdminDetailViewModel subAdminDetailViewModel = new SubAdminDetailViewModel();
                subAdminDetailViewModel.Mode = 2;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _subAdminImageFile = files["ProfileImage"]; // change name
                subAdminDetailViewModel.ProfileImage = _subAdminImageFile;


                string _subAdminImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = subAdminDetailViewModel.VaildInformationProfileImage();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SubAdminProfileDetail_VM respProfileData = new SubAdminProfileDetail_VM();
                respProfileData = subService.GetSubAdminDetail(_LoginID_Exact);



                if (_subAdminImageFile != null && files.Count > 0)
                {
                    _subAdminImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_subAdminImageFile);

                    _PreviousProfileImageFileName = respProfileData.ProfileImage;
                }
                else
                {
                    _subAdminImageFileNameGenerated = respProfileData.ProfileImage;
                }

                var resp = storedProcedureRepository.SP_InsertUpdateSubAdminProfile_Get<SPResponseViewModel>(new SP_InsertUpdateSubAdminProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    ProfileImage = _subAdminImageFileNameGenerated,
                    Mode = subAdminDetailViewModel.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update subadmin Image.
                #region Insert-Update [SubAdmin Profile Image] on Server
                if (files.Count > 0)
                {
                    // if subAdmin Profile Image passed then Add-Update Profile Image
                    if (_subAdminImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubAdminImage), _subAdminImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_subAdminImageFile, fileWithPathImage);
                    }


                }
                #endregion

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

    }
}