using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace MasterZoneMvc.Services
{
    public class ExamService
    {
        private MasterZoneDbContext db;

        public ExamService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get exam form list data by BusinessId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ExamFormResponse_VM> GetExamFormByBusinessId(long UserLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("userLoginId", UserLoginId),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<ExamFormResponse_VM>("exec sp_ManageExamForm @id,@userLoginId,@mode", queryParams).ToList();
            return resp;
        }

        /// <summary>
        /// Get exam form single data by ExamFromId BusinessId and Active only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExamFormResponse_VM GetExamFormByExamFormId(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

            var resp = db.Database.SqlQuery<ExamFormResponse_VM>("exec sp_ManageExamForm @id,@userLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }


        /// <summary>
        /// Get All exam form list which is active
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public List<ExamFormResponse_VM> GetAllExamFormList()
        {
            SqlParameter[] queryParams = new SqlParameter[]
                {
                    new SqlParameter("id", "0"),
                    new SqlParameter("userLoginId", "0"),
                    new SqlParameter("mode", "3")
                };
            var examFormResponse_VMs = db.Database.SqlQuery<ExamFormResponse_VM>("exec sp_ManageExamForm @id,@userLoginId,@mode", queryParams).ToList();
            return examFormResponse_VMs;
        }

    }
}