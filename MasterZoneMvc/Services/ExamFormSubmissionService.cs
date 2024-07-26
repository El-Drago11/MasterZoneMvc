using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class ExamFormSubmissionService
    {
        private MasterZoneDbContext db;

        public ExamFormSubmissionService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Retrieve submitted Exam Form data based on Id from the ExamFormSubmit Table
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Single data of exam form</returns>
        public ExamFormSubmissionResponse_VM GetExamFormSubmissionById(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userMasterId", ""),
                            new SqlParameter("examFormId", "0"),
                            new SqlParameter("businessId", "0"),
                            new SqlParameter("businessMasterId", "0"),
                            new SqlParameter("mode", 1)
                            };

            var resp = db.Database.SqlQuery<ExamFormSubmissionResponse_VM>("exec sp_ManageExamFormSubmission @id,@userMasterId,@examFormId,@businessId,@businessMasterId,@mode", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Retrieve submitted Exam Form data based on userMasterId, examFormId, businessId, and
        /// businessMasterId from the ExamFormSubmit Table. These parameters are optional; if no values are
        /// provided, retrieve all data from the table. If you provide the ExamFormId, there is no need to pass
        /// the BusinessId and BusinessMasterId. First, check the ExamFormId; if it is zero, then filter by
        /// BusinessId; otherwise, filter by ExamFormId. If BusinessId is also zero, then filter by
        /// BusinessMasterId.
        /// </summary>
        /// <param name="userMasterId"></param>
        /// <param name="examFormId"></param>
        /// <param name="businessId"></param>
        /// <param name="businessMasterId"></param>
        /// <returns>list of Exam form</returns>
        public List<ExamFormSubmissionResponse_VM> GetExamFormSubmissionList(string userMasterId = "", long examFormId = 0, long businessId = 0, long businessMasterId = 0)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userMasterId", userMasterId),
                            new SqlParameter("examFormId", examFormId),
                            new SqlParameter("businessId", businessId),
                            new SqlParameter("businessMasterId", businessMasterId),
                            new SqlParameter("mode", 2)
                            };

            var resp = db.Database.SqlQuery<ExamFormSubmissionResponse_VM>("exec sp_ManageExamFormSubmission @id,@userMasterId,@examFormId,@businessId,@businessMasterId,@mode", queryParams).ToList();
            return resp;
        }

        /// <summary>
        /// Retrieve Profile Image and Signature image data from ExamFormSubmits Table
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Image data of submitted exam form</returns>
        public ExamFormSubmissionResponse_VM GetExamFormSubmissionImage(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userMasterId", "0"),
                            new SqlParameter("examFormId", "0"),
                            new SqlParameter("businessId", "0"),
                            new SqlParameter("businessMasterId", "0"),
                            new SqlParameter("mode", "3")
                            };

            var resp = db.Database.SqlQuery<ExamFormSubmissionResponse_VM>("exec sp_ManageExamFormSubmit @id,@userMasterId,@examFormId,@businessId,@businessMasterId,@mode", queryParams).FirstOrDefault();
            return resp;
        }

    }
}