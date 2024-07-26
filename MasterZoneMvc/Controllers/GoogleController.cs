using Google.Apis.Auth.OAuth2;
using GoogleAuthentication.Services;
using iTextSharp.text.pdf.qrcode;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MasterZoneMvc.Controllers
{
    public class GoogleController : Controller
    {
        private MasterZoneDbContext db;
        private LoginService loginService;
        private StudentService studentService;

        private string _GoogleClientId = ConfigurationManager.AppSettings["GoogleClientId"];
        private string _GoogleClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];

        public GoogleController()
        {
            db = new MasterZoneDbContext();
            loginService = new LoginService(db);
            studentService = new StudentService(db);
        }

        // GET: Google
        public ActionResult Index()
        {
            string domainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var clientId = _GoogleClientId;
            var url = domainName + "/Google/GoogleLoginCallback";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.response = response;
            return View();
        }

        public async Task<ActionResult> GoogleLoginCallback(string code)
        {
            string domainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var clientId = _GoogleClientId;
            var url = domainName + "/Google/GoogleLoginCallback";
            var clientsecret = _GoogleClientSecret;
            var token = await GoogleAuth.GetAuthAccessToken(code, clientId, clientsecret, url);
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
            //return Json(userProfile, JsonRequestBehavior.AllowGet);
            UserInfo_VM userInfo_VM = JsonConvert.DeserializeObject<UserInfo_VM>(userProfile);

            LoginViewModel objLoginModel = new LoginViewModel();

            LoginService_VM loginService_VM = new LoginService_VM()
            {
                Email = userInfo_VM.email,
                Password = "",
                Mode = 4,
                SocialLoginId = userInfo_VM.id,
                MasterId = "",
            };
            var user = loginService.GetLoginValidationInformation(loginService_VM);
            if (user != null)
            {
                // Remove/Expire Login Cookies
                CookieHelper cookieHelper = new CookieHelper();
                cookieHelper.RemoveOrExpireLoginCookies(Response);

                TokenGenerator tokenGenerator = new TokenGenerator();
                var _JWT_User_Token = tokenGenerator.Create_JWT(user.Id, "Student", 0);

                //Create Admin login cookie
                HttpCookie myCookie = new HttpCookie(CookieKeyNames.StudentCookie);
                myCookie["UserToken"] = _JWT_User_Token;
                myCookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(myCookie);

                return RedirectToAction("MyClass", "Home");

            }
            else
            {
                if (userInfo_VM.family_name == null)
                {
                    userInfo_VM.family_name = string.Empty;
                }
                var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                {
                    Email = userInfo_VM.email,
                    RoleId = 3,
                    PhoneNumberCountryCode = "+91",
                    FirstName = userInfo_VM.given_name,
                    LastName = userInfo_VM.family_name,
                    Mode = 1
                });
                if (resp.Id != 0)
                {
                    SqlParameter[] queryParam = new SqlParameter[]
                                        {
                                            new SqlParameter("id",resp.Id),
                                            new SqlParameter("googleUserId",userInfo_VM.id),
                                            new SqlParameter("facebookUserId",""),
                                            new SqlParameter("googleAccessToken",token.AccessToken),
                                            new SqlParameter("facebookAccessToken",""),
                                            new SqlParameter("submittedByLoginId","0"),
                                            new SqlParameter("mode","1")
                                        };

                    var respp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStudentWithSocial @id,@googleUserId,@facebookUserId,@googleAccessToken,@facebookAccessToken,@submittedByLoginId,@mode", queryParam).FirstOrDefault();
                    if (respp.ret <= 0)
                    {
                        return Json(new { status = resp.ret, message = resp.responseMessage });
                    }
                    else
                    {
                        EmailSender emailSender = new EmailSender();
                        emailSender.Send(userInfo_VM.given_name + " " + userInfo_VM.family_name, "Registration successful", userInfo_VM.email, "You have been successfully registered with Masterzone", "");

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

                        return RedirectToAction("MyClass", "Home");
                    }
                }

                else
                {
                    return Json(new { status = resp.ret, message = resp.responseMessage });
                }



            }

        }
        public async Task<ActionResult> Logout()
        {
            CookieHelper cookieHelper = new CookieHelper();
            cookieHelper.RemoveOrExpireLoginCookies(Response);
            return RedirectToAction("Index", "Home");
        }




       

    }
}