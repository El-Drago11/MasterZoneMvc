using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
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
using static MasterZoneMvc.ViewModels.CourseViewModel;

namespace MasterZoneMvc.WebAPIs
{
    public class CourseAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private CourseService courseService;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;
        public CourseAPIController()
        {
            db = new MasterZoneDbContext();
            courseService = new CourseService(db);
            fileHelper = new FileHelper();
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Validate Logged-in user. 
        /// </summary>
        /// <returns></returns>
        /// <returns></returns>
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            ValidateUserLogin_VM validateUserLogin_VM = new ValidateUserLogin_VM();

            ApiResponse_VM apiResponse = new ApiResponse_VM();

            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// To Add/Update Course Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/AddUpdateCourse")]
        public HttpResponseMessage AddUpdateCourse()
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
                CourseViewModel courseViewModel = new CourseViewModel();
                courseViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                //requestEvent_VM.UserLoginId = _LoginId;
                courseViewModel.Name = HttpRequest.Params["Name"].Trim();
                courseViewModel.Description = HttpUtility.UrlDecode(HttpRequest.Form.Get("Description"));
                courseViewModel.ShortDescription = HttpRequest.Params["ShortDescription"].Trim();
                courseViewModel.CourseStartDate = HttpRequest.Params["CourseStartDate"].Trim();
                courseViewModel.Price = Convert.ToDecimal(HttpRequest.Params["CoursePrice"]);
                courseViewModel.CoursePriceType = HttpRequest.Params["coursePaidOption"];
                courseViewModel.CourseMode = HttpRequest.Params["CourseMode"];
                courseViewModel.OnlineCourseLink = HttpRequest.Params["courseOnlineURLLink"];
                courseViewModel.CourseURLLinkPassword = HttpRequest.Params["courseOnlineURLLinkPassword"];
                courseViewModel.IsPaid = Convert.ToInt32(HttpRequest.Params["coursePriceOption"]);
                courseViewModel.Address = HttpRequest.Params["courseLocationAddress"];
                courseViewModel.City = HttpRequest.Params["courseLocationCity"];
                courseViewModel.Country = HttpRequest.Params["courseLocationCountry"];
                courseViewModel.State = HttpRequest.Params["courseLocationState"];
                courseViewModel.LandMark = HttpRequest.Params["courseLocationLandMark"];
                courseViewModel.InstructorLoginId = Convert.ToInt64(HttpRequest.Params["InstructorId"]);
                courseViewModel.GroupId = Convert.ToInt64(HttpRequest.Params["GroupId"]);
                courseViewModel.Pincode = HttpRequest.Params["courseLocationPinCode"];
                courseViewModel.HowToBookText = HttpRequest.Params["HowToBookText"];
                courseViewModel.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);
                courseViewModel.Duration = Convert.ToInt32(HttpRequest.Params["Duration"]);
                courseViewModel.DurationType = HttpRequest.Params["DurationType"];
                courseViewModel.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["courseLocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["courseLocationLatitude"]) : 0;
                courseViewModel.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["courseLocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["courseLocationLongitude"]) : 0;
                courseViewModel.CourseCategoryId = Convert.ToInt64(HttpRequest.Params["CourseCategoryId"]);
                courseViewModel.IsActive = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                courseViewModel.ExamType = HttpRequest.Params["ExamType"];
                courseViewModel.CertificateType = HttpRequest.Params["CertificateType"];
                if(courseViewModel.ExamType == "1")
                {
                    courseViewModel.ExamId = Convert.ToInt64(HttpRequest.Params["ExamId"]);
                }

                else
                {
                    courseViewModel.ExamId = 0;
                }
                 if (courseViewModel.CertificateType  =="1")
                {
                    courseViewModel.CertificateProfileId = Convert.ToInt64(HttpRequest.Params["CertificateProfileId"]);
                    courseViewModel.CertificateId = Convert.ToInt64(HttpRequest.Params["CertificateId"]);
                }
                else
                {
                    courseViewModel.CertificateId = 0;
                    courseViewModel.CertificateProfileId = 0;
                }


                    // Get Attatched Files
                    HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _CourseImageFile = files["CourseImage"];
                courseViewModel.CourseImage = _CourseImageFile; // for validation
                string _CourseImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousCourseImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = courseViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_CourseImageFile != null)
                    {
                        _CourseImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CourseImageFile);
                    }
                }

                if (courseViewModel.Mode == 2)
                {

                    var respGetCourseData = courseService.GetCourseDetail(courseViewModel.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _CourseImageFileNameGenerated = respGetCourseData.CourseImage ?? "";
                    }
                    else
                    {
                        _PreviousCourseImageFileName = respGetCourseData.CourseImage ?? "";
                    }
                }

                if (courseViewModel.CourseMode == "1")
                {
                    courseViewModel.CourseMode = "Online";
                    courseViewModel.Address = "";
                    courseViewModel.City = "";
                    courseViewModel.State = "";
                    courseViewModel.Country = "";
                    courseViewModel.LandMark = "";
                    courseViewModel.Pincode = "0";
                }
                else
                {
                    courseViewModel.CourseMode = "Offline";
                    courseViewModel.OnlineCourseLink = "";
                    courseViewModel.CourseURLLinkPassword = "";
                }

                var resp = storedProcedureRepository.SP_InsertUpdateCourse_Param_Get<SPResponseViewModel>(new SP_InsertUpdateCourse_Param_VM
                {
                    Id = courseViewModel.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    Name = courseViewModel.Name,
                    ShortDescription = courseViewModel.ShortDescription,
                    Description = courseViewModel.Description,
                    CourseStartDate = courseViewModel.CourseStartDate,
                    CourseMode = courseViewModel.CourseMode,
                    Duration = courseViewModel.Duration,
                    Price = courseViewModel.Price,
                    OnlineCourseLink = courseViewModel.OnlineCourseLink,
                    CoursePriceType = courseViewModel.CoursePriceType,
                    HowToBookText = courseViewModel.HowToBookText,
                    IsActive = courseViewModel.IsActive,
                    ExamId = courseViewModel.ExamId,
                    CertificateId = courseViewModel.CertificateId,
                    ExamType = courseViewModel.ExamType,
                    CertificateType = courseViewModel.CertificateType,
                    DurationType = courseViewModel.DurationType,
                    GroupId =   courseViewModel.GroupId,
                    Address = courseViewModel.Address,
                    Country = courseViewModel.Country,
                    City = courseViewModel.City,
                    Pincode = courseViewModel.Pincode,
                    LandMark = courseViewModel.LandMark,
                    State = courseViewModel.State,
                    IsPaid = courseViewModel.IsPaid,
                    CourseURLLinkPassword = courseViewModel.CourseURLLinkPassword,
                    InstructorLoginId = courseViewModel.InstructorLoginId,
                    CourseImage = _CourseImageFileNameGenerated,
                    Latitude = courseViewModel.Latitude,
                    Longitude = courseViewModel.Longitude,
                    CourseCategoryId = courseViewModel.CourseCategoryId,
                    CertificateProfileId = courseViewModel.CertificateProfileId,
                    SubmittedByLoginId = _BusinessOwnerLoginId,
                    Mode = courseViewModel.Mode
                });


                        if (resp.ret <= 0)
                        {
                            apiResponse.status = resp.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                // Update Class Image.
                #region Insert-Update Course Image on Server
                if (files.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(_PreviousCourseImageFileName))
                            {
                                // remove previous file
                                string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_CourseImage), _PreviousCourseImageFileName);
                                fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                            }

                            // save new file
                            string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_CourseImage), _CourseImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_CourseImageFile, FileWithPath);
                        }
                        #endregion


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
        /// To Get Course Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin, Staff")]
        [Route("api/Course/GetCourseDetailById")]
        public HttpResponseMessage GetCourseDetailById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                CourseDetail_VM resp = new CourseDetail_VM();
                resp = courseService.GetCourseDetail(id); // Pass _BusinessOwnerLoginId as the first parameter

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
        /// To Get Course Detail By Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin, Staff")]
        [Route("api/Course/GetCourseDetail_ByPagination")]

        public HttpResponseMessage GetCourseDetail_ByPagination()
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

                CourseList_Pagination_SQL_Params_VM _Params_VM = new CourseList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = courseService.GetBusinessCourseListDetail_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Delete the Course Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/DeleteCourseDetailById")]
        public HttpResponseMessage DeleteCourseDetailById(long id)
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



                // Delete By Id
                var resp = courseService.DeleteBusinessCourseDetail(id);


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
        /// To Change the Course Status Detail by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/ChangeCourseStatus")]
        public HttpResponseMessage ChangeCourseStatus(long id)
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

                var resp = courseService.ChangeStatusCourseDetail(id);

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
        /// get course data by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/GetCourseDataById")]
        public HttpResponseMessage GetCourseDataById(long courseId)
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

                CourseDetail_VM class_VM = new CourseDetail_VM();



                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", courseId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                class_VM = db.Database.SqlQuery<CourseDetail_VM>("exec sp_ManageCourseDetail @id,@userLoginId,@mode", queryParamsGetEvent).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = class_VM;

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
        /// Get Course Datas By Id without authorize
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>

        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/GetCourseDatasById")]
        public HttpResponseMessage GetCourseDatasById(long courseId)
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

                CourseDetail_VM class_VM = new CourseDetail_VM();



                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", courseId),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                class_VM = db.Database.SqlQuery<CourseDetail_VM>("exec sp_ManageCourseDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParamsGetEvent).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = class_VM;

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
        /// To Get Course Detail List For Visitor Panel 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/GetCourseDetail")]
        public HttpResponseMessage GetCourseDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessCourseDetail_VM> course_VMList = courseService.GetCourseDetailList(businessOwnerLoginId);
                List<BusinessCourseDetail_VM> courseListwithCertificate = courseService.GetCourseDetailListWithCertificate(businessOwnerLoginId);
                List<BusinessCourseDetail_VM> courseListwithCertificateAndExam = courseService.GetCourseDetailListWithCertificateAndExam(businessOwnerLoginId);
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    CourseList = course_VMList,
                    CourseListwithCertificate = courseListwithCertificate,
                    CourseListwithCertificateAndExam = courseListwithCertificateAndExam
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

        /// <summary>
        /// To Get Single Course Detail By Course Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin, Staff")]
        [Route("api/Course/GetCourseDetailByCourseId")]
        public HttpResponseMessage GetCourseDetailByCourseId(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessCourseDetail_VM resp = new BusinessCourseDetail_VM();
                resp = courseService.GetBusinessCourseDetail(id); // Pass _BusinessOwnerLoginId as the first parameter

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
        /// To Get Other Course Detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin, Staff")]
        [Route("api/Course/GetOtherCourseDetailByCourseId")]
        public HttpResponseMessage GetOtherCourseDetailByCourseId(long id, long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                //BusinessCourseDetail_VM resp = new BusinessCourseDetail_VM();
                List<BusinessCourseDetail_VM> resp = courseService.GetBusinessOtherCourseDetail(id , businessOwnerLoginId); // Pass _BusinessOwnerLoginId as the first parameter

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
        /// To  Business get Course Booking Detail  For Business Panel 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/GetBusinessCourseBookingDetails")]
        public HttpResponseMessage GetBusinessCourseBookingDetails()
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
                List<CourseBooking_ViewModel> course_VMList = courseService.GetCourseBookingDetail(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = course_VMList;

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
        /// To Get Business Course Booking Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Course/GetBusinessCourseBookingDetailById")]
        public HttpResponseMessage GetBusinessClassesBookingDetailById(long id)
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

                // Get Class-Booking-Detail
                var courseBookingDetail = courseService.GetBusinessCourseBookingDetailById(id, _BusinessOwnerLoginId);

                // Get Class-Detail  
                CourseBooking_ViewModel coursebookingDetail = new CourseBooking_ViewModel();
                coursebookingDetail = courseService.GetBusinessCourseBookingDetail(courseBookingDetail.Id, _BusinessOwnerLoginId);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(courseBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    CourseDetail = coursebookingDetail,
                    CourseBookingDetail = courseBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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
        /// get course details by id , order details and payment details included
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Course/GetCourseBookingDetailsbyId")]
        public HttpResponseMessage GetCourseBookingDetailsbyId(long id)
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

                // Get Course-Booking-Detail
                var courseBookingDetail = courseService.GetCourseListbyId(id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(courseBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    CourseBookingDetail = courseBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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
        /// To Get Course Booking Detail For Visitor Panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Course/GetCourseBookingDetails")]
        public HttpResponseMessage GetCourseBookingDetails()
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
                List<CourseBooking_ViewModel> event_VMList = courseService.GetCourseList(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = event_VMList;

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
        /// Get Business Course Detail List 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Course/GetBusinessCourseDetails")]
        public HttpResponseMessage GetBusinessCourseDetails(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
             
                List<BusinessCourseDetail_VM> course_VMList = courseService.GetCourseCategoryList(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = course_VMList;

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
        /// To Get Business Course Detail List By Search 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Course/GetCourseDetailListBySerach")]
        public HttpResponseMessage GetCourseDetailListBySerach(long id, long businessOwnerLoginId, string name )
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if(name == null)
                {
                    name= string.Empty;
                }

                List<BusinessCourseDetail_VM> course_VMList = courseService.GetCourseSearchDetailList(id, businessOwnerLoginId, name);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = course_VMList;

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
        /// To Get SPORTS Booking Detail For Visitor Panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Course/GetSportsBookingDetails")]
        public HttpResponseMessage GetSportsBookingDetails()
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
                List<TennisBokingDetail_VM> event_VMList = courseService.GetSportsBookingDetails(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = event_VMList;

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
        /// to get course admit card by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Course/GetCourseAdmitCardbyId")]
        public HttpResponseMessage GetCourseAdmitCardbyId(long id)
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

                // Get Course-Booking-Detail
                var courseBookingDetail = courseService.GetCourseAdmitCardServicebyId(id, _LoginID_Exact);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    CourseBookingDetail = courseBookingDetail,
                };

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
    }
}