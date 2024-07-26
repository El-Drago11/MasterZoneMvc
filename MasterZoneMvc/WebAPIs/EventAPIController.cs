using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class EventAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private EventService eventService;
        private BusinessOwnerService businessOwnerService;
        private StoredProcedureRepository storedProcedureRepository;

        public EventAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            eventService = new EventService(db);
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

        /// <summary>
        /// Add or Upadate Event -Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/AddUpdate")]
        public HttpResponseMessage AddUpdateEvent()
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
                AddUpdateEvent_VM requestEvent_VM = new AddUpdateEvent_VM();
                requestEvent_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                //requestEvent_VM.UserLoginId = _LoginId;
                requestEvent_VM.Title = HttpRequest.Params["title"].Trim();
                requestEvent_VM.StartDate = HttpRequest.Params["startDate"].Trim();
                requestEvent_VM.EndDate = HttpRequest.Params["endDate"].Trim();
                requestEvent_VM.StartTime_24HF = HttpRequest.Params["startTime"].Trim();
                requestEvent_VM.EndTime_24HF = HttpRequest.Params["endTime"];
                requestEvent_VM.IsPaid = Convert.ToInt32(HttpRequest.Params["isPaid"]);
                requestEvent_VM.Walkings = Convert.ToInt32(HttpRequest.Params["walking"]);
                requestEvent_VM.EventLocationURL = HttpRequest.Params["locationURL"];
                requestEvent_VM.ShortDescription = HttpRequest.Params["shortDescription"];
                requestEvent_VM.AdditionalInformation = HttpUtility.UrlDecode(HttpRequest.Form.Get("additionalInforamtion"));
                requestEvent_VM.TicketInformation = HttpUtility.UrlDecode(HttpRequest.Form.Get("ticketInforamtion"));
                requestEvent_VM.AboutEvent = HttpUtility.UrlDecode(HttpRequest.Form.Get("about"));
                requestEvent_VM.Price = Convert.ToDecimal(HttpRequest.Params["price"]);
                requestEvent_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);
                requestEvent_VM.LandMark = HttpRequest.Params["LandMark"].Trim();
                requestEvent_VM.Address = HttpRequest.Params["Address"].Trim();
                requestEvent_VM.City = HttpRequest.Params["City"].Trim();
                requestEvent_VM.State = HttpRequest.Params["State"].Trim();
                requestEvent_VM.Country = HttpRequest.Params["Country"].Trim();
                requestEvent_VM.PinCode = HttpRequest.Params["PinCode"].Trim();
                requestEvent_VM.Latitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLatitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLatitude"]) : 0;
                requestEvent_VM.Longitude = (!String.IsNullOrEmpty(HttpRequest.Params["LocationLongitude"])) ? Convert.ToDecimal(HttpRequest.Params["LocationLongitude"]) : 0;
                requestEvent_VM.EventCategoryId = Convert.ToInt32(HttpRequest.Params["EventCategoryId"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _EventImageFile = files["featuredImage"];
                requestEvent_VM.FeaturedImage = _EventImageFile; // for validation
                string _EventImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousEventImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestEvent_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_EventImageFile != null)
                    {
                        _EventImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_EventImageFile);
                    }
                }

                // if Edit Mode
                if (requestEvent_VM.Mode == 2)
                {
                    
                    Event_VM event_VM = new Event_VM();

                    // Get Event By Id
                    event_VM = eventService.GetEventById(requestEvent_VM.Id);
                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _EventImageFileNameGenerated = event_VM.FeaturedImage;
                    }
                    else
                    {
                        _PreviousEventImageFileName = event_VM.FeaturedImage;
                    }
                }

                // Insert-Update Event Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestEvent_VM.Id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("title", requestEvent_VM.Title),
                            new SqlParameter("startDate", requestEvent_VM.StartDate),
                            new SqlParameter("endDate", requestEvent_VM.EndDate),
                            new SqlParameter("startTime24HF",requestEvent_VM.StartTime_24HF),
                            new SqlParameter("endTime24HF",requestEvent_VM.EndTime_24HF),
                            new SqlParameter("isPaid",requestEvent_VM.IsPaid),
                            new SqlParameter("walkings",requestEvent_VM.Walkings),
                            new SqlParameter("eventLocationURL",requestEvent_VM.EventLocationURL),
                            new SqlParameter("shortDescription",requestEvent_VM.ShortDescription),
                            new SqlParameter("additionalInforamtion",requestEvent_VM.AdditionalInformation),
                            new SqlParameter("ticketInforamtion",requestEvent_VM.TicketInformation),
                            new SqlParameter("about",requestEvent_VM.AboutEvent),
                            new SqlParameter("eventImage", _EventImageFileNameGenerated),
                            new SqlParameter("price",requestEvent_VM.Price),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestEvent_VM.Mode),
                            new SqlParameter("address", requestEvent_VM.Address),
                            new SqlParameter("country", requestEvent_VM.Country),
                            new SqlParameter("state", requestEvent_VM.State),
                            new SqlParameter("city", requestEvent_VM.City),
                            new SqlParameter("pinCode",requestEvent_VM.PinCode),
                            new SqlParameter("landMark",requestEvent_VM.LandMark),
                            new SqlParameter("latitude",requestEvent_VM.Latitude),
                            new SqlParameter("longitude",requestEvent_VM.Longitude),
                            new SqlParameter("eventCategoryId",requestEvent_VM.EventCategoryId),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEvent @id,@userLoginId,@title,@startDate,@endDate,@startTime24HF,@endTime24HF,@isPaid,@walkings,@eventLocationURL,@shortDescription,@additionalInforamtion,@ticketInforamtion,@about,@eventImage,@price,@submittedbyLoginId,@mode,@address,@country,@state,@city,@pinCode,@landMark,@latitude,@longitude,@eventCategoryId", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Event Image.
                    #region Insert-Update Event Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousEventImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EventImage), _PreviousEventImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EventImage), _EventImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_EventImageFile, FileWithPath);
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
                apiResponse.message = "Internal Server Error!";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get All Event with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllEventForDataTablePagination()
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

                EventList_Pagination_SQL_Params_VM _Params_VM = new EventList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.UserLoginId = _BusinessOwnerLoginId;
                _Params_VM.CreatedByLoginId = _LoginId;

                var paginationResponse = eventService.GetEventList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Event Data By EventId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetById")]
        public HttpResponseMessage GetEventById(long id)
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
                Event_VM event_VM = new Event_VM();

                // Get Event By Id
                event_VM = eventService.GetEventById(id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = event_VM;

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
        /// Delete Event By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/DeleteById")]
        public HttpResponseMessage DeleteEventById(long id)
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

                // Delete Event   Information by Id 

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("title", ""),
                            new SqlParameter("startDate", ""),
                            new SqlParameter("endDate", ""),
                            new SqlParameter("startTime24HF",""),
                            new SqlParameter("endTime24HF",""),
                            new SqlParameter("isPaid","0"),
                            new SqlParameter("walkings","0"),
                            new SqlParameter("eventLocationURL",""),
                            new SqlParameter("shortDescription",""),
                            new SqlParameter("additionalInforamtion",""),
                            new SqlParameter("ticketInforamtion",""),
                            new SqlParameter("about",""),
                            new SqlParameter("eventImage",""),
                            new SqlParameter("price","0"),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("address", ""),
                            new SqlParameter("country", ""),
                            new SqlParameter("state", ""),
                            new SqlParameter("city", ""),
                            new SqlParameter("pinCode",""),
                            new SqlParameter("landMark",""),
                            new SqlParameter("latitude","0"),
                            new SqlParameter("longitude","0"),
                            new SqlParameter("eventCategoryId","0"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEvent @id,@userLoginId,@title,@startDate,@endDate,@startTime24HF,@endTime24HF,@isPaid,@walkings,@eventLocationURL,@shortDescription,@additionalInforamtion,@ticketInforamtion,@about,@eventImage,@price,@submittedbyLoginId,@mode,@address,@country,@state,@city,@pinCode,@landMark,@latitude,@longitude,@eventCategoryId", queryParams).FirstOrDefault();

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
        /// Get All Events with Pagination For the Super-Admin-Panel [Super, SubAdmin]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns>All Classes created by business owners list with pagination</returns>
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Event/GetAllEventsByPaginationForSuperAdmin")]
        [HttpPost]
        public HttpResponseMessage GetAllEventsByPaginationForSuperAdmin()
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

                EventList_Pagination_SQL_Params_VM _Params_VM = new EventList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 2;

                var paginationResponse = eventService.GetAllEventList_Pagination_ForSuperAdminPanel(HttpRequestParams, _Params_VM);

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
        /// To change the Event Home Visibility Status - [Super-Admin-Panel]
        /// </summary>
        /// <param name="id">Event-Id</param>
        /// <returns>Success or Error response</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/Event/ToggleHomePageVisibilityStatus/{id}")]
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("title", ""),
                            new SqlParameter("startDate", ""),
                            new SqlParameter("endDate", ""),
                            new SqlParameter("startTime24HF",""),
                            new SqlParameter("endTime24HF",""),
                            new SqlParameter("isPaid","0"),
                            new SqlParameter("walkings","0"),
                            new SqlParameter("eventLocationURL",""),
                            new SqlParameter("shortDescription",""),
                            new SqlParameter("additionalInforamtion",""),
                            new SqlParameter("ticketInforamtion",""),
                            new SqlParameter("about",""),
                            new SqlParameter("eventImage",""),
                            new SqlParameter("price","0"),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "4"),
                            new SqlParameter("address", ""),
                            new SqlParameter("country", ""),
                            new SqlParameter("state", ""),
                            new SqlParameter("city", ""),
                            new SqlParameter("pinCode",""),
                            new SqlParameter("landMark",""),
                            new SqlParameter("latitude","0"),
                            new SqlParameter("longitude","0"),
                            new SqlParameter("eventCategoryId","0"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEvent @id,@userLoginId,@title,@startDate,@endDate,@startTime24HF,@endTime24HF,@isPaid,@walkings,@eventLocationURL,@shortDescription,@additionalInforamtion,@ticketInforamtion,@about,@eventImage,@price,@submittedbyLoginId,@mode,@address,@country,@state,@city,@pinCode,@landMark,@latitude,@longitude,@eventCategoryId", queryParams).FirstOrDefault();


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
        /// Add or Update Event-Sponsor for Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Sponsor/AddUpdate")]
        public HttpResponseMessage AddUpdateEventSponsor()
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
                AddUpdateEventSponsor_Params requestEventSponsor_VM = new AddUpdateEventSponsor_Params();
                requestEventSponsor_VM.Id = Convert.ToInt64(HttpRequest.Params["id"]);
                requestEventSponsor_VM.EventId = Convert.ToInt64(HttpRequest.Params["EventId"]);
                requestEventSponsor_VM.SponsorTitle = HttpRequest.Params["Title"].Trim();
                requestEventSponsor_VM.SponsorLink = HttpRequest.Params["Link"].Trim();
                requestEventSponsor_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _EventSponsorIconFile = files["Icon"];
                requestEventSponsor_VM.SponsorIcon = _EventSponsorIconFile; // for validation
                string _EventSponsorIconFileNameGenerated = ""; //will contains generated file name
                string _PreviousEventSponsorImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestEventSponsor_VM.ValidInformation();

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
                        _EventSponsorIconFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_EventSponsorIconFile);
                    }
                }

                // if Edit Mode
                if (requestEventSponsor_VM.Mode == 2)
                {
                    EventSponsor_VM eventSponsor = new EventSponsor_VM();

                    SqlParameter[] queryParams_VM = new SqlParameter[] {
                                new SqlParameter("id", requestEventSponsor_VM.Id),
                                new SqlParameter("eventId",  requestEventSponsor_VM.EventId),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("mode", "2"),
                 };
                    eventSponsor = db.Database.SqlQuery<EventSponsor_VM>("exec sp_ManageEventSponsor @id,@eventId,@userLoginId,@mode", queryParams_VM).FirstOrDefault();


                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _PreviousEventSponsorImageFileName = eventSponsor.SponsorIcon;
                    }
                    else
                    {
                        _PreviousEventSponsorImageFileName = eventSponsor.SponsorIcon;
                    }
                }

                // Insert-Update Event Sponsor Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  requestEventSponsor_VM.Id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("eventId", requestEventSponsor_VM.EventId),
                            new SqlParameter("sponsorTitle", requestEventSponsor_VM.SponsorTitle),
                            new SqlParameter("sponsorLink",requestEventSponsor_VM.SponsorLink),
                            new SqlParameter("sponsorIcon", _EventSponsorIconFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestEventSponsor_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEventSponsor @id,@userLoginId,@eventId,@sponsorTitle,@sponsorLink,@sponsorIcon,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
                        if (!String.IsNullOrEmpty(_PreviousEventSponsorImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EventSponsorIcon), _PreviousEventSponsorImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EventSponsorIcon), _EventSponsorIconFileNameGenerated);
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
        /// Get All List Event Sponsor by EventId 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Sponsor/GetAllList")]
        public HttpResponseMessage GetAllEventSponsorList(long id)
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

                List<EventSponsor_VM> eventSponsorList = new List<EventSponsor_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("eventId", id),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("mode", "1"),
                };
                eventSponsorList = db.Database.SqlQuery<EventSponsor_VM>("exec sp_ManageEventSponsor @id,@eventId,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = eventSponsorList;

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
        /// Get Event Sponsor Data by Id and EventId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="EventId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Sponsor/GetById")]
        public HttpResponseMessage GetByIdEventSponsor(long id, long EventId)
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

                EventSponsor_VM eventSponsor = new EventSponsor_VM();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", id),
                                new SqlParameter("eventId", EventId),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("mode", "2"),
                };
                eventSponsor = db.Database.SqlQuery<EventSponsor_VM>("exec sp_ManageEventSponsor @id,@eventId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = eventSponsor;

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
        /// Delete EventSponsor by EventSponsorId and EventId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="EventId"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Sponsor/DeleteById")]
        public HttpResponseMessage DeleteEventSponsorById(long id, long EventId)
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
                            new SqlParameter("id",  id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("eventId", EventId),
                            new SqlParameter("sponsorTitle", ""),
                            new SqlParameter("sponsorLink",""),
                            new SqlParameter("sponsorIcon", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEventSponsor @id,@userLoginId,@eventId,@sponsorTitle,@sponsorLink,@sponsorIcon,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
        /// Add EventDeatils Update Functionality Padding due to requirement
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Deatils/AddUpdate")]
        public HttpResponseMessage AddUpdateEventDetails()
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

                EventDetails_VM requestEventDetails_VM = new EventDetails_VM();
                requestEventDetails_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestEventDetails_VM.EventId = Convert.ToInt64(HttpRequest.Params["eventId"]);
                requestEventDetails_VM.Name = HttpRequest.Params["name"].Trim();
                requestEventDetails_VM.Link = HttpRequest.Params["link"].Trim();
                requestEventDetails_VM.Designation = HttpRequest.Params["designation"].Trim();
                requestEventDetails_VM.DetailsType = HttpRequest.Params["eventDetailsType"].Trim();


                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _EventDetailImageFile = files["image"];
                requestEventDetails_VM.Image = _EventDetailImageFile; // for validation
                string _EventDetailsImageFileNameGenerated = ""; //will contains generated file name


                // Validate infromation passed
                Error_VM error_VM = requestEventDetails_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_EventDetailImageFile != null)
                    {
                        _EventDetailsImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_EventDetailImageFile);
                    }
                }
                if (requestEventDetails_VM.DetailsType == "Host")
                {
                    if (String.IsNullOrEmpty(requestEventDetails_VM.Designation))
                    {
                        var vm = new Error_VM();
                        vm.Valid = true;

                        if (vm.Valid == false)
                        {
                            vm.Message = Resources.BusinessPanel.DesignationErrorMessageEventDetails;
                        }

                        Request.CreateResponse(HttpStatusCode.OK, vm);

                    }

                }
                else
                {
                    requestEventDetails_VM.Designation = "";
                }
                // Insert-Update Event Details Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestEventDetails_VM.Id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("eventId", requestEventDetails_VM.EventId),
                            new SqlParameter("name", requestEventDetails_VM.Name),
                            new SqlParameter("link", requestEventDetails_VM.Link),
                            new SqlParameter("detailsType",requestEventDetails_VM.DetailsType),
                             new SqlParameter("designation",requestEventDetails_VM.Designation),
                            new SqlParameter("image", _EventDetailsImageFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEventDetails @id,@userLoginId,@eventId,@name,@link,@detailsType,@designation,@image,@submittedbyLoginId,@mode", queryParams).FirstOrDefault();

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
                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_EventDetailsImage), _EventDetailsImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_EventDetailImageFile, FileWithPath);
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
        /// Get All Event Details By EventID  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Details/GetAllList")]
        public HttpResponseMessage GetAllEventDetailsList(long id)
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

                List<EventDetailsList_VM> eventDetailsList = new List<EventDetailsList_VM>();
                List<EventDetailsList_VM> organizerList = new List<EventDetailsList_VM>();
                List<EventDetailsList_VM> hostList = new List<EventDetailsList_VM>();
                List<EventDetailsList_VM> specialGuestList = new List<EventDetailsList_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                                new SqlParameter("eventId", id),
                                new SqlParameter("mode", "1"),
                };
                eventDetailsList = db.Database.SqlQuery<EventDetailsList_VM>("exec sp_ManageEventDetails @id,@userLoginId,@eventId,@mode", queryParams).ToList();


                foreach (var eventDetail in eventDetailsList)
                {
                    if (eventDetail.DetailsType == "Organizer")
                    {
                        organizerList.Add(eventDetail);
                    }
                    if (eventDetail.DetailsType == "Host")
                    {
                        hostList.Add(eventDetail);
                    }

                    if (eventDetail.DetailsType == "SpecialGuest")
                    {
                        specialGuestList.Add(eventDetail);
                    }

                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    OrganiserList = organizerList,
                    HostList = hostList,
                    SpecialGuestList = specialGuestList,
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
        /// Delete Event Details by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/Details/DeleteById")]
        public HttpResponseMessage DeleteEventDetailsById(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("eventId", "0"),
                            new SqlParameter("name", ""),
                            new SqlParameter("link", ""),
                            new SqlParameter("detailsType",""),
                             new SqlParameter("designation",""),
                            new SqlParameter("image", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateEventDetails @id,@userLoginId,@eventId,@name,@link,@detailsType,@designation,@image,@submittedbyLoginId,@mode", queryParams).FirstOrDefault();

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
        /// Get Event Full Deatils By Event-Id  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetAllDetailById")]
        public HttpResponseMessage GetAllEventListById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _LoginId = validateResponse.UserLoginId;

                Event_VM event_VM = new Event_VM();

                // Get Event By Id
                event_VM = eventService.GetEventById(id);

                //Get EventSponsor List By Event Id 

                List<EventSponsor_VM> eventSponsorList = new List<EventSponsor_VM>();

                SqlParameter[] queryParams_SponsorList = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("eventId", id),
                                new SqlParameter("userLoginId", "0"),
                                new SqlParameter("mode", "1"),
                };
                eventSponsorList = db.Database.SqlQuery<EventSponsor_VM>("exec sp_ManageEventSponsor @id,@eventId,@userLoginId,@mode", queryParams_SponsorList).ToList();

                //Get Event Details List By Event Id 

                List<EventDetailsList_VM> eventDetailsList = new List<EventDetailsList_VM>();
                List<EventDetailsList_VM> organizerList = new List<EventDetailsList_VM>();
                List<EventDetailsList_VM> hostList = new List<EventDetailsList_VM>();
                List<EventDetailsList_VM> specialGuestList = new List<EventDetailsList_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", "0"),
                                new SqlParameter("eventId", id),
                                new SqlParameter("mode", "1"),
                };
                eventDetailsList = db.Database.SqlQuery<EventDetailsList_VM>("exec sp_ManageEventDetails @id,@userLoginId,@eventId,@mode", queryParams).ToList();

                //List<EventDetailsList_VM> organizerList = eventDetailsList.Where(details => details.DetailsType == "Organizer").ToList();
                foreach (var eventDetail in eventDetailsList)
                {
                    if (eventDetail.DetailsType == "Organizer")
                    {
                        organizerList.Add(eventDetail);
                    }
                    if (eventDetail.DetailsType == "Host")
                    {
                        hostList.Add(eventDetail);
                    }

                    if (eventDetail.DetailsType == "SpecialGuest")
                    {
                        specialGuestList.Add(eventDetail);
                    }

                }
                #region Upcoming events 




                List<UpComingEvent_Vm> event_VMUpComingLst = new List<UpComingEvent_Vm>();
                // Get Event By Id
                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "3")
                            };

                event_VMUpComingLst = db.Database.SqlQuery<UpComingEvent_Vm>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).ToList();
                foreach (var upComingEventList in event_VMUpComingLst)
                {
                    SqlParameter[] query_Params = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", "0"),
                                new SqlParameter("eventId", upComingEventList.Id),
                                new SqlParameter("mode", "1"),
                    };
                    var DetailsList = db.Database.SqlQuery<EventDetailsList_VM>("exec sp_ManageEventDetails @id,@userLoginId,@eventId,@mode", query_Params).ToList();
                    EventDetailsList_VM FirstHostName = DetailsList.Where(details => details.DetailsType == "Host").FirstOrDefault();
                    
                    if (FirstHostName != null)
                        upComingEventList.HostName = FirstHostName.Name;
                    else
                        upComingEventList.HostName = "";

                    //event_VMLst.Add(upComingEventList);
                }
                #endregion
                #region Get UserName 
                EventCreatorName_VM eventCreatorName = new EventCreatorName_VM();

                eventCreatorName = eventService.GetEventCreatorName(id);
                #endregion

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    EventData = event_VM,
                    OrganiserList = organizerList,
                    HostList = hostList,
                    SpecialGuestList = specialGuestList,
                    EventSponsorList = eventSponsorList,
                    UpComingEventList = event_VMUpComingLst,
                    EventCreatorName = eventCreatorName,
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
        /// Get All Event List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetAllList")]
        public HttpResponseMessage GetAllEventList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _LoginId = validateResponse.UserLoginId;
                List<Event_VM> event_VM = new List<Event_VM>();
                // Get Event By Id
                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                event_VM = db.Database.SqlQuery<Event_VM>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    EventList = event_VM,
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
        /// Get All Events For Home Page display
        /// </summary>
        /// <returns>Events List</returns>
        [HttpGet]
        [Route("api/Event/GetAllEventsForHomePage")]
        public HttpResponseMessage GetAllCertificatesForHomePage()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var response = eventService.GetEventListForHomePage();

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
        /// Get Event Detail By UserLoginId for Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Event/GetAllEventBooking")]
        public HttpResponseMessage GetAllEventBookingList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<EventList_VM> response = eventService.GetAllEventList(_LoginId, lastRecordId, recordLimit);



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
        /// Get Event Detail By UserLoginId for Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Event/GetAllCoachesEventById")]
        public HttpResponseMessage GetAllCoachesEventById(long Id, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }


                //long _LoginId = validateResponse.UserLoginId;

                List<EventList_VM> response = eventService.GetCoachesEventById(Id, lastRecordId, recordLimit);



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
        /// Get all events by Business-Sub-Category
        /// </summary>
        /// <param name="businessSubCategoryId">Business Sub-Category-Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Event/GetAllEventDetail")]
        public HttpResponseMessage GetAllEventDetail(string menuTag, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<EventList_VM> response = eventService.GetAllEventDetailByMenuTag(menuTag, lastRecordId, recordLimit);

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
        /// To Add Business Content Event Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessEventDescription")]

        public HttpResponseMessage AddUpdateBusinessContentEventDetail()
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
                BusinessContentEvent_VM businessContentEvent_VM = new BusinessContentEvent_VM();
                businessContentEvent_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentEvent_VM.Description = HttpRequest.Params["Description"].Trim();
                businessContentEvent_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Validate infromation passed
                Error_VM error_VM = businessContentEvent_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentEvent_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentEvent_Param_VM
                {
                    Id = businessContentEvent_VM.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    Description = businessContentEvent_VM.Description,
                    Mode = businessContentEvent_VM.Mode
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
        /// To Get Business Content Event Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEventDetail")]
        public HttpResponseMessage GetBusinessContentEvent()
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


                BusinessContentEventDetail_VM resp = new BusinessContentEventDetail_VM();

                resp = eventService.GetBusinessContentEventDetail(_BusinessOwnerLoginId);



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
        /// Get event description by business id for showing for visitor panel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEventDescriptionForVisitorPanel")]
        public HttpResponseMessage GetBusinessContentEventDescriptionForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessContentEventDetail_VM resp = new BusinessContentEventDetail_VM();

                resp = eventService.GetBusinessContentEventDetail(businessOwnerLoginId);



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

        ////////////////////////////////////////////////// Event Company Detail /////////////////////////////////



        /// <summary>
        /// To Add Business  Banner Detail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpdateBusinessContentEventCompany")]

        public HttpResponseMessage AddUpdateBusinessEventCompanyProfile()
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
                BusinessContentEventCompany_ViewModel businessContentEventCompany_ViewModel = new BusinessContentEventCompany_ViewModel();

                // Parse and assign values from HTTP request parameters
                businessContentEventCompany_ViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentEventCompany_ViewModel.Title = HttpRequest.Params["Title"]?.Trim();
                businessContentEventCompany_ViewModel.Description = HttpRequest.Params["Description"]?.Trim();
                businessContentEventCompany_ViewModel.EventOptions = HttpRequest.Params["EventOptions"]?.Trim();
                businessContentEventCompany_ViewModel.Mode = Convert.ToInt32(HttpContext.Current.Request.Params["Mode"]);




                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssEventCompanyImageFile = files["Image"];
                businessContentEventCompany_ViewModel.Image = _BusinesssEventCompanyImageFile; // for validation
                string _BusinessEventCompanyImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousBusinessEventCompanyImageFileName = ""; // will be used to delete file while updating.



                // Validate infromation passed
                Error_VM error_VM = businessContentEventCompany_ViewModel.ValidInformation();



                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_BusinesssEventCompanyImageFile != null)
                    {

                        _BusinessEventCompanyImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssEventCompanyImageFile);
                    }

                }




                if (businessContentEventCompany_ViewModel.Mode == 1)
                {
                    var respGetBusinessBannerDetail = eventService.GetBusinessEventCompanyDetail(_BusinessOwnerLoginId);

                    if (respGetBusinessBannerDetail != null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessBannerDetail.Image == null && _BusinesssEventCompanyImageFile == null)
                        {
                            _PreviousBusinessEventCompanyImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessEventCompanyImageFileNameGenerated = respGetBusinessBannerDetail.Image;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessEventCompanyImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentEventCompany_Get<SPResponseViewModel>(new SP_BusinessContentEventCompany_Param_VM
                {
                    Id = businessContentEventCompany_ViewModel.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageType.Id,
                    Title = businessContentEventCompany_ViewModel.Title,
                    EventOptions = businessContentEventCompany_ViewModel.EventOptions,
                    Description = businessContentEventCompany_ViewModel.Description,
                    Image = _BusinessEventCompanyImageFileNameGenerated,
                    Mode = businessContentEventCompany_ViewModel.Mode
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
                    #region Insert-Update Event Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessEventCompanyImageFileName))
                        {
                            // remove previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessEventCompanyImage), _PreviousBusinessEventCompanyImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessEventCompanyImage), _BusinessEventCompanyImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssEventCompanyImageFile, NewImageFileWithPath);


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
        /// To Get Event Company Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEventCompanyDetail")]
        public HttpResponseMessage GetEventCompanyDetailById()
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


                //var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //BusinesssProfilePageType.Key = "yoga_web";



                BusinessContentEventCompanyDetail_VM businessbannerDetail = eventService.GetBusinessEventCompanyDetail(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessbannerDetail;

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
        /// To Get Event Company Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEventCompanyDetailForVisitorPanel")]
        public HttpResponseMessage GetEventCompanyDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {


                //var BusinesssProfilePageType = businessOwnerService.GetBusinessProfileTypeDetailsById(_BusinessOwnerLoginId);
                //BusinesssProfilePageType.Key = "yoga_web";



                BusinessContentEventCompanyDetail_VM businessEventCompanyDetail = eventService.GetBusinessEventCompanyDetail(businessOwnerLoginId);

                string _eventOptions = (businessEventCompanyDetail == null) ? "" : businessEventCompanyDetail.EventOptions;
                string[] stringArray = _eventOptions.Split(',');

                List<BusinessContentEventCompanyDetailEventOptions_VM> businessEventCompanyDetailEventOptions = new List<BusinessContentEventCompanyDetailEventOptions_VM>();
                foreach (string item in stringArray)
                {
                    // Create an instance of BusinessContentEventCompanyDetailEventOptions_VM and set its properties
                    BusinessContentEventCompanyDetailEventOptions_VM option = new BusinessContentEventCompanyDetailEventOptions_VM
                    {
                        EventOptionsName = item // Assuming 'Name' is the property in BusinessContentEventCompanyDetailEventOptions_VM
                    };

                    businessEventCompanyDetailEventOptions.Add(option);
                }

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                //apiResponse.data = businessbannerDetail;
                apiResponse.data = new
                {
                    BusinessEventCompanyDetails = businessEventCompanyDetail,
                    BusinessEventCompanyEventOptions = businessEventCompanyDetailEventOptions
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
        /// Get Business Upcoming-Events List for businesss panel
        /// </summary>
        /// <returns>Business Upcoming-Events-List Data</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetUpcomingEventsListForBusinessPanel")]
        public HttpResponseMessage GetBusinessUpcomingEventsList()
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


                //Get Business-Upcoming-Events
                var _UserCurrentDateTime = DateTime.Now; // IST
                var _UserCurrentTimezoneOffset = "";
                var _BusinessUpcomingEvents = businessOwnerService.GetUpcomingEventsForBusinessProfile(_BusinessOwnerLoginId, _UserCurrentTimezoneOffset, 10);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = _BusinessUpcomingEvents;

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
        /// To Add  Events Images 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/AddUpcomingEventImages")]

        public HttpResponseMessage AddUpcomingEventImages()
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
                BusinessContentEventOrganisationImages_VM businessContentEventOrganisationImages_VM = new BusinessContentEventOrganisationImages_VM();

                // Check and set Title 
                businessContentEventOrganisationImages_VM.EventId = Convert.ToInt64(HttpRequest.Params["EventId"]);




                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssEventImageFile = files["EventImage"];
                businessContentEventOrganisationImages_VM.Image = _BusinesssEventImageFile; // for validation
                string _BusinessEventImageFileNameGenerated = ""; //will contains generated file name

                // Validate infromation passed
                Error_VM error_VM = businessContentEventOrganisationImages_VM.ValidInformation();



                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }




                if (files.Count > 0)
                {
                    if (_BusinesssEventImageFile != null)
                    {

                        _BusinessEventImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssEventImageFile);
                    }

                }


                var resp = storedProcedureRepository.SP_InsertUpdateBusinessContentEventImage_Get<SPResponseViewModel>(new SP_BusinessContentEventImages_Param_VM
                {
                    Id = businessContentEventOrganisationImages_VM.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    ProfilePageTypeId = BusinesssProfilePageTypeId,
                    EventId = businessContentEventOrganisationImages_VM.EventId,
                    Image = _BusinessEventImageFileNameGenerated,
                    Mode = 1
                });




                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    #region Insert Events Images on Server
                    if (files.Count > 0)
                    {

                        // save new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessEventImage), _BusinessEventImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssEventImageFile, NewImageFileWithPath);


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
        /// Get All Business Content Event Image with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllEventImageDetailByPagination")]
        public HttpResponseMessage GetAllBusinessContentEventImageDataTablePagination()
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

                BusinessContentEventOrganisationDetail_Pagination_SQL_Params_VM _Params_VM = new BusinessContentEventOrganisationDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = eventService.GetBusinessContentEventImageList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Delete Event Image Detail
        /// </summary>
        /// <param name="id">EventId</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteEventImageDetail")]
        public HttpResponseMessage DeleteEventImageById(long id)
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

                // Delete Content EventId  
                resp = eventService.DeleteEventImageDetail(id);


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


        /// <summary>
        /// To Get Event Company Detail By BusinessOwnerLoginId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBusinessContentEventImageSectionDetailForVisitorPanel")]
        public HttpResponseMessage GetEventImageSectionDetail(long businessOwnerLoginId, long eventId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<BusinessContentEventOrganisationImagesDetail_VM> businessEventImageDetail = eventService.GetBusinessEventImageDetaillst(businessOwnerLoginId, eventId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = businessEventImageDetail;


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
        /// Get All EventList For visitor panel with View-More Pagination Functionality
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Student,BusinessAdmin,Staff,SubAdmin,SuperAdmin")]
        [Route("api/Event/GetAllUpComingEvents")]
        public HttpResponseMessage GetAllUpcomingEventsByPagination(long businessOwnerLoginId, long lastRecordId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                var _UserCurrentDateTime = DateTime.Now; // IST
                //var _UserCurrentTimezoneOffset = "";

                List<Event_VM> lst = new List<Event_VM>();

                SqlParameter[] queryParams = new SqlParameter[] {
                             new SqlParameter("id", "0"),
                                new SqlParameter("eventUserLoginId", businessOwnerLoginId),
                                new SqlParameter("currentDateTime", _UserCurrentDateTime),
                                new SqlParameter("recordLimit", "0"),
                                new SqlParameter("lastRecordId", lastRecordId),
                                new SqlParameter("mode", "2")
                };

                lst = db.Database.SqlQuery<Event_VM>("exec sp_GetUpcomingEvents @id, @eventUserLoginId, @currentDateTime, @recordLimit, @lastRecordId, @mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    UpcomingEventsList = lst,
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
        /// Get All Event List by Business
        /// </summary>
        /// <returns>Business Events list</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetAllByBusiness")]
        public HttpResponseMessage GetAllBusinessEventList()
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

                List<Event_VM> event_VM = new List<Event_VM>();
                // Get Event By Id
                SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "8")
                            };

                event_VM = db.Database.SqlQuery<Event_VM>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = event_VM;

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
        /// Get all events by Search-Filter - VISITOR PANEL
        /// </summary>
        /// <returns>Filtered Events List</returns>
        [HttpPost]
        [Route("api/Event/GetEventsBySearchFilter")]
        public HttpResponseMessage GetEventsBySearchFilter(SearchFilter_APIParmas_VM filterParams)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<EventList_VM> response = eventService.GetEventsListBySearchFilter(filterParams);

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
        /// get event details for my booking
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Event/GetEventDetails")]
        public HttpResponseMessage GetEventDetails()
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
                List<EventBooking_ViewModel> event_VMList = eventService.GetEventList(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = event_VMList;

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
        /// get event details by id for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Event/GetEventBookingDetailById")]
        public HttpResponseMessage GetEventBookingDetailById(long id)
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

                // Get Event-Booking-Detail
                var eventBookingDetail = eventService.GetEventBookingDetailbyId(id, _LoginID_Exact);

                // Get Event-Detail
                //EventBooking_ViewModel eventbookingDetail = new EventBooking_ViewModel();
                //eventbookingDetail = eventService.GetEventBookingDetail(eventBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(eventBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    // EventDetail = eventbookingDetail,
                    EventBookingDetail = eventBookingDetail,
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
        /// get event details for my booking
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetBusinessEventBookingDetails")]
        public HttpResponseMessage GetBusinessEventBookingDetails()
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
                List<EventBooking_ViewModel> event_VMList = eventService.GetBusinessEventList(_LoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = event_VMList;

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
        /// get event details by id for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Event/GetBusinessEventBookingDetailById")]
        public HttpResponseMessage GetBusinessEventBookingDetailById(long id)
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

                // Get Event-Booking-Detail
                var eventBookingDetail = eventService.GetEventBookingDetailById(id, _LoginID_Exact);

                // Get Event-Detail
                EventBooking_ViewModel eventbookingDetail = new EventBooking_ViewModel();
                eventbookingDetail = eventService.GetEventBookingDetail(eventBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(eventBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    EventDetail = eventbookingDetail,
                    EventBookingDetail = eventBookingDetail,
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

    }
}