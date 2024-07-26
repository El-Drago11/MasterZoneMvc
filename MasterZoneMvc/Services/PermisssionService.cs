using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class PermisssionService
    {
        private MasterZoneDbContext db;

        public PermisssionService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get User Permissions from 
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public List<PermissionHierarchy_VM> GetAllUserPermissions(long userLoginId)
        {
            SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                          new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("mode", "1")
                            };

            var respPermissions = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManageUserPermissions @id,@userLoginId,@mode", queryParamsPermissions).ToList();
            return respPermissions;
        }

        //public List<PermissionHierarchy_VM> GetAllUserPermissionsInHierarchy(long userLoginId)
        //{
        //    var Permissions = GetAllUserPermissions(userLoginId);

        //}

        /// <summary>
        /// Get all permissions available in specific Panel (Business, SuperAdmin)
        /// </summary>
        /// <param name="panelType">Panel Type (BusinessPanel, SuperAdmin.. from StaticResources)</param>
        /// <returns>All permissions available in specific panel</returns>
        public List<PermissionHierarchy_VM> GetAllPanelPermissionList(int panelType)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("panelTypeId", panelType),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManagePermissions @id,@panelTypeId,@mode", queryParams).ToList();
            return resp;
        }
    }
}