using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
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
using MasterZoneMvc.Services;
using System.IO;

namespace MasterZoneMvc.WebAPIs
{
    public class ClassCategoryTypeAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private ClassCategoryTypeService classCategoryTypeService;
        private FileHelper fileHelper;

        public ClassCategoryTypeAPIController()
        {
            db = new MasterZoneDbContext();
            classCategoryTypeService = new ClassCategoryTypeService(db);
            fileHelper = new FileHelper();
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
        /// Add or Update Class-Category-Type
        /// </summary>
        /// <returns>Success or Error Message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategoryType/AddUpdate")]
        public HttpResponseMessage AddUpdateClassCategoryType()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestClassCategoryType requestClassCategoryType_VM = new RequestClassCategoryType();
                requestClassCategoryType_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestClassCategoryType_VM.BusinessCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessCategoryId"]);
                requestClassCategoryType_VM.IsActive = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                requestClassCategoryType_VM.Name = HttpRequest.Params["Name"].Trim();
                requestClassCategoryType_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestClassCategoryType_VM.ParentClassCategoryTypeId = Convert.ToInt64(HttpRequest.Params["ParentClassCategoryTypeId"]);
                requestClassCategoryType_VM.Description = string.IsNullOrEmpty(HttpRequest.Params["Description"]) ? "" : HttpRequest.Params["Description"].Trim();

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _CategoryImageFile = files["CategoryImage"];
                string _CategoryImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousCategoryImageFileName = ""; // will be used to delete file while updating.
                requestClassCategoryType_VM.Image = _CategoryImageFile; // to validate

                // Validate infromation passed
                Error_VM error_VM = requestClassCategoryType_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_CategoryImageFile != null)
                    {
                        _CategoryImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CategoryImageFile);
                    }
                }

                if (requestClassCategoryType_VM.Mode == 2)
                {
                    var classCategoryType = classCategoryTypeService.GetClassCategoryTypeById(requestClassCategoryType_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _CategoryImageFileNameGenerated = classCategoryType.Image;
                    }
                    else
                    {
                        _PreviousCategoryImageFileName = classCategoryType.Image;
                    }
                }

                // Insert-Update
                var resp = classCategoryTypeService.AddUpdate(new ViewModels.StoredProcedureParams.SP_InsertUpdateClassCategoryType_Params_VM()
                {
                    Id = requestClassCategoryType_VM.Id,
                    BusinessCategoryId = requestClassCategoryType_VM.BusinessCategoryId,
                    ParentClassCategoryTypeId = requestClassCategoryType_VM.ParentClassCategoryTypeId,
                    Name = requestClassCategoryType_VM.Name,
                    Image = _CategoryImageFileNameGenerated,
                    IsActive = requestClassCategoryType_VM.IsActive,
                    Mode = requestClassCategoryType_VM.Mode,
                    SubmittedByLoginId = _LoginID_Exact,
                    Description = requestClassCategoryType_VM.Description
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update class category Image.
                    #region Insert-Update Sponsor Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousCategoryImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassCategoryTypeImage), _PreviousCategoryImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassCategoryTypeImage), _CategoryImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_CategoryImageFile, FileWithPath);


                    }

                    #endregion
                }

                //// Add-Update Category Image.
                //if (resp.ret == 1)
                //{
                //    if (requestClassCategoryType_VM.Mode == 1)
                //    {
                //        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassCategoryTypeImage), _CategoryImageFileNameGenerated);
                //        fileHelper.SaveUploadedFile(_CategoryImageFile, FileWithPath);
                //    }
                //    else if (requestClassCategoryType_VM.Mode == 2 && files.Count > 0)
                //    {

                //        // remove previous file
                //        if(!string.IsNullOrEmpty(_PreviousCategoryImageFileName))
                //        {
                //            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassCategoryTypeImage), _PreviousCategoryImageFileName);
                //            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                //        }

                //        // save new file
                //        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassCategoryTypeImage), _CategoryImageFileNameGenerated);
                //        fileHelper.SaveUploadedFile(_CategoryImageFile, FileWithPath);
                //    }

                //}

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
        /// Get Single Class-Category-Type Detail
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategoryType/GetById")]
        public HttpResponseMessage GetClassCategoryTypeById(long id)
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

                var resp = classCategoryTypeService.GetClassCategoryTypeById(id);

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
        /// Change Active-Inactive Status by Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns>Returns response +ve for success, else -ve with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategoryType/ChangeStatus")]
        public HttpResponseMessage ChangeStatusClassCategoryType(long id)
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

                var resp = classCategoryTypeService.ChangeStatusClassCategoryType(id);

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
        /// Delete Class-Category-Type By Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategoryType/Delete")]
        public HttpResponseMessage DeleteClassCategoryType(long id)
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

                // Delete By Id
                var resp = classCategoryTypeService.DeleteClassCategoryType(id);
                
                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new {};


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
        /// To change the Class-Category-Types Home Visibility Status
        /// </summary>
        /// <param name="id">Class-Category-Type-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategoryType/ToggleHomePageVisibilityStatus/{id}")]
        public HttpResponseMessage ToggleHomePageVisibilityStatus(long id)
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
                    apiResponse.data = new { };

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                // Insert-Update
                var resp = classCategoryTypeService.AddUpdate(new ViewModels.StoredProcedureParams.SP_InsertUpdateClassCategoryType_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    Mode = 5,
                });

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
        /// Get All Class-Category-Types (Sub) For Home Page display
        /// </summary>
        /// <returns>Class-Category-Type List</returns>
        [HttpGet]
        [Route("api/ClassCategoryType/GetAllClassSubCategoriesForHomePage")]
        public HttpResponseMessage GetAllClassSubCategoriesForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // Get Records For Home Page (All active)
                var resp = classCategoryTypeService.GetAllSubClassCategoryTypesForHomePage();

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
        /// Get All Active Class-Category-Types by logged in business =>  Business-Category-Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns>All Active Class-Category-Types</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassCategoryType/GetByBusinessCategory")]
        public HttpResponseMessage GetClassCategoryTypeByBusinessCategory()
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

                // Get Business-Owner-Table-Data by Id
                BusinessOwnerService businessOwnerService = new BusinessOwnerService(db);
                var businessData = businessOwnerService.GetBusinessOwnerTableDataById(_BusinessOwnerLoginId);

                var activeTypes = classCategoryTypeService.GetAllActiveClassCategoryTypesByBusinessCategory(businessData.BusinessSubCategoryId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = activeTypes;

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
        [Route("api/ClassCategoryType/GetAllClasssCategoryListDetail")]
        [HttpGet]
        public HttpResponseMessage GetAllClasssCategoryDetail()
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

                List<ClassCategoryType_VM> ParentBusinessCategory = classCategoryTypeService.Get_ManageClasssCategoryDetail(_LoginID_Exact);

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
        /// To get all  Parent-Class-Categories For SuperAdmin (ClassCategory)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategory/GetAllParentClassCategories")]
        [HttpGet]
        public HttpResponseMessage GetAllParentBusinessCategoryDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var ParentBusinessCategory = classCategoryTypeService.GetAllParentClassCategoryList();

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
        /// To get all Sub-Class-Categories of Parent-Class-Category by Id For SuperAdmin (ClassCategory)
        /// </summary>
        /// <param name="id">Parent-Class-Category-Type-Id</param>
        /// <returns>Returns Sub-Class-Categories of Parent-Category</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ClassCategory/GetAllSubClassCategories")]
        [HttpGet]
        public HttpResponseMessage GetAllSubClassCategories(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var ParentBusinessCategory = classCategoryTypeService.GetAllSubClassCategoryTypesByParentCategory(id);

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
        /// To get all Active Parent-ClassCategoryDetail
        /// </summary>
        /// <returns>List of active Parent Categories</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin,BusinessAdmin,Staff")]
        [Route("api/ClassCategoryType/GetAllActiveParentClassCategoryTypes")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveParentClassCategoryTypes()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var ParentBusinessCategory = classCategoryTypeService.GetAllActiveParentClassCategoryList();

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
        /// Get All Active Class-Category-Types by logged in business =>  Business-Category-Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns>All Active Class-Category-Types</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/ClassCategoryType/GetByParentCategory")]
        public HttpResponseMessage GetClassCategoryTypeByParentCategory(long classCategoryTypeId)
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

                var activeTypes = classCategoryTypeService.GetAllActiveClassCategoryTypesByParentCategory(classCategoryTypeId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = activeTypes;

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
        /// Get All Classs Category Types Detail DropDown for frontpage dropdown 
        /// </summary>
        /// <returns>List of Classs-Category-Types Parent with Their Sub-Categories List.</returns>
        [Route("api/ClassCategoryType/GetAllClasssCategoryDetailDropDown")]
        [HttpGet]
        public HttpResponseMessage GetAllClasssCategoryDetailDropDown()
        
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<ClassCategoryType_VM> classCategoryTypes = classCategoryTypeService.Get_ManageClasssCategoryDetailDropdown();
                List<ClassCategoryType_VM> ParentClassCategories = classCategoryTypes.Where(c => c.ParentClassCategoryTypeId == 0).ToList();

                foreach (var p in ParentClassCategories)
                {
                    p.SubClassCategory = classCategoryTypes.Where(c => c.ParentClassCategoryTypeId == p.Id).ToList();
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = ParentClassCategories;

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