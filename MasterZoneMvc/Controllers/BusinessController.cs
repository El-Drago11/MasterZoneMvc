using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MasterZoneMvc.Models;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.Common;
using System.Data.SqlClient;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Services;
using System.IdentityModel.Tokens.Jwt;
using MasterZoneMvc.Common.Helpers;
using System.IO;
using MasterZoneMvc.Common.Filters;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System.Net;

namespace MasterZoneMvc.Controllers
{
    public class BusinessController : Controller
    {
        private MasterZoneDbContext db;
        private BusinessOwnerService businessOwnerService;
        private LoginService loginService;
        private CookieHelper cookieHelper;
        private BookingService bookingService;

        public BusinessController()
        {
            db = new MasterZoneDbContext();
            businessOwnerService = new BusinessOwnerService(db);
            loginService = new LoginService(db);
            cookieHelper = new CookieHelper();
            bookingService = new BookingService(db);
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
        private void SetSidebarCookieInfo(string _val)
        {
            CookieHelper cookieHelper = new CookieHelper();
            cookieHelper.ResetSidebarCookie(Response, _val);
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

        #region private members 
        private bool ValidateStaffCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.StaffCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }

        private string GetStaffLoginIdFromStaffCookie()
        {
            HttpCookie myCookie_Customer = Request.Cookies[CookieKeyNames.StaffCookie];
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
        #endregion

        public JsonResult GetSidebarCookieDetail()
        {
            string _SelectedLink = "";
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.SidebarCookie];

            if (myCookie != null)
            {
                _SelectedLink = myCookie["SelectedLink"].ToString();
            }
            return Json(_SelectedLink, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessAdminToken()
        {
            string _Token = "";
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.BusinessAdminCookie];

            if (myCookie != null)
            {
                _Token = myCookie["UserToken"].ToString();
            }
            return Json(_Token, JsonRequestBehavior.AllowGet);
        }




        //Get staff token 
        public JsonResult GetStaffToken()
        {
            string _Token = "";
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.StaffCookie];

            if (myCookie != null)
            {
                _Token = myCookie["UserToken"].ToString();
            }
            return Json(_Token, JsonRequestBehavior.AllowGet);
        }

        // GET: Business
        public ActionResult Index()
        {
            if (ValidateBusinessAdminCookie())
            {
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            if (ValidateBusinessAdminCookie())
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }


    


        [HttpPost]
        public ActionResult Login(LoginViewModel objLoginModel)
        {
            try
            {
                if (objLoginModel == null || String.IsNullOrEmpty(objLoginModel.MasterId) || String.IsNullOrEmpty(objLoginModel.Password))
                {
                    ViewBag.Message = "Please enter required fields!";
                    return View(objLoginModel);
                }

                #region Business login, validate if success and claim user login ---------------------------------------

                LoginService_VM loginService_VM = new LoginService_VM()
                {
                    Email = "",
                    Password = objLoginModel.Password,
                    Mode = 2,
                    SocialLoginId = "",
                    MasterId = objLoginModel.MasterId,
                };
                var user = loginService.GetLoginValidationInformation(loginService_VM);


                if (user == null)
                {
                    //Add logic here to display some message to user    
                    ViewBag.Message = "Invalid Credential";
                    return View(objLoginModel);
                }
                else if (user.Status != 1)
                {
                    //Add logic here to display some message to user    
                    ViewBag.Message = "Your Account is Inactive";
                    return View(objLoginModel);
                }
                else
                {
                    // Remove/Expire Login Cookies
                    CookieHelper cookieHelper = new CookieHelper();
                    cookieHelper.RemoveOrExpireLoginCookies(Response);

                    TokenGenerator tokenGenerator = new TokenGenerator();
                    var _JWT_User_Token = tokenGenerator.Create_JWT(user.Id, user.RoleName, 0, user.BusinessOwnerLoginId);

                    //Create Admin login cookie
                    var cookieName = (user.RoleName == "BusinessAdmin") ? CookieKeyNames.BusinessAdminCookie : CookieKeyNames.StaffCookie;
                    HttpCookie myCookie = new HttpCookie(cookieName);
                    myCookie["UserToken"] = _JWT_User_Token;
                    myCookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(myCookie);

                    if (user.RoleName == "SuperAdmin")
                    {
                        return RedirectToAction("Dashboard", "SuperAdmin");
                    }
                    else if (user.RoleName == "SubAdmin")
                    {
                        return RedirectToAction("Dashboard", "SubAdmin");
                    }
                    else if (user.RoleName == "BusinessAdmin")
                    {
                        return RedirectToAction("Dashboard", "Business");
                    }
                    else if (user.RoleName == "Staff")
                    {
                        return RedirectToAction("Dashboard", "Staff");
                    }
                    else if (user.RoleName == "Student")
                    {
                        return RedirectToAction("Dashboard", "Student");
                    }
                    else
                    {
                        return Redirect(objLoginModel.ReturnUrl);
                    }
                }
                #endregion ------------------------------------------------------------------------

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objLoginModel);
            }
        }
        public ActionResult ForgotPassword(string returnUrl = "") {
            if (ValidateBusinessAdminCookie())
            {
                return RedirectToAction("Dashboard");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(); 
        }
        public ActionResult Register() {
            if (ValidateBusinessAdminCookie())
            {
                return RedirectToAction("Dashboard");
               
            }

            return View(); 
        }

        [HttpPost]
        public JsonResult Register(RegisterBusinessAdminViewModel registerBusinessAdminViewModel)
        {
            try
            {
                if (registerBusinessAdminViewModel == null)
                {
                    //ViewBag.Message = "Invalid Data!";
                    return Json(new { status = -1, message = "Invalid Data" });
                }

                // Validate infromation passed
                Error_VM error_VM = registerBusinessAdminViewModel.ValidInformation();
                if (!error_VM.Valid)
                {
                    return Json(new { status = -1, message = error_VM.Message });
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    BusinessName = registerBusinessAdminViewModel.BusinessName,
                    Email = registerBusinessAdminViewModel.Email,
                    Password = EDClass.Encrypt(registerBusinessAdminViewModel.Password),
                    PhoneNumber = registerBusinessAdminViewModel.PhoneNumber,
                    PhoneNumberCountryCode = "+91",
                    FirstName = registerBusinessAdminViewModel.FirstName,
                    LastName = registerBusinessAdminViewModel.LastName,
                    DOB = registerBusinessAdminViewModel.DOB,
                    BusinessCategoryId = registerBusinessAdminViewModel.BusinessCategoryId,
                    BusinessSubCategoryId = registerBusinessAdminViewModel.BusinessSubCategoryId,
                    Mode = 1
                });

                if (resp.ret <= 0)
                {
                    return Json(new { status = resp.ret, message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey) });
                }
                else
                {
                    EmailSender emailSender = new EmailSender();
                    emailSender.Send(registerBusinessAdminViewModel.FirstName + " " + registerBusinessAdminViewModel.LastName, "Registration successful", registerBusinessAdminViewModel.Email, "You have been successfully registered as business owner with Masterzone. Your MasterID is :  " + resp.MasterId , "");

                    // Remove/Expire Login Cookies
                    CookieHelper cookieHelper = new CookieHelper();
                    cookieHelper.RemoveOrExpireLoginCookies(Response);

                    TokenGenerator tokenGenerator = new TokenGenerator();
                    var _JWT_User_Token = tokenGenerator.Create_JWT(resp.Id, "BusinessAdmin", 0);

                    //Create Admin login cookie
                    HttpCookie myCookie = new HttpCookie(CookieKeyNames.BusinessAdminCookie);
                    myCookie["UserToken"] = _JWT_User_Token;
                    myCookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(myCookie);

                    //RedirectToAction("Dashboard");
                    return Json(new { status = 1, message = "success" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -101, message = "Something went wrong. Please try again!" });
            }
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageStaff()
        {
            SetSidebarCookieInfo("manageViewStaff");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Dashboard()
        {
            //if(!ValidateBusinessAdminCookie())
            //{
            //    return RedirectToAction("Login","Business");
            //}
            SetSidebarCookieInfo("dashboard");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Advertisement()
        {

            string BusinessLoginId = (ValidateBusinessAdminCookie()) ? cookieHelper.GetClaimValueFromCookie(CookieKeyNames.BusinessAdminCookie, "businessAdminLoginId") : cookieHelper.GetClaimValueFromCookie(CookieKeyNames.StaffCookie, "businessAdminLoginId");

            var IsPrimeMember = businessOwnerService.IsBusinessOwnerPrimeMember(Convert.ToInt64(BusinessLoginId));
            ViewBag.IsBusinessOwnerPrimeMember = 0;
            if (IsPrimeMember == 1)
            {
                ViewBag.IsBusinessOwnerPrimeMember = 1;
            }

            SetSidebarCookieInfo("manageAdvertisement");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Message()
        {
            SetSidebarCookieInfo("manageMessage");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult TimeSlot()
        {
            SetSidebarCookieInfo("manageTimeSlot");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Notification()
        {
            SetSidebarCookieInfo("manageNotification");
            return View();
        }

        //[BusinessPanelPermissionActionFilter]
        //public ActionResult ManagePayment()
        //{
        //    SetSidebarCookieInfo("managePayment");
        //    return View();
        //}

        [BusinessPanelPermissionActionFilter]
        public ActionResult Enquiry()
        {
            SetSidebarCookieInfo("manageEnquiry");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageQueries()
        {
            SetSidebarCookieInfo("manageQueries");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Event()
        {
            SetSidebarCookieInfo("manageEvents");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Exam()
        {
            SetSidebarCookieInfo("manageExam");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Discount()
        {
            SetSidebarCookieInfo("manageDiscount");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult MyAccount()
        {
            SetSidebarCookieInfo("myAccount");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Group()
        {
            SetSidebarCookieInfo("manageGroup");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Package()
        {
            SetSidebarCookieInfo("managePackage");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Attendance()
        {
            SetSidebarCookieInfo("manageStaffAttendance");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Certificate()
        {
            SetSidebarCookieInfo("manageCertificate");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult CertificateRequests()
        {
            SetSidebarCookieInfo("manageCertificateRequests");
            return View();
        }
        
        [BusinessPanelPermissionActionFilter]
        public ActionResult BookCertificateLicense()
        {
            SetSidebarCookieInfo("manageBookCertificateLicense");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult MyBookedLicenses()
        {
            SetSidebarCookieInfo("manageMyBookedLicenses");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Backup()
        {
            SetSidebarCookieInfo("manageBackup");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult TransferPackage()
        {
            SetSidebarCookieInfo("managePackageTransfer");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageSalary()
        {
            SetSidebarCookieInfo("manageSalary");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ScheduleClass()
        {
            SetSidebarCookieInfo("manageScheduleClass");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Student()
        {
            SetSidebarCookieInfo("manageStudent");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ProfileSettings()
        {
            string UserRole = HttpContext.Items["UserRole"] as string;
            if (UserRole == "BusinessAdmin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("StaffProfileSettings", "Business");
            }
            //ViewBag.Permissions = UserRole;

        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult StudentWithClassInstructor()
        {
            SetSidebarCookieInfo("manageStudentWithClassInstructor");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Training()
        {
            SetSidebarCookieInfo("manageTraining");
            return View();
        }

        /// <summary>
        /// Message Chat page
        /// </summary>
        /// <param name="toUserLoginId">Encrypted To-User-Login-Id</param>
        /// <returns></returns>
        /// 

        [BusinessPanelPermissionActionFilter]
        public ActionResult MessageChat()
        {
            //ViewBag.ToUserLoginId = EDClass.Decrypt(toUserLoginId);
            SetSidebarCookieInfo("manageMessage");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageReviews()
        {
            SetSidebarCookieInfo("manageReviews");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult UpgradePackage()
        {
            SetSidebarCookieInfo("");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult PaymentsTransactionDetailsBusiness()
        {
            SetSidebarCookieInfo("paymentsTransactionDetailsBusiness");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ViewDocumentsBusiness()
        {
            SetSidebarCookieInfo("");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        [HttpPost]
        public FileResult Download(string ExportData)
        {

            var filePath = Path.Combine(HttpContext.Server.MapPath("/Content/Uploads/Documents/"), ExportData);
            //return File(filePath, ExportData);

            return File(filePath, "application/octet-stream", Path.GetFileName(filePath));

        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageBusinessVideos()
        {

            SetSidebarCookieInfo("manageBusinessVideos");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageBusinessImages()
        {
            SetSidebarCookieInfo("manageBusinessImages");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult StaffProfileSettings()
        {
            SetSidebarCookieInfo("");
            return View();
        }

        // TODO: [Meena Integration] check if Panel Permission required?
        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageExpense()
        {
            var roleName = "";
            if (ValidateStaffCookie())
            {
                roleName = cookieHelper.GetClaimValueFromCookie(CookieKeyNames.StaffCookie, ClaimTypes.Role);
            }
            else if (ValidateBusinessAdminCookie())
            {
                roleName = cookieHelper.GetClaimValueFromCookie(CookieKeyNames.BusinessAdminCookie, ClaimTypes.Role);
            }
            ViewBag.UserRole = roleName;
            SetSidebarCookieInfo("manageExpense");
            return View();
        }

        // TODO: [Meena Integration] check if Panel Permission required?
        [BusinessPanelPermissionActionFilter]
        public ActionResult EnquiryFollowsup()
        {
            SetSidebarCookieInfo("manageEnquiryFollowsup");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult TransferNotification()
        {
            SetSidebarCookieInfo("manageTransferPackageNotification");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult NotificationDetail(long id)
        {
            ViewBag.BusinessOwnerLoginId = id;
            SetSidebarCookieInfo("");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageBatch()
        {
            SetSidebarCookieInfo("manageBatch");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageApartment()
        {
            SetSidebarCookieInfo("manageApartment");
            return View();
        }
        
        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageApartmentBooking()
        {
            SetSidebarCookieInfo("manageApartmentBooking");
            return View();
        }

        /// <summary>
        /// Apartment Booking Details pange
        /// </summary>
        /// <param name="id">Apartment-Booking-Id</param>
        /// <returns></returns>
        [BusinessPanelPermissionActionFilter]
        public ActionResult ApartmentBookingDetail(long id)
        {
            ViewBag.ApartmentBookingId = id;

            SetSidebarCookieInfo("manageApartmentBookingDetail");
            return View();
        }

        /// <summary>
        /// Apartment Booking Details pange
        /// </summary>
        /// <param name="id">Apartment-Booking-Id</param>
        /// <returns></returns>
        [BusinessPanelPermissionActionFilter]
        public ActionResult ApartmentBookingAllDetail()
        {
            SetSidebarCookieInfo("viewApartmentBookingAllDetail");
            return View();
        }

        /// <summary>
        /// Get the all register student or candidate who submit that form
        /// </summary>
        /// <param name="examId">Form id</param>
        /// <returns></returns>
        [BusinessPanelPermissionActionFilter]
        public async Task<ActionResult> ViewSubmitExamForm(Int64 examId)
        {
            HttpClient client = new HttpClient();
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage ExamFormAPIResponse = await client.GetAsync($"/api/Exam/GetExamFormForVistor?id={examId}");
            // Set the Authorization header
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", (string)GetBusinessAdminToken().Data);

            HttpResponseMessage CandidateAPIResponse = await client.GetAsync($"/api/Exam/GetSubmitExamForm?ExamFormId={examId}");

            if (ExamFormAPIResponse.IsSuccessStatusCode && ExamFormAPIResponse.IsSuccessStatusCode)
            {
                string exam_ApiResponse = await ExamFormAPIResponse.Content.ReadAsStringAsync();
                string candi_ApiResponse = await CandidateAPIResponse.Content.ReadAsStringAsync();

                // Deserialize the response content to ApiResponse_VM
                ApiResponse_VM examFormResponseObject = JsonConvert.DeserializeObject<ApiResponse_VM>(exam_ApiResponse);
                // Deserialize the 'data' property as a ExamFormResponse_VM
                ExamFormResponse_VM ExamFormResponse = JsonConvert.DeserializeObject<ExamFormResponse_VM>((examFormResponseObject.data).ToString());

                // Deserialize the response content to ApiResponse_VM
                ApiResponse_VM CandidateResponseObject = JsonConvert.DeserializeObject<ApiResponse_VM>(candi_ApiResponse);

                // Deserialize the 'data' property as a List<SubmitExamFormResponse_VM>
                List<ExamFormSubmissionResponse_VM> CandidateResponses = JsonConvert.DeserializeObject<List<ExamFormSubmissionResponse_VM>>(CandidateResponseObject.data.ToString());



                C_Business_A_SubmitExamForm_VM SubmitExamFromResp = new C_Business_A_SubmitExamForm_VM
                {
                    SomeDetailsOfCandidateList = CandidateResponses.Select(s => new C_Business_A_SubmitExamForm_ExamTable_VM
                    {
                        Id = s.Id,
                        CandidateName = s.CandidateName,
                        CandidateFather = s.CandidateFather,
                        CandidateMother = s.CandidateMother,
                        CandidateProfileImageWithPath = s.CandidateProfileImageWithPath,
                        CurrentRollNo = s.CurrentRollNo,
                    }).ToList()
                };

                SubmitExamFromResp.AllDetailsOfCandidateList = CandidateResponses;
                SubmitExamFromResp.ExamFromDetails = ExamFormResponse;


                return View(SubmitExamFromResp);
            }
            else
            {
                return View("Error");
            }
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageBusinessService()
        {

            SetSidebarCookieInfo("manageBusinessService");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ProfilePage()
        {
            string BusinessLoginId = (ValidateBusinessAdminCookie()) ? cookieHelper.GetClaimValueFromCookie(CookieKeyNames.BusinessAdminCookie, "businessAdminLoginId") : cookieHelper.GetClaimValueFromCookie(CookieKeyNames.StaffCookie, "businessAdminLoginId");


            ViewBag.BusinessLoginId = BusinessLoginId;
            BusinessProfileTypeKey_VM ProfileTypeKeyResponse = new BusinessProfileTypeKey_VM();

            ProfileTypeKeyResponse = businessOwnerService.GetBusinessProfileTypeDetailsById(Convert.ToInt64(BusinessLoginId));
            ViewBag.ProfilePageTypeKey = ProfileTypeKeyResponse.Key;
            // ViewBag.ProfilePageTypeKey = "dance_web";
            //ViewBag.ProfilePageTypeKey = "gym_web";
            // ViewBag.ProfilePageTypeKey = "yoga_web";
            // ViewBag.ProfilePageTypeKey = "music_web";
            //  ViewBag.ProfilePageTypeKey = "music_web";
            SetSidebarCookieInfo("manageProfilePage");

            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult Sponsors()
        {
            SetSidebarCookieInfo("manageSponsors");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult StudentDetail(long studentLoginId)
        {
            ViewBag.StudentLoginId = studentLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ViewTrainingBookings(long trainingId)
        {
            ViewBag.TrainingId = trainingId;
            return View();
        }
        
        [BusinessPanelPermissionActionFilter]
        public ActionResult TrainingBookingDetail(long trainingBookingId)
        {
            ViewBag.TrainingBookingId = trainingBookingId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageBranches()
        {
            SetSidebarCookieInfo("manageBranches");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult BranchEvent(long businessbranchLoginId)
        {
            SetSidebarCookieInfo("branchEvent");
            ViewBag.BusinessBranchLoginId = businessbranchLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult BranchStaff(long businessbranchLoginId)
        {
            SetSidebarCookieInfo("branchStaff");
            ViewBag.BusinessBranchLoginId = businessbranchLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult BranchTansactions(long businessbranchLoginId)
        {
            SetSidebarCookieInfo("branchTansactions");
            ViewBag.BusinessBranchLoginId = businessbranchLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult BranchStudent(long businessbranchLoginId)
        {
            SetSidebarCookieInfo("branchStudent");
            ViewBag.BusinessBranchLoginId = businessbranchLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult PauseClassDetail()
        {
            SetSidebarCookieInfo("pauseClassDetail");
            return View();
        }

        public ActionResult Logout()
        {
            //SignOutAsync is Extension method for SignOut    
            /*await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page    
            return LocalRedirect("/");*/

            CookieHelper cookieHelper = new CookieHelper();
            cookieHelper.RemoveOrExpireLoginCookies(Response);
            return RedirectToAction("Login", "Business");
        }

        public ActionResult ResetPassword(string token)
        {
            ViewBag.ResetPasswordToken = token;
            try
            {
                if (String.IsNullOrEmpty(token))
                {
                    ViewBag.ErrorMessage = "Invalid Link!";
                    return View();
                }
                ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
                DateTime UTC_DateTime = DateTime.UtcNow;
                //UTC_DateTime = DateTime.UtcNow.AddHours(1);   // For Testing Token Expiry, uncomment this.
                string decryptedToken = EDClass.Decrypt(token);
                ResetPasswordToken_VM resetPasswordToken_VM = JsonConvert.DeserializeObject<ResetPasswordToken_VM>(decryptedToken);
                bool is_Valid = true;

                #region Get User-Details in User-Database -----------------


                SP_InsertUpdateResetPasswordDetail_Param_VM sP_InsertUpdateUserLogin_Param_VM = new SP_InsertUpdateResetPasswordDetail_Param_VM()
                {
                    Id = resetPasswordToken_VM.UserId,
                    Mode = 2
                };
                StoredProcedureRepository storedProcedureRepository = new StoredProcedureRepository(db);
                User_VM updateTokenResponse = storedProcedureRepository.SP_InsertUpdateResetPasswordDetail<User_VM>(sP_InsertUpdateUserLogin_Param_VM);


                if (updateTokenResponse == null || String.IsNullOrEmpty(updateTokenResponse.ResetPasswordToken) || (updateTokenResponse.ResetPasswordToken != token))
                {
                    is_Valid = false;
                    ViewBag.ErrorMessage = "Invalid Token or Link has been expired!";
                    ViewBag.IsTokenValid = is_Valid;
                    return View(resetPasswordViewModel);
                }

                #endregion --------------------------------------------

                var dateTime_diff = resetPasswordToken_VM.ValidTill_UTCDateTime - UTC_DateTime;
                //validate date by having same date and hours difference.
                // Total Hours will be negative if exceeding validTill-DateTime-Hours.
                if (Convert.ToInt32(dateTime_diff.TotalDays) == 0 && dateTime_diff.TotalHours >= 0)
                {
                    // token valid
                    is_Valid = true;
                }
                else
                {
                    is_Valid = false;
                    ViewBag.ErrorMessage = "Link has been expired!";
                }
                ViewBag.IsTokenValid = is_Valid;

                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Status = -1;
                ViewBag.ErrorMessage = "Internal Server Error!";
                return View();
            }
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult CustomForm()
        {

            SetSidebarCookieInfo("customForm");
            return View();
        }
        [BusinessPanelPermissionActionFilter]

        public ActionResult ViewCustomForm()
        {

            SetSidebarCookieInfo("viewCustomForm");
            return View();
        }

        [BusinessPanelPermissionActionFilter]

        public ActionResult BusinessTransferForm(long customFormId)
        {
            ViewBag.CustomFormId = customFormId;
            SetSidebarCookieInfo("businessTransferForm");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult StaffDetail(long staffLoginId)
        {
            ViewBag.StaffLoginId = staffLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageMasterproExtraInformationService()
        {
            return View();
        }
        
        [BusinessPanelPermissionActionFilter]
        public ActionResult ManageMasterProPdfService()
        {
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult ScheduleCourses()
        {
            SetSidebarCookieInfo("scheduleCourses");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult MyBooking()
        {
            SetSidebarCookieInfo("myBooking");
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult MyPurchaseClassBookingDetail(long classId)
        {
            ViewBag.ClassId = classId;
            return View();
        }


        [BusinessPanelPermissionActionFilter]
        public ActionResult MyTrainingPurchaseDetails(long trainingId)
        {
            ViewBag.TrainingId = trainingId;
            return View();

        }
        [BusinessPanelPermissionActionFilter]
        public ActionResult MyEventPurchaseDetails(long eventId)
        {
            ViewBag.EventId = eventId;
            return View();
        }
        [BusinessPanelPermissionActionFilter]
        public ActionResult MyPlanPurchaseDetails(long planId)
        {
            ViewBag.PlanId = planId;
            return View();
        }
        [BusinessPanelPermissionActionFilter]
        public ActionResult MyCoursePurchaseDetails(long courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }

        #region Student Booking in Business Panel --------------------------------

        [BusinessPanelPermissionActionFilter]
        public ActionResult BookingCheckout(long studentLoginId, long itemId, string itemType)
        {
            if (!ValidateBusinessAdminCookie() && !ValidateStaffCookie())
            {
                var returnUrl = Url.Action("BookingCheckout", routeValues: new { studentLoginId = studentLoginId, itemId = itemId, itemType = itemType });
                return RedirectToAction("Login", "Business", new { returnUrl = Server.UrlEncode(returnUrl) });
            }

            long userLoginId = studentLoginId;
            int IsItemAlreadyPurchased = 0;

            string AlreadyPurchasedMessage = "";

            var eventService = new EventService(db);
            var planService = new PlanService(db);

            itemType = itemType.ToLower();
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

            ViewBag.ItemId = itemId;
            ViewBag.ItemType = itemType;
            ViewBag.IsItemAlreadyPurchased = IsItemAlreadyPurchased;
            ViewBag.AlreadyPurchasedMessage = AlreadyPurchasedMessage;
            ViewBag.StudentLoginId = studentLoginId;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult BookPackage(long studentLoginId, long itemId, int paymentMode, long couponId, decimal totalAmountPaid)
        {
            if (!ValidateBusinessAdminCookie() && !ValidateStaffCookie())
            {
                return RedirectToAction("Business", "Login");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = itemId,
                    UserLoginId = studentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId
                };

                var bookingService = new BookingService(db);


                ServiceResponse_VM bookingResponse = bookingService.CreatePlanBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("CheckoutOrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("CheckoutOrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                return Content("Payment Provider Page for payment");
            }

            return View();
        }

        public ActionResult BookMainPlanPackage(long itemId, int paymentMode, decimal totalAmountPaid)
        {
            //if (!ValidateStudentCookie())
            //{
            //    return RedirectToAction("Login", "Home");
            //}

            // if cash payment
            if (paymentMode == 1)
            {
                CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM()
                {
                    PlanId = itemId,
                    UserLoginId = Convert.ToInt64(GetLoginIdFromBusinessAdminCookie()),
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = 0
                };

                ServiceResponse_VM bookingResponse = bookingService.CreateBusinessMainPlanBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("CheckoutSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("OrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                return Content("Payment Provider Page for payment");
            }

            return View();
        }


        [BusinessPanelPermissionActionFilter]
        public ActionResult BookEvent(long studentLoginId, long itemId, int paymentMode, long couponId, decimal totalAmountPaid)
        {
            if (!ValidateBusinessAdminCookie() && !ValidateStaffCookie())
            {
                return RedirectToAction("Business", "Login");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateEventBooking_VM createEventBooking_VM = new CreateEventBooking_VM()
                {
                    EventId = itemId,
                    UserLoginId = studentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId
                };

                var bookingService = new BookingService(db);
                ServiceResponse_VM bookingResponse = bookingService.CreateEventBooking(createEventBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("CheckoutOrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("CheckoutOrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                return Content("Payment Provider Page for payment");
            }

            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult BookClass(long studentLoginId, long itemId, int paymentMode, long couponId, decimal totalAmountPaid, string joinClassDate, int batchId)
        {
            if (!ValidateBusinessAdminCookie() && !ValidateStaffCookie())
            {
                return RedirectToAction("Business", "Login");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateClassBooking_VM createClassBooking_VM = new CreateClassBooking_VM()
                {
                    ClassId = itemId,
                    UserLoginId = studentLoginId,
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

                var bookingService = new BookingService(db);
                ServiceResponse_VM bookingResponse = bookingService.CreateClassBooking(createClassBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("CheckoutOrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("CheckoutOrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                return Content("Payment Provider Page for payment");
            }

            return View();
        }
        
        [BusinessPanelPermissionActionFilter]
        public ActionResult BookTraining(long studentLoginId, long itemId, int paymentMode, long couponId, decimal totalAmountPaid)
        {
            if (!ValidateBusinessAdminCookie() && !ValidateStaffCookie())
            {
                return RedirectToAction("Business", "Login");
            }

            // if cash payment
            if (paymentMode == 1)
            {
                CreateTrainingBooking_VM createPlanBooking_VM = new CreateTrainingBooking_VM()
                {
                    TrainingId = itemId,
                    UserLoginId = studentLoginId,
                    OnlinePayment = 0,
                    TransactionID = "",
                    PaymentResponseStatus = "",
                    PaymentProvider = "ManualPayment",
                    PaymentMethod = "Cash",
                    PaymentDescription = "Cash Payment",
                    TotalAmountPaid = totalAmountPaid,
                    CouponId = couponId
                };

                var bookingService = new BookingService(db);
                ServiceResponse_VM bookingResponse = bookingService.CreateTrainingBooking(createPlanBooking_VM);

                if (bookingResponse.Status == 1)
                {
                    return RedirectToAction("CheckoutOrderSuccess");
                }
                else if (bookingResponse.Status < 0)
                {
                    return RedirectToAction("CheckoutOrderError", new { errorTitle = "Error!", errorMessage = bookingResponse.Message });
                }
            }
            else
            {
                // online payment
                return Content("Payment Provider Page for payment");
            }

            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult CheckoutOrderSuccess()
        {
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult CheckoutSuccess()
        {
            return View();
        }


        [BusinessPanelPermissionActionFilter]
        public ActionResult CheckoutOrderError(string errorTitle, string errorMessage)
        {
            ViewBag.ErrorTitle = errorTitle;
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        [BusinessPanelPermissionActionFilter]
        public ActionResult UpgradePackagePlan(long itemId)
        {
            ViewBag.ItemId = itemId;
            

         
            int IsItemAlreadyPurchased = 0;

            string AlreadyPurchasedMessage = "";

            var planService = new PlanService(db);

            long userLoginId = 0;
            
         
             if ( planService.IsAlreadyMainPlanPurchased(itemId, userLoginId = Convert.ToInt64(GetLoginIdFromBusinessAdminCookie())))
            {
                IsItemAlreadyPurchased = 1;
                AlreadyPurchasedMessage = Resources.ErrorMessage.PlanAlreadyPurchased;
                //return RedirectToAction("AlreadyPurchased", "Booking");
            }

            ViewBag.ItemId = itemId;
            ViewBag.IsItemAlreadyPurchased = IsItemAlreadyPurchased;
            ViewBag.AlreadyPurchasedMessage = AlreadyPurchasedMessage;

            return View();
        }


        [BusinessPanelPermissionActionFilter]
        public ActionResult GroupMessageChat(long toUserLoginId)
        {
            ViewBag.ToUserLoginId = toUserLoginId;
            return View();
        }

        #endregion ----------------------------------------------------------


        [BusinessPanelPermissionActionFilter]
        public ActionResult TennisBooking()
        {

            SetSidebarCookieInfo("tennisBooking");
            return View();
        }
    }
}