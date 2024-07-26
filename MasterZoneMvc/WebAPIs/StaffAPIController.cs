using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class StaffAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private EmailSender emailSender;
        private StoredProcedureRepository storedProcedureRepository;


        public StaffAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            emailSender = new EmailSender();
            storedProcedureRepository = new StoredProcedureRepository(db);

        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAll")]
        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<Staff_VM>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetById/{id}")]
        public HttpResponseMessage GetById(int id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<StaffEdit_VM>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode", queryParams).FirstOrDefault();
                //return resp;
                resp.Password = EDClass.Decrypt(resp.Password);

                SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                             new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", resp.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                resp.Permissions = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManageUserPermissions @id,@userLoginId,@mode", queryParamsPermissions).ToList();


                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
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

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/AddUpdate")]
        public HttpResponseMessage AddUpdateStaff()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestStaffViewModel requestStaff_VM = new RequestStaffViewModel();
                requestStaff_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestStaff_VM.BusinessOwnerLoginId = 0;
                requestStaff_VM.Email = HttpRequest.Params["Email"].Trim();
                requestStaff_VM.Password = HttpRequest.Params["Password"].Trim();
                requestStaff_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestStaff_VM.FirstName = HttpRequest.Params["FirstName"].Trim();
                requestStaff_VM.LastName = HttpRequest.Params["LastName"].Trim();
                requestStaff_VM.StaffCategoryId = Convert.ToInt64(HttpRequest.Params["StaffCategoryId"]);
                requestStaff_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestStaff_VM.PermissionIds = HttpRequest.Params["PermissionIds"];
                requestStaff_VM.IsMasterId = Convert.ToInt32(HttpRequest.Params["IsMasterId"]);
                requestStaff_VM.MasterId = HttpRequest.Params["MasterIdValue"];
                requestStaff_VM.BasicSalary = 0;
                requestStaff_VM.HouseRentAllowance = 0;
                requestStaff_VM.TravellingAllowance = 0;
                requestStaff_VM.DearnessAllowance = 0;
                requestStaff_VM.Remarks = String.Empty;
                requestStaff_VM.IsProfessional = Convert.ToInt32(HttpRequest.Params["IsProfessional"]);
                requestStaff_VM.Designation = HttpRequest.Params["Designation"].Trim();

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _StaffImageFile = files["StaffProfileImage"];
                requestStaff_VM.ProfileImage = _StaffImageFile; // for validation
                string _StaffImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousStaffImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestStaff_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else if (!PasswordValidator.IsValidPassword(requestStaff_VM.Password))
                {
                    apiResponse.status = -1;
                    apiResponse.message = PasswordValidator.PasswordValidationMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (requestStaff_VM.IsMasterId == 0)
                {
                    requestStaff_VM.MasterId = "";
                }


                if (files.Count > 0)
                {
                    if (_StaffImageFile != null)
                    {
                        _StaffImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_StaffImageFile);
                    }
                }

                if (requestStaff_VM.Mode == 2)
                {
                    //BusinessCategory_VM businessCategory = businessCategoryService.GetBusinessCategoryById(requestStaff_VM.Id);
                    SqlParameter[] queryParamsGetStaff = new SqlParameter[] {
                            new SqlParameter("id", requestStaff_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("mode", "2")
                            };

                    var respGetStaff = db.Database.SqlQuery<StaffEdit_VM>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode", queryParamsGetStaff).FirstOrDefault();

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _StaffImageFileNameGenerated = respGetStaff.ProfileImage;
                    }
                    else
                    {
                        _PreviousStaffImageFileName = respGetStaff.ProfileImage;
                    }
                }

                string _UniqueUserIdForUser = "";
                string _UniqueUserIdForStaff = "";

                // if Insert-Staff
                if (requestStaff_VM.Mode == 1)
                {
                    UserLoginService userLoginService = new UserLoginService(db);
                    if (string.IsNullOrEmpty(requestStaff_VM.MasterId))
                    {
                        _UniqueUserIdForUser = userLoginService.GenerateRandomUniqueUserId(StaticResources.UserId_Prefix_StudentUser);
                    }

                    _UniqueUserIdForStaff = userLoginService.GenerateRandomUniqueUserId(StaticResources.UserId_Prefix_StaffUser);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestStaff_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", requestStaff_VM.StaffCategoryId),
                            new SqlParameter("email", requestStaff_VM.Email),
                            new SqlParameter("password", EDClass.Encrypt(requestStaff_VM.Password)),
                            new SqlParameter("firstName", requestStaff_VM.FirstName),
                            new SqlParameter("lastName", requestStaff_VM.LastName),
                            new SqlParameter("profileImage", _StaffImageFileNameGenerated),
                            new SqlParameter("status", requestStaff_VM.Status),
                            new SqlParameter("isProfessional", requestStaff_VM.IsProfessional),
                            new SqlParameter("designation", requestStaff_VM.Designation),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("masterId",requestStaff_VM.MasterId),
                            new SqlParameter("mode", requestStaff_VM.Mode),
                            new SqlParameter("basicSalary", requestStaff_VM.BasicSalary),
                            new SqlParameter("HRA", requestStaff_VM.HouseRentAllowance),
                            new SqlParameter("TA", requestStaff_VM.TravellingAllowance),
                            new SqlParameter("DA", requestStaff_VM.DearnessAllowance),
                            new SqlParameter("remarks", requestStaff_VM.Remarks),
                            new SqlParameter("uniqueUserIdForUser", _UniqueUserIdForUser),
                            new SqlParameter("uniqueUserIdForStaff", _UniqueUserIdForStaff),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel_UserAddUpdate>("exec sp_InsertUpdateStaff @id,@businessOwnerLoginId,@staffCategoryId,@email,@password,@firstName,@lastName,@profileImage,@status,@isProfessional,@designation,@submittedByLoginId,@masterId,@mode,@basicSalary,@HRA,@TA,@DA,@remarks,@uniqueUserIdForUser,@uniqueUserIdForStaff", queryParams).FirstOrDefault();
                //return resp;

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (resp.ret == 1)
                {
                    // Send Email with Credentials when created
                    if (requestStaff_VM.Mode == 1)
                    {
                        #region Send Email --------------------

                        string toEmail = requestStaff_VM.Email;
                        string receiverName = requestStaff_VM.FirstName + " " + requestStaff_VM.LastName;
                        string emailMessage = "Your staff account has been created. Below are the credentials<br/> <b>Email:</b> " + requestStaff_VM.Email + " <br/> <b>Master ID:</b> " + resp.MasterId + " <br/> <b>Password:</b> " + requestStaff_VM.Password;
                        emailSender.Send(receiverName, "Account Created", toEmail, emailMessage, "");

                        #endregion ----------------
                    }

                    // Update Category Image.
                    #region Insert-Update Category Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousStaffImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousStaffImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_StaffProfileImage), _StaffImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_StaffImageFile, FileWithPath);
                    }
                    #endregion

                    // Update User-Permissions
                    #region Insert-Update User Permissions
                    SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                            new SqlParameter("userLoginId", resp.Id),
                            new SqlParameter("permissionIds", requestStaff_VM.PermissionIds),
                            new SqlParameter("mode", "1")
                            };

                    var respPermissions = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateUserPermissions @userLoginId,@permissionIds,@mode", queryParamsPermissions).FirstOrDefault();

                    if (respPermissions.ret <= 0)
                    {
                        apiResponse.status = respPermissions.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        apiResponse.data = new { };
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                    #endregion
                }

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/Delete/{id}")]
        public HttpResponseMessage DeleteStaff(int id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Id";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("email", ""),
                            new SqlParameter("password", ""),
                            new SqlParameter("firstName", ""),
                            new SqlParameter("lastName", ""),
                            new SqlParameter("profileImage", ""),
                            new SqlParameter("status", ""),
                            new SqlParameter("isProfessional", "0"),
                            new SqlParameter("designation", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("masterId", ""),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("basicSalary", "0"),
                            new SqlParameter("HRA", "0"),
                            new SqlParameter("TA", "0"),
                            new SqlParameter("DA", "0"),
                            new SqlParameter("remarks", "0"),
                            new SqlParameter("uniqueUserIdForUser", "0"),
                            new SqlParameter("uniqueUserIdForStaff", "0")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStaff @id,@businessOwnerLoginId,@staffCategoryId,@email,@password,@firstName,@lastName,@profileImage,@status,@isProfessional,@designation,@submittedByLoginId,@masterId,@mode,@basicSalary,@HRA,@TA,@DA,@remarks,@uniqueUserIdForUser,@uniqueUserIdForStaff", queryParams).FirstOrDefault();
                //return resp;
                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = resp;

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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeStaffStatus(int id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = resp;

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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/UpdateSalary")]
        public HttpResponseMessage UpdateStaffSalary()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestStaffViewModel requestStaff_VM = new RequestStaffViewModel();
                requestStaff_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestStaff_VM.BasicSalary = Convert.ToDecimal(HttpRequest.Params["Salary"]);
                requestStaff_VM.HouseRentAllowance = (String.IsNullOrEmpty(HttpRequest.Params["HRA"])) ? 0 : Convert.ToDecimal(HttpRequest.Params["HRA"]);
                requestStaff_VM.TravellingAllowance = (String.IsNullOrEmpty(HttpRequest.Params["TA"])) ? 0 : Convert.ToDecimal(HttpRequest.Params["TA"]);
                requestStaff_VM.DearnessAllowance = (String.IsNullOrEmpty(HttpRequest.Params["DA"])) ? 0 : Convert.ToDecimal(HttpRequest.Params["DA"]);
                requestStaff_VM.Remarks = HttpRequest.Params["Remarks"];
                requestStaff_VM.Mode = 4;

                // Validate infromation passed
                #region validate and return if not valid
                Error_VM error_VM = new Error_VM();
                error_VM.Valid = true;
                if (requestStaff_VM.Id <= 0)
                {
                    error_VM.Valid = false;
                    error_VM.Message = Resources.ErrorMessage.InvalidIdErrorMessage;
                }
                else if (requestStaff_VM.BasicSalary <= 0)
                {
                    error_VM.Valid = false;
                    error_VM.Message = Resources.BusinessPanel.MonthlySalary;
                }

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                #endregion


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestStaff_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("email", ""),
                            new SqlParameter("password", ""),
                            new SqlParameter("firstName", ""),
                            new SqlParameter("lastName", ""),
                            new SqlParameter("profileImage", ""),
                            new SqlParameter("status", "0"),
                            new SqlParameter("isProfessional", "0"),
                            new SqlParameter("designation", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("masterId", ""),
                            new SqlParameter("mode", requestStaff_VM.Mode),
                            new SqlParameter("basicSalary", requestStaff_VM.BasicSalary),
                            new SqlParameter("HRA", requestStaff_VM.HouseRentAllowance),
                            new SqlParameter("TA", requestStaff_VM.TravellingAllowance),
                            new SqlParameter("DA", requestStaff_VM.DearnessAllowance),
                            new SqlParameter("remarks", requestStaff_VM.Remarks),
                            new SqlParameter("uniqueUserIdForUser", ""),
                            new SqlParameter("uniqueUserIdForStaff", "")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStaff @id,@businessOwnerLoginId,@staffCategoryId,@email,@password,@firstName,@lastName,@profileImage,@status,@isProfessional,@designation,@submittedByLoginId,@masterId,@mode,@basicSalary,@HRA,@TA,@DA,@remarks,@uniqueUserIdForUser,@uniqueUserIdForStaff", queryParams).FirstOrDefault();
                //return resp;

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/Categories/GetAllActive")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveStaffCategories()
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
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<StaffCategoryViewModel>("exec sp_ManageStaffCategories @id,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetStaffDetailsById/")]
        public HttpResponseMessage GetStaffDetailsById(int id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("mode", "6")
                            };

                var resp = db.Database.SqlQuery<StaffEdit_VM>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode", queryParams).FirstOrDefault();
                //return resp;
                resp.Password = EDClass.Decrypt(resp.Password);

                SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", resp.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                resp.Permissions = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManageUserPermissions @id, @userLoginId,@mode", queryParamsPermissions).ToList();

                var listPermissionHierarchy_VM = resp.Permissions.Where(p => p.ParentPermissionId == 0).ToList();

                foreach (var permission in listPermissionHierarchy_VM)
                {
                    permission.SubPermissions = resp.Permissions.Where(p => p.ParentPermissionId == permission.Id).ToList();
                    foreach (var subPermission in permission.SubPermissions)
                    {
                        subPermission.SubPermissions = resp.Permissions.Where(p => p.ParentPermissionId == subPermission.Id).ToList();
                    }
                }

                resp.PermissionsHierarchy = listPermissionHierarchy_VM;


                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
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

        /// <summary>
        /// Get All Staff Attendance Data Current Date
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAll/GetAllSatffAttendanceById")]
        [HttpGet]
        public HttpResponseMessage GetAllSatffAttendanceById(long id, string currentYear = "", string currentMonth = "")
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

                List<StaffAttendance_VM> staffAttendance_VMs = new List<StaffAttendance_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                              new SqlParameter("att_date", ""),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                           // new SqlParameter("attendanceMonth", currentMonth),
                           // new SqlParameter("attendanceYear", currentYear),
                            new SqlParameter("mode", "3")
                            };

                staffAttendance_VMs = db.Database.SqlQuery<StaffAttendance_VM>("exec sp_ManageStaffAttendance @id,@att_date,@businessOwnerLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = staffAttendance_VMs;
                //apiResponse.data = ;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }



        #region Attendance Api's ---------------------------------------------------------------------------------------
        /// <summary>
        /// Get All Staff Attendance Data Current Date
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAll/StaffAttendance")]
        [HttpGet]
        public HttpResponseMessage GetAllSatffAttendanceList()
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

                List<StaffAttendance_VM> staffAttendance_VMs = new List<StaffAttendance_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("att_date", ""),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "1")
                            };

                staffAttendance_VMs = db.Database.SqlQuery<StaffAttendance_VM>("exec sp_ManageStaffAttendance @id,@att_date,@businessOwnerLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = staffAttendance_VMs;
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
        ///  Insert  Attendance data
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/AddUpdate/StaffAttendance")]
        [HttpPost]
        public HttpResponseMessage AddUpdateSatffAttendance(List<RequestStaffAttendance_VM> requestStaffAttendance_VMs)
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

                if (requestStaffAttendance_VMs.Count() <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.NoStaffAttendanceErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                foreach (var item in requestStaffAttendance_VMs)
                {
                    if (item.attendanceStatus == 1)
                    {
                        if (String.IsNullOrEmpty(item.staffAttendanceInTime24HF))
                        {
                            apiResponse.status = -1;
                            apiResponse.message = Resources.BusinessPanel.ErrorMessageAttendanceTimeIn;

                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                        else if (item.staffAttendanceOutTime24HF != "")
                        {
                            if (TimeSpan.TryParse(item.staffAttendanceInTime24HF, out TimeSpan inTime) && TimeSpan.TryParse(item.staffAttendanceOutTime24HF, out TimeSpan outTime))
                            {
                                if (inTime >= outTime)
                                {
                                    apiResponse.status = -1;
                                    apiResponse.message = Resources.BusinessPanel.AttendanceOuTimeErrorMessage;

                                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                }
                            }
                        }
                    }

                }

                // delete Current Date Data 
                SqlParameter[] query_Params = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                              new SqlParameter("att_date", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "2")
                            };

                var deleteResp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageStaffAttendance @id,@att_date,@businessOwnerLoginId,@mode", query_Params).ToList();

                SPResponseViewModel staffAttendanceResp = new SPResponseViewModel();
                foreach (var data in requestStaffAttendance_VMs)
                {
                    var Date = DateTime.Now.ToString("dd-MMMM-yyyy hh:mm tt");
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffId",data.staffId),
                            new SqlParameter("attendanceStatus",data.attendanceStatus),
                            new SqlParameter("attendanceDate",Date),
                            new SqlParameter("attendanceMonth",DateTime.Now.Month),
                            new SqlParameter("attendanceYear",DateTime.Now.Year),
                            new SqlParameter("leaveReason",data.leaveReason),
                            new SqlParameter("isApproved",data.isApproved),
                            new SqlParameter("staffAttendanceInTime_24HF",data.staffAttendanceInTime24HF),
                            new SqlParameter("staffAttendanceOutTime_24HF",data.staffAttendanceOutTime24HF),
                            new SqlParameter("submittedByLoginId",_LoginId),
                            new SqlParameter("mode", "1")
                            };

                    staffAttendanceResp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStaffAttendance @id,@businessOwnerLoginId,@staffId,@attendanceStatus,@attendanceDate,@attendanceMonth,@attendanceYear,@leaveReason,@isApproved,@staffAttendanceInTime_24HF,@staffAttendanceOutTime_24HF,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                }

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.AttendanceAdded_SuccessMessage;
                apiResponse.data = staffAttendanceResp;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }
        #endregion --------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get Basic Details for staff
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetBasicProfileDetail")]
        public HttpResponseMessage GetBasicProfileDetailBySatff()
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


                // List<BasicProfileDetail_VM> lst = new List<BasicProfileDetail_VM>();
                BasicProfileDetail_VM basicProfileDetail_VM = new BasicProfileDetail_VM();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _LoginId),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", "0"),
                            new SqlParameter("mode", "4")
                            };

                basicProfileDetail_VM = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode", queryParams).FirstOrDefault();


                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = basicProfileDetail_VM;
                //}

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #region Staff Profile Setting ---------------------------------------------------------------------------
        /// <summary>
        /// AddUpdate Staff profile-image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [Route("api/Staff/Profile/AddUpdateProfileImage")]
        public HttpResponseMessage AddUpdateStaffProfileImage()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestStaffProfile_VM requestStaffProfile_VM = new RequestStaffProfile_VM();
                requestStaffProfile_VM.Mode = 2;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _staffImageFile = files["ProfileImage"]; // change name
                requestStaffProfile_VM.ProfileImage = _staffImageFile;


                string _StaffImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestStaffProfile_VM.VaildInformationProfileImage();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                StaffProfileViewModel respProfileData = new StaffProfileViewModel();
                // Get Staff Profile Data for Image Names
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", _LoginID_Exact),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<StaffProfileViewModel>("exec sp_ManageStaffProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();


                if (_staffImageFile != null && files.Count > 0)
                {
                    _StaffImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_staffImageFile);

                    _PreviousProfileImageFileName = respProfileData.ProfileImage;
                }
                else
                {
                    _StaffImageFileNameGenerated = respProfileData.ProfileImage;
                }

                var resp = storedProcedureRepository.SP_InsertUpdateStaffProfile_Get<SPResponseViewModel>(new SP_InsertUpdateStaffProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    ProfileImage = _StaffImageFileNameGenerated,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    Mode = requestStaffProfile_VM.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                // Update Staff Image.
                #region Insert-Update [Staff Profile Image] on Server
                if (files.Count > 0)
                {
                    // if Staff Profile Image passed then Add-Update Profile Image
                    if (_staffImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_StaffProfileImage), _StaffImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_staffImageFile, fileWithPathImage);
                    }


                }
                #endregion

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
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
        /// Add update Staff Profile Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [Route("api/Staff/Profile/AddUpdate")]
        public HttpResponseMessage AddUpdateStaffProfile()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestStaffProfile_VM requestStaffProfile_VM = new RequestStaffProfile_VM();
                //updateProfieSetting.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestStaffProfile_VM.Email = HttpRequest.Params["Email"].Trim();
                requestStaffProfile_VM.FirstName = HttpRequest.Params["FirstName"].Trim();
                requestStaffProfile_VM.LastName = HttpRequest.Params["LastName"].Trim();
                requestStaffProfile_VM.Address = HttpRequest.Params["Address"];
                requestStaffProfile_VM.City = HttpRequest.Params["City"];
                requestStaffProfile_VM.State = HttpRequest.Params["State"];
                requestStaffProfile_VM.Country = HttpRequest.Params["Country"];
                requestStaffProfile_VM.PhoneNumber = HttpRequest.Params["PhoneNumber"];
                requestStaffProfile_VM.LandMark = HttpRequest.Params["LandMark"];
                requestStaffProfile_VM.PinCode = HttpRequest.Params["PinCode"];
                requestStaffProfile_VM.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLatitude"]) : 0;
                requestStaffProfile_VM.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLongitude"]) : 0;
                requestStaffProfile_VM.Mode = 1;
                requestStaffProfile_VM.FacebookProfileLink = HttpRequest.Params["FacebookProfileLink"];
                requestStaffProfile_VM.InstagramProfileLink = HttpRequest.Params["InstagramProfileLink"];
                requestStaffProfile_VM.LinkedInProfileLink = HttpRequest.Params["LinkedInProfileLink"];
                requestStaffProfile_VM.TwitterProfileLink = HttpRequest.Params["TwitterProfileLink"];
                requestStaffProfile_VM.DOB = HttpRequest.Params["DOB"];
                requestStaffProfile_VM.About = HttpRequest.Params["About"];
                // Validate infromation passed
                Error_VM error_VM = requestStaffProfile_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateStaffProfile_Get<SPResponseViewModel>(new SP_InsertUpdateStaffProfile_Params_VM
                {
                    Id = _LoginID_Exact,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    FirstName = requestStaffProfile_VM.FirstName,
                    LastName = requestStaffProfile_VM.LastName,
                    Email = requestStaffProfile_VM.Email,
                    Address = requestStaffProfile_VM.Address,
                    Country = requestStaffProfile_VM.Country,
                    State = requestStaffProfile_VM.State,
                    City = requestStaffProfile_VM.City,
                    PinCode = requestStaffProfile_VM.PinCode,
                    LandMark = requestStaffProfile_VM.LandMark,
                    Latitude = requestStaffProfile_VM.Latitude,
                    Longitude = requestStaffProfile_VM.Longitude,
                    PhoneNumber = requestStaffProfile_VM.PhoneNumber,
                    Mode = requestStaffProfile_VM.Mode,
                    FacebookProfileLink = requestStaffProfile_VM.FacebookProfileLink,
                    LinkedInProfileLink = requestStaffProfile_VM.LinkedInProfileLink,
                    InstagramProfileLink = requestStaffProfile_VM.InstagramProfileLink,
                    TwitterProfileLink = requestStaffProfile_VM.TwitterProfileLink,
                    About = requestStaffProfile_VM.About,
                    DOB = requestStaffProfile_VM.DOB
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
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
        /// Get Staff profile detials
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Staff")]
        [Route("api/Staff/Profile/Get")]
        public HttpResponseMessage GetStaffProfileData()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                StaffProfileViewModel respProfileData = new StaffProfileViewModel();
                // Get Staff Profile Data 
                SqlParameter[] queryParamsGetProfile = new SqlParameter[] {
                            new SqlParameter("id", _LoginID_Exact),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "1")
                            };

                respProfileData = db.Database.SqlQuery<StaffProfileViewModel>("exec sp_ManageStaffProfile @id,@businessOwnerLoginId,@mode", queryParamsGetProfile).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = respProfileData;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion ---------------------------------------------------------------------------------------------------


        /// <summary>
        /// To Get SuperAdmin Package Permission Detail List 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAllPermissionsDetails")]
        public HttpResponseMessage GetAllPermissionsDetails(long itemId)
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


                SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                         new SqlParameter("id", itemId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                var Permissions = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManageUserPermissions  @id,@userLoginId,@mode", queryParamsPermissions).ToList();

                var listPermissionHierarchy_VM = Permissions.Where(p => p.ParentPermissionId == 0).ToList();

                foreach (var permission in listPermissionHierarchy_VM)
                {
                    permission.SubPermissions = Permissions.Where(p => p.ParentPermissionId == permission.Id).ToList();
                    foreach (var subPermission in permission.SubPermissions)
                    {
                        subPermission.SubPermissions = Permissions.Where(p => p.ParentPermissionId == subPermission.Id).ToList();
                    }
                }

                var PermissionsHierarchy = listPermissionHierarchy_VM;


                if (PermissionsHierarchy == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = PermissionsHierarchy;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAll/GetAllStaffAttendanceListByDate")]
        [HttpGet]
        public HttpResponseMessage GetAllStaffAttendanceListByDate(string searchDate)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                //  string formattedDate = ("'" + searchDate.Replace("\"", "") + "'").Trim('"');

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;



                List<StaffAttendance_VM> staffAttendance_VMs = new List<StaffAttendance_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                     new SqlParameter("id", "0"),
                     new SqlParameter("att_date", searchDate),
                     new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                     new SqlParameter("mode", "4")
                     };

                staffAttendance_VMs = db.Database.SqlQuery<StaffAttendance_VM>("exec sp_ManageStaffAttendance @id,@att_date,@businessOwnerLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = staffAttendance_VMs;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }


        //for mobile (not usable)

        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAllStaffDetails_ForMobile")]
        [HttpGet]
        public HttpResponseMessage GetAllStaffDetails_ForMobile(int? StaffCatId = 0, string SearchText="")
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
                SearchText = string.IsNullOrEmpty(SearchText) ? "" : SearchText;
                StaffCatId = StaffCatId==null ? 0 : StaffCatId;
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("staffCategoryId", StaffCatId),
                            new SqlParameter("mode", "7"),
                            new SqlParameter("SearchText", SearchText),
                            };

                var resp = db.Database.SqlQuery<Staff_VM>("exec sp_ManageStaff @id,@businessOwnerLoginId,@staffCategoryId,@mode,@SearchText", queryParams).ToList();

                var groupedData = resp.GroupBy(s => new { s.StaffCategoryName,s.StaffCategoryId })
                              .Select(g => new
                              {
                                  StaffCategoryId= g.Key.StaffCategoryId,
                                  StaffCategoryName = g.Key.StaffCategoryName,
                                  StaffMembers = g.ToList()
                              })
                              .ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = groupedData;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }


        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/InOut/StaffAttendance")]
        [HttpPost]
        public HttpResponseMessage InOutSatffTiming(List<RequestStaffAttendance_VM> requestStaffAttendance_VMs)
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

                if (requestStaffAttendance_VMs.Count() <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.NoStaffAttendanceErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                foreach (var item in requestStaffAttendance_VMs)
                {
                    if (item.attendanceStatus == 1)
                    {
                        if (String.IsNullOrEmpty(item.staffAttendanceInTime24HF))
                        {
                            apiResponse.status = -1;
                            apiResponse.message = Resources.BusinessPanel.ErrorMessageAttendanceTimeIn;

                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                        else if (item.staffAttendanceOutTime24HF != "")
                        {
                            if (TimeSpan.TryParse(item.staffAttendanceInTime24HF, out TimeSpan inTime) && TimeSpan.TryParse(item.staffAttendanceOutTime24HF, out TimeSpan outTime))
                            {
                                if (inTime >= outTime)
                                {
                                    apiResponse.status = -1;
                                    apiResponse.message = Resources.BusinessPanel.AttendanceOuTimeErrorMessage;

                                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                }
                            }
                        }
                    }

                }
                SPResponseViewModel staffAttendanceResp = new SPResponseViewModel();
                foreach (var data in requestStaffAttendance_VMs)
                {
                    var mode = 2;
                    if (data.staffAttendanceOutTime24HF == "")
                    {
                        mode = 1;
                    }

                    var Date = DateTime.Now.ToString("dd-MMMM-yyyy hh:mm tt");
                    SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", "0"),
                    new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                    new SqlParameter("staffId",data.staffId),
                    new SqlParameter("attendanceStatus",data.attendanceStatus),
                    new SqlParameter("attendanceDate",Date),
                    new SqlParameter("attendanceMonth",DateTime.Now.Month),
                    new SqlParameter("attendanceYear",DateTime.Now.Year),
                    new SqlParameter("leaveReason",data.leaveReason),
                    new SqlParameter("isApproved",data.isApproved),
                    new SqlParameter("staffAttendanceInTime_24HF",data.staffAttendanceInTime24HF),
                    new SqlParameter("staffAttendanceOutTime_24HF",data.staffAttendanceOutTime24HF),
                    new SqlParameter("submittedByLoginId",_LoginId),
                    new SqlParameter("mode", mode),

                    };

                    staffAttendanceResp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateStaffAttendance @id,@businessOwnerLoginId,@staffId,@attendanceStatus,@attendanceDate,@attendanceMonth,@attendanceYear,@leaveReason,@isApproved,@staffAttendanceInTime_24HF,@staffAttendanceOutTime_24HF,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                }

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.AttendanceAdded_SuccessMessage;
                apiResponse.data = staffAttendanceResp;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.data = ex;
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Staff/GetAllDetails/StaffAttendance")]
        [HttpGet]
        public HttpResponseMessage GetAllDetailsSatffAttendanceList()
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

                List<StaffAttendance_VM> staffAttendance_VMs = new List<StaffAttendance_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", "0"),
                    new SqlParameter("att_date", ""),
                    new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                    new SqlParameter("mode", "5")
                    };

                staffAttendance_VMs = db.Database.SqlQuery<StaffAttendance_VM>("exec sp_ManageStaffAttendance @id,@att_date,@businessOwnerLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = staffAttendance_VMs;
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