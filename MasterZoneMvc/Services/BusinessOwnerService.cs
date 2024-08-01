using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class BusinessOwnerService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessOwnerService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Business Owner Table-Data By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Onwer Login-Id</param>
        /// <returns>Business Owner Table-Record</returns>
        public BusinessOwnerViewModel GetBusinessOwnerTableDataById(long businessOwnerLoginId)
        {
            SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4")
                            };

            var businessOwner = db.Database.SqlQuery<BusinessOwnerViewModel>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@userLoginId,@mode", queryParamsPermissions).FirstOrDefault();
            return businessOwner;
        }

        /// <summary>
        /// Check if Business Owner is Prime Member or Not
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Owner Login ID</param>
        /// <returns>Returns: -1 if not Business-Not-Found, 1 if Prime, 0 if not a Prime Member</returns>
        public int IsBusinessOwnerPrimeMember(long businessOwnerLoginId)
        {
            var businessOwnerTableData = GetBusinessOwnerTableDataById(businessOwnerLoginId);

            return (businessOwnerTableData == null) ? -1 :  businessOwnerTableData.IsPrimeMember;
        }

        /// <summary>
        /// Get Business Owners Detail By Pagination [Jquery Datatable Pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessDetail_Pagination_VM> GetBusinessList_Pagination(NameValueCollection httpRequestParams, BusinessOwnerList_Pagination_SQL_Params_VM _Params_VM)
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


            List<BusinessDetail_Pagination_VM> lstBusinessRecords = new List<BusinessDetail_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstBusinessRecords = db.Database.SqlQuery<BusinessDetail_Pagination_VM>("exec sp_ManageBusinessDetail_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstBusinessRecords.Count > 0 ? lstBusinessRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstBusinessRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }
        
        /// <summary>
        /// Get Business-Instructors List Detail By Pagination [Jquery Datatable Pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessInstrctor_Pagination_SuperAdminPanel_VM> GetInstructorListForSuperAdmin_Pagination(NameValueCollection httpRequestParams, BusinessOwnerList_Pagination_SQL_Params_VM _Params_VM)
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


            List<BusinessInstrctor_Pagination_SuperAdminPanel_VM> lstBusinessRecords = new List<BusinessInstrctor_Pagination_SuperAdminPanel_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "2")
                            };

            lstBusinessRecords = db.Database.SqlQuery<BusinessInstrctor_Pagination_SuperAdminPanel_VM>("exec sp_ManageBusinessDetail_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstBusinessRecords.Count > 0 ? lstBusinessRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessInstrctor_Pagination_SuperAdminPanel_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessInstrctor_Pagination_SuperAdminPanel_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstBusinessRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Request InsertUpdate Stored-Procedure
        /// </summary>
        /// <param name="params_VM">Stored-Procedure Parameters</param>
        /// <returns>Response Object having status</returns>
        public SPResponseViewModel_UserAddUpdate InsertUpdateBusinessOwner(SP_InsertUpdateBusinessOwner_Params_VM params_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            
            if (params_VM.Mode == 1)
            {
                // Generate Unique-User-Id
                UserLoginService userLoginService = new UserLoginService(db);
                params_VM.UniqueUserId = userLoginService.GenerateRandomUniqueUserId(StaticResources.UserId_Prefix_BusinessUser);
            }

            return storedProcedureRepository.SP_InsertUpdateBusinessOwner<SPResponseViewModel_UserAddUpdate>(params_VM);
        }

        /// <summary>
        /// Get Business Owner Basic-Profile-Page-Data By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Onwer Login-Id</param>
        /// <returns>Profile page basic data</returns>
        public BusinessProfilePageDetail_ForVisitor_VM GetBusinessOwnerBasicProfilePageForVisitorDataById(long businessOwnerLoginId)
        {
            SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "8")
                            };

            var businessOwner = db.Database.SqlQuery<BusinessProfilePageDetail_ForVisitor_VM>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@userLoginId,@mode", queryParamsPermissions).FirstOrDefault();
            return businessOwner;
        }

        /// <summary>
        /// Get Business Owner Business-Content-Videos List By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Onwer Login-Id</param>
        /// <returns>Business-Content-Videos List</returns>
        public List<BusinessVideoResponse_VM> GetAllBusinessContentVideosList(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessVideos_GetAll<BusinessVideoResponse_VM>(new SP_ManageBusinessVideos_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All-Active Business-Owner Classes List with Batch For Visitor-Panel By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Onwer Login-Id</param>
        /// <returns>Classes with batch List</returns>
        public List<ClassSearchList_VM> GetAllActiveBusinessClassesForVisitor(long businessOwnerLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("userLoginId", businessOwnerLoginId),
                                 new SqlParameter("dayname", "0"),
                                new SqlParameter("mode", "23"),
                };
            return db.Database.SqlQuery<ClassSearchList_VM>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Get Related Business-Owners List For Visitor-Panel By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business Onwer Login-Id</param>
        /// <returns>Related Business-Owners List</returns>
        public List<BusinessOnwerListByMenuTag_VM> GetRelatedBusiness(long businessOwnerLoginId, int recordLimit)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                                new SqlParameter("recordLimit", recordLimit),
                                new SqlParameter("mode", "1")
                };
            return db.Database.SqlQuery<BusinessOnwerListByMenuTag_VM>("exec sp_GetRelatedBusinesses @businessOwnerLoginId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Get Upcoming Events Business-Owners List For Visitor-Panel By Business-Owner-Login-Id
        /// </summary>
        /// <param name="eventUserLoginId">Business Onwer Login-Id</param>
        /// <param name="currentUserDateTime">User-Current Date-Time</param>
        /// <param name="recordLimit">No. of records to fetch</param>
        /// <returns>Upcoming Events List</returns>
        public List<EventListByMenuTag_VM> GetUpcomingEventsForBusinessProfile(long eventUserLoginId, string currentUserDateTime, int recordLimit)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", "0"),
                                new SqlParameter("eventUserLoginId", eventUserLoginId),
                                new SqlParameter("currentDateTime", currentUserDateTime),
                                new SqlParameter("recordLimit", recordLimit),
                                new SqlParameter("lastRecordId", "0"),
                                new SqlParameter("mode", "1")
                };
            return db.Database.SqlQuery<EventListByMenuTag_VM>("exec sp_GetUpcomingEvents @id,@eventUserLoginId,@currentDateTime,@recordLimit,@lastRecordId,@mode", queryParams).ToList();
        }


        /// <summary>
        /// To Get Instructor About List by Id 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<CoachesInstructorAboutDetail_VM> GetCoachesAboutDetailListById(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetInstructorProfileDetail<CoachesInstructorAboutDetail_VM>(new SP_GetInstructorProfileDetail_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Business-Content-Image List
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns> User Image Detail</returns>
        public List<UserImageDetail_VM> GetAllBusinessContentImages(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessImages_GetAll<UserImageDetail_VM>(new SP_ManageBusinessImages_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// Delete Business Image by Id and Business-Login-Id
        /// </summary>
        /// <param name="Id">Image-Id</param>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel DeleteBusinessImage(long Id, long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateBusinessImages<SPResponseViewModel>(new SP_InsertUpdateBusinessImages_Params_VM()
            {
                Id = Id,
                UserLoginId = UserLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// Delete Business Vedio by Id and Business-Owner-Login-Id
        /// </summary>
        /// <param name="Id">Video-Id</param>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel DeleteBusinessVedio(long Id, long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateBusinessVideos<SPResponseViewModel>(new SP_InsertUpdateBusinessVideos_Params_VM()
            {
                Id = Id,
                UserLoginId = UserLoginId,
                Mode = 2
            });
        }


        /// <summary>
        /// Get All Business Trainings by Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Trainings List</returns>
        public List<UserTrainingsDetail_VM> GetBusinessTrainingDetailById(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTraining_GetAll<UserTrainingsDetail_VM>(new SP_ManageTraining_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Business Main-Plan-Bookings detail
        /// </summary>
        /// <param name="businessownerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Main-Plan Booking Detail</returns>
        public List<BusinessPlanDetail_VM> GetBusinessMainPlanBookingDetailsById(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageMainPlanBooking_GetAll<BusinessPlanDetail_VM>(new SP_ManageMainPlanBooking_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }


        /// <summary>
        /// Get Business Sports  detail
        /// </summary>
        /// <param name="businessownerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Sports  Detail</returns>
        public List<BusinessOwnerDetailList_VM> Get_BusinessSportsDetailList(string MenuTag, string Latitude, string Longitude, long LastRecordedId, int RecordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessDistanceLocation<BusinessOwnerDetailList_VM>(new SP_ManageBusinessDistanceLocation_Param_VM()
            {
                Latitude = Latitude, // Explicitly cast to decimal
                Longitude = Longitude,
                MenuTag = MenuTag,
                LastRecordedId = LastRecordedId,
                RecordedId = RecordLimit,
                Mode = 1
            });
        }


        /// <summary>
        /// Get Businesses By Search Filter
        /// </summary>
        /// <param name="parmas_VM">Search-Filter Params</param>
        /// <returns>Filtered Businesses list</returns>
        public List<BusinessOwnerDetailList_VM> GetB2BBusinessesListBySearchFilter(SearchFilter_APIParmas_VM parmas_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<BusinessOwnerDetailList_VM>(new SP_GetRecordsBySearch_Params_VM()
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
                Mode = 1
            });
        }


        /// <summary>
        /// Get Instructors List By Search Filter
        /// </summary>
        /// <param name="parmas_VM">Search-Filter Params</param>
        /// <returns>Filtered Instructors list</returns>
        public List<InstructorList_VM> GetInstructorsListBySearchFilter(SearchFilter_APIParmas_VM parmas_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            if(parmas_VM.UserLoginId > 0)
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<InstructorList_VM>(new SP_GetRecordsBySearch_Params_VM()
                {
                    UserLoginId = parmas_VM.UserLoginId,
                    Mode = 7
                });
            }
            else
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<InstructorList_VM>(new SP_GetRecordsBySearch_Params_VM()
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
                    Mode = 2
                });
            }
            
        }

        /// <summary>
        /// Business Instructor 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Instructor</returns>
        public List<InstructorList_VM> GetBusinessInstructorList(long BusinessOwnerLoginId, int mode)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusiness_GetAll<InstructorList_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = mode
            });
        }

        /// <summary>
        /// Business Instructor 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Instructor</returns>
        public InstructorList_VM GetBusinessInstructorDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_Get<InstructorList_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 8
            });

        }
        /// <summary>
        /// Business Service Pagination For business Owner Login page
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>

        public JqueryDataTable_Pagination_Response_VM<BusinessService_Pagination_VM> GetBusinessServiceList_Pagination(NameValueCollection httpRequestParams, BusinessService_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessService_Pagination_VM> lstBusinessServiceRecords = new List<BusinessService_Pagination_VM>();

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

            lstBusinessServiceRecords = db.Database.SqlQuery<BusinessService_Pagination_VM>("exec sp_ManageBusinessService_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstBusinessServiceRecords.Count > 0 ? lstBusinessServiceRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessService_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessService_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstBusinessServiceRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Business  Service Detail  Data by Id
        /// </summary>
        /// <param name="businessServiceId">businessService Id</param>
        /// <returns>Returns the Table-Row data</returns>


        public BusinessService_ViewModel GetBusinessServiceDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessService_Get<BusinessService_ViewModel>(new SP_ManageBusinessService_Params_VM()
            {
                Id = Id,
                Mode = 1
            });
        }

        /// <summary>
        /// Business Service Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete  Business Service Detail</returns>
        public SPResponseViewModel DeleteBusinessService(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessService_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }
        /// <summary>
        /// To Get Business Service data (List)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public List<BusinessService_ViewModel> GetBusinessServiceDetailList(long BusinessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessService<BusinessService_ViewModel>(new SP_ManageBusinessService_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2

            });

        }
        /// <summary>
        /// To Get Business Content Service data (List) For Yoga (Visitor-Panel)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public List<BusinessService_ViewModel> GetBusinessContentServiceDetailList(long BusinessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessService<BusinessService_ViewModel>(new SP_ManageBusinessService_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 3

            });

        }



        /// <summary>
        /// Get Business  Service Detail 
        /// </summary>
        /// <param name="businessServiceId">businessService Id</param>
        /// <returns>Returns the Table-Row data</returns>


        public BusinessService_ViewModel GetBusinessServiceDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessService_Get<BusinessService_ViewModel>(new SP_ManageBusinessService_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2
            });
        }
        /// <summary>
        /// Get Business  Timing Detail 
        /// </summary>
        /// <param name=""> </param>
        /// <returns>Returns the Table-Row data</returns>
        public BusinessContactDetail_VM GetBusinessTiming(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessTiming_Get<BusinessContactDetail_VM>(new SP_ManageBusinessTiming_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 9
            });
        }

        /// <summary>
        /// To Get Business Professional data (List)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public List<InstructorProfessional_VM> GetBusinessInstructorProfessionalList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_GetAll<InstructorProfessional_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 10

            });

        }
        /// <summary>
        /// Get Business  Prfile Type Key Detail 
        /// </summary>
        /// <param name=""> </param>
        /// <returns>Returns the Table-Row data</returns>


        public BusinessProfileTypeKey_VM GetBusinessProfileTypeDetailsById(long BusinessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_Get<BusinessProfileTypeKey_VM>(new SP_ManageBusiness_Params_VM()
            {
                BusinessOwnerLoginId = BusinessLoginId,
                Mode = 11
            });
        }

        /// <summary>
        /// Get Business   Content Service Detail For Yoga Visitor Panel
        /// </summary>
        /// <param name="businessServiceId">businessService Id</param>
        /// <returns>Returns the Table-Row data</returns>


        public BusinessService_ViewModel GetBusinessContentServiceDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentService_Get<BusinessService_ViewModel>(new SP_ManageBusinessService_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Business Content Video  PPCMeta Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentVideos_PPCMetaDetail GetBusinessContentVideoPPCMetaDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentVideoPPCMeta_GetSingle<BusinessContentVideos_PPCMetaDetail>(new SP_ManageBusinessContentVideo_PPCMeta_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To get Business Content Service Detail PPC Meta (List)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentVideos_PPCMetaDetail> GetBusinessContentVideoDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentVideoPPCMeta_GetList<BusinessContentVideos_PPCMetaDetail>(new SP_ManageBusinessContentVideo_PPCMeta_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// To Get Business Content Professional  PPCMeta Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentProfessionalDetail_VM GetBusinessContentProfessionalDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentProfessional_Get<BusinessContentProfessionalDetail_VM>(new SP_ManageBusinessContentProfessional_PPCMeta_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To get Business Content Sponsor Detail (List)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentSponsor_VM> GetBusinessContentSponsorList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentSponsor_GetList<BusinessContentSponsor_VM>(new SP_ManageBusinessContentSponsor_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Business Content Fitness Movement  PPCMeta Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentFitnessMovementDetail_VM GetBusinessContentFitnessDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentFitness_Get<BusinessContentFitnessMovementDetail_VM>(new SP_ManageBusinessContentFitness_PPCMeta_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }


        /// <summary>
        /// To Get Business Content Fitness Movement   Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentFitnessMovementDetailViewModel GetBusinessContentFitnessMovementDetail(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentFitnessMovement_Get<BusinessContentFitnessMovementDetailViewModel>(new SP_ManageBusinessContentFitness_PPCMeta_Params_VM()
            {
                Id = Id,
                Mode = 1
            });
        }

        /// <summary>
        /// Business Content Fitness Movement Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Fitness Movement  Detail</returns>
        public SPResponseViewModel DeleteFitnessMovementDetail(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentFitnessMovement_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentFitnessMovement_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }

        /// <summary>
        /// Fitness Movement Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentFitnessMovementDetail_Pagination_VM> GetBusinessContentFitnessMovementList_Pagination(NameValueCollection httpRequestParams, BusinessContentFitnessMovementDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentFitnessMovementDetail_Pagination_VM> lstFitnessMovementRecords = new List<BusinessContentFitnessMovementDetail_Pagination_VM>();

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

            lstFitnessMovementRecords = db.Database.SqlQuery<BusinessContentFitnessMovementDetail_Pagination_VM>("exec sp_ManageBusinessContentFitnessMovement_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstFitnessMovementRecords.Count > 0 ? lstFitnessMovementRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentFitnessMovementDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentFitnessMovementDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstFitnessMovementRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        //////////////////////////////////////////////////////////////////Much More Service //////////////////////////////////////////////



        /// <summary>
        /// To Get Business Content Much More Service  Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentMuchMoreServiceDetail_VM GetBusinessContentMuchMoreServiceDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentMuchMoreService_Get<BusinessContentMuchMoreServiceDetail_VM>(new SP_ManageBusinessContentMuchMoreService_Param_VM()
            {
                Id = Id,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Business Content Much More Service  Title Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentMuchMoreServiceDetailViewModel GetBusinessContentMuchMoreServiceDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentMuchMoreService_Get<BusinessContentMuchMoreServiceDetailViewModel>(new SP_ManageBusinessContentMuchMoreService_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// Business Content Much More Service Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content  Much More Service Detail</returns>
        public SPResponseViewModel DeleteMuchMoreServiceDetail(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentMuchMoreService_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentMuchMoreService_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }

        /// <summary>
        /// Much More  Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentMuchMoreServiceDetail_Pagination_VM> GetBusinessContentMuchMoreServiceList_Pagination(NameValueCollection httpRequestParams, BusinessContentMuchMoreServiceDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentMuchMoreServiceDetail_Pagination_VM> lstMuchMoreServiceRecords = new List<BusinessContentMuchMoreServiceDetail_Pagination_VM>();

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

            lstMuchMoreServiceRecords = db.Database.SqlQuery<BusinessContentMuchMoreServiceDetail_Pagination_VM>("exec sp_ManageBusinessContentMuchMoreService_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstMuchMoreServiceRecords.Count > 0 ? lstMuchMoreServiceRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentMuchMoreServiceDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentMuchMoreServiceDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstMuchMoreServiceRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// To get Business Content Much More Service Detail (List)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentMuchMoreServiceDetail_VM> GetBusinessContentMuchMoreServiceList_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentMuchMoreServicelstDetails_Get<BusinessContentMuchMoreServiceDetail_VM>(new SP_ManageBusinessContentMuchMoreService_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 3
            });
        }


        /// <summary>
        /// To Get Business Content Fitness Movement   Detail (List)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentFitnessMovementDetailViewModel> GetBusinessContentFitnessMovementDetailList_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentFitnessMovementList_Get<BusinessContentFitnessMovementDetailViewModel>(new SP_ManageBusinessContentFitness_PPCMeta_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2
            });
        }

        //////////////////////////////////////////////////////////////       Client Detail             //////////////////////////////
        /// <summary>
        /// Business Client Detail By Id   
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Client Detail </returns>
        public BusinessContentClientDetail_VM GetBusinessClientDetail_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentClientDetail_Get<BusinessContentClientDetail_VM>(new SP_ManageBusinessContentClientDetail_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }




        /// <summary>
        /// Business Client Detail By BusinessOwnerLoginId  
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Client Detail </returns>
        public BusinessContentClientDetail_VM GetBusinessClientDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentClientDetail_Get<BusinessContentClientDetail_VM>(new SP_ManageBusinessContentClientDetail_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        /// <summary>
        /// Business Client Detail By BusinessOwnerLoginId   
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Client Detail </returns>
        public List<BusinessContentClientDetail_VM> GetBusinessClientDetail_lst(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentClientDetail_lst_Get<BusinessContentClientDetail_VM>(new SP_ManageBusinessContentClientDetail_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }



        /// <summary>
        /// Client Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentClientDetail_Pagination_VM> GetBusinessclientList_Pagination(NameValueCollection httpRequestParams, BusinessContentClientDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentClientDetail_Pagination_VM> lstClientRecords = new List<BusinessContentClientDetail_Pagination_VM>();

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

            lstClientRecords = db.Database.SqlQuery<BusinessContentClientDetail_Pagination_VM>("exec sp_ManageBusinessContentClient_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstClientRecords.Count > 0 ? lstClientRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentClientDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentClientDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstClientRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Business Content client Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content client Detail</returns>
        public SPResponseViewModel DeleteBusinessClientById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessClient_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentClient_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }


        //////////////////////////////////// Business  Currently Working Detail//////////////////////
        ///
        /// <summary>
        /// Business Working Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Currently Working Business </returns>
        public List<BusinessCurrentlyWorkDetail_VM> Get_BusinessWorkDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_GetAll<BusinessCurrentlyWorkDetail_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 12
            });

        }

        /// <summary>
        /// To Get Business Content Profile Detail By BusinessOwnerLoginId (Visitor-Panel)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessOwnerList_VM SP_ManageBusinessContentProfileDetail_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_Get<BusinessOwnerList_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 14
            });

        }

        /// <summary>
        /// To Get Instructor Detail By BusinessOwnerLoginId (Visitor-Panel)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessOwnerList_VM SP_ManageInstructorProfileDetail_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_Get<BusinessOwnerList_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 15
            });

        }

        /// <summary>
        /// To Get Instructor Rating Detail By BusinessOwnerLoginId
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessOwnerList_VM> Get_InstructorRatingDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_GetAll<BusinessOwnerList_VM>(new SP_ManageBusiness_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 15
            });

        }

        //////////////////////////////////// Business  Course image Detail////////////////////// 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Course |Image Detail </returns>
        public BusinessContentCourseImagesDetails_VM GetBusinessCourseImage_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessCourseImageDetail_Get<BusinessContentCourseImagesDetails_VM>(new SP_ManageBusinessContentCourseImage_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }
        /// <summary>
        /// Business Content course image Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content course image Detail</returns>
        public SPResponseViewModel DeleteBusinessCourseImageById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentCourseImages_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentCourseImages_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }


        /// <summary>
        /// Course Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentCourseDetail_Pagination_VM> GetBusinessCourseList_Pagination(NameValueCollection httpRequestParams, BusinessContentCourseDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentCourseDetail_Pagination_VM> lstCourseRecords = new List<BusinessContentCourseDetail_Pagination_VM>();

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

            lstCourseRecords = db.Database.SqlQuery<BusinessContentCourseDetail_Pagination_VM>("exec sp_ManageBusinessContentCourse_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstCourseRecords.Count > 0 ? lstCourseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentCourseDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentCourseDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstCourseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }



        //////////////////////////////////// Business  Course image Detail List////////////////////// 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Course |Image Detail </returns>
        public List<BusinessContentCourseImagesDetails_VM> Get_BusinessCourseDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessCourseImageDetail_GetLst<BusinessContentCourseImagesDetails_VM>(new SP_ManageBusinessContentCourseImage_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }
        //////////////////////////////////////////////////////////////Business Curriculum ///////////////////////////////////////

        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Course |Image Detail </returns>
        public BusinessContentCurriculumDetail_VM GetBusinessCurriculumDetail_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessCurriculumDetail_Get<BusinessContentCurriculumDetail_VM>(new SP_ManageBusinessContentCurriculum_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }

        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Curriculum |Image Detail </returns>
        public List<BusinessContentCurriculumDetail_VM> Get_BusinessCurriculumDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessCurriculumDetail_GetLst<BusinessContentCurriculumDetail_VM>(new SP_ManageBusinessContentCurriculum_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        /// <summary>
        /// Business Content Curriculum  Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Curriculum  Detail</returns>
        public SPResponseViewModel DeleteBusinessCurriculumById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentCurriculum_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentCurriculum_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }


        /// <summary>
        /// Curriculum Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentCurriculumDetail_Pagination_VM> GetBusinessCurriculumList_Pagination(NameValueCollection httpRequestParams, BusinessContentCurriculumDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentCurriculumDetail_Pagination_VM> lstCurriculumRecords = new List<BusinessContentCurriculumDetail_Pagination_VM>();

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

            lstCurriculumRecords = db.Database.SqlQuery<BusinessContentCurriculumDetail_Pagination_VM>("exec sp_ManageBusinessContentCurriculum_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstCurriculumRecords.Count > 0 ? lstCurriculumRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentCurriculumDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentCurriculumDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstCurriculumRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }




        //////////////////////////////////////////////////////////////Business Education ///////////////////////////////////////

        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Course |Image Detail </returns>
        public BusinessContentEducationDetail_VM GetBusinessEducationDetail_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessEducationDetail_Get<BusinessContentEducationDetail_VM>(new SP_ManageBusinessContentEducationDetail_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }



        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Course |Image Detail </returns>
        public BusinessContentEducationDetail_VM GetBusinessEducationDetail_BybusinessOwnerLoginId(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessEducationDetail_Get<BusinessContentEducationDetail_VM>(new SP_ManageBusinessContentEducationDetail_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 3


            });

        }

        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Education |Image Detail </returns>
        public List<BusinessContentEducationDetail_VM> Get_BusinessEducationDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessEducationDetail_GetLst<BusinessContentEducationDetail_VM>(new SP_ManageBusinessContentEducationDetail_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 3


            });

        }


        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Education |Image Detail </returns>
        public List<BusinessContentEducationDetail_VM> Get_BusinessUniversityDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessEducationDetail_GetLst<BusinessContentEducationDetail_VM>(new SP_ManageBusinessContentEducationDetail_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }








        /// <summary>
        /// Business Content Curriculum  Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Curriculum  Detail</returns>
        public SPResponseViewModel DeleteBusinessEducationDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentEducation_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentEducation_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }


        /// <summary>
        /// Education Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentEducationDetail_Pagination_VM> GetBusinessEducationList_Pagination(NameValueCollection httpRequestParams, BusinessContentEducationDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentEducationDetail_Pagination_VM> lstEducationRecords = new List<BusinessContentEducationDetail_Pagination_VM>();

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

            lstEducationRecords = db.Database.SqlQuery<BusinessContentEducationDetail_Pagination_VM>("exec sp_ManageBusinessContentEducation_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstEducationRecords.Count > 0 ? lstEducationRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentEducationDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentEducationDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstEducationRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }




        //////////////////////////////////// Business  Language Icon Detail////////////////////// 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Language |Icon Detail </returns>
        public BusinessLanguageDetail_VM GetBusinessLanguageIcon_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessLanguageIconDetail_Get<BusinessLanguageDetail_VM>(new SP_ManageBussinessContentLanguage_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }
        /// <summary>
        /// Business Content Language Icon Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Language Icon Detail</returns>
        public SPResponseViewModel DeleteBusinessLanguageIconById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentLanguage_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentLanguage_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }


        /// <summary>
        /// Language Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessLanguageDetail_Pagination_VM> GetBusinessLanguageList_Pagination(NameValueCollection httpRequestParams, BusinessLanguageDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessLanguageDetail_Pagination_VM> lstLanguageRecords = new List<BusinessLanguageDetail_Pagination_VM>();

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

            lstLanguageRecords = db.Database.SqlQuery<BusinessLanguageDetail_Pagination_VM>("exec sp_ManageBusinessContentLanguage_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLanguageRecords.Count > 0 ? lstLanguageRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessLanguageDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessLanguageDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLanguageRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }



        //////////////////////////////////// Business  Language Icon Detail List////////////////////// 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Language |Image Detail </returns>
        public List<BusinessLanguageDetail_VM> GetBusinessLanguageIconDetaillst_ById(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessLanguageIconDetail_GetLst<BusinessLanguageDetail_VM>(new SP_ManageBussinessContentLanguage_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        //////////////////////////////////// Business  Language Icon Detail List////////////////////// 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Language |Image Detail </returns>
        public List<BusinessLanguageDetail_VM> GetBusinessLanguageIconDetaillst()
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessLanguageIconDetail_GetLst<BusinessLanguageDetail_VM>(new SP_ManageBussinessContentLanguage_Param_VM
            {


                Mode = 2


            });

        }

        //////////////////////////////////// Business  Language  Detail List////////////////////// 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Language |Image Detail </returns>
        public List<BusinessContentLanguageDetail_VM> GetBusinessLanguageDetaillst(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.ManageBusinessContentLanguageDetail_GetList<BusinessContentLanguageDetail_VM>(new SP_ManageBusinessLanguage_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Business Content Language  Detail Delete 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content Language  Detail</returns>
        public SPResponseViewModel DeleteBusinessLanguageBybusinessOwnerLoginId(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentLanguages_Get<SPResponseViewModel>(new SP_InsertUpdateLanguage_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        /////////////////////////////////////////////////////////////////////////////Education University Detail Delete 

        /// <summary>
        /// Business Content University  Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content University  Detail</returns>
        public SPResponseViewModel DeleteBusinessUniversityById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessUniversity_Get<SPResponseViewModel>(new SP_InsertUpdateUniversityDetail
            {
                Id = Id,
                Mode = 3


            });

        }


        ////////////////////////////////////////////////////////////////////  
        /// <summary>
        /// Business Content University  Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content University  Detail</returns>
        public SPResponseViewModel DeleteBusinessUniversityBybusinessOwnerLoginId(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessUniversityId_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentUniversityDetail_PPCMeta
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        //////////////////////////////////////////////////////  Teacher University detail Edit ////////////////////////////////
        ///


        //////////////////////////////////////////////////////////////Business Education ///////////////////////////////////////

        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get University  Detail </returns>
        public BusinessUniversityDetail_VM GetBusinessEducationTeacherDetail_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.ManageUniversityDetail_Get<BusinessUniversityDetail_VM>(new SP_ManageBusinessContentEducationDetail_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }
        // List show in teacher side for visitor panel 
        public List<BusinessUniversityDetail_VM> GetBusinessEducationTeacherDetailList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.ManageUniversityDetail_GetList<BusinessUniversityDetail_VM>(new SP_ManageBusinessContentEducationDetail_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }



        /// <summary>
        /// Language Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentUniversityDetail_Pagination_VM> GetBusinessTeacherList_Pagination(NameValueCollection httpRequestParams, BusinessContentUniversityDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentUniversityDetail_Pagination_VM> lstteacherRecords = new List<BusinessContentUniversityDetail_Pagination_VM>();

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

            lstteacherRecords = db.Database.SqlQuery<BusinessContentUniversityDetail_Pagination_VM>("exec sp_ManageBusinessContentTeacherUniversityDetail_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstteacherRecords.Count > 0 ? lstteacherRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentUniversityDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentUniversityDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstteacherRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /////////////////////////////////////////////////////////////////////  Notice Board /////////////////////////
        ///


        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Notice Board   Detail </returns>
        public BusinessContentNoticeBoardDetail_VM GetBusinessNoticeBoardDetail_ById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessNoticeBoard_Get<BusinessContentNoticeBoardDetail_VM>(new SP_ManageBusinessNoticeBoard_Param_VM
            {
                Id = Id,
                Mode = 1


            });

        }
        // List show in teacher side for visitor panel 
        public List<BusinessContentNoticeBoardDetail_VM> SP_ManageBusinessNoticeBoard_GetList(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessNoticeBoard_GetList<BusinessContentNoticeBoardDetail_VM>(new SP_ManageBusinessNoticeBoard_Param_VM
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }


        ////////////////////////////////////////////////////////////////////  
        /// <summary>
        /// Business Content Notice Board  Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content University  Detail</returns>
        public SPResponseViewModel DeleteNoticeBoardById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentNoticeBoard<SPResponseViewModel>(new SP_InsertUpdateBusinessNoticeBoard_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }
        ////////////////////////////////////////////////////////// Business course category Detail//////////////////////////
        ///

        // </summary>
        /// <param name=""></param>
        /// <returns>To Get  course category   Detail </returns>
        public BusinessContentCourseCategoryDetail_VM GetBusinessCourseCategoryDetail_ById(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessCourseCategoryDetail_Get<BusinessContentCourseCategoryDetail_VM>(new SP_ManageBusinessContentCourseCategory_Param_VM
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }
        /////////////////////////////////////////////////////// Not In Used ////////////////////////////////////////////////////////////////////////
        //// </summary>
        ///// <param name=""></param>
        ///// <returns>To Get  course category   Detail </returns>
        //public List<BusinessContentCourseCategoryDetail_VM> GetBusinessCourseCategoryDetail(long BusinessOwnerLoginId)
        //{
        //    storedProcedureRepository = new StoredProcedureRepository(db);

        //    return storedProcedureRepository.SP_ManageBusinessCourseCategoryDetail<BusinessContentCourseCategoryDetail_VM>(new SP_ManageBusinessContentCourseCategory_Param_VM
        //    {
        //        UserLoginId = BusinessOwnerLoginId,
        //        Mode = 1


        //    });

        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //////////////////////////////////////////////////////// Access Course Detail //////////////////////////////////////


        // </summary>
        /// <param name=""></param>
        /// <returns>To Get Access course    Detail </returns>
        public BusinessContentAccessCourseDetail_VM GetBusinessAccessCourseDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessAccessCourseDetail_Get<BusinessContentAccessCourseDetail_VM>(new SP_ManageBusinessContentAccessCours_Param_VM
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Get Business and User Master Id's with Temporary Ids List
        /// </summary>
        /// <param name="userLoginId">Business-Owner/Staff Login-Id</param>
        /// <returns>Return Master Id List details</returns>
        public MasterIdDetails_VM GetAllMasterIdDetails(long userLoginId)
        {
            // Get Busienss Master Id and linked Student-User Master Id
            var MasterId = storedProcedureRepository.SP_ManageBusiness_Get<MasterIdDetails_VM>(new SP_ManageBusiness_Params_VM()
            {
                UserLoginId = userLoginId,
                Mode = 16
            });

            if(MasterId != null)
            {
                // Get Temporary Master Ids as Staff linked with Business/User Master Id
                var TemporaryMasterIds = storedProcedureRepository.SP_ManageBusiness_GetAll<MasterIdDetails_VM>(new SP_ManageBusiness_Params_VM()
                {
                    UserLoginId = userLoginId,
                    Mode = 17
                });

                MasterId.TemporaryMasterIds = TemporaryMasterIds;
            }
            
            return MasterId;
        }

        /// <summary>
        /// To Get Staff BusinessOwners Currently working Detail By StaffUserLoginId 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public List<BusinessCurrentlyWorkDetail_VM> Get_BusinessUserStaffWorkDetailList(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_GetAll<BusinessCurrentlyWorkDetail_VM>(new SP_ManageBusiness_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 18
            });

        }

        /// <summary>
        /// To Get User Other Profile Detail List for (Other Profile In Visitor Panel)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessUserOtherProfileDetail GetUserOtherProfileDetailList(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_Get<BusinessUserOtherProfileDetail>(new SP_ManageBusiness_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 19
            });

        }

        /// <summary>
        /// Get Instructors List for Home Page (Active)
        /// </summary>
        /// <returns>Instructors List</returns>
        public List<InstructorList_ForHomePage_VM> GetAllInstructorListForHomePage()
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_GetAll<InstructorList_ForHomePage_VM>(new SP_ManageBusiness_Params_VM
            {
                Mode = 20
            });

        }

        /// <summary>
        /// To Get Single Instructor Detail By UserLoginId
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public InstructorList_VM GetBusinessSingleInstructorDetail(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusiness_Get<InstructorList_VM>(new SP_ManageBusiness_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 21
            });

        }


        // Business Content Contact Information 
        public BusinessContentContactInformation_VM GetBusinessContactInformation(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContactInformationDetail<BusinessContentContactInformation_VM>(new BusinessContentContactInformation_Param_VM
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Follows Detail 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public BusinessFollowerDetail_VM GetBusinessInstructorFollowingDetail(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageFollower_Get<BusinessFollowerDetail_VM>(new SP_ManageToGetFollowerUser_Param_VM
            {
                FavouriteUserLoginId = UserLoginId,
                Mode = 1
            });

        }

        /// <summary>
        /// To Get Business Follower Detail 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public BusinessFollowerDetail_VM GetBusinessInstructorFollowerDetail(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageFollower_Get<BusinessFollowerDetail_VM>(new SP_ManageToGetFollowerUser_Param_VM
            {
                FollowingUserLoginId = UserLoginId,
                Mode = 2
            });

        }
        /// <summary>
        /// to get service details for main type
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessService_ViewModel> GetBusinessMainServiceDetailList(long BusinessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessService<BusinessService_ViewModel>(new SP_ManageBusinessService_Params_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 4

            });

        }
        /// <summary>
        /// to get business details for dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AdminDashBoard_VM GetBusinessDetailsDashboard(long id)
        {
            SqlParameter[] quereyParameter = new SqlParameter[] {
             new SqlParameter("userLoginId", id)
             };
             var resp = db.Database.SqlQuery<AdminDashBoard_VM>("exec sp_AdminDashBoardDetails @userLoginId", quereyParameter).FirstOrDefault();
            return resp;
        }

    }
}