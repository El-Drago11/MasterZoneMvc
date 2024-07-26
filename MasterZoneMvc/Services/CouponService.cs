using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.Services
{
    public class CouponService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public CouponService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Coupon Detail By Coupon-ID
        /// </summary>
        /// <param name="id">Coupon-Id</param>
        /// <returns>Coupon Detail</returns>
        public Coupon_VM GetCouponDetailById(long id)
        {
            return storedPorcedureRepository.SP_ManageCoupon_Get<Coupon_VM>(new SP_ManageCoupon_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get Coupon Detail by Coupon Code
        /// </summary>
        /// <param name="couponCode">Coupon Code</param>
        /// <returns>Coupon Detail</returns>
        public Coupon_VM GetCouponDetailByCouponCode(string couponCode, long userLoginId)
        {
            return storedPorcedureRepository.SP_ManageCoupon_Get<Coupon_VM>(new SP_ManageCoupon_Params_VM() { 
                UserLoginId = userLoginId,
                CouponCode = couponCode,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Coupon Detail With Students List by Coupon Id
        /// </summary>
        /// <param name="id">Coupon Id</param>
        /// <returns>Coupon Detail With Students List</returns>
        public CouponDetailWithStudent_VM GetCouponDetailWithStudentById(long id)
        {
            return storedPorcedureRepository.SP_ManageCoupon_Get<CouponDetailWithStudent_VM>(new SP_ManageCoupon_Params_VM() { 
                Id = id,
                Mode = 3
            });
        }
        
        public JqueryDataTable_Pagination_Response_VM<Coupon_Pagination_VM> GetCouponsListByBusinessOwner_Pagination(NameValueCollection httpRequestParams, CouponList_Pagination_SQL_Params_VM _Params_VM)
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


            List<Coupon_Pagination_VM> lstCouponRecords = new List<Coupon_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstCouponRecords = db.Database.SqlQuery<Coupon_Pagination_VM>("exec sp_ManageCoupon_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstCouponRecords.Count > 0 ? lstCouponRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Coupon_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Coupon_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstCouponRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        #region Coupon Consumption
        /// <summary>
        /// Get User Coupon Consumption by Coupon Code
        /// </summary>
        /// <param name="ConsumerUserLoginId">Student/User Login Id</param>
        /// <param name="couponCode">Coupon Code to check</param>
        /// <returns>Returns Consumption if already used, else null</returns>
        public CouponConsumptionViewModel GetUserCouponConsumption(long ConsumerUserLoginId, string couponCode)
        {
            return storedPorcedureRepository.SP_ManageCouponConsumption_Get<CouponConsumptionViewModel>(new SP_ManageCouponConsumption_Params_VM()
            {
                ConsumerUserLoginId = ConsumerUserLoginId,
                CouponCode = couponCode,
                Mode = 1
            });
        }

        /// <summary>
        /// Get User Coupon Consumption by Coupon-Id
        /// </summary>
        /// <param name="ConsumerUserLoginId">Student/User Login Id</param>
        /// <param name="couponId">Coupon-Id</param>
        /// <returns>Returns Consumption if already used, else null</returns>
        public CouponConsumptionViewModel GetUserCouponConsumption(long ConsumerUserLoginId, long couponId)
        {
            return storedPorcedureRepository.SP_ManageCouponConsumption_Get<CouponConsumptionViewModel>(new SP_ManageCouponConsumption_Params_VM()
            {
                Id = couponId,
                ConsumerUserLoginId = ConsumerUserLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// Make Coupon Consumption for User
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="consumerUserLoginId">Consumer User-Login-Id</param>
        /// <returns></returns>
        public SPResponseViewModel AddCouponConsumption(long couponId, long consumerUserLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", "0"),
                            new SqlParameter("@couponId", couponId),
                            new SqlParameter("@consumerUserLoginId", consumerUserLoginId),
                            new SqlParameter("@mode", "1")
                            };

            return db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateCouponConsumption @id,@couponId,@consumerUserLoginId,@mode", queryParams).FirstOrDefault();
        }

        #endregion
    }
}