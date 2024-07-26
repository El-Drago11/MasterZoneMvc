using MasterZoneMvc.Models;
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
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.Common;
using System.Data.SqlClient;
using MasterZoneMvc.DAL;
using System.IO;
using System.Configuration;
using MasterZoneMvc.Services;
using MasterZoneMvc.Common.Filters;
using System.Drawing.Printing;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.Text.RegularExpressions;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.Controllers
{
    public class SuperAdminController : Controller
    {
        private MasterZoneDbContext db;
        private LoginService loginService;
        public SuperAdminController()
        {
            db = new MasterZoneDbContext();
            loginService = new LoginService(db);
        }

        private bool ValidateSuperAdminCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.SuperAdminCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }
        private bool ValidateSubAdminCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.SubAdminCookie];

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

        public JsonResult GetSuperAdminToken()
        {
            string _Token = "";
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.SuperAdminCookie];

            if (myCookie != null)
            {
                _Token = myCookie["UserToken"].ToString();
            }
            return Json(_Token, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubAdminToken()
        {
            string _Token = "";
            HttpCookie myCookie = Request.Cookies[CookieKeyNames.SubAdminCookie];

            if (myCookie != null)
            {
                _Token = myCookie["UserToken"].ToString();
            }
            return Json(_Token, JsonRequestBehavior.AllowGet);
        }

        // GET: SuperAdmin
        public ActionResult Index()
        {
            if (ValidateSuperAdminCookie())
            {
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Login");
        }
       
        public ActionResult Login()
        {
            if (ValidateSuperAdminCookie() || ValidateSubAdminCookie())
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
                if (objLoginModel == null || String.IsNullOrEmpty(objLoginModel.Email) || String.IsNullOrEmpty(objLoginModel.Password))
                {
                    ViewBag.Message = "Please enter required fields!";
                    return View(objLoginModel);
                }


                #region SuperAdmin login, validate if success and claim user login ---------------------------------------

                //SqlParameter[] queryParams = new SqlParameter[] {
                //    new SqlParameter("id", "0"),
                //    new SqlParameter("email", objLoginModel.Email),
                //    new SqlParameter("password", EDClass.Encrypt(objLoginModel.Password)),
                //    new SqlParameter("mode", "1")
                //    };

                //UserLoginViewModel user = db.Database.SqlQuery<UserLoginViewModel>("exec sp_Login @id,@email,@password,@mode", queryParams).FirstOrDefault();

                LoginService_VM loginService_VM = new LoginService_VM()
                {
                    Email = objLoginModel.Email,
                    Password = objLoginModel.Password,
                    Mode = 1,
                    SocialLoginId = "",
                    MasterId = "",
                };
                var user = loginService.GetLoginValidationInformation(loginService_VM);


                if (user == null)
                {
                    //Add logic here to display some message to user    
                    ViewBag.Message = "Invalid Credential";
                    return View(objLoginModel);
                }
                else if(user.Status != 1)
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
                    var _JWT_User_Token = tokenGenerator.Create_JWT(user.Id, user.RoleName, 0);

                    //Create Admin login cookie
                    var cookieName = (user.RoleName == "SuperAdmin") ? CookieKeyNames.SuperAdminCookie : CookieKeyNames.SubAdminCookie;
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

        public ActionResult ForgotPassword(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult Dashboard()
        {
            SetSidebarCookieInfo("dashboard");
            return View();
        }

        //public ActionResult ContactUs()
        //{
        //    return View();
        //}
        //public ActionResult About()
        //{
        //    return View();
        //}
       
        public ActionResult Register()
        {
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageBusiness()
        {
            SetSidebarCookieInfo("manageBusiness");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult UserAccount()
        {
            SetSidebarCookieInfo("userAccount");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult Payment()
        {
            SetSidebarCookieInfo("managePayments");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageCertificate()
        {
            SetSidebarCookieInfo("manageCertificates");
            return View();
        }
        
        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageLicense(long id)
        {
            ViewBag.CertificateId = id;
            SetSidebarCookieInfo("manageLicenses");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageLicenseBookings()
        {
            SetSidebarCookieInfo("manageLicenseBookings");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageCategory()
        {
            SetSidebarCookieInfo("manageCategories");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult AccountSetting()
        {
            SetSidebarCookieInfo("manageAccountSettings");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageAdvertisement()
        {
            SetSidebarCookieInfo("manageAdvertisements");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult Backup()
        {
            ViewBag.Status = 0;
            ViewBag.Message = "";

            SetSidebarCookieInfo("manageBackup");
            return View();
        }

        [HttpPost]
        [SuperAdminPanelPermissionActionFilter]
        public ActionResult BackupDatabase(string type = "bak")
        {
            if (!ValidateSuperAdminCookie())
            {
                return RedirectToAction("Login");
            }
            ViewBag.Status = 0;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MasterZoneDbContext"].ConnectionString;
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string databaseName = connection.Database;
                    string backupName = databaseName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".bak";
                    string backupPath = Path.Combine(Server.MapPath("~/Content/Uploads/Backups"), backupName);
                    
                    using (SqlCommand command = new SqlCommand($"BACKUP DATABASE {databaseName} TO DISK='{backupPath}'", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(backupPath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, backupName);
                }


                //ViewBag.Status = 1;
                //ViewBag.Message = "Database backup created successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Status = -1;
                ViewBag.Message = "Error creating database backup: " + ex.Message;
            }

            return View("Backup");
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManagePayment()
        {
            return View();
        }

        //public ActionResult AddMessage()
        //{
        //    if (!ValidateSuperAdminCookie())
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    return View();
        //}
        //public ActionResult AddNewAdvertisement()
        //{
        //    if (!ValidateSuperAdminCookie())
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    return View();
        //}

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageSubAdmin()
        {
            SetSidebarCookieInfo("manageSubAdmin");
            return View();
        }

        //public ActionResult EditSub()
        //{
        //    return View();
        //}
        //public ActionResult CreateSubAdmin()
        //{
        //    return View();
        //}
        //public ActionResult ManageRole()
        //{
        //    return View();
        //}

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ContactDetail()
        {
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult BusinessDetail(Int64 businessOwnerLoginId)
        {
            SetSidebarCookieInfo("manageBusiness");

            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageMenu()
        {
            
            SetSidebarCookieInfo("manageMenu");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManagePackage()
        {
            SetSidebarCookieInfo("managePackage");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult StudentDetail(Int64 userLoginId)
        {
            SetSidebarCookieInfo("userAccount");
            ViewBag.UserLoginId = userLoginId;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult UserImageDetail(Int64 userLoginId)
        {
            SetSidebarCookieInfo("userAccount");

            ViewBag.UserLoginId = userLoginId;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult UserVedioDetail(Int64 userLoginId)
        {
            SetSidebarCookieInfo("userAccount");

            ViewBag.UserLoginId = userLoginId;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult BusinessUserImageDetail(Int64 businessOwnerLoginId)
        {
            SetSidebarCookieInfo("manageBusiness");

            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult BusinessUserVedioDetail(Int64 businessOwnerLoginId)
        {
            SetSidebarCookieInfo("manageBusiness");
            
            ViewBag.BusinessOwnerLoginId = businessOwnerLoginId;
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageLanguage()
        {
            if (!ValidateSuperAdminCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("manageLanguage");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageUniversity()
        {
            if (!ValidateSuperAdminCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("manageUniversity");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageCourseCategory()
        {
            if (!ValidateSuperAdminCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("manageCourseCategory");
            return View();
        }

        public ActionResult SubAdminAccountSetting()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateInput(false)]
        public FileResult Download_MainDepartment_ReportasPDF()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //#region Save Graph-Image into the Temp-Folder

                //string[] arrBase64Image = hdn_MainDepartment_Graph_base64.Split(',');

                //string ImagePath = "/Content/ReportsTemp/";
                //string ImageName = "MainDepartmentGraph_" + Guid.NewGuid().ToString().Split('-')[0] + Guid.NewGuid().ToString().Split('-')[1] + Guid.NewGuid().ToString().Split('-')[2] + Guid.NewGuid().ToString().Split('-')[3] + ".png";
                //string ImageFile = arrBase64Image[1];

                //var uploadStatus = objImage.SaveImage(ImageFile, ImageName, ImagePath);
                //#endregion

                //#region Set Graph-Image Data
                //string _ImageURL = _SiteURL + ImagePath + ImageName;
                //string _GraphImage_HTML = @"<div>
                //                            <img src='" + _ImageURL + @"' />
                //                            </div>";
                //#endregion

                string hdn_MainDepartmentReport_Html = @"<html><style>.cls { color:red; }</style><body width='100%' height='100%'>
                            <div class='cls' style='width:100%; height:50px; background:#cecece;align:center;'>Hello</div>
                            <img src='https://marketplace.canva.com/EAFNlUJs5g4/2/0/1600w/canva-white-simple-certificate-of-appreciation-Fcz7KkZ5YaU.jpg' style='width:100%;height:100%'/>
                </body></html>";

                StringReader sr = new StringReader(hdn_MainDepartmentReport_Html);
                Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                //iTextSharp.text.Document doc;
                pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", "MZ_Certificate_DEMO.pdf");
            }
        }

        /// <summary>
        /// License Preview Certificate Downlaod.
        /// </summary>
        /// <param name="licenseCertificateTemplate_Html">License HTML Template</param>
        /// <param name="licenseId">License-Id</param>
        /// <returns>PDF file</returns>
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Download_LicenseCertificatePreviewPDF(string licenseCertificateTemplate_Html, long licenseId)
        {
            // Get License and certificate data by License Id
            LicenseService licenseService = new LicenseService(db);
            var licenseData = licenseService.GetLicenseRecordDataById(licenseId);

            CertificateService certificateService = new CertificateService(db);
            var certificateData = certificateService.GetCertificateById(licenseData.CertificateId);

            // Bind Abbrivations with data
            var bindedContent = LicenseCertificateGenerator.BindValuesInCertificateHTML(licenseCertificateTemplate_Html, new LicenseCertificateHTMLContent_VM()
            {
                UserFirstName = "John",
                UserLastName = "Doe",
                CertificateId = licenseData.CertificateId,
                CertificateLogoPath = certificateData.CertificateIconWithPath,
                CertificateTitle = certificateData.Name,
                LicenseId = licenseId,
                LicenseLogoPath = licenseData.LicenseLogoImageWithPath,
                LicenseTitle = licenseData.Title,
                LicenseDescription = licenseData.Description,
                Signature1Path = licenseData.SignatureImageWithPath, 
                Signature2Path = licenseData.Signature2ImageWithPath,
                Signature3Path = licenseData.Signature3ImageWithPath,
                TimePeriod = licenseData.TimePeriod,
                UniqueCertificateNumber = "PREV_1234",
                IssueDate = DateTime.UtcNow,
                IssueDate_Format = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            });

            // Generate and return PDF document
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(bindedContent);
                Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "LicenseCertificatePreview.pdf");
            }
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageSponsors()
        {
            if (!ValidateSuperAdminCookie())
            {
                return RedirectToAction("Login");
            }

            SetSidebarCookieInfo("manageSponsors");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageClassCategory()
        {
            SetSidebarCookieInfo("manageClassCategory");
            return View();
        }

        /// <summary>
        /// Manage Sub-Categories of a Parent-Category
        /// </summary>
        /// <param name="id">Parent-Class-Category-Type-Id</param>
        /// <returns>View</returns>
        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageClassSubCategory(long id)
        {
            ViewBag.ParentClassCategoryTypeId = id;
            SetSidebarCookieInfo("manageClassCategory");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageAllClasses()
        {
            SetSidebarCookieInfo("manageAllClasses");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageAllTrainings()
        {
            SetSidebarCookieInfo("manageAllTrainings");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageAllEvents()
        {
            SetSidebarCookieInfo("manageAllEvents");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageAllInstructors()
        {
            SetSidebarCookieInfo("manageAllInstructors");
            return View();
        }
        
        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageHomePage()
        {
            SetSidebarCookieInfo("manageHomePage");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageMultipleImageSection()
        {
            SetSidebarCookieInfo("manageHomePage");
            return View();
        }
        
        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageMultipleVideoSection()
        {
            SetSidebarCookieInfo("manageHomePage");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageHomePageBannerItem()
        {
            SetSidebarCookieInfo("manageHomePage");
            return View();
        }

        [HttpPost]
        public FileResult DownloadBusinessDocument(string ExportData)
        {

            var filePath = Path.Combine(HttpContext.Server.MapPath("/Content/Uploads/Documents/"), ExportData);
            //return File(filePath, ExportData);

            return File(filePath, "application/octet-stream", Path.GetFileName(filePath));

        }


        public ActionResult Logout()
        {
            //SignOutAsync is Extension method for SignOut    
            /*await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page    
            return LocalRedirect("/");*/

            CookieHelper cookieHelper = new CookieHelper();
            cookieHelper.RemoveOrExpireLoginCookies(Response);
            return RedirectToAction("Login", "SuperAdmin");
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

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult CustomForm()
        {
            SetSidebarCookieInfo("customForm");
            return View();
        }
        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ViewCustomForm()
        {
            SetSidebarCookieInfo("viewCustomForm");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult TransferForm(long customformId)
        {
            ViewBag.CustomFormId = customformId;
            SetSidebarCookieInfo("transferForm");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageEventCategory()
        {
            SetSidebarCookieInfo("manageEventCategory");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageStaffCategory()
        {
            SetSidebarCookieInfo("manageStaffCategory");
            return View();
        }

        [SuperAdminPanelPermissionActionFilter]
        public ActionResult About()
        {
            SetSidebarCookieInfo("about");
            return View();
        }


        [SuperAdminPanelPermissionActionFilter]
        public ActionResult ManageVideo()
        {
           SetSidebarCookieInfo("manageVideo");
            return View();
        }




       
    }
}