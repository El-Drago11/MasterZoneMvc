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
using MasterZoneMvc.Models.Enum;

namespace MasterZoneMvc.WebAPIs
{
    public class EncryptionAPIController : ApiController
    {
        private MasterZoneDbContext db;
        public EncryptionAPIController()
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

        /// <summary>
        /// Encrypt value and return
        /// </summary>
        /// <param name="value">value which needs to be encrypted like ID</param>
        /// <returns>Encrypted value</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin,BusinessAdmin,Staff,Student")]
        [Route("api/Encryption/Encrypt")]
        public HttpResponseMessage GetEncryptedValue(string value)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
               
                long _LoginID_Exact = validateResponse.UserLoginId;

                var encryptedValue = EDClass.Encrypt(value);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = encryptedValue;

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