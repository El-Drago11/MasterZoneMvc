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

namespace MasterZoneMvc.Services
{
    public class ApartmentService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public ApartmentService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }


        /// <summary>
        /// Insert Update Apartment
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public SPResponseViewModel InsertUpdateApartment(SP_InsertUpdateApartment_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_InsertUpdateApartment_Get<SPResponseViewModel>(params_VM);
        }

        /// <summary>
        /// Get Apartment Detail By Id
        /// </summary>
        /// <param name="id">Apartment Id</param>
        /// <returns>Apartment Deatail</returns>
        public Apartment_VM GetApartmentById(long id)
        {

            return storedProcedureRepository.SP_ManageApartment_Get<Apartment_VM>(new SP_ManageApartment_Params_VM()
            {
                Id = id,
                Mode = 2
            });
        }
        
        /// <summary>
        /// Delete Apartment By Id (soft delete)
        /// </summary>
        /// <param name="id">Apartment Id</param>
        /// <returns>Apartment Deatail</returns>
        public SPResponseViewModel DeleteApartmentById(long id, long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_InsertUpdateApartment_Get<SPResponseViewModel>(new SP_InsertUpdateApartment_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Change Apartment Status By Id
        /// </summary>
        /// <param name="id">Apartment Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel ChangeApartmentStatusById(long id, long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_InsertUpdateApartment_Get<SPResponseViewModel>(new SP_InsertUpdateApartment_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Apartments with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams">HttpParams NameValue pair object</param>
        /// <param name="_Params_VM">License Filter Parameters</param>
        /// <returns>List of Apartments</returns>
        public JqueryDataTable_Pagination_Response_VM<ApartmentPagination_VM> GetAllApartments_Pagination(NameValueCollection httpRequestParams, Apartment_Pagination_SQL_Params_VM _Params_VM)
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


            List<ApartmentPagination_VM> lstLicenseRecords = new List<ApartmentPagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstLicenseRecords = db.Database.SqlQuery<ApartmentPagination_VM>("exec sp_GetAllApartments_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLicenseRecords.Count > 0 ? lstLicenseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<ApartmentPagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<ApartmentPagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLicenseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// Get All Active Apartment List for Dropdown
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Apartment List</returns>
        public List<Apartment_Dropdown_VM> GetAllActiveApartmentsForDropdown(long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_ManageApartment_GetAll<Apartment_Dropdown_VM>(new SP_ManageApartment_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }


        /// <summary>
        /// Insert or Update Apartment Booking
        /// </summary>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel InsertUpdateApartmentBooking(SP_InsertUpdateApartmentBooking_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_InsertUpdateApartmentBooking_Get<SPResponseViewModel>(params_VM);
        }

        /// <summary>
        /// Get All License Bookings Details by Business-Owner-Login-Id with Jquery Pagination
        /// </summary>
        /// <param name="httpRequestParams">HttpParams NameValue pair object</param>
        /// <param name="_Params_VM">License Filter Parameters</param>
        /// <returns>List of License Bookings Requests</returns>
        public JqueryDataTable_Pagination_Response_VM<ApartmentBooking_ForBO_Pagination_VM> GetAllApartmentBookingsByBusiness_Pagination(NameValueCollection httpRequestParams, ApartmentBooking_Pagination_SQL_Params_VM _Params_VM)
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


            List<ApartmentBooking_ForBO_Pagination_VM> lstLicenseRecords = new List<ApartmentBooking_ForBO_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", (string.IsNullOrEmpty(_Params_VM.JqueryDataTableParams.sortColumn) ? "" : _Params_VM.JqueryDataTableParams.sortColumn)),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstLicenseRecords = db.Database.SqlQuery<ApartmentBooking_ForBO_Pagination_VM>("exec sp_GetAllApartmentBookings_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstLicenseRecords.Count > 0 ? lstLicenseRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<ApartmentBooking_ForBO_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<ApartmentBooking_ForBO_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstLicenseRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get Apartment-Booking Detail By Apartment-Booking-Id [by Business-Owner]
        /// </summary>
        /// <param name="id">Apartment-Booking Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Apartment-Booking Detail</returns>
        public ApartmentBookingDetail_VM GetApartmentBookingDetailById(long id, long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageApartmentBooking_Get<ApartmentBookingDetail_VM>(new SP_ManageApartmentBooking_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }


        /// <summary>
        /// Get All Apartment-Booking Detail By User-Login-Id [by Business-Owner]
        /// </summary>
        /// <param name="userLoginId">User-Login Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Get all Apartment-Bookings</returns>
        public List<ApartmentBookingDetail_VM> GetAllApartmentBookingDetailsByUserLoginId(long userLoginId, long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageApartmentBooking_GetAll<ApartmentBookingDetail_VM>(new SP_ManageApartmentBooking_Params_VM()
            {
                UserLoginId = userLoginId,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get All Apartment-Booking Distinct User List By Apartment Id [by Business-Owner]
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Apartment-Booking Users List Detail</returns>
        public List<ApartmentBookingUserDetail_VM> GetAllApartmentBookingUserListByApartmentId(long apartmentId, long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageApartmentBooking_GetAll<ApartmentBookingUserDetail_VM>(new SP_ManageApartmentBooking_Params_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                ApartmentId = apartmentId,
                Mode = 2
            });
        }


        #region Apartment Block ----------------------------------------

        /// <summary>
        /// Insert Update Apartment Block
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns>Success or error response with message</returns>
        public SPResponseViewModel InsertUpdateApartmentBlock(SP_InsertUpdateApartmentBlock_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_InsertUpdateApartmentBlock_Get<SPResponseViewModel>(params_VM);
        }

        /// <summary>
        /// Get Apartment-Block Detail By Id
        /// </summary>
        /// <param name="id">Apartment-Block Id</param>
        /// <returns>Apartment Deatail</returns>
        public Apartment_VM GetApartmentBlockById(long id)
        {
            return storedProcedureRepository.SP_ManageApartmentBlock_Get<Apartment_VM>(new SP_ManageApartmentBlock_Params_VM()
            {
                Id = id,
                Mode = 2
            });
        }

        /// <summary>
        /// Delete Apartment-Block By Id (soft delete)
        /// </summary>
        /// <param name="id">Apartment-Block Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Success or error Response with message</returns>
        public SPResponseViewModel DeleteApartmentBlockById(long id, long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_InsertUpdateApartmentBlock_Get<SPResponseViewModel>(new SP_InsertUpdateApartmentBlock_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Change Apartment-Block Status By Id
        /// </summary>
        /// <param name="id">Apartment-Block Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel ChangeApartmentBlockStatusById(long id, long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_InsertUpdateApartmentBlock_Get<SPResponseViewModel>(new SP_InsertUpdateApartmentBlock_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Apartment-Blocks by Apartemnt-Id
        /// </summary>
        /// <param name="apartmentId">Apartment-Id</param>
        /// <returns>List of Apartment Blocks</returns>
        public List<ApartmentBlock_VM> GetAllApartmentBlocks(long apartmentId)
        {
            return storedProcedureRepository.SP_ManageApartmentBlock_GetAll<ApartmentBlock_VM>(new SP_ManageApartmentBlock_Params_VM()
            {
                ApartmentId = apartmentId,
                Mode = 1
            });
        }

        #endregion ----------------------------------------------------

        #region Apartment Area ----------------------------------------

        /// <summary>
        /// Insert Update Apartment Area
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns>Success or error response with message</returns>
        public SPResponseViewModel InsertUpdateApartmentArea(SP_InsertUpdateApartmentArea_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_InsertUpdateApartmentArea_Get<SPResponseViewModel>(params_VM);
        }

        /// <summary>
        /// Get Apartment-Area Detail By Id
        /// </summary>
        /// <param name="id">Apartment-Area Id</param>
        /// <returns>Apartment Deatail</returns>
        public Apartment_VM GetApartmentAreaById(long id)
        {
            return storedProcedureRepository.SP_ManageApartmentArea_Get<Apartment_VM>(new SP_ManageApartmentArea_Params_VM()
            {
                Id = id,
                Mode = 2
            });
        }

        /// <summary>
        /// Delete Apartment-Area By Id (soft delete)
        /// </summary>
        /// <param name="id">Apartment-Area Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Success or error Response with message</returns>
        public SPResponseViewModel DeleteApartmentAreaById(long id, long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_InsertUpdateApartmentArea_Get<SPResponseViewModel>(new SP_InsertUpdateApartmentArea_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// Change Apartment-Area Status By Id
        /// </summary>
        /// <param name="id">Apartment-Area Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel ChangeApartmentAreaStatusById(long id, long businessOwnerLoginId)
        {

            return storedProcedureRepository.SP_InsertUpdateApartmentArea_Get<SPResponseViewModel>(new SP_InsertUpdateApartmentArea_Params_VM()
            {
                Id = id,
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Apartment-Areas by Apartemnt-Id
        /// </summary>
        /// <param name="apartmentId">Apartment-Id</param>
        /// <returns>List of Apartment Areas</returns>
        public List<ApartmentArea_VM> GetAllApartmentAreas(long apartmentId)
        {
            return storedProcedureRepository.SP_ManageApartmentArea_GetAll<ApartmentArea_VM>(new SP_ManageApartmentArea_Params_VM()
            {
                ApartmentId = apartmentId,
                Mode = 1
            });
        }

        #endregion ----------------------------------------------------
    }
}