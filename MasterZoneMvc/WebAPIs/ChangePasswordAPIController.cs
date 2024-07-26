using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class ChangePasswordAPIController : ApiController
    {
        private MasterZoneDbContext db;
        
        public ChangePasswordAPIController()
        {
            db = new MasterZoneDbContext();
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        //--Change Password-- 
        [Authorize(Roles = "BusinessAdmin,Staff,SubAdmin,SuperAdmin,Student")]
        [HttpPost]
        [Route("api/ChangePassword/Update")]
        public HttpResponseMessage UpdateUserPassword()
        {
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                //--Get all parameter's value of Form-Data (by Key-Name)
                string _OldPassword = HttpRequest.Params["oldPassword"];
                string _NewPassword = HttpRequest.Params["newPassword"];
                string _ConfirmNewPassword = HttpRequest.Params["confirmNewPassword"];

                //  Validate Current Password.
                if (!_UserLogin.Password.Equals(EDClass.Encrypt(_OldPassword)))
                {
                    //--Create response as Error
                    var objResponseError = new { status = -1, message = Resources.BusinessPanel.InCorrectOldPassword, data = "" };
                    //sending response as error
                    return Request.CreateResponse(HttpStatusCode.OK, objResponseError);
                }
                else if (!PasswordValidator.IsValidPassword(_NewPassword))
                {
                    var objResponseError = new { status = -1, message = PasswordValidator.PasswordValidationMessage, data = "" };
                    return Request.CreateResponse(HttpStatusCode.OK, objResponseError);
                }

                // Confirm Password
                if (!_NewPassword.Equals(_ConfirmNewPassword))
                {
                    //--Create response as Error
                    var objResponseError = new { status = -1, message = Resources.BusinessPanel.ReEntryNewPasswordRequired, data = "" };
                    //sending response as error
                    return Request.CreateResponse(HttpStatusCode.OK, objResponseError);
                }

                SqlParameter[] query = new SqlParameter[] {
                    new SqlParameter("userLoginId", _UserLogin.Id),
                    new SqlParameter("password", EDClass.Encrypt(_NewPassword)),
                    new SqlParameter("mode", "1")
                    };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_UpdatePassword @userLoginId,@password,@mode", query).FirstOrDefault();

                //--Create response
                var objResponse = new
                {
                    status = resp.ret,
                    message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey),
                };

                //sending response as OK
                return Request.CreateResponse(HttpStatusCode.OK, objResponse);
            }
            catch (Exception ex)
            {
                //--Create response as Error
                var objResponse = new { status = -100, message = Resources.ErrorMessage.InternalServerErrorMessage, data = "" };
                //sending response as error
                return Request.CreateResponse(HttpStatusCode.InternalServerError, objResponse);
            }
        }
    }
}