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
    public class StudentFavouriteService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;
        public StudentFavouriteService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }
        public List<StudentFavouritesViewModel> GetStudentFavoriteUserLoginListById(long id)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                             new SqlParameter("studentLoginId", ""),
                            new SqlParameter("favouriteUserLoginId ", ""),
                            new SqlParameter("mode", "5")
                            };

            var FavoriteUserList = db.Database.SqlQuery<StudentFavouritesViewModel>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", queryParams).ToList();
            return FavoriteUserList;
        }

        /// <summary>
        /// TODO: [Meena Integration] need setting in Stored Procedure 2023-08-07
        /// Get Instructor Favourite  List 
        /// </summary>
        /// <param name="userLoginId">Student-Login-Id</param>
        /// <param name="lastRecordId">Last fetched Record Id</param>
        /// <param name="recordLimit">No. of records to fetch next</param>
        /// <returns>List of Favourites</returns>
        public List<UserInstructor_VM> GetUserFavouriteInstructorList(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedPorcedureRepository.SP_GetInstructorProfileDetail<UserInstructor_VM>(new SP_GetInstructorProfileDetail_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });
        }
    }
}