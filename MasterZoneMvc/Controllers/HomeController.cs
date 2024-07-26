using Facebook;
using GoogleAuthentication.Services;
using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using static MasterZoneMvc.ViewModels.BusinessStudentRegisterViewModel;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
namespace MasterZoneMvc.Controllers
{
    public class HomeController : Controller
    {
        private MasterZoneDbContext db;
        private LoginService loginService;
        private StudentService studentService;
        private ClassService classService;


        private string _GoogleClientId = ConfigurationManager.AppSettings["GoogleClientId"];
        private string _FacebookClientId = ConfigurationManager.AppSettings["FacebookClientId"];
        private string sId = ConfigurationManager.AppSettings["TwiloSId"];
        private string authToken = ConfigurationManager.AppSettings["TwiloAuthToken"];
        private string fromPhoneNumber = ConfigurationManager.AppSettings["TwilofromPhoneNumber"];
        public HomeController()
        {
            db = new MasterZoneDbContext();
            loginService = new LoginService(db);
            classService = new ClassService(db);

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

        public JsonResult GetStudentToken()
        {
            string _Token = "";
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.StudentCookie];

            if (myCookie != null)
            {
                _Token = myCookie["UserToken"].ToString();
            }
            return Json(_Token, JsonRequestBehavior.AllowGet);
        }

        private void SetSidebarCookieInfo(string _val)
        {
            CookieHelper cookieHelper = new CookieHelper();
            cookieHelper.ResetSidebarCookie(Response, _val);
        }

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

        public ActionResult Index()
        {
            //if (ValidateStudentCookie())
            //{
            //    return RedirectToAction("MyClass");
            //}
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Search(string searchKeyword = "", string latitude="", string longitude="", string searchBy = "", int lastRecordId = 0, int businessCategoryId = 0)
        {
            RequestBusinessSearchViewModel requestMasterSearchViewModel = new RequestBusinessSearchViewModel()
            {
                SearchKeyword = searchKeyword,
                Latitude = latitude,
                Longitude = longitude,
                BusinessCategoryId = businessCategoryId,
                LastRecordId = lastRecordId,
                SearchBy = searchBy
            }; 

            ViewBag.SearchParametersJsonObject = JsonConvert.SerializeObject(requestMasterSearchViewModel);
            return View(requestMasterSearchViewModel);
        }

        public ActionResult MoreCategories()
        {
            return View();
        }

        #region Register Login User/Student -------------------------------------------------------------
        public ActionResult Register(string returnUrl = "")
        {
            string domainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            //Google Login
            var clientId = _GoogleClientId;
            var url = domainName + "/Google/GoogleLoginCallback";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.response = response;
            //Facebook Login
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = _FacebookClientId,
                redirect_uri = domainName + "/Facebook/FacebookRedirect",
                scope = "public_profile,email"
            });

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Url = loginUrl;
            return View();
        }

        [HttpPost]
        public JsonResult Register(StudentRegisterViewModel studentregisterViewModel)
        {
            try
            {
                if (studentregisterViewModel == null)
                {
                    return Json(new { status = 1, message = "Invalid Data" });
                }
                // Validate infromation passed
                Error_VM error_VM = studentregisterViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    return Json(new { status = -1, message = error_VM.Message });
                }

                // Generate OTP
                var otp = OtpGenerator.GenerateOtp();
                var messageOTP = $"OTP: {otp}"; //ADD the OTP into the Db
                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                studentService = new StudentService(db);
                var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                {
                    Email = studentregisterViewModel.Email,
                    Password = EDClass.Encrypt(studentregisterViewModel.Password),
                    PhoneNumberCountryCode = "+91",
                    PhoneNumber = studentregisterViewModel.PhoneNumber,
                    RoleId = 3,
                    FirstName = studentregisterViewModel.FirstName,
                    LastName = studentregisterViewModel.LastName,
                    Gender = studentregisterViewModel.Gender,
                    Mode = 1,
                    OTP = "12345",
                });

