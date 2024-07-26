using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
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
    public class AdvertisementAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private AdvertisementService advertisementService;
        private BusinessOwnerService businessOwnerService;
        public AdvertisementAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            advertisementService = new AdvertisementService(db);
            businessOwnerService = new BusinessOwnerService(db);
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
        /// Check if Logged-in User has API access. 
        /// For Business Owner or Staff, it must be a Prime Member
        /// else By Pass for other users
        /// </summary>
        /// <returns>Api Response</returns>
        private ApiResponse_VM PrimeMemberValidationForBusinessOwner(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;

            string _UserRole = claims.Where(p => p.Type == ClaimTypes.Role).FirstOrDefault()?.Value;

            if (_UserRole == "BusinessAdmin" || _UserRole == "Staff")
            {
                // Check if Business Owner is a prime Member
                int IsBusinessOwnerPrimeMember = businessOwnerService.IsBusinessOwnerPrimeMember(businessOwnerLoginId);
                // if Not a Prime Member then return error response
                if (IsBusinessOwnerPrimeMember == 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "You are not a Prime Member!";
                    return apiResponse;
                }
            }

            // Access granted for API
            apiResponse.status = 1;
            apiResponse.message = "Access Granted!";

            return apiResponse;
        }

        /// <summary>
        /// Delete Advertisement
        /// </summary>
        /// <param name="id">Advertisement Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [Route("api/Advertisement/Delete/{id}")]
        public HttpResponseMessage DeleteAdvertisementById(long id)
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

                #region Prime Member Validation
                // Check if Business Owner is a prime Member
                var apiResponsebusinessOwnerPrimeMember = PrimeMemberValidationForBusinessOwner(_BusinessOwnerLoginId);
                if (apiResponsebusinessOwnerPrimeMember.status <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponsebusinessOwnerPrimeMember);
                }
                #endregion

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                             new SqlParameter("advertisementLink", "0"),
                            new SqlParameter("image", "0"),
                            new SqlParameter("advertisementCategory ","0"),
                            new SqlParameter("imageOrientationType","0"),
                             new SqlParameter("isActive", "0"),
                            new SqlParameter("submittedByLoginId",_BusinessOwnerLoginId),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("createdForLoginId", "0")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateAdvertisement @id,@advertisementLink,@image,@advertisementCategory,@imageOrientationType,@isActive,@submittedByLoginId,@mode,@createdForLoginId", queryParams).FirstOrDefault();
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
        /// Add Or Update Advertisement
        /// </summary>
        /// <returns>Success or Error Message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,BusinessAdmin,Staff,SubAdmin")]
        [Route("api/Advertisement/AddUpdate")]
        public HttpResponseMessage AddUpdateAdvertisement()
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
                long _CreatedForLoginId = _LoginID_Exact;

                #region Prime Member Validation
                // Check if Business Owner is a prime Member
                var apiResponsebusinessOwnerPrimeMember = PrimeMemberValidationForBusinessOwner(_BusinessOwnerLoginId);
                if (apiResponsebusinessOwnerPrimeMember.status <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponsebusinessOwnerPrimeMember);
                }
                #endregion

                #region ----- Get and Set CreatedFor based on Logged-in User Role
                var identity = User.Identity as ClaimsIdentity;
                IEnumerable<Claim> claims = identity.Claims;

                string _UserRole = claims.Where(p => p.Type == ClaimTypes.Role).FirstOrDefault()?.Value;

                if (_UserRole == "BusinessAdmin" || _UserRole == "Staff")
                    _CreatedForLoginId = _BusinessOwnerLoginId;
                else if (_UserRole == "SuperAdmin")
                    _CreatedForLoginId = _LoginID_Exact;
                #endregion

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                AdvertisementProfile_VM advertisementProfile_VM = new AdvertisementProfile_VM();
                advertisementProfile_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                advertisementProfile_VM.AdvertisementLink = HttpRequest.Params["AdvertisementLink"].Trim();
                advertisementProfile_VM.ImageOrientationType = HttpRequest.Params["ImageOrientationType"].Trim();
                if (_UserRole == "SuperAdmin")
                {
                    advertisementProfile_VM.AdvertisementCategory = HttpRequest.Params["AdvertisementCategory"].Trim();
                }
                else if (advertisementProfile_VM.AdvertisementCategory == null)
                {
                    var _businessSubCategoryId = businessOwnerService.GetBusinessOwnerTableDataById(_BusinessOwnerLoginId).BusinessSubCategoryId;
                    advertisementProfile_VM.AdvertisementCategory = _businessSubCategoryId.ToString();
                }

                advertisementProfile_VM.IsActive = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                advertisementProfile_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _advertisementImageFile = files["ProfileImage"]; // change name  
                advertisementProfile_VM.Image = _advertisementImageFile;

                string _advertisementFileNameGenerated = ""; //will contains generated file name
                string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.

                //// Validate infromation passed
                //Error_VM error_VM = advertisementProfile_VM.ValidInformation();

                //if (!error_VM.Valid)

                //{
                //    apiResponse.status = -1;
                //    apiResponse.message = error_VM.Message;
                //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                //}



                if (files.Count > 0)
                {
                    if (_advertisementImageFile != null)
                    {
                        _advertisementFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_advertisementImageFile);
                    }
                }

                if (advertisementProfile_VM.Mode == 2)
                {

                    var respGetAdvertisementData = advertisementService.GetAdvertisementDetail(advertisementProfile_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _advertisementFileNameGenerated = respGetAdvertisementData.Image ?? "";
                    }
                    else
                    {
                        _PreviousProfileImageFileName = respGetAdvertisementData.Image ?? "";
                    }
                }



                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", advertisementProfile_VM.Id),
                            new SqlParameter("image", _advertisementFileNameGenerated),
                            new SqlParameter("advertisementLink", advertisementProfile_VM.AdvertisementLink),
                            new SqlParameter("advertisementCategory ",advertisementProfile_VM.AdvertisementCategory),
                            new SqlParameter("imageOrientationType", advertisementProfile_VM.ImageOrientationType),
                            new SqlParameter("isActive", advertisementProfile_VM.IsActive),
                            new SqlParameter("submittedByLoginId",_LoginID_Exact),
                            new SqlParameter("mode", advertisementProfile_VM.Mode),
                            new SqlParameter("createdForLoginId", _CreatedForLoginId)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateAdvertisement @id,@advertisementLink,@image,@advertisementCategory,@imageOrientationType,@isActive,@submittedByLoginId,@mode,@createdForLoginId", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = resp.responseMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update Advertisement Image.
                #region Insert-Update [Advertisement Image] on Server
                if (files.Count > 0)
                {
                    // if business Profile Image passed then Add-Update Profile Image
                    if (_advertisementImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_Advertisement), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_Advertisement), _advertisementFileNameGenerated);
                        fileHelper.SaveUploadedFile(_advertisementImageFile, fileWithPathImage);
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

        /// <summary>
        /// Get Single Advertisement Detial By Id
        /// </summary>
        /// <param name="id">Advertisement Id</param>
        /// <returns>Single Advertisement or null</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [Route("api/Advertisement/GetSingleAdvertisement/{id}")]
        public HttpResponseMessage GetAdvertisementDetailsById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.UserLoginId;

                #region Prime Member Validation
                // Check if Business Owner is a prime Member
                var apiResponsebusinessOwnerPrimeMember = PrimeMemberValidationForBusinessOwner(_BusinessOwnerLoginId);
                if (apiResponsebusinessOwnerPrimeMember.status <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponsebusinessOwnerPrimeMember);
                }
                #endregion

                Advertisement_VM contactUsAddress_VMs = new Advertisement_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "2")
                            };

                contactUsAddress_VMs = db.Database.SqlQuery<Advertisement_VM>("exec sp_ManageAdvertisement @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = contactUsAddress_VMs;

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
        /// Change Advertisement Status (Active/Inactive)
        /// </summary>
        /// <param name="id">Advertisement Id</param>
        /// <returns>Success or Error Message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [Route("api/Advertisement/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeAdvertisementStatus(int id)
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

                #region Prime Member Validation
                // Check if Business Owner is a prime Member
                var apiResponsebusinessOwnerPrimeMember = PrimeMemberValidationForBusinessOwner(_BusinessOwnerLoginId);
                if (apiResponsebusinessOwnerPrimeMember.status <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponsebusinessOwnerPrimeMember);
                }
                #endregion

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "3")
                            };
                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageAdvertisement @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

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
        /// Get All Advertisements For Business with DataTable-Pagination
        /// </summary>
        /// <returns>Advertisements List for Jquery-DataTable</returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Advertisement/Business/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllAdvertisementsOfBusinessByPagination()
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

                #region Prime Member Validation
                // Check if Business Owner is a prime Member
                var apiResponsebusinessOwnerPrimeMember = PrimeMemberValidationForBusinessOwner(_BusinessOwnerLoginId);
                if (apiResponsebusinessOwnerPrimeMember.status <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponsebusinessOwnerPrimeMember);
                }
                #endregion

                AdvertisementList_Pagination_SQL_Params_VM _Params_VM = new AdvertisementList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 2;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.CreatedForLoginId = _BusinessOwnerLoginId;

                var paginationResponse = advertisementService.GetAdvertisementsList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// Get All Advertisements For SuperAdmin with DataTable-Pagination
        /// </summary>
        /// <returns>Advertisements List for Jquery-DataTable</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Advertisement/SuperAdmin/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllAdvertisementsForSuperAdminByPagination()
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

                AdvertisementList_Pagination_SQL_Params_VM _Params_VM = new AdvertisementList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.CreatedForLoginId = _LoginId;

                var paginationResponse = advertisementService.GetAdvertisementsList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// Get Single Advertisement Detail - Randomly [Created by SuperAdmin/Masterzone]
        /// </summary>
        /// <returns>Single Advertisement or null</returns>
        [HttpGet]
        [Route("api/Advertisement/GetRandomAdvertisement")]
        public HttpResponseMessage GetRandomAdvertisementDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                SingleUserAdvertisement_VM singleUserAdvertisement_VM = new SingleUserAdvertisement_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4")
                            };

                singleUserAdvertisement_VM = db.Database.SqlQuery<SingleUserAdvertisement_VM>("exec sp_ManageAdvertisement @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = singleUserAdvertisement_VM;

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
        [Route("api/BusinessCategory/GetAllAdvertisementSlider")]
        public HttpResponseMessage GetAllAdvertisementSlider()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                List<Advertisement_VM> advertisementDeatils = advertisementService.GetAdvertisementDetailslider();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = advertisementDeatils;

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