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
    public class BusinessContentSponsorAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private BusinessContentSponsorService businessContentSponsorService;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;
        public BusinessContentSponsorAPIController()
        {
            db = new MasterZoneDbContext();
            businessContentSponsorService = new BusinessContentSponsorService(db);
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
        /// To Add Business Service Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessSponsor")]
        public HttpResponseMessage AddUpdateBusinessSponsorProfile()
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
                BusinessContentSponsor_ViewModel businessContentSponsor_VM = new BusinessContentSponsor_ViewModel();
                businessContentSponsor_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentSponsor_VM.SponsorTitle = HttpRequest.Params["SponsorTitle"].Trim();
                businessContentSponsor_VM.SponsorLink = HttpRequest.Params["SponsorLink"].Trim();
                businessContentSponsor_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
               


                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssSponsorIconFile = files["SponsorIcon"];
                businessContentSponsor_VM.SponsorIcon = _BusinesssSponsorIconFile; // for validation
                string _BusinessSponsorIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessSponsorIconFileName = ""; // will be used to delete file while updating.


                

                // Validate infromation passed
                Error_VM error_VM = businessContentSponsor_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                   
                        if (_BusinesssSponsorIconFile != null)
                        {
                            _BusinessSponsorIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssSponsorIconFile);
                        }

                    
                }


                if (businessContentSponsor_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessContentSponsorService.GetBusinessSponsorDetailById(businessContentSponsor_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessSponsorIconFileNameGenerated = respGetBusinessServiceData.SponsorIcon ?? "";
                    
                    }
                    else
                    {
                        _PreviousBusinessSponsorIconFileName = respGetBusinessServiceData.SponsorIcon ?? "";
              
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessSponsor_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessSponsor_Params_VM
                {
                    Id = businessContentSponsor_VM.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    SponsorTitle = businessContentSponsor_VM.SponsorTitle,
                    SponsorLink = businessContentSponsor_VM.SponsorLink,
                    SponsorIcon = _BusinessSponsorIconFileNameGenerated,
                    Mode = businessContentSponsor_VM.Mode
                });




                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Sponsor Image.
                    #region Insert-Update Sponsor Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessSponsorIconFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentSponsors), _PreviousBusinessSponsorIconFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentSponsors), _BusinessSponsorIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssSponsorIconFile, NewImageFileWithPath);

                     
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
        /// To Get Business Sponsor Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessSponsorDetailById")]
        public HttpResponseMessage GetBusinessSponsorDetailById(long Id)
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


                BusinessContentSponsorDetail_VM resp = new BusinessContentSponsorDetail_VM();

                resp = businessContentSponsorService.GetBusinessSponsorDetailById(Id);


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
        /// Delete Sponsor by SponsorId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/Sponsor/DeleteById")]
        public HttpResponseMessage DeleteSponsorById(long Id)
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

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Business Sponsor 
                resp = businessContentSponsorService.DeleteBusinessSponsor(Id);


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
        /// Get All Business Sponsor with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessSponsorByPagination")]
        public HttpResponseMessage GetAllBusinessSponsorDataTablePagination()
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

                BusinessContentSponsorDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentSponsorDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessContentSponsorService.GetBusinessSponsorList_Pagination(HttpRequestParams, _Params_VM);

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


    }
}