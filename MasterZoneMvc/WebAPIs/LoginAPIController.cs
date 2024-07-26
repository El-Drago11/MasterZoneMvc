using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Google.Apis.Requests.BatchRequest;


namespace MasterZoneMvc.WebAPIs
{
    public class LoginAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private LoginService loginService;
        private StudentService studentService;
        public LoginAPIController()
        {
            db = new MasterZoneDbContext();
            loginService = new LoginService(db);
            studentService = new StudentService(db);
        }


        [HttpPost]
        [Route("api/Login/Business")]
        public HttpResponseMessage BusinessLogin(LoginViewModel objLoginModel)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                // Validate infromation passed
                Error_VM error_VM = objLoginModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                //SqlParameter[] queryParams = new SqlParameter[] {
                //    new SqlParameter("id", "0"),
                //    new SqlParameter("email", objLoginModel.Email),
                //    new SqlParameter("password", EDClass.Encrypt(objLoginModel.Password)),
                //    new SqlParameter("mode", "2")
                //    };

                //var user = db.Database.SqlQuery<UserLoginViewModel>("exec sp_Login @id,@email,@password,@mode", queryParams).FirstOrDefault();

                LoginService_VM loginService_VM = new LoginService_VM()
                {
                    Email = objLoginModel.Email,
                    Password = objLoginModel.Password,
                    Mode = 2,
                    SocialLoginId = objLoginModel.SocialLoginId,
                    MasterId = objLoginModel.MasterId,
                };
                var user = loginService.GetLoginValidationInformation(loginService_VM);

                if (user == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Credential";
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else if (user.Status != 1)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Your Account is Inactive";
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                TokenGenerator tokenGenerator = new TokenGenerator();
                var _JWT_User_Token = tokenGenerator.Create_JWT(user.Id, "BusinessAdmin", 0);

                user.Token = _JWT_User_Token;
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = user;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpPost]
        [Route("api/Login/Student")]
        public HttpResponseMessage StudentLogin(LoginViewModel objLoginModel)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                // Validate infromation passed
                Error_VM error_VM = objLoginModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();
                //SPResponseViewModel resp = new SPResponseViewModel();
                //SqlParameter[] queryParams = new SqlParameter[] {
                //    new SqlParameter("id", "0"),
                //    new SqlParameter("email", objLoginModel.Email),
                //    new SqlParameter("password", EDClass.Encrypt(objLoginModel.Password)),
                //    new SqlParameter("mode", "3")
                //    };

                //var user = db.Database.SqlQuery<UserLoginViewModel>("exec sp_Login @id,@email,@password,@mode", queryParams).FirstOrDefault();

                LoginService_VM loginService_VM = new LoginService_VM()
                {
                    Email = objLoginModel.Email,
                    Password = objLoginModel.Password,
                    Mode = 3,
                    SocialLoginId = objLoginModel.SocialLoginId,
                    MasterId = objLoginModel.MasterId,
                };
                var user = loginService.GetLoginValidationInformation(loginService_VM);


                if (user == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Credential";
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else if (user.Status != 1)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Your Account is Inactive";
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                TokenGenerator tokenGenerator = new TokenGenerator();
                var _JWT_User_Token = tokenGenerator.Create_JWT(user.Id, "Student", 0);

                user.Token = _JWT_User_Token;


                apiResponse.status = 1;
                apiResponse.message = Resources.VisitorPanel.StudentRegistered_SuccessMessage;
                apiResponse.data = user;
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
        /// Forgot Password, send email to the user.
        /// </summary>
        /// <param name="resetPasswordViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Login/ForgotPassword")]
        public HttpResponseMessage ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                // Validate infromation passed
                Error_VM error_VM = forgotPasswordViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                List<User_VM> lstLogin = new List<User_VM>();
                User_VM objUser = null;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("uniqueUserId",""),
                            new SqlParameter("email", forgotPasswordViewModel.Email),
                            new SqlParameter("password", ""),
                            new SqlParameter("roleId",forgotPasswordViewModel.RoleId),
                            new SqlParameter("mode", "2")
                        };

