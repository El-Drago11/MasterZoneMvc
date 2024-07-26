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
    public class EnquiryService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public EnquiryService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Insert Or Update Enquiry detail
        /// </summary>
        /// <param name="enquiries_Params_VM">Enquiry Stored procedure Params</param>
        /// <returns>Status : +ve value if created/updated, else -ve value with response message</returns>
        public SPResponseViewModel InsertUpdateEnquiry(SP_InsertUpdateEnquiries_Params_VM enquiries_Params_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateEnquiries_Get<SPResponseViewModel>(enquiries_Params_VM);
        }

        /// <summary>
        /// Delete Enquiry by Id
        /// </summary>
        /// <param name="enquiryId">Enquiry Id</param>
        /// <param name="BusinessAdminLoginId">Business-Owner-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel DeleteEnquiry(long enquiryId, long BusinessAdminLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateEnquiries_Get<SPResponseViewModel>(new SP_InsertUpdateEnquiries_Params_VM() { 
                Id = enquiryId,
                UserLoginId = BusinessAdminLoginId,                
                Mode = 3
            });
        }

        /// <summary>
        /// Get Enquiry by Id
        /// </summary>
        /// <param name="enquiryId">Enquiry Id</param>
        /// <param name="BusinessAdminLoginId">Business-Owner-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public EnquiryDescription_VM GetEnquiryById(long enquiryId, long BusinessAdminLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            
            return storedProcedureRepository.SP_ManageEnquiries_Get<EnquiryDescription_VM>(new SP_ManageEnquiries_Params_VM() { 
                Id = enquiryId,
                UserLoginId = BusinessAdminLoginId,                
                Mode = 2
            });
        }

        public JqueryDataTable_Pagination_Response_VM<Enquiry_Pagination_VM> GetBusinessEnquiriesList_Pagination(NameValueCollection httpRequestParams, ManageEnquiry_Pagination_SQL_Params_VM _Params_VM)
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

            List<Enquiry_Pagination_VM> lstPaginationRecords = new List<Enquiry_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("userLoginId", _Params_VM.BusinessOwnerLoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstPaginationRecords = db.Database.SqlQuery<Enquiry_Pagination_VM>("exec sp_ManageEnquiry_Pagination @id,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstPaginationRecords.Count > 0 ? lstPaginationRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Enquiry_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Enquiry_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstPaginationRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Update Enquiry detail
        /// </summary>
        /// <param name="enquiries_Params_VM">Enquiry Stored procedure Params</param>
        /// <returns>Status : +ve value if created/updated, else -ve value with response message</returns>
        public SPResponseViewModel UpdateEnquiry(SP_InsertUpdateEnquiries_Params_VM enquiries_Params_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateEnquiries_Get<SPResponseViewModel>(enquiries_Params_VM);
        }

        ///// <summary>
        ///// Get Enquiry  Detail by Id
        ///// </summary>
        ///// <param name="enquiryId">Enquiry Id</param>
        ///// <param name="BusinessAdminLoginId">Business-Owner-Login-Id</param>
        ///// <returns>Status 1 if deleted, else -ve value with message</returns>
        //public BusinessEnquiryDetail_VM GetEnquiryDetailById(long enquiryId, long BusinessAdminLoginId)
        //{
        //    storedProcedureRepository = new StoredProcedureRepository(db);

        //    return storedProcedureRepository.SP_ManageEnquiriesDetail_Get<BusinessEnquiryDetail_VM>(new SP_ManageEnquiriesDetail_Params_VM()
        //    {
        //        Id = enquiryId,
        //        UserLoginId = BusinessAdminLoginId,
        //        Mode = 3
        //    });
        //}

        /// <summary>
        /// Update  Enquiry-Status by Id
        /// </summary>
        /// <param name="enquiryId">Enquiry Id</param>
        /// <param name="BusinessAdminLoginId">Business-Owner-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel UpdateEnquiryStatus(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageEnquiries_Get<SPResponseViewModel>(new SP_ManageEnquiries_Params_VM()
            {
                Id = Id,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Enquiry Follow-Up Comments By Business-Login-Id
        /// </summary>
        /// <param name="Id">Enquiry-Id</param>
        /// <param name="businessLoginId">Business-Owner-Login-Id</param>
        /// <returns>Enquiry Follow-up Comments List</returns>
        public List<EnquiryFollowCommentList_VM> GetCommentsForBusinessById(long Id, long businessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageEnquiries_GetAll<EnquiryFollowCommentList_VM>(new SP_ManageEnquiries_Params_VM()
            {
                Id = Id,
                UserLoginId = businessLoginId,
                Mode = 6
            });
        }

        /// <summary>
        /// get enquiry by details by id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>
        public List<EnquiryDescription_VM> GetEnquiryDetailById(long Id, long businessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageEnquiries_GetAll<EnquiryDescription_VM>(new SP_ManageEnquiries_Params_VM()
            {
                Id = Id,
                UserLoginId = businessLoginId,
                Mode = 7
            });
        }
    }
}