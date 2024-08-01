using MasterZoneMvc.DAL;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using static MasterZoneMvc.ViewModels.CourseViewModel;
using MasterZoneMvc.ViewModels;
using System.Collections.Specialized;
using System.Data.SqlClient;
using MasterZoneMvc.Models;

namespace MasterZoneMvc.Services
{
    public class CourseService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public CourseService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// To Get Course Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public CourseDetail_VM GetCourseDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetail_Get<CourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                Id = Id,
                Mode = 1


            });

        }

        /// <summary>
        /// To Get Business Course Detail By Course Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public BusinessCourseDetail_VM GetBusinessCourseDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetail_Get<BusinessCourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                Id = Id,
                Mode = 5


            });

        }

        /// <summary>
        /// To Get Other Course Detail By Course Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<BusinessCourseDetail_VM> GetBusinessOtherCourseDetail(long Id ,long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetailList_Get<BusinessCourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                Id = Id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 6

            });

        }


        /// <summary>
        /// To Get Course Detail List By BusinessOwnerLoginid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<BusinessCourseDetail_VM> GetCourseDetailList(long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetailList_Get<BusinessCourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 4


            });

        }

        /// <summary>
        /// Get course List Detail For Detail Page 
        /// </summary>
        /// <returns></returns>

        public List<BusinessCourseDetail_VM> GetCourseCategoryList(long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetailList_Get<BusinessCourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 7

            });

        }



        /// <summary>
        /// To Delete Course Detail by Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBusinessCourseDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdateCourse_Param_Get<SPResponseViewModel>(new SP_InsertUpdateCourse_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }

        /// <summary>
        /// To change the Course Status Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SPResponseViewModel ChangeStatusCourseDetail(long id)
        {
            return storedPorcedureRepository.SP_InsertUpdateCourse_Param_Get<SPResponseViewModel>(new SP_InsertUpdateCourse_Param_VM()
            {
                Id = id,
                Mode = 4
            });
        }


        /// <summary>
        /// To Get Course Detail by Pagination
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<CourseDetail_ByPagination> GetBusinessCourseListDetail_Pagination(NameValueCollection httpRequestParams, CourseList_Pagination_SQL_Params_VM _Params_VM)
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


            List<CourseDetail_ByPagination> lstCourseRecords = new List<CourseDetail_ByPagination>();

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

            lstCourseRecords = db.Database.SqlQuery<CourseDetail_ByPagination>("exec sp_ManageCourse_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstCourseRecords.Count > 0 ? lstCourseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<CourseDetail_ByPagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<CourseDetail_ByPagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstCourseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// get  course details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CourseDetail_VM GetCourseDataByID(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", id),
                    new SqlParameter("businessOwnerLoginId", "0"),
                    new SqlParameter("userLoginId", "0"),
                    new SqlParameter("mode", "3")
                    };

            return db.Database.SqlQuery<CourseDetail_VM>("exec sp_ManageCourseDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
        }
        /// <summary>
        /// To verify that is already course purchased
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userLoginId"></param>
        /// <param name="joinDate"></param>
        /// <returns></returns>

        public bool IsAlreadyCoursePurchased(long courseId, long userLoginId, string joinDate)
        {

            var response = storedPorcedureRepository.SP_ManageCourseBooking_GetAll<CourseBooking>(new SP_ManageCourseBooking_Params_VM()
            {
                StudentUserLoginId = userLoginId,
                CourseId = courseId,
                UserLoginId = userLoginId, //added
                Mode = 1
            });

            if (response == null || response.Count() <= 0)
            {
                return false;
            }
            else
            {
                //DateTime _CourseJoinDate = DateTime.ParseExact(joinDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                // check if same data exists in already purchased course.
                //if (response[0].CoursePriceType == "per_course" || response[0].CoursePriceType == "demo")
                //{
                //    foreach (var item in response)
                //    {
                //        if (item.CourseId == courseId)
                //        {
                //            return true;
                //        }
                //    };
                //}
                //else if (response[0].CoursePriceType == "per_month")
                //{
                //    foreach (var item in response)
                //    {
                //        if (courseId <= item.CourseId)
                //        {
                //            return true;
                //        }
                //    }
                //}
                //else if (response[0].CoursePriceType == "0") //added
                //{
                //    foreach (var item in response)
                //    {
                //        if (courseId <= item.CourseId)
                //        {
                //            return true;
                //        }
                //    }
                //}
                foreach (var item in response)
                {
                    if (item.CourseId == courseId)
                    {
                        return true;
                    }
                };
            }

            return false;
        }


        /// <summary>
        /// To Get Business Course Detail List 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<CourseBooking_ViewModel> GetCourseBookingDetail(long userLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseBooking_GetAll<CourseBooking_ViewModel>(new SP_ManageCourseBooking_Params_VM
            {

                UserLoginId = userLoginId,
                Mode = 6
            });

        }


        /// <summary>
        /// get detail class booking by id  & userLoginId 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CourseBooking_ViewModel GetBusinessCourseBookingDetailById(long id, long userloginid)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseBooking_Get<CourseBooking_ViewModel>(new SP_ManageCourseBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 7
            });
        }

        /// <summary>
        /// To Get Business Class Detail By CourseId and UserLoginId  
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public CourseBooking_ViewModel GetBusinessCourseBookingDetail(long id, long userloginid)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseBooking_GetByID<CourseBooking_ViewModel>(new SP_ManageCourseBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 7
            });
        }


        /// <summary>
        /// get course list by id
        /// </summary>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public CourseBooking_ViewModel GetCourseListbyId(long id, long userloginid)
        {
            return storedPorcedureRepository.SP_ManageCourseBooking_GetById<CourseBooking_ViewModel>(new SP_ManageCourseBooking_Params_VM()
            {
                Id = id,
                StudentUserLoginId = userloginid,
                Mode = 9
            });
        }

        /// <summary>
        /// To Get Course Booking Detail For Visitor Panel 
        /// </summary>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public List<CourseBooking_ViewModel> GetCourseList(long userloginid)
        {
            return storedPorcedureRepository.SP_ManageCourseBooking_GetAll<CourseBooking_ViewModel>(new SP_ManageCourseBooking_Params_VM()
            {
                StudentUserLoginId = userloginid,
                Mode = 8
            });
        }


        /// <summary>
        ///  To Get Business Course Detail List Through Search 
        /// </summary>
        /// <returns></returns>
        public List<BusinessCourseDetail_VM> GetCourseSearchDetailList(long id, long businessOnwerLoginId,string name)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseSerachDetail_GetAll<BusinessCourseDetail_VM>(new SP_GetBusinessCourseSearch_Param_VM
            {
                Id = id,
                BusinessOwnerLoginId = businessOnwerLoginId,
                Name = name,
                Mode = 1

            });

        }


        /// <summary>
        /// get list of course details ith certificate 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        public List<BusinessCourseDetail_VM> GetCourseDetailListWithCertificate(long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetailList_Get<BusinessCourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 8


            });

        }

        /// <summary>
        /// Get course details list with certifiate and exam 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        public List<BusinessCourseDetail_VM> GetCourseDetailListWithCertificateAndExam(long businessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageCourseDetailList_Get<BusinessCourseDetail_VM>(new SP_ManageCourseDetail_Param_VM
            {

                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 9


            });

        }
        /// <summary>
        /// to get sports booking details
        /// </summary>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public List<TennisBokingDetail_VM> GetSportsBookingDetails(long userloginid)
        {
            return storedPorcedureRepository.SP_ManageBusinessContentTennisDetail_Get<TennisBokingDetail_VM>(new SP_ManageBusinessContentTennis_PPCMeta_Params_VM()
            {
                UserLoginId = userloginid,
                Mode = 4
            });
        }
        /// <summary>
        /// to get admit card
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public CourseBooking_ViewModel GetCourseAdmitCardServicebyId(long id, long userloginid)
        {
            return storedPorcedureRepository.SP_ManageCourseBooking_GetById<CourseBooking_ViewModel>(new SP_ManageCourseBooking_Params_VM()
            {
                Id = id,
                StudentUserLoginId = userloginid,
                Mode = 10
            });
        }
    }
}