                lstLogin = db.Database.SqlQuery<User_VM>("exec sp_ManageUserLogin @id,@uniqueUserId,@email,@password,@roleId,@mode", queryParams).ToList();

                if (lstLogin.Count > 0)
                {
                    objUser = lstLogin.First();
                }

                if (objUser != null)
                {
                    // Fix the variable mismatch
                    objUser = (objUser.Email == forgotPasswordViewModel.Email) ? objUser : null;
                }

                //--Check if Username Not-Exists--
                if (objUser == null)
                {
                    // return Json(new { status = 1, message = "Sorry, Email does not exist! " });
                    //--Create "Username not exists" Response--
                    apiResponse.status = -1;
                    apiResponse.message = "Sorry, Email does not exist!";
                    apiResponse.data = forgotPasswordViewModel;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    #region Send Forgot-Password Email to the Admin

                    ResetPasswordToken_VM resetPasswordToken_VM = new ResetPasswordToken_VM
                    {
                        UserId = objUser.Id,
                        ValidTill_UTCDateTime = DateTime.UtcNow.AddHours(1)
                    };
                    string serializedToken = JsonConvert.SerializeObject(resetPasswordToken_VM).ToString();
                    //--Encrypt the Admin-ID
                    string Encrypted_Reset_Token = EDClass.Encrypt(serializedToken);
                    //--Encode Encrypted-Admin-ID according to the URL Encoding
                    //string url_encoded_AdminID = HttpContext.Current.Server.UrlEncode(Encrypted_Admin_Id);
                    string url_encoded_token = HttpUtility.UrlEncode(Encrypted_Reset_Token);

                    #region Update Token in User-Database -----------------
                    
                    SP_InsertUpdateResetPasswordDetail_Param_VM SP_InsertUpdateResetPasswordDetail_Param = new SP_InsertUpdateResetPasswordDetail_Param_VM()
                    {
                        Id = objUser.Id,
                        ResetPasswordToken = Encrypted_Reset_Token,
                        Mode = 1
                    };
                    StoredProcedureRepository storedProcedureRepository = new StoredProcedureRepository(db);
                    SPResponseViewModel updateTokenResponse = storedProcedureRepository.SP_InsertUpdateResetPasswordDetail<SPResponseViewModel>(SP_InsertUpdateResetPasswordDetail_Param);

                    if (updateTokenResponse.ret != 1)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = "Something went wrong. Please try again!";
                        apiResponse.data = forgotPasswordViewModel;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    #endregion --------------------------------------------
                    var URL = "";


                    if (forgotPasswordViewModel.RoleId == 4 || forgotPasswordViewModel.RoleId == 5)
                    {
                        URL = ConfigurationManager.AppSettings["SiteURL"] + "/Business/ResetPassword?token=" + url_encoded_token;
                    }
                    else if (forgotPasswordViewModel.RoleId == 3)
                    {
                        URL = ConfigurationManager.AppSettings["SiteURL"] + "/Home/ResetPasswordDetail?token=" + url_encoded_token;
                    }
                    if (forgotPasswordViewModel.RoleId == 1 || forgotPasswordViewModel.RoleId == 2)
                    {
                        URL = ConfigurationManager.AppSettings["SiteURL"] + "/SuperAdmin/ResetPassword?token=" + url_encoded_token;
                    }

                    string Receiver_Name = ""; //"Admin / Student / Staff";
                    string Receiver_Email = objUser.Email;
                    string Subject = "Reset Password";
                    string Message = @"<table>
                                    <tr>
                                        <td style='padding-bottom:20px;'>
                                            <h2>Reset Password</h2>
                                            Please click on the below link to reset your password.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:center;'>
                                            <a href='" + URL + @"' style='padding:10px 20px;background-color:green;color:white;text-decoration:none;'>Click Here</a>
                                        </td>
                                    </tr>
                                </table>";



                    ////// TODO: ResetPassword email link Comment This testing code and enable sending email
                    ////-------------------------------------TESTING------------------------------ -
                    ////--Create response as Successfully Sent Email
                    //objResponse = new JsonResponseViewModel() { status = 1, message = "Reset-Password link has been successfully sent to your email address, please check!", data = new { token = URL } };

                    ////sending response as OK
                    //return Json(objResponse, JsonRequestBehavior.AllowGet);
                    //////------------------------------------- TESTING ------------------------------- Returns back here

                    EmailSender emailSender = new EmailSender();
                    //SendEmail objEmail = new SendEmail();
                    emailSender.Send(Receiver_Name, Subject, Receiver_Email, Message, "");
                    #endregion

                    //--Create response as Successfully Sent Email
                    apiResponse.status = 1;
                    apiResponse.message = "Reset-Password link has been successfully sent to your email address, please check! ";
                    apiResponse.data = new {
                        ResetPasswordLink = URL
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Reset Password (New password set) for any user by Token containing UserLogin-Id.
        /// </summary>
        /// <param name="resetPasswordViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Login/ResetPassword")]
        public HttpResponseMessage ResetPassword(ResetPassword_VM resetPasswordViewModel)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                // Validate infromation passed
                Error_VM error_VM = resetPasswordViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                #region Decrypt Reset-Password Token & Check Token validity

                DateTime UTC_DateTime = DateTime.UtcNow;
                //DateTime UTC_DateTime = DateTime.UtcNow.AddHours(1);   // For Testing Token Expiry, uncomment this.
                string decryptedToken = EDClass.Decrypt(resetPasswordViewModel.Token);
                ResetPasswordToken_VM resetPasswordToken_VM = JsonConvert.DeserializeObject<ResetPasswordToken_VM>(decryptedToken);
                bool is_Valid = true;

                #region Get User-Details in User-Database -----------------
               
                StoredProcedureRepository storedProcedureRepository = new StoredProcedureRepository(db);

                User_VM updateTokenResponse = storedProcedureRepository.SP_InsertUpdateResetPasswordDetail<User_VM>(new ViewModels.StoredProcedureParams.SP_InsertUpdateResetPasswordDetail_Param_VM()
                {
                    Id = resetPasswordToken_VM.UserId,
                    Mode = 2
                });


                if (updateTokenResponse == null || String.IsNullOrEmpty(updateTokenResponse.ResetPasswordToken) || (updateTokenResponse.ResetPasswordToken != resetPasswordViewModel.Token))
                {
                    apiResponse.status = -2;
                    apiResponse.message = "Invalid Token or Link has been expired!";
                    apiResponse.data = new { isTokenValid = false};
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                #endregion --------------------------------------------

                var dateTime_diff = resetPasswordToken_VM.ValidTill_UTCDateTime - UTC_DateTime;
                //validate date by having same date and hours difference.
                // Difference of Total Hours will be negative if exceeding validTill-DateTime-Hours.
                if (Convert.ToInt32(dateTime_diff.TotalDays) == 0 && dateTime_diff.TotalHours >= 0)
                {
                    // token valid
                    is_Valid = true;
                }
                else
                {
                    apiResponse.status = -2;
                    apiResponse.message = "Link has been expired!";
                    apiResponse.data = new { isTokenValid = false };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);

                }
                #endregion

                Int64 userId = resetPasswordToken_VM.UserId;

                #region LinQ code commented
                //UserLogin userLogin = db.UserLogin.FirstOrDefault(u => u.Id == userId);

                //if (userLogin == null)
                //{
                //    ViewBag.IsValid = false;
                //    ViewBag.ErrorMessage("Invalid User!");
                //    return View(resetPasswordViewModel);
                //}

                //userLogin.Password = EDClass.Encrypt(resetPasswordViewModel.Password);
                //db.SaveChanges();
                #endregion

                SqlParameter[] queryParams = new SqlParameter[] {
                new SqlParameter("id", userId),
                new SqlParameter("uniqueUserId", ""),
                new SqlParameter("email", ""),
                new SqlParameter("password", EDClass.Encrypt(resetPasswordViewModel.Password)),
                new SqlParameter("roleId",""),
                new SqlParameter("mode", "3")
                };

                SPResponseViewModel resetResponse = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageUserLogin @id,@uniqueUserId,@email,@password,@roleId,@mode", queryParams).FirstOrDefault();

                if (resetResponse.ret == 1)
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = new {};
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    apiResponse.status = -1;
                    apiResponse.message = resetResponse.responseMessage;
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// To Register And Login Google Api 
        /// </summary>
        /// <param name="objLoginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Login/SocialLogin")]
        public HttpResponseMessage GoogleLogin(Login_VM objLoginModel)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                // Validate infromation passed
                Error_VM error_VM = objLoginModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();


                LoginService_VM loginService_VM = null;

                if (objLoginModel.SocialMediaType == "Google")
                {
                    loginService_VM = new LoginService_VM()
                    {
                        Email = objLoginModel.Email,
                        Password = "",
                        Mode = 4,
                        SocialLoginId = objLoginModel.SocialLoginId,
                        MasterId = "",
                    };
                }

                else if (objLoginModel.SocialMediaType == "Facebook")
                {
                    loginService_VM = new LoginService_VM()
                    {
                        Email = objLoginModel.Email,
                        Password = "",
                        Mode = 5,
                        SocialLoginId = objLoginModel.SocialLoginId,
                        MasterId = "",
                    };
                }

                var user = loginService.GetLoginValidationInformation(loginService_VM);

                if (user == null)
                {
                    // User is not found, register them
                    studentService = new StudentService(db);
                    var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                    {
                        Email = objLoginModel.Email,
                        RoleId = 3,
                        PhoneNumberCountryCode = "+91",
                        FirstName = objLoginModel.FirstName,
                        LastName = "",
                        Mode = 1
                    });
                    if (resp.ret <= 0)
                    {
                        apiResponse.status = resp.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    // Registration successful, send email
                    EmailSender emailSender = new EmailSender();
                    emailSender.Send(objLoginModel.FirstName, "Registration successful", objLoginModel.Email, "You have been successfully registered with Masterzone. Your MasterID is: " + resp.MasterId, "");

                    // Generate token
                    TokenGenerator tokenGenerators = new TokenGenerator();
                    var _JWT_User_Token = tokenGenerators.Create_JWT(resp.Id, "Student", 0);

                    // Update user record with social login information
                    if (resp.Id != 0)
                    {
                        SqlParameter[] queryParam = new SqlParameter[]
                        {
                        new SqlParameter("id",resp.Id),
                        new SqlParameter("googleUserId",objLoginModel.SocialLoginId),
                        new SqlParameter("facebookUserId",""),
                        new SqlParameter("googleAccessToken",_JWT_User_Token),
                        new SqlParameter("facebookAccessToken",""),
                        new SqlParameter("submittedByLoginId","0"),
                        new SqlParameter("mode","1")
                        };

                        var respp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStudentWithSocial @id,@googleUserId,@facebookUserId,@googleAccessToken,@facebookAccessToken,@submittedByLoginId,@mode", queryParam).FirstOrDefault();

                        // Update response
                        apiResponse.status = 1;
                        apiResponse.message = "Registration successful. Your MasterID is: " + resp.MasterId;
                        apiResponse.data = new { token = _JWT_User_Token };
                        
                        
                    }
                }
                else if (user.Status != 1)
                {
                    // User exists but account is inactive
                    apiResponse.status = -1;
                    apiResponse.message = "Your Account is Inactive";
                    apiResponse.data = new { };
                   
                    
                }
                else
                {
                    // User exists and account is active, generate token
                    TokenGenerator tokenGenerator = new TokenGenerator();
                    var _JWT_User_Tokens = tokenGenerator.Create_JWT(user.Id, "Student", 0);
                    // Update response
                    apiResponse.status = 1;
                    apiResponse.message = "Success";
                    apiResponse.data = new { token = _JWT_User_Tokens };
                    
                    
                }

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