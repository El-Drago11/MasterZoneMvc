using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
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
using MasterZoneMvc.Repository;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Common.ValidationHelpers;
using System.Text.Json;

namespace MasterZoneMvc.WebAPIs
{
    public class ClassAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private ClassService classService;
        private FileHelper fileHelper;
        private GroupService groupService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public ClassAPIController()
        {
            db = new MasterZoneDbContext();
            classService = new ClassService(db);
            fileHelper = new FileHelper();
            businessOwnerService = new BusinessOwnerService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        #region Class APIs ---------------------------------------------------------------------------

        /// <summary>
        /// Add or Upadate Class -Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/AddUpdate")]
        public HttpResponseMessage AddUpdateClass()
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
                RequestClassViewModel requestClass_VM = new RequestClassViewModel();
                requestClass_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                //requestEvent_VM.UserLoginId = _LoginId;
                requestClass_VM.Name = HttpRequest.Params["className"].Trim();
                requestClass_VM.Description = HttpRequest.Params["classDescription"].Trim();
                //requestClass_VM.ScheduledStartOnTime_24HF = HttpRequest.Params["classStartTime"].Trim();
                //requestClass_VM.ScheduledEndOnTime_24HF = HttpRequest.Params["classEndTime"].Trim();
                requestClass_VM.Price = Convert.ToDecimal(HttpRequest.Params["classPrice"]);
                requestClass_VM.ClassMode = HttpRequest.Params["classmode"];
                //requestClass_VM.StudentMaxStrength = Convert.ToInt32(HttpRequest.Params["classLimitofStudent"]);
                requestClass_VM.ClassDays = HttpRequest.Params["classSelectedDays"];
                requestClass_VM.OnlineClassLink = HttpRequest.Params["classOnlineURLLink"];
                requestClass_VM.ClassURLLinkPassword = HttpRequest.Params["classOnlineURLLinkPassword"];
                requestClass_VM.IsPaid = Convert.ToInt32(HttpRequest.Params["classPriceOption"]);
                requestClass_VM.ClassPriceType = HttpRequest.Params["classPaidOption"];
                requestClass_VM.Address = HttpRequest.Params["classLocationAddress"];
                requestClass_VM.City = HttpRequest.Params["classLocationCity"];
                requestClass_VM.Country = HttpRequest.Params["classLocationCountry"];
                requestClass_VM.State = HttpRequest.Params["classLocationState"];
                requestClass_VM.LandMark = HttpRequest.Params["classLocationLandMark"];
                //requestClass_VM.InstructorLoginId = Convert.ToInt64(HttpRequest.Params["instructor"]);
                requestClass_VM.Pincode = HttpRequest.Params["classLocationPinCode"];
                requestClass_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);
                //requestClass_VM.GroupId = Convert.ToInt64(HttpRequest.Params["groupId"]);
                requestClass_VM.ClassType = HttpRequest.Params["classType"];
                requestClass_VM.HowToBookText = HttpRequest.Params["howToBookText"].Trim();
                requestClass_VM.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["classLocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["classLocationLatitude"]) : 0;
                requestClass_VM.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["classLocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["classLocationLongitude"]) : 0;
                requestClass_VM.ClassCategoryTypeId = Convert.ToInt64(HttpRequest.Params["classCategoryTypeId"]);

                string _classBatchList = HttpRequest.Params["classBatchesList"];
                List<string> classBatchListType = new List<string>(_classBatchList.Split(','));
                requestClass_VM.ClassBatches = classBatchListType; // validated in View-Model

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ClassImageFile = files["ClassImage"];
                requestClass_VM.ClassImage = _ClassImageFile; // for validation
                string _ClassImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousClassImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestClass_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_ClassImageFile != null)
                    {
                        _ClassImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ClassImageFile);
                    }
                }

                if (requestClass_VM.Mode == 2)
                {

                    var respGetClassData = classService.GetClassDataByID(requestClass_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _ClassImageFileNameGenerated = respGetClassData.ClassImage ?? "";
                    }
                    else
                    {
                        _PreviousClassImageFileName = respGetClassData.ClassImage ?? "";
                    }
                }

                if (requestClass_VM.ClassMode == "1")
                {
                    requestClass_VM.ClassMode = "Online";
                    requestClass_VM.Address = "";
                    requestClass_VM.City = "";
                    requestClass_VM.State = "";
                    requestClass_VM.Country = "";
                    requestClass_VM.LandMark = "";
                    requestClass_VM.Pincode = "0";
                }
                else
                {
                    requestClass_VM.ClassMode = "Offline";
                    requestClass_VM.OnlineClassLink = "";
                    requestClass_VM.ClassURLLinkPassword = "";
                }

                // Class Days Short Form
                string _classDays_ShortForm = "";

                if (requestClass_VM.ClassDays != null)
                {
                    string[] shortForms = requestClass_VM.ClassDays.Split(',');
                    for (int i = 0; i < shortForms.Length; i++)
                    {
                        if (shortForms[i] == "Sunday" || shortForms[i] == "Saturday" || shortForms[i] == "Thursday")
                            shortForms[i] = shortForms[i].Substring(0, 2);
                        else
                            shortForms[i] = shortForms[i].Substring(0, 1);
                    }
                    _classDays_ShortForm = String.Join(" ", shortForms);
                }


                //Insert-Update Class Batches
                #region Insert-Update Class-Batches ---------------------------------

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Insert-Update Class Record --------------------
                        SqlParameter[] queryParams = new SqlParameter[]
                        {
                                new SqlParameter("id", requestClass_VM.Id),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("name", requestClass_VM.Name),
                                new SqlParameter("classDescription", requestClass_VM.Description),
                                new SqlParameter("startTime24HF", "00"),
                                new SqlParameter("endTime24HF", "00"),
                                new SqlParameter("classMode", requestClass_VM.ClassMode),
                                new SqlParameter("studentMaxStrength", "-1"),
                                new SqlParameter("classDays", requestClass_VM.ClassDays),
                                new SqlParameter("price", requestClass_VM.Price),
                                new SqlParameter("onlineClassLink",  requestClass_VM.OnlineClassLink),
                                new SqlParameter("classOfflineAddress",  requestClass_VM.Address),
                                new SqlParameter("classOfflineCountry",requestClass_VM.Country),
                                new SqlParameter("classOfflineCity",requestClass_VM.City),
                                new SqlParameter("classOfflinePinCode",requestClass_VM.Pincode),
                                new SqlParameter("classOfflineLandMark",requestClass_VM.LandMark),
                                new SqlParameter("classOfflineState",requestClass_VM.State),
                                new SqlParameter("classPriceType",requestClass_VM.ClassPriceType),
                                new SqlParameter("isPaid",requestClass_VM.IsPaid),
                                new SqlParameter("classOnlineURLLinkPassword",requestClass_VM.ClassURLLinkPassword),
                                new SqlParameter("instructor", "-1"),
                                new SqlParameter("submittedByLoginId", _LoginId),
                                new SqlParameter("mode", requestClass_VM.Mode),
                                new SqlParameter("classImage", _ClassImageFileNameGenerated),
                                new SqlParameter("howToBookText", requestClass_VM.HowToBookText),
                                new SqlParameter("groupId", "-1"),
                                new SqlParameter("classType", requestClass_VM.ClassType),
                                new SqlParameter("classDuration", "-1"),
                                new SqlParameter("classLocationLatitude",requestClass_VM.Latitude),
                                new SqlParameter("classLocationLongitude",requestClass_VM.Longitude),
                                new SqlParameter("classDays_ShortForm",_classDays_ShortForm),
                                new SqlParameter("classCategoryTypeId", requestClass_VM.ClassCategoryTypeId)
                        };

                        var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateClass @id,@userLoginId,@name,@classDescription,@startTime24HF,@endTime24HF,@classMode,@studentMaxStrength,@classDays,@price,@onlineClassLink,@classOfflineAddress,@classOfflineCountry,@classOfflineCity,@classOfflinePinCode,@classOfflineLandMark,@classOfflineState,@classPriceType,@isPaid,@classOnlineURLLinkPassword,@instructor,@submittedbyLoginId,@mode,@classImage,@howToBookText,@groupId,@classType,@classDuration,@classLocationLatitude,@classLocationLongitude,@classDays_ShortForm,@classCategoryTypeId", queryParams).FirstOrDefault();

                        if (resp.ret <= 0)
                        {
                            apiResponse.status = resp.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        // Update Class Image.
                        #region Insert-Update Class Image on Server
                        if (files.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(_PreviousClassImageFileName))
                            {
                                // remove previous file
                                string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassImage), _PreviousClassImageFileName);
                                fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                            }

                            // save new file
                            string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassImage), _ClassImageFileNameGenerated);
                            fileHelper.SaveUploadedFile(_ClassImageFile, FileWithPath);
                        }
                        #endregion

                        #endregion --------------------

                        #region Insert-Update Class Batches --------------------------------------
                        if (resp.ret == 1)
                        {
                            long _ClassId = (requestClass_VM.Mode == 2) ? requestClass_VM.Id : resp.Id;
                            // delete all class-batches for the class
                            var deleteClassBatchResp = classService.DeleteClassBatchesByClass(_BusinessOwnerLoginId, _ClassId);

                            if (requestClass_VM.Mode == 2 && deleteClassBatchResp.ret <= 0)
                            {
                                transaction.Rollback();
                                apiResponse.status = deleteClassBatchResp.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(deleteClassBatchResp.resourceFileName, deleteClassBatchResp.resourceKey);
                                apiResponse.data = new { };
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }
                            else
                            {
                                // insert all class-batches from list.
                                foreach (string batchId in requestClass_VM.ClassBatches)
                                {
                                    var insertUpdateClassBatchResp = classService.InsertUpdateClassBatches(new ViewModels.StoredProcedureParams.SP_InsertUpdateClassBatches_Params_VM()
                                    {
                                        BatchId = Convert.ToInt64(batchId),
                                        ClassId = _ClassId,
                                        BusinessOwnerLoginId = _BusinessOwnerLoginId,
                                    });

                                    if (insertUpdateClassBatchResp.ret <= 0)
                                    {
                                        transaction.Rollback();
                                        apiResponse.status = insertUpdateClassBatchResp.ret;
                                        apiResponse.message = ResourcesHelper.GetResourceValue(insertUpdateClassBatchResp.resourceFileName, insertUpdateClassBatchResp.resourceKey);
                                        apiResponse.data = new { };
                                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                    }
                                }
                            }
                        }
                        #endregion ----------------------------------------

                        db.SaveChanges(); // Save changes to the database
                        transaction.Commit(); // Commit the transaction if everything is successful

                        // send success response
                        apiResponse.status = 1;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                        apiResponse.data = new { };
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions and perform error handling or logging
                        transaction.Rollback(); // Roll back the transaction
                        apiResponse.status = -100;
                        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                        apiResponse.data = new { };
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                }

                #endregion -----------------------

            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = "Internal Server Error!";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get All Class with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllClassForDataTablePagination()
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

                ClasstList_Pagination_SQL_Params_VM _Params_VM = new ClasstList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.UserLoginId = _BusinessOwnerLoginId;
                _Params_VM.CreatedByLoginId = _LoginId;

                var paginationResponse = classService.GetClassList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// Get All Class with Pagination For the Super-Admin-Panel [Super, SubAdmin]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns>All Classes created by business owners list with pagination</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Class/GetAllClassesByPaginationForSuperAdmin")]
        [HttpPost]
        public HttpResponseMessage GetAllClassesByPaginationForSuperAdmin()
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

                ClasstList_Pagination_SQL_Params_VM _Params_VM = new ClasstList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                // no need to set data

                var paginationResponse = classService.GetClassList_Pagination_ForSuperAdmin(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// To change the Class Home Visibility Status - [Super-Admin-Panel]
        /// </summary>
        /// <param name="id">Class-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Class/ToggleHomePageVisibilityStatus/{id}")]
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

                SqlParameter[] queryParams = new SqlParameter[]
                  {
                        new SqlParameter("id", id),
                        new SqlParameter("userLoginId", ""),
                        new SqlParameter("name", ""),
                        new SqlParameter("classDescription", ""),
                        new SqlParameter("startTime24HF", ""),
                        new SqlParameter("endTime24HF", ""),
                        new SqlParameter("classMode", ""),
                        new SqlParameter("studentMaxStrength", ""),
                        new SqlParameter("classDays", ""),
                        new SqlParameter("price", "0"),
                        new SqlParameter("onlineClassLink",  ""),
                        new SqlParameter("classOfflineAddress",  ""),
                        new SqlParameter("classOfflineCountry",""),
                        new SqlParameter("classOfflineCity",""),
                        new SqlParameter("classOfflinePinCode","0"),
                        new SqlParameter("classOfflineLandMark",""),
                        new SqlParameter("classOfflineState",""),
                        new SqlParameter("classPriceType",""),
                        new SqlParameter("isPaid","0"),
                        new SqlParameter("classOnlineURLLinkPassword",""),
                        new SqlParameter("instructor","0"),
                        new SqlParameter("submittedByLoginId", _LoginId),
                        new SqlParameter("mode", "4"),
                        new SqlParameter("classImage", ""),
                        new SqlParameter("howToBookText", ""),
                        new SqlParameter("groupId", "0"),
                        new SqlParameter("classType", ""),
                        new SqlParameter("classDuration", "0"),
                        new SqlParameter("classLocationLatitude","0"),
                        new SqlParameter("classLocationLongitude","0"),
                        new SqlParameter("classDays_ShortForm",""),
                        new SqlParameter("@classCategoryTypeId","0")

                };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateClass @id,@userLoginId,@name,@classDescription,@startTime24HF,@endTime24HF,@classMode,@studentMaxStrength,@classDays,@price,@onlineClassLink,@classOfflineAddress,@classOfflineCountry,@classOfflineCity,@classOfflinePinCode,@classOfflineLandMark,@classOfflineState,@classPriceType,@isPaid,@classOnlineURLLinkPassword,@instructor,@submittedbyLoginId,@mode,@classImage,@howToBookText,@groupId,@classType,@classDuration,@classLocationLatitude,@classLocationLongitude,@classDays_ShortForm,@classCategoryTypeId", queryParams).FirstOrDefault();


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
        /// Get All Classes For Home Page display
        /// </summary>
        /// <returns>Classes List</returns>
        [HttpGet]
        [Route("api/Class/GetAllClassessForHomePage")]
        public HttpResponseMessage GetAllSponsorsForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                var response = classService.GetClassesListForHomePage();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

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
        /// Get Class Data By ClassId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetById")]
        public HttpResponseMessage GetClassById(long id)
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

                Class_VM class_VM = new Class_VM();

                // Get Class By Id
                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "1")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).FirstOrDefault();

                // Get All Class-Batch List 
                var ClassBatches = classService.GetAllClassBatchesByClass(id);
                class_VM.ClassBatchesIdList = ClassBatches.Select(x => x.Id).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = class_VM;

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
        /// Delete Class By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/DeleteById")]
        public HttpResponseMessage DeleteClassById(long id)
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

                // Delete Class   Information by Id 

                SqlParameter[] queryParams = new SqlParameter[]
                  {
                        new SqlParameter("id", id),
                        new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                        new SqlParameter("name", ""),
                        new SqlParameter("classDescription", ""),
                        new SqlParameter("startTime24HF", ""),
                        new SqlParameter("endTime24HF", ""),
                        new SqlParameter("classMode", ""),
                        new SqlParameter("studentMaxStrength", ""),
                        new SqlParameter("classDays", ""),
                        new SqlParameter("price", "0"),
                        new SqlParameter("onlineClassLink",  ""),
                        new SqlParameter("classOfflineAddress",  ""),
                        new SqlParameter("classOfflineCountry",""),
                        new SqlParameter("classOfflineCity",""),
                        new SqlParameter("classOfflinePinCode","0"),
                        new SqlParameter("classOfflineLandMark",""),
                        new SqlParameter("classOfflineState",""),
                        new SqlParameter("classPriceType",""),
                        new SqlParameter("isPaid","0"),
                        new SqlParameter("classOnlineURLLinkPassword",""),
                        new SqlParameter("instructor","0"),
                        new SqlParameter("submittedByLoginId", _LoginId),
                        new SqlParameter("mode", "3"),
                        new SqlParameter("classImage", ""),
                        new SqlParameter("howToBookText", ""),
                        new SqlParameter("groupId", "0"),
                        new SqlParameter("classType", ""),
                        new SqlParameter("classDuration", "0"),
                        new SqlParameter("classLocationLatitude","0"),
                        new SqlParameter("classLocationLongitude","0"),
                        new SqlParameter("classDays_ShortForm",""),
                        new SqlParameter("classCategoryTypeId","0")

                };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateClass @id,@userLoginId,@name,@classDescription,@startTime24HF,@endTime24HF,@classMode,@studentMaxStrength,@classDays,@price,@onlineClassLink,@classOfflineAddress,@classOfflineCountry,@classOfflineCity,@classOfflinePinCode,@classOfflineLandMark,@classOfflineState,@classPriceType,@isPaid,@classOnlineURLLinkPassword,@instructor,@submittedbyLoginId,@mode,@classImage,@howToBookText,@groupId,@classType,@classDuration,@classLocationLatitude,@classLocationLongitude,@classDays_ShortForm,@classCategoryTypeId", queryParams).FirstOrDefault();

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
        /// Get All Active Classes List For Home Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllList")]
        public HttpResponseMessage GetAllClassesList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<Class_VM> class_VM = new List<Class_VM>();

                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", "0"),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "2")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    ClassList = class_VM,
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

        /// <summary>
        /// Get Class Data By ClassId For Visitor Panel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/GetById/ForVisiotrPanel")]
        public HttpResponseMessage GetClassDataById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                Class_VM class_VM = new Class_VM();

                // Get Class By Id

                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "1")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).FirstOrDefault();
                // Get All Active Class-Batch List 
                var ClassBatches = classService.GetAllActiveClassBatchesByClass(id);
                class_VM.ClassBatchList = ClassBatches.ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = class_VM;

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
        /// Get Class Data With all Batches By ClassId For Business Panel
        /// </summary>
        /// <param name="id">Class Id</param>
        /// <returns>Class Detail with all Class Batch list</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetClassDataWithAllBatchListById")]
        public HttpResponseMessage GetClassDataWithAllBatchListById(long id)
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

                Class_VM class_VM = new Class_VM();

                // Get Class By Id

                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "1")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).FirstOrDefault();
                // Get All Class-Batch List 
                var ClassBatches = classService.GetAllClassBatchesByClass(id);
                class_VM.ClassBatchList = ClassBatches.ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = class_VM;

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
        /// Get All Instructor List linked with the BusinessOnwer
        /// </summary>
        /// <returns>List of All-Business-Instructor</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllInstructor")]
        public HttpResponseMessage GetAllInstructorListByBusiness()
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

                List<InstructorList_VM> lst = new List<InstructorList_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", "0"),
                          new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                          new SqlParameter("mode", "1")
                           };

                var resp = db.Database.SqlQuery<InstructorList_VM>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

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
        /// Get all active distinct Classes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllActiveDistinctClasses")]
        public HttpResponseMessage GetAllActiveDistinctClasses(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<Class_VM> class_VM = new List<Class_VM>();

                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", businessOwnerLoginId),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "4")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    ClassList = class_VM,
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

        /// <summary>
        /// Get Near Expiry Courses/Classes Detail By UserLoginId for Visitor Panel
        /// </summary>
        /// <param name="lastRecordId">Last fetched-Record Id</param>
        /// <param name="recordLimit">No. of items to fetch</param>
        /// <returns>Classes List near expiry</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Class/GetNearExpiryEnrolledCourses")]
        public HttpResponseMessage GetNearExpiryEnrolledCoursesList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<EnrollCourse_VM> response = classService.GetNearExpiryEnrolledCourses(_LoginId, lastRecordId, recordLimit);

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
        /// Get Instructor Class List Detail By Id for Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Class/GetInstructorClassListById")]
        public HttpResponseMessage GetClassListById(long Id, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                List<InstructorClassList_VM> response = classService.GetClassListById(Id, lastRecordId, recordLimit);

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
        /// Get All Business Created Class-Category Types by Class Mode (Online or Offline)
        /// </summary>
        /// <param name="classMode">Online/Offline pass any one</param>
        /// <returns>Class-Category-Types list based on Class-Mode</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetCategoryTypeListByClassMode")]
        public HttpResponseMessage GetCategoryTypeListByClassMode(string classMode)
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

                if (classMode != "Online" && classMode != "Offline")
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Request! Class-Mode value must be 'Online' or 'Offline'!";
                    apiResponse.data = new { };

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                List<ClassCategoryType_VM> responseData = new List<ClassCategoryType_VM>();
                responseData = (classMode == "Online") ? classService.GetAllBusinessOnlineClassesCategoriesTypesList(_BusinessOwnerLoginId) : classService.GetAllBusinessOfflineClassesCategoriesTypesList(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = responseData;

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
        /// Get All Business Created Class-List by Class Mode and other filters(serach, categoryType ... ) (Online or Offline)
        /// if Category-Type-Id passed = 0 then it will list all category type classes of the mode(online/offline) with search queryValue passed.
        /// </summary>
        /// <param name="params_VM">Filter parameters passed for class-list</param>
        /// <returns>Classes list based on Class-Mode and filters</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetClassListByFilter")]
        public HttpResponseMessage GetClassListByClassMode(ClassList_Filter_Params_BOApp_VM params_VM)
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

                if (params_VM.ClassMode != "Online" && params_VM.ClassMode != "Offline")
                {
                    apiResponse.status = -1;
                    apiResponse.message = "Invalid Request! Class-Mode value must be 'Online' or 'Offline'!";
                    apiResponse.data = new { };

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                List<ClassList_Filter_BOApp_VM> responseData = new List<ClassList_Filter_BOApp_VM>();
                responseData = classService.GetAllClassListByFilter_ForBusinessOwner(new ViewModels.StoredProcedureParams.SP_GetClassListByFilter_Params_VM()
                {
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    ClassCategoryTypeId = params_VM.ClassCategoryTypeId,
                    ClassMode = params_VM.ClassMode,
                    SearchValue = params_VM.SearchValue
                });

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = responseData;

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
        /// Get All Active Offline Classes by Business-Owner
        /// </summary>
        /// <returns>Offline Class List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllActiveOfflineClasses")]
        public HttpResponseMessage GetAllActiveOfflineClasses()
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

                // Get All active offline Class list
                var responseData = classService.GetAllActiveOfflineClassesByClassMode_ForBusinessOwner(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = responseData;

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
        /// Get All Active Class-Batches by Class Id
        /// </summary>
        /// <param name="classId">Class-Id</param>
        /// <returns>Class-Batch List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllActiveClassBatchesForDropdown")]
        public HttpResponseMessage GetAllActiveClassBatchesByClass(long classId)
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

                // Get All active offline Class list
                var responseData = classService.GetAllActiveClassBatchesByClassForDropdown(classId, _BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = responseData;

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
        /// To Get Classes Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student")]
        [Route("api/Class/GetAllClassesCategoryList")]
        public HttpResponseMessage GetAllClass(string menuTag, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessAllClassesDetail> allclassdetailList = classService.GetAllClassesDetailByMenuTag(menuTag, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {

                    ClassListDetail = allclassdetailList,
                };

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
        /// To Get Booked Classes Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student")]
        [Route("api/Class/GetAllbookedClassesList")]
        public HttpResponseMessage GetAllbookedClass(long classId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                BusinessSingleClassBookingDetail batchId = new BusinessSingleClassBookingDetail();

                batchId = classService.GetAllClassBatchId(classId);
                BusinessSingleClassBookingDetail businessSingleClassBookingDetail = new BusinessSingleClassBookingDetail();


                businessSingleClassBookingDetail = classService.GetAllClassBookingDetail(classId, batchId.BatchId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    AllbookingClassDetail = businessSingleClassBookingDetail,
                };

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
        /// To Get Classes Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student")]
        [Route("api/Class/GetAllClassesList")]
        public HttpResponseMessage GetAllClass()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessAllClassesDetail> allclassresponse = classService.GetAllClasses_ForBusinessOwner();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    AllClassList = allclassresponse,

                };

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        #endregion -------------------------------------------------------------------------------------------------


        #region Class-Feature APIs --------------------------------------------------------------------------

        /// <summary>
        /// Add or Update Class-Feature for Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/Feature/AddUpdate")]
        public HttpResponseMessage AddUpdateClassFeature()
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
                RequestClassFeature_Params requestClassFeature_VM = new RequestClassFeature_Params();
                requestClassFeature_VM.Id = Convert.ToInt64(HttpRequest.Params["id"]);
                requestClassFeature_VM.ClassId = Convert.ToInt64(HttpRequest.Params["ClassId"]);
                requestClassFeature_VM.Title = HttpRequest.Params["Title"].Trim();
                requestClassFeature_VM.Description = HttpRequest.Params["Description"].Trim();
                requestClassFeature_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _EventSponsorIconFile = files["Icon"];
                requestClassFeature_VM.ClassFeatureIcon = _EventSponsorIconFile; // for validation
                string _ClassFeatureIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousClassFeatureImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestClassFeature_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_EventSponsorIconFile != null)
                    {
                        _ClassFeatureIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_EventSponsorIconFile);
                    }
                }

                // if Edit Mode
                if (requestClassFeature_VM.Mode == 2)
                {
                    // Get Class-Feature detail by Id
                    ClassFeature_VM classFeature = new ClassFeature_VM();
                    classFeature = classService.GetClassFeatureById(requestClassFeature_VM.Id);


                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _PreviousClassFeatureImageFileName = classFeature.Icon;
                    }
                    else
                    {
                        _PreviousClassFeatureImageFileName = classFeature.Icon;
                    }
                }

                // Insert-Update Class Feature Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  requestClassFeature_VM.Id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("classId", requestClassFeature_VM.ClassId),
                            new SqlParameter("title", requestClassFeature_VM.Title),
                            new SqlParameter("description",requestClassFeature_VM.Description),
                            new SqlParameter("icon", _ClassFeatureIconFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestClassFeature_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateClassFeature @id,@userLoginId,@classId,@title,@description,@icon,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Group Image.
                    #region Insert-Update Group Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousClassFeatureImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassFeatureIcon), _PreviousClassFeatureImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ClassFeatureIcon), _ClassFeatureIconFileNameGenerated);
                        fileHelper.SaveUploadedFile(_EventSponsorIconFile, FileWithPath);
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
        /// Get All Class-Feature List by ClassId 
        /// </summary>
        /// <param name="classId">Class-Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/Feature/GetAllList")]
        public HttpResponseMessage GetAllClassFeatureList(long classId)
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
                List<ClassFeature_VM> classFeatureList = new List<ClassFeature_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("classId", classId),
                                new SqlParameter("mode", "1"),
                };
                classFeatureList = db.Database.SqlQuery<ClassFeature_VM>("exec sp_ManageClassFeature @id,@classId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = classFeatureList;

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
        /// Get Class-Feature Data by Id
        /// </summary>
        /// <param name="id">Class-Feature-Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/Feature/GetById")]
        public HttpResponseMessage GetByIdClassFeature(long id)
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
                ClassFeature_VM classFeature = new ClassFeature_VM();
                classFeature = classService.GetClassFeatureById(id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = classFeature;

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
        /// Delete Class-Feature by ClassFeatureId and ClassId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/Feature/DeleteById")]
        public HttpResponseMessage DeleteClassFeatureById(long id, long classId)
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

                // Delete Event Sponsor  Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("classId", classId),
                            new SqlParameter("title", ""),
                            new SqlParameter("description",""),
                            new SqlParameter("icon", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateClassFeature @id,@userLoginId,@classId,@title,@description,@icon,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
        #endregion ----------------------------------------------------------------------------------------------

        #region Batch CRUD APIs --------------------------------------------------------------------

        /// <summary>
        /// Add or Upadate Batch -Business Panel
        /// </summary>
        /// <returns>Api Response with Status and message, +ve if success, -ve if error</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/AddUpdate")]
        public HttpResponseMessage AddUpdateBatch()
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
                RequestBatchViewModel requestBatch_VM = new RequestBatchViewModel();
                requestBatch_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBatch_VM.Name = HttpRequest.Params["batchName"].Trim();
                requestBatch_VM.ScheduledStartOnTime_24HF = HttpRequest.Params["batchStartTime"].Trim();
                requestBatch_VM.ScheduledEndOnTime_24HF = HttpRequest.Params["batchEndTime"].Trim();
                requestBatch_VM.StudentMaxStrength = Convert.ToInt32(HttpRequest.Params["batchLimitofStudent"]);
                requestBatch_VM.InstructorLoginId = Convert.ToInt64(HttpRequest.Params["instructorLoginId"]);
                requestBatch_VM.GroupId = Convert.ToInt64(HttpRequest.Params["groupId"]);
                requestBatch_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);
                requestBatch_VM.Status = Convert.ToInt32(HttpRequest.Params["status"]);

                // Validate infromation passed
                Error_VM error_VM = requestBatch_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Calculate Class Duration in seconds
                DateTime t1 = DateTime.Parse(requestBatch_VM.ScheduledStartOnTime_24HF);
                DateTime t2 = DateTime.Parse(requestBatch_VM.ScheduledEndOnTime_24HF);
                TimeSpan timeSpan = t2 - t1;
                int duration = (int)Math.Round(timeSpan.TotalSeconds);

                // Add or Update Batch
                var resp = classService.AddUpdateBatch(new ViewModels.StoredProcedureParams.SP_InsertUpdateBatch_Params_VM()
                {
                    Id = requestBatch_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    Name = requestBatch_VM.Name,
                    StartTime24HF = requestBatch_VM.ScheduledStartOnTime_24HF,
                    EndTime24HF = requestBatch_VM.ScheduledEndOnTime_24HF,
                    GroupId = requestBatch_VM.GroupId,
                    InstructorLoginId = requestBatch_VM.InstructorLoginId,
                    ClassDuration = duration,
                    StudentMaxStrength = requestBatch_VM.StudentMaxStrength,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestBatch_VM.Mode,
                    Status = requestBatch_VM.Status
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
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
                apiResponse.message = "Internal Server Error!";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Delete Batch By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Api Response, +ve if deleted, otherwise -ve status with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/Delete")]
        public HttpResponseMessage DeleteBatchById(long id)
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

                // Delete Batch Information by Id 
                var resp = classService.AddUpdateBatch(new ViewModels.StoredProcedureParams.SP_InsertUpdateBatch_Params_VM()
                {
                    Id = id,
                    UserLoginId = _BusinessOwnerLoginId,
                    SubmittedByLoginId = _LoginId,
                    Mode = 3
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
        /// Get All Batch with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns>Batch List for Jquery Data-table pagination</returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllBatchesForDataTablePagination()
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

                BatchList_Pagination_SQL_Params_VM _Params_VM = new BatchList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.BusinessOwnerLoginId = _BusinessOwnerLoginId;
                _Params_VM.UserLoginId = _LoginId;

                var paginationResponse = classService.GetBatchList_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// Get Batch Detail By BatchId
        /// </summary>
        /// <param name="id">Batch-Id</param>
        /// <returns>Batch Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/GetById")]
        public HttpResponseMessage GetBatchById(long id)
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

                Batch_VM batch_VM = new Batch_VM();

                // Get Batch By Id
                batch_VM = classService.GetBatchById(id);

                groupService = new GroupService(db);
                var groupMembersCount = groupService.GetGroupMemberTableDataById(batch_VM.GroupId).Count();
                batch_VM.GroupMembersCount = groupMembersCount;

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = batch_VM;

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
        /// Change Batch Batch Status By Id 
        /// </summary>
        /// <param name="id">Batch Id</param>
        /// <returns>Api Response, +ve if changed, otherwise -ve status with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/ChangeStatus")]
        public HttpResponseMessage ChangeBatchStatusById(long id)
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

                // Change Batch Status by Id 
                var resp = classService.ChangeBatchStatus(id, _BusinessOwnerLoginId);

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
        /// Get Active Batch-Names for dropdown which are not linked to any class by Business-Owner-Login-Id
        /// </summary>
        /// <returns>Batch Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/GetActiveNonLinkedBatchList")]
        public HttpResponseMessage GetActiveNonLinkedBatchList()
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

                List<Batch_DropdownList_VM> batch_VM = new List<Batch_DropdownList_VM>();

                // Get Batch By Id
                batch_VM = classService.GetActiveBatchNotLinkedWithClass_DropdownList(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = batch_VM;

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
        /// Get Active Batch-Names for dropdown which are not linked to any other class (excluding the passed class) by Business-Owner-Login-Id
        /// </summary>
        /// <returns>Batch List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Batch/GetActiveNonLinkedWithOtherClassBatchList")]
        public HttpResponseMessage GetActiveNonLinkedWithOtherClassBatchList(long classId)
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

                List<Batch_DropdownList_VM> batch_VM = new List<Batch_DropdownList_VM>();

                // Get Batches List
                batch_VM = classService.GetActiveBatchNotLinkedWithOtherClasses_DropdownList(_BusinessOwnerLoginId, classId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = batch_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion Batch CRUD APIs -----------------------------------------------------------------

        /// <summary>
        /// AddUpdate Business content Classes details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentClasses")]

        public HttpResponseMessage AddUpdateBusinessContentClasses()
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
                long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
                BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentClass_VM businessContentClass_VM = new BusinessContentClass_VM();
                businessContentClass_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentClass_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentClass_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentClass_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentClass_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentClass_Param_VM
                {
                    Id = businessContentClass_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Description = businessContentClass_VM.Description,
                    Title = businessContentClass_VM.Title,
                    Mode = businessContentClass_VM.Mode
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
        /// To Get Business Content Class Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentClassDetail")]
        public HttpResponseMessage GetBusinessContentClassDetails()
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


                BusinessContentClassDetail_VM resp = new BusinessContentClassDetail_VM();

                resp = classService.GetBusinessContentClassDetail(_BusinessOwnerLoginId);



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
        /// Get Class ContentDetails by business id for showing for visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentClassDetailsForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentClassDetailsForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessContentClassDetail_VM resp = new BusinessContentClassDetail_VM();

                resp = classService.GetBusinessContentClassDetail(businessOwnerLoginId);



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
        /// Get All Active Classes List by businessOwnerLoginId  For Home Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllListByBusinessOwnerLoginId")]
        public HttpResponseMessage GetAllListByBusinessOwnerLoginId(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<Class_VM> class_VM = new List<Class_VM>();

                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId",businessOwnerLoginId),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "11")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    ClassList = class_VM,
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

        //To Add Or Edit World Class 
        /// <summary>
        /// To Add World Class Program
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentWorldClasses")]

        public HttpResponseMessage AddUpdateBusinessContentWorldClassDetail()
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
                long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
                BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentWorldClass_VM businessContentWorldClass_VM = new BusinessContentWorldClass_VM();
                businessContentWorldClass_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentWorldClass_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentWorldClass_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentWorldClass_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentWorldClass_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentWorldClass_Param_VM
                {
                    Id = businessContentWorldClass_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = businessContentWorldClass_VM.Title,
                    Description = businessContentWorldClass_VM.Description,
                    Mode = businessContentWorldClass_VM.Mode
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
        /// To Get Business Content World Class Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentWorldClassDetail")]
        public HttpResponseMessage GetBusinessContentWorldClassDetail()
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


                BusinessContentWorldClassDetail_VM resp = new BusinessContentWorldClassDetail_VM();

                resp = classService.GetBusinessContentWorldClassDetail(_BusinessOwnerLoginId);



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


        //To Add The World Classes Detail ////////With Pagination
        ///
        /// <summary>
        /// To Add Business  Content World Classes Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentWorldClassProgram")]

        public HttpResponseMessage AddUpdateBusinessContentWorldClassProfile()
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
                long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
                //BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;
                var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //  BusinesssProfilePageType.Key = "yoga_web";
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessContentWorldClassProgram_VM businessContentWorldClassProgram_VM = new BusinessContentWorldClassProgram_VM();

                // Parse and assign values from HTTP request parameters
                businessContentWorldClassProgram_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentWorldClassProgram_VM.Title = HttpRequest.Params["Title"].Trim();
                businessContentWorldClassProgram_VM.Options = HttpRequest.Params["Options"].Trim();
                businessContentWorldClassProgram_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Validate information passed


                Error_VM error_VM = businessContentWorldClassProgram_VM.ValidInformation();


                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssContentWorldClassImageFile = files["Image"];
                businessContentWorldClassProgram_VM.ImageWorldClass = _BusinesssContentWorldClassImageFile; // for validation
                string _BusinessWorldClassImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessWorldClassImageFileName = ""; // will be used to delete file while updating.



                if (files.Count > 0)
                {
                    if (_BusinesssContentWorldClassImageFile != null)
                    {

                        _BusinessWorldClassImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssContentWorldClassImageFile);
                    }

                }

                if (businessContentWorldClassProgram_VM.Mode == 2)
                {

                    var respGetBusinessContentWorldClassProgramDetail = classService.GetBusinessContentWorldClassProgramDetailById(businessContentWorldClassProgram_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _BusinessWorldClassImageFileNameGenerated = respGetBusinessContentWorldClassProgramDetail.Image ?? "";
                    }
                    else
                    {
                        _PreviousBusinessWorldClassImageFileName = respGetBusinessContentWorldClassProgramDetail.Image ?? "";
                    }
                }




                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentWorldClassProgram_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentWorldClassProgram_Param_VM
                {
                    Id = businessContentWorldClassProgram_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentWorldClassProgram_VM.Title,
                    Options = businessContentWorldClassProgram_VM.Options,
                    Image = _BusinessWorldClassImageFileNameGenerated,
                    Mode = businessContentWorldClassProgram_VM.Mode
                });




                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Class Image.
                    #region Insert-Update Business Content  Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessWorldClassImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessWorldClassProgramImage), _PreviousBusinessWorldClassImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessWorldClassProgramImage), _BusinessWorldClassImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssContentWorldClassImageFile, NewImageFileWithPath);


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


        /// <summary>
        /// Get All Business Content World Class with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBusinessContentWorldClassProgramDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentWorldClassDataTablePagination()
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

                BusinessContentWorldClassProgramDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentWorldClassProgramDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = classService.GetBusinessContentWorldClassProgramList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Business Content World Class  Program Detail By Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentWorldClassProgramDetail/ById/{id}")]
        public HttpResponseMessage GetBusinessContentWorldClassProgramDetail(long Id)
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


                BusinessContentWorldClassProgramDetail_VM resp = new BusinessContentWorldClassProgramDetail_VM();

                resp = classService.GetBusinessContentWorldClassProgramDetailById(Id);



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
        /// Delete World Class Program Detail
        /// </summary>
        /// <param name="id">WorldClassProgram Id</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/Business/DeleteBusinessContentWorldClassProgramDetail")]
        public HttpResponseMessage DeleteWorldClassProgramById(long id)
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

                SPResponseViewModel resp = new SPResponseViewModel();

                // Delete World Class Program Detail 
                resp = classService.DeleteWorldClassProgramDetail(id);


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

        ////////////////////////////////////////////////////////World class Detail Show////////////////////////////////////////

        /// <summary>
        /// To Get Business Content World Class Detail (List)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentWorldClassProgramDetail_PPCMeta")]
        public HttpResponseMessage GetBusinessContentWorldClassDetailProfile(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                BusinessContentWorldClassDetail_VM resp = new BusinessContentWorldClassDetail_VM();

                resp = classService.GetBusinessContentWorldClassDetail(businessOwnerLoginId);

                List<BusinessContentWorldClassProgramDetail_VM> businessWorldClassProgramlst = classService.GetBusinessContentWorldClassProgramDetaillst_Get(businessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentWorldClassDetail = resp,
                    BusiessContentWorldClassProgramlstDetail = businessWorldClassProgramlst
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

        // Course Detail //
        /// <summary>
        /// To Get Course Detail By BusinessOwnerLoginId
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentCourseDetails_ForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentClassDetail(long classId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessSingleClassBookingDetail businessContentdetail = classService.GetBusinessContentClassDetail_Get(classId);

                BusinessSingleClassBookingDetail batchId = classService.GetAllClassBatchId(classId);
                BusinessSingleClassBookingDetail businessSingleClassBookingDetail = new BusinessSingleClassBookingDetail();
                List<InstructorList_VM> batchInstructorDetail = new List<InstructorList_VM>();

                if (batchId != null)
                {
                    businessSingleClassBookingDetail = classService.GetAllClassBookingDetail(classId, batchId.BatchId);
                    batchInstructorDetail = classService.GetBusinessContentClassBatchInstructorDetail_Get(classId);
                }


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    AllbookingClassDetail = businessSingleClassBookingDetail,
                    BatchInstructorDetail = batchInstructorDetail

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

        //////////////////////////////////////////////////////// class Detail ////////////////////////////////////////

        /// <summary>
        /// To Get Business Content  Class Detail (List)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentClassDetail_ForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentClassDetailProfile(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessContentClassesDetail_VM> businessUpcomingClasslst = classService.GetBusinessContentUpcomingClassDetaillst_Get(businessOwnerLoginId);


                List<BusinessContentClassesDetail_VM> businessOnlineClasslst = classService.GetBusinessContentOnlineClassDetaillst_Get(businessOwnerLoginId);

                List<BusinessContentClassesDetail_VM> businessOfflineClasslst = classService.GetBusinessContentOfflineClassDetaillst_Get(businessOwnerLoginId);

                List<BusinessContentClassesDetail_VM> businessAllClasslst = classService.GetBusinessContentAllClassDetaillst_Get(businessOwnerLoginId);



                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    //BusinessUpcomingClassDetaillst = businessUpcomingClasslst,
                    BusinessUpcomingClassDetaillst = businessAllClasslst,
                    BusinessOnlineClassDetaillst = businessOnlineClasslst,
                    BusinessOfflineClassDetaillst = businessOfflineClasslst,
                    BusinessAllClassDetaillst = businessAllClasslst
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


        /// <summary>
        /// Get All ClassList For visitor panel with View-More Pagination Functionality
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student,BusinessAdmin,Staff,SubAdmin,SuperAdmin")]
        [Route("api/Business/GetAllClassListLastRecorded")]
        public HttpResponseMessage GetAllClassListByPagination(long businessOwnerLoginId, long lastRecordId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                var _UserCurrentDateTime = DateTime.Now; // IST 
                //var _UserCurrentTimezoneOffset = "";

                List<Class_VM> lst = new List<Class_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                             new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", businessOwnerLoginId),
                                new SqlParameter("recordLimit", "4"),
                                new SqlParameter("lastRecordId", lastRecordId),
                                new SqlParameter("mode", "1")
                };

                lst = db.Database.SqlQuery<Class_VM>("exec sp_GetAllClassViewPagination @id, @userLoginId, @recordLimit, @lastRecordId, @mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    ClassList = lst,
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


        /// <summary>
        /// Get All ClassList offline For visitor panel with View-More Pagination Functionality
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student,BusinessAdmin,Staff,SubAdmin,SuperAdmin")]
        [Route("api/Business/GetAllClassOfflineListLastRecorded")]
        public HttpResponseMessage GetAllClassOfflineListByPagination(long businessOwnerLoginId, long lastRecordId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                var _UserCurrentDateTime = DateTime.Now;// IST
                //var _UserCurrentTimezoneOffset = "";      

                List<BusinessContentClassesDetail_VM> lst = new List<BusinessContentClassesDetail_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                             new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", businessOwnerLoginId),
                                new SqlParameter("recordLimit", "0"),
                                new SqlParameter("lastRecordId", lastRecordId),
                                new SqlParameter("mode", "2")
                };

                lst = db.Database.SqlQuery<BusinessContentClassesDetail_VM>("exec sp_GetAllClassViewPagination @id, @userLoginId, @recordLimit, @lastRecordId, @mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    OfflineClassList = lst,
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

        /// <summary>
        /// Get All Other Classes List
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student")]
        [Route("api/Class/GetAllOtherClassesList")]
        public HttpResponseMessage GetAllClassList(long classId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessAllClassesDetail> allclassdetailList = classService.GetOtherClassDetailList(classId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {

                    ClassListDetail = allclassdetailList,
                };

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
        /// Get Instructor Profile Classes Detail
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student")]
        [Route("api/Class/GetInstructorProfileClassesDetail")]
        public HttpResponseMessage GetInstructorClassList(long classId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessAllClassesDetail resp = classService.GetInstructorClassDetailList(classId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {

                    ClassListDetail = resp,
                };

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
        /// Get All Active/Inactive Classes List For Booking
        /// </summary>
        /// <returns>All Class List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetAllActiveInactive")]
        public HttpResponseMessage GetAllActiveInactiveClassesList()
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

                List<Class_VM> class_VM = new List<Class_VM>();

                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "17")
                            };

                class_VM = db.Database.SqlQuery<Class_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = class_VM;

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
        /// To Get Classes Timing Detail 
        /// </summary>
        /// <param name="classDays"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/GetClasesTimingDetail")]
        public HttpResponseMessage GetClasesTimingDetail(string classDays, long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinssClassesTimingDetail> resp = classService.GetClassTimingDetailList(classDays, businessOwnerLoginId);


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


        /// <summary>
        /// To Get Classes List by Search-Filter - VISITOR PANEL 
        /// </summary>
        /// <returns>Filtered Classes List</returns>
        [HttpPost]
        [Route("api/Class/GetAllClassesBySearchFilter")]
        public HttpResponseMessage GetAllClassesBySearchFilter(SearchFilter_APIParmas_VM filterParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<ClassDetail_BySearchFilter_VP> allclassdetailList = classService.GetClassesListBySearchFilter(filterParams);

                foreach (var classdetail in allclassdetailList)
                {
                    DateTime nextClassDate = ClassService.GetNextClassDateFromClassDays(classdetail.ClassDays);

                    classdetail.NextClassDate = nextClassDate.ToString();
                    classdetail.NextClassDay = nextClassDate.Day;
                    classdetail.NextClassMonth = nextClassDate.ToString("MMMM");
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    ClassListDetail = allclassdetailList,
                };

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
        /// To Get Class Category Type Detail List 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessAllClassCategoryList")]
        public HttpResponseMessage GetBusinessAllClassCategoryList(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<ClassCategoryDetailList_VM> resp = classService.GetBusinessClassCategoryDetail_Get(businessOwnerLoginId);


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
        /// To get All Classes detail using for time table in visitor panel (filter )
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="classCategoryTypeId"></param>
        /// <param name="InstructorLoginId"></param>
        /// <param name="classDays"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/GetAllClasesTimeTableDetail")]
        public HttpResponseMessage GetAllClasesDetail(long businessOwnerLoginId, long classCategoryTypeId, long InstructorLoginId, string classDays,string classMode)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinssClassesTimingDetail> resp = classService.GetallClassDetailList(businessOwnerLoginId, classCategoryTypeId, InstructorLoginId, classDays,classMode);


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

        /// <summary>
        ///  Get Batch Details  by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Enquiry/GetBatchDetailsById")]
        public HttpResponseMessage GetBatchDetailsById(long id , string searchtext = "")
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
                searchtext = string.IsNullOrEmpty(searchtext) ? "" : searchtext;
                BatchInfo_VM batchResponse = classService.GetBatchDetailsById(_BusinessOwnerLoginId, id, searchtext); // Pass _BusinessOwnerLoginId as the first parameter
              //  List<Class_VM> ClassDetails = classService.GetclassDetailsById(_BusinessOwnerLoginId, id, searchtext); // Pass _BusinessOwnerLoginId as the first parameter

                if (batchResponse != null && !string.IsNullOrEmpty(batchResponse.BatchDetailJson))
                {
                    // Deserialize the JSON string into a list of BatchInfo_VM objects
                    batchResponse.BatchDetails = JsonSerializer.Deserialize<List<Batches_VM>>(batchResponse.BatchDetailJson);
                    batchResponse.BatchDetailJson = null;
                    batchResponse.BatchDetails?.ForEach(item =>
                    {
                        if (!string.IsNullOrEmpty(item.StudentImageWithPathJson))
                        {
                            item.StudentImageWithPath = item.StudentImageWithPathJson.Split('*');
                            item.StudentImageWithPathJson = null;
                        }
                    });

                }


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = batchResponse;

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
        /// To Get Class Refer Detail By UserLoginId for Visitor Panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetClassReferDetail_PPCMeta")]
        public HttpResponseMessage GetClassReferDetail_PPCMeta()
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

                ClassReferDetail_VM resp = new ClassReferDetail_VM();
                resp = classService.GetClassReferDetails_Get(_BusinessOwnerLoginId); // Pass _BusinessOwnerLoginId as the first parameter

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
        /// To Add.Update Class Refer Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/AddUpdateClassReferDetail_PPCMeta")]

        public HttpResponseMessage AddUpdateClassReferDetail_PPCMeta()
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
                long BusinesssProfilePageTypeId = 0;

                // --Get User - Login Detail
                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();
                BusinesssProfilePageTypeId = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId).Id;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                ClassReferDetailViewModel classReferDetailViewModel = new ClassReferDetailViewModel();
                classReferDetailViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);

                // Check and set Title 

                string TitleParam = HttpRequest.Params["Title"];
                classReferDetailViewModel.Title = !string.IsNullOrEmpty(TitleParam) ? TitleParam.Trim() : string.Empty;

                // Check and set Description
                string descriptionParam = HttpRequest.Params["Description"];
                classReferDetailViewModel.Description = !string.IsNullOrEmpty(descriptionParam) ? descriptionParam.Trim() : string.Empty;

                classReferDetailViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Get Attatched Files

                // Validate infromation passed
                Error_VM error_VM = classReferDetailViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }



                var resp = storedProcedureRepository.SP_InsertUpdateClassReferDetail<SPResponseViewModel>(new SP_InsertUpdateClassReferDetail_Param_VM
                {
                    Id = classReferDetailViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Title = classReferDetailViewModel.Title,
                    Description = classReferDetailViewModel.Description,
                    SubmittedByLoginId = _BusinessOwnerLoginId,
                    Mode = classReferDetailViewModel.Mode
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
        /// To Get Class Refer Detail For Visitor Panel 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/GetClassReferDetail_PPCMetaForVisitorPanel")]
        public HttpResponseMessage GetClassReferDetail_PPCMetaForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                ClassReferDetail_VM resp = new ClassReferDetail_VM();
                resp = classService.GetClassReferDetails_Get(businessOwnerLoginId); // Pass _BusinessOwnerLoginId as the first parameter

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
        /// To Get class Booking  Detail 
        /// </summary>
        /// <param name="classDays"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Class/GetClassesBookingDetail")]
        public HttpResponseMessage GetClassesBookingDetail()
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
                List<ClassBooking_ViewModel> resp = classService.GetClassBookingDetailList(_LoginId);


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


        /// <summary>
        /// get classes , order, payment details by id for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Class/GetClassesBookingDetailById")]
        public HttpResponseMessage GetClassesBookingDetailById(long id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Class-Booking-Detail
                var classBookingDetail = classService.GetClassBookingDetailById(id, _LoginID_Exact);

                //// Get Class-Detail  
                //ClassBooking_ViewModel classbookingDetail = new ClassBooking_ViewModel();
                //classbookingDetail = classService.GetClassBookingDetailById(classBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(classBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    // ClassDetail = classbookingDetail,
                    ClassBookingDetail = classBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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


        /// <summary>
        /// To Get Business Class Booking Detail By UserLoginId & Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetBusinessClassesBookingDetailById")]
        public HttpResponseMessage GetBusinessClassesBookingDetailById(long id)
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

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Class-Booking-Detail
                var classBookingDetail = classService.GetBusinessClassBookingDetailById(id, _LoginID_Exact);

                // Get Class-Detail  
                ClassBooking_ViewModel classbookingDetail = new ClassBooking_ViewModel();
                classbookingDetail = classService.GetBusinessClassBookingDetail(classBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(classBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    ClassDetail = classbookingDetail,
                    ClassBookingDetail = classBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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


        /// <summary>
        /// To Get Business Class Booking Detail For Business Panel (My Booking)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetBusinessClassesBookingDetail")]
        public HttpResponseMessage GetBusinessClassesBookingDetail()
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
                List<ClassBooking_ViewModel> resp = classService.GetBusinessClassBookingDetailList(_LoginId);


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

        /// <summary>
        /// To Get Business Class Detail List For Visitor-Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/GetClassesDetailList")]
        public HttpResponseMessage GetClassesDetailList(long businessOwnerLoginId ,long id,string dayname)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                ClassDetail_VM resp = new ClassDetail_VM();

                resp = classService.GetSingleClassDetail(businessOwnerLoginId);

                List<ClassDetail_VM> classdetailsList = classService.GetClassDetailList(businessOwnerLoginId);
                List<ClassDetail_VM> classdetailListByInstructor = classService.GetClassListByInstructor(businessOwnerLoginId,id);
                List<ClassdetailsbyInstructor_VM> classesList = classService.GetClassListbyInstuctor(businessOwnerLoginId,id, dayname);
                var groupedData = classesList.GroupBy(s => new { s.CurrentDate})
                              .Select(g => new
                              {
                                  CurrentDate = g.Key.CurrentDate,
                                  StaffMembers = g.ToList()
                              })
                              .ToList();
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    ClassDetailList = classdetailsList,
                    ClassesListInstructor = classesList,
                    ClasseslistByInstructor = classdetailListByInstructor,
                    ClassDetail = resp,
                    GroupByDetail = groupedData,
                };



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
        /// Get All Class-Feature List by ClassId 
        /// </summary>
        /// <param name="classId">Class-Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/Feature/GetAllClassFeatureDetailList")]
        public HttpResponseMessage GetAllClassFeatureDetailList(long businessOwnerLoginId, long classId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<ClassFeature_VM> classFeatureList = new List<ClassFeature_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                          new SqlParameter("id", classId),
                          new SqlParameter("classId", businessOwnerLoginId),
                          new SqlParameter("mode", "3"),
                };
                classFeatureList = db.Database.SqlQuery<ClassFeature_VM>("exec sp_ManageClassFeature @id,@classId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = classFeatureList;

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
        ///  Get certificate  Details  by Id  for manage staff (intructor) 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Class/GetCertificateDetailsById")]
        public HttpResponseMessage GetCertificateDetailsById(long id,string searchtext)
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
                searchtext = string.IsNullOrEmpty(searchtext) ? "" : searchtext;
                InstructorInfo_VM certificateResponse = classService.GetcertificateDetailsById(_BusinessOwnerLoginId, id, searchtext); // Pass _BusinessOwnerLoginId as the first parameter

                if (certificateResponse != null && !string.IsNullOrEmpty(certificateResponse.CertificateJson))
                {
                    // Deserialize the JSON string into a list of BatchInfo_VM objects
                    certificateResponse.CertificateDetails = JsonSerializer.Deserialize<List<CertificateDetailsViewModel>>(certificateResponse.CertificateJson);
                    certificateResponse.CertificateJson = null;

                }
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = certificateResponse;

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
        /// To Get Business Class Intermediate Detail List For Visitor-Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Class/GetClassIntermediateList")]
        public HttpResponseMessage GetClassIntermediateList(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                ClassIntermediatesDetail_VM resp = new ClassIntermediatesDetail_VM();

                resp = classService.GetClassIntermediateDetail(businessOwnerLoginId);
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    ClassIntermediateDetail = resp,
                };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

    }
}