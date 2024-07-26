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

namespace MasterZoneMvc.WebAPIs
{
    public class FieldTypeCatalogAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FieldTypeCatalogService fieldTypeCatalogService;

        public FieldTypeCatalogAPIController()
        {
            db = new MasterZoneDbContext();
            fieldTypeCatalogService = new FieldTypeCatalogService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Active Field-Types For Super-Admin Panel
        /// </summary>
        /// <returns>List string of Tags</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin,BusinessAdmin,Staff,Student")]
        [Route("api/FieldTypeCatalog/GetAllActiveByKeyName")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveFieldTypeCatalogs(string keyName)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if(string.IsNullOrEmpty(keyName))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                    apiResponse.data = null;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var fieldTypes = fieldTypeCatalogService.GetAllActiveFieldTypeCatologByKey(keyName);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = fieldTypes;

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