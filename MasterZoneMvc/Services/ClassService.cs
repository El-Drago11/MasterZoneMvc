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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Controls;
using iTextSharp.tool.xml.html;

namespace MasterZoneMvc.Services
{
    public class ClassService
    {
        private MasterZoneDbContext db;
        private string _SiteURL = ConfigurationManager.AppSettings["SiteURL"];
        private StoredProcedureRepository storedProcedureRepository;

        /// <summary>
        /// Returns class date form class-days(available) list
        /// </summary>
        /// <param name="classDays">Class Days eg. (Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday)</param>
        /// <returns>Date of the Next class available day</returns>
        public static DateTime GetNextClassDateFromClassDays(string classDays)
        {
            DateTime today = DateTime.Today;

            var classDaysList = classDays.Split(',').ToList();
            int[] arrClassDaysInt = { 0, 0, 0, 0, 0, 0, 0 };

            // Mark arr list when class is available
            foreach (var day in classDaysList)
            {
                DayOfWeek dayOW = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), day);
                arrClassDaysInt[(int)dayOW] = 1;
            }

            int classDay = -1;
            var flag = 1;
            var todayDayNum = 4;
            for (var d = todayDayNum; d <= 6; d++)
            {
                //Console.WriteLine("d=" + d + " : " + classDays[d]);
                if (arrClassDaysInt[d] == 1)
                {
                    classDay = d;
                    break;
                }
                if (d == 6 && flag == 1)
                {
                    d = -1;
                    flag = 0;
                }
            }

            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilTuesday = ((int)classDay - (int)today.DayOfWeek + 7) % 7;
            DateTime nextClassDate = today.AddDays(daysUntilTuesday);
            return nextClassDate;
        }

        public ClassService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Class List For Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<Class_Pagination_VM> GetClassList_Pagination(NameValueCollection httpRequestParams, ClasstList_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";



            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<Class_Pagination_VM> lstClassRecords = new List<Class_Pagination_VM>();

            //IExpenditureRepository expenditureRepository = new ExpenditureRepository(db);
            //lstExpenditures = expenditureRepository.GetExpendituresList_Pagination(_Params_VM);

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

            lstClassRecords = db.Database.SqlQuery<Class_Pagination_VM>("exec sp_ManageClass_Pagination @id,@createdByLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstClassRecords.Count > 0 ? lstClassRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Class_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Class_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstClassRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Get All Class List With Pagination For Super-Admin-Panel
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>All Classes created by business list</returns>
        public JqueryDataTable_Pagination_Response_VM<Class_Pagination_VM_SuperAdminPanel> GetClassList_Pagination_ForSuperAdmin(NameValueCollection httpRequestParams, ClasstList_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";



            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<Class_Pagination_VM_SuperAdminPanel> lstClassRecords = new List<Class_Pagination_VM_SuperAdminPanel>();

            //IExpenditureRepository expenditureRepository = new ExpenditureRepository(db);
            //lstExpenditures = expenditureRepository.GetExpendituresList_Pagination(_Params_VM);

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("createdByLoginId", _Params_VM.CreatedByLoginId),
                            new SqlParameter("userLoginId", _Params_VM.UserLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "2")
                            };

            lstClassRecords = db.Database.SqlQuery<Class_Pagination_VM_SuperAdminPanel>("exec sp_ManageClass_Pagination @id,@createdByLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstClassRecords.Count > 0 ? lstClassRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Class_Pagination_VM_SuperAdminPanel> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Class_Pagination_VM_SuperAdminPanel>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstClassRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Get class Table Data by Id  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClassViewModel GetClassDataByID(long id)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                             new SqlParameter("dayname", "0"),
                            new SqlParameter("mode", "3")
                            };

            return db.Database.SqlQuery<ClassViewModel>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParamsGetEvent).FirstOrDefault();
        }

        /// <summary>
        /// Get Classes List For Home Page (Show on Home Page active)
        /// </summary>
        /// <returns>Classes List</returns>
        public List<ClassList_ForHomePage_VM> GetClassesListForHomePage()
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<ClassList_ForHomePage_VM>(new SP_ManageClass_Params_VM()
            {
                Mode = 19
            });
        }

        /// <summary>
        /// Create QR-Code Class Image and save on server
        /// </summary>
        /// <param name="id">Class Id</param>
        /// <returns>
        ///    QR-Code Generated Image Name
        /// </returns>
        public string CreateAndSaveQRCodeTicket(long classId, long OrderId)
        {

            // QR-Code Image Create and save code
            string Encrypted_Class_Id = HttpContext.Current.Server.UrlEncode(EDClass.Encrypt(classId.ToString()));
            string Encrypted_Order_Id = HttpContext.Current.Server.UrlEncode(EDClass.Encrypt(OrderId.ToString()));

            string URL = _SiteURL + "/Booking/ClassBookingVerification?classId=" + Encrypted_Class_Id + "&orderId=" + Encrypted_Order_Id;
            //string QRCode_Image_Name = Guid.NewGuid().ToString().Split('-')[0] + Guid.NewGuid().ToString().Split('-')[1] + Guid.NewGuid().ToString().Split('-')[2] + Guid.NewGuid().ToString().Split('-')[0] + ".png";
            string QRCode_Image_Name = "ClassTicket_" + Guid.NewGuid().ToString().Replace("-", "") + ".png";
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
        /// Verify if Class has already been purchased by User [Paid Order Status]
        /// </summary>
        /// <param name="classId">Class Id</param>
        /// <param name="userLoginId">Logged In User-Login-Id (Buyer)</param>
        /// <returns>
        /// Returns true if already purchased, and false if not purchased
        /// </returns>
        public bool IsAlreadyClassPurchased(long classId, long userLoginId, string joinDate)
        {
            // Get Class-Booking-Table-Data
            //SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
            //                new SqlParameter("id", "0"),
            //                new SqlParameter("userLoginId", userLoginId),
            //                new SqlParameter("classId", classId),
            //                new SqlParameter("batchId", "0"),
            //                new SqlParameter("mode", "1")
            //                };

            //var response = db.Database.SqlQuery<ClassBooking>("exec sp_ManageClassBooking @id,@userLoginId,@classId,@batchId,@mode", queryParamsGetEvent).ToList();
            var response = storedProcedureRepository.SP_ManageClassBooking_GetAll<ClassBooking>(new SP_ManageClassBooking_Params_VM()
            {
                UserLoginId = userLoginId,
                ClassId = classId,
                Mode = 1
            });

            if (response == null || response.Count() <= 0)
            {
                return false;
            }
            else
            {
                DateTime _ClassJoinDate = DateTime.ParseExact(joinDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                // check if same data exists in already purchased class.
                if (response[0].ClassPriceType == "per_class" || response[0].ClassPriceType == "demo")
                {
                    foreach (var item in response)
                    {
                        if (item.EndDate_DateTimeFormat == _ClassJoinDate)
                        {
                            return true;
                        }
                    };
                }
                else if (response[0].ClassPriceType == "per_month")
                {
                    foreach (var item in response)
                    {
                        if (_ClassJoinDate <= item.EndDate_DateTimeFormat)
                        {
                            return true;
                        }
                    }
                }

            }

            // user is able to purchase
            return false;
        }

        /// <summary>
        /// Is Class Limit of students reached.
        /// </summary>
        /// <param name="classId">Class Id</param>
        /// <param name="batchId">Batch Id</param>
        /// <param name="limit">Limit of batch</param>
        /// <returns>Returns true limit reached (i.e. students can't join) and false if not reached (i.e students can join)</returns>
        public bool IsClassStudentLimitReached(long classId, long batchId, int limit = -1)
        {
            // get class max-student-limit if not passed
            if (limit < 0)
            {
                var classData = GetClassDataByID(classId);
                //limit = classData.StudentMaxStrength;
            }

            // get class bookings count
            var count = storedProcedureRepository.SP_ManageClassBooking_Get<int>(new SP_ManageClassBooking_Params_VM()
            {
                ClassId = classId,
                BatchId = batchId,
                Mode = 2
            });

            // if count is less then class-batch is available for booking
            if (count < limit)
                return true;

            return false;
        }

        /// <summary>
        /// Get Class-Batch Booking count by Class and Batch Id
        /// </summary>
        /// <param name="classId">Class-Id</param>
        /// <param name="batchId">Batch-Id</param>
        /// <param name="joiningDate">Date on which bookings need to check</param>
        /// <returns>Booking count of Classes</returns>
        public int GetCurrentClassBatchBookingCount(long classId, long batchId, string joiningDate)
        {
            // get class bookings count
            var count = storedProcedureRepository.SP_ManageClassBooking_Get<int>(new SP_ManageClassBooking_Params_VM()
            {
                ClassId = classId,
                BatchId = batchId,
                JoiningDate = joiningDate,
                Mode = 2
            });

            return count;
        }

        /// <summary>
        /// Get Enroll Courses/Classes by User 
        /// </summary>
        /// <param name="userLoginId">Student-User-Login-Id</param>
        /// <param name="lastRecordId">Last-Fetched-Record-Id</param>
        /// <param name="recordLimit">No. of records to fetch</param>
        /// <returns>All Enrolled Course/Classes List</returns>
        public List<EnrollCourse_VM> GetNearExpiryEnrolledCourses(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetAllStudentCourseDetail_GetAll<EnrollCourse_VM>(new SP_GetAllStudentCourseDetail_Param_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Instructor Class List by Id 
        /// </summary>
        /// <param name="userLoginId">Instructor-Business-Owner-Login-Id</param>
        /// <param name="lastRecordId">Last Fetched Record Id</param>
        /// <param name="recordLimit">No. of records to fetch next</param>
        /// <returns>Classes List</returns>
        public List<InstructorClassList_VM> GetClassListById(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetInstructorProfileDetail<InstructorClassList_VM>(new SP_GetInstructorProfileDetail_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 3
            });

        }

        /// <summary>
        /// Get All Distinct Class-Category-Types List of created classes by business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Class-Category-Types List</returns>
        public List<ClassCategoryType_VM> GetAllBusinessClassesCategoriesTypesList(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<ClassCategoryType_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 5
            });
        }

        /// <summary>
        /// Get All Distinct Class-Category-Types List of created classes (of Online Mode) by business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Class-Category-Types List</returns>
        public List<ClassCategoryType_VM> GetAllBusinessOnlineClassesCategoriesTypesList(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<ClassCategoryType_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 6
            });
        }

        /// <summary>
        /// Get All Distinct Class-Category-Types List of created classes (of Offline Mode) by business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Class-Category-Types List</returns>
        public List<ClassCategoryType_VM> GetAllBusinessOfflineClassesCategoriesTypesList(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<ClassCategoryType_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 7
            });
        }

        /// <summary>
        /// Get Class List based on mode and other filters for Business-Owner [includes in Business-Owner-App]
        /// </summary>
        /// <param name="params_VM">Filter Parameter values</param>
        /// <returns>Class List</returns>
        public List<ClassList_Filter_BOApp_VM> GetAllClassListByFilter_ForBusinessOwner(SP_GetClassListByFilter_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_GetClassListByFilter_GetAll<ClassList_Filter_BOApp_VM>(new SP_GetClassListByFilter_Params_VM()
            {
                BusinessOwnerLoginId = params_VM.BusinessOwnerLoginId,
                ClassCategoryTypeId = params_VM.ClassCategoryTypeId,
                ClassMode = params_VM.ClassMode,
                SearchValue = params_VM.SearchValue,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All active offline Class List For dropdown By business 
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Class List</returns>
        public List<Class_DropdownList_VM> GetAllActiveOfflineClassesByClassMode_ForBusinessOwner(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<Class_DropdownList_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 8
            });
        }

        // <summary>
        /// Get All Classes Detail With InstructorDetail By business 
        /// </summary>
        /// <returns>Class List</returns>
        public List<BusinessAllClassesDetail> GetAllClasses_ForBusinessOwner()
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<BusinessAllClassesDetail>(new SP_ManageClass_Params_VM()
            {
                Mode = 9
            });
        }

        /// <summary>
        /// To Get Class Detail By UserLoginId For Visitor-Panel In Course detail Page
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>

        public BusinessSingleClassBookingDetail GetBusinessContentClassDetail_Get(long classId)
        {
            //return storedProcedureRepository.SP_ManageClassBooking_Get<BusinessSingleClassBookingDetail>(new SP_ManageClassBooking_Params_VM()
            //{
            //    Id = classId,
            //    Mode = 15
            //});

            return storedProcedureRepository.SP_ManageClass_Get<BusinessSingleClassBookingDetail>(new SP_ManageClass_Params_VM()
            {
                Id = classId,
                Mode = 15
            });
        }


        public List<InstructorList_VM> GetBusinessContentClassBatchInstructorDetail_Get(long classId)
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<InstructorList_VM>(new SP_ManageClass_Params_VM()
            {
                Id = classId,
                Mode = 16
            });
        }



        //<summary>
        ///get class booked detail By Class-Id in visitor-panel 
        ///</summary>
        /// <param name="classId"> Class Id </param>
        /// <param name="batchId"> Batch Id</param>
        /// <return>Class Detail </return>
        public BusinessSingleClassBookingDetail GetAllClassBookingDetail(long classId, long batchId)
        {
            return storedProcedureRepository.SP_ManageClassBooking_Get<BusinessSingleClassBookingDetail>(new SP_ManageClassBooking_Params_VM()
            {
                ClassId = classId,
                BatchId = batchId,
                Mode = 3
            });
        }

        public BusinessSingleClassBookingDetail GetAllClassBatchId(long classId)
        {
            return storedProcedureRepository.SP_ManageClassBooking_Get<BusinessSingleClassBookingDetail>(new SP_ManageClassBooking_Params_VM()
            {
                ClassId = classId,
                Mode = 4
            });
        }

        /// <summary>
        /// To Get Business Classes Detail By Menu-Tag
        /// </summary>
        /// <returns>List of Classes for particular sub-category</returns>
        public List<BusinessAllClassesDetail> GetAllClassesDetailByMenuTag(string MenuTag, long LastRecordId, int RecordLimit)
        {
            return storedProcedureRepository.SP_ManageBusinessDetailByMenuTag_GetAll<BusinessAllClassesDetail>(new SP_ManageBusinessDetailByMenuTag_Params_VM()
            {
                MenuTag = MenuTag,
                LastRecordId = LastRecordId,
                RecordLimit = RecordLimit,
                Mode = 1
            });
        }

        /// <summary>
        /// Instructor Detail For Class Detail
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public BusinessAllClassesDetail GetInstructorClassDetailList(long classId)
        {
            return storedProcedureRepository.SP_ManageClassBooking_Get<BusinessAllClassesDetail>(new SP_ManageClassBooking_Params_VM()
            {
                ClassId = classId,
                Mode = 5
            });
        }

        /// <summary>
        /// To Get BusinessOwnerLoginId
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public long GetBusinessOwnerLoginId(long classId)
        {
            return storedProcedureRepository.SP_ManageClassBooking_Get<SPResponseViewModel>(new SP_ManageClassBooking_Params_VM()
            {
                ClassId = classId,
                Mode = 6
            }).Id;
        }

        public List<BusinessAllClassesDetail> GetOtherClassDetailList(long Id)
        {
            return storedProcedureRepository.SP_ManageBusinessDetailByMenuTag_GetAll<BusinessAllClassesDetail>(new SP_ManageBusinessDetailByMenuTag_Params_VM()
            {
                Id = Id,
                Mode = 5
            });
        }


        #region Class-Feature --------------------------------------------------------------------------

        /// <summary>
        /// Get Class-Feature-Detail By Id
        /// </summary>
        /// <param name="classFeatureId">Class Feature Id</param>
        /// <returns>Single Class Feature Detail</returns>
        public ClassFeature_VM GetClassFeatureById(long classFeatureId)
        {
            SqlParameter[] queryParams_VM = new SqlParameter[] {
                                new SqlParameter("id", classFeatureId),
                                new SqlParameter("classId", "0"),
                                new SqlParameter("mode", "2"),
                 };
            return db.Database.SqlQuery<ClassFeature_VM>("exec sp_ManageClassFeature @id,@classId,@mode", queryParams_VM).FirstOrDefault();
        }

        #endregion -------------------------------------------------------------------------------------

        #region Batch ----------------------------------------------------------------------------------

        /// <summary>
        /// Add or Update Batch
        /// </summary>
        /// <param name="model">Stored Procedure Params</param>
        /// <returns>Response with status and message</returns>
        public SPResponseViewModel AddUpdateBatch(SP_InsertUpdateBatch_Params_VM model)
        {
            return storedProcedureRepository.SP_InsertUpdateBatch_Get<SPResponseViewModel>(model);
        }

        /// <summary>
        /// Get Batch Detail By Id
        /// </summary>
        /// <param name="id">Batch Id</param>
        /// <returns>Batch Detail</returns>
        public Batch_VM GetBatchById(long id)
        {
            return storedProcedureRepository.SP_ManageBatch_Get<Batch_VM>(new SP_ManageBatch_Params_VM { Id = id, Mode = 3 });
        }

        /// <summary>
        /// Get Batch List For Pagination - Jquery Datatable Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<Batch_Pagination_VM> GetBatchList_Pagination(NameValueCollection httpRequestParams, BatchList_Pagination_SQL_Params_VM _Params_VM)
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

            List<Batch_Pagination_VM> lstBatchRecords = new List<Batch_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("userLoginId", _Params_VM.UserLoginId),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessOwnerLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstBatchRecords = db.Database.SqlQuery<Batch_Pagination_VM>("exec sp_ManageBatch_Pagination @id,@userLoginId,@businessOwnerLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstBatchRecords.Count > 0 ? lstBatchRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Batch_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Batch_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstBatchRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Change Batch Status By Id
        /// </summary>
        /// <param name="id">Batch Id</param>
        /// <returns>Response with status and message</returns>
        public SPResponseViewModel ChangeBatchStatus(long id, long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageBatch_Get<SPResponseViewModel>(new SP_ManageBatch_Params_VM { Id = id, BusinessOwnerLoginId = businessOwnerLoginId, Mode = 4 });
        }

        /// <summary>
        /// Get Active Batch-Names for dropdown which are not linked to any(including this) class By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>List of Batches</returns>
        public List<Batch_DropdownList_VM> GetActiveBatchNotLinkedWithClass_DropdownList(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageBatch_GetAll<Batch_DropdownList_VM>(new SP_ManageBatch_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 5
            });
        }

        #endregion Batch -------------------------------------------------------------------------------

        #region Class-Batches --------------------------------------------------------------------------

        /// <summary>
        /// Insert or update Class-Batches
        /// </summary>
        /// <param name="params_VM">Parameters for Class-Batch</param>
        /// <returns>Returns +ve response if success otherwise -ve status with error message</returns>
        public SPResponseViewModel InsertUpdateClassBatches(SP_InsertUpdateClassBatches_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_InsertUpdateClassBatches_Get<SPResponseViewModel>(new SP_InsertUpdateClassBatches_Params_VM()
            {
                Id = params_VM.Id,
                BusinessOwnerLoginId = params_VM.BusinessOwnerLoginId,
                ClassId = params_VM.ClassId,
                BatchId = params_VM.BatchId,
                Mode = 1
            });
        }

        /// <summary>
        /// Delete all Class-Batches by Class-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="classId">Class-Id</param>
        /// <returns>Returns +ve response if success otherwise -ve status with error message</returns>
        public SPResponseViewModel DeleteClassBatchesByClass(long businessOwnerLoginId, long classId)
        {
            return storedProcedureRepository.SP_InsertUpdateClassBatches_Get<SPResponseViewModel>(new SP_InsertUpdateClassBatches_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                ClassId = classId,
                Mode = 2
            });
        }

        /// <summary>
        /// Get All Batches (linked with Class) by ClassId and Business
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="classId">Class-Id</param>
        /// <returns>All Class-Batches List</returns>
        public List<Batch_VM> GetAllClassBatchesByClass(long classId)
        {
            return storedProcedureRepository.SP_ManageClassBatches_GetAll<Batch_VM>(new SP_ManageClassBatches_Params_VM()
            {
                ClassId = classId,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All Active Batches (linked with Class) by ClassId and Business
        /// </summary>
        /// <param name="classId">Class-Id</param>
        /// <returns>All Class-Batches List</returns>
        public List<Batch_VM> GetAllActiveClassBatchesByClass(long classId)
        {
            return storedProcedureRepository.SP_ManageClassBatches_GetAll<Batch_VM>(new SP_ManageClassBatches_Params_VM()
            {
                ClassId = classId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get All Active Batches (linked with Class) by ClassId and Business For Dropdown
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="classId">Class-Id</param>
        /// <returns>All Class-Batches List</returns>
        public List<Batch_DropdownList_VM> GetAllActiveClassBatchesByClassForDropdown(long classId, long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageClassBatches_GetAll<Batch_DropdownList_VM>(new SP_ManageClassBatches_Params_VM()
            {
                ClassId = classId,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Active Batch-Names for dropdown which are not linked to other class (excluding current passed class) By Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="classId">Class-Id whose batches are included in list</param>
        /// <returns>List of Batches</returns>
        public List<Batch_DropdownList_VM> GetActiveBatchNotLinkedWithOtherClasses_DropdownList(long businessOwnerLoginId, long classId)
        {
            return storedProcedureRepository.SP_ManageClassBatches_GetAll<Batch_DropdownList_VM>(new SP_ManageClassBatches_Params_VM()
            {
                ClassId = classId,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 2
            });
        }


        #endregion Class-Batches -----------------------------------------------------------------------

        /// <summary>
        /// Business Content Class Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Class Detail</returns>
        public BusinessContentClassDetail_VM GetBusinessContentClassDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentClass_Get<BusinessContentClassDetail_VM>(new SP_ManageBusinessContentClass_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Business Content  World Class Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get  World Class Detail</returns>
        public BusinessContentWorldClassDetail_VM GetBusinessContentWorldClassDetail(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentWorldClass_Get<BusinessContentWorldClassDetail_VM>(new SP_ManageBusinessContentWorldClass_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// Business Content  World Class  Program Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get  World Class Program Detail</returns>
        public BusinessContentWorldClassProgramDetail_VM GetBusinessContentWorldClassProgramDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentWorldClassProgram_Get<BusinessContentWorldClassProgramDetail_VM>(new SP_ManageBusinessContentWorldClass_Param_VM
            {

                Id = Id,
                Mode = 1


            });

        }

        /// <summary>
        /// Business Content  World Class  Program Detail (List)
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get  World Class Program Detail</returns>
        public List<BusinessContentWorldClassProgramDetail_VM> GetBusinessContentWorldClassProgramDetaillst_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessContentWorldClassProgram_GetList<BusinessContentWorldClassProgramDetail_VM>(new SP_ManageBusinessContentWorldClass_Param_VM
            {

                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }


        /// <summary>
        /// World Class Program Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentWorldClassProgramDetail_Pagination_VM> GetBusinessContentWorldClassProgramList_Pagination(NameValueCollection httpRequestParams, BusinessContentWorldClassProgramDetail_Pagination_SQL_Params_VM _Params_VM)
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

            List<BusinessContentWorldClassProgramDetail_Pagination_VM> lstWorldClassProgramRecords = new List<BusinessContentWorldClassProgramDetail_Pagination_VM>();

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

            lstWorldClassProgramRecords = db.Database.SqlQuery<BusinessContentWorldClassProgramDetail_Pagination_VM>("exec sp_ManageBusinessContentWorldClassProgram_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstWorldClassProgramRecords.Count > 0 ? lstWorldClassProgramRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentWorldClassProgramDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentWorldClassProgramDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstWorldClassProgramRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }



        /// <summary>
        /// Business Content World Class Program Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content World Class Program  Detail</returns>
        public SPResponseViewModel DeleteWorldClassProgramDetail(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateBusinessContentWorldClassProgram_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentWorldClassProgram_Param_VM
            {
                Id = Id,
                Mode = 3


            });

        }


        /// <summary>
        /// To Get Classic Dance Timing Detail 
        /// </summary>
        /// <param name="ClassDays"></param>
        /// <returns></returns>
        public List<BusinssClassesTimingDetail> GetClassTimingDetailList(string ClassDays, long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessClassTimingDetail_GetLst<BusinssClassesTimingDetail>(new SP_ManageBusinessTimingDetail_Param_VM
            {
                ClassDays = ClassDays,
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }


        /// <summary>
        /// To get Category Class Detail 
        /// </summary>
        /// <returns></returns>
        public List<ClassCategoryDetailList_VM> GetBusinessClassCategoryDetail_Get(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageClass_GetAll<ClassCategoryDetailList_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 20
            });
        }

        /// <summary>
        /// To Get All Class Time Table Detail (For Using filter)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <param name="ClassClategoryTypeId"></param>
        /// <param name="InstructorLoginId"></param>
        /// <param name="ClassDays"></param>
        /// <returns></returns>
        public List<BusinssClassesTimingDetail> GetallClassDetailList(long BusinessOwnerLoginId, long ClassClategoryTypeId, long InstructorLoginId, string ClassDays,string ClassMode)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageAllClassTimeTableDetailList<BusinssClassesTimingDetail>(new SP_ManageAllClassTimeTableDetail_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                ClassCategoryTypeId = ClassClategoryTypeId,
                InstructorLoginId = InstructorLoginId,
                ClassDays = ClassDays,
                ClassMode = ClassMode,
                Mode = 1
            });

        }

        ///////////////////////////////////Online Classes /////////////////////////////////////
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Online Classes | Detail </returns>
        public List<BusinessContentClassesDetail_VM> GetBusinessContentOnlineClassDetaillst_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessClassDetail_GetLst<BusinessContentClassesDetail_VM>(new SP_ManageClass_Params_VM
            {

                UserLoginId = BusinessOwnerLoginId,
                Mode = 14


            });

        }
        ///////////////////////////////////Offline Classes /////////////////////////////////////
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Online Classes | Detail </returns>
        public List<BusinessContentClassesDetail_VM> GetBusinessContentOfflineClassDetaillst_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessClassDetail_GetLst<BusinessContentClassesDetail_VM>(new SP_ManageClass_Params_VM
            {

                UserLoginId = BusinessOwnerLoginId,
                Mode = 13


            });

        }

        ///////////////////////////////////Upcoming Classes /////////////////////////////////////
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Online Classes | Detail </returns>
        public List<BusinessContentClassesDetail_VM> GetBusinessContentUpcomingClassDetaillst_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessClassDetail_GetLst<BusinessContentClassesDetail_VM>(new SP_ManageClass_Params_VM
            {

                UserLoginId = BusinessOwnerLoginId,
                Mode = 12


            });

        }

        /// <summary>
        /// Get Classes List By Search Filter
        /// </summary>
        /// <param name="parmas_VM">Search-Filter Params</param>
        /// <returns>Filtered Classes list</returns>
        public List<ClassDetail_BySearchFilter_VP> GetClassesListBySearchFilter(SearchFilter_APIParmas_VM parmas_VM)
        {

            storedProcedureRepository = new StoredProcedureRepository(db);
            if (parmas_VM.UserLoginId > 0)
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<ClassDetail_BySearchFilter_VP>(new SP_GetRecordsBySearch_Params_VM()
                {
                    UserLoginId = parmas_VM.UserLoginId,
                    Mode = 6,
                });
            }
            else
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<ClassDetail_BySearchFilter_VP>(new SP_GetRecordsBySearch_Params_VM()
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
                    Mode = 5,
                    StartDate = parmas_VM.StartDate,
                    Days = parmas_VM.Days
                });
            }


        }

        /// <summary>
        /// to get batch details for android batch info section
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="id"></param>
        /// <param name="searchtext"></param>
        /// <returns></returns>
        public BatchInfo_VM GetBatchDetailsById(long businessOwnerLoginId, long id, string searchtext)
        {
            return storedProcedureRepository.SP_ManageBatch_Get<BatchInfo_VM>(new SP_ManageBatch_Params_VM()
            {
                SearchText = searchtext,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Id = id, // Pass id as the parameter
                Mode = 7
            });
        }

        /// <summary>
        ///  To Get Class Refer Detail By UserLoginId 
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public ClassReferDetail_VM GetClassReferDetails_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassReferDetail_Get<ClassReferDetail_VM>(new SP_ManageClassReferDetail_Param_VM
            {

                UserLoginId = BusinessOwnerLoginId,
                Mode = 1


            });

        }





        /// <summary>
        /// To Get Class Booking Detail 
        /// </summary>
        /// <param name="ClassDays"></param>
        /// <returns></returns>
        public List<ClassBooking_ViewModel> GetClassBookingDetailList(long id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassBooking_GetAll<ClassBooking_ViewModel>(new SP_ManageClassBooking_Params_VM
            {
                UserLoginId = id,
                Mode = 7
            });
        }


        /// <summary>
        /// get detail class booking by id for view modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClassBooking_ViewModel GetClassBookingDetailById(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassBooking_Get<ClassBooking_ViewModel>(new SP_ManageClassBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 8
            });
        }



        /// <summary>
        /// get detail class booking by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClassBooking_ViewModel GetBusinessClassBookingDetailById(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassBooking_Get<ClassBooking_ViewModel>(new SP_ManageClassBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 10
            });
        }

        /// <summary>
        /// To Get Business Class Detail By UserLoginId and Id  
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public ClassBooking_ViewModel GetBusinessClassBookingDetail(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassBooking_GetById<ClassBooking_ViewModel>(new SP_ManageClassBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 10
            });
        }
        /// <summary>
        /// To Get Business Class Boking Detail List (My Booking )
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ClassBooking_ViewModel> GetBusinessClassBookingDetailList(long id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassBooking_GetAll<ClassBooking_ViewModel>(new SP_ManageClassBooking_Params_VM
            {
                UserLoginId = id,
                Mode = 9
            });
        }

        /// <summary>
        /// To Get  Class  Detail List 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<ClassDetail_VM> GetClassDetailList(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClass_GetAll<ClassDetail_VM>(new SP_ManageClass_Params_VM
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 22
            });
        }


        /// <summary>
        /// get single class detail
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public ClassDetail_VM GetSingleClassDetail(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClass_Get<ClassDetail_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 22
            });
        }

        /// <summary>
        /// to get list of class
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentClassesDetail_VM> GetBusinessContentAllClassDetaillst_Get(long BusinessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageBusinessClassDetail_GetLst<BusinessContentClassesDetail_VM>(new SP_ManageClass_Params_VM
            {

                UserLoginId = BusinessOwnerLoginId,
                Mode = 24


            });

        }
        ///// <summary>
        ///// to get class details for dropdown of batch info (android)
        ///// </summary>
        ///// <param name="businessOwnerLoginId"></param>
        ///// <param name="id"></param>
        ///// <param name="searchtext"></param>
        ///// <returns></returns>
        //public List<Class_VM> GetclassDetailsById(long businessOwnerLoginId, long id, string searchtext)
        //{
        //    return storedProcedureRepository.SP_ManageBatch_GetAll<Class_VM>(new SP_ManageBatch_Params_VM()
        //    {
        //        SearchText = searchtext,
        //        BusinessOwnerLoginId = businessOwnerLoginId,
        //        Id = id, // Pass id as the parameter
        //        Mode = 7
        //    });
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="id"></param>
        /// <param name="searchtext"></param>
        /// <returns></returns>
        public InstructorInfo_VM GetcertificateDetailsById(long businessOwnerLoginId, long id,string searchtext)
        {
            return storedProcedureRepository.SP_ManageCertificate_Get<InstructorInfo_VM>(new SP_ManageCertificate_Params_VM()
            {
                Searchtext = searchtext,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Id = id, // Pass id as the parameter
                Mode = 10
            });
        }
        /// <summary>
        /// to get classes details by instructor in masterprofileapi profile page 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<ClassdetailsbyInstructor_VM> GetClassListbyInstuctor(long businessOwnerLoginId,long id,string dayname)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClass_GetAll<ClassdetailsbyInstructor_VM>(new SP_ManageClass_Params_VM
            {
                dayNames= dayname,
                Id = id,
                UserLoginId = businessOwnerLoginId,
                Mode = 25
            });
        }
        /// <summary>
        /// to get class list by instructor
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<ClassDetail_VM> GetClassListByInstructor(long businessOwnerLoginId,long id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClass_GetAll<ClassDetail_VM>(new SP_ManageClass_Params_VM
            {
                Id = id,
                UserLoginId = businessOwnerLoginId,
                Mode = 26
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public ClassIntermediatesDetail_VM GetClassIntermediateDetail(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageClassInterMediate_Get<ClassIntermediatesDetail_VM>(new SP_ManageClassIntermediatesDetail_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 2
            });
        }
    }
}