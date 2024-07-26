using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace MasterZoneMvc.Services
{
    public class BusinessStudentService
    {
        private MasterZoneDbContext db;

        public BusinessStudentService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Link Student with Business 
        /// Adds/links student with business if not exists
        /// </summary>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <param name="studentUserLoginId">Student-User-Login-Id</param>
        /// <returns></returns>
        public SPResponseViewModel AddStudentLinkWithBusinessOwner(long businessOwnerLoginId, long studentUserLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessId", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                            new SqlParameter("studentId", "0"),
                            new SqlParameter("studentUserLoginId", studentUserLoginId),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessStudents @id,@businessId,@businessOwnerLoginId,@studentId,@studentUserLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }
    }
}