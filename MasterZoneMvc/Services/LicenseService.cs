using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
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
    public class LicenseService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public LicenseService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get License Detail By Id
        /// </summary>
        /// <param name="id">License Id</param>
        /// <returns>License Detail</returns>
        public LicenseEdit_VM GetLicenseById(long id)
        {
            return storedPorcedureRepository.SP_ManageLicense_Get<LicenseEdit_VM>(new ViewModels.StoredProcedureParams.SP_ManageLicense_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get License Table-Data By Id
        /// </summary>
        /// <param name="id">License Id</param>
        /// <returns>License Table Data</returns>
        public LicenseEdit_VM GetLicenseRecordDataById(long id)
        {
            return storedPorcedureRepository.SP_ManageLicense_Get<LicenseEdit_VM>(new ViewModels.StoredProcedureParams.SP_ManageLicense_Params_VM()
            {
                Id = id,
                Mode = 3
            });
        }

        /// <summary>
        /// Insert Update License
        /// </summary>
        /// <param name="licenseId">License Data</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel InsertUpdateLicense(SP_InsertUpdateLicense_Params_VM params_VM)
        {
            return storedPorcedureRepository.SP_InsertUpdateLicense<SPResponseViewModel>(params_VM);
        }

        /// <summary>
        /// Change License Status By Id
        /// </summary>
        /// <param name="licenseId">License Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel ChangeLicenseStatus(long licenseId, long loginId)
        {
            return storedPorcedureRepository.SP_InsertUpdateLicense<SPResponseViewModel>(new SP_InsertUpdateLicense_Params_VM()
            {
                Id = licenseId,
                SubmittedByLoginId = loginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Soft Delete License By Id
        /// </summary>
        /// <param name="licenseId">License Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel DeleteLicense(long licenseId, long loginId)
        {
            return storedPorcedureRepository.SP_InsertUpdateLicense<SPResponseViewModel>(new SP_InsertUpdateLicense_Params_VM()
            {
                Id = licenseId,
                SubmittedByLoginId = loginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get All Licenses by Certificate Id with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams">HttpParams NameValue pair object</param>
        /// <param name="_Params_VM">License Filter Parameters</param>
        /// <returns>List of Licenses</returns>
        public JqueryDataTable_Pagination_Response_VM<LicensePagination_VM> GetAllLicense_Pagination(NameValueCollection httpRequestParams, License_Pagination_SQL_Params_VM _Params_VM)
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


            List<LicensePagination_VM> lstLicenseRecords = new List<LicensePagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("certificateId", _Params_VM.CertificateId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstLicenseRecords = db.Database.SqlQuery<LicensePagination_VM>("exec sp_ManageLicense_Pagination @id,@certificateId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLicenseRecords.Count > 0 ? lstLicenseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<LicensePagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<LicensePagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLicenseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Get All active Licenses for business Owner by Certificate-Id
        /// </summary>
        /// <param name="certificateId">Certificate Id</param>
        /// <returns>List of assigned certification profiles</returns>
        public List<LicenseList_ForBO_VM> GetAllActiveLicensesByCertificateIdForBO(long certificateId)
        {
            return storedPorcedureRepository.SP_ManageLicense_GetAll<LicenseList_ForBO_VM>(new SP_ManageLicense_Params_VM()
            {
                CertificateId = certificateId,
                Mode = 2
            });
        }

        #region License Booking --------------------

        /// <summary>
        /// Get License-Booking-Request Detail for Business-Owner by License-Booking-Id
        /// </summary>
        /// <param name="id">License-Booking-Id</param>
        /// <returns>License Booking Detail</returns>
        public LicenseBookingRequest_ForBO_VM GetLicenseBookingDetailForBOById(long id)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_Get<LicenseBookingRequest_ForBO_VM>(new SP_ManageLicenseBooking_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All License Bookings Details by Business-Owner-Login-Id with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams">HttpParams NameValue pair object</param>
        /// <param name="_Params_VM">License Filter Parameters</param>
        /// <returns>List of License Bookings Requests</returns>
        public JqueryDataTable_Pagination_Response_VM<LicenseBookingRequest_ForBO_Pagination_VM> GetAllLicenseBookingsByBusiness_Pagination(NameValueCollection httpRequestParams, License_Pagination_SQL_Params_VM _Params_VM)
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


            List<LicenseBookingRequest_ForBO_Pagination_VM> lstLicenseRecords = new List<LicenseBookingRequest_ForBO_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("certificateId", _Params_VM.CertificateId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstLicenseRecords = db.Database.SqlQuery<LicenseBookingRequest_ForBO_Pagination_VM>("exec sp_GetAllLicenseBookingsByBusinessOwner_Pagination @id,@certificateId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLicenseRecords.Count > 0 ? lstLicenseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<LicenseBookingRequest_ForBO_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<LicenseBookingRequest_ForBO_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLicenseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get All License Booking Request Details for Super-Admin-Panel with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams">HttpParams NameValue pair object</param>
        /// <param name="_Params_VM">License Filter Parameters</param>
        /// <returns>List of License Bookings Requests</returns>
        public JqueryDataTable_Pagination_Response_VM<LicenseBookingRequest_ForSA_Pagination_VM> GetAllLicenseBookingsBySuperAdmin_Pagination(NameValueCollection httpRequestParams, License_Pagination_SQL_Params_VM _Params_VM)
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


            List<LicenseBookingRequest_ForSA_Pagination_VM> lstLicenseRecords = new List<LicenseBookingRequest_ForSA_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("certificateId", _Params_VM.CertificateId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstLicenseRecords = db.Database.SqlQuery<LicenseBookingRequest_ForSA_Pagination_VM>("exec sp_GetAllLicenseBookingsBySuperAdmin_Pagination @id,@certificateId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLicenseRecords.Count > 0 ? lstLicenseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<LicenseBookingRequest_ForSA_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<LicenseBookingRequest_ForSA_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLicenseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get License-Booking-Request Detail for Super-Admin by License-Booking-Id
        /// </summary>
        /// <param name="id">License-Booking-Id</param>
        /// <returns>License Booking Detail</returns>
        public LicenseBookingRequest_ForSA_VM GetLicenseBookingDetailForSAById(long id)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_Get<LicenseBookingRequest_ForSA_VM>(new SP_ManageLicenseBooking_Params_VM()
            {
                Id = id,
                Mode = 2
            });
        }
        #endregion ---------------------------------

        /// <summary>
        /// Insert Business Licenses
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns>success or error response</returns>
        public SPResponseViewModel InsertBusinessLicenses(SP_InsertUpdateBusinessLicenses_Params_VM params_VM)
        {
            return storedPorcedureRepository.SP_InsertUpdateBusinessLicenses_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessLicenses_Params_VM()
            {
                Id = params_VM.Id,
                BusinessOwnerLoginId = params_VM.BusinessOwnerLoginId,
                LicenseBookingId = params_VM.LicenseBookingId,
                LicenseId = params_VM.LicenseId,
                Quantity = params_VM.Quantity,
                QuantityUsed = params_VM.QuantityUsed,
                Mode = 1
            });
        }

        /// <summary>
        /// Approve License Booking by License-Booking-Id and Assign Licenses to Business-Owner only if it's in Pending state
        /// </summary>
        /// <param name="licenseBookingId">License Booking Id</param>
        /// <returns>Success or Error response</returns>
        public SPResponseViewModel ApproveLicenseBooking(long licenseBookingId)
        {
            var resp_updateStatus = storedPorcedureRepository.SP_InsertUpdateLicenseBooking_Get<SPResponseViewModel>(new SP_InsertUpdateLicenseBooking_Params_VM()
            {
                Id = licenseBookingId,
                Status = 2,
                Mode = 2
            });
            
            //if(resp_updateStatus != null && resp_updateStatus.ret == 1)
            //{
            //    return InsertBusinessLicenses(new SP_InsertUpdateBusinessLicenses_Params_VM()
            //    {
            //        LicenseBookingId = licenseBookingId,
            //        QuantityUsed = 0
            //    });
            //}

            return resp_updateStatus;
        }

        /// <summary>
        /// Decline License Booking by License-Booking-Id only if it's in Pending state
        /// </summary>
        /// <param name="licenseBookingId">License Booking Id</param>
        /// <returns>Success or Error response</returns>
        public SPResponseViewModel DeclineLicenseBooking(long licenseBookingId)
        {
            var resp_updateStatus = storedPorcedureRepository.SP_InsertUpdateLicenseBooking_Get<SPResponseViewModel>(new SP_InsertUpdateLicenseBooking_Params_VM()
            {
                Id = licenseBookingId,
                Status = 3,
                Mode = 2
            });

            return resp_updateStatus;
        }

        /// <summary>
        /// Approve License Booking Payemnt (Paid) by License-Booking-Id
        /// </summary>
        /// <param name="PaymentResponseId">License Booking Id</param>
        /// <returns>Success or Error response</returns>
        public SPResponseViewModel ApproveLicenseBookingPayment(long PaymentResponseId)
        {
            var resp_updateStatus = storedPorcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(new SP_InsertUpdatePaymentResponse_Params_VM()
            {
                Id = PaymentResponseId,
                IsApproved = 1,
                Mode = 2
            });

            return resp_updateStatus;
        }

        /// <summary>
        /// Get All Business-Licenses by Business-Owner-Login-Id with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams">HttpParams NameValue pair object</param>
        /// <param name="_Params_VM">License Filter Parameters</param>
        /// <returns>List of Business-License</returns>
        public JqueryDataTable_Pagination_Response_VM<BusinessBookedLicense_Pagination_VM> GetAllBusinesssLicenses_Pagination(NameValueCollection httpRequestParams, License_Pagination_SQL_Params_VM _Params_VM)
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


            List<BusinessBookedLicense_Pagination_VM> lstLicenseRecords = new List<BusinessBookedLicense_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("certificateId", _Params_VM.CertificateId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstLicenseRecords = db.Database.SqlQuery<BusinessBookedLicense_Pagination_VM>("exec sp_GetAllLicenseBookingsByBusinessOwner_Pagination @id,@certificateId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLicenseRecords.Count > 0 ? lstLicenseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<BusinessBookedLicense_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BusinessBookedLicense_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLicenseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Available License Achieving order number in same certification Profile.
        /// </summary>
        /// <param name="certificateId">Certificate Id</param>
        /// <returns>Available Achieving-Order Number</returns>
        public int GetAvailableLicenseAchievingOrder(long certificateId)
        {
            return storedPorcedureRepository.SP_ManageLicense_Get<int>(new SP_ManageLicense_Params_VM()
            {
                CertificateId = certificateId,
                Mode = 4
            });
        }

        /// <summary>
        /// To Get License To Teach Detail 
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<LicenseToTeachDetail_VM> SP_ManageLicenseToTeach_GetAll(long BusinessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_GetAll<LicenseToTeachDetail_VM>(new SP_ManageLicenseBooking_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get All Booked-Certification-Profiles (from Approved Licenses-Bookings) by Business-Owenr which are available
        /// if Training-Id is passed then it also includes its Certification instead of its Availability/quantity
        /// </summary>
        /// <param name="BusinessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="trainingId">Training-Id</param>
        /// <returns>List of Available Certification Profiles</returns>
        public List<CertificateListForDropdown_VM> GetAllAvailableBookedCertificationProfilesByBusiness(long BusinessOwnerLoginId, long trainingId = 0)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_GetAll<CertificateListForDropdown_VM>(new SP_ManageLicenseBooking_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                TrainingId = trainingId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Booked-Licenses (from Approved Licenses Bookings) by certification-profile by Business-Owenr which are available
        /// if TrainingId is passed then it also includes its License instead of its Availability/quantity
        /// </summary>
        /// <param name="BusinessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="certificateId">Certificate-Id</param>
        /// <param name="trainingId">Training-Id</param>
        /// <returns>List of Available Busienss-Booked Licenses</returns>
        public List<BusinessBookedLicense_VM> GetAllAvailableBookedLicensesByBusiness(long BusinessOwnerLoginId, long certificateId, long trainingId = 0)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_GetAll<BusinessBookedLicense_VM>(new SP_ManageLicenseBooking_Params_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                CertificateId = certificateId,
                TrainingId = trainingId,
                Mode = 5
            });
        }

        /// <summary>
        /// Get All Booked-Licenses (from Approved Licenses Bookings) by license-booking-Id by Business-Owenr which are available
        /// </summary>
        /// <param name="BusinessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="licenseBookingId">License-Booking-Id</param>
        /// <returns>List of Available Busienss-Booked Licenses</returns>
        public int GetBookedLicensesAvailableQuantity(long BusinessOwnerLoginId, long licenseBookingId)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_Get<int>(new SP_ManageLicenseBooking_Params_VM()
            {
                Id = licenseBookingId,
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 6
            });
        }

        /// <summary>
        /// Get List of Licence-Bookings which are approved and the quantity has not consumed full yet. [having remaining quantity]
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="licenseId">License-Id</param>
        /// <returns>List of Approved license bookings having quantity</returns>
        public List<LicenseBookingViewModel> GetApprovedLicenseBookingListHavingRemainingQuantity(long businessOwnerLoginId, long licenseId)
        {
            return storedPorcedureRepository.SP_ManageLicenseBooking_GetAll<LicenseBookingViewModel>(new SP_ManageLicenseBooking_Params_VM()
            {
                LicenseId = licenseId,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 7
            });
        }

        /// <summary>
        /// Get All Licenses
        /// </summary>
        /// <param name="id">License Id</param>
        /// <returns>All license detail</returns>
        public List<LicenseEdit_VM> GetAllLicenses()
        {
            return storedPorcedureRepository.SP_ManageLicense_GetAll<LicenseEdit_VM>(new ViewModels.StoredProcedureParams.SP_ManageLicense_Params_VM()
            {
                Mode = 5
            });
        }
    }
}