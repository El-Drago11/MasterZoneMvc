using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
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

namespace MasterZoneMvc.WebAPIs
{
    public class MenuAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private MenuService menuService;

        public MenuAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            menuService = new MenuService(db);
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
        [Route("api/Menu/GetAllVisitorMenuItems")]
        [HttpGet]
        public HttpResponseMessage GetAllVisitorMenuItems()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                // Get All Visitor Menu Items List
                menuService = new MenuService(db);
                List<VisitorMenu_VM> menuItems = menuService.GetAllVisitorMenuItems();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = menuItems;

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
        /// Get All Active Visitor Menu List
        /// </summary>
        /// <returns></returns>
        [Route("api/Menu/GetAllActiveVisitorMenuItems")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveMenuItems()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                menuService = new MenuService(db);
                List<VisitorMenu_VM> menuTags = menuService.GetAllActiveVisitorMenuList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = menuTags;

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
        /// Get All Active Menu-Tags For Super-Admin Panel
        /// </summary>
        /// <returns>List string of Tags</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Menu/GetAllActiveMenuTagsBySuperAdmin")]
        [HttpGet]
        public HttpResponseMessage GetAllActiveMenuTags()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                menuService = new MenuService(db);
                List<string> menuTags = menuService.GetAllActiveVisitorMenuTagsList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = menuTags;

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
        /// Get All Active Menu-Tags For Super-Admin Panel
        /// </summary>
        /// <returns>List string of Tags</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Menu/GetAllVisitorMenuBySuperAdmin")]
        [HttpGet]
        public HttpResponseMessage GetAllVisitorMenuItemListBySuperAdmin()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                // Get All Visitor Menu Items List
                menuService = new MenuService(db);
                List<VisitorMenu_VM> menuItems = menuService.GetAllVisitorMenuItems();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = menuItems;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        #region CRUD ---------------------------------------------------------------------

        /// <summary>
        /// Get Menu Detail By Id for Admin Panel
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Menu</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Menu/GetById/{menuId}")]
        public HttpResponseMessage MenuGetById(long menuId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (menuId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Menu By Id
                var menuItem = menuService.GetMenuById(menuId);
                
                if (menuItem == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.SuperAdminPanel.MenuNotFound_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = menuItem;
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
        /// Add or Update Menu Item - Admin Panel
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Menu/AddUpdate")]
        public HttpResponseMessage AddUpdateMenu()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestMenuViewModel requestMenu_VM = new RequestMenuViewModel();
                requestMenu_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestMenu_VM.IsActive = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                requestMenu_VM.IsShowOnHomePage = Convert.ToInt32(HttpRequest.Params["IsShowOnHomePage"]);
                requestMenu_VM.Name = HttpRequest.Params["Name"].Trim();
                requestMenu_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestMenu_VM.MenuTag = HttpRequest.Params["MenuTag"].Trim();
                requestMenu_VM.PageLink = HttpRequest.Params["PageLink"].Trim();
                requestMenu_VM.SortOrder = String.IsNullOrEmpty(HttpRequest.Params["SortOrder"].Trim()) ? 0 : Convert.ToInt32(HttpRequest.Params["SortOrder"].Trim());
               
                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _MenuImageFile = files["MenuImage"];
                string _MenuImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousMenuImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestMenu_VM.ValidInformation();
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
                    if (!validImageTypes.Contains(_MenuImageFile.ContentType))
                    {
                        isValidImage = false;
                        apiResponse.message = Resources.SuperAdminPanel.ValidImageFile_ErrorMessage;
                    }
                    else if (_MenuImageFile.ContentLength > 1024 * 1024) // 1 MB
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

                    if (_MenuImageFile != null)
                    {
                        _MenuImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_MenuImageFile);
                    }
                }

                if (requestMenu_VM.Mode == 2)
                {
                    VisitorMenu_VM menuItem = menuService.GetMenuById(requestMenu_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _MenuImageFileNameGenerated = menuItem.Image;
                    }
                    else
                    {
                        _PreviousMenuImageFileName = menuItem.Image;
                    }
                }

                // Insert-Update Menu Record
                var resp = menuService.InsertUpdateVisitorMenu(new ViewModels.StoredProcedureParams.SP_InsertUpdateMenu_Params_VM { 
                    Id = requestMenu_VM.Id,
                    ParentMenuId = requestMenu_VM.ParentMenuId,
                    Name = requestMenu_VM.Name,
                    Image = _MenuImageFileNameGenerated,
                    PageLink = requestMenu_VM.PageLink,
                    IsActive = requestMenu_VM.IsActive,
                    Tag = requestMenu_VM.MenuTag,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestMenu_VM.Mode,
                    IsShowOnHomePage = requestMenu_VM.IsShowOnHomePage,
                    SortOrder = requestMenu_VM.SortOrder
                });
                
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                if (resp.ret == 1)
                {
                    if (requestMenu_VM.Mode == 1)
                    {
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MenuImage), _MenuImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_MenuImageFile, FileWithPath);
                    }
                    else if (requestMenu_VM.Mode == 2 && files.Count > 0)
                    {
                        if(!String.IsNullOrEmpty(_PreviousMenuImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MenuImage), _PreviousMenuImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_MenuImage), _MenuImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_MenuImageFile, FileWithPath);
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

        /// <summary>
        /// Delete Menu By Menu-Id
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Status 1 if deleted else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Menu/Delete/{menuId}")]
        public HttpResponseMessage DeleteMenu(long menuId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (menuId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Delete Menu by Id
                SPResponseViewModel respDeleteMenu = menuService.DeleteMenuById(menuId);

                apiResponse.status = respDeleteMenu.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(respDeleteMenu.resourceFileName, respDeleteMenu.resourceKey);
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
        /// Change Menu Active/Inactive status [automatically swithes to opposite value]
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Api Resoponse: Status 1 if updated else -ve value with error message.</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Menu/ChangeStatus/{menuId}")]
        public HttpResponseMessage ChangeStatusMenu(long menuId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (menuId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Change Menu Status
                var resp = menuService.ChangeMenuStatusById(menuId);

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

        #endregion -------------------------------------------------------------------------


        #region Filter Records List By Menu Tag --------------------------------------------

        /// <summary>
        /// Get All Academy/Business Owners list (B2B Category) by Menu Tag and location filter 
        /// </summary>
        /// <param name="MenuTag">The business owners to be filtered by Menu-Tag. From which user came</param>
        /// <param name="CategoryKey"></param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Record Id fetched</param>
        /// <param name="recordLimit">No. of next records to fetch</param>
        /// <returns><List of Record/returns>
        [HttpPost]
        [Route("api/Menu/GetAllBusinessAcademyByMenuTag")]
        public HttpResponseMessage GetAllBusinessAcademyListByMenuTag(SearchByMenu_APIParmas_VM params_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                
                int recordLimit = StaticResources.RecordLimit_Default;
                if (!string.IsNullOrEmpty(params_VM.RecordLimit))
                {
                    recordLimit = Convert.ToInt32(params_VM.RecordLimit);
                }
                
                List<BusinessOnwerListByMenuTag_VM> response = menuService.GetAllBusinessAcademiesListByMenutag(new FilterRecordByMenu_VM() { MenuTag = params_VM.MenuTag, City = params_VM.City, LastRecordId = params_VM.LastRecordId, RecordLimit = recordLimit, CategorySearchValue = params_VM.CategorySearchValue, SearchType = params_VM.SearchType, SearchValue = params_VM.SearchValue});

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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
        /// Get All Academy/Business Owners list (B2B Category) by Menu Tag and location filter - For Logged in user (Favourite)
        /// </summary>
        /// <param name="MenuTag">The business owners to be filtered by Menu-Tag. From which user came</param>
        /// <param name="CategoryKey"></param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Record Id fetched</param>
        /// <param name="recordLimit">No. of next records to fetch</param>
        /// <returns>List of Record</returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Menu/GetAllCoachesByMenuTag")]
        public HttpResponseMessage GetAllActiveInstructor(SearchByMenu_APIParmas_VM params_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();  
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                int recordLimit = StaticResources.RecordLimit_Default;
                if (!string.IsNullOrEmpty(params_VM.RecordLimit))
                {
                    recordLimit = Convert.ToInt32(params_VM.RecordLimit);
                }

                List<InstructorListByMenuTag_VM> response = menuService.GetAllInstructorByMenutag(new FilterRecordByMenu_VM() { MenuTag = params_VM.MenuTag, City = params_VM.City, LastRecordId = params_VM.LastRecordId, RecordLimit = recordLimit, UserLoginId = _LoginID_Exact, CategorySearchValue = params_VM.CategorySearchValue, SearchType = params_VM.SearchType, SearchValue = params_VM.SearchValue });

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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
        /// Get All Academy/Business Owners list (B2B Category) by Menu Tag and location filter 
        /// </summary>
        /// <param name="MenuTag">The business owners to be filtered by Menu-Tag. From which user came</param>
        /// <param name="CategoryKey"></param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Record Id fetched</param>
        /// <param name="recordLimit">No. of next records to fetch</param>
        /// <returns>List of Record</returns>
        [HttpPost]
        [Route("api/Menu/GetAllActivityByMenuTag")]
        public HttpResponseMessage GetAllActiveEvents(SearchByMenu_APIParmas_VM params_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                int recordLimit = StaticResources.RecordLimit_Default;
                if (!string.IsNullOrEmpty(params_VM.RecordLimit))
                {
                    recordLimit = Convert.ToInt32(params_VM.RecordLimit);
                }
                List<EventListByMenuTag_VM> response = menuService.GetAllEventActivitiesByMenutag(new FilterRecordByMenu_VM() { MenuTag = params_VM.MenuTag, City = params_VM.City, LastRecordId = params_VM.LastRecordId, RecordLimit = recordLimit, CategorySearchValue = params_VM.CategorySearchValue, SearchType = params_VM.SearchType, SearchValue = params_VM.SearchValue });

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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
        /// Get All Academy/Business Owners list (B2B Category) by Menu Tag and location filter 
        /// </summary>
        /// <param name="MenuTag">The business owners to be filtered by Menu-Tag. From which user came</param>
        /// <param name="CategoryKey"></param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Record Id fetched</param>
        /// <param name="recordLimit">No. of next records to fetch</param>
        /// <returns>List of Record</returns> 
        [HttpPost]
        [Route("api/Menu/GetAllClassesByMenuTag")]
        public HttpResponseMessage GetAllClasses(SearchByMenu_APIParmas_VM params_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                int recordLimit = StaticResources.RecordLimit_Default;
                if (!string.IsNullOrEmpty(params_VM.RecordLimit))
                {
                    recordLimit = Convert.ToInt32(params_VM.RecordLimit);
                }
                List<ClassSearchList_VM> response = menuService.GetAllClassesByMenutag(new FilterRecordByMenu_VM() { MenuTag = params_VM.MenuTag, City = params_VM.City, LastRecordId = params_VM.LastRecordId, RecordLimit = recordLimit, CategorySearchValue = params_VM.CategorySearchValue, SearchType = params_VM.SearchType, SearchValue = params_VM.SearchValue });

                DateTime currentDateTime = DateTime.UtcNow;
                string currentDay = currentDateTime.ToString("dddd");
                foreach(var objClass in response)
                {
                    List<string> classDays = objClass.ClassDays.Split(',').ToList();
                    if(classDays.Contains(currentDay))
                    {
                        objClass.DayAbbr = currentDay.Substring(0, 3);
                    }
                    else
                    {
                        objClass.DayAbbr = classDays.First().Substring(0, 3);
                    }
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }

        }

        #endregion --------------------------------------------------------------------------
    }
}