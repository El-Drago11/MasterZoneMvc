using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
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
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class ReviewAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private ReviewService reviewService;
        private StoredProcedureRepository storedProcedureRepository;
        private BusinessOwnerService businessOwnerService;
        private FileHelper fileHelper;

        public ReviewAPIController()
        {
            db = new MasterZoneDbContext();
            reviewService = new ReviewService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
            businessOwnerService = new BusinessOwnerService(db);
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

        /// <summary>
        /// Get All Rating with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Review/GetAllByPagination")]
        public HttpResponseMessage GetAllReviewDataTablePagination()
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

                Review_Pagination_SQL_Params_VM _Params_VM = new Review_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = reviewService.GetRatingReview_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Rating Review  Detail by Id 
        /// </summary>
        /// <param name="id">Review Id</param>
        /// <returns>review </returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Review/GetById")]
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Review-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "1")
                            };

                var responseReview = db.Database.SqlQuery<Review_VM>("exec sp_ManageReview @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    RatingreviewDetail = responseReview,
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
        /// To delete the Rating Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Review/DeleteById/{id}")]
        public HttpResponseMessage DeleteRatingById(long id)
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

                // Insert-Update Rating Information
                SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", id),
                    new SqlParameter("itemId ", _UserLoginId),
                    new SqlParameter("itemType",""),
                    new SqlParameter("rating","0"),
                    new SqlParameter("reviewBody ",""),
                    new SqlParameter("reviewerUserLoginId  ",""),
                    new SqlParameter("mode","3"),
                    };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateReview @id,@itemId,@itemType,@rating,@reviewBody,@reviewerUserLoginId,@mode", queryParams).FirstOrDefault();

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
        /// Rating Record To Change status by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Review/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeRatingStatus(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", id),
                    new SqlParameter("businessOwnerLoginId",_BusinessOwnerLoginId),
                    new SqlParameter("userLoginId", _LoginID_Exact),
                    new SqlParameter("mode", "2")
                    };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageReview @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

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
        /// To Add/Update  Review Rating
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Review/AddUpdateByBusiness")]
        public HttpResponseMessage AddUpdateRatingReview()
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
                RequestReview_VM ratingReview = new RequestReview_VM();
                ratingReview.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                ratingReview.ItemId = Convert.ToInt64(HttpRequest.Params["ItemId"]);
                ratingReview.ItemType = HttpRequest.Params["ItemType"];
                ratingReview.Rating = Convert.ToInt32(HttpRequest.Params["Rating"]);
                ratingReview.ReviewBody = HttpRequest.Params["ReviewBody"];
                ratingReview.ReviewerUserLoginId = Convert.ToInt64(HttpRequest.Params["ReviewerUserLoginId"]);
                ratingReview.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Insert-Update Rating Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", ratingReview.Id ),
                            new SqlParameter("itemId ", _BusinessOwnerLoginId),
                            new SqlParameter("itemType",""),
                            new SqlParameter("rating",""),
                            new SqlParameter("reviewBody", ratingReview.ReviewBody),
                            new SqlParameter("reviewerUserLoginId  ","0"),
                            new SqlParameter("mode",ratingReview.Mode),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateReview @id,@itemId,@itemType,@rating,@reviewBody,@reviewerUserLoginId,@mode", queryParams).FirstOrDefault();

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

        ///// <summary>
        ///// To Add/Update  Instructor Rating
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize(Roles = "Student")]
        //[Route("api/Rating/AddUpdate")]
        //public HttpResponseMessage AddUpdateInstructorRating()
        //{
        //    ApiResponse_VM apiResponse = new ApiResponse_VM();
        //    try
        //    {
        //        var validateResponse = ValidateLoggedInUser();

        //        if (validateResponse.ApiResponse_VM.status < 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
        //        }
        //        long _LoginID_Exact = validateResponse.UserLoginId;
        //        long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

        //        //--Create object of HttpRequest
        //        var HttpRequest = HttpContext.Current.Request;
        //        InstructorRating instructorRating = new InstructorRating();
        //        instructorRating.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
        //        instructorRating.Rating = Convert.ToInt32(HttpRequest.Params["Rating"]);
        //        instructorRating.RatingFeedback = HttpRequest.Params["RatingFeedback"];
        //        //  instructorRating.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


        //        // Insert-Update Rating Information
        //        SqlParameter[] queryParams = new SqlParameter[] {
        //                    new SqlParameter("id", instructorRating.Id ),
        //                    new SqlParameter("itemId ", "0"),
        //                    new SqlParameter("itemType","Instructor"),
        //                    new SqlParameter("rating",  instructorRating.Rating),
        //                    new SqlParameter("reviewBody", instructorRating.RatingFeedback),
        //                    new SqlParameter("reviewerUserLoginId  ",_LoginID_Exact),
        //                    new SqlParameter("mode",1),
        //                    };

        //        var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateRating @id,@itemId,@itemType,@rating,@reviewBody,@reviewerUserLoginId,@mode", queryParams).FirstOrDefault();

        //        if (resp.ret <= 0)
        //        {
        //            apiResponse.status = resp.ret;
        //            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
        //            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //        }



        //        apiResponse.status = 1;
        //        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
        //        apiResponse.data = new { };
        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.status = -500;
        //        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Review/InstructorReviewDetailGet")]
        public HttpResponseMessage GetReviewDetailById(long UserLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (UserLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                BusinessReviewDetail instructorReviewDetail = new BusinessReviewDetail();
                instructorReviewDetail = reviewService.GetInstructorReviewDetail(UserLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {

                    BusinessReviewDetail = instructorReviewDetail,


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
        /// To Add Business  Rating Review
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Review/AddReviewByUser")]
        public HttpResponseMessage AddRatingReview(long UserLoginId)
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


                long ItemId = UserLoginId;


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessRatingReview_VM businessRatingReview = new BusinessRatingReview_VM();
                businessRatingReview.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessRatingReview.ItemId = Convert.ToInt64(HttpRequest.Params["ItemId"]);
                businessRatingReview.ItemType = HttpRequest.Params["ItemType"];
                businessRatingReview.Rating = Convert.ToInt32(HttpRequest.Params["Rating"]);
                businessRatingReview.ReviewBody = HttpRequest.Params["ReviewBody"];
                businessRatingReview.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                var resp = storedProcedureRepository.SP_InsertUpdateReview_GetSingle<SPResponseViewModel>(new SP_InsertUpdateReview_Params_VM
                {
                    Id = businessRatingReview.Id,
                    ItemId = UserLoginId,
                    Rating = businessRatingReview.Rating,
                    ReviewBody = businessRatingReview.ReviewBody,
                    ReviewerUserLoginId = _LoginID_Exact,
                    Mode = businessRatingReview.Mode
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
        /// To Add/Update  Instructor Rating
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Review/AddUpdate")]
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
                    ItemId = BusinessOwnerLoginId,
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
        /// To Get Rating Review  Detail 
        /// /// </summary>
        /// <param name="id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Review/GetReviewRatingDetail")]
        public HttpResponseMessage GetRatingReviewDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _LoginID_Exact = validateResponse.UserLoginId;
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                DanceRatingReview_VM resp = new DanceRatingReview_VM();

                List<DanceRatingReview_VM> ratingReview = reviewService.GetRatingReviewDetail(businessOwnerLoginId);
                BusinessContentReviewDetail_VM businessReviewDetail = reviewService.GetBusinessContentReviewDetail(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessReviewDetail = businessReviewDetail,
                    DanceRatingReviewDetail = ratingReview,
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

        ////////////////////////////////////Review Detail For Pages//////////////////////////
        ///
        /// <summary>
        /// To Add Business  Review Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessReview")]

        public HttpResponseMessage AddUpdateBusinessReviewProfile()
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
                BusinessContentReview_VM businessContentReview_VM = new BusinessContentReview_VM();

                // Parse and assign values from HTTP request parameters
                businessContentReview_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentReview_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentReview_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssReviewImageFile = files["ReviewImage"];
                businessContentReview_VM.ReviewImage = _BusinesssReviewImageFile; // for validation
                string _BusinessReviewImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessReviewImageFileName = ""; // will be used to delete file while updating.

                // Validate information passed


                // Validate infromation passed
                Error_VM error_VM = businessContentReview_VM.ValidInformation();

                if (files.Count > 0)
                {
                    if (_BusinesssReviewImageFile != null)
                    {

                        _BusinessReviewImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssReviewImageFile);
                    }

                }


                if (businessContentReview_VM.Mode == 1)
                {
                    var respGetBusinessReviewDetail = reviewService.GetBusinessContentReviewDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessReviewDetail != null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessReviewDetail.ReviewImage == null)
                        {
                            _PreviousBusinessReviewImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessReviewImageFileNameGenerated = respGetBusinessReviewDetail.ReviewImage;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessReviewImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }




                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentReview_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentReview_Params_VM
                {
                    Id = businessContentReview_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Description = businessContentReview_VM.Description,
                    ReviewImage = _BusinessReviewImageFileNameGenerated,
                    Mode = businessContentReview_VM.Mode
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
                    #region Insert-Update Review Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessReviewImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessReviewImage), _PreviousBusinessReviewImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessReviewImage), _BusinessReviewImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssReviewImageFile, NewImageFileWithPath);


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
        /// To Get Banner Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/ReviewDetail/Get")]
        public HttpResponseMessage GetReviewDetailById()
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


                //var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //BusinesssProfilePageType.Key = "yoga_web";



                BusinessContentReviewDetail_VM businessReviewDetail = reviewService.GetBusinessContentReviewDetail(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessReviewDetail = businessReviewDetail,
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
        /// To Get Banner Detail By BusinessOwnerLoginId For Sports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetReviewDetail")]
        public HttpResponseMessage GetReviewDetails(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessContentReviewDetail_VM businessReviewDetail = reviewService.GetBusinessContentReviewDetail(businessOwnerLoginId);
                DanceRatingReview_VM resp = new DanceRatingReview_VM();

                List<DanceRatingReview_VM> ratingReview = reviewService.GetRatingReviewDetail(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessReviewDetail = businessReviewDetail,
                    BusinessRatingReviewList = ratingReview,

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



        [HttpGet]
        //  [Authorize(Roles = "Student")]
        [Route("api/Review/InstructorReviewDetailGetDettail")]
        public HttpResponseMessage GetReviewDetailList(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {



                BusinessReviewDetail instructorReviewDetail = new BusinessReviewDetail();
                instructorReviewDetail = reviewService.GetBusinessReviewDetail_Get(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {

                    BusinessReviewDetail = instructorReviewDetail,


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
        /// To Get Business Detail For Reviews Showing in Resume Page for visitor Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBusinessReviewDetail")]
        public HttpResponseMessage GetBusinessReviewDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<DanceRatingReview_VM> ratingReview = reviewService.GetBusinessRatingReviewDetail(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = ratingReview;

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