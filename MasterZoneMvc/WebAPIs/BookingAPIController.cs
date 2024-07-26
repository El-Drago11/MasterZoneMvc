using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class BookingAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private BookingService bookingService;
        private StoredProcedureRepository storedProcedureRepository;

        public BookingAPIController()
        {
            db = new MasterZoneDbContext();
            bookingService = new BookingService(db);
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
        /// Verify if Event Can be purchased or not.
        /// Validates Event, and coupons
        /// </summary>
        /// <param name="eventBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Booking/VerifyEventCanBePurchased")]
        public HttpResponseMessage VerifyEventCanBePurchased(CheckoutItem_Params_VM eventBooking_Params_VM)
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

                CreateEventBooking_VM createEventBooking_VM = new CreateEventBooking_VM()
                {
                    EventId = eventBooking_Params_VM.itemId,
                    UserLoginId = _LoginID_Exact,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = eventBooking_Params_VM.totalAmountPaid,
                    CouponId = eventBooking_Params_VM.couponId
                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyEventCanBeBooked(createEventBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if Plan Can be purchased or not.
        /// Validates Plan, and coupons
        /// </summary>
        /// <param name="planBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Booking/VerifyPlanCanBePurchased")]
        public HttpResponseMessage VerifyPlanCanBePurchased(CheckoutItem_Params_VM planBooking_Params_VM)
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

                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = planBooking_Params_VM.itemId,
                    UserLoginId = _LoginID_Exact,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = planBooking_Params_VM.totalAmountPaid,
                    CouponId = planBooking_Params_VM.couponId,
                    PlanType = planBooking_Params_VM.PlanType

                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyPlanCanBeBooked(createPlanBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if Class Can be purchased or not.
        /// Validates Class, and coupons
        /// </summary>
        /// <param name="classBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Booking/VerifyClassCanBePurchased")]
        public HttpResponseMessage VerifyClassCanBePurchased(CheckOutClass_Parama_VM classBooking_Params_VM)
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

                CreateClassBooking_VM createClassBooking_VM = new CreateClassBooking_VM()
                {
                    ClassId = classBooking_Params_VM.itemId,
                    UserLoginId = _LoginID_Exact,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = classBooking_Params_VM.totalAmountPaid,
                    CouponId = classBooking_Params_VM.couponId,
                    JoinClassDate = classBooking_Params_VM.joinClassDate,
                    BatchId = classBooking_Params_VM.BatchId
                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyClassCanBeBooked(createClassBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if Training Can be purchased or not.
        /// Validates Training, and coupons
        /// </summary>
        /// <param name="trainingBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Booking/VerifyTrainingCanBePurchased")]
        public HttpResponseMessage VerifyTrainingCanBePurchased(CheckoutItem_Params_VM trainingBooking_Params_VM)
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

                CreateTrainingBooking_VM createTrainingBooking_VM = new CreateTrainingBooking_VM()
                {
                    TrainingId = trainingBooking_Params_VM.itemId,
                    UserLoginId = _LoginID_Exact,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = trainingBooking_Params_VM.totalAmountPaid,
                    CouponId = trainingBooking_Params_VM.couponId

                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyTrainingCanBeBooked(createTrainingBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Get All Booking Details By Student-UserLoginId
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Booking/GetAllBooking")]
        public HttpResponseMessage GetAllBookingList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<UserBookingList_VM> response = bookingService.GetAllBookingList(_LoginId, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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
        /// Verify if License Can be purchased or not.
        /// Validates License, and coupons
        /// </summary>
        /// <param name="licenseBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/VerifyLicenseCanBePurchased")]
        public HttpResponseMessage VerifyLicenseCanBePurchased(CheckoutLicense_Params_VM licenseBooking_Params_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                #region Validate Request data and return if not valid -------------------
                Error_VM validData = new Error_VM() { Valid = true };
                if (licenseBooking_Params_VM.itemId <= 0)
                {
                    validData.Valid = false;
                    validData.Message = "Item Id Required!";
                }
                else if (licenseBooking_Params_VM.paymentMode <= 0)
                {
                    validData.Valid = false;
                    validData.Message = "Payment Mode is Required!";
                }

                if (!validData.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = validData.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                #endregion ----------------------------

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                CreateLicenseBooking_VM createLicenseBooking_VM = new CreateLicenseBooking_VM()
                {
                    LicenseId = licenseBooking_Params_VM.itemId,
                    UserLoginId = _BusinessOwnerLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = licenseBooking_Params_VM.totalAmountPaid,
                    CouponId = licenseBooking_Params_VM.couponId,
                    Quantity = licenseBooking_Params_VM.quantity
                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyLicenseCanBeBooked(createLicenseBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if License Can be purchased or not.
        /// Validates License
        /// </summary>
        /// <param name="licenseBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/BookLicense")]
        public HttpResponseMessage BookLicense(CheckoutLicense_Params_VM licenseBooking_Params_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                #region Validate Request data and return if not valid -------------------
                Error_VM validData = new Error_VM() { Valid = true };
                if (licenseBooking_Params_VM.itemId <= 0)
                {
                    validData.Valid = false;
                    validData.Message = "Item Id Required!";
                }
                else if (licenseBooking_Params_VM.paymentMode <= 0)
                {
                    validData.Valid = false;
                    validData.Message = "Payment Mode is Required!";
                }

                if (!validData.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = validData.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                #endregion ----------------------------

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();

                // if cash payment
                if (licenseBooking_Params_VM.paymentMode == 1)
                {
                    CreateLicenseBooking_VM createLicenseBooking_VM = new CreateLicenseBooking_VM()
                    {
                        LicenseId = licenseBooking_Params_VM.itemId,
                        UserLoginId = _BusinessOwnerLoginId,
                        OnlinePayment = 0,
                        TransactionID = "",
                        PaymentResponseStatus = "",
                        PaymentProvider = "ManualPayment",
                        PaymentMethod = "Cash",
                        PaymentDescription = "Cash Payment",
                        TotalAmountPaid = licenseBooking_Params_VM.totalAmountPaid,
                        CouponId = 0, //licenseBooking_Params_VM.couponId
                        Quantity = licenseBooking_Params_VM.quantity
                    };

                    serviceResponse_VM = bookingService.CreateLicenseBooking(createLicenseBooking_VM);
                }
                else
                {
                    // online payment
                    serviceResponse_VM.Status = 2;
                    serviceResponse_VM.Message = "Redirect to Online Payment!";
                }

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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


        #region Booking for Student From Business Owner -----------------------

        /// <summary>
        /// Verify if Event Can be purchased or not.
        /// Validates Event, and coupons
        /// </summary>
        /// <param name="eventBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/VerifyStudentEventCanBePurchasedFromBusiness")]
        public HttpResponseMessage VerifyStudentEventCanBePurchasedFromBusiness(CheckoutItem_Params_VM eventBooking_Params_VM)
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

                if (eventBooking_Params_VM.StudentLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage + " Student required!";
                    return Request.CreateResponse(HttpStatusCode.OK, validateResponse.ApiResponse_VM);
                }

                CreateEventBooking_VM createEventBooking_VM = new CreateEventBooking_VM()
                {
                    EventId = eventBooking_Params_VM.itemId,
                    UserLoginId = eventBooking_Params_VM.StudentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = eventBooking_Params_VM.totalAmountPaid,
                    CouponId = eventBooking_Params_VM.couponId
                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyEventCanBeBooked(createEventBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if Plan Can be purchased or not.
        /// Validates Plan, and coupons
        /// </summary>
        /// <param name="planBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/VerifyStudentPlanCanBePurchasedFromBusiness")]
        public HttpResponseMessage VerifyStudentPlanCanBePurchasedFromBusiness(CheckoutItem_Params_VM planBooking_Params_VM)
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

                if (planBooking_Params_VM.StudentLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage + " Student required!";
                    return Request.CreateResponse(HttpStatusCode.OK, validateResponse.ApiResponse_VM);
                }

                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = planBooking_Params_VM.itemId,
                    UserLoginId = planBooking_Params_VM.StudentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = planBooking_Params_VM.totalAmountPaid,
                    CouponId = planBooking_Params_VM.couponId

                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyPlanCanBeBooked(createPlanBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if Class Can be purchased or not.
        /// Validates Class, and coupons
        /// </summary>
        /// <param name="classBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/VerifyStudentClassCanBePurchasedFromBusiness")]
        public HttpResponseMessage VerifyStudentClassCanBePurchasedFromBusiness(CheckOutClass_Parama_VM classBooking_Params_VM)
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

                if(classBooking_Params_VM.StudentLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage + " Student required!";
                    return Request.CreateResponse(HttpStatusCode.OK, validateResponse.ApiResponse_VM);
                }

                CreateClassBooking_VM createClassBooking_VM = new CreateClassBooking_VM()
                {
                    ClassId = classBooking_Params_VM.itemId,
                    UserLoginId = classBooking_Params_VM.StudentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = classBooking_Params_VM.totalAmountPaid,
                    CouponId = classBooking_Params_VM.couponId,
                    JoinClassDate = classBooking_Params_VM.joinClassDate,
                    BatchId = classBooking_Params_VM.BatchId
                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyClassCanBeBooked(createClassBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// Verify if Training Can be purchased or not.
        /// Validates Training, and coupons
        /// </summary>
        /// <param name="trainingBooking_Params_VM"></param>
        /// <returns>
        /// API Resoponse status = 1 (+ve) if can be purchased, and status = -1 or (-ve) value if some validation error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/VerifyStudentTrainingCanBePurchasedFromBusiness")]
        public HttpResponseMessage VerifyStudentTrainingCanBePurchasedFromBusiness(CheckoutItem_Params_VM trainingBooking_Params_VM)
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

                if (trainingBooking_Params_VM.StudentLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage + " Student required!";
                    return Request.CreateResponse(HttpStatusCode.OK, validateResponse.ApiResponse_VM);
                }

                CreateTrainingBooking_VM createPlanBooking_VM = new CreateTrainingBooking_VM()
                {
                    TrainingId = trainingBooking_Params_VM.itemId,
                    UserLoginId = trainingBooking_Params_VM.StudentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = trainingBooking_Params_VM.totalAmountPaid,
                    CouponId = trainingBooking_Params_VM.couponId

                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyTrainingCanBeBooked(createPlanBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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


        /// <summary>
        /// Verify if course Can be purchased or not.
        /// Validates course, and coupons
        /// </summary>
        /// <param name="courseBooking_Params_VM"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Booking/VerifyCourseCanBePurchased")]
        public HttpResponseMessage VerifyCourseCanBePurchased(CheckOutCourse_Parama_VM courseBooking_Params_VM)
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

                CreateCourseBooking_VM createClassBooking_VM = new CreateCourseBooking_VM()
                {
                    CourseId = courseBooking_Params_VM.itemId,
                    UserLoginId = _LoginID_Exact,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = courseBooking_Params_VM.totalAmountPaid,
                    CouponId = courseBooking_Params_VM.couponId,
                    //JoinCourseDate = courseBooking_Params_VM.joinCourseDate,

                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyCourseCanBeBooked(createClassBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// To Get Sports Booking Detail By businessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Booking/GetBusinessSportsBookingDetail_PPCMeta")]
        public HttpResponseMessage GetBusinessSportsBookingDetail_PPCMeta(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                SportsBookingCheaqueDetail_VM resp = new SportsBookingCheaqueDetail_VM();
                resp = bookingService.GetSportsBooingCheaqueDetail_Get(businessOwnerLoginId);

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
        /// To Add/Update Sports Booking Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpPost]
        //   [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/AddUpdateSportsBookingDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateSportsBookingDetail_PPCMeta(long businessOwnerLoginId)
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

                SportsBookingCheaque_ViewModel sportsBookingCheaque_ViewModel = new SportsBookingCheaque_ViewModel();
                sportsBookingCheaque_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                sportsBookingCheaque_ViewModel.Name = HttpRequest.Params["Name"].Trim();
                sportsBookingCheaque_ViewModel.SurName = HttpRequest.Params["SurName"].Trim();
                sportsBookingCheaque_ViewModel.Email = HttpRequest.Params["Email"].Trim();
                sportsBookingCheaque_ViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                sportsBookingCheaque_ViewModel.BookedId = HttpRequest.Params["BookedId"].Trim();
                sportsBookingCheaque_ViewModel.Department = HttpRequest.Params["Department"].Trim();
                sportsBookingCheaque_ViewModel.Apartment = HttpRequest.Params["Apartment"].Trim();
                sportsBookingCheaque_ViewModel.HouseNumber = HttpRequest.Params["HouseNumber"].Trim();
                sportsBookingCheaque_ViewModel.Message = HttpRequest.Params["Message"].Trim();
                sportsBookingCheaque_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                sportsBookingCheaque_ViewModel.RoomTime = HttpRequest.Params["RoomTme"];
                sportsBookingCheaque_ViewModel.RoomService = HttpRequest.Params["RoomService"];
                sportsBookingCheaque_ViewModel.SelectDate = HttpRequest.Params["SelectDate"];
                sportsBookingCheaque_ViewModel.TennisTitle = HttpRequest.Params["TennisTitle"];
                sportsBookingCheaque_ViewModel.TennisImage = HttpRequest.Params["TennisImage"];
                sportsBookingCheaque_ViewModel.PlayerCount = Convert.ToInt32(HttpRequest.Params["PlayerCount"]);
                sportsBookingCheaque_ViewModel.Price = Convert.ToInt32(HttpRequest.Params["Price"]);
                sportsBookingCheaque_ViewModel.Request = Convert.ToInt32(HttpRequest.Params["Request"]);
                sportsBookingCheaque_ViewModel.SlotId = Convert.ToInt32(HttpRequest.Params["SlotId"]);
                // Validate information passed
                Error_VM error_VM = sportsBookingCheaque_ViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdateSportsBookingDetail_Get<SPResponseViewModel>(new SP_InsertUpdateSportsBookingCheaque_Param_VM
                {
                    Id = sportsBookingCheaque_ViewModel.Id,
                    BusinessOwnerLoginId = businessOwnerLoginId,
                    UserLoginId = _LoginID_Exact,
                    Name = sportsBookingCheaque_ViewModel.Name,
                    SurName = sportsBookingCheaque_ViewModel.SurName,
                    Email = sportsBookingCheaque_ViewModel.Email,
                    PhoneNumber = sportsBookingCheaque_ViewModel.PhoneNumber,
                    Department = sportsBookingCheaque_ViewModel.Department,
                    Apartment = sportsBookingCheaque_ViewModel.Apartment,
                    BookedId = sportsBookingCheaque_ViewModel.BookedId,
                    HouseNumber = sportsBookingCheaque_ViewModel.HouseNumber,
                    Message = sportsBookingCheaque_ViewModel.Message,
                    SubmittedByLoginId = businessOwnerLoginId,
                    TennisTitle = sportsBookingCheaque_ViewModel.TennisTitle,
                    TennisImage = sportsBookingCheaque_ViewModel.TennisImage,
                    RoomTime = sportsBookingCheaque_ViewModel.RoomTime,
                    SelectDate = sportsBookingCheaque_ViewModel.SelectDate,
                    RoomService = sportsBookingCheaque_ViewModel.RoomService,
                    PlayerCount = sportsBookingCheaque_ViewModel.PlayerCount,
                    Request = sportsBookingCheaque_ViewModel.Request,
                    Price = sportsBookingCheaque_ViewModel.Price,
                    SlotId = sportsBookingCheaque_ViewModel.SlotId,

                    Mode = sportsBookingCheaque_ViewModel.Mode
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
        ///  To add Business Payment Booking Detail by BusinessOwnerLoginId 
        /// </summary>
        /// <param name="planBooking_Params_VM"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/VerifyPlanCanBePurchasedByBusiness")]
        public HttpResponseMessage VerifyPlanCanBePurchasedByBusiness(CheckoutItem_Params_VM planBooking_Params_VM)
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


                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = planBooking_Params_VM.itemId,
                    UserLoginId = _BusinessOwnerLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "",
                    PaymentMethod = "",
                    PaymentDescription = "",
                    TotalAmountPaid = planBooking_Params_VM.totalAmountPaid,
                    CouponId = 0

                };

                ServiceResponse_VM serviceResponse_VM = bookingService.VerifyBusinessPlanCanBeBooked(createPlanBooking_VM);

                // API Response
                apiResponse.status = serviceResponse_VM.Status;
                apiResponse.message = serviceResponse_VM.Message;
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
        /// To Get Current Package Detail By businessOwnerLoginId 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/GetBusinessCurrentPackageBookingDetail")]
        public HttpResponseMessage GetBusinessCurrentPackageBookingDetail()
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

                MainPlanBooking_VM resp = new MainPlanBooking_VM();
                resp = bookingService.GetCurrentMainPackageDetail(_BusinessOwnerLoginId);

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
        /// to change the status of request in tennis booking request 
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Booking/UpdateSportsBookingRequest")]

        public HttpResponseMessage UpdateSportsBookingRequest()
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

                SportsBookingCheaque_ViewModel sportsBookingCheaque_ViewModel = new SportsBookingCheaque_ViewModel();
                sportsBookingCheaque_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                sportsBookingCheaque_ViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                sportsBookingCheaque_ViewModel.Request = Convert.ToInt32(HttpRequest.Params["Request"]);
                // Validate information passed


                var resp = storedProcedureRepository.SP_InsertUpdateSportsBookingDetail_Get<SPResponseViewModel>(new SP_InsertUpdateSportsBookingCheaque_Param_VM
                {
                    Id = sportsBookingCheaque_ViewModel.Id,
                    BusinessOwnerLoginId = _LoginID_Exact,
                    Request = sportsBookingCheaque_ViewModel.Request,
                    Mode = sportsBookingCheaque_ViewModel.Mode
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


      
    }
}