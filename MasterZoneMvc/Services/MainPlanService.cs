using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class MainPlanService
    {
        private MasterZoneDbContext db;

        public MainPlanService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get MainPlan Detail by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MainPlan_VM GetMainPlanById(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "1"),
                            new SqlParameter("mode", "2")
                            };

            var resp = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }
    }
}