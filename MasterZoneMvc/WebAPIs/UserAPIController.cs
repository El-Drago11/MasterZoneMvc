using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using Newtonsoft.Json;
using System.Configuration;
using System.Web;
using MasterZoneMvc.Common.Helpers;
using Google.Apis.Auth.OAuth2;
using GoogleAuthentication.Services;
using iTextSharp.text.pdf.qrcode;
using static Google.Apis.Requests.BatchRequest;
using System.Data.SqlClient;

namespace MasterZoneMvc.WebAPIs
{
    /// <summary>
    /// This API is made to list APIs which are used to get data of user as a common instead of Business/Student specific
    /// </summary>
    public class UserAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private UserService userService;
        private UserLoginService userLoginService;
       


        public UserAPIController()
        {
            db = new MasterZoneDbContext();
            userService = new UserService(db);
            userLoginService = new UserLoginService(db);
          
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Visitor Menu Items List
        /// </summary>
        /// <returns></returns>
        [Route("api/User/GetUserBasicDetailByMasterId")]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [HttpGet]
        public HttpResponseMessage GetUserBasicDetailByMasterId(string masterId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                if (string.IsNullOrEmpty(masterId))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                int IsMasterIdExists = 0;

                var userDetail = userService.GetUserBasicDetailByMasterId(masterId);
                if (userDetail != null)
                {
                    IsMasterIdExists = 1;
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                }
                else
                {
                    IsMasterIdExists = 0;
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                }

                apiResponse.data = userDetail;

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
        /// Get All Visitor Menu Items List
        /// </summary>
        /// <returns></returns>
        [Route("api/User/SendEmailVerificationLink")]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin,Student")]
        [HttpPost]
        public HttpResponseMessage SendEmailVerificationLink()
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

                var userLoginDetail = userLoginService.GetUserLoginData(_LoginId);

                if (userLoginDetail != null)
                {
                    if(userLoginDetail.EmailConfirmed == 1)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = Resources.ErrorMessage.EmailAlreadyConfirmed;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    #region Send Email-Verification Link Email to the User

                    ResetPasswordToken_VM resetPasswordToken_VM = new ResetPasswordToken_VM
                    {
                        UserId = userLoginDetail.Id,
                        ValidTill_UTCDateTime = DateTime.UtcNow.AddHours(1)
                    };

                    string serializedToken = JsonConvert.SerializeObject(resetPasswordToken_VM).ToString();
                    //--Encrypt the Admin-ID
                    string Encrypted_Token = EDClass.Encrypt(serializedToken);
                    //--Encode Encrypted-Admin-ID according to the URL Encoding
                    string url_encoded_token = HttpUtility.UrlEncode(Encrypted_Token);

                    var URL = ConfigurationManager.AppSettings["SiteURL"] + "/Home/EmailConfirmation?token=" + url_encoded_token;

                    string Receiver_Name = "";
                    string Receiver_Email = userLoginDetail.Email;
                    string Subject = "Confirm Email";
                    string Message = @"<table>
                                    <tr>
                                        <td style='padding-bottom:20px;'>
                                            <h2>Confirm Email</h2>
                                            Please click on the button to confirm your email. Link is valid only for 1 hour.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:center;'>
                                            <a href='" + URL + @"' style='padding:10px 20px;background-color:green;color:white;text-decoration:none;'>Confirm Email</a>
                                        </td>
                                    </tr>
                                </table>";

                    EmailSender emailSender = new EmailSender();
                    emailSender.Send(Receiver_Name, Subject, Receiver_Email, Message, "");
                    #endregion

                    apiResponse.status = 1;
                    apiResponse.message = Resources.VisitorPanel.ConfirmationEmailSent_SuccessMessage;
                }
                else
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage;
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