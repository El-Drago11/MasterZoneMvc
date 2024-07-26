using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
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
    public class StudentService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public StudentService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get Student List with Instructor and Class  [Jquery Data-Table pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<StudentInstructor_Pagination_VM> GetBusinessStudentListWithClassInstructor_Pagination(NameValueCollection httpRequestParams, StudentList_Pagination_SQL_Params_VM _Params_VM)
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


            List<StudentInstructor_Pagination_VM> lstStudentRecords = new List<StudentInstructor_Pagination_VM>();

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

            lstStudentRecords = db.Database.SqlQuery<StudentInstructor_Pagination_VM>("exec sp_ManageStudent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<StudentInstructor_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentInstructor_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Request InsertUpdate Student Stored-Procedure
        /// </summary>
        /// <param name="params_VM">Stored-Procedure Parameters</param>
        /// <returns>Response Object having status</returns>
        public SPResponseViewModel_UserAddUpdate InsertUpdateStudent(SP_InsertUpdateStudent_Params_VM params_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            if (params_VM.Mode == 1)
            {
                // Generate Unique-User-Id
                UserLoginService userLoginService = new UserLoginService(db);
                params_VM.UniqueUserId = userLoginService.GenerateRandomUniqueUserId(StaticResources.UserId_Prefix_StudentUser);
            }

            return storedProcedureRepository.SP_InsertUpdateStudent<SPResponseViewModel_UserAddUpdate>(params_VM);
        }

        /// <summary>
        /// Get Student Active(Enrolled) course/class list
        /// </summary>
        /// <param name="userLoginId">Student-User-Login-Id</param>
        /// <param name="lastRecordId">Last-Fetched-Record-Id</param>
        /// <param name="recordLimit">No. of records to return</param>
        /// <returns>List of Student Active Courses</returns>
        public List<ClassBookingList_VM> GetActiveCourseList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentCourseDetail_GetAll<ClassBookingList_VM>(new SP_GetAllStudentCourseDetail_Param_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });
        }

        /// <summary>
        /// Get Student Ended(Enrolled and expired) course/class list
        /// </summary>
        /// <param name="userLoginId">Student-User-Login-Id</param>
        /// <param name="lastRecordId">Last-Fetched-Record-Id</param>
        /// <param name="recordLimit">No. of records to return</param>
        /// <returns>List of Student Expired Courses</returns>
        public List<ClassBookingList_VM> GetEndedCourseList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentCourseDetail_GetAll<ClassBookingList_VM>(new SP_GetAllStudentCourseDetail_Param_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 2
            });
        }

        public JqueryDataTable_Pagination_Response_VM<StudentUser_Pagination_VM> GetStudentUserList_Pagination(NameValueCollection httpRequestParams, StudentUser_Pagination_SQL_Params_VM _Params_VM)
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

            List<StudentUser_Pagination_VM> lstStudentUserRecords = new List<StudentUser_Pagination_VM>();

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

            lstStudentUserRecords = db.Database.SqlQuery<StudentUser_Pagination_VM>("exec sp_ManageUserAccount_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentUserRecords.Count > 0 ? lstStudentUserRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<StudentUser_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentUser_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentUserRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Student User by User-Login-Id
        /// </summary>
        /// <param name="studentLoginId">student Login-Id</param>
        /// <returns>Student Detail</returns>
        public StudentUserDetail_VM GetStudentUserById(long studentLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_Get<StudentUserDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentLoginId,
                UserLoginId = studentLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Student  User  course detail by Id
        /// </summary>
        /// <param name="studentId">student Id</param>
        /// <returns>User Course Detail</returns>
        public List<StudentCourseDetail_VM> GetStudentCourseDetailById(long studentId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_GetAll<StudentCourseDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentId,
                Mode = 6
            });
        }

        /// <summary>
        /// Get All User Content-Image by Id
        /// </summary>
        /// <param name="studentId">student Id</param>
        /// <returns> User Conent-Image List</returns>
        public List<UserImageDetail_VM> GetAllUserContentImagesById(long studentId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_GetAll<UserImageDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentId,
                Mode = 7
            });
        }

        /// <summary>
        /// Get All User Plan details by Id
        /// </summary>
        /// <param name="studentId">student Id</param>
        /// <returns>User Plan List</returns>
        public List<UserPlanDetail_VM> GetAllUserPlanDetailById(long studentId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_GetAll<UserPlanDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentId,
                Mode = 8
            });
        }

        /// <summary>
        /// Get All User Training detail by Id
        /// </summary>
        /// <param name="studentId">student Id</param>
        /// <returns> User Training List</returns>
        public List<UserTrainingsDetail_VM> GetAllUserTrainingDetailById(long studentId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_GetAll<UserTrainingsDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentId,
                Mode = 9
            });
        }

        /// <summary>
        /// Get All User-Content Videos detail by Id
        /// </summary>
        /// <param name="studentId">student Id</param>
        /// <returns> User Content-Videos List</returns>
        public List<UserContentVedio_VM> GetAllUserContentVedioDetailById(long studentId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_GetAll<UserContentVedio_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentId,
                Mode = 10
            });
        }

        /// <summary>
        /// Get All User Certificate detail by Id
        /// </summary>
        /// <param name="studentId">student Id</param>
        /// <returns> User Certificates List</returns>
        public List<UserCertificateDetail_VM> GetAllUserCertificateDetailById(long studentId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageStudent_GetAll<UserCertificateDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                Id = studentId,
                Mode = 13
            });
        }

        /// <summary>
        /// Block Student
        /// </summary>
        /// <param name="Id">Student-Id</param>
        /// <param name="blockReason">Reson for block</param>
        /// <returns>Status 1 if updated, else -ve value with message</returns>
        public SPResponseViewModel BlockStudent(long Id, string blockReason)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateStudent<SPResponseViewModel>(new SP_InsertUpdateStudent_Params_VM()
            {
                Id = Id,
                BlockReason = blockReason,
                Mode = 3
            });
        }

        /// <summary>
        /// Un-Block Student
        /// </summary>
        /// <param name="Id">Student-Id</param>
        /// <returns>Status 1 if updated, else -ve value with message</returns>
        public SPResponseViewModel UnBlockStudent(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateStudent<SPResponseViewModel>(new SP_InsertUpdateStudent_Params_VM()
            {
                Id = Id,
                Mode = 4
            });
        }

        /// <summary>
        /// Delete User-Content-Image by Id
        /// </summary>
        /// <param name="Id">Content-Image-Id</param>
        /// <param name="UserLoginId">Student-User-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel DeleteUserContentImage(long Id, long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_Get<SPResponseViewModel>(new SP_ManageStudent_Params_VM()
            {
                Id = Id,
                UserLoginId = UserLoginId,
                Mode = 11
            });
        }

        /// <summary>
        /// Delete  User-Content-Vedio by Id
        /// </summary>
        /// <param name="Id">Content-Video-Id</param>
        /// <param name="UserLoginId">Student-User-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel DeleteUserContentVedio(long Id, long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_Get<SPResponseViewModel>(new SP_ManageStudent_Params_VM()
            {
                Id = Id,
                UserLoginId = UserLoginId,
                Mode = 12
            });
        }

        /// <summary>
        /// Randomly Student Training Detail 
        /// </summary>
        /// <returns></returns>
        public StudentTrainingDetail_VM GetTrainingDetailrandomly(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_Get<StudentTrainingDetail_VM>(new SP_ManageStudent_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 15
            });

        }
        /// <summary>
        /// Randomly Student Training Detail 
        /// </summary>
        /// <returns></returns>
        public List<StudentBusinessTrainingDetail_VM> GetStudentBusinessDetail(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_GetAll<StudentBusinessTrainingDetail_VM>(new SP_ManageStudent_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 16
            });

        }
        /// <summary>
        /// Business  Training Detail By BusinessId
        /// </summary>
        /// <returns></returns>
        public StudentBusinessTrainingDetail_VM GetStudentBusinessDetailByBusinessId(long UserLoginId, long businessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_Get<StudentBusinessTrainingDetail_VM>(new SP_ManageStudent_Params_VM
            {
                UserLoginId = UserLoginId,
                BusinessOwnerLoginId = businessLoginId,
                Mode = 17
            });

        }

        /// <summary>
        /// Business  Training Detail By BusinessId
        /// </summary>
        /// <returns></returns>
        public List<StudentTrainingDetail_VM> GetStudentBusinessTrainingDetail(long UserLoginId, long businessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_GetAll<StudentTrainingDetail_VM>(new SP_ManageStudent_Params_VM
            {

                UserLoginId = UserLoginId,
                BusinessOwnerLoginId = businessLoginId,
                Mode = 14


            });

        }

        /// <summary>
        /// Get Student-User Basic Detail By MasterId 
        /// </summary>
        /// <param name="MasterId"></param>
        /// <returns></returns>
        public StudentBasicDetail_VM GetStudentBasicDetailDataByMasterId(string MasterId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetDetailByMasterId_Get<StudentBasicDetail_VM>(new SP_GetDetailByMasterId_Params_VM()
            {
                MasterId = MasterId,
                Mode = 1
            });
        }

        /// <summary>
        /// Get Student-User Basic Detail By Id 
        /// </summary>
        /// <param name="userLoginId">User-Login-Id</param>
        /// <returns>Student User Data</returns>
        public StudentBasicDetail_VM GetStudentBasicDetailDataByLoginId(long userLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudent_Get<StudentBasicDetail_VM>(new SP_ManageStudent_Params_VM()
            {
                UserLoginId = userLoginId,
                Mode = 18
            });
        }

        /// <summary>
        /// Get Student Class List for Business [Jquery Data-Table pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Joined/Booked Classes list of Business By student</returns>
        public JqueryDataTable_Pagination_Response_VM<StudentClassForBO_Pagination_VM> GetBusinessStudentClassesList_Pagination(NameValueCollection httpRequestParams, StudentList_Pagination_SQL_Params_VM _Params_VM)
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


            List<StudentClassForBO_Pagination_VM> lstStudentRecords = new List<StudentClassForBO_Pagination_VM>();

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

            lstStudentRecords = db.Database.SqlQuery<StudentClassForBO_Pagination_VM>("exec sp_ManageStudent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<StudentClassForBO_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentClassForBO_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Student Plan List for Business [Jquery Data-Table pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Joined/Booked Plans list of Business By student</returns>
        public JqueryDataTable_Pagination_Response_VM<StudentPlanForBO_Pagination_VM> GetBusinessStudentPlanList_Pagination(NameValueCollection httpRequestParams, StudentList_Pagination_SQL_Params_VM _Params_VM)
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


            List<StudentPlanForBO_Pagination_VM> lstStudentRecords = new List<StudentPlanForBO_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "3")
                            };

            lstStudentRecords = db.Database.SqlQuery<StudentPlanForBO_Pagination_VM>("exec sp_ManageStudent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<StudentPlanForBO_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentPlanForBO_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Student Event List for Business [Jquery Data-Table pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Joined/Booked Event list of Business By student</returns>
        public JqueryDataTable_Pagination_Response_VM<StudentEventForBO_Pagination_VM> GetBusinessStudentEventList_Pagination(NameValueCollection httpRequestParams, StudentList_Pagination_SQL_Params_VM _Params_VM)
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


            List<StudentEventForBO_Pagination_VM> lstStudentRecords = new List<StudentEventForBO_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "4")
                            };

            lstStudentRecords = db.Database.SqlQuery<StudentEventForBO_Pagination_VM>("exec sp_ManageStudent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<StudentEventForBO_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentEventForBO_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Student Training List for Business [Jquery Data-Table pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns>Joined/Booked Training list of Business By student</returns>
        public JqueryDataTable_Pagination_Response_VM<StudentTrainingForBO_Pagination_VM> GetBusinessStudentTrainingList_Pagination(NameValueCollection httpRequestParams, StudentList_Pagination_SQL_Params_VM _Params_VM)
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


            List<StudentTrainingForBO_Pagination_VM> lstStudentRecords = new List<StudentTrainingForBO_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "5")
                            };

            lstStudentRecords = db.Database.SqlQuery<StudentTrainingForBO_Pagination_VM>("exec sp_ManageStudent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstStudentRecords.Count > 0 ? lstStudentRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<StudentTrainingForBO_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<StudentTrainingForBO_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstStudentRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }
        /// <summary>
        /// to get plan date 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<PlanBooking_ViewModel> GetPlanDetailForDate(long businessOwnerLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", businessOwnerLoginId),
                             new SqlParameter("planId", "0"),
                            new SqlParameter("mode", "7")
                            };

            var response = db.Database.SqlQuery<PlanBooking_ViewModel>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).ToList();
            return response;
        }

        /// <summary>
        /// To get training list for student panel
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<AvailableTraining_VM> GetActiveTrainingList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentTrainingDetail_GetAll<AvailableTraining_VM>(new SP_GetAllTrainingDetailSearch_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });
        }
        /// <summary>
        ///  to get endend training list
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<AvailableTraining_VM> GetEndedTrainingList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentTrainingDetail_GetAll<AvailableTraining_VM>(new SP_GetAllTrainingDetailSearch_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 2
            });
        }
        /// <summary>
        /// to get event details for student panel 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<EventBookingDetail_VM> GetActiveEventList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentEventDetail_GetAll<EventBookingDetail_VM>(new SP_GetAllStudentEventList_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });
        }

        /// <summary>
        /// to get sponser , guests details for eventdetails
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<EventBookingDetail_VM> GetEventDetailsGuestList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentEventDetail_GetAll<EventBookingDetail_VM>(new SP_GetAllStudentEventList_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 5
            });
        }
        /// <summary>
        /// to get event sponsers details list
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<EventBookingDetail_VM> GetEventSponsersList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentEventDetail_GetAll<EventBookingDetail_VM>(new SP_GetAllStudentEventList_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 6
            });
        }
        /// <summary>
        /// to get evenet
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public EventBookingDetail_VM GetEventTicketCount(long userLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageStudentEventDetail_Get<EventBookingDetail_VM>(new SP_GetAllStudentEventList_Params_VM
            {
                UserLoginId = userLoginId,
                Mode = 7
            });
        }
        /// <summary>
        /// to get endend event list
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<EventBookingDetail_VM> GetEndendEventList(long userLoginId, long lastRecordId, int recordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_GetAllStudentEventDetail_GetAll<EventBookingDetail_VM>(new SP_GetAllStudentEventList_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 2
            });
        }
        /// <summary>
        /// to verify otp when user signing
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public SPResponseViewModel_UserAddUpdate InsertUpdateStudentDb(SP_InsertUpdateStudent_Params_VM params_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_VerifyOtpStudent<SPResponseViewModel_UserAddUpdate>(params_VM);
        }
    }
}