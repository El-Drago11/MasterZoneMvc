using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.Common.ValidationHelpers
{
    public class WebApiValidationHelper
    {
        private MasterZoneDbContext db;
        public WebApiValidationHelper(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Validate Logged in User is valid or not
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public ValidateUserLogin_VM ValidateLoggedInLogin(ClaimsIdentity identity)
        {
            ValidateUserLogin_VM validateUserLogin_VM = new ValidateUserLogin_VM();
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            //--Check if user is authorized user or not
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string _UserRole = claims.Where(p => p.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
                string _LoginID = claims.Where(p => p.Type == "loginid").FirstOrDefault()?.Value;
                string _BusinessAdminLoginID = claims.Where(p => p.Type == "businessAdminLoginId").FirstOrDefault()?.Value;
                long _LoginID_Exact = 0;
                UserLogin _UserLogin = null;

                if (_LoginID != "" && _LoginID != null)
                {
                    _LoginID_Exact = Convert.ToInt64(_LoginID);

                    //--Get User-Login Detail
                    _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault(); //_userLoginService.GetById(_LoginID_Exact);
                }

                if (_UserLogin != null)
                {
                    apiResponse.status = 1;
                    apiResponse.message = Resources.ErrorMessage.Authenticated;
                    apiResponse.data = new { UserLoginId = _UserLogin.Id };

                    validateUserLogin_VM.UserLoginId = _UserLogin.Id;
                    validateUserLogin_VM.UserRoleName = _UserRole;

                    if (_UserRole == "BusinessAdmin")
                        validateUserLogin_VM.BusinessAdminLoginId = _LoginID_Exact;
                    else if (_UserRole == "Staff")
                        validateUserLogin_VM.BusinessAdminLoginId = Convert.ToInt64(_BusinessAdminLoginID);
                    else if (_UserRole == "SuperAdmin")
                        validateUserLogin_VM.SuperAdminLoginId = Convert.ToInt64(_UserLogin.Id);
                    else if (_UserRole == "SubAdmin")
                        validateUserLogin_VM.SubAdminLoginId = Convert.ToInt64(_UserLogin.Id);
                }
                else
                {
                    apiResponse.status = -101;
                    apiResponse.message = Resources.ErrorMessage.AuthorizationErrorMessage;
                }
            }
            else
            {
                apiResponse.status = -101;
                apiResponse.message = Resources.ErrorMessage.AuthorizationErrorMessage;
            }
            validateUserLogin_VM.ApiResponse_VM = apiResponse;
            return validateUserLogin_VM;
        }

        //public ValidateUserLogin_VM ValidateLoggedInUser(User user)
        //{
        //    ValidateUserLogin_VM validateUserLogin_VM = new ValidateUserLogin_VM();

        //    ApiResponse_VM apiResponse = new ApiResponse_VM();

        //    //--Get User Identity
        //    var identity = User.Identity as ClaimsIdentity;

        //    //--Check if user is authorized user or not
        //    if (identity != null)
        //    {
        //        IEnumerable<Claim> claims = identity.Claims;
        //        string _UserRole = claims.Where(p => p.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
        //        string _LoginID = claims.Where(p => p.Type == "loginid").FirstOrDefault()?.Value;
        //        string _BusinessAdminLoginID = claims.Where(p => p.Type == "businessAdminLoginId").FirstOrDefault()?.Value;
        //        long _LoginID_Exact = 0;
        //        UserLogin _UserLogin = null;

        //        if (_LoginID != "" && _LoginID != null)
        //        {
        //            _LoginID_Exact = Convert.ToInt64(_LoginID);

        //            //--Get User-Login Detail
        //            _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault(); //_userLoginService.GetById(_LoginID_Exact);
        //        }

        //        if (_UserLogin != null)
        //        {
        //            apiResponse.status = 1;
        //            apiResponse.message = "Authenticated!";

        //            validateUserLogin_VM.UserLoginId = _UserLogin.Id;

        //            if (_UserRole == "BusinessAdmin")
        //                validateUserLogin_VM.BusinessAdminLoginId = _LoginID_Exact;
        //            else if (_UserRole == "Staff")
        //                validateUserLogin_VM.BusinessAdminLoginId = Convert.ToInt64(_BusinessAdminLoginID);
        //        }
        //        else
        //        {
        //            apiResponse.status = -101;
        //            apiResponse.message = "Authorization has been denied for this request!";
        //        }
        //    }
        //    else
        //    {
        //        apiResponse.status = -101;
        //        apiResponse.message = "Authorization has been denied for this request!";
        //    }
        //    validateUserLogin_VM.ApiResponse_VM = apiResponse;
        //    return validateUserLogin_VM;
        //}
    }
}