                if (resp.ret <= 0)
                {
                    return Json(new { status = resp.ret, message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey) });
                }
                else
                {
                    //Send Otp

                    //TwilioClient.Init(sId, authToken);
                    //var message = MessageResource.Create(
                    //    body: messageOTP,
                    //    from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
                    //    to: new Twilio.Types.PhoneNumber($"+91{studentregisterViewModel.PhoneNumber}") //add receiver's phone number
                    //);

                    //EmailSender emailSender = new EmailSender();
                    //emailSender.Send(studentregisterViewModel.FirstName + " " + studentregisterViewModel.LastName, "Registration successful", studentregisterViewModel.Email, "You have been successfully registered with Masterzone. Your MasterID is " + resp.MasterId, "");

                    // Remove/Expire Login Cookies
                    CookieHelper cookieHelper = new CookieHelper();
                    cookieHelper.RemoveOrExpireLoginCookies(Response);

                    //TokenGenerator tokenGenerator = new TokenGenerator();
                    //var _JWT_User_Token = tokenGenerator.Create_JWT(resp.Id, "Student", 0);

                    //Create Admin login cookie
                    //HttpCookie myCookie = new HttpCookie(CookieKeyNames.StudentCookie);
                    //myCookie["UserToken"] = _JWT_User_Token;
                    //myCookie.Expires = DateTime.Now.AddDays(1);
                    //Response.Cookies.Add(myCookie);

                    return Json(new { status = 1, message = "success" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -101, message = "Something went wrong. Please try again!" });
            }

        }

        [HttpPost]
        public JsonResult VerifyOtp(VerifyOtpViewModel verifyOtpViewModel)
        {
            try
            {
                //get the Otp 
                var OTP = verifyOtpViewModel.OTP;
                var Email = verifyOtpViewModel.Email;

                //Procedure to fetch the OTP of that particular user

                studentService = new StudentService(db);
                //STEP--> Create a procedure to update the status of the user
                var resp = studentService.InsertUpdateStudentDb(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                {
                    Email = verifyOtpViewModel.Email,
                    OTP = verifyOtpViewModel.OTP
                });

                if (resp.ret <= 0)
                {
                    return Json(new { status = resp.ret, message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey) });
                }
                else
                {
                    //STEP-->Get firstName,lastName and Email from the resp
                    EmailSender emailSender = new EmailSender();
                    emailSender.Send(verifyOtpViewModel.FirstName + " " + verifyOtpViewModel.LastName, "Registration successful", verifyOtpViewModel.Email, "You have been successfully registered with Masterzone. Your MasterID is " + resp.MasterId, "");

                    // Remove/Expire Login Cookies
                    CookieHelper cookieHelper = new CookieHelper();
                    cookieHelper.RemoveOrExpireLoginCookies(Response);

                    TokenGenerator tokenGenerator = new TokenGenerator();
                    var _JWT_User_Token = tokenGenerator.Create_JWT(resp.Id, "Student", 0);

                    //Create Admin login cookie
                    HttpCookie myCookie = new HttpCookie(CookieKeyNames.StudentCookie);
                    myCookie["UserToken"] = _JWT_User_Token;
                    myCookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(myCookie);

                    return Json(new { status = 1, message = "success" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -101, message = "Unable to verify OTP. Please enter Correct OTP" });
            }
        }
        public ActionResult Login(string returnUrl = "")
        {
            if (ValidateStudentCookie())
            {
                return RedirectToAction("MyClass");
            }
            string domainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            
            //Google Login
            var clientId = _GoogleClientId;
            var url = domainName + "/Google/GoogleLoginCallback";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.response = response;
            //Facebook Login
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = _FacebookClientId,
                redirect_uri = domainName + "/Facebook/FacebookRedirect",
                scope = "public_profile,email"
            });

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Url = loginUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel objLoginModel, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            try
            {
                if (objLoginModel == null || String.IsNullOrEmpty(objLoginModel.MasterId) || String.IsNullOrEmpty(objLoginModel.Password))
                {
                    ViewBag.Message = "Please enter required fields!";
                    return View(objLoginModel);
                }

                #region Student login, validate if success and claim user login ---------------------------------------

                //SqlParameter[] queryParams = new SqlParameter[] {
                //    new SqlParameter("id", "0"),
                //    new SqlParameter("email", objLoginModel.Email),
                //    new SqlParameter("password", EDClass.Encrypt(objLoginModel.Password)),
                //    new SqlParameter("mode", "3")
                //    };
                //var st = EDClass.Encrypt(objLoginModel.Password);
                //UserLoginViewModel user = db.Database.SqlQuery<UserLoginViewModel>("exec sp_Login @id,@email,@password,@mode", queryParams).FirstOrDefault();

                LoginService_VM loginService_VM = new LoginService_VM()
                {
                    Email = "",
                    Password = objLoginModel.Password,
                    Mode = 3,
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
                    studentService = new StudentService(db);
                    var student = studentService.GetStudentUserById(user.Id);
                    if (student.IsBlocked == 0)
                    {
                        //Add logic here to display some message to user    
                        ViewBag.Message = "Your Account is Inactive";
                    }
                    else {
                        var blockReason = (!string.IsNullOrEmpty(student.BlockReason)) ? "" : " Reason: " + student.BlockReason;
                        ViewBag.Message = "Your Account is Blocked!" + student.BlockReason;
                    }
                    return View(objLoginModel);
                }
                else
                {
                    // Remove/Expire Login Cookies
                    CookieHelper cookieHelper = new CookieHelper();
                    cookieHelper.RemoveOrExpireLoginCookies(Response);

                    TokenGenerator tokenGenerator = new TokenGenerator();
                    var _JWT_User_Token = tokenGenerator.Create_JWT(user.Id, user.RoleName, 0);

                    //Create Admin login cookie
                    HttpCookie myCookie = new HttpCookie(CookieKeyNames.StudentCookie);
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
                        string decodedUrl = "";
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            decodedUrl = Server.UrlDecode(returnUrl);
                        }

                        if (Url.IsLocalUrl(decodedUrl))
                        {
                            return Redirect(decodedUrl);
                        }
                        else
                        {
                            return RedirectToAction("MyClass", "Home");
                        }

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

        #endregion  --------------------------------------------------------------------------------------



        public ActionResult Gym(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Yoga(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult TermCondition()
        {
            return View();
        }

        public ActionResult ForgotPassword(string returnUrl = "")
        {
            if (Request.Cookies[CookieKeyNames.BusinessAdminCookie] != null || Request.Cookies[CookieKeyNames.StaffCookie] != null)
            {
                return RedirectToAction("Dashboard", "BusinessAdmin");
            }
            else if (Request.Cookies[CookieKeyNames.SuperAdminCookie] != null || Request.Cookies[CookieKeyNames.SubAdminCookie] != null)
            {
                return RedirectToAction("Dashboard", "SuperAdmin");
            }
            else if (Request.Cookies[CookieKeyNames.StudentCookie] != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        public ActionResult EmailConfirmation(string token)
        {
            ViewBag.Status = -1;
            ViewBag.Message = Resources.ErrorMessage.InvalidLink;

            if (String.IsNullOrEmpty(token))
            {
                ViewBag.Status = -1;
                ViewBag.Message = Resources.ErrorMessage.InvalidLink;
                return View();
            }

            try
            {
                ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
                DateTime UTC_DateTime = DateTime.UtcNow;
                //UTC_DateTime = DateTime.UtcNow.AddHours(1);   // For Testing Token Expiry, uncomment this.
                string decryptedToken = EDClass.Decrypt(token);
                ResetPasswordToken_VM resetPasswordToken_VM = JsonConvert.DeserializeObject<ResetPasswordToken_VM>(decryptedToken);
                bool is_Valid = true;

                var dateTime_diff = resetPasswordToken_VM.ValidTill_UTCDateTime - UTC_DateTime;
                //validate date by having same date and hours difference.
                // Total Hours will be negative if exceeding validTill-DateTime-Hours.
                if (Convert.ToInt32(dateTime_diff.TotalDays) == 0 && dateTime_diff.TotalHours >= 0)
                {
                    #region Update Email Confirmed to User -----------------

                    // To mark email as confirmed
                    StoredProcedureRepository storedProcedureRepository = new StoredProcedureRepository(db);
                    SPResponseViewModel updateTokenResponse = storedProcedureRepository.SP_ManageUserLogin_Get<SPResponseViewModel>(new SP_ManageUserLogin_Params_VM()
                    {
                        Id = resetPasswordToken_VM.UserId,
                        Mode = 5
                    });

                    #endregion --------------------------------------------

                    ViewBag.Status = 1;
                    ViewBag.Message = Resources.VisitorPanel.EmailConfirmedMessage;
                }
                else
                {
                    ViewBag.Status = false;
                    ViewBag.Message = Resources.ErrorMessage.LinkHasbeenExpired;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Status = -1;
                ViewBag.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                return View();
            }
        }

        #region Logged-In User Action Methods ---------------------------------------------
        public ActionResult MyClass()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myClass");
            return View();
        }
        public ActionResult Queries()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("manageQueries");
            return View();
        }

        public ActionResult SavedInstructor()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("savedInstructors");
            return View();
        }

        public ActionResult Notification()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("notifications");
            return View();
        }

        public ActionResult MyCoupons()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myCoupons");
            return View();
        }

        public ActionResult MyAccount()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myaccount");
            return View();
        }

        public ActionResult ChangePassword()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public FileResult Download(string ExportData)
        {
            if (!ValidateStudentCookie())
            {
                return null;
            }

            var filePath = Path.Combine(HttpContext.Server.MapPath("/Content/Uploads/Certificates/"), ExportData);
            return File(filePath, "application/pdf", "ExportData.pdf");

        }

        public ActionResult Messages()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("messages");
            return View();
        }

        public ActionResult MessageChat(string toUserLoginId)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("");
            ViewBag.BusinessLoginId = EDClass.Decrypt(toUserLoginId);
            return View();
        }

        public ActionResult TransferPackage()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("transferPackage");
            return View();
        }

        public ActionResult StudentPauseClassRequest()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("studentPauseClassRequest");
            return View();
        }

        public ActionResult MyBooking()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myBooking");

            return View();
        }
        public ActionResult MyCourses()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myCourses");

            return View();
        }


        #endregion --------------------------------------------------------

        public ActionResult BusinessDetail()
        {
            return View();
        }
        public ActionResult ListingPage()
        {
            return View();
        }

        public ActionResult Category(long categoryId)
        {
            ViewBag.CategoryId = categoryId;
            return View();
        }

        public ActionResult Dance(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }
        public ActionResult Instructor(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }
        public ActionResult Music(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }
        public ActionResult EventOrganisation(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult Event(Int64 eventId)
        {
            ViewBag.EventId = eventId;
            return View();
        }

        public ActionResult PackageDetail(long packageId)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }

            ViewBag.PackageId = packageId;
            return View();
        }

        public ActionResult Class(Int64 classId)
        {
            ViewBag.ClassId = classId;
            return View();
        }

        //------------------static page ClassDetails--------------
        public ActionResult ClassDetails(Int64 classId)
        {
            ViewBag.ClassId = classId;
            return View();
        }


        public async Task<ActionResult> SubmitExamForm(Int64 examId, long itemcourseid)
        {
            HttpClient client = new HttpClient();
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            client.BaseAddress = new Uri(baseUrl);
            HttpResponseMessage response = await client.GetAsync($"/api/Exam/GetExamFormForVistor?id={examId}");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response content to ApiResponse_VM
                ApiResponse_VM apiResponseObject = JsonConvert.DeserializeObject<ApiResponse_VM>(apiResponse);
                ExamFormResponse_VM examFormResponse = JsonConvert.DeserializeObject<ExamFormResponse_VM>((apiResponseObject.data).ToString());


                ViewBag.Itemcourseid = itemcourseid;
                return View(examFormResponse);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult SearchInstructor()
        {
            return View();
        }
        public ActionResult InstructorTrainingSearch()
        {
            return View();
        }
        public ActionResult AllClasses()
        {
            return View();
        }

        public ActionResult ClassDetail(long classId)
        {
            ViewBag.ClassId = classId;

            long businessOwnerLoginId = classService.GetBusinessOwnerLoginId(classId);
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult InstructorResearch(string menuTag = "all", string latitude = "", string longitude="", string location = "", string itemType = "academies", long categoryId = 0, string days = "",long UserLoginId = 0)
        {
            ViewBag.MenuTag = menuTag;
            ViewBag.Latitude = latitude;
            ViewBag.Longitude = longitude;
            ViewBag.Location = location;
            ViewBag.CategoryId = categoryId;
            ViewBag.Days = days;
            ViewBag.ItemType = itemType;
            ViewBag.UserLoginId = UserLoginId;
            return View();
        }

        public ActionResult TrainingSearch()
        {
            //if (!ValidateStudentCookie())
            //{
            //    return RedirectToAction("Login");
            //}
            return View();
        }

        public ActionResult Certification(Int64 Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        public ActionResult Review(Int64 UserLoginId)
        {
            ViewBag.UserLoginId = UserLoginId;
            return View();
        }

        public ActionResult CertificateCategory()
        {
            return View();
        }

        public ActionResult Sports(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult Teacher(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult Booking(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId ;
            return View();
        }

        public ActionResult AllCourse(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult Appointment()
        {
            return View();
        }

        public ActionResult OnlineCourse()
        {
            return View();
        }
        public ActionResult ProfileDetail()
        {
            return View();
        }
        public ActionResult Education(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }
        public ActionResult Details(long courseId,long businessOwnerLoginId)
        {
            ViewBag.CourseId = courseId;
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult VedioGallery()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("vedioGallery");
            return View();
        }
        public ActionResult License()
        {

            SetSidebarCookieInfo("license");
            return View();
        }
        public ActionResult ManageVedios()
        {
            SetSidebarCookieInfo("manageVedios");
            return View();
        }
        public ActionResult ManageImages()
        {
            SetSidebarCookieInfo("manageImages");
            return View();
        }

        /// <summary>
        /// Redirect to Business Profile Page
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Owner Login Id</param>
        /// <returns>Redirects to business corressponding page</returns>
        public ActionResult BusinessProfile(long businessOwnerLoginId)
        {
            BusinessOwnerService businessOwnerService = new BusinessOwnerService(db);
            var businessProfilePageTypeDetail = businessOwnerService.GetBusinessProfileTypeDetailsById(businessOwnerLoginId);

            //respProfileData.ProfilePageTypeKey = businessProfilePageTypeDetail.Key;
            if(businessProfilePageTypeDetail == null)
            {
                return Content("Page not found!");
            }

            if (businessProfilePageTypeDetail.Key == "sports")
            {
                return Redirect($"/Home/Sports?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "dance_web")
            {
                return Redirect($"/Home/Dance?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "gym_web")
            {
                return Redirect($"/Home/Gym?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "music_web")
            {
                return Redirect($"/Home/Music?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "yoga_web")
            {
                return Redirect($"/Home/Yoga?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "education_web")
            {
                return Redirect($"/Home/Education?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "event_organisation")
            {
                return Redirect($"/Home/EventOrganisation?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "teacher")
            {
                return Redirect($"/Home/Teacher?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "instructor")
            {
                return Redirect($"/Home/Instructor?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "instructor")
            {
                return Redirect($"/Home/ClassicDance?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "instructor")
            {
                return Redirect($"/Home/OtherProfile?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "masterpro")
            {
                return Redirect($"/Home/MasterPro?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "other")
            {
                return Redirect($"/Home/OtherProfile?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "classic_dance")
            {
                return Redirect($"/Home/ClassicDance?businessOwnerLoginId={businessOwnerLoginId}");
            }
            else if (businessProfilePageTypeDetail.Key == "master_profile_api")
            {
                return Redirect($"/Home/MasterProfileAPI?businessOwnerLoginId={businessOwnerLoginId}");
            }
            return Content("Page not found!");
        }

        public ActionResult ClassicDance(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult ResumeProfile1(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult ResumeProfile2()
        {
            return View();
        }
        public ActionResult MasterProfileAPI(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }
        public ActionResult OtherProfile(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult Masterpro(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult MasterInstructors(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult MemberShip(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        } 
        public ActionResult BusinessRoom(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }
        public ActionResult TimeTable(long businessOwnerLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult MasterInstructorDetail(long userLoginId,long businessOwnerLoginId,long instructorUserLoginId)
        {
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            ViewBag.UserLoginId = userLoginId;
            ViewBag.InstructorUserLoginId = instructorUserLoginId;
            return View();
        }

        public ActionResult ResetPasswordDetail(string token)
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
                resetPasswordViewModel.Token = token;
                
                return View(resetPasswordViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Status = -1;
                ViewBag.ErrorMessage = "Internal Server Error!";
                return View();
            }
        }


        ///// <summary>
        ///// To Forgot Password
        ///// </summary>
        ///// <param name="forgotPasswordViewModel"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel, string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    try
        //    {

        //        if (forgotPasswordViewModel == null)
        //        {
        //            ViewBag.Message = "Invalid Data!";
        //            return View(forgotPasswordViewModel);
        //            // return Json(new { status = 1, message = "Invalid Data" });
        //        }

        //        ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();
        //        List<User_VM> lstLogin = new List<User_VM>();
        //        User_VM objUser = null;


        //        SqlParameter[] queryParams = new SqlParameter[] {
        //                    new SqlParameter("id", "0"),
        //                    new SqlParameter("uniqueUserId",""),
        //                    new SqlParameter("email", forgotPasswordViewModel.Email),
        //                    new SqlParameter("password", ""),
        //                    new SqlParameter("roleId",forgotPasswordViewModel.RoleId),
        //                    new SqlParameter("mode", "2")
        //                };

        //        lstLogin = db.Database.SqlQuery<User_VM>("exec sp_ManageUserLogin @id,@uniqueUserId,@email,@password,@roleId,@mode", queryParams).ToList();

        //        if (lstLogin.Count > 0)
        //        {
        //            objUser = lstLogin.First();
        //        }

        //        if (objUser != null)
        //        {
        //            // Fix the variable mismatch
        //            objUser = (objUser.Email == forgotPasswordViewModel.Email) ? objUser : null;
        //        }

        //        //--Check if Username Not-Exists--
        //        if (objUser == null)
        //        {
        //            // return Json(new { status = 1, message = "Sorry, Email does not exist! " });
        //            //--Create "Username not exists" Response--
        //            ViewBag.Message = "Sorry, Email does not exist! ";
        //            return View(forgotPasswordViewModel);

        //        }
        //        //else if (objUser != null)
        //        //{
        //        //    //--Create "Username not exists" Response--
        //        //    // return Json(new { status = -1, message = "Your account is inactive. Contact your admin for activation. " });
        //        //    ViewBag.Message = "Your account is inactive. Contact your admin for activation.";
        //        //    return View(forgotPasswordViewModel);
        //        //}
        //        else
        //        {


        //            #region Send Forgot-Password Email to the Admin

        //            ResetPasswordToken_VM resetPasswordToken_VM = new ResetPasswordToken_VM
        //            {
        //                UserId = objUser.Id,
        //                ValidTill_UTCDateTime = DateTime.UtcNow.AddHours(1)
        //            };
        //            string serializedToken = JsonConvert.SerializeObject(resetPasswordToken_VM).ToString();
        //            //--Encrypt the Admin-ID
        //            string Encrypted_Reset_Token = EDClass.Encrypt(serializedToken);
        //            //--Encode Encrypted-Admin-ID according to the URL Encoding
        //            //string url_encoded_AdminID = HttpContext.Current.Server.UrlEncode(Encrypted_Admin_Id);
        //            string url_encoded_token = HttpUtility.UrlEncode(Encrypted_Reset_Token);

        //            #region Update Token in User-Database -----------------
        //            //ManageUserLogin_SQL_Params_VM manageUserLogin_SQL_Params = new ManageUserLogin_SQL_Params_VM()
        //            //{
        //            //    Id = objUser.Id,
        //            //    ResetPasswordToken = Encrypted_Reset_Token,
        //            //    Mode = 1
        //            //};
        //            SP_InsertUpdateResetPasswordDetail_Param_VM SP_InsertUpdateResetPasswordDetail_Param = new SP_InsertUpdateResetPasswordDetail_Param_VM()
        //            {
        //                Id = objUser.Id,
        //                ResetPasswordToken = Encrypted_Reset_Token,
        //                Mode = 1
        //            };
        //            StoredProcedureRepository storedProcedureRepository = new StoredProcedureRepository(db);
        //            SPResponseViewModel updateTokenResponse = storedProcedureRepository.SP_InsertUpdateResetPasswordDetail<SPResponseViewModel>(SP_InsertUpdateResetPasswordDetail_Param);

        //            if (updateTokenResponse.ret != 1)
        //            {
        //                //--Create "Username not exists" Response--
        //                //return Json(new { status = -101, message = "Something went wrong. Please try again!" });
        //                ViewBag.Message = "Something went wrong. Please try again!";
        //                return View(forgotPasswordViewModel);

        //            }

        //            #endregion --------------------------------------------
        //            var URL = "";


        //            if (forgotPasswordViewModel.RoleId == 4 || forgotPasswordViewModel.RoleId == 5)
        //            {
        //                URL = ConfigurationManager.AppSettings["SiteURL"] + "/Business/ResetPassword?token=" + url_encoded_token;
        //            }
        //            else if (forgotPasswordViewModel.RoleId == 3)
        //            {
        //                URL = ConfigurationManager.AppSettings["SiteURL"] + "/Home/ResetPasswordDetail?token=" + url_encoded_token;
        //            }
        //            if (forgotPasswordViewModel.RoleId == 1 || forgotPasswordViewModel.RoleId == 2)
        //            {
        //                URL = ConfigurationManager.AppSettings["SiteURL"] + "/SuperAdmin/ResetPassword?token=" + url_encoded_token;
        //            }

        //            string Receiver_Name = ""; //"Admin / Student / Staff";
        //            string Receiver_Email = objUser.Email;
        //            string Subject = "Reset Password";
        //            string Message = @"<table>
        //                            <tr>
        //                                <td style='padding-bottom:20px;'>
        //                                    <h2>Reset Password</h2>
        //                                    Please click on the below link to reset your password.
        //                                </td>
        //                            </tr>
        //                            <tr>
        //                                <td style='text-align:center;'>
        //                                    <a href='" + URL + @"' style='padding:10px 20px;background-color:green;color:white;text-decoration:none;'>Click Here</a>
        //                                </td>
        //                            </tr>
        //                        </table>";



        //            ////// TODO: ResetPassword email link Comment This testing code and enable sending email
        //            ////-------------------------------------TESTING------------------------------ -
        //            ////--Create response as Successfully Sent Email
        //            //objResponse = new JsonResponseViewModel() { status = 1, message = "Reset-Password link has been successfully sent to your email address, please check!", data = new { token = URL } };

        //            ////sending response as OK
        //            //return Json(objResponse, JsonRequestBehavior.AllowGet);
        //            //////------------------------------------- TESTING ------------------------------- Returns back here

        //            EmailSender emailSender = new EmailSender();
        //            //SendEmail objEmail = new SendEmail();
        //            emailSender.Send(Receiver_Name, Subject, Receiver_Email, Message, "");
        //            #endregion

        //            //--Create response as Successfully Sent Email

        //            // return Json(new { status = -1, message = "Reset-Password link has been successfully sent to your email address, please check! " });
        //            //sending response as OK
        //            ViewBag.Message = "Reset-Password link has been successfully sent to your email address, please check! ";
        //            return View(forgotPasswordViewModel);
        //        }


        //    }

        //    catch (Exception ex)
        //    {


        //        //--Create response as Error

        //        //return Json(new { status = -100, message = "Internal Server Error! " });
        //        ViewBag.Message = ex.Message;
        //        ViewBag.Message = "Internal Server Error!";
        //        return View(forgotPasswordViewModel);

        //    }
        //}


        ///// <summary>
        ///// To Update Reset Password 
        ///// </summary>
        ///// <param name="resetPasswordViewModel"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        //{
        //    ViewBag.AppName = ConfigurationManager.AppSettings["First_Word_AppName"] + " " + ConfigurationManager.AppSettings["Second_Word_AppName"];

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            ViewBag.IsValid = false;
        //            ViewBag.ErrorMessage("All fields are required!");
        //            return View(resetPasswordViewModel);
        //        }

        //        #region Decrypt Reset-Password Token & Check Token validity

        //        DateTime UTC_DateTime = DateTime.UtcNow;
        //        //DateTime UTC_DateTime = DateTime.UtcNow.AddHours(1);   // For Testing Token Expiry, uncomment this.
        //        string decryptedToken = EDClass.Decrypt(resetPasswordViewModel.Token);
        //        ResetPasswordToken_VM resetPasswordToken_VM = JsonConvert.DeserializeObject<ResetPasswordToken_VM>(decryptedToken);
        //        bool is_Valid = true;

        //        #region Get User-Details in User-Database -----------------
        //        SP_InsertUpdateResetPasswordDetail_Param_VM SP_InsertUpdateResetPasswordDetail_Param_VM = new SP_InsertUpdateResetPasswordDetail_Param_VM()
        //        {
        //            Id = resetPasswordToken_VM.UserId,
        //            Mode = 2
        //        };
        //        StoredProcedureRepository storedProcedureRepository = new StoredProcedureRepository(db);

        //        User_VM updateTokenResponse = storedProcedureRepository.SP_InsertUpdateResetPasswordDetail<User_VM>(SP_InsertUpdateResetPasswordDetail_Param_VM);


        //        if (updateTokenResponse == null || String.IsNullOrEmpty(updateTokenResponse.ResetPasswordToken) || (updateTokenResponse.ResetPasswordToken != resetPasswordViewModel.Token))
        //        {
        //            is_Valid = false;
        //            ViewBag.ErrorMessage = "Invalid Token or Link has been expired!";
        //            ViewBag.IsTokenValid = is_Valid;
        //            return View(resetPasswordViewModel);
        //        }

        //        #endregion --------------------------------------------

        //        var dateTime_diff = resetPasswordToken_VM.ValidTill_UTCDateTime - UTC_DateTime;
        //        //validate date by having same date and hours difference.
        //        // Difference of Total Hours will be negative if exceeding validTill-DateTime-Hours.
        //        if (Convert.ToInt32(dateTime_diff.TotalDays) == 0 && dateTime_diff.TotalHours >= 0)
        //        {
        //            // token valid
        //            is_Valid = true;
        //        }
        //        else
        //        {
        //            is_Valid = false;
        //            ViewBag.ErrorMessage = "Link has been expired!";
        //            ViewBag.IsTokenValid = is_Valid;
        //            return View(resetPasswordViewModel);
        //        }
        //        #endregion

        //        Int64 userId = resetPasswordToken_VM.UserId;

        //        #region LinQ code commented
        //        //UserLogin userLogin = db.UserLogin.FirstOrDefault(u => u.Id == userId);

        //        //if (userLogin == null)
        //        //{
        //        //    ViewBag.IsValid = false;
        //        //    ViewBag.ErrorMessage("Invalid User!");
        //        //    return View(resetPasswordViewModel);
        //        //}

        //        //userLogin.Password = EDClass.Encrypt(resetPasswordViewModel.Password);
        //        //db.SaveChanges();
        //        #endregion

        //        SqlParameter[] queryParams = new SqlParameter[] {
        //        new SqlParameter("id", userId),
        //        new SqlParameter("uniqueUserId", ""),
        //        new SqlParameter("email", ""),
        //        new SqlParameter("password", EDClass.Encrypt(resetPasswordViewModel.Password)),
        //        new SqlParameter("roleId",""),
        //        new SqlParameter("mode", "3")
        //        };

        //        SPResponseViewModel resetResponse = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageUserLogin @id,@uniqueUserId,@email,@password,@roleId,@mode", queryParams).FirstOrDefault();

        //        if (resetResponse.ret == 1)
        //        {
        //            ViewBag.IsResetSuccessful = true;
        //            return View();
        //        }
        //        else
        //        {
        //            ViewBag.IsValid = false;
        //            ViewBag.ErrorMessage(resetResponse.responseMessage);
        //            return View(resetPasswordViewModel);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        ViewBag.Message = "Internal Server Error!";
        //        return View(resetPasswordViewModel);
        //    }
        //}


        public ActionResult Form(long customFormId, long businessOwnerLoginId)
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            ViewBag.CustomFormId = customFormId;
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        public ActionResult MyClassPurchaseDetails(long classid)
        {
            ViewBag.Id = classid;
            return View();
        }

        public ActionResult MyEventPurchaseDetails(long eventId)
        {
            ViewBag.Id = eventId;
            return View();
        }
        public ActionResult MyTrainingPurchaseDetails(long trainingId)
        {
            ViewBag.Id = trainingId;
            return View();
        }
        public ActionResult MyPlanPurchaseDetails(long planId , int planBookingType)
        {
            ViewBag.Id = planId;
            ViewBag.PlanBookingType = planBookingType;
            return View();
        }

        public ActionResult BusinessListing()
        {
            return View();
        }


        public ActionResult MyCoursePurchaseDetails(long courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }
        public ActionResult MyTraining()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myTraining");
            return View();
        }

        public ActionResult MyEvent()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("myEvent");
            return View();
        }

        public ActionResult GroupMessages()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("groupmessages");
            return View();
        }

        public ActionResult GroupMessageChat(string toUserLoginId )
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("groupMessageChat");
            ViewBag.GroupId = EDClass.Decrypt(toUserLoginId);
            return View();
        }

        public ActionResult SportBooking()
        {
            if (!ValidateStudentCookie())
            {
                return RedirectToAction("Login");
            }
            SetSidebarCookieInfo("sportBooking");

            return View();
        }

        public static class OtpGenerator
        {
            private static Random _random = new Random();

            public static string GenerateOtp()
            {
                const int otpLength = 4; // Change as needed
                var otp = new StringBuilder();
                for (int i = 0; i < otpLength; i++)
                {
                    otp.Append(_random.Next(0, 9)); // Generates a random number between 0-9
                }
                return otp.ToString();
            }
        }
    }
}