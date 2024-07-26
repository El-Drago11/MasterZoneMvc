using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class SuperAdminService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public SuperAdminService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);

        }
        /// <summary>
        /// To Get SuperAdmin Profile  Detail 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public SuperAdminDetail_VM GetSuperAdmin_ProfileDetail(long UserLoginId)
        {
            SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", UserLoginId),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<SuperAdminDetail_VM>("exec sp_ManageSuperAdminProfile @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();
            return resp;
        }

        /// <summary>
        ///  To Get SuperAdminDashboard Profile LoggedIn User Detail
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public SuperAdminDetail_VM GetSuperAdminDetail(long UserLoginId)
        {
            SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", UserLoginId),
                            new SqlParameter("mode", "2")
                            };

            var resp = db.Database.SqlQuery<SuperAdminDetail_VM>("exec sp_ManageSuperAdminProfile @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();
            return resp;
        }
        /// <summary>
        /// To Get  SuperAdmin Dashboard Section Detail 
        /// </summary>
        /// <returns></returns>
        public SuperAdminDashboardDetail_VM GetSuperAdminDashboardDetail()
        {
            SqlParameter[] queryParamsGetSubAdmin = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<SuperAdminDashboardDetail_VM>("exec sp_ManageSuperAdminDashBoard @id,@userLoginId,@mode", queryParamsGetSubAdmin).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// To Add/Update About Detail through SuperAdmin 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public SuperAdminAboutDetail_VM GetSuperAdminAboutDetail_Get(long UserLoginId)
        {
            return storedPorcedureRepository.SP_ManageSuperAdminAboutDetail_Get<SuperAdminAboutDetail_VM>(new SP_ManageSuperAdminAboutDetail_Param_VM
            {
                UserLoginId = UserLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// To Get SuperAdmin About Detail For Visitor Panel
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public SuperAdminAboutDetail_VM GetSuperAdminAboutDetailGet()
        {
            return storedPorcedureRepository.SP_ManageSuperAdminAboutDetail_Get<SuperAdminAboutDetail_VM>(new SP_ManageSuperAdminAboutDetail_Param_VM
            {

                Mode = 2

            });

        }

    }
}