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
using static MasterZoneMvc.ViewModels.RequestCertificate_VM;

namespace MasterZoneMvc.Services
{
    public class CertificateService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public CertificateService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        public CertificateEdit_VM GetCertificateById(long id)
        {

            return storedProcedureRepository.SP_ManageCertificate_Get<CertificateEdit_VM>(new ViewModels.StoredProcedureParams.SP_ManageCertificate_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        public JqueryDataTable_Pagination_Response_VM<CertificatePagination_VM> GetCertificate_Pagination(NameValueCollection httpRequestParams, Certificate_Pagination_SQL_Params_VM _Params_VM)
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


            List<CertificatePagination_VM> lstCertificateRecords = new List<CertificatePagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstCertificateRecords = db.Database.SqlQuery<CertificatePagination_VM>("exec sp_ManageCertificate_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstCertificateRecords.Count > 0 ? lstCertificateRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<CertificatePagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<CertificatePagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstCertificateRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Assign Certificate to business-Owner by business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="certificateId">Certificate-Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel AssignBusinessCertificate(long businessOwnerLoginId, long certificateId)
        {
            return storedProcedureRepository.SP_ManageCertificate_Get<SPResponseViewModel>(new ViewModels.StoredProcedureParams.SP_ManageCertificate_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Id = certificateId,
                Mode = 4
            });
        }

        /// <summary>
        /// Un-Assign Certificate to business-Owner by business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="certificateId">Certificate-Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel UnAssignBusinessCertificate(long businessOwnerLoginId, long certificateId)
        {
            return storedProcedureRepository.SP_ManageCertificate_Get<SPResponseViewModel>(new ViewModels.StoredProcedureParams.SP_ManageCertificate_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Id = certificateId,
                Mode = 5
            });
        }

        /// <summary>
        /// Get All active Assigned certifications to business.
        /// </summary>
        /// <param name="businessownerLoginId">Business-Owner-Login-Id</param>
        /// <returns>List of assigned certification profiles</returns>
        public List<CertificateList_ForBO_VM> GetAllActiveBusinessCertificationsById(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageCertificate_GetAll<CertificateList_ForBO_VM>(new SP_ManageCertificate_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 6
            });
        }

        /// <summary>
        /// Get All Assigned Business-Owners to Certificate by Certificate-Id
        /// </summary>
        /// <param name="certificateId">Certificate-Id</param>
        /// <returns>List of assigned Business-Owners</returns>
        public List<BusinessSearch_ForSuperAdmin_VM> GetAllAssignedBusinessOwnersToCertificationById(long certificateId)
        {
            return storedProcedureRepository.SP_ManageCertificate_GetAll<BusinessSearch_ForSuperAdmin_VM>(new SP_ManageCertificate_Params_VM()
            {
                Id = certificateId,
                Mode = 7
            });
        }



        /// <summary>
        /// Certificate Icon By UserLoginId
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public List<TrainingCertificateIconDetail> GetCertificateIcon(long UserLoginId)
        {
            return storedProcedureRepository.SP_ManageCertificate_GetAll<TrainingCertificateIconDetail>(new SP_ManageCertificate_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 8
            });

        }

        /// <summary>
        /// Get User-Certificate by Certificate Number
        /// </summary>
        /// <param name="certifiateNumber">Issued Certificate Number</param>
        /// <returns>User Certificate Detail</returns>
        public UserCertificate_VM GetUserCertificateByCertificateNumber(string certifiateNumber)
        {
            return storedProcedureRepository.SP_ManageUserCertificates_Get<UserCertificate_VM>(new SP_ManageUserCertificates_Params_VM()
            {
                CertificateNumber = certifiateNumber,
                Mode = 2
            });
        }
        
        /// <summary>
        /// Get All User-Certificates Basic Infromation List by User
        /// </summary>
        /// <param name="userLoginId">User-Login-Id</param>
        /// <returns>User Certificates List</returns>
        public List<UserCertificateBasicInfo_VM> GetAllUserCertificates(long userLoginId)
        {
            return storedProcedureRepository.SP_ManageUserCertificates_GetAll<UserCertificateBasicInfo_VM>(new SP_ManageUserCertificates_Params_VM()
            {
                UserLoginId = userLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get All Certificates List
        /// </summary>
        /// <returns>Certificates</returns>
        public List<CertificateEdit_VM> GetAllCertificates()
        {
            return storedProcedureRepository.SP_ManageCertificate_GetAll<CertificateEdit_VM>(new SP_ManageCertificate_Params_VM()
            {
                Mode = 3
            });
        }
    }
}