using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MasterZoneMvc.Services
{
    public class FeatureService
    {
        private MasterZoneDbContext db;

        public FeatureService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get User Features 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<FeatureViewModel> GetAllUserFeatures(long userLoginId)
        {
            SqlParameter[] queryParamsFeatures = new SqlParameter[] {
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("mode", "1")
                            };

            var respFeatures = db.Database.SqlQuery<FeatureViewModel>("exec sp_ManageUserFeatures @userLoginId,@mode", queryParamsFeatures).ToList();
            return respFeatures;
        }

        /// <summary>
        /// Get all features available in specific Panel (Business, SuperAdmin)
        /// </summary>
        /// <param name="panelType">Panel Type (BusinessPanel, SuperAdmin.. from StaticResources)</param>
        /// <returns>All features available in specific panel</returns>
        public List<FeatureViewModel> GetAllPanelFeatureList(int panelType)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("panelTypeId", panelType),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<FeatureViewModel>("exec sp_ManageFeatures @id,@panelTypeId,@mode", queryParams).ToList();
            return resp;
        }

        /// <summary>
        /// Get all Active features in specific Panel (Business, SuperAdmin)
        /// </summary>
        /// <param name="panelType">Panel Type (BusinessPanel, SuperAdmin.. from StaticResources)</param>
        /// <returns>All features available in specific panel</returns>
        public List<FeatureViewModel> GetAllPanelActiveFeatureList(int panelType)
        {
            return GetAllPanelFeatureList(panelType).Where(f => f.IsActive == 1).ToList();
        }

    }
}