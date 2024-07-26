using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class ProfilePageTypeService
    {
        private MasterZoneDbContext db;

        public ProfilePageTypeService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get All Page Types In Masterzone
        /// </summary>
        /// <returns>List of all Profile-page-Types</returns>
        public List<ProfilePageType> GetAllProfilePageTypes()
        {
            // Get All Profile-Page-Types
            SqlParameter[] queryParamsGetGroup = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("key", ""),
                            new SqlParameter("mode", "1")
                            };

            return db.Database.SqlQuery<ProfilePageType>("exec sp_ManageProfilePageTypes @id,@key,@mode", queryParamsGetGroup).ToList();
        }

        /// <summary>
        /// Get All Active Profile-page-Types
        /// </summary>
        /// <returns>List</returns>
        public List<ProfilePageType> GetAllActiveProfilePageTypes()
        {
            return GetAllProfilePageTypes().Where(x => x.IsActive == 1).ToList();
        }
    }
}