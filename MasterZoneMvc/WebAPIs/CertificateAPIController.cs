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
using static MasterZoneMvc.ViewModels.RequestCertificate_VM;

namespace MasterZoneMvc.WebAPIs
{
    public class CertificateAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private CertificateService certificateService;
        public CertificateAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            certificateService = new CertificateService(db);
        }
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        #region Certificate CRUD --------------------------------------------------------------
        /// <summary>
        /// Add/Update Certificate By SuperAdmin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/AddUpdate")]
        public HttpResponseMessage AddUpdateCertificateBySuperAdmin()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestCertificate_VM requestCertificate_VM = new RequestCertificate_VM();
                requestCertificate_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestCertificate_VM.Name = HttpRequest.Params["Name"].Trim();
                requestCertificate_VM.ShortDescription = HttpRequest.Params["ShortDescription"].Trim();
                requestCertificate_VM.ProfilePageTypeId = Convert.ToInt64(HttpRequest.Params["ProfilePageTypeId"]);
                requestCertificate_VM.CertificateTypeKey = HttpRequest.Params["CertificateType"];
                requestCertificate_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestCertificate_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestCertificate_VM.Link = (string.IsNullOrEmpty(HttpRequest.Params["Link"].Trim())) ? "" : HttpRequest.Params["Link"].Trim();

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _CertificateIconFile = files["CertificateIcon"];
                string _CertificateIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousCertificateIconFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestCertificate_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_CertificateIconFile != null)
                    {
                        _CertificateIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CertificateIconFile);
                    }

                }


                if (requestCertificate_VM.Mode == 2)
                {
                    CertificateEdit_VM certificateRecord_VM = certificateService.GetCertificateById(requestCertificate_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        // set current image name
                        _CertificateIconFileNameGenerated = certificateRecord_VM.CertificateIcon;
                    }
                    else
                    {
                        // set previous image name
                        _PreviousCertificateIconFileName = certificateRecord_VM.CertificateIcon;
                    }
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestCertificate_VM.Id),
                            new SqlParameter("name", requestCertificate_VM.Name),
                            new SqlParameter("certificateIcon", _CertificateIconFileNameGenerated),
                            new SqlParameter("shortDescription",requestCertificate_VM.ShortDescription),
                            new SqlParameter("status",requestCertificate_VM.Status),
                            new SqlParameter("profilePageTypeId",requestCertificate_VM.ProfilePageTypeId),
                            new SqlParameter("certificateTypeKey",requestCertificate_VM.CertificateTypeKey),
                            new SqlParameter("submittedByLoginId", _LoginID_Exact),
                            new SqlParameter("mode", requestCertificate_VM.Mode),
                            new SqlParameter("link", requestCertificate_VM.Link)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateCertificate @id,@name,@certificateIcon,@shortDescription,@status,@profilePageTypeId,@certificateTypeKey,@submittedByLoginId,@mode,@link", queryParams).FirstOrDefault();
                
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update Certificate Image.
                #region Insert-Update [Certificate Image] on Server

                if (resp.ret == 1)
                {

                    if (requestCertificate_VM.Mode == 1)
                    {
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_Certificate), _CertificateIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_CertificateIconFile, FileWithPath);
                    }
                    else if (requestCertificate_VM.Mode == 2 && files.Count > 0)
                    {
                        // remove previous file
                        if(!String.IsNullOrEmpty(_PreviousCertificateIconFileName))
                        {
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousCertificateIconFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_Certificate), _CertificateIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_CertificateIconFile, FileWithPath);
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
        /// To View Certificate data by the Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/GetAllCertificateByPagination")]
        public HttpResponseMessage GetAllCertificateDataTablePagination()
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

                Certificate_Pagination_SQL_Params_VM _Params_VM = new Certificate_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = certificateService.GetCertificate_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Certificate  Detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/GetById/{id}")]
        public HttpResponseMessage GetById(long id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Certificate-Record-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("id", id),
                        new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                        new SqlParameter("userLoginId", _LoginID_Exact),
                        new SqlParameter("searchtext", "0"),
                        new SqlParameter("mode", "1")
                        };

                var resp = db.Database.SqlQuery<CertificateEdit_VM>("exec sp_ManageCertificate @id,@businessOwnerLoginId,@userLoginId,@searchtext,@mode", queryParams).FirstOrDefault();

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
        /// To delete Certificate Record{Soft delete}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/DeleteById/{id}")]
        public HttpResponseMessage DeleteCertificateById(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",id),
                            new SqlParameter("name", ""),
                            new SqlParameter("certificateIcon", ""),
                            new SqlParameter("shortDescription",""),
                            new SqlParameter("status","0"),
                            new SqlParameter("profilePageTypeId","0"),
                            new SqlParameter("certificateTypeKey",""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("link", "")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateCertificate @id,@name,@certificateIcon,@shortDescription,@status,@profilePageTypeId,@certificateTypeKey,@submittedByLoginId,@mode,@link", queryParams).FirstOrDefault();

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
        /// Certificate Record To Change status by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeCertificateStatus(int id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                             new SqlParameter("businessOwnerLoginId", ""),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("searchtext", "0"),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageCertificate @id,@businessOwnerLoginId,@userLoginId,@searchtext,@mode", queryParams).FirstOrDefault();

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
        /// To change the Certificate Home Visibility Status
        /// </summary>
        /// <param name="id">Certificate-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/ToggleHomePageVisibilityStatus/{id}")]
        public HttpResponseMessage ToggleHomePageVisibilityStatus(long id)
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
                    apiResponse.data = new { };

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",id),
                            new SqlParameter("name", ""),
                            new SqlParameter("certificateIcon", ""),
                            new SqlParameter("shortDescription",""),
                            new SqlParameter("status","0"),
                            new SqlParameter("profilePageTypeId","0"),
                            new SqlParameter("certificateTypeKey",""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "4"),
                            new SqlParameter("link", "")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateCertificate @id,@name,@certificateIcon,@shortDescription,@status,@profilePageTypeId,@certificateTypeKey,@submittedByLoginId,@mode,@link", queryParams).FirstOrDefault();

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
        /// Get All Certificates For Dropdown
        /// </summary>
        /// <returns>Certificate List</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/GetAll")]
        public HttpResponseMessage GetAllCertificationsListForDropdown()
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

                // Get Certificate-Records
                SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("businessOwnerLoginId", "0"),
                        new SqlParameter("userLoginId", "0"),
                        new SqlParameter("searchtext", "0"),
                        new SqlParameter("mode", "3")
                        };

                var resp = db.Database.SqlQuery<CertificateListForDropdown_VM>("exec sp_ManageCertificate @id,@businessOwnerLoginId,@userLoginId,@searchtext,@mode", queryParams).ToList();

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
        /// Get All Certificates For Home Page display
        /// </summary>
        /// <returns>Certificate List</returns>
        [HttpGet]
        [Route("api/Certificate/GetAllCertificatesForHomePage")]
        public HttpResponseMessage GetAllCertificatesForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // Get Certificate-Records For Home Page (All active)
                SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("userLoginId", "0"),
                        new SqlParameter("businessOwnerLoginId", "0"),
                        new SqlParameter("searchtext", "0"),

                        new SqlParameter("mode", "9")
                        };

                var resp = db.Database.SqlQuery<CertificateListForHomePage_VM>("exec sp_ManageCertificate @id,@userLoginId,@businessOwnerLoginId,@searchtext,@mode", queryParams).ToList();

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
        #endregion ---------------------------------------------------------------------------

        /// <summary>
        /// Get All Active Assigned-Business-Certificates List by Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>List of active certifications for business</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Certificate/GetAllActiveBusinesCertificateList")]
        public HttpResponseMessage GetAllActiveBusinesCertificateList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var resp = certificateService.GetAllActiveBusinessCertificationsById(_BusinessOwnerLoginId);

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
        /// Get All Business-Owners List Assigned to the Certification Profile
        /// </summary>
        /// <param name="certificateId">Certificate-Id</param>
        /// <returns>Assigned List of Business-Owners</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Certificate/GetAllAssignedBusinessOwnerList")]
        public HttpResponseMessage GetAllAssignedBusinessOwnerList(long certificateId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (certificateId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvaildRequestDataPassed;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                
                var resp = certificateService.GetAllAssignedBusinessOwnersToCertificationById(certificateId);

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
        /// To Get The Certificate Detail By BusinessOwnerLoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]

        [Route("api/Certificate/GetAllBusinesCertificateList")]
        public HttpResponseMessage GetAllBusinesCertificateList(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                var resp = certificateService.GetAllActiveBusinessCertificationsById(businessOwnerLoginId);

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
        /// Verify User-Certificate by Certificate-Number by Business-Owner-Login-Id
        /// </summary>
        /// <param name="certificateNumber">Certificate-Number</param>
        /// <returns>Certificate Detail if exits else null</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Certificate/VerifyUserCertificate")]
        public HttpResponseMessage VerifyUserCertificate(string certificateNumber)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                if(string.IsNullOrEmpty(certificateNumber))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.CertificateNumberRequired;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var resp = certificateService.GetUserCertificateByCertificateNumber(certificateNumber);

                if(resp  == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Certificate Not Found!";
                    apiResponse.data = resp;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
                }

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