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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class NotificationAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private NotificationService notificationService;

        public NotificationAPIController()
        {
            db = new MasterZoneDbContext();
            notificationService = new NotificationService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Created Notifications with Pagination
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Notification/Created/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllCreatedNotificationsByPagination()
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
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                NotificationRecordList_Pagination_SQL_Params_VM _Params_VM = new NotificationRecordList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;

                var paginationResponse = notificationService.GetNotificationsList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Created Notification Detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Notification/GetById/{id}")]
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

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Notification-Record-Detail 
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("notificationRecordId", id),
                            new SqlParameter("fromUserLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

                var responseNotificationRecord = db.Database.SqlQuery<NotificationRecord_Detail_VM>("exec sp_ManageNotification @id,@notificationRecordId,@fromUserLoginId,@toUserLoginId,@mode", queryParams).FirstOrDefault();

                if (responseNotificationRecord != null)
                {
                    SqlParameter[] queryParams_NotificationUsers = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("notificationRecordId", id),
                            new SqlParameter("fromUserLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                    var NotificationUsers = db.Database.SqlQuery<Notification>("exec sp_ManageNotification @id,@notificationRecordId,@fromUserLoginId,@toUserLoginId,@mode", queryParams_NotificationUsers).ToList();
                    responseNotificationRecord.NotificationUserIdList = NotificationUsers.Select(n => n.UserLoginId.ToString()).ToList();
                }

                if (responseNotificationRecord == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.NotificationRecordNotFound_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = responseNotificationRecord;
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
        /// Create/Update notification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Notification/AddUpdate")]
        public HttpResponseMessage AddUpdateNotification()
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
                long _BusinessOwnerLoginID = validateResponse.BusinessAdminLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestNotificationRecord_VM requestNotificationRecord_VM = new RequestNotificationRecord_VM();
                requestNotificationRecord_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestNotificationRecord_VM.NotificationTitle = HttpRequest.Params["NotificationTitle"].Trim();
                requestNotificationRecord_VM.NotificationText = HttpRequest.Params["NotificationText"].Trim();
                requestNotificationRecord_VM.NotificationUsersList = HttpRequest.Params["NotificationUsersList"].Trim();
                requestNotificationRecord_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = requestNotificationRecord_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPInsertUpdateNotification_Params_VM sp_IU_Notification_VM = new SPInsertUpdateNotification_Params_VM()
                {
                    Id = requestNotificationRecord_VM.Id,
                    NotificationTitle = requestNotificationRecord_VM.NotificationTitle,
                    NotificationText = requestNotificationRecord_VM.NotificationText,
                    NotificationUsersList = requestNotificationRecord_VM.NotificationUsersList,
                    Mode = requestNotificationRecord_VM.Mode,
                    SubmittedByLoginId = _LoginId,
                    FromUserLoginId = _BusinessOwnerLoginID,
                    NotificationType = "CustomNotification"
                };
                var resp = notificationService.InsertUpdateNotification(sp_IU_Notification_VM);

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
        /// Delete Notification created by the Admin-Panel-Users(Busienss, staff)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Notification/Delete/{id}")]
        public HttpResponseMessage DeleteNotificationById(int id)
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
                long _BusinessOwnerLoginID = validateResponse.BusinessAdminLoginId;

                SPInsertUpdateNotification_Params_VM sp_IU_Notification_VM = new SPInsertUpdateNotification_Params_VM()
                {
                    Id = id,
                    SubmittedByLoginId = _LoginId,
                    FromUserLoginId = _BusinessOwnerLoginID,
                    Mode = 3
                };
                SPResponseViewModel resp = notificationService.InsertUpdateNotification(sp_IU_Notification_VM);
                //var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageBusinessCategory @id,@parentBusinessCategoryId,@mode", queryParams).FirstOrDefault();

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

        /// <summary>
        /// Get All Notifications with Pagination For the Logged-In-User [Used in Business-Notification-Datatable]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff,Student")]
        [Route("api/Notification/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllNotificationsByDataTablePaginationForLoggedInUser()
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
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                NotificationRecordList_Pagination_SQL_Params_VM _Params_VM = new NotificationRecordList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 2;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.ToUserLoginId = _LoginId;

                var paginationResponse = notificationService.GetNotificationsList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get All Notifications For the Logged-In-User with View-More Pagination Functionality
        /// </summary>
        /// <param name="lastRecordId">Last Record Fetched Id.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student,BusinessAdmin,Staff,SubAdmin,SuperAdmin")]
        [Route("api/Notification/GetAllNotification")]
        public HttpResponseMessage GetAllNotificationByPagination(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<UserNotification_Pagination_VM> lst = new List<UserNotification_Pagination_VM>();
                Notification notification = new Notification();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", ""),
                            new SqlParameter("toUserLoginId", _LoginId),
                            new SqlParameter("lastRecordId", lastRecordId),
                            new SqlParameter("recordLimit", recordLimit),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "1")
                            };

                lst = db.Database.SqlQuery<UserNotification_Pagination_VM>("exec sp_GetNotifications_Pagination @id,@toUserLoginId,@lastRecordId,@recordLimit,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    NotificationList = lst,
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
        /// Make Notification Read/Unread based on parameters passed.
        /// </summary>
        /// <param name="requestNotificationReadUnread_VM">{ NotificationId : ID, ReadStatus: (1 for read and 0 for unread)}</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Notification/MarkAsReadUnread")]
        public HttpResponseMessage ChangeStatusMarkAsRead(RequestNotificationReadUnread_VM requestNotificationReadUnread_VM)
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

                int _Mode = 0;
                if (requestNotificationReadUnread_VM.ReadStatus == 1)
                {
                    _Mode = 3; // Mark as Read
                }
                else
                {
                    _Mode = 4; // Mark as Unread
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                          new SqlParameter("id", requestNotificationReadUnread_VM.NotificationId),
                            new SqlParameter("notificationRecordId", "0"),
                            new SqlParameter("fromUserLoginId", "0"),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("mode", _Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageNotification @id,@notificationRecordId,@fromUserLoginId,@toUserLoginId,@mode", queryParams).FirstOrDefault();

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
    }
}