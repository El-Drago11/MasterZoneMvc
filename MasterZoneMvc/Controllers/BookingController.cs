using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCA.Util;

namespace MasterZoneMvc.Controllers
{
    public class BookingController : Controller
    {
        private MasterZoneDbContext db;
        private BookingService bookingService;
        private EventService eventService;
        private ClassService classService;
        private PlanService planService;
        private string workingKey = ConfigurationManager.AppSettings["workingKey"];
        public string strAccessCode = ConfigurationManager.AppSettings["strAccessCode"];
        public string merchantid = ConfigurationManager.AppSettings["merchantid"];
        public BookingController()
        {
            db = new MasterZoneDbContext();
            bookingService = new BookingService(db);
            eventService = new EventService(db);
            classService = new ClassService(db);
            planService = new PlanService(db);
        }

        private bool ValidateBusinessAdminCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.BusinessAdminCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }

        private string GetLoginIdFromBusinessAdminCookie()
        {
            HttpCookie myCookie_Customer = Request.Cookies[CookieKeyNames.BusinessAdminCookie];
            string _LoginId = null;
            //-- if Cookie not null
            if (myCookie_Customer != null)
            {
                string JWT_Token = myCookie_Customer["UserToken"];

                var stream = JWT_Token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var id = tokenS.Claims.First(claim => claim.Type == "loginid")?.Value;
                _LoginId = id;
            }
            return _LoginId;
        }

