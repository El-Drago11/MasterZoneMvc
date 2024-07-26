using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using static iTextSharp.text.pdf.AcroFields;
using iTextSharp.text.pdf.qrcode;

namespace MasterZoneMvc.Services
{
    public class PlanService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public PlanService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Get Active Plan Detail By Id
        /// </summary>
        /// <param name="id"></param>
        public void GetActivePlanDetailById(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

            var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Plan Table Data By Id
        /// </summary>
        /// <param name="id">Plan Id</param>
        public BusinessPlanViewModel GetPlanDataById(long id ,int plantype)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("planType", plantype), 
                            new SqlParameter("mode", "6")
                            };

            return db.Database.SqlQuery<BusinessPlanViewModel>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        /// To Get Busines Main Plan Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MainPlan_VM GetMainPlanDataById(long id)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",id), 
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4")
                            };

            return  db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        public bool IsAlreadyBusibnessPlanPurchased(long planId, long userLoginId)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("planId", planId),
                            new SqlParameter("mode", "1")
                            };

            var response = db.Database.SqlQuery<BusinessPackageBooking>("exec sp_ManagePackageBooking @id,@userLoginId,@planId,@mode", queryParamsGetEvent).FirstOrDefault();

            if (response == null)
            {
                return false;
            }
            else
            {
                DateTime _PlanEndDate = DateTime.ParseExact(response.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (_PlanEndDate < DateTime.UtcNow.Date)
                {
                    return false;
                }
            }

            return true;
        }



        /// <summary>
        /// Verify if Plan has already been purchased by User [Paid Order Status]
        /// </summary>
        /// <param name="planId">Plan Id</param>
        /// <param name="userLoginId">Logged In User-Login-Id (Buyer)</param>
        /// <returns>
        /// Returns true if already purchased, and false if not purchased
        /// </returns>
        public bool IsAlreadyPlanPurchased(long planId, long userLoginId)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("planId", planId),
                            new SqlParameter("mode", "1")
                            };

            var response = db.Database.SqlQuery<PlanBooking>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParamsGetEvent).FirstOrDefault();

            if (response == null)
            {
                return false;
            }
            else
            {
                DateTime _PlanEndDate = DateTime.ParseExact(response.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (_PlanEndDate < DateTime.UtcNow.Date)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get All Business Plans for Visitor/Home view. Also can filter by location City or lat/long
        /// </summary>
        /// <param name="city"></param>
        /// <param name="lastRecordId">Last Record Id of record</param>
        /// <param name="recordLimit">no. of next records to fetch</param>
        /// <returns>List of Business Plans/Packages</returns>
        public List<BusinessPlan_VM> GetAllBusinessPlan(string city, long lastRecordId, int recordLimit)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_GetAllBusinessPlans_GetAll<BusinessPlan_VM>(new SP_GetBusinessPlanRecord_Params_VM
            {
                City = city,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All Active Business-Plans list by Business-Owner-Login-Id
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns>List of Active Plans if exists else returns null</returns>
        public List<BusinessPlan_VM> GetAllActiveBusinessPlans(long businessOwnerLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                            new SqlParameter("planType", "0"),
                            new SqlParameter("mode", "9")
                            };

            var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).ToList();
            return resp;
        }

        /// <summary>
        /// Get Instructor Package Detail List by Id 
        /// </summary>
        /// <param name="userLoginId">Instructor-Business-Owner-Login-Id</param>
        /// <param name="lastRecordId">Last Fetched Record Id</param>
        /// <param name="recordLimit">No. of Records to fetch next</param>
        /// <returns>Packages/Plans List</returns>
        public List<CoachesInstructorPackageDetail> GetCoachesPackageDetailListById(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedPorcedureRepository.SP_GetInstructorProfileDetail<CoachesInstructorPackageDetail>(new SP_GetInstructorProfileDetail_Params_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 5
            });
        }


        /// <summary>
        /// To Get Business Content Plan  PPCMeta Detail
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentPlan_PPCMetaDetail_VM GetBusinessContentPlanPPCMetaDetail(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentPlanPPCMeta_Get<BusinessContentPlan_PPCMetaDetail_VM>(new SP_ManageBusinessContentPlan_PPCMeta_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Business Content Plan  PPCMeta Detail(List)
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentPlan_PPCMetaDetail_VM> GetBusinessContentPlanDetailList(long BusinessOwnerLoginId)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageBusinessContentPlanPPCMeta_GetList<BusinessContentPlan_PPCMetaDetail_VM>(new SP_ManageBusinessContentPlan_PPCMeta_Param_VM()
            {
                BusinessOwnerLoginId = BusinessOwnerLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// get plan details from list to single
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public PlanBooking_ViewModel GetPlanBookingDetailById(long id, long userloginid ,int planbookingtype)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            if (planbookingtype == 2)
            {
                return storedPorcedureRepository.SP_ManagePlanBooking_Get<PlanBooking_ViewModel>(new SP_ManagePlanBooking_Params_VM
                {
                    Id = id,
                    UserLoginId = userloginid,
                    Mode = 4
                });

            }
            else
            {
                return storedPorcedureRepository.SP_ManagePlanBooking_Get<PlanBooking_ViewModel>(new SP_ManagePlanBooking_Params_VM
                {
                    Id = id,
                    UserLoginId = userloginid,
                    Mode = 8
                });
            }
            
        }


        /// <summary>
        /// get plan details from list to single
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public PlanBooking_ViewModel GetBusinessPlanBookingDetailById(long id, long userloginid)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManagePlanBooking_GetById<PlanBooking_ViewModel>(new SP_ManagePlanBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 6
            });
        }
        /// <summary>
        /// get plan details by id 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userloginid"></param>
        /// <returns></returns>
        public PlanBooking_ViewModel GetBusinessClassBookingDetail(long id, long userloginid)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManagePlanBooking_Get<PlanBooking_ViewModel>(new SP_ManagePlanBooking_Params_VM
            {
                Id = id,
                UserLoginId = userloginid,
                Mode = 6
            });
        }

        public bool IsAlreadyMainPlanPurchased(long planId, long userLoginId)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", userLoginId),
                            new SqlParameter("mainPlanId", planId),
                            new SqlParameter("mode", "2")
                            };

            var response = db.Database.SqlQuery<MainPlanBooking_VM>("exec sp_ManageMainPlanBooking @id,@userLoginId,@mainPlanId,@mode", queryParamsGetEvent).FirstOrDefault();

            if (response == null)
            {
                return false;
            }
            else
            {
                DateTime _PlanEndDate = DateTime.ParseExact(response.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (_PlanEndDate < DateTime.UtcNow.Date)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// to get advance details 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessPlan_VM> GetAllAdvanceBusinessPlans(long businessOwnerLoginId)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwnerLoginId),
                            new SqlParameter("planType", "0"),
                            new SqlParameter("mode", "10")
                            };

            var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).ToList();
            return resp;
        }
        
    }
}