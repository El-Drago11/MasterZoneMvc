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
using MasterZoneMvc.Services;
using System.Data.SqlClient;
using Google.Apis.Auth.OAuth2;
using GoogleAuthentication.Services;
using iTextSharp.text.pdf.qrcode;

namespace MasterZoneMvc.Controllers
{
    public class LoginController : Controller
    {
       
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult>Login(LoginViewModel objLoginModel)
        {
            try
            {
                if (objLoginModel == null || String.IsNullOrEmpty(objLoginModel.Email) || String.IsNullOrEmpty(objLoginModel.Password))
                {
                    ViewBag.Message = "Please enter required fields!";
                    return View(objLoginModel);
                }

                if (true)
                {
                    HttpClient client = new HttpClient();
                    ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                    using (var httpClient = new HttpClient())
                    {
                        LoginRequestViewModel loginRequestViewModel = new LoginRequestViewModel()
                        {
                            Email = objLoginModel.Email,
                            Password = objLoginModel.Password,
                            RememberLogin = objLoginModel.RememberLogin
                        };
                        StringContent content = new StringContent(JsonConvert.SerializeObject(loginRequestViewModel), Encoding.UTF8, "application/json");
                        var jsonconv = JsonConvert.SerializeObject(loginRequestViewModel);
                        //httpClient.DefaultRequestHeaders.ConnectionClose = true;
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        using (var response = await httpClient.PostAsync( "" +"/api/Login", content))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();

                            if (String.IsNullOrEmpty(apiResponse))
                            {
                                return View(objLoginModel);
                            }

                            apiResponseViewModel = JsonConvert.DeserializeObject<ApiResponseViewModel<UserLoginViewModel>>(apiResponse);

                            #region validate if success and claim user login ---------------------------------------

                            UserLoginViewModel user = apiResponseViewModel.data;
                            if (user == null || apiResponseViewModel.status < 0)
                            {
                                //Add logic here to display some message to user    
                                ViewBag.Message = "Invalid Credential";
                                return View(objLoginModel);
                            }
                            else
                            {
                                //A claim is a statement about a subject by an issuer and    
                                //represent attributes of the subject that are useful in the context of authentication and authorization operations.    
                                //var claims = new List<Claim>() {
                                //    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
                                //    new Claim(ClaimTypes.Email, user.Email),
                                //    new Claim(ClaimTypes.Role, user.Role),
                                //    new Claim("UserLoginId", user.Id.ToString()),
                                //    new Claim("ApiToken", user.Token)
                                //};

                               /* //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                                var principal = new ClaimsPrincipal(identity);
                                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                                {
                                    IsPersistent = objLoginModel.RememberLogin
                                });*/


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
                                    /*return LocalRedirect(objLoginModel.ReturnUrl);*/
                                }
                            }
                            #endregion ------------------------------------------------------------------------

                        }
                    }


                }


            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objLoginModel);
            }
            return View(objLoginModel);
        }

        public ActionResult Logout()
        {
            //SignOutAsync is Extension method for SignOut    
            /*await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page    
            return LocalRedirect("/");*/

            CookieHelper cookieHelper = new CookieHelper();
            cookieHelper.RemoveOrExpireLoginCookies(Response);
            return RedirectToAction("Index","Home");
        }


    }
}