        private bool ValidateStudentCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.StudentCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }

        private string GetStudentLoginIdFromStudentCookie()
        {
            HttpCookie myCookie_Customer = Request.Cookies[CookieKeyNames.StudentCookie];
            string _LoginId = null;
            //-- if RestaurantLoginId Cookie not null
            if (myCookie_Customer != null)
            {
                string JWT_Token = myCookie_Customer["UserToken"];

                var stream = JWT_Token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var id = tokenS.Claims.First(claim => claim.Type == "loginid")?.Value;
                _LoginId = id;
            }
            return _LoginId;
        }


        public ActionResult OrderSuccess()
        {
            return View();
        }

        public ActionResult OrderError(string errorTitle, string errorMessage)
        {
            ViewBag.ErrorTitle = errorTitle;
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        public ActionResult AlreadyPurchased()
        {
            return View();
        }


        

        public ActionResult Checkout(long itemId, string itemType)
        {
            if (!ValidateStudentCookie())
            {
                var returnUrl = Url.Action("Checkout", routeValues: new { itemId = itemId, itemType = itemType});
                return RedirectToAction("Login", "Home", new { returnUrl = Server.UrlEncode(returnUrl)});
            }

            long userLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie());
            int IsItemAlreadyPurchased = 0;

            string AlreadyPurchasedMessage = "";

            itemType = itemType.ToLower();
            TrainingService trainingService = new TrainingService(db);
            if(itemType == "event" && eventService.IsAlreadyEventPurchased(itemId, userLoginId))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.EventAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "class")
            {
                IsItemAlreadyPurchased = 0;
                AlreadyPurchasedMessage = Resources.ErrorMessage.ClassAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "plan" && planService.IsAlreadyPlanPurchased(itemId, userLoginId))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.PlanAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "training" && trainingService.IsAlreadyTrainingPurchased(itemId, userLoginId))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.PlanAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "course")
            {
                IsItemAlreadyPurchased = 0;
                AlreadyPurchasedMessage = Resources.ErrorMessage.CourseAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }




          //  ViewBag.PlanTypeId = plantype;
            ViewBag.ItemId = itemId;
            ViewBag.ItemType = itemType;
            ViewBag.IsItemAlreadyPurchased = IsItemAlreadyPurchased;
            ViewBag.AlreadyPurchasedMessage = AlreadyPurchasedMessage;
            return View();
        }

        public ActionResult CheckoutCourse(long itemId, string itemType, long examstatus)
        {
            if (!ValidateStudentCookie())
            {
                var returnUrl = Url.Action("Checkout", routeValues: new { itemId = itemId, itemType = itemType });
                return RedirectToAction("Login", "Home", new { returnUrl = Server.UrlEncode(returnUrl) });
            }

            long userLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie());
            int IsItemAlreadyPurchased = 0;

            string AlreadyPurchasedMessage = "";

            itemType = itemType.ToLower();
            TrainingService trainingService = new TrainingService(db);
            CourseService courseService = new CourseService(db); //added
            if (itemType == "event" && eventService.IsAlreadyEventPurchased(itemId, userLoginId))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.EventAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "class")
            {
                IsItemAlreadyPurchased = 0;
                AlreadyPurchasedMessage = Resources.ErrorMessage.ClassAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "plan" && planService.IsAlreadyPlanPurchased(itemId, userLoginId))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.PlanAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "training" && trainingService.IsAlreadyTrainingPurchased(itemId, userLoginId))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.PlanAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }
            else if (itemType == "course" && courseService.IsAlreadyCoursePurchased(itemId, userLoginId, "")) //added
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.CourseAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }

            ViewBag.ItemId = itemId;
            ViewBag.ItemType = itemType;
            ViewBag.ExamStatus = examstatus;
            ViewBag.IsItemAlreadyPurchased = IsItemAlreadyPurchased;
            ViewBag.AlreadyPurchasedMessage = AlreadyPurchasedMessage;
            return View();
        }

        public ActionResult BookPackage(long itemId, int paymentMode, long couponId, decimal totalAmountPaid, int planType)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login", "Home");
            }
            // if cash payment
            if (paymentMode == 1)
            {
                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 0,
                    TransactionID = "",
                    PlanType = planType,
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId

                };

                ServiceResponse_VM bookingResponse = bookingService.CreatePlanBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                CCACrypto ccaCrypto = new CCACrypto();
                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 1,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = "Online Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = (long)couponId,

                    merchant_id = merchantid,
                    redirect_url = $"http://localhost:51713/Booking/OnlineBookPackageOrder?itemId={itemId}",
                    cancel_url = "http://localhost:51713/Booking/OrderError",
                    order_id = GetStudentLoginIdFromStudentCookie(),
                    amount = totalAmountPaid.ToString(),
                    currency = "INR",
                    offer_code = couponId.ToString(),
                };

                string ccaRequest = "";
                foreach (var property in typeof(CreatePlanBooking_VM).GetProperties())
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(createPlanBooking_VM);
                    if (propertyValue != null && !propertyName.StartsWith("_"))
                    {
                        ccaRequest += propertyName + "=" + propertyValue.ToString() + "&";
                    }
                }

                string strEncRequest = ccaCrypto.Encrypt(ccaRequest.TrimEnd('&'), workingKey);

                string ccaAvenueUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction";
                string redirectUrl = $"{ccaAvenueUrl}&encRequest={HttpUtility.UrlEncode(strEncRequest)}&access_code={strAccessCode}";

                return Redirect(redirectUrl);
            }

            return View();
        }

        public ActionResult OnlineBookPackageOrder(string itemId, string encResp)
        {

            // Decrypt the response received from ccAvenue
            CCACrypto ccaCrypto = new CCACrypto();
            string decResp = ccaCrypto.Decrypt(encResp, workingKey);

            // Parse the decrypted response
            NameValueCollection Params = HttpUtility.ParseQueryString(decResp);

            string orderId = Params["order_id"];
            string paymentStatus = Params["order_status"];
            string EventId = itemId;
            string TotalAmountPaid = Params["amount"];
            string CouponId = Params["offer_code"];
            string failure_message = Params["failure_message"];
            string status_message = Params["status_message"];
            string payment_mode = Params["payment_mode"];

            //handling payment success
            if (paymentStatus == "Success")
            {
                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = Convert.ToInt64(EventId),
                    UserLoginId = Convert.ToInt64(orderId),
                    OnlinePayment = 1,
                    TransactionID = orderId,
                    PaymentResponseStatus = paymentStatus,
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = payment_mode,
                    TotalAmountPaid = Convert.ToInt64(TotalAmountPaid),
                    CouponId = Convert.ToInt64(CouponId)
                };

                ServiceResponse_VM bookingResponse = bookingService.CreatePlanBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }

                return RedirectToAction("OrderSuccess");
            }

            return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = status_message + failure_message });
        }


        public ActionResult BookEvent(long itemId, int paymentMode, long? couponId, decimal totalAmountPaid)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateEventBooking_VM createEventBooking_VM = new CreateEventBooking_VM()
                {
                    EventId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = (long)couponId
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateEventBooking(createEventBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                CCACrypto ccaCrypto = new CCACrypto();
                CreateEventBooking_VM createEventBooking_VM = new CreateEventBooking_VM()
                {
                    EventId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 1,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = "Online Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = (long)couponId,

                    merchant_id = merchantid,
                    redirect_url = $"http://localhost:51713/Booking/OnlineBookEventOrder?itemId={itemId}",
                    cancel_url = "http://localhost:51713/Booking/OrderError",
                    order_id = GetStudentLoginIdFromStudentCookie(),
                    amount = totalAmountPaid.ToString(),
                    currency = "INR",
                    offer_code = couponId.ToString(),
                };

                string ccaRequest = "";
                foreach (var property in typeof(CreateEventBooking_VM).GetProperties())
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(createEventBooking_VM);
                    if (propertyValue != null && !propertyName.StartsWith("_"))
                    {
                        ccaRequest += propertyName + "=" + propertyValue.ToString() + "&";
                    }
                }

                string strEncRequest = ccaCrypto.Encrypt(ccaRequest.TrimEnd('&'), workingKey);

                string ccaAvenueUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction";
                string redirectUrl = $"{ccaAvenueUrl}&encRequest={HttpUtility.UrlEncode(strEncRequest)}&access_code={strAccessCode}";

                return Redirect(redirectUrl);

            }

            return View();
        }

        public ActionResult OnlineBookEventOrder(string itemId, string encResp)
        {

            // Decrypt the response received from ccAvenue
            CCACrypto ccaCrypto = new CCACrypto();
            string decResp = ccaCrypto.Decrypt(encResp, workingKey);

            // Parse the decrypted response
            NameValueCollection Params = HttpUtility.ParseQueryString(decResp);

            string orderId = Params["order_id"];
            string paymentStatus = Params["order_status"];
            string EventId = itemId;
            string TotalAmountPaid = Params["amount"];
            string CouponId = Params["offer_code"];
            string failure_message = Params["failure_message"];
            string status_message = Params["status_message"];
            string payment_mode = Params["payment_mode"];

            //handling payment success
            if (paymentStatus == "Success")
            {
                CreateEventBooking_VM createEventBooking_VM = new CreateEventBooking_VM()
                {
                    EventId = Convert.ToInt64(EventId),
                    UserLoginId = Convert.ToInt64(orderId),
                    OnlinePayment = 1,
                    TransactionID = orderId,
                    PaymentResponseStatus = paymentStatus,
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = payment_mode,
                    TotalAmountPaid = Convert.ToInt64(TotalAmountPaid),
                    CouponId = Convert.ToInt64(CouponId)
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateEventBooking(createEventBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }

                return RedirectToAction("OrderSuccess");
            }

            return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = status_message + failure_message });
        }

        public ActionResult BookClass(long itemId, int paymentMode, long couponId, decimal totalAmountPaid, string joinClassDate, int batchId)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateClassBooking_VM createClassBooking_VM = new CreateClassBooking_VM()
                {
                    ClassId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId,
                    JoinClassDate = joinClassDate,
                    BatchId = batchId,
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateClassBooking(createClassBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                if (totalAmountPaid == 0)
                {
                    totalAmountPaid = 1;
                }
                CCACrypto ccaCrypto = new CCACrypto();
                CreateClassBooking_VM createClassBooking_VM = new CreateClassBooking_VM()
                {
                    ClassId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 1,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = "Online Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = (long)couponId,

                    merchant_id = merchantid,
                    redirect_url = $"http://localhost:51713/Booking/OnlineBookClassOrder?itemId={itemId}",
                    cancel_url = "http://localhost:51713/Booking/OrderError",
                    order_id = GetStudentLoginIdFromStudentCookie(),
                    amount = totalAmountPaid.ToString(),
                    currency = "INR",
                    offer_code = couponId.ToString(),
                };

                string ccaRequest = "";
                foreach (var property in typeof(CreateClassBooking_VM).GetProperties())
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(createClassBooking_VM);
                    if (propertyValue != null && !propertyName.StartsWith("_"))
                    {
                        ccaRequest += propertyName + "=" + propertyValue.ToString() + "&";
                    }
                }

                string strEncRequest = ccaCrypto.Encrypt(ccaRequest.TrimEnd('&'), workingKey);

                string ccaAvenueUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction";
                string redirectUrl = $"{ccaAvenueUrl}&encRequest={HttpUtility.UrlEncode(strEncRequest)}&access_code={strAccessCode}";

                return Redirect(redirectUrl);
            }

            return View();
        }

        public ActionResult OnlineBookClassOrder(string itemId, string encResp)
        {

            // Decrypt the response received from ccAvenue
            CCACrypto ccaCrypto = new CCACrypto();
            string decResp = ccaCrypto.Decrypt(encResp, workingKey);

            // Parse the decrypted response
            NameValueCollection Params = HttpUtility.ParseQueryString(decResp);

            string orderId = Params["order_id"];
            string paymentStatus = Params["order_status"];
            string EventId = itemId;
            string TotalAmountPaid = Params["amount"];
            string CouponId = Params["offer_code"];
            string failure_message = Params["failure_message"];
            string status_message = Params["status_message"];
            string payment_mode = Params["payment_mode"];

            //handling payment success
            if (paymentStatus == "Success")
            {
                CreateClassBooking_VM createClassBooking_VM = new CreateClassBooking_VM()
                {
                    ClassId = Convert.ToInt64(EventId),
                    UserLoginId = Convert.ToInt64(orderId),
                    OnlinePayment = 1,
                    TransactionID = orderId,
                    PaymentResponseStatus = paymentStatus,
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = payment_mode,
                    TotalAmountPaid = Convert.ToInt64(TotalAmountPaid),
                    CouponId = Convert.ToInt64(CouponId)
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateClassBooking(createClassBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }

                return RedirectToAction("OrderSuccess");
            }

            return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = status_message + failure_message });
        }



        public ActionResult BookTraining(long itemId, int paymentMode, long couponId, decimal totalAmountPaid)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateTrainingBooking_VM createPlanBooking_VM = new CreateTrainingBooking_VM()
                {
                    TrainingId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateTrainingBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                CCACrypto ccaCrypto = new CCACrypto();
                CreateTrainingBooking_VM createPlanBooking_VM = new CreateTrainingBooking_VM()
                {
                    TrainingId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 1,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = "Online Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = (long)couponId,

                    merchant_id = merchantid,
                    redirect_url = $"http://localhost:51713/Booking/OnlineBookTrainingOrder?itemId={itemId}",
                    cancel_url = "http://localhost:51713/Booking/OrderError",
                    order_id = GetStudentLoginIdFromStudentCookie(),
                    amount = totalAmountPaid.ToString(),
                    currency = "INR",
                    offer_code = couponId.ToString(),
                };

                string ccaRequest = "";
                foreach (var property in typeof(CreateTrainingBooking_VM).GetProperties())
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(createPlanBooking_VM);
                    if (propertyValue != null && !propertyName.StartsWith("_"))
                    {
                        ccaRequest += propertyName + "=" + propertyValue.ToString() + "&";
                    }
                }

                string strEncRequest = ccaCrypto.Encrypt(ccaRequest.TrimEnd('&'), workingKey);

                string ccaAvenueUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction";
                string redirectUrl = $"{ccaAvenueUrl}&encRequest={HttpUtility.UrlEncode(strEncRequest)}&access_code={strAccessCode}";

                return Redirect(redirectUrl);
            }

            return View();
        }
        public ActionResult OnlineBookTrainingOrder(string itemId, string encResp)
        {

            // Decrypt the response received from ccAvenue
            CCACrypto ccaCrypto = new CCACrypto();
            string decResp = ccaCrypto.Decrypt(encResp, workingKey);

            // Parse the decrypted response
            NameValueCollection Params = HttpUtility.ParseQueryString(decResp);

            string orderId = Params["order_id"];
            string paymentStatus = Params["order_status"];
            string EventId = itemId;
            string TotalAmountPaid = Params["amount"];
            string CouponId = Params["offer_code"];
            string failure_message = Params["failure_message"];
            string status_message = Params["status_message"];
            string payment_mode = Params["payment_mode"];

            //handling payment success
            if (paymentStatus == "Success")
            {
                CreateTrainingBooking_VM createPlanBooking_VM = new CreateTrainingBooking_VM()
                {
                    TrainingId = Convert.ToInt64(EventId),
                    UserLoginId = Convert.ToInt64(orderId),
                    OnlinePayment = 1,
                    TransactionID = orderId,
                    PaymentResponseStatus = paymentStatus,
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = payment_mode,
                    TotalAmountPaid = Convert.ToInt64(TotalAmountPaid),
                    CouponId = Convert.ToInt64(CouponId)
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateTrainingBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }

                return RedirectToAction("OrderSuccess");
            }

            return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = status_message + failure_message });
        }

        /// <summary>
        /// Class QR-Code verification Page.
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult ClassQRCodeVerfication(string classId, string orderId)
        {
            string ClassID = EDClass.Decrypt(classId);
            string OrderId = EDClass.Decrypt(orderId);
            return Content("OrderId  " + OrderId + "  ClassID  " + ClassID);
        }



        public ActionResult BookCourse(long itemId, int paymentMode, long couponId, decimal totalAmountPaid)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login", "Home");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateCourseBooking_VM createCourseBooking_VM = new CreateCourseBooking_VM()
                {
                    CourseId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId,


                };

                ServiceResponse_VM bookingResponse = bookingService.CreateCourseBooking(createCourseBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                CCACrypto ccaCrypto = new CCACrypto();
                CreateCourseBooking_VM createCourseBooking_VM = new CreateCourseBooking_VM()
                {
                    CourseId = itemId,
                    UserLoginId = Convert.ToInt64(GetStudentLoginIdFromStudentCookie()),
                    OnlinePayment = 1,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = "Online Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = (long)couponId,

                    merchant_id = merchantid,
                    redirect_url = $"http://localhost:51713/Booking/OnlineBookCourseOrder?itemId={itemId}",
                    cancel_url = "http://localhost:51713/Booking/OrderError",
                    order_id = GetStudentLoginIdFromStudentCookie(),
                    amount = totalAmountPaid.ToString(),
                    currency = "INR",
                    offer_code = couponId.ToString(),
                };

                string ccaRequest = "";
                foreach (var property in typeof(CreateCourseBooking_VM).GetProperties())
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(createCourseBooking_VM);
                    if (propertyValue != null && !propertyName.StartsWith("_"))
                    {
                        ccaRequest += propertyName + "=" + propertyValue.ToString() + "&";
                    }
                }

                string strEncRequest = ccaCrypto.Encrypt(ccaRequest.TrimEnd('&'), workingKey);

                string ccaAvenueUrl = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction";
                string redirectUrl = $"{ccaAvenueUrl}&encRequest={HttpUtility.UrlEncode(strEncRequest)}&access_code={strAccessCode}";

                return Redirect(redirectUrl);
            }

            return View();
        }

        public ActionResult OnlineBookCourseOrder(string itemId, string encResp)
        {

            // Decrypt the response received from ccAvenue
            CCACrypto ccaCrypto = new CCACrypto();
            string decResp = ccaCrypto.Decrypt(encResp, workingKey);

            // Parse the decrypted response
            NameValueCollection Params = HttpUtility.ParseQueryString(decResp);

            string orderId = Params["order_id"];
            string paymentStatus = Params["order_status"];
            string EventId = itemId;
            string TotalAmountPaid = Params["amount"];
            string CouponId = Params["offer_code"];
            string failure_message = Params["failure_message"];
            string status_message = Params["status_message"];
            string payment_mode = Params["payment_mode"];

            //handling payment success
            if (paymentStatus == "Success")
            {
                CreateCourseBooking_VM createCourseBooking_VM = new CreateCourseBooking_VM()
                {
                    CourseId = Convert.ToInt64(EventId),
                    UserLoginId = Convert.ToInt64(orderId),
                    OnlinePayment = 1,
                    TransactionID = orderId,
                    PaymentResponseStatus = paymentStatus,
                    PaymentProvider = "ccAvenue",
                    PaymentMethod = "Online",
                    PaymentDescription = payment_mode,
                    TotalAmountPaid = Convert.ToInt64(TotalAmountPaid),
                    CouponId = Convert.ToInt64(CouponId)
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateCourseBooking(createCourseBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("OrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }

                return RedirectToAction("OrderSuccess");
            }

            return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = status_message + failure_message });
        }
    }
}