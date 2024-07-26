using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
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
using static MasterZoneMvc.ViewModels.BusinessCourseCategory_VM;

namespace MasterZoneMvc.Services
{
    public class BusinessCategoryService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public BusinessCategoryService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        public IEnumerable<BusinessCategoryViewModel> GetAllActiveBusinessCategories()
        {
            return null;
        }

        public BusinessCategory_VM GetBusinessCategoryById(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                             new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", "3")
                            };

            var resp = db.Database.SqlQuery<BusinessCategory_VM>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Get Business  Course Category Detail  Data by Id
        /// </summary>
        /// <param name="UserLoginId">UserLoginId Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public BusinessCourseCategoryDetail_VM GetBusinessCourseCategoryDetail_ById(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCourseCategory_Get<BusinessCourseCategoryDetail_VM>(new SP_ManageBusinessCourseCategory_Param_VM()
            {
                Id = Id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get Business  Course Category Detail  Data List
        /// </summary>
        /// <param name="UserLoginId">UserLoginId Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<BusinessCourseCategoryDetail_VM> GetManageBusinessCourseCategorydetailLst(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCourseCategory_Getlst<BusinessCourseCategoryDetail_VM>(new SP_ManageBusinessCourseCategory_Param_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// Get Business Owners Course Category Detail By Pagination [Jquery Datatable Pagination]
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessContentCourseCategoryDetail_Pagination_VM> GetBusinessCourseCategoryList_Pagination(NameValueCollection httpRequestParams, BusinessContentCourseCategoryDetail_Pagination_SQL_Params_VM _Params_VM)
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


            List<BusinessContentCourseCategoryDetail_Pagination_VM> lstCourseCategoryRecords = new List<BusinessContentCourseCategoryDetail_Pagination_VM>();

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

            lstCourseCategoryRecords = db.Database.SqlQuery<BusinessContentCourseCategoryDetail_Pagination_VM>("exec sp_ManageBusinessCourseCategory_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstCourseCategoryRecords.Count > 0 ? lstCourseCategoryRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessContentCourseCategoryDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessContentCourseCategoryDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstCourseCategoryRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Business Content  Course Category   Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content  Course Category Detail</returns>
        public SPResponseViewModel DeleteCourseCategoryById(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateBusinessCourseCategory_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessCourseCategory_Param_Vm
            {
                Id = Id,
                Mode = 3
            });

        }

        /// <summary>
        /// Business Content  Course Category   Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   Business Content  Course Category Detail</returns>
        public SPResponseViewModel DeleteCourseCategoryDetailById(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateBusinessContentCourceCategory_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentCourceCategory_PPCMeta_Param_VM
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2


            });

        }

        /// <summary>
        /// Get Business  Course Category Detail  Data List
        /// </summary>
        /// <param name="UserLoginId">UserLoginId Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<BusinessCourseCategoryDetail_VM> GetBusinessCourseCategoryDetailList(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCourseCategory_Getlst<BusinessCourseCategoryDetail_VM>(new SP_ManageBusinessCourseCategory_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get Business  Course Category Detail  Data 
        /// </summary>
        /// <param name="UserLoginId">UserLoginId Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<BusinessContentCourseCategoryDetail_PPCMeta> GetBusinessCourseCategoryDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCourseCategoryDetail_Getlst<BusinessContentCourseCategoryDetail_PPCMeta>(new SP_ManageBusinessCourseCategory_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        public List<BusinessContentCourseCategoryDetail_PPCMeta> GetCourseCategoryDetailList()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCourseCategoryDetail_Getlst<BusinessContentCourseCategoryDetail_PPCMeta>(new SP_ManageBusinessCourseCategory_Param_VM()
            {
                Mode = 2
            });
        }

        /// <summary>
        ///  To Get All ParentBusinessCategory Detail Where Id is zero 
        /// </summary>
        /// <returns></returns>
        public List<ClassCategoryType_VM> Get_ManageParentClassCategoryDetail()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCategory_GetAll<ClassCategoryType_VM>(new SP_ManageBusinessCategorty_Param_VM()
            {

                Mode = 8
            });
        }

        /// <summary>
        /// To Get Sub Category Detail  by Category Key In BusinessAdmin using for (branches)
        /// </summary>
        /// <returns></returns>
        public List<BusinessCategory_VM> GetAllB2BSubCategories()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCategory_GetAll<BusinessCategory_VM>(new SP_ManageBusinessCategorty_Param_VM()
            {
                Mode = 9
            });
        }

        /// <summary>
        /// To Get All Active B2B Sub-Categories List for Home Page
        /// </summary>
        /// <returns>Business Sub-Categories List</returns>
        public List<BusinessCategory_VM> GetAllB2BSubCategoriesForHomePage()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCategory_GetAll<BusinessCategory_VM>(new SP_ManageBusinessCategorty_Param_VM()
            {
                Mode = 10
            });
        }

        /// <summary>
        /// To Get All Active B2B Businesses by Sub-Category for Home Page
        /// </summary>
        /// <param name="subCategoryId">Business Sub-Category Id</param>
        /// <returns>Business List</returns>
        public List<BusinessListByCategory_HomePage_VM> GetAllB2BBusinessListBySubCategoryForHomePage(long subCategoryId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCategory_GetAll<BusinessListByCategory_HomePage_VM>(new SP_ManageBusinessCategorty_Param_VM()
            {
                Id = subCategoryId,
                Mode = 11
            });
        }


        /// <summary>
        /// Get All Instructor Business-Categories. (Which are specified for instructor registration)
        /// </summary>
        /// <returns>Available Instructor Business-Categories</returns>
        public List<BusinessCategory_VM> GetAllInstructorBusinessCategories()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessCategory_GetAll<BusinessCategory_VM>(new SP_ManageBusinessCategorty_Param_VM()
            {
                Mode = 12
            });
        }


        /// <summary>
        /// Get All B2B Business List By Sub Category For Busines List Page
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns></returns>


        public List<BusinessListByCategory_HomePage_VM> GetAllB2BBusinessListBySubCategoryBusinessListPage(long subCategoryId, long lastRecordId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessList_Get<BusinessListByCategory_HomePage_VM>(new SP_ManageBusinessCategorty_Param_VM()
            {
                Id = subCategoryId,
                LastRecordId = lastRecordId,
                Mode = 13
            });
        }

        ///// <summary>
        ///// get course detils List
        ///// </summary>
        ///// <returns></returns>
        //public List<BusinessContentCourseCategoryDetail_PPCMeta> GetCourseCategoryDetailList()
        //{
        //    storedPorcedureRepository = new StoredProcedureRepository(db);

        //    return storedPorcedureRepository.SP_ManageBusinessCourseCategoryDetail_Getlst<BusinessContentCourseCategoryDetail_PPCMeta>(new SP_ManageBusinessCourseCategory_Param_VM()
        //    {

        //        Mode = 2
        //    });
        //}

    }
}