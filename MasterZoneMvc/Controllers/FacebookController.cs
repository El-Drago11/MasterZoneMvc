using Facebook;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterZoneMvc.Controllers
{
    public class FacebookController : Controller
    {
        // GET: Facebook
        private MasterZoneDbContext db;
        private LoginService loginService;
        private StudentService studentService;

        private string _FacebookClientId = ConfigurationManager.AppSettings["FacebookClientId"];
        private string _FacebookClientSecret = ConfigurationManager.AppSettings["FacebookClientSecret"];

        public FacebookController()
        {
            db = new MasterZoneDbContext();
            loginService = new LoginService(db);
            studentService = new StudentService(db);
        }

        public ActionResult Index()
        {
            string domainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = _FacebookClientId,
                redirect_uri =  domainName + "/Facebook/FacebookRedirect",
                scope = "public_profile,email"
            });
            ViewBag.Url = loginUrl;
            return View();

        }
        public ActionResult FacebookRedirect(string code)
        {
            string domainName = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var fb = new FacebookClient();
            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = _FacebookClientId,
                client_secret = _FacebookClientSecret,
                redirect_uri = domainName + "/Facebook/FacebookRedirect",
                code = code
            });
            fb.AccessToken = result.access_token;

            dynamic me = fb.Get("/me?fields=name,email");
            string name = me.name;
            string email = me.email;
            string[] fullName = name.Split(' ');
            string firstName = fullName[0];
            string lastName = fullName[1];
            //return Json(me, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");
            //FacebookInfo_VM userInfo_VM = JsonConvert.DeserializeObject<FacebookInfo_VM>(me);
            //string firstName = userInfo_VM.name.first_name;
            //string lastName = userInfo_VM.name.last_name;

            //FacebookInfo_VM userInfo_VM = new FacebookInfo_VM();
            LoginViewModel objLoginModel = new LoginViewModel();
            //SqlParameter[] queryParams = new SqlParameter[] {
            //        new SqlParameter("id","0"),
            //        new SqlParameter("email", me.email),
            //        new SqlParameter("password", ""),
            //         new SqlParameter("socialLoginId", me.id),
            //        new SqlParameter("mode", "4")
            //        };

            //objLoginModel = db.Database.SqlQuery<LoginViewModel>("exec sp_Login @id,@email,@password,@socialLoginId,@mode", queryParams).FirstOrDefault();
            LoginService_VM loginService_VM = new LoginService_VM()
            {
                Email = me.email,
                Password = "",
                Mode = 5,
                SocialLoginId = me.id,
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
                var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                {
                    Email = me.email,
                    RoleId = 3,
                    PhoneNumberCountryCode = "+91",
                    FirstName = firstName,
                    LastName = lastName,
                    Mode = 1
                });
                if (resp.Id != 0)
                {
                    SqlParameter[] queryParam = new SqlParameter[]
                                        {
                                            new SqlParameter("id",resp.Id),
                                            new SqlParameter("googleUserId",""),
                                            new SqlParameter("facebookUserId",me.id),
                                            new SqlParameter("googleAccessToken",""),
                                            new SqlParameter("facebookAccessToken",fb.AccessToken),
                                            new SqlParameter("submittedByLoginId","0"),
                                            new SqlParameter("mode","2")
                                        };

                    var respp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStudentWithSocial @id,@googleUserId,@facebookUserId,@googleAccessToken,@facebookAccessToken,@submittedByLoginId,@mode", queryParam).FirstOrDefault();
                    if (respp.ret <= 0)
                    {
                        return Json(new { status = resp.ret, message = resp.responseMessage });
                    }
                    else
                    {
                        EmailSender emailSender = new EmailSender();
                        emailSender.Send(me.name + " " + me.name, "Registration successful", me.email, "You have been successfully registered with Masterzone", "");

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
    }
}