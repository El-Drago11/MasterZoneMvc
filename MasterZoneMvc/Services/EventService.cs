using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class EventService
    {
        private MasterZoneDbContext db;
        private string _SiteURL = ConfigurationManager.AppSettings["SiteURL"];
        private StoredProcedureRepository storedProcedureRepository;


        public EventService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Event Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Event_VM GetEventById(long id)
        {
            // Get Event By Id
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

            var event_VM = db.Database.SqlQuery<Event_VM>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).FirstOrDefault();
            return event_VM;
        }

        /// <summary>
        /// Get Events List For Home Page (Show on Home Page active)
        /// </summary>
        /// <returns>Events List</returns>
        public List<Event_VM> GetEventListForHomePage()
        {
            return storedProcedureRepository.SP_ManageEvent_GetAll<Event_VM>(new SP_ManageEvent_Param_VM()
            {
                Mode = 9
            });
        }

        /// <summary>
        /// Get Event List For Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<Event_Pagination_VM> GetEventList_Pagination(NameValueCollection httpRequestParams, EventList_Pagination_SQL_Params_VM _Params_VM)
        {

            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";
            //_Params_VM.GetExpenditure_Params_VM = JsonConvert.DeserializeObject<GetExpenditure_Params_VM>(HttpContext.Current.Request.Params["_Params"]);

            long recordsTotal = 0;

            //// optimized code
            //string[] expenditureSortColumn = {
            //                "CreatedOn",
            //                "CreatedOn",
            //                "Name",
            //                "Description",
            //                "CreatedOn"
            //            };

            // _Params_VM.JqueryDataTableParams.sortColumn = expenditureSortColumn[_Params_VM.JqueryDataTableParams.sortColumnIndex];

            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";

            List<Event_Pagination_VM> lstEventRecords = new List<Event_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("createdByLoginId", _Params_VM.CreatedByLoginId),
                            new SqlParameter("userLoginId", _Params_VM.UserLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstEventRecords = db.Database.SqlQuery<Event_Pagination_VM>("exec sp_ManageEvent_Pagination @id,@createdByLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstEventRecords.Count > 0 ? lstEventRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Event_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Event_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstEventRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Event List For Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<Event_Pagination_VM_SuperAdminPanel> GetAllEventList_Pagination_ForSuperAdminPanel(NameValueCollection httpRequestParams, EventList_Pagination_SQL_Params_VM _Params_VM)
        {

            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";
            
            long recordsTotal = 0;

            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";

            List<Event_Pagination_VM_SuperAdminPanel> lstEventRecords = new List<Event_Pagination_VM_SuperAdminPanel>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("createdByLoginId", _Params_VM.CreatedByLoginId),
                            new SqlParameter("userLoginId", _Params_VM.UserLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstEventRecords = db.Database.SqlQuery<Event_Pagination_VM_SuperAdminPanel>("exec sp_ManageEvent_Pagination @id,@createdByLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstEventRecords.Count > 0 ? lstEventRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Event_Pagination_VM_SuperAdminPanel> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Event_Pagination_VM_SuperAdminPanel>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstEventRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get EventCreator Name 
        /// </summary>
        /// <returns></returns>
        public EventCreatorName_VM GetEventCreatorName(long id)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4")
                            };

            var event_VM = db.Database.SqlQuery<EventCreatorName_VM>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).FirstOrDefault();
            return event_VM;
        }

        /// <summary>
        /// Get Event Table Data By Id
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <returns>
        ///     Returns Event-Table-Record-Entity
        /// </returns>
        public EventViewModel GetEventDataById(long id)
        {
            // Get Event-Table-Data By Id
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "5")
                            };

            var event_VM = db.Database.SqlQuery<EventViewModel>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).FirstOrDefault();
            return event_VM;
        }

        /// <summary>
        /// Increase Totoal Joined by 1 for Event By Event-Id
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <returns>
        ///    
        /// </returns>
        public SPResponseViewModel UpdateEventTotalJoinedById(long id)
        {
            // Get Event-Table-Data By Id
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "6")
                            };

            var responseViewModel = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParamsGetEvent).FirstOrDefault();
            return responseViewModel;
        }

        /// <summary>
        /// Create QR-Code Ticket Image and save on server
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <returns>
        ///    QR-Code Generated Image Name
        /// </returns>
        public string CreateAndSaveQRCodeTicket(long eventId, long OrderId)
        {
            // QR-Code Image Create and save code
            string Encrypted_Class_Id = HttpContext.Current.Server.UrlEncode(EDClass.Encrypt(eventId.ToString()));
            string Encrypted_Order_Id = HttpContext.Current.Server.UrlEncode(EDClass.Encrypt(OrderId.ToString()));

            string URL = _SiteURL + "/Booking/ClassBookingVerification?classId=" + Encrypted_Class_Id + "&orderId=" + Encrypted_Order_Id;
            string QRCode_Image_Name = "EventTicket_" + Guid.NewGuid().ToString().Replace("-","") + ".png";

            #region Generate QR-Code 
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(URL, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap bitMap = qrCode.GetGraphic(20);
                bitMap.Save(ms, ImageFormat.Png);
                // return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_QRCodeTicketImage + QRCode_Image_Name), ms.ToArray());
                ms.Dispose();
            }
            #endregion
            return QRCode_Image_Name;// new Guid().ToString();
        }

        /// <summary>
        /// Verify if Event has already been purchased by User [Paid Order Status]
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="userLoginId">Logged In User-Login-Id (Buyer)</param>
        /// <returns>
        /// Returns true if already purchased, and false if not purchased
        /// </returns>
        public bool IsAlreadyEventPurchased(long eventId, long userLoginId)
        {
            // Get Event-Booking-Table-Data
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("eventId", eventId),
                            new SqlParameter("mode", "1")
                            };

            var response = db.Database.SqlQuery<EventBooking>("exec sp_ManageEventBooking @id,@userLoginId,@eventId,@mode", queryParamsGetEvent).FirstOrDefault();
            
            if(response == null)
            {
                return false;
            }

            return true;
        }

        public List<EventList_VM> GetAllEventList(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetAllStudentEventList<EventList_VM>(new SP_GetAllStudentEventList_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });

        }

        /// <summary>
        /// Get Instructor Events (Instructor organised from different or its businesses).
        /// </summary>
        /// <param name="userLoginId">Instructor-Business-Owner-Login-Id</param>
        /// <param name="lastRecordId">Last Fetched Record Id</param>
        /// <param name="recordLimit">No. of Items to fetch Next</param>
        /// <returns>Events List</returns>
        public List<EventList_VM> GetCoachesEventById(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetInstructorProfileDetail<EventList_VM>(new SP_GetInstructorProfileDetail_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 2
            });

        }

        /// <summary>
        /// To Get Event Detail (sports) by Menu Tag 
        /// </summary>
        /// <returns></returns>
        public List<EventList_VM> GetAllEventDetailByMenuTag(string MenuTag, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_ManageBusinessDetailByMenuTag_GetAll<EventList_VM>(new SP_ManageBusinessDetailByMenuTag_Params_VM()
            {
                MenuTag = MenuTag,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 2
            });
        }

        /// <summary>
        /// Get Events List By Search Filter
        /// </summary>
        /// <param name="parmas_VM">Search-Filter Params</param>
        /// <returns>Filtered Events list</returns>
        public List<EventList_VM> GetEventsListBySearchFilter(SearchFilter_APIParmas_VM parmas_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            if(parmas_VM.UserLoginId > 0)
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<EventList_VM>(new SP_GetRecordsBySearch_Params_VM()
                {
                    UserLoginId = parmas_VM.UserLoginId,
                    Mode = 9,
                });
            }
            else
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<EventList_VM>(new SP_GetRecordsBySearch_Params_VM()
                {
                    UserLoginId = parmas_VM.UserLoginId,
                    MenuTag = parmas_VM.MenuTag,
                    Location = parmas_VM.Location,
                    Latitude = parmas_VM.Latitude,
                    Longitude = parmas_VM.Longitude,
                    ItemType = parmas_VM.ItemType,
                    SearchText = parmas_VM.SearchText,
                    ItemMode = parmas_VM.ItemMode,
                    PriceType = parmas_VM.PriceType,
                    CategoryId = parmas_VM.CategoryId,
                    PageSize = parmas_VM.PageSize,
                    Page = parmas_VM.Page,
                    Mode = 3,
                    StartDate = parmas_VM.StartDate,
                });
            }

        }



        /// <summary>
        /// Business Content Event Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Event Detail</returns>
        public BusinessContentEventDetail_VM GetBusinessContentEventDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentEvent_Get<BusinessContentEventDetail_VM>(new SP_ManageBusinessContentEvent_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }



        /// <summary>
        /// Business Content Event Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Event Detail</returns>
        public BusinessContentEventCompanyDetail_VM GetBusinessEventCompanyDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentEventCompanyDetail_Get<BusinessContentEventCompanyDetail_VM>(new SP_ManageBusinessContentEventCompany_Parasm_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }
        /// <summary>
        /// Event Image Detail 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentEventOrganisationDetail_Pagination_VM> GetBusinessContentEventImageList_Pagination(NameValueCollection httpRequestParams, BusinessContentEventOrganisationDetail_Pagination_SQL_Params_VM _Params_VM)
        {
            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";

            long recordsTotal = 0;

            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";

            List<BusinessContentEventOrganisationDetail_Pagination_VM> lstEventImageRecords = new List<BusinessContentEventOrganisationDetail_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstEventImageRecords = db.Database.SqlQuery<BusinessContentEventOrganisationDetail_Pagination_VM>("exec sp_ManageBusinessContentEventImage_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstEventImageRecords.Count > 0 ? lstEventImageRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentEventOrganisationDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentEventOrganisationDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstEventImageRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Business Content Event Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Event Detail</returns>
        public SPResponseViewModel DeleteEventImageDetail(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentEventImage_Get<SPResponseViewModel>(new SP_BusinessContentEventImages_Param_VM
            {
                Id = Id,
                Mode = 2


            });

        }


        /// <summary>
        /// Business Content Event Image (List) Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Event Detail</returns>
        public List<BusinessContentEventOrganisationImagesDetail_VM> GetBusinessEventImageDetaillst(long BusinessOwnerLoginId, long EventId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentEventImageDetail_Get<BusinessContentEventOrganisationImagesDetail_VM>(new SP_ManageBusinessContentEventImage_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                EventId = EventId,
                Mode = 1


            });

        }

        #region Event Category -----------------------------------------------------

        /// <summary>
        /// Insert or Update Insert Update Event Category
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateEventCategory(SP_InsertUpdateEventCategory_Params_VM param)
        {
            return storedProcedureRepository.SP_InsertUpdateEventCategory_Get<SPResponseViewModel>(param);
        }

        /// <summary>
        /// Get All Event Category List
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Event Category list</returns>
        public List<EventCategory_VM> GetAllEventCategories()
        {
            return storedProcedureRepository.SP_ManageEventCategory_GetAll<EventCategory_VM>(new ViewModels.StoredProcedureParams.SP_ManageEventCategory_Params_VM()
            {
                Mode = 1
            });
        }


        /// <summary>
        /// Get Event Category ById-Type Detail By Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns>Detail</returns>
        public EventCategory_VM GetEventCategoryById(long id)
        {
            return storedProcedureRepository.SP_ManageEventCategory_Get<EventCategory_VM>(new ViewModels.StoredProcedureParams.SP_ManageEventCategory_Params_VM()
            {
                Id = id,
                Mode = 2
            });
        }

        /// <summary>
        /// delete Event category
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>

        public SPResponseViewModel DeleteEventCategory(long id)
        {
            return storedProcedureRepository.SP_InsertUpdateEventCategory_Get<SPResponseViewModel>(new SP_InsertUpdateEventCategory_Params_VM()
            {
                Id = id,
                Mode = 3,
            });
        }

        /// <summary>
        /// Change Status Class-Category-Type By Id
        /// </summary>
        /// <param name="id">Event-Category-Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel ChangeStatusEventCategoryType(long id)
        {
            return storedProcedureRepository.SP_InsertUpdateEventCategory_Get<SPResponseViewModel>(new SP_InsertUpdateEventCategory_Params_VM()
            {
                Id = id,
                Mode = 4
            });
        }
        #endregion -----------------------------------------------------------------
    
        /// <summary>
        /// get event booking details for my booking
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<EventBooking_ViewModel> GetEventList(long userloginid)
        {
            return storedProcedureRepository.SP_ManageEventBooking_GetAll<EventBooking_ViewModel>(new SP_ManageEventDetails_Params_VM()
            {
                UserLoginId = userloginid,
                Mode = 2
            });
        }

        /// <summary>
        /// get details event booking 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventBooking_ViewModel GetEventBookingDetailbyId(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageEventBooking_Get<EventBooking_ViewModel>(new SP_ManageEventDetails_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 3
            });
        }


        // <summary>
        /// get event booking details for my booking
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<EventBooking_ViewModel> GetBusinessEventList(long id)
        {
            return storedProcedureRepository.SP_ManageEventDetails_Get<EventBooking_ViewModel>(new SP_ManageEventDetails_Params_VM()
            {
                UserLoginId = id,
                Mode = 4
            });
        }

        /// <summary>
        /// get event booking details List  by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public EventBooking_ViewModel GetEventBookingDetailById(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageEventBooking_GetById<EventBooking_ViewModel>(new SP_ManageEventDetails_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 5
            });
        }


        /// <summary>
        /// get details event booking by UserLoginId & Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventBooking_ViewModel GetEventBookingDetail(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageEventBooking_Get<EventBooking_ViewModel>(new SP_ManageEventDetails_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 5
            });
        }



    }
}