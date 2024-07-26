using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using MasterZoneMvc.Common.Helpers;
using static MasterZoneMvc.ViewModels.RequestCertificate_VM;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.Windows.Diagnostics;
using Newtonsoft.Json;
using MasterZoneMvc.Models.Enum;
using Microsoft.IdentityModel.Tokens;
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.WebAPIs
{
    public class LicenseAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private LicenseService licenseService;
        private ItemFeatureService itemFeatureService;
        private CertificateService certificateService;

        public LicenseAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            licenseService = new LicenseService(db);
            itemFeatureService = new ItemFeatureService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Remove Prevoius file if not empty and upload new image on server
        /// </summary>
        /// <param name="fileServerPath">File Path/directory on Server</param>
        /// <param name="previousFileName">Previous File Name to match in Server Path</param>
        /// <param name="newFileName">New File Name to save</param>
        /// <param name="file">File to save</param>
        private void InsertOrDeleteFileFromServer(string fileServerPath, string previousFileName, string newFileName, HttpPostedFile file)
        {
            // if no new file is passed then return
            if (file == null)
            {
                return;
            }

            // remove previous file
            if (!String.IsNullOrEmpty(previousFileName))
            {
                string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(fileServerPath), previousFileName);
                fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
            }

            // save new file
            string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(fileServerPath), newFileName);
            fileHelper.SaveUploadedFile(file, FileWithPath);
        }

        #region License CRUD APIs For SuperAdmin -------------------------------------------------------------------------------
        /// <summary>
        /// Add/Update License By Super/Sub Admin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/AddUpdate")]
        public HttpResponseMessage AddUpdateLicenseBySuperAdmin()
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
                RequestLicense_VM requestLicense_VM = new RequestLicense_VM();
                requestLicense_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestLicense_VM.CertificateId = Convert.ToInt64(HttpRequest.Params["CertificateId"]);
                requestLicense_VM.Title = HttpRequest.Params["Title"].Trim();
                requestLicense_VM.Description = HttpRequest.Params["Description"].Trim();
                requestLicense_VM.CommissionType = HttpRequest.Params["CommissionType"].Trim();
                //requestCertificate_VM.AdditionalInformation = HttpUtility.UrlDecode(HttpRequest.Form.Get("AdditionalInformation"));
                requestLicense_VM.CommissionValue = Convert.ToDecimal(HttpRequest.Params["CommissionValue"]);
                requestLicense_VM.BusinessPanelPermissions = HttpRequest.Params["BusinessPanelPermissions"];
                requestLicense_VM.FeaturePermissions = (!string.IsNullOrEmpty(HttpRequest.Params["FeaturePermissions"])) ? JsonConvert.DeserializeObject<List<RequestItemFeaturePermission_VM>>(HttpRequest.Params["FeaturePermissions"]) : new List<RequestItemFeaturePermission_VM>();
                requestLicense_VM.IsPaid = Convert.ToInt32(HttpRequest.Params["IsPaid"]);
                requestLicense_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestLicense_VM.AchievingOrder = Convert.ToInt32(HttpRequest.Params["AchievingOrder"]);
                requestLicense_VM.LicenseDuration = Convert.ToInt32(HttpRequest.Params["LicenseDuration"]);
                requestLicense_VM.TimePeriod = HttpRequest.Params["TimePeriod"].Trim();
                requestLicense_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestLicense_VM.Price = (string.IsNullOrEmpty(HttpRequest.Params["Price"].Trim())) ? 0 : Convert.ToDecimal(HttpRequest.Params["Price"]);
                requestLicense_VM.MinSellingPrice = Convert.ToDecimal(HttpRequest.Params["MinSellingPrice"]);
                requestLicense_VM.GSTPercent = (string.IsNullOrEmpty(HttpRequest.Params["GSTPercent"].Trim())) ? 0 : Convert.ToDecimal(HttpRequest.Params["GSTPercent"]);
                requestLicense_VM.GSTDescription = HttpRequest.Params["GSTDescription"].Trim();
                requestLicense_VM.LicenseCertificateHTMLContent = HttpUtility.UrlDecode(HttpRequest.Params["LicenseCertificateHTMLContent"]);
                requestLicense_VM.IsLicenseToTeach = Convert.ToInt32(HttpRequest.Params["IsLicenseToTeach"]);
                requestLicense_VM.LicenseToTeach_Type = HttpRequest.Params["LicenseToTeach_Type"].Trim();
                requestLicense_VM.LicenseToTeach_DisplayName = HttpRequest.Params["LicenseToTeach_DisplayName"].Trim();
                requestLicense_VM.MasterPro = Convert.ToInt32(HttpRequest.Params["MasterPro"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _CertificateFile = files["CertificateImage"];
                string _CertificateFileNameGenerated = ""; //will contains generated file name
                string _PreviousCertificateFileName = ""; // will be used to delete file while updating.
                requestLicense_VM.CertificateImage = _CertificateFile;

                HttpPostedFile _SignatureFile = files["SignatureImage"];
                string _SignatureFileNameGenerated = ""; //will contains generated file name
                string _PreviousSignatureFileName = ""; // will be used to delete file while updating.
                requestLicense_VM.SignatureImage = _SignatureFile;

                HttpPostedFile _Signature2File = files["Signature2Image"];
                string _Signature2FileNameGenerated = ""; //will contains generated file name
                string _PreviousSignature2FileName = ""; // will be used to delete file while updating.
                requestLicense_VM.Signature2Image = _Signature2File;

                HttpPostedFile _Signature3File = files["Signature3Image"];
                string _Signature3FileNameGenerated = ""; //will contains generated file name
                string _PreviousSignature3FileName = ""; // will be used to delete file while updating.
                requestLicense_VM.Signature3Image = _Signature3File;

                HttpPostedFile _LicenseLogoFile = files["LicenseLogoImage"];
                string _LicenseLogoFileNameGenerated = ""; //will contains generated file name
                string _PreviousLicenseLogoFileName = ""; // will be used to delete file while updating.
                requestLicense_VM.LicenseLogoImage = _LicenseLogoFile;

                // Validate infromation passed
                Error_VM error_VM = requestLicense_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_CertificateFile != null)
                    {
                        _CertificateFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CertificateFile);
                    }
                    if (_SignatureFile != null)
                    {
                        _SignatureFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_SignatureFile);
                    }
                    if (_Signature2File != null)
                    {
                        _Signature2FileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_Signature2File);
                    }
                    if (_Signature3File != null)
                    {
                        _Signature3FileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_Signature3File);
                    }
                    if (_LicenseLogoFile != null)
                    {
                        _LicenseLogoFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_LicenseLogoFile);
                    }
                }


                if (requestLicense_VM.Mode == 2)
                {
                    LicenseEdit_VM certificateRecord_VM = licenseService.GetLicenseById(requestLicense_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        // set current image name
                        _CertificateFileNameGenerated = certificateRecord_VM.CertificateImage;
                        _LicenseLogoFileNameGenerated = certificateRecord_VM.LicenseLogo;
                        _SignatureFileNameGenerated = certificateRecord_VM.SignatureImage;
                        _Signature2FileNameGenerated = certificateRecord_VM.Signature2Image;
                        _Signature3FileNameGenerated = certificateRecord_VM.Signature3Image;
                    }
                    else
                    {
                        // set previous image name for removal from server 
                        if (_CertificateFile != null)
                            _PreviousCertificateFileName = certificateRecord_VM.CertificateImage;
                        else
                            _CertificateFileNameGenerated = certificateRecord_VM.CertificateImage;

                        if(_SignatureFile != null)
                            _PreviousSignatureFileName = certificateRecord_VM.SignatureImage;
                        else
                            _SignatureFileNameGenerated= certificateRecord_VM.SignatureImage;

                        if (_Signature2File != null)
                            _PreviousSignature2FileName = certificateRecord_VM.Signature2Image;
                        else
                            _Signature2FileNameGenerated = certificateRecord_VM.Signature2Image;

                        if(_Signature3File != null)
                            _PreviousSignature3FileName = certificateRecord_VM.Signature3Image;
                        else 
                            _Signature3FileNameGenerated= certificateRecord_VM.Signature3Image;

                        if(_LicenseLogoFile != null)
                            _PreviousLicenseLogoFileName = certificateRecord_VM.LicenseLogo;
                        else
                            _LicenseLogoFileNameGenerated = certificateRecord_VM.LicenseLogo;
                    }
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        SP_InsertUpdateLicense_Params_VM params_VM = new SP_InsertUpdateLicense_Params_VM()
                        {
                            Id =  requestLicense_VM.Id,
                            CertificateId =  requestLicense_VM.CertificateId,
                            Title =  requestLicense_VM.Title,
                            CertificateImage =  _CertificateFileNameGenerated,
                            LicenseLogoImage =  _LicenseLogoFileNameGenerated,
                            SignatureImage =  _SignatureFileNameGenerated,
                            Description = requestLicense_VM.Description,
                            IsPaid = requestLicense_VM.IsPaid,
                            Status = requestLicense_VM.Status,
                            CommissionType = requestLicense_VM.CommissionType,
                            CommissionValue = requestLicense_VM.CommissionValue,
                            LicenseDuration = requestLicense_VM.LicenseDuration,
                            TimePeriod = requestLicense_VM.TimePeriod,
                            AchievingOrder = requestLicense_VM.AchievingOrder,
                            LicensePermissions = requestLicense_VM.BusinessPanelPermissions,
                            SubmittedByLoginId =  _LoginID_Exact,
                            Mode =  requestLicense_VM.Mode,
                            Signature2Image =  _Signature2FileNameGenerated,
                            Signature3Image =  _Signature3FileNameGenerated,
                            Price =  requestLicense_VM.Price,
                            GSTPercent =  requestLicense_VM.GSTPercent,
                            GSTDescription =  requestLicense_VM.GSTDescription,
                            MinSellingPrice =  requestLicense_VM.MinSellingPrice,
                            LicenseCertificateHTMLContent = requestLicense_VM.LicenseCertificateHTMLContent,
                            IsLicenseToTeach = requestLicense_VM.IsLicenseToTeach,
                            LicenseToTeach_Type = requestLicense_VM.LicenseToTeach_Type,
                            LicenseToTeach_DisplayName = requestLicense_VM.LicenseToTeach_DisplayName,
                            MasterPro = requestLicense_VM.MasterPro
                        };

                        var resp_IULicense = licenseService.InsertUpdateLicense(params_VM);

                        if (resp_IULicense.ret <= 0)
                        {
                            apiResponse.status = resp_IULicense.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp_IULicense.resourceFileName, resp_IULicense.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        #region Insert/Update Feature Permissions On Licence creation ----------------------------
                        // if update request then delete existing item-features
                        if (requestLicense_VM.Mode == 2)
                        {
                            var resp_DeleteExistingItemFeatures = itemFeatureService.DeleteItemFeatures(resp_IULicense.Id, ItemFeatures_ItemType.License.ToString());
                            string enumstring = ItemFeatures_ItemType.License.ToString();
                        }

                        // insert all passed item features.
                        foreach(var itemFeature in requestLicense_VM.FeaturePermissions)
                        {
                            var resp_itemFeature = itemFeatureService.InsertItemFeatures(new ViewModels.StoredProcedureParams.SP_InsertUpdateItemFeatures_Params_VM()
                            {
                                ItemId = resp_IULicense.Id,
                                ItemType = ItemFeatures_ItemType.License.ToString(),
                                FeatureId = itemFeature.FeatureId,
                                IsLimited = itemFeature.IsLimited,
                                Limit = itemFeature.Limit
                            });
                        }
                        #endregion ------------------------------------------------------------------------

                        db.SaveChanges(); // Save changes to the database

                        transaction.Commit(); // Commit the transaction if everything is successful

                        // Insert-Update Certificate Image.
                        #region Insert-Update [Certificate Image] on Server

                        if (resp_IULicense.ret == 1)
                        {
                            if (files.Count > 0)
                            {
                                InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_LicenseCertificate, _PreviousCertificateFileName, _CertificateFileNameGenerated, _CertificateFile);
                                InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_LicenseLogo, _PreviousLicenseLogoFileName, _LicenseLogoFileNameGenerated, _LicenseLogoFile);
                                InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_LicenseSignature, _PreviousSignatureFileName, _SignatureFileNameGenerated, _SignatureFile);
                                InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_LicenseSignature, _PreviousSignature2FileName, _Signature2FileNameGenerated, _Signature2File);
                                InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_LicenseSignature, _PreviousSignature3FileName, _Signature3FileNameGenerated, _Signature3File);
                            }
                        }
                        #endregion

                        apiResponse.status = 1;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp_IULicense.resourceFileName, resp_IULicense.resourceKey);
                        apiResponse.data = new { };
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions and perform error handling or logging
                        transaction.Rollback(); // Roll back the transaction
                        apiResponse.status = -100;
                        apiResponse.message = Resources.ErrorMessage.DatabaseTransactionFailedErrorMessage;
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
                    }
                } // transaction scope ends

            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get All Licenses data by the Pagination
        /// </summary>
        /// <returns>List of Created Licenses</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/GetAllLicenseByPagination")]
        public HttpResponseMessage GetAllLicenseDataTablePagination(long certificateId)
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

                License_Pagination_SQL_Params_VM _Params_VM = new License_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.CertificateId = certificateId;
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;

                var paginationResponse = licenseService.GetAllLicense_Pagination(HttpRequestParams, _Params_VM);

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
        [Route("api/License/GetById/{id}")]
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

                // Get License-Detail-By-Id
                var resp = licenseService.GetLicenseById(id);

                resp.FeaturePermissions = itemFeatureService.GetItemFeatures(id, ItemFeatures_ItemType.License.ToString());

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
        /// Delete License Record (Soft delete)
        /// </summary>
        /// <param name="id">License Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/DeleteById/{id}")]
        public HttpResponseMessage DeleteLicenseById(long id)
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

                var resp = licenseService.DeleteLicense(id, _LoginId);

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
        /// Change License status by License-Id (Toggles the status i.e. Active if stauts is inactive and vice versa)
        /// </summary>
        /// <param name="id">License Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeLicenseStatus(int id)
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

                var resp = licenseService.ChangeLicenseStatus(id, _LoginID_Exact);

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
        /// Get Available License Achieving Order in same certification profile by certificate-Id
        /// </summary>
        /// <param name="certificateId">Certificate-Id</param>
        /// <returns>Available achieving Order number</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/GetAvailableLicenseAchievingOrder")]
        public HttpResponseMessage GetAvailableLicenseAchievingOrder(long certificateId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (certificateId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                // Get available Achieving order number
                var resp = licenseService.GetAvailableLicenseAchievingOrder(certificateId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    AvailableAchievingOrder = resp,
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

        #endregion ---------------------------------------------------------------------------

        /// <summary>
        /// Get All Active Licenses Detail by Certificate-Id for Business-Owner/Panel
        /// </summary>
        /// <param name="certificateId">Certificate Id</param>
        /// <returns>List of Active Certificate-Licenses</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/License/GetAllActiveLicenseDetailByCertificateForBO")]
        public HttpResponseMessage GetAllActiveLicenseDetailByCertificateForBO(long certificateId)
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

                // Get License-Detail-By-Id
                var resp = licenseService.GetAllActiveLicensesByCertificateIdForBO(certificateId);

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
        /// Get All Licenses-Booking Request data by the Pagination [for Business-Admin Panel]
        /// </summary>
        /// <returns>List of Created Licenses</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/License/GetAllLicenseBookingRequestForBOByPagination")]
        public HttpResponseMessage GetAllLicenseBookingRequestForBODataTablePagination()
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

                License_Pagination_SQL_Params_VM _Params_VM = new License_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _BusinessOwnerLoginId;

                var paginationResponse = licenseService.GetAllLicenseBookingsByBusiness_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get All Business-Licenses (Approved) data by the Pagination [for Business-Admin Panel]
        /// </summary>
        /// <returns>List of Business Approved Licenses</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/License/GetAllBusinessApprovedLicensesBookingByPagination")]
        public HttpResponseMessage GetAllBusinessApprovedLicensesBookingByPagination()
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

                License_Pagination_SQL_Params_VM _Params_VM = new License_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 2;
                _Params_VM.LoginId = _BusinessOwnerLoginId;

                var paginationResponse = licenseService.GetAllBusinesssLicenses_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get License Booking Detail by Id
        /// </summary>
        /// <param name="id">License-Booking-Id</param>
        /// <returns>License booking detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/License/GetLicenseBookingDetailForBOById")]
        public HttpResponseMessage GetLicenseBookingDetailForBOById(long id)
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

                // Get License-Booking-Detail-By-Id
                var resp = licenseService.GetLicenseBookingDetailForBOById(id);

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
        /// Get All Licenses-Booking Request data by the Pagination [for Super-Admin Panel]
        /// </summary>
        /// <returns>List of Licenses Requests</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/GetAllLicenseBookingRequestForSAByPagination")]
        public HttpResponseMessage GetAllLicenseBookingRequestForSADataTablePagination()
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

                License_Pagination_SQL_Params_VM _Params_VM = new License_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;

                var paginationResponse = licenseService.GetAllLicenseBookingsBySuperAdmin_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get License Booking Detail by Id
        /// </summary>
        /// <param name="id">License-Booking-Id</param>
        /// <returns>License booking detail</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/GetLicenseBookingDetailForSAById")]
        public HttpResponseMessage GetLicenseBookingDetailForSAById(long id)
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

                // Get License-Booking-Detail-By-Id
                var resp = licenseService.GetLicenseBookingDetailForSAById(id);

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
        /// Update License Booking Request Status by License-Booking-Id
        /// </summary>
        /// <param name="id">License-Booking-Id</param>
        /// <param name="status">Booking Status: [2 -> to Approve], [3 -> to Decline]</param>
        /// <returns>License booking detail</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/UpdateLicenseBookingStatusById")]
        public HttpResponseMessage UpdateLicenseBookingStatusById(long id, int status)
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
                else if(status <= 1)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvaildRequestDataPassed + " Status value must be 2 or 3";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                // Get License-Booking-Detail-By-Id
                SPResponseViewModel resp = new SPResponseViewModel();
                
                if(status == 2) { 
                    resp = licenseService.ApproveLicenseBooking(id);
                }
                else if(status == 3)
                {
                    resp = licenseService.DeclineLicenseBooking(id);
                }

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
        /// Update License Booking Request Status by License-Booking-Id
        /// </summary>
        /// <param name="paymentResponseId">Payment-Response-Id</param>
        /// <returns>License booking detail</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/License/ApproveLicenseBookingPaymentStatusById")]
        public HttpResponseMessage ApproveLicenseBookingPaymentStatusById(long paymentResponseId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (paymentResponseId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                // Get License-Booking-Detail-By-Id
                SPResponseViewModel resp = new SPResponseViewModel();

                resp = licenseService.ApproveLicenseBookingPayment(paymentResponseId);

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
        /// To Get License To Teach Detail By BusinessOwnerLoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentLicenseToTeachForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentLicenseToTeachForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // LicenseToTeachDetail_VM resp = new LicenseToTeachDetail_VM();
                List<LicenseToTeachDetail_VM> resp = licenseService.SP_ManageLicenseToTeach_GetAll(businessOwnerLoginId);

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
        /// Get All Available Booked-License Certification Profiles by Business for dropdown
        /// if TrainingId is passed then it also includes its Certification instead of its Availability/quantity
        /// </summary>
        /// <param name="trainingId">Training Id</param>
        /// <returns>List of Certification Profiles</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/License/GetAllAvaliableBookedLicenseCertificationProfiles")]
        public HttpResponseMessage GetAllAvaliableBookedLicenseCertificationProfiles(long trainingId = 0)
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

                // Get License-Booking-Detail-By-Id
                var certificationProfiles = licenseService.GetAllAvailableBookedCertificationProfilesByBusiness(_BusinessOwnerLoginId, trainingId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = certificationProfiles;

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
        /// Get All Available Booked-Licenses by certificate Id by Business for dropdown
        /// if TrainingId is passed then it also includes its License instead of its Availability/quantity
        /// </summary>
        /// <param name="certificateId">Certification Profile Id</param>
        /// <param name="trainingId">Training-Id</param>
        /// <returns>Booked Business-License List by certification profile</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/License/GetAllAvaliableBookedLicenses")]
        public HttpResponseMessage GetAllAvaliableBookedLicenseCertificationProfiles(long certificateId, long trainingId = 0)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if(certificateId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get All Available Buisiness Licenses by Certification Profile
                var businessLicenses = licenseService.GetAllAvailableBookedLicensesByBusiness(_BusinessOwnerLoginId, certificateId, trainingId: trainingId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessLicenses;

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
        /// Get All Liecenses by Certificates Detail DropDown for frontpage dropdown 
        /// </summary>
        /// <returns>List of Licenses-Types by Certificates List.</returns>
        [Route("api/License/GetAllLicensesListByCertificatesForDropDown")]
        [HttpGet]
        public HttpResponseMessage GetAllLicensesListByCertificatesForDropDown()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                //List<ClassCategoryType_VM> classCategoryTypes = classCategoryTypeService.Get_ManageClasssCategoryDetailDropdown();
                //List<ClassCategoryType_VM> ParentClassCategories = classCategoryTypes.Where(c => c.ParentClassCategoryTypeId == 0).ToList();

                //foreach (var p in ParentClassCategories)
                //{
                //    p.SubClassCategory = classCategoryTypes.Where(c => c.ParentClassCategoryTypeId == p.Id).ToList();
                //}
                certificateService = new CertificateService(db);
                var activeCertificates = certificateService.GetAllCertificates().Where(c => c.Status == 1).ToList();

                var activeLicenses = licenseService.GetAllLicenses();
                List<CertificateLicenseListForDropdown_VM> certificateLicenseListForDropdown_VM = new List<CertificateLicenseListForDropdown_VM>();
                foreach(var certificate in activeCertificates)
                {
                    List<LicenseListForDropdown_VM> activeLicensesList = activeLicenses.Where(l => l.CertificateId ==  certificate.Id && l.Status == 1).Select(s => new LicenseListForDropdown_VM() { Id = s.Id, Title = s.Title }).ToList();

                    if (activeLicensesList.Count <= 0)
                        continue;

                    // get licenses list
                    certificateLicenseListForDropdown_VM.Add(new CertificateLicenseListForDropdown_VM()
                    {
                        Id = certificate.Id,
                        Name = certificate.Name,
                        Licenses = activeLicensesList
                    });
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = certificateLicenseListForDropdown_VM;

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