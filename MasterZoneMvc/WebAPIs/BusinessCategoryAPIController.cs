using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
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
using static MasterZoneMvc.ViewModels.BusinessCourseCategory_VM;
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessCategoryAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private BusinessCategoryService businessCategoryService;
        private ProfilePageTypeService profilePageTypeService;
        private StoredProcedureRepository storedProcedureRepository;
        private BusinessOwnerService businessOwnerService;


        public BusinessCategoryAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            businessCategoryService = new BusinessCategoryService(db);
            profilePageTypeService = new ProfilePageTypeService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
            businessOwnerService = new BusinessOwnerService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        [HttpGet]
        [Route("api/BusinessCategory/Parent/GetAllActive")]
        public HttpResponseMessage GetAllActiveParentCategories()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                             new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", "5")
                            };

                var resp = db.Database.SqlQuery<BusinessCategory_VM>("exec sp_ManageBusinessCategory @id, @lastRecordId, @recordLimit,@parentBusinessCategoryId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }

        }

        [HttpGet]
        [Route("api/BusinessCategory/Parent/{id}/GetAllActiveSubCategories")]
        public HttpResponseMessage GetAllActiveParentSubCategories(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessCategory_VM Parent_businessCategory_VM = businessCategoryService.GetBusinessCategoryById(id);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", id),
                            new SqlParameter("mode", "7")
                            };

                var resp = db.Database.SqlQuery<BusinessCategory_VM>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).ToList();

                if (Parent_businessCategory_VM == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.CategoryNotFound;
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                BusinessCategoryHierarchy_VM businessCategoryHierarchy_VM = new BusinessCategoryHierarchy_VM()
                {
                    Id = Parent_businessCategory_VM.Id,
                    CategoryImage = Parent_businessCategory_VM.CategoryImage,
                    CategoryImageWithPath = Parent_businessCategory_VM.CategoryImageWithPath,
                    IsActive = Parent_businessCategory_VM.IsActive,
                    Name = Parent_businessCategory_VM.Name,
                    ParentBusinessCategoryId = Parent_businessCategory_VM.ParentBusinessCategoryId
                };
                businessCategoryHierarchy_VM.SubCategories = resp;

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = businessCategoryHierarchy_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }

        }

        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/BusinessCategory/GetAll")]
        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                              new SqlParameter("lastRecordId", "0"),
                              new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<BusinessCategory_VM>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).ToList();

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
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/BusinessCategory/GetById/{id}")]
        public HttpResponseMessage GetById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                           new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<BusinessCategory_VM>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).FirstOrDefault();
                //return resp;

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.SuperAdminPanel.BusinessCategory_ErrorMessage;
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
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/BusinessCategory/AddUpdate")]
        public HttpResponseMessage AddUpdateBusinessCategory()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestBusinessCategoryViewModel requestBusinessCategory_VM = new RequestBusinessCategoryViewModel();
                requestBusinessCategory_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessCategory_VM.ParentBusinessCategoryId = Convert.ToInt64(HttpRequest.Params["ParentBusinessCategoryId"]);
                requestBusinessCategory_VM.IsActive = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                requestBusinessCategory_VM.Name = HttpRequest.Params["Name"].Trim();
                requestBusinessCategory_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestBusinessCategory_VM.CategoryKey = HttpRequest.Params["CategoryKey"].Trim();
                requestBusinessCategory_VM.MenuTag = HttpRequest.Params["MenuTag"].Trim();
                requestBusinessCategory_VM.ProfilePageTypeId = Convert.ToInt32(HttpRequest.Params["ProfilePageTypeId"].Trim());
                
                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _CategoryImageFile = files["CategoryImage"];
                string _CategoryImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousCategoryImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestBusinessCategory_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {

                    // Validate Uploded Image File
                    bool isValidImage = true;
                    string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                    if (!validImageTypes.Contains(_CategoryImageFile.ContentType))
                    {
                        isValidImage = false;
                        apiResponse.message = Resources.SuperAdminPanel.ValidImageFile_ErrorMessage;
                    }
                    else if (_CategoryImageFile.ContentLength > 1024 * 1024) // 1 MB
                    {
                        isValidImage = false;
                        apiResponse.message = String.Format(Resources.SuperAdminPanel.ValidFileSize_ErrorMessage, "1 MB");
                    }

                    if (!isValidImage)
                    {
                        apiResponse.status = -1;
                        apiResponse.message = error_VM.Message;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    if (_CategoryImageFile != null)
                    {
                        _CategoryImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CategoryImageFile);
                    }
                }

                if (requestBusinessCategory_VM.Mode == 2)
                {
                    BusinessCategory_VM businessCategory = businessCategoryService.GetBusinessCategoryById(requestBusinessCategory_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _CategoryImageFileNameGenerated = businessCategory.CategoryImage;
                    }
                    else
                    {
                        _PreviousCategoryImageFileName = businessCategory.CategoryImage;
                    }
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestBusinessCategory_VM.Id),
                            new SqlParameter("parentBusinessCategoryId", requestBusinessCategory_VM.ParentBusinessCategoryId),
                            new SqlParameter("name", requestBusinessCategory_VM.Name),
                            new SqlParameter("categoryImage", _CategoryImageFileNameGenerated),
                            new SqlParameter("isActive", requestBusinessCategory_VM.IsActive),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode", requestBusinessCategory_VM.Mode),
                            new SqlParameter("categoryKey", requestBusinessCategory_VM.CategoryKey),
                            new SqlParameter("menuTag", requestBusinessCategory_VM.MenuTag),
                            new SqlParameter("profilePageTypeId", requestBusinessCategory_VM.ProfilePageTypeId)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessCategory @id,@parentBusinessCategoryId,@name,@categoryImage,@isActive,@submittedByLoginId,@mode,@categoryKey,@menuTag,@profilePageTypeId", queryParams).FirstOrDefault();
                //return resp;

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                if (resp.ret == 1)
                {

                    if (requestBusinessCategory_VM.Mode == 1)
                    {
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessCategory), _CategoryImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_CategoryImageFile, FileWithPath);
                    }
                    else if (requestBusinessCategory_VM.Mode == 2 && files.Count > 0)
                    {
                        // remove previous file
                        string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousCategoryImageFileName);
                        fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessCategory), _CategoryImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_CategoryImageFile, FileWithPath);
                    }
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
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/BusinessCategory/Delete/{id}")]
        public HttpResponseMessage DeleteBusinessCategory(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                              new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", "4")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).FirstOrDefault();

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
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/BusinessCategory/ChangeStatus/{id}")]
        public HttpResponseMessage ChangeStatusCategory(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                              new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", "6")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).FirstOrDefault();

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

        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/BusinessCategory/GetAllActiveProfilePageTypes")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveProfilePageTypes()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                List<ProfilePageType> profilePageTypes = profilePageTypeService.GetAllActiveProfilePageTypes();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = profilePageTypes;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }


        ///////////////////////////////////////////////////////////////////// Business Course Category /////////////////////////////////////////////////

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessCourseCategory")]
        public HttpResponseMessage AddUpdateCourseCategory_Details()
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
                BusinessCourseCategory_VM requestBusinessCourseCategory_VM = new BusinessCourseCategory_VM();
                requestBusinessCourseCategory_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessCourseCategory_VM.Title = HttpRequest.Params["Title"];
                requestBusinessCourseCategory_VM.Description = HttpRequest.Params["Description"];
                requestBusinessCourseCategory_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ManageBusinessCourseCategoryFile = files["CourseCategoryImage"];
                requestBusinessCourseCategory_VM.CourseCategoryImage = _ManageBusinessCourseCategoryFile; // for validation
                string _BusinessCourseCategoryFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessCourseCategoryFileName = ""; // will be used to delete file while updating.


                // Validate infromation passed
                Error_VM error_VM = requestBusinessCourseCategory_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_ManageBusinessCourseCategoryFile != null)
                    {

                        _BusinessCourseCategoryFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ManageBusinessCourseCategoryFile);
                    }

                }


                if (requestBusinessCourseCategory_VM.Mode == 2)
                {

                    var respGetBusinessServiceData = businessCategoryService.GetBusinessCourseCategoryDetail_ById(requestBusinessCourseCategory_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessCourseCategoryFileNameGenerated = respGetBusinessServiceData.CourseCategoryImage ?? "";


                    }
                    else
                    {
                        _PreviousBusinessCourseCategoryFileName = respGetBusinessServiceData.CourseCategoryImage ?? "";


                    }
                }


                // Call the stored procedure with Mode = 1 to insert a record for each number
                var resp = storedProcedureRepository.SP_InsertUpdateBusinessCourseCategory_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessCourseCategory_Param_Vm
                {
                    Id = requestBusinessCourseCategory_VM.Id,
                    UserLoginId = _LoginID_Exact,
                    Title = requestBusinessCourseCategory_VM.Title,
                    Description = requestBusinessCourseCategory_VM.Description,
                    CourseCategoryImage = _BusinessCourseCategoryFileNameGenerated,
                    Mode = requestBusinessCourseCategory_VM.Mode,
                });

                // Handle the result of the insertion if needed
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                if (resp.ret == 1)
                {

                    // Update Group Image.
                    #region Insert-Update Manage Business UniversityLogo    on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessCourseCategoryFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentCourseCategoryImage), _PreviousBusinessCourseCategoryFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessContentCourseCategoryImage), _BusinessCourseCategoryFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ManageBusinessCourseCategoryFile, NewImageFileWithPath);





                    }
                    #endregion
                }

                // send success response
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
        /// Get Cource Category Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentCourseCategoryDetail")]
        public HttpResponseMessage GetBusinessCourseCategoryDetailById(long Id)
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


                BusinessCourseCategoryDetail_VM resp = new BusinessCourseCategoryDetail_VM();

                resp = businessCategoryService.GetBusinessCourseCategoryDetail_ById(Id);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// <summary>
        /// Delete Course image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/DeleteCourseCategoryDetail")]
        public HttpResponseMessage DeleteCourseCategoryDetailById(long id)
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

                SPResponseViewModel resp = new SPResponseViewModel();


                resp = businessCategoryService.DeleteCourseCategoryById(id);

                apiResponse.status = resp.ret;
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
        /// Get All Business content Teacher University Detail  with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetAllBusinessContentCourseCategoryByPagination")]
        public HttpResponseMessage GetAllBusinessCourseCategoryDataTablePagination()
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

                BusinessContentCourseCategoryDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentCourseCategoryDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = businessCategoryService.GetBusinessCourseCategoryList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = Resources.BusinessPanel.Success,
                    draw = paginationResponse.draw,
                    data = paginationResponse.data,
                    recordsTotal = paginationResponse.recordsTotal,
                    recordsFiltered = paginationResponse.recordsFiltered
                };

                return Request.CreateResponse(HttpStatusCode.OK, objResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        /// <summary>
        /// Get Cource Category Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/GetBusinessContentCourseCategoryDetailList")]
        public HttpResponseMessage GetBusinessCourseCategoryDetai(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessCourseCategoryDetail_VM> resp = businessCategoryService.GetManageBusinessCourseCategorydetailLst(businessOwnerLoginId);




                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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

        /// <summary>
        /// Get business Course Category Detail Icon  by Business -ownerlogin Id  for Visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessContent/GetBusinessCourseCategoryDetailVisitorPanel")]
        public HttpResponseMessage GetBusinessCourseCategoryVisitorPanel()
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


                List<BusinessCourseCategoryDetail_VM> resp = businessCategoryService.GetBusinessCourseCategoryDetailList(_BusinessOwnerLoginId);



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


        /// <summary>
        /// To Add/Update Business Course Category Detail 
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,SuperAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessCourseCategoryDetail")]
        public HttpResponseMessage AddUpdateCourceCategory_Details()
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
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentCourceCategory_PPCMeta_Param_VM requestBusinessCourceCategory_VM = new BusinessContentCourceCategory_PPCMeta_Param_VM();
                // requestBusinessCourceCategory_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                string CourseCategoryId = HttpRequest.Params["CourseCategoryId"];
                //requestBusinessCourceCategory_VM.CategoryCourseId = Convert.ToInt32(HttpRequest.Params["CategoryCourseId"]);
                requestBusinessCourceCategory_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                List<int> integerList = CourseCategoryId.Split(',')
                                  .Select(int.Parse)
                                  .ToList();

                // Validate infromation passed
                //Error_VM error_VM = requestBusinessLanguage_VM.ValidInformation();

                //if (!error_VM.Valid)
                //{
                //    apiResponse.status = -1;
                //    apiResponse.message = error_VM.Message;
                //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                //}





                SPResponseViewModel resp = new SPResponseViewModel();


                resp = businessCategoryService.DeleteCourseCategoryDetailById(_BusinessOwnerLoginId);




                foreach (int number in integerList)
                {
                    // Call the stored procedure with Mode = 1 to insert a record for each number
                    resp = storedProcedureRepository.SP_InsertUpdateBusinessContentCourceCategory_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentCourceCategory_PPCMeta_Param_VM
                    {
                        // Id = requestBusinessCourceCategory_VM.Id,
                        ProfilePageTypeId = BusinesssProfilePageType.Id,
                        BusinessOwnerLoginId = _BusinessOwnerLoginId,
                        CourseCategoryId = number,
                        Mode = 1
                    });

                    // Handle the result of the insertion if needed
                    if (resp.ret <= 0)
                    {
                        apiResponse.status = resp.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }

                // send success response
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


        #region To Get All User Course  Category Detail -----------------------------------
        /// <summary>
        /// To Get All User 
        /// </summary>
        /// <returns>To Get  All  Inserted User Course Category  Detail </returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessCourseCategoryDetail")]
        public HttpResponseMessage GetAllUserContentCourseCategoryList()
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

                List<BusinessContentCourseCategoryDetail_PPCMeta> usercontentcoursecategorydetailresponse = businessCategoryService.GetBusinessCourseCategoryDetail(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = usercontentcoursecategorydetailresponse;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }
        #endregion


        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessCourseCategoryDetailList")]
        public HttpResponseMessage GetAllUserContentCourseCategoryListDetail(long businessOwnerLoginId)
        {

            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                List<BusinessContentCourseCategoryDetail_PPCMeta> usercontentcategorydetailresponse = businessCategoryService.GetBusinessCourseCategoryDetail(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = usercontentcategorydetailresponse;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        /// <summary>
        /// To Get SubCategory Detail List By Category Key 'b2b'
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessCategory/GetAllSubBusinessCategoryDetail")]
        [HttpGet]
        public HttpResponseMessage GetAllSubBusinessCategoryDetail()
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


                List<BusinessCategory_VM> ParentBusinessCategory = businessCategoryService.GetAllB2BSubCategories();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = ParentBusinessCategory;

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
        /// To Get All B2B Sub-Category Detail List By Category Key 'b2b' for Home Page.
        /// </summary>
        /// <returns>Sub Categories List</returns>
        [HttpGet]
        [Route("api/BusinessCategory/GetAllB2BSubBusinessCategoryListForHomePage")]
        public HttpResponseMessage GetAllB2BSubBusinessCategoryListForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                
                List<BusinessCategory_VM> ParentBusinessCategory = businessCategoryService.GetAllB2BSubCategoriesForHomePage();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = ParentBusinessCategory;

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
        /// To Get All B2B Business List By Sub-Category for Home Page.
        /// </summary>
        /// <param name="subCategoryId">Busiens Sub-Category Id</param>
        /// <returns>Businesses List</returns>
        [HttpGet]
        [Route("api/BusinessCategory/GetAllB2BBusinessListBySubCategoryForHomePage")]
        public HttpResponseMessage GetAllB2BBusinessListBySubCategoryForHomePage(long subCategoryId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                
                List<BusinessListByCategory_HomePage_VM> ParentBusinessCategory = businessCategoryService.GetAllB2BBusinessListBySubCategoryForHomePage(subCategoryId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = ParentBusinessCategory;

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
        /// Get All Active B2B Sub-Categories List for dropdown
        /// </summary>
        /// <returns>B2B Active Sub-Categoires</returns>
        [Route("api/BusinessCategory/GetAllActiveB2BSubCategories")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveB2BSubCategories()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                List<BusinessCategory_VM> B2bSubCategories = businessCategoryService.GetAllB2BSubCategories();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = B2bSubCategories;

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
        /// for dropdown list of instructor category
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessCategory/GetAllInstructorBusinessCategories")]
        public HttpResponseMessage GetAllInstructorBusinessCategories()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<BusinessCategory_VM> ParentBusinessCategory = businessCategoryService.GetAllInstructorBusinessCategories();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = ParentBusinessCategory;

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
        [Route("api/BusinessCategory/GetAllB2BBusinessListBySubCategoryForBusinessListPage")]
        public HttpResponseMessage GetAllB2BBusinessListBySubCategoryForBusinessListPage(long subCategoryId, long lastRecordId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessListByCategory_HomePage_VM> lst = businessCategoryService.GetAllB2BBusinessListBySubCategoryBusinessListPage(subCategoryId, lastRecordId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    BusinessList = lst,
                };

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