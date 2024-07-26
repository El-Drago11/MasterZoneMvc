using MasterZoneMvc.Common;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class FeatureAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FeatureService featureService;

        public FeatureAPIController()
        {
            db = new MasterZoneDbContext();
            featureService = new FeatureService(db);
        }

        /// <summary>
        /// Validate Logged-in user. 
        /// </summary>
        /// <returns></returns>
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Active Business Panel Features List
        /// </summary>
        /// <returns>List of Features</returns>
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [Route("api/Features/GetAllActive/BusinessPanel")]
        [HttpGet]
        public HttpResponseMessage GetAllBusinessPanelFeatures()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                var activeFeatures = featureService.GetAllPanelActiveFeatureList(StaticResources.PanelType_Business);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = activeFeatures;

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