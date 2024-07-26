using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.Views;
using Org.BouncyCastle.Asn1.BC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using MasterZoneMvc.Common;
using System.Windows;
using MasterZoneMvc.PageTemplateViewModels;

namespace MasterZoneMvc.Services
{
    public class TrainingService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public TrainingService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        public JqueryDataTable_Pagination_Response_VM<Training_Pagination_VM> GetBusinessInstructorList_Pagination(NameValueCollection httpRequestParams, Training_Pagination_SQL_Params_VM _Params_VM)
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

            List<Training_Pagination_VM> lstStudentRecords = new List<Training_Pagination_VM>();

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

            lstStudentRecords = db.Database.SqlQuery<Training_Pagination_VM>("exec sp_ManageTraining_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Training_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Training_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        public JqueryDataTable_Pagination_Response_VM<Training_Pagination_VM_SuperAdminPanel> GetAllTrainingsList_Pagination_ForSuperAdminPanel(NameValueCollection httpRequestParams, Training_Pagination_SQL_Params_VM _Params_VM)
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

            List<Training_Pagination_VM_SuperAdminPanel> lstStudentRecords = new List<Training_Pagination_VM_SuperAdminPanel>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "2")
                            };

            lstStudentRecords = db.Database.SqlQuery<Training_Pagination_VM_SuperAdminPanel>("exec sp_ManageTraining_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Training_Pagination_VM_SuperAdminPanel> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Training_Pagination_VM_SuperAdminPanel>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get All Trainings for Visitor/Home view. Also can filter by location City or lat/long
        /// </summary>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Record Id of record</param>
        /// <param name="recordLimit">no. of next records to fetch</param>
        /// <returns>List of Tranings</returns>
        public List<TrainingList_ForVisitorPanel_VM> GetAllTrainingList(string City, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetAllBusinessTraining_GetAll<TrainingList_ForVisitorPanel_VM>(new SP_GetTrainingRecord_Params_VM
            {
                City = City,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All Trainings for Home Page.
        /// </summary>
        /// <returns>List of Tranings</returns>
        public List<TrainingList_ForHomePage_VM> GetAllTrainingListForHomePage()
        {
            return storedProcedureRepository.SP_ManageTraining_GetAll<TrainingList_ForHomePage_VM>(new SP_ManageTraining_Params_VM()
            {
                Mode = 11
            });
        }

        /// <summary>
        /// Get Training Table Data by Id
        /// </summary>
        /// <param name="trainingId">Training Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public TrainingViewModel GetTrainingDataById(long trainingId)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", trainingId),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "3")
                            };

            return db.Database.SqlQuery<TrainingViewModel>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        //public List<UserTrainingsDetail_VM> GetAllTraining(long lastRecordId, int recordLimit)
        //{
        //    return storedPorcedureRepository.SP_GetAllTraining_Get<UserTrainingsDetail_VM>(new Sp_ManageTraining_List_VM
        //    {
        //        LastRecordId = lastRecordId,
        //        RecordLimit = recordLimit,
        //    });

        //}

        /// <summary>
        /// To Get Training Detail By Menu-Tag
        /// </summary>
        /// <returns>Trainings list by Menu-Tag</returns>
        public List<TrainingDetail_VisitorPanel_VM> GetTrainingDetailListByMenuTag(string MenuTag, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_ManageBusinessDetailByMenuTag_GetAll<TrainingDetail_VisitorPanel_VM>(new SP_ManageBusinessDetailByMenuTag_Params_VM()
            {
                MenuTag = MenuTag,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 3
            });
        }

        /// <summary>
        /// To Get Instructor Detail By Menu-Tag
        /// </summary>
        /// <returns>Instructors List by Menu-Tag</returns>
        public List<InstructorList_VM> GetAllInstructorDetailByMenuTag(string MenuTag, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_ManageBusinessDetailByMenuTag_GetAll<InstructorList_VM>(new SP_ManageBusinessDetailByMenuTag_Params_VM()
            {
                MenuTag = MenuTag,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 4
            });
        }

        /// <summary>
        /// Training Record
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<UserTrainingsDetail_VM> GetAllTraining(long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetAllTrainingDetailSearch_GetList<UserTrainingsDetail_VM>(new SP_GetAllTrainingDetailSearch_Params_VM
            {
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });

        }

        /// <summary>
        /// Training Record By Location
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<TrainingList_ForVisitorPanel_VM> GetTrainingListByLocation(string searchkeyword = "", string searchBy = "", long lastRecordId = 0, int recordLimit = 10)
        {
            return storedProcedureRepository.SP_GetAllTrainingDetailSearch_GetList<TrainingList_ForVisitorPanel_VM>(new SP_GetAllTrainingDetailSearch_Params_VM
            {
                Searchkeyword = searchkeyword,
                SearchBy = searchBy,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 2
            });

        }

        /// <summary>
        /// Get Training Detail  by Id -In certification 
        /// <param name="trainingId">training Id</param>
        /// <returns>Status 1 To training Detail, else -ve value with message</returns>
        public TrainingDetail_VisitorPanel_VM GetTrainingDetailsById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTraining_Get<TrainingDetail_VisitorPanel_VM>(new SP_ManageTraining_Params_VM()
            {
                Id = Id,
                Mode = 7
            });
        }

        /// <summary>
        /// Get Training  Instructor Detail  by Id -For certification 
        /// </summary>
        /// <param name="trainingId">Enquiry Id</param>
        /// <returns>Status 1 To training Instructor Detail, else -ve value with message</returns>
        public TrainingDetail_VisitorPanel_VM GetTrainingInstructorDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTraining_Get<TrainingDetail_VisitorPanel_VM>(new SP_ManageTraining_Params_VM()
            {
                Id = Id,
                Mode = 8
            });
        }

        /// <summary>
        /// Get Training  Description Detail  by Id -For certification 
        /// </summary>
        /// <param name="trainingId">Enquiry Id</param>
        /// <returns>Status 1 To training Instructor Detail, else -ve value with message</returns>
        public TrainingDescriptionList_VM GetTrainingDescriptionById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTraining_Get<TrainingDescriptionList_VM>(new SP_ManageTraining_Params_VM()
            {
                Id = Id,
                Mode = 9
            });
        }

        /// <summary>
        /// Verify if Training has already been purchased by User [Paid Order Status]
        /// </summary>
        /// <param name="trainingId">Training Id</param>
        /// <param name="userLoginId">Logged In User-Login-Id (Buyer)</param>
        /// <returns>
        /// Returns true if already purchased, and false if not purchased
        /// </returns>
        public bool IsAlreadyTrainingPurchased(long trainingId, long userLoginId)
        {
            // Get Training-Booking-Table-Data Detail having Order
            var response = storedProcedureRepository.SP_ManageTrainingBooking_Get<TrainingBooking>(new SP_ManageTrainingBooking_Params_VM()
            {
                UserLoginId = userLoginId,
                TrainingId = trainingId,
                Mode = 1
            });

            if (response == null)
            {
                return false;
            }
            else
            {
                DateTime _TrainingEndDate = DateTime.ParseExact(response.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (_TrainingEndDate < DateTime.UtcNow.Date)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get All Training Bookings by Training-Id for Business-Panel.
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Training booking list</returns>
        public JqueryDataTable_Pagination_Response_VM<TrainingBooking_Pagination_BO_VM> GetTrainingBookingList_Pagination(NameValueCollection httpRequestParams, Training_Pagination_SQL_Params_VM _Params_VM)
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

            List<TrainingBooking_Pagination_BO_VM> lstStudentRecords = new List<TrainingBooking_Pagination_BO_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("trainingId", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstStudentRecords = db.Database.SqlQuery<TrainingBooking_Pagination_BO_VM>("exec sp_ManageTrainingBooking_Pagination @id,@trainingId,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<TrainingBooking_Pagination_BO_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<TrainingBooking_Pagination_BO_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get All Available Trainings for booking by Business
        /// </summary>
        /// <param name="trainingBookingId">Business-Owner-Login-Id</param>
        /// <returns>Returns list of trainings</returns>
        public List<AvailableTraining_VM> GetAllAvailableTrainingForBooking(long trainingBookingId)
        {
            return storedProcedureRepository.SP_ManageTraining_GetAll<AvailableTraining_VM>(new SP_ManageTraining_Params_VM()
            {
                BusinessOwnerLoginId = trainingBookingId,
                Mode = 10
            });
        }

        /// <summary>
        /// Get Training Booking Detail By ID
        /// </summary>
        /// <param name="trainingBookingId">Training-Booking-Id</param>
        /// <returns>Returns detail of Training Booking</returns>
        public TrainingBookingDetail_VM GetTrainingBookingDetailById(long trainingBookingId)
        {
            return storedProcedureRepository.SP_ManageTrainingBooking_Get<TrainingBookingDetail_VM>(new SP_ManageTrainingBooking_Params_VM()
            {
                Id = trainingBookingId,
                Mode = 2
            });
        }

        /// <summary>
        /// Mark Training Completed Only by Training-Booking-Id
        /// </summary>
        /// <param name="trainingBookingId">Training-Booking-Id</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel MarkTrainingCompletedById(long trainingBookingId)
        {
            return storedProcedureRepository.SP_InsertUpdateTrainingBooking_Get<SPResponseViewModel>(new SP_InsertUpdateTrainingBooking_Params_VM()
            {
                Id = trainingBookingId,
                Mode = 2
            });
        }


        /// <summary>
        /// Mark Training Completed And Generate Certificate by Training-Booking-Id
        /// </summary>
        /// <param name="trainingBookingId">Training-Booking-Id</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel CompleteTrainingAndGenerateCertificateById(long trainingBookingId)
        {
            // Mark Complete
            var respMarkComplete = MarkTrainingCompletedById(trainingBookingId);

            if (respMarkComplete == null || respMarkComplete.ret <= 0)
            {
                return respMarkComplete;
            }

            // Generate Certificate 
            return null;
        }

        public SPResponseViewModel GenerateTrainingCertificate(long trainingBookingId)
        {
            // Get Training-Booking Detail
            var trainingBooking = GetTrainingBookingDetailById(trainingBookingId);

            if (trainingBooking == null)
            {
                return new SPResponseViewModel() { ret = -1, resourceKey = "InvalidIdErrorMessage", resourceFileName = "ErrorMessage", responseMessage = "Invalid Id! No data found!" };
            }

            long licenseId = trainingBooking.LicenseId;

            StudentService studentService = new StudentService(db);
            var studentDetail = studentService.GetStudentUserById(trainingBooking.UserLoginId);

            LicenseService licenseService = new LicenseService(db);
            var licenseData = licenseService.GetLicenseRecordDataById(licenseId);

            CertificateService certificateService = new CertificateService(db);
            var certificateData = certificateService.GetCertificateById(licenseData.CertificateId);

            // Bind Abbrivations with data
            var bindedContent = LicenseCertificateGenerator.BindValuesInCertificateHTML(licenseData.LicenseCertificateHTMLContent, new LicenseCertificateHTMLContent_VM()
            {
                UserFirstName = studentDetail.FirstName,
                UserLastName = studentDetail.LastName,
                CertificateId = licenseData.CertificateId,
                CertificateLogoPath = certificateData.CertificateIconWithPath,
                CertificateTitle = certificateData.Name,
                LicenseId = licenseId,
                LicenseLogoPath = licenseData.LicenseLogoImageWithPath,
                LicenseTitle = licenseData.Title,
                LicenseDescription = licenseData.Description,
                Signature1Path = licenseData.SignatureImageWithPath,
                Signature2Path = licenseData.Signature2ImageWithPath,
                Signature3Path = licenseData.Signature3ImageWithPath,
                TimePeriod = licenseData.TimePeriod,
                UniqueCertificateNumber = "PREV_1234",
                IssueDate = DateTime.UtcNow,
                IssueDate_Format = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            });

            var fileName = FileHelper.GenerateFileNameTimeStamp("Certificate", ".pdf");

            // Generate and save PDF document
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(bindedContent);
                Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();

                // Save PDF
                byte[] bytes = stream.ToArray();
                var path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(StaticResources.FileUploadPath_Certificate), fileName);
                System.IO.File.WriteAllBytes(path, bytes);
            }

            // Save Certificate Details in database. and mark certificate Complete. [Transaction Management]

            return null;
        }

        /// <summary>
        /// Get Trainings List By Search Filter
        /// </summary>
        /// <param name="parmas_VM">Search-Filter Params</param>
        /// <returns>Filtered Trainings list</returns>
        public List<TrainingDetail_VisitorPanel_VM> GetTrainingsListBySearchFilter(SearchFilter_APIParmas_VM parmas_VM)
        {
            if(parmas_VM.UserLoginId > 0)
            {
                return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<TrainingDetail_VisitorPanel_VM>(new SP_GetRecordsBySearch_Params_VM()
                {
                    UserLoginId = parmas_VM.UserLoginId,
                    Mode = 8
                });
            }
            else
            {
            return storedProcedureRepository.SP_GetRecordsBySearch_GetAll<TrainingDetail_VisitorPanel_VM>(new SP_GetRecordsBySearch_Params_VM()
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
                Mode = 4,
                StartDate = parmas_VM.StartDate,
            });
            }
        }


        /// <summary>
        /// To Get Without Licensed Training Detail 
        /// </summary>
        /// <param name="trainingBookingId"></param>
        /// <returns></returns>
        public List<BusinessTrainingDetail_VM> GetAllTrainingDetailForBusiness(long businessOnwerLoginId)
        {
            return storedProcedureRepository.SP_ManageTraining_GetAll<BusinessTrainingDetail_VM>(new SP_ManageTraining_Params_VM()
            {
                UserLoginId = businessOnwerLoginId,
                Mode = 12
            });
        }

        /// <summary>
        /// To Get With Licensed Certificate Training Detail List 
        /// </summary>
        /// <param name="trainingBookingId"></param>
        /// <returns></returns>
        public List<BusinessTrainingDetail_VM> GetAllTrainingLicensedDetailForBusiness(long businessOnwerLoginId)
        {
            return storedProcedureRepository.SP_ManageTraining_GetAll<BusinessTrainingDetail_VM>(new SP_ManageTraining_Params_VM()
            {
                UserLoginId = businessOnwerLoginId,
                Mode = 13
            });
        }



        /// <summary>
        /// get training boking details by id and userloginid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public TrainingBooking_ViewModel GetTrainingBookingDetailbyId(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTrainingBooking_Get<TrainingBooking_ViewModel>(new SP_ManageTrainingBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 4
            });
        }

        /// <summary>
        /// get training details by list to single
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>

        public TrainingBooking_ViewModel GetBusinessTrainingBookingDetailById(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTrainingBooking_Get<TrainingBooking_ViewModel>(new SP_ManageTrainingBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 6
            });
        }

        /// <summary>
        /// get training boking details by id and userloginid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public TrainingBooking_ViewModel GetBusinessTrainingBookingDetail(long id, long userloginid)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageTrainingBooking_GetByID<TrainingBooking_ViewModel>(new SP_ManageTrainingBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 6
            });
        }




    }

}