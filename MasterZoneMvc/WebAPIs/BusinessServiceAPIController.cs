using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
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
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessServiceAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private BusinessOwnerService businessOwnerService;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;        
        public BusinessServiceAPIController()
        {
            db = new MasterZoneDbContext();
            businessOwnerService = new BusinessOwnerService(db);
            fileHelper = new FileHelper();
            storedProcedureRepository =  new StoredProcedureRepository(db);
            
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
        /// Get All Business Service with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessServiceByPagination")]
        public HttpResponseMessage GetAllBusinessServiceDataTablePagination()
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

                BusinessService_Pagination_SQL_Params_VM _Params_VM = new BusinessService_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessServiceList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Add Business Service Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessServiceInformation")]
        public HttpResponseMessage AddUpdateBusinessServiceProfile()
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
                BusinessService_VM businessService_VM = new BusinessService_VM();
                businessService_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessService_VM.Title = HttpRequest.Params["Title"].Trim();
                businessService_VM.Description = HttpRequest.Params["Description"].Trim();
                businessService_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                businessService_VM.ServiceType = Convert.ToInt32(HttpRequest.Params["ServiceType"]);
                if (businessService_VM.Mode == 2)
                {
                    businessService_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                }


                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssServiceImageFile = files["FeaturedImage"];
                businessService_VM.FeaturedImage = _BusinesssServiceImageFile; // for validation
                string _BusinessServiceImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessServiceImageFileName = ""; // will be used to delete file while updating.


                HttpPostedFile _BusinessServiceIconFile = files["Icon"];
                businessService_VM.Icon = _BusinessServiceIconFile; // for validation
                string _BusinessServiceIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessServiceIconFileName = ""; // will be used to delete file while updating.


                // Validate infromation passed
                Error_VM error_VM = businessService_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_BusinesssServiceImageFile != null || _BusinessServiceIconFile != null)
                    {
                        if (_BusinesssServiceImageFile != null)
                        {
                            _BusinessServiceImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssServiceImageFile);
                        }

                        if (_BusinessServiceIconFile != null)
                        {
                            _BusinessServiceIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinessServiceIconFile);
                        }
                    }
                }


                if (businessService_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessOwnerService.GetBusinessServiceDetailById(businessService_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessServiceImageFileNameGenerated = respGetBusinessServiceData.FeaturedImage ?? "";
                        _BusinessServiceIconFileNameGenerated = respGetBusinessServiceData.Icon ?? "";
                    }
                    else
                    {
                        _PreviousBusinessServiceImageFileName = respGetBusinessServiceData.FeaturedImage ?? "";
                        _PreviousBusinessServiceIconFileName = respGetBusinessServiceData.Icon ?? "";
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessService_Params_VM
                {
                    Id = businessService_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Title = businessService_VM.Title,
                    Description = businessService_VM.Description,
                    FeaturedImage = _BusinessServiceImageFileNameGenerated,
                    Icon = _BusinessServiceIconFileNameGenerated,
                    Status = businessService_VM.Status,
                    ServiceType = businessService_VM.ServiceType,
                    Mode = businessService_VM.Mode
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
                    #region Insert-Update Training Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessServiceImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessServiceImage), _PreviousBusinessServiceImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessServiceImage), _BusinessServiceImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssServiceImageFile, NewImageFileWithPath);

                        if (!String.IsNullOrEmpty(_PreviousBusinessServiceIconFileName))
                        {
                            // remove previous icon file
                            string RemoveIconFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessServiceIcon), _PreviousBusinessServiceIconFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveIconFileWithPath);
                        }

                        // save new icon file
                        string NewIconFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessServiceIcon), _BusinessServiceIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinessServiceIconFile, NewIconFileWithPath);
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
        /// To Get Business Service Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessServiceDetail")]
        public HttpResponseMessage GetBusinessServiceDetailById(long Id)
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


                BusinessService_ViewModel resp = new BusinessService_ViewModel();

                resp = businessOwnerService.GetBusinessServiceDetailById( Id);
               

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
        /// To delete the Business Service Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/BusinessServiceDelete")]
        public HttpResponseMessage DeleteBusinessServiceById(long Id)
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


                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Business Service 
                resp = businessOwnerService.DeleteBusinessService(Id);


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
         /// To Get Business Service detail
         /// </summary>
         /// <param name="Id"></param>
         /// <returns></returns>

        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessServiceDetails")]
        public HttpResponseMessage GetBusinessServiceDetails(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                

                BusinessService_ViewModel resp = new BusinessService_ViewModel();

                 resp = businessOwnerService.GetBusinessServiceDetail(businessOwnerLoginId);

                List<BusinessService_ViewModel> businessserviceList = businessOwnerService.GetBusinessServiceDetailList(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessServiceDetailList = businessserviceList,
                    BusinessServiceDetail = resp,
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
        /// To Add/Update  Instructor Rating
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Business/AddUpdate")]
        public HttpResponseMessage AddUpdateInstructorRating(long BusinessOwnerLoginId)
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                InstrucrtorRatingReview_VM instructorRating = new InstrucrtorRatingReview_VM();
                instructorRating.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                instructorRating.ItemId = Convert.ToInt64(HttpRequest.Params["ItemId"]);
                instructorRating.Rating = Convert.ToInt32(HttpRequest.Params["Rating"]);
                instructorRating.ReviewBody = HttpRequest.Params["ReviewBody"];
                instructorRating.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Insert-Update Rating Information
                var resp = storedProcedureRepository.SP_InsertUpdateDanceRatingReview_Get<SPResponseViewModel>(new SP_InsertUpdateRatingReview_Params_VM
                {
                    Id = instructorRating.Id,
                    ItemId =  BusinessOwnerLoginId,
                    Rating = instructorRating.Rating,
                    ReviewBody = instructorRating.ReviewBody,
                    ReviewerUserLoginId = _LoginID_Exact,
                    Mode = instructorRating.Mode


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
        /// To Add Business Content  Service Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentService")]

        public HttpResponseMessage AddUpdateBusinessContentService()
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
               // long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
               
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentServiceViewModel businessContentServiceViewModel = new BusinessContentServiceViewModel();
                businessContentServiceViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentServiceViewModel.ServiceTitle = HttpRequest.Params["ServiceTitle"].Trim();
                businessContentServiceViewModel.ShortDescription = HttpRequest.Params["ShortDescription"].Trim();
                businessContentServiceViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Validate information passed

                Error_VM error_VM = businessContentServiceViewModel.ValidInformation();
               


                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

               

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentService
                {
                    Id = businessContentServiceViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ServiceTitle = businessContentServiceViewModel.ServiceTitle,
                    ShortDescription = businessContentServiceViewModel.ShortDescription,
                    Mode = businessContentServiceViewModel.Mode
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
        /// To Get Business Content Detail By UserLoginId For Yoga page to show services
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBusinessContentService")]
        public HttpResponseMessage GetBusinesscontentServiceDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessService_ViewModel resp = new BusinessService_ViewModel();

                resp = businessOwnerService.GetBusinessContentServiceDetail(businessOwnerLoginId);
                List<BusinessService_ViewModel> businessserviceList = businessOwnerService.GetBusinessContentServiceDetailList(businessOwnerLoginId);
                List<BusinessService_ViewModel> businessmainserviceList = businessOwnerService.GetBusinessMainServiceDetailList(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessServiceDetailList = businessserviceList,
                    BusinessMainServiceDetailList = businessmainserviceList,
                    BusinessServiceDetail = resp,
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
        /// To Get Business Content Service Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentServiceDetail")]
        public HttpResponseMessage GetBusinessContentAbout()
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

                BusinessService_ViewModel resp = new BusinessService_ViewModel();

                resp = businessOwnerService.GetBusinessContentServiceDetail(_BusinessOwnerLoginId);

               // List<BusinessService_ViewModel> businessserviceList = businessOwnerService.GetBusinessContentServiceDetailList(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                   // BusinessServiceDetailList = businessserviceList,
                    BusinessServiceDetail = resp,
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






        ///////////////////////////////To Add Much More Service Detail ////////////////////////////////With Pagination/////////////////////////////
        ///
        /// <summary>
        /// To Add Business  Content Much More Service  Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentMuchMore")]

        public HttpResponseMessage AddUpdateBusinessContentMuchMoreProfile()
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
                BusinessContentMuchMoreService_VM businessContentMuchMoreService_VM = new BusinessContentMuchMoreService_VM();

                // Parse and assign values from HTTP request parameters
                businessContentMuchMoreService_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentMuchMoreService_VM.Content = HttpRequest.Params["Content"].Trim();
                businessContentMuchMoreService_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


              
                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssContentMuchMoreServiceImageFile = files["ServiceIcon"];
                businessContentMuchMoreService_VM.ServiceIcon = _BusinesssContentMuchMoreServiceImageFile; // for validation
                string _BusinessMuchMoreServiceImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessMuchMoreServiceImageFileName = ""; // will be used to delete file while updating.

                // Validate information passed


                Error_VM error_VM = businessContentMuchMoreService_VM.ValidInformation();


                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                if (files.Count > 0)
                {
                    if (_BusinesssContentMuchMoreServiceImageFile != null)
                    {

                        _BusinessMuchMoreServiceImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssContentMuchMoreServiceImageFile);
                    }

                }

                if (businessContentMuchMoreService_VM.Mode == 2)
                {

                    var respGetBusinessContentWorldClassProgramDetail = businessOwnerService.GetBusinessContentMuchMoreServiceDetailById(businessContentMuchMoreService_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessMuchMoreServiceImageFileNameGenerated = respGetBusinessContentWorldClassProgramDetail.ServiceIcon ?? "";
                    }
                    else
                    {
                        _PreviousBusinessMuchMoreServiceImageFileName = respGetBusinessContentWorldClassProgramDetail.ServiceIcon ?? "";
                    }
                }




                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentMuchMoreService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentMuchMoreService_Param_VM
                {
                    Id = businessContentMuchMoreService_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Content = businessContentMuchMoreService_VM.Content,
                    ServiceIcon = _BusinessMuchMoreServiceImageFileNameGenerated,
                    Mode = businessContentMuchMoreService_VM.Mode
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
                    #region Insert-Update Business Content  Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessMuchMoreServiceImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessMuchMoreServiceIcon), _PreviousBusinessMuchMoreServiceImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessMuchMoreServiceIcon), _BusinessMuchMoreServiceImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssContentMuchMoreServiceImageFile, NewImageFileWithPath);


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
        /// Get All Business Content Much More Service with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentMuchMoreServiceDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentWorldClassDataTablePagination()
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

                BusinessContentMuchMoreServiceDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentMuchMoreServiceDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessOwnerService.GetBusinessContentMuchMoreServiceList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Business Content Much More Service Detail By Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEditBusinessContentMuchMoreServiceDetail/ById/{id}")]
        public HttpResponseMessage GetBusinessContentMuchMoreDetail(long Id)
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


                BusinessContentMuchMoreServiceDetail_VM resp = new BusinessContentMuchMoreServiceDetail_VM();

                resp = businessOwnerService.GetBusinessContentMuchMoreServiceDetailById(Id);



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
        /// Delete Much More Service Detail
        /// </summary>
        /// <param name="id">MuchMoreServiceId</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteBusinessContentMuchMoreDetail")]
        public HttpResponseMessage DeleteWorldClassProgramById(long id)
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

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete Much More Service Detail 
                resp = businessOwnerService.DeleteMuchMoreServiceDetail(id);


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


        /// To Add Business  Content Much More Service  Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentMuchMoreService")]

        public HttpResponseMessage AddUpdateBusinessContentMuchMoreServiceProfile()
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
                BusinessContentMuchMoreServiceDetailViewModel businessContentMuchMoreServiceDetailViewModel = new BusinessContentMuchMoreServiceDetailViewModel();

                // Parse and assign values from HTTP request parameters
                businessContentMuchMoreServiceDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentMuchMoreServiceDetailViewModel.Title = HttpRequest.Params["Title"].Trim();
                businessContentMuchMoreServiceDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);








                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentMuchMoreServiceDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentMuchMoreServiceDetail_Param_VM
                {
                    Id = businessContentMuchMoreServiceDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentMuchMoreServiceDetailViewModel.Title,
                    Mode = businessContentMuchMoreServiceDetailViewModel.Mode
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
        /// To Get Business Content Much More Service Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEditBusinessContentMuchMoreServiceDetail")]
        public HttpResponseMessage GetBusinessContentMuchMoreDetailByBusinessOwnerLoginId()
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


                BusinessContentMuchMoreServiceDetailViewModel resp = new BusinessContentMuchMoreServiceDetailViewModel();

                resp = businessOwnerService.GetBusinessContentMuchMoreServiceDetail(_BusinessOwnerLoginId);



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


        /////////////////////////////////////To Show Gym Page Much More Service//////////////////////////////////////////

        /// <summary>
        /// To Get Business Content Much More Service Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEditBusinessContentMuchMoreServiceDetail_PPCMeta")]
        public HttpResponseMessage GetBusinessContentMuchMoreServiceDetailByBusinessOwnerLoginId(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessContentMuchMoreServiceDetailViewModel resp = new BusinessContentMuchMoreServiceDetailViewModel();

                resp = businessOwnerService.GetBusinessContentMuchMoreServiceDetail(businessOwnerLoginId);

                List<BusinessContentMuchMoreServiceDetail_VM> businessContentMuchMoreServicelst = businessOwnerService.GetBusinessContentMuchMoreServiceList_Get(businessOwnerLoginId);



                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentMuchMoreServiceTitleDetail = resp,
                    BusinessContentMuchMoreServiceDetail = businessContentMuchMoreServicelst
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
        /// To Get Business Service detail For Master-Pro Resume Page
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBusinessServiceDetails_MasterPro")]
        public HttpResponseMessage GetBusinessServiceDetails_MasterPro(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessService_ViewModel> resp = new List<BusinessService_ViewModel>();
                resp = businessOwnerService.GetBusinessServiceDetailList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessServiceDetail = resp,
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