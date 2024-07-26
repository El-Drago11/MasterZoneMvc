using MasterZoneMvc.Common;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
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
    public class PermissionAPIController : ApiController
    {

        private MasterZoneDbContext db;
        public PermissionAPIController()
        {
            db = new MasterZoneDbContext();
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

        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [Route("api/Permisssions/GetAll/BusinessPanel")]
        [HttpGet]
        public HttpResponseMessage GetAllBusinessPanelPermissions()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("panelTypeId", StaticResources.PanelType_Business),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManagePermissions @id,@panelTypeId,@mode", queryParams).ToList();

                List<PermissionHierarchy_VM> listPermissionHierarchy_VM = new List<PermissionHierarchy_VM>();
                listPermissionHierarchy_VM = resp.Where(p => p.ParentPermissionId == 0).ToList();

                foreach (var permission in listPermissionHierarchy_VM)
                {
                    permission.SubPermissions = resp.Where(p => p.ParentPermissionId == permission.Id).ToList();
                    foreach (var subPermission in permission.SubPermissions)
                    {
                        subPermission.SubPermissions = resp.Where(p => p.ParentPermissionId == subPermission.Id).ToList();
                    }
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = listPermissionHierarchy_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        [Authorize(Roles = "SuperAdmin")]
        [Route("api/Permisssions/GetAll/SuperAdminPanel")]
        [HttpGet]
        public HttpResponseMessage GetAllSuperAdminPanelPermissions()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("panelTypeId", StaticResources.PanelType_SuperAdmin),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManagePermissions @id,@panelTypeId,@mode", queryParams).ToList();

                List<PermissionHierarchy_VM> listPermissionHierarchy_VM = new List<PermissionHierarchy_VM>();
                listPermissionHierarchy_VM = resp.Where(p => p.ParentPermissionId == 0).ToList();

                foreach (var permission in listPermissionHierarchy_VM)
                {
                    permission.SubPermissions = resp.Where(p => p.ParentPermissionId == permission.Id).ToList();
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = listPermissionHierarchy_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }



        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,SubAdmin")]
        [Route("api/Permisssions/GetAll/GetAllUserPanelPermissions")]
        [HttpGet]
        public HttpResponseMessage GetAllUserPanelPermissions()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("panelTypeId", StaticResources.PanelType_User),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManagePermissions @id,@panelTypeId,@mode", queryParams).ToList();

                List<PermissionHierarchy_VM> listPermissionHierarchy_VM = new List<PermissionHierarchy_VM>();
                listPermissionHierarchy_VM = resp.Where(p => p.ParentPermissionId == 0).ToList();

                foreach (var permission in listPermissionHierarchy_VM)
                {
                    permission.SubPermissions = resp.Where(p => p.ParentPermissionId == permission.Id).ToList();
                    foreach (var subPermission in permission.SubPermissions)
                    {
                        subPermission.SubPermissions = resp.Where(p => p.ParentPermissionId == subPermission.Id).ToList();
                    }
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = listPermissionHierarchy_VM;

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