using MasterZoneMvc.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System.Data.SqlClient;
using System.Xml.Linq;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using System.Windows;
using MasterZoneMvc.Common;
using System.Numerics;
using MasterZoneMvc.PageTemplateViewModels;

namespace MasterZoneMvc.Repository
{
    public class StoredProcedureRepository
    {
        private MasterZoneDbContext _dbContext;

        public StoredProcedureRepository(MasterZoneDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Not in Use till now
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_Login<T>(SPLogin_Params_VM params_VM)
        {
            List<T> result = new List<T>();

            SqlParameter[] queryParams = new SqlParameter[] {
                new SqlParameter("id", params_VM.Id),
                new SqlParameter("email", params_VM.Email),
                new SqlParameter("password", params_VM.Password),
                new SqlParameter("mode", params_VM.Mode)
                };

            result = _dbContext.Database.SqlQuery<T>("exec sp_Login @id,@email,@password,@mode", queryParams).ToList();
            return result;
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateOrder 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateOrder_Get<T>(SP_InsertUpdateOrder_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@itemId", params_VM.ItemId),
                            new SqlParameter("@itemType", params_VM.ItemType),
                            new SqlParameter("@onlinePayment", params_VM.OnlinePayment),
                            new SqlParameter("@paymentMethod", params_VM.PaymentMethod),
                            new SqlParameter("@couponId", params_VM.CouponId),
                            new SqlParameter("@couponDiscountValue", params_VM.CouponDiscountValue),
                            new SqlParameter("@totalDiscount", params_VM.TotalDiscount),
                            new SqlParameter("@isTaxable", params_VM.IsTaxable),
                            new SqlParameter("@gst", params_VM.Gst),
                            new SqlParameter("@totalAmount", params_VM.TotalAmount),
                            new SqlParameter("@status", params_VM.Status),
                            new SqlParameter("@mode", params_VM.Mode),
                            new SqlParameter("@ownerUserLoginId",params_VM.OwnerUserLoginId)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateOrder @id, @userLoginId, @itemId, @itemType, @onlinePayment, @paymentMethod, @couponId, @couponDiscountValue, @totalDiscount, @isTaxable, @gst, @totalAmount, @status, @mode, @ownerUserLoginId", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdatePlanBooking
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdatePlanBooking_Get<T>(SP_InsertUpdatePlanBooking_Params_VM params_VM, int plantype)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@orderId", params_VM.OrderId),
                            new SqlParameter("@plantype", plantype),
                            new SqlParameter("@planId", params_VM.PlanId),
                            new SqlParameter("@studentUserLoginId", params_VM.StudentUserLoginId),
                            new SqlParameter("@planStartDate", params_VM.PlanStartDate),
                            new SqlParameter("@planEndDate", params_VM.PlanEndDate),
                            new SqlParameter("@planBookingType", params_VM.PlanBookingType),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdatePlanBooking @id, @orderId, @plantype, @planId, @studentUserLoginId, @planStartDate, @planEndDate,@planBookingType,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        ///  To Insert Main Plan Booking Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateMainPlanBooking_Get<T>(SP_InsertUpdateMainPackageBooking_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("orderId", params_VM.OrderId),
                            new SqlParameter("planId", params_VM.PlanId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("planStartDate", params_VM.PlanStartDate),
                            new SqlParameter("planEndDate", params_VM.PlanEndDate),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("status", params_VM.Status)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateMainPackageBooking @id, @orderId, @planId, @businessOwnerLoginId, @planStartDate, @planEndDate,@mode,@status", queryParams).FirstOrDefault();
        }




        /// <summary>
        /// Call stored-procedure sp_InsertUpdatePaymentResponse
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdatePaymentResponse_Get<T>(SP_InsertUpdatePaymentResponse_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@orderId", params_VM.OrderId),
                            new SqlParameter("@provider", params_VM.Provider),
                            new SqlParameter("@responseStatus", params_VM.ResponseStatus),
                            new SqlParameter("@transactionID", params_VM.TransactionID),
                            new SqlParameter("@amount", params_VM.Amount),
                            new SqlParameter("@isApproved", params_VM.IsApproved),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@method", params_VM.Method),
                            new SqlParameter("@submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdatePaymentResponse @id,@orderId,@provider,@responseStatus,@transactionID,@amount,@isApproved,@description,@method,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessPlans
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_ManageBusinessPlans_Get<T>(SP_ManageBusinessPlans_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("planType", params_VM.PlanType),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).FirstOrDefault();
        }







        /// <summary>
        /// Call stored-procedure sp_ManageCoupon
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Single Model Result</returns>
        public T SP_ManageCoupon_Get<T>(SP_ManageCoupon_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("couponCode", params_VM.CouponCode),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            var query = _dbContext.Database.SqlQuery<T>("exec sp_ManageCoupon @id,@userLoginId,@businessOwnerLoginId,@couponCode,@mode", queryParams);

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageCoupon
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>List List<typeparamref name="T"/> Result</returns>
        public List<T> SP_ManageCoupon_GetAll<T>(SP_ManageCoupon_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("couponCode", params_VM.CouponCode),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            var query = _dbContext.Database.SqlQuery<T>("exec sp_ManageCoupon @id,@userLoginId,@businessOwnerLoginId,@couponCode,@mode", queryParams);
            return query.ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageCouponConsumption
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Single Model Result</returns>
        public T SP_ManageCouponConsumption_Get<T>(SP_ManageCouponConsumption_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@consumerUserLoginId", params_VM.ConsumerUserLoginId),
                            new SqlParameter("@couponCode", params_VM.CouponCode),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCouponConsumption @id,@consumerUserLoginId,@couponCode,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageCouponConsumption
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>List List<typeparamref name="T"/> Result</returns>
        public List<T> SP_ManageCouponConsumption_GetAll<T>(SP_ManageCouponConsumption_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@consumerUserLoginId", params_VM.ConsumerUserLoginId),
                            new SqlParameter("@couponCode", params_VM.CouponCode),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCouponConsumption @id,@consumerUserLoginId,@couponCode,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageCouponConsumption
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Single Model Result</returns>
        public T SP_InsertUpdateEventBooking<T>(SP_InsertUpdateEventBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@orderId", params_VM.OrderId),
                            new SqlParameter("@eventId", params_VM.EventId),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@eventQRCodeTicket", params_VM.EventQRCodeTicket),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateEventBooking @id, @orderId, @eventId, @userLoginId, @eventQRCodeTicket, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateClassBooking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateClassBooking_Get<T>(SP_InsertUpdateClassBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@orderId", params_VM.OrderId),
                            new SqlParameter("@classId", params_VM.ClassId),
                            new SqlParameter("@UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("@classQRCode", params_VM.ClassQRCode),
                            new SqlParameter("@classStartDate",params_VM.ClassStartDate),
                            new SqlParameter("@classEndDate",params_VM.ClassEndDate),
                            new SqlParameter("@mode", params_VM.Mode),
                            new SqlParameter("@batchId", params_VM.BatchId)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateClassBooking @id, @orderId, @classId, @UserLoginId,@classQRCode,@classStartDate,@classEndDate,@mode,@batchId", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateMenuBooking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateMenu_Get<T>(SP_InsertUpdateMenu_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@parentMenuId", params_VM.ParentMenuId),
                            new SqlParameter("@name", params_VM.Name),
                            new SqlParameter("@image", params_VM.Image),
                            new SqlParameter("@pageLink", params_VM.PageLink),
                            new SqlParameter("@isActive",params_VM.IsActive),
                            new SqlParameter("@tag",params_VM.Tag),
                            new SqlParameter("@submittedByLoginId",params_VM.SubmittedByLoginId),
                            new SqlParameter("@mode", params_VM.Mode),
                            new SqlParameter("@isShowOnHomePage", params_VM.IsShowOnHomePage),
                            new SqlParameter("@sortOrder", params_VM.SortOrder)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateMenu @id,@parentMenuId,@name,@image,@pageLink,@isActive,@tag,@submittedByLoginId,@mode,@isShowOnHomePage,@sortOrder", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageMenu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageMenu_Get<T>(SP_ManageMenu_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@parentMenuId", params_VM.ParentMenuId),
                            new SqlParameter("@tag",params_VM.Tag),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMenu @id,@parentMenuId,@tag,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageMenu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Retuens List</returns>
        public List<T> SP_ManageMenu_GetAll<T>(SP_ManageMenu_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@parentMenuId", params_VM.ParentMenuId),
                            new SqlParameter("@tag",params_VM.Tag),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMenu @id,@parentMenuId,@tag,@mode", queryParams).ToList();
        }


        /// <summary>
        /// Call stored-procedure sp_InsertUpdateEnquiries
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateEnquiries_Get<T>(SP_InsertUpdateEnquiries_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                new SqlParameter("id", params_VM.Id),
                new SqlParameter("userLoginId", params_VM.UserLoginId),
                new SqlParameter("name", params_VM.Name),
                new SqlParameter("gender", params_VM.Gender),
                new SqlParameter("email", params_VM.Email),
                new SqlParameter("dob", params_VM.DOB),
                new SqlParameter("phoneNumber", params_VM.PhoneNumber),
                new SqlParameter("alternatePhoneNumber", params_VM.AlternatePhoneNumber),
                new SqlParameter("address", params_VM.Address),
                new SqlParameter("activity", params_VM.Activity),
                new SqlParameter("levelId", params_VM.LevelId),
                new SqlParameter("businessPlanId", params_VM.BusinessPlanId),
                new SqlParameter("classId", params_VM.ClassId),
                new SqlParameter("startFromDate", params_VM.StartFromDate),
                new SqlParameter("status", params_VM.Status),
                new SqlParameter("staffId", params_VM.StaffId),
                new SqlParameter("followUpDate", params_VM.FollowUpDate),
                new SqlParameter("notes", params_VM.Notes),
                new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                new SqlParameter("mode", params_VM.Mode)
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateEnquries @id,@userLoginId,@name,@gender,@email,@dob,@phoneNumber,@alternatePhoneNumber,@address,@activity,@levelId,@businessPlanId,@classId,@startFromDate,@status,@staffId,@followUpDate,@notes,@submittedByLoginId,@mode ", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageEnquiries
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageEnquiries_Get<T>(SP_ManageEnquiries_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEnquries @id,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageEnquiries
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Retuens List</returns>
        public List<T> SP_ManageEnquiries_GetAll<T>(SP_ManageEnquiries_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEnquries @id,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetRecordByMenu
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM"></param>
        /// <returns>List</returns>
        public List<T> SP_GetRecordByMenu_GetAll<T>(SP_GetRecordByMenu_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("menutag",params_VM.MenuTag),
                            new SqlParameter("categoryKey", params_VM.CategoryKey),
                            new SqlParameter("itemType", params_VM.ItemType),
                            new SqlParameter("city",params_VM.City),
                            new SqlParameter("latitude",params_VM.Latitude),
                            new SqlParameter("longitude",params_VM.Longitude),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("categorySearch", params_VM.CategorySearch),
                            new SqlParameter("searchType", params_VM.SearchType),
                            new SqlParameter("searchValue", params_VM.SearchValue)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetRecordByMenu @id,@userLoginId,@menutag,@categoryKey,@itemType,@city,@latitude,@longitude,@lastRecordId,@recordLimit,@mode,@categorySearch,@searchType,@searchValue", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetBusinessPlanRecord
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters VM</param>
        /// <returns>List</returns>
        public List<T> SP_GetAllBusinessPlans_GetAll<T>(SP_GetBusinessPlanRecord_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("city",params_VM.City),
                            new SqlParameter("latitude", params_VM.Latitude),
                            new SqlParameter("longitude", params_VM.Longitude),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit) ,
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetBusinessPlanRecord @id,@city,@latitude,@longitude,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetTrainingRecord
        /// </summary>
        /// <typeparam name="T">Return Type Class </typeparam>
        /// <param name="params_VM">Stored Procedure Parameters</param>
        /// <returns>List</returns>
        public List<T> SP_GetAllBusinessTraining_GetAll<T>(SP_GetTrainingRecord_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("city",params_VM.City),
                            new SqlParameter("latitude",params_VM.Latitude),
                            new SqlParameter("longitude",params_VM.Longitude),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit) ,
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetTrainingRecord @id,@city,@latitude,@longitude,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageUserLogin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageUserLogin_Get<T>(SP_ManageUserLogin_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("uniqueUserId", params_VM.UniqueUserId),
                             new SqlParameter("email",params_VM.Email),
                            new SqlParameter("password",params_VM.Password),
                            new SqlParameter("roleId",params_VM.RoleId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserLogin @id,@uniqueUserId,@email,@password,@roleId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageUserLogin
        /// </summary>
        /// <typeparam name="T">Return Type Object</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters</param>
        /// <returns>Returns List</returns>
        public List<T> SP_ManageUserLogin_GetAll<T>(SP_ManageUserLogin_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("uniqueUserId", params_VM.UniqueUserId),
                             new SqlParameter("email",params_VM.Email),
                            new SqlParameter("password",params_VM.Password),
                            new SqlParameter("roleId",params_VM.RoleId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserLogin @id,@uniqueUserId,@email,@password,@roleId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessOwner 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Owner Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_InsertUpdateBusinessOwner<T>(SP_InsertUpdateBusinessOwner_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("id", params_VM.Id),
                        new SqlParameter("email", params_VM.Email),
                        new SqlParameter("password",params_VM.Password),
                        new SqlParameter("phoneNumber", params_VM.PhoneNumber),
                        new SqlParameter("phoneNumber_CountryCode", params_VM.PhoneNumberCountryCode),
                        new SqlParameter("roleId", "4"),
                        new SqlParameter("businessName", params_VM.BusinessName),
                        new SqlParameter("firstName", params_VM.FirstName),
                        new SqlParameter("lastName", params_VM.LastName),
                        new SqlParameter("address", params_VM.Address),
                        new SqlParameter("DOB", params_VM.DOB),
                        new SqlParameter("rejectionReason", params_VM.RejectionReason),
                        new SqlParameter("businessCategoryId", params_VM.BusinessCategoryId),
                        new SqlParameter("IsPrimeMember", params_VM.IsPrimeMember),
                        new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                        new SqlParameter("mode", params_VM.Mode),
                        new SqlParameter("businessSubCategoryId", params_VM.BusinessSubCategoryId),
                        new SqlParameter("verified",params_VM.Verified),
                        new SqlParameter("uniqueUserId",params_VM.UniqueUserId)
                        };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessOwner @id,@email,@password,@phoneNumber,@phoneNumber_CountryCode,@roleId,@businessName,@firstName,@lastName,@address,@DOB,@rejectionReason,@businessCategoryId,@IsPrimeMember,@submittedByLoginId,@mode,@businessSubCategoryId,@verified,@uniqueUserId", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessOwner 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Owner Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_InsertUpdateStudent<T>(SP_InsertUpdateStudent_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[]
                                    {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("email", params_VM.Email),
                    new SqlParameter("password", params_VM.Password),
                    new SqlParameter("phoneNumber", params_VM.PhoneNumber),
                    new SqlParameter("phoneNumber_CountryCode", params_VM.PhoneNumberCountryCode),
                    new SqlParameter("roleId", params_VM.RoleId),
                    new SqlParameter("firstname", params_VM.FirstName),
                    new SqlParameter("lastname", params_VM.LastName),
                    new SqlParameter("profileimage", params_VM.ProfileImage),
                    new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                    new SqlParameter("mode", params_VM.Mode),
                    new SqlParameter("uniqueUserId", params_VM.UniqueUserId),
                    new SqlParameter("gender", params_VM.Gender),
                    new SqlParameter("blockReason", params_VM.BlockReason),
                    new SqlParameter("businessStudentProfileImage", params_VM.BusinessStudentProfileImage),
                    new SqlParameter("studentUserLoginId", params_VM.StudentUserLoginId),
                    new SqlParameter("otp",params_VM.OTP)
                                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateStudent @id,@email,@password,@phoneNumber,@phoneNumber_CountryCode,@roleId,@firstname,@lastname,@profileimage,@submittedByLoginId,@mode,@uniqueUserId,@gender,@blockReason,@businessStudentProfileImage,@studentUserLoginId,@otp", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessProfile_Get 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Profile Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_InsertUpdateBusinessProfile_Get<T>(SP_InsertUpdateBusinessProfile_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("email", _Params_VM.Email),
                            new SqlParameter("firstName", _Params_VM.FirstName),
                            new SqlParameter("lastName", _Params_VM.LastName),
                            new SqlParameter("profileImage", _Params_VM.ProfileImage),
                            new SqlParameter("businessLogo", _Params_VM.BusinessLogo),
                            new SqlParameter("about", _Params_VM.About),
                            new SqlParameter("address", _Params_VM.Address),
                            new SqlParameter("country", _Params_VM.Country),
                            new SqlParameter("state", _Params_VM.State),
                            new SqlParameter("city", _Params_VM.City),
                            new SqlParameter("pinCode", _Params_VM.PinCode),
                            new SqlParameter("landMark",_Params_VM.LandMark),
                            new SqlParameter("latitude",_Params_VM.Latitude),
                            new SqlParameter("longitude",_Params_VM.Longitude),
                            new SqlParameter("phoneNumber",_Params_VM.PhoneNumber),
                            new SqlParameter("documentTitle",_Params_VM.DocumentTitle),
                            new SqlParameter("documentUploadedFiles",   _Params_VM.DocumentUploadedFile),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode",_Params_VM.Mode),
                            new SqlParameter("businessName",_Params_VM.BusinessName),
                            new SqlParameter("experience", _Params_VM.Experience),
                            new SqlParameter("privacy_UniqueUserId",_Params_VM.Privacy_UniqueUserId),
                            new SqlParameter("officialWebSiteUrl", _Params_VM.OfficialWebSiteUrl),
                            new SqlParameter("faceBookProfileLink", _Params_VM.FacebookProfileLink),
                            new SqlParameter("linkedInProfileLink", _Params_VM.LinkedInProfileLink),
                            new SqlParameter("instagramProfileLink", _Params_VM.InstagramProfileLink),
                            new SqlParameter("twitterProfileLink", _Params_VM.TwitterProfileLink),
                            new SqlParameter("coverImage", _Params_VM.CoverImage)

                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessProfile @id,@email,@firstName,@lastName,@profileImage,@businessLogo,@about,@address,@country,@state,@city,@pinCode,@landMark,@latitude,@longitude,@phoneNumber,@documentTitle,@documentUploadedFiles,@submittedByLoginId,@mode,@businessName,@experience,@privacy_UniqueUserId,@officialWebSiteUrl,@faceBookProfileLink,@linkedInProfileLink,@instagramProfileLink,@twitterProfileLink,@coverImage", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateStaffProfile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateStaffProfile_Get<T>(SP_InsertUpdateStaffProfile_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId",_Params_VM.BusinessOwnerLoginId),
                            new SqlParameter("email", _Params_VM.Email),
                            new SqlParameter("firstName", _Params_VM.FirstName),
                            new SqlParameter("lastName", _Params_VM.LastName),
                            new SqlParameter("profileImage", _Params_VM.ProfileImage),
                            new SqlParameter("address", _Params_VM.Address),
                            new SqlParameter("country", _Params_VM.Country),
                            new SqlParameter("state", _Params_VM.State),
                            new SqlParameter("city", _Params_VM.City),
                            new SqlParameter("pinCode", _Params_VM.PinCode),
                            new SqlParameter("landMark",_Params_VM.LandMark),
                            new SqlParameter("latitude",_Params_VM.Latitude),
                            new SqlParameter("longitude",_Params_VM.Longitude),
                            new SqlParameter("phoneNumber",_Params_VM.PhoneNumber),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("faceBookProfileLink", _Params_VM.FacebookProfileLink),
                            new SqlParameter("linkedInProfileLink", _Params_VM.LinkedInProfileLink),
                            new SqlParameter("instagramProfileLink", _Params_VM.InstagramProfileLink),
                            new SqlParameter("twitterProfileLink", _Params_VM.TwitterProfileLink),
                            new SqlParameter("dob", _Params_VM.DOB),
                            new SqlParameter("about", _Params_VM.About),
                            new SqlParameter("mode",_Params_VM.Mode)
                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateStaffProfile @id,@businessOwnerLoginId,@email,@firstName,@lastName,@profileImage,@address,@country,@state,@city,@pinCode,@landMark,@latitude,@longitude,@phoneNumber,@submittedByLoginId,@faceBookProfileLink,@linkedInProfileLink,@instagramProfileLink,@twitterProfileLink,@dob,@about,@mode", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Call stored-procedure sp_ManageUserCertificate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_Params_VM"></param>
        /// <returns>Single Object</returns>
        public T SP_ManageUserCertificates_Get<T>(SP_ManageUserCertificates_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("userLoginId",_Params_VM.UserLoginId),
                            new SqlParameter("mode",_Params_VM.Mode),
                            new SqlParameter("certificateNumber",_Params_VM.CertificateNumber)
                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_ManageUserCertificates @id,@userLoginId,@mode,@certificateNumber", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Call stored-procedure sp_ManageUserCertificate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_Params_VM"></param>
        /// <returns>List Object</returns>
        public List<T> SP_ManageUserCertificates_GetAll<T>(SP_ManageUserCertificates_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("userLoginId",_Params_VM.UserLoginId),
                            new SqlParameter("mode",_Params_VM.Mode),
                            new SqlParameter("certificateNumber",_Params_VM.CertificateNumber)
                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_ManageUserCertificates @id,@userLoginId,@mode,@certificateNumber", queryParams).ToList();
            return resp;
        }

        // TODO: [Meena Integration] change the Name 

        /// <summary>
        /// Call stored-procedure sp_GetAllStudentCourseDetail - Get List
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Owner Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_GetAllStudentCourseDetail_GetAll<T>(SP_GetAllStudentCourseDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit) ,
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllStudentCourseDetail @id,@userLoginId,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetAllStudentEventList - Get List
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_GetAllStudentEventList<T>(SP_GetAllStudentEventList_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit) ,
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllStudentEventList @id,@userLoginId,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateOrder 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public List<T> SP_GetAllStudentGroupRecord<T>(SP_GetAllStudentGroupRecord_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllStudentGroupRecord @id,@userLoginId,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetInstructorProfileDetail - Get List
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public List<T> SP_GetInstructorProfileDetail<T>(SP_GetInstructorProfileDetail_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetInstructorProfileDetail @id,@userLoginId,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateEnquiresFollowsup 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateEnquiriesFollowup_Get<T>(SP_InsertUpdateEnquiriesFollowsup_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@enquiryId",params_VM.EnquiryId),
                            new SqlParameter("@followedby ", params_VM.FollowedbyLoginId),
                            new SqlParameter("@comments", params_VM.Comments),
                            new SqlParameter("@submittedByLoginId",params_VM.SubmittedByLoginId),
                            new SqlParameter("@mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateEnquiresFollowsup @id, @enquiryId, @followedby, @comments, @submittedByLoginId,  @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageStudent - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageStudent_Get<T>(SP_ManageStudent_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageStudent @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageStudent - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageStudent_GetAll<T>(SP_ManageStudent_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageStudent @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusiness - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageBusiness_GetAll<T>(SP_ManageBusiness_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusiness - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageBusiness_Get<T>(SP_ManageBusiness_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageTraining - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageTraining_GetAll<T>(SP_ManageTraining_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageTraining -Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageTraining_Get<T>(SP_ManageTraining_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageTraining @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessPlanDetailFor SuperAdmin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageBusinessPlans_GetAll<T>(SP_ManageBusinessPlans_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                           new SqlParameter("planType",params_VM.PlanType),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessImages - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageBusinessImages_GetAll<T>(SP_ManageBusinessImages_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessImages @id,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessVideos - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageBusinessVideos_GetAll<T>(SP_ManageBusinessVideos_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", params_VM.Id),
                                new SqlParameter("userLoginId", params_VM.UserLoginId),
                                new SqlParameter("mode", params_VM.Mode),
                };
            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessVideos @id,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessVideos - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public T SP_ManageBusinessVideos_Get<T>(SP_ManageBusinessVideos_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                                new SqlParameter("id", params_VM.Id),
                                new SqlParameter("userLoginId", params_VM.UserLoginId),
                                new SqlParameter("mode", params_VM.Mode),
            };
            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessVideos @id,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessVideos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateBusinessVideos<T>(SP_InsertUpdateBusinessVideos_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("videoTitle", params_VM.VideoTitle),
                            new SqlParameter("videoLink", params_VM.VideoLink),
                            new SqlParameter("thumbNail", params_VM.Thumbnail),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("businessContentCategoryId", params_VM.BusinessContentVideoCategoryId)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessVideos @id,@userLoginId,@videoTitle,@videoLink,@thumbNail,@description,@submittedByLoginId,@mode,@businessContentCategoryId", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessImages
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateBusinessImages<T>(SP_InsertUpdateBusinessImages_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("imageTitle", params_VM.ImageTitle),
                            new SqlParameter("image",params_VM.Image),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessImages @id,@userLoginId,@imageTitle,@image ,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Call stored-procedure sp_ManageMainPlanBooking - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageMainPlanBooking_GetAll<T>(SP_ManageMainPlanBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("userLoginId",params_VM.UserLoginId),
                    new SqlParameter("mainPlanId",params_VM.MainPlanId),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMainPlanBooking @id,@userLoginId,@mainPlanId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageMainPlanBooking - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageMainPlanBooking_Get<T>(SP_ManageMainPlanBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("userLoginId",params_VM.UserLoginId),
                    new SqlParameter("mainPlanId",params_VM.MainPlanId),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMainPlanBooking @id,@userLoginId,@mainPlanId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBatch
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBatch_Get<T>(SP_InsertUpdateBatch_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@name", params_VM.Name),
                            new SqlParameter("@startTime24HF", params_VM.StartTime24HF),
                            new SqlParameter("@endTime24HF", params_VM.EndTime24HF),
                            new SqlParameter("@studentMaxStrength", params_VM.StudentMaxStrength),
                            new SqlParameter("@instructorLoginId", params_VM.InstructorLoginId),
                            new SqlParameter("@groupId", params_VM.GroupId),
                            new SqlParameter("@classDuration", params_VM.ClassDuration),
                            new SqlParameter("@submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("@mode", params_VM.Mode),
                            new SqlParameter("@status", params_VM.Status)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBatch @id,@userLoginId,@name,@startTime24HF,@endTime24HF,@studentMaxStrength,@instructorLoginId,@groupId,@classDuration,@submittedByLoginId,@mode,@status", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBatch - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageBatch_Get<T>(SP_ManageBatch_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("masterid", params_VM.MasterId),
                    new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                    new SqlParameter("searchtext",params_VM.SearchText),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBatch @id,@masterid,@businessOwnerLoginId,@searchtext,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBatch - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageBatch_GetAll<T>(SP_ManageBatch_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("masterid", params_VM.MasterId),
                    new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                    new SqlParameter("searchtext",params_VM.SearchText),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBatch @id,@masterid,@businessOwnerLoginId,@searchtext,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateClassCategoryType
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateClassCategoryType_Get<T>(SP_InsertUpdateClassCategoryType_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessCategoryId", params_VM.BusinessCategoryId),
                            new SqlParameter("parentClassCategoryTypeId", params_VM.ParentClassCategoryTypeId),
                            new SqlParameter("name", params_VM.Name),
                            new SqlParameter("image", params_VM.Image),
                            new SqlParameter("isActive", params_VM.IsActive),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("description", params_VM.Description),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateClassCategoryType @id,@businessCategoryId,@parentClassCategoryTypeId,@name,@image,@isActive,@submittedByLoginId,@mode,@description", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClassCategoryType - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageClassCategoryType_Get<T>(SP_ManageClassCategoryType_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("businessCategoryId",params_VM.BusinessCategoryId),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassCategoryType @id,@businessCategoryId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClassCategoryType - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageClassCategoryType_GetAll<T>(SP_ManageClassCategoryType_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("businessCategoryId",params_VM.BusinessCategoryId),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassCategoryType @id,@businessCategoryId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateClassBatches
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateClassBatches_Get<T>(SP_InsertUpdateClassBatches_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("@classId", params_VM.ClassId),
                            new SqlParameter("@batchId", params_VM.BatchId),
                            new SqlParameter("@mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateClassBatches @id,@businessOwnerLoginId,@classId,@batchId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClassBatches - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageClassBatches_Get<T>(SP_ManageClassBatches_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                        new SqlParameter("@classId", params_VM.ClassId),
                        new SqlParameter("@batchId", params_VM.BatchId),
                        new SqlParameter("@mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassBatches @id,@businessOwnerLoginId,@classId,@batchId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClassBatches - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageClassBatches_GetAll<T>(SP_ManageClassBatches_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                        new SqlParameter("@classId", params_VM.ClassId),
                        new SqlParameter("@batchId", params_VM.BatchId),
                        new SqlParameter("@mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassBatches @id,@businessOwnerLoginId,@classId,@batchId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClass - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageClass_Get<T>(SP_ManageClass_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("id", params_VM.Id),
                        new SqlParameter("userLoginId", params_VM.UserLoginId),
                         new SqlParameter("dayname", "0"),
                        new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClass - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageClass_GetAll<T>(SP_ManageClass_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@userLoginId", params_VM.UserLoginId),
                        new SqlParameter("@dayname", params_VM.dayNames),
                        new SqlParameter("@mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClass @id,@userLoginId,@dayname,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetClassListByFilter - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_GetClassListByFilter_GetAll<T>(SP_GetClassListByFilter_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                        new SqlParameter("@searchValue", params_VM.SearchValue),
                        new SqlParameter("@classCategoryTypeId", params_VM.ClassCategoryTypeId),
                        new SqlParameter("@classMode", params_VM.ClassMode),
                        new SqlParameter("@mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetClassListByFilter @id,@businessOwnerLoginId,@searchValue,@classCategoryTypeId,@classMode,@mode", queryParams).ToList();
        }


        /// <summary>
        /// Call stored-procedure sp_ManageBusinessContentVideoCategories - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageBusinessContentVideoCategories_Get<T>(SP_ManageBusinessContentVideoCategories_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@mode", params_VM.Mode)
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentVideoCategories @id,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessContentVideoCategories - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageBusinessContentVideoCategories_GetAll<T>(SP_ManageBusinessContentVideoCategories_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@mode", params_VM.Mode)
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentVideoCategories @id,@mode", queryParams).ToList();
        }


        /// <summary>
        /// Call stored-procedure sp_ManageClassBooking - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageClassBooking_Get<T>(SP_ManageClassBooking_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("classId", params_VM.ClassId),
                            new SqlParameter("batchId", params_VM.BatchId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("joiningDate", params_VM.JoiningDate)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassBooking @id,@userLoginId,@classId,@batchId,@mode,@joiningDate", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageClassBooking - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageClassBooking_GetAll<T>(SP_ManageClassBooking_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("classId", params_VM.ClassId),
                            new SqlParameter("batchId", params_VM.BatchId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("joiningDate", params_VM.JoiningDate)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassBooking @id,@userLoginId,@classId,@batchId,@mode,@joiningDate", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageItemFeatures - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageItemFeatures_GetAll<T>(SP_ManageItemFeatures_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("itemId", params_VM.ItemId),
                            new SqlParameter("itemType", params_VM.ItemType),
                            new SqlParameter("featureId", params_VM.FeatureId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageItemFeatures @id,@itemId,@itemType,@featureId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageItemFeatures - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public T SP_ManageItemFeatures_Get<T>(SP_ManageItemFeatures_Params_VM params_VM)
        {
            return SP_ManageItemFeatures_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateItemFeatures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateItemFeatures<T>(SP_InsertUpdateItemFeatures_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                new SqlParameter("id", params_VM.Id),
                new SqlParameter("itemId", params_VM.ItemId),
                new SqlParameter("itemType", params_VM.ItemType),
                new SqlParameter("featureId", params_VM.FeatureId),
                new SqlParameter("isLimited", params_VM.IsLimited),
                new SqlParameter("limit", params_VM.Limit),
                new SqlParameter("mode", params_VM.Mode)
                };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateItemFeatures @id,@itemId,@itemType,@featureId,@isLimited,@limit,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateLicense 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateLicense<T>(SP_InsertUpdateLicense_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("certificateId", params_VM.CertificateId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("certificateImage", params_VM.CertificateImage),
                            new SqlParameter("licenseLogoImage", params_VM.LicenseLogoImage),
                            new SqlParameter("signatureImage", params_VM.SignatureImage),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("isPaid",params_VM.IsPaid),
                            new SqlParameter("status",params_VM.Status),
                            new SqlParameter("commissionType",params_VM.CommissionType),
                            new SqlParameter("commissionValue",params_VM.CommissionValue),
                            new SqlParameter("timePeriod",params_VM.TimePeriod),
                            new SqlParameter("licenseDuration",params_VM.LicenseDuration),
                            new SqlParameter("achievingOrder",params_VM.AchievingOrder),
                            new SqlParameter("licensePermissions", params_VM.LicensePermissions),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("signature2Image", params_VM.Signature2Image),
                            new SqlParameter("signature3Image", params_VM.Signature3Image),
                            new SqlParameter("price", params_VM.Price),
                            new SqlParameter("gstPercent", params_VM.GSTPercent),
                            new SqlParameter("gstDescription", params_VM.GSTDescription),
                            new SqlParameter("minSellingPrice", params_VM.MinSellingPrice),
                            new SqlParameter("licenseCertificateHTMLContent", params_VM.LicenseCertificateHTMLContent),
                            new SqlParameter("isLicenseToTeach", params_VM.IsLicenseToTeach),
                            new SqlParameter("licenseToTeach_Type", params_VM.LicenseToTeach_Type),
                            new SqlParameter("licenseToTeach_DisplayName", params_VM.LicenseToTeach_DisplayName),
                            new SqlParameter("masterPro", params_VM.MasterPro)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateLicense @id,@certificateId,@title,@certificateImage,@licenseLogoImage,@signatureImage,@description,@isPaid,@status,@commissionType,@commissionValue,@timePeriod,@licenseDuration,@achievingOrder,@licensePermissions,@submittedByLoginId,@mode,@signature2Image,@signature3Image,@price,@gstPercent,@gstDescription,@minSellingPrice,@licenseCertificateHTMLContent,@isLicenseToTeach,@licenseToTeach_Type,@licenseToTeach_DisplayName,@masterPro", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageLicense - Get List
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageLicense_GetAll<T>(SP_ManageLicense_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("certificateId", params_VM.CertificateId),
                            new SqlParameter("achievingOrder", params_VM.AchievingOrder),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageLicense @id,@certificateId,@achievingOrder,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageLicense - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public T SP_ManageLicense_Get<T>(SP_ManageLicense_Params_VM params_VM)
        {
            return SP_ManageLicense_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageFieldTypeCatalog - Get List
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageFieldTypeCatalog_GetAll<T>(SP_ManageFieldTypeCatalog_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("panelTypeId", params_VM.PanelTypeId),
                            new SqlParameter("keyName", params_VM.KeyName),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageFieldTypeCatalog @id,@panelTypeId,@keyName,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageFieldTypeCatalog - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public T SP_ManageFieldTypeCatalog_Get<T>(SP_ManageFieldTypeCatalog_Params_VM params_VM)
        {
            return SP_ManageFieldTypeCatalog_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageCertificate - Get List
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageCertificate_GetAll<T>(SP_ManageCertificate_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("searchtext", params_VM.Searchtext),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCertificate @id,@userLoginId,@businessOwnerLoginId,@searchtext,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageCertificate - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageCertificate_Get<T>(SP_ManageCertificate_Params_VM params_VM)
        {
            return SP_ManageCertificate_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateLicenseBooking
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateLicenseBooking_Get<T>(SP_InsertUpdateLicenseBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("orderId", params_VM.OrderId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("licenseId", params_VM.LicenseId),
                            new SqlParameter("quantity", params_VM.Quantity),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateLicenseBooking @id,@orderId,@businessOwnerLoginId,@licenseId,@quantity,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageLicenseBooking - Get List
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageLicenseBooking_GetAll<T>(SP_ManageLicenseBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("licenseId", params_VM.LicenseId),
                            new SqlParameter("certificateId", params_VM.CertificateId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("trainingId", params_VM.TrainingId),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageLicenseBooking @id,@businessOwnerLoginId,@licenseId,@certificateId,@mode,@trainingId", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageLicenseBooking - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageLicenseBooking_Get<T>(SP_ManageLicenseBooking_Params_VM params_VM)
        {
            return SP_ManageLicenseBooking_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessLicenses
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessLicenses_Get<T>(SP_InsertUpdateBusinessLicenses_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("licenseBookingId", params_VM.LicenseBookingId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("licenseId", params_VM.LicenseId),
                            new SqlParameter("quantity", params_VM.Quantity),
                            new SqlParameter("quantityUsed", params_VM.QuantityUsed),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessLicenses @id,@licenseBookingId,@businessOwnerLoginId,@licenseId,@quantity,@quantityUsed,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateApartment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateApartment_Get<T>(SP_InsertUpdateApartment_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("name", params_VM.Name),
                            new SqlParameter("blocks", params_VM.Blocks),
                            new SqlParameter("areas", params_VM.Areas),
                            new SqlParameter("status",params_VM.Status),
                            new SqlParameter("submittedByLoginId",params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateApartment @id,@businessOwnerLoginId,@name,@blocks,@areas,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageApartment_GetAll<T>(SP_ManageApartment_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageApartment @id,@businessOwnerLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageApartment_Get<T>(SP_ManageApartment_Params_VM params_VM)
        {
            return SP_ManageApartment_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateFamilyMember
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateFamilyMember_Get<T>(SP_InsertUpdateFamilyMember_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("firstName", params_VM.FirstName),
                            new SqlParameter("lastName", params_VM.LastName),
                            new SqlParameter("relation", params_VM.Relation),
                            new SqlParameter("profileImage",params_VM.ProfileImage),
                            new SqlParameter("gender",params_VM.Gender),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateFamilyMember @id,@userLoginId,@firstName,@lastName,@relation,@profileImage,@gender,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateApartmentBooking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateApartmentBooking_Get<T>(SP_InsertUpdateApartmentBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("masterId", params_VM.MasterId),
                            new SqlParameter("batchId", params_VM.BatchId),
                            new SqlParameter("apartmentId", params_VM.ApartmentId),
                            new SqlParameter("apartmentBlockId",params_VM.ApartmentBlockId),
                            new SqlParameter("flatOrVillaNumber",params_VM.FlatOrVillaNumber),
                            new SqlParameter("phase",params_VM.Phase),
                            new SqlParameter("lane",params_VM.Lane),
                            new SqlParameter("occupantType",params_VM.OccupantType),
                            new SqlParameter("apartmentAreaId",params_VM.ApartmentAreaId),
                            new SqlParameter("activity",params_VM.Activity),
                            new SqlParameter("submittedByLoginId",params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateApartmentBooking @id,@businessOwnerLoginId,@userLoginId,@masterId,@batchId,@apartmentId,@apartmentBlockId,@flatOrVillaNumber,@phase,@lane,@occupantType,@apartmentAreaId,@activity,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageFamilyMember Get-List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageFamilyMember_GetAll<T>(SP_ManageFamilyMember_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageFamilyMember @id,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageFamilyMember Get-Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageFamilyMember_Get<T>(SP_ManageFamilyMember_Params_VM params_VM)
        {
            return SP_ManageFamilyMember_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentBooking Get-List
        /// </summary>
        /// <typeparam name="T">View-Model Class for Data-Binding</typeparam>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageApartmentBooking_GetAll<T>(SP_ManageApartmentBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("apartmentId", params_VM.ApartmentId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageApartmentBooking @id,@businessOwnerLoginId,@userLoginId,@apartmentId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentBooking Get-Single
        /// </summary>
        /// <typeparam name="T">View-Model Class for Data-Binding</typeparam>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageApartmentBooking_Get<T>(SP_ManageApartmentBooking_Params_VM params_VM)
        {
            return SP_ManageApartmentBooking_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateApartmentBlock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateApartmentBlock_Get<T>(SP_InsertUpdateApartmentBlock_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("apartmentId", params_VM.ApartmentId),
                            new SqlParameter("name", params_VM.Name),
                            new SqlParameter("isActive",params_VM.IsActive),
                            new SqlParameter("submittedByLoginId",params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateApartmentBlock @id,@businessOwnerLoginId,@apartmentId,@name,@isActive,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentBlock Get-List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageApartmentBlock_GetAll<T>(SP_ManageApartmentBlock_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("apartmentId", params_VM.ApartmentId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageApartmentBlock @id,@businessOwnerLoginId,@apartmentId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentBlock Get-Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageApartmentBlock_Get<T>(SP_ManageApartmentBlock_Params_VM params_VM)
        {
            return SP_ManageApartmentBlock_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateApartmentArea
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateApartmentArea_Get<T>(SP_InsertUpdateApartmentArea_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("apartmentId", params_VM.ApartmentId),
                            new SqlParameter("name", params_VM.Name),
                            new SqlParameter("subTitle", params_VM.SubTitle),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("price", params_VM.Price),
                            new SqlParameter("apartmentImage", params_VM.ApartmentImage),
                            new SqlParameter("isActive",params_VM.IsActive),
                            new SqlParameter("submittedByLoginId",params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateApartmentArea @id,@businessOwnerLoginId,@apartmentId,@name,@subTitle,@description,@price,@apartmentImage,@isActive,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentArea Get-List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageApartmentArea_GetAll<T>(SP_ManageApartmentArea_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("apartmentId", params_VM.ApartmentId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageApartmentArea @id,@businessOwnerLoginId,@apartmentId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentArea Get-Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageApartmentArea_Get<T>(SP_ManageApartmentArea_Params_VM params_VM)
        {
            return SP_ManageApartmentArea_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageEvent -Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns List</returns>
        public List<T> SP_ManageEvent_GetAll<T>(SP_ManageEvent_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEvent @id,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageEvent -Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageEvent_Get<T>(SP_ManageEvent_Param_VM params_VM)
        {

            return SP_ManageEvent_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// TCall stored-procedure sp_ManageBusinessDistanceLocation -Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessDistanceLocation<T>(SP_ManageBusinessDistanceLocation_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("latitude",params_VM.Latitude),
                            new SqlParameter("longitude",params_VM.Longitude),
                            new SqlParameter("menuTag", params_VM.MenuTag),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("recordLimit", params_VM.RecordedId),
                            new SqlParameter("lastRecordId", params_VM.LastRecordedId)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessDistanceLocation @id,@businessOwnerLoginId,@userLoginId,@latitude,@longitude,@menutag,@mode,@recordLimit,@lastRecordId", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageApartmentArea -Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns List</returns>
        public List<T> SP_ManageBusinessDetailByMenuTag_GetAll<T>(SP_ManageBusinessDetailByMenuTag_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("id", params_VM.Id),
                        new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                        new SqlParameter("userLoginId", params_VM.UserLoginId),
                        new SqlParameter("menuTag", params_VM.MenuTag),
                        new SqlParameter("mode", params_VM.Mode),
                        new SqlParameter("recordLimit", params_VM.RecordLimit),
                        new SqlParameter("lastRecordId", params_VM.LastRecordId),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessDetailByMenuTag @id,@businessOwnerLoginId,@userLoginId,@menuTag,@mode,@recordLimit,@lastRecordId", queryParams).ToList();
        }


        /// <summary>
        /// Training Training Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_GetAllTrainingDetailSearch_GetList<T>(SP_GetAllTrainingDetailSearch_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId",params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit),
                            new SqlParameter("location",params_VM.Location),
                            new SqlParameter("latitude","0"),
                            new SqlParameter("longitude","0"),
                            new SqlParameter("searchkeyword", params_VM.Searchkeyword),
                            new SqlParameter("searchBy", params_VM.SearchBy),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllTrainingDetailSearch @id,@userLoginId,@lastRecordId,@recordLimit,@location,@latitude,@longitude,@searchkeyword,@searchBy,@mode", queryParams).ToList();
        }

        /// <summary>
        ///  Insert-Business Rating Information
        /// </summary>
        public T SP_InsertUpdateReview_GetSingle<T>(SP_InsertUpdateReview_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("itemId ", params_VM.ItemId),
                            new SqlParameter("itemType",params_VM.ItemType),
                            new SqlParameter("rating",params_VM.Rating),
                            new SqlParameter("reviewBody", params_VM.ReviewBody),
                            new SqlParameter("reviewerUserLoginId",params_VM.ReviewerUserLoginId),
                            new SqlParameter("mode",params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateReview @id,@itemId,@itemType,@rating,@reviewBody,@reviewerUserLoginId,@mode", queryParams).FirstOrDefault();
        }
        /// <summary>
        /// To Insert Update  User Content Vedio
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateUserContentVideos<T>(SP_InsertUpdateUserContentVideo_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId",params_VM.UserLoginId),
                            new SqlParameter("videoTitle", params_VM.VideoTitle),
                             new SqlParameter("videoDescription", params_VM.VideoDescription),
                            new SqlParameter("videoLink",params_VM.VideoLink),
                            new SqlParameter("thumbNail",params_VM.VideoThumbnail),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateUserContentVideos @id,@userLoginId,@videoTitle,@videoDescription,@videoLink,@thumbNail ,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get User Content Video Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageUserContentVideos_GetList<T>(SP_ManageUserContentVideos_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("searchtitle", ""),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserContentVideos @id,@lastRecordId,@recordLimit,@searchtitle,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Get User Content Video Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Get user detail by Id </returns>
        public T SP_ManageUserContentVideo_GetSingle<T>(SP_ManageUserContentVideos_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                            new SqlParameter("searchtitle", ""),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserContentVideos @id,@lastRecordId,@recordLimit,@searchtitle,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// To Insert Update  User Content Images
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateUserContentImage<T>(SP_InsertUpdateUserContentImage_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId",params_VM.UserLoginId),
                            new SqlParameter("imageTitle", params_VM.ImageTitle),
                             new SqlParameter("image", params_VM.ImageThumbnail),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateUserContentImages @id,@userLoginId,@imageTitle,@image,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get User Content Image Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageUserContentImage_GetList<T>(SP_ManageUserContentImage_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserContentImage @id,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Get User Content Image Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Get user detail by Id </returns>
        public T SP_ManageUserContentImage_GetSingle<T>(SP_ManageUserContentImage_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserContentImage @id,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        ///  Rating Detail By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageReview<T>(SP_ManageReview_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageReview @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Get Review Rating Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageReview_Get<T>(SP_ManageReview_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageReview @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }


        /// <summary>
        /// To Insert Update  Business Service Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessService_Get<T>(SP_InsertUpdateBusinessService_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",params_VM.Id ),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("featuredImage",params_VM.FeaturedImage),
                            new SqlParameter("icon", params_VM.Icon),
                            new SqlParameter("status",params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("serviceType", params_VM.ServiceType),
                            new SqlParameter("mode", params_VM.Mode),

                };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessService @id,@userLoginId,@title,@description,@featuredImage,@icon,@status,@submittedByLoginId,@serviceType,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  Service Detail  Data by Id
        /// </summary>
        /// <param name="businessServiceId">businessService Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessService_Get<T>(SP_ManageBusinessService_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Get Business  Service Detail 
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessService<T>(SP_ManageBusinessService_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Get Business Content  Service Detail For Yoga (Visitor-Panel) 
        /// </summary>
        /// <param name="">Training Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentService<T>(SP_ManageBusinessService_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Get Business Content Service Detail  Data by BusinessOwnerLoginId For visitor panel
        /// </summary>
        /// <param name="businessownerLoginId">businessownerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessContentService_Get<T>(SP_ManageBusinessService_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// To Insert Update Rating Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateRatingReview_Get<T>(SP_InsertUpdateRatingReview_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id ),
                            new SqlParameter("itemId ", params_VM.ItemId),
                            new SqlParameter("rating",  params_VM.Rating),
                            new SqlParameter("reviewBody", params_VM.ReviewBody),
                            new SqlParameter("reviewerUserLoginId",params_VM.ReviewerUserLoginId),
                            new SqlParameter("mode",1),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateRatingReview @id,@itemId,@rating,@reviewBody,@reviewerUserLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// To Insert Update Rating Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateDanceRatingReview_Get<T>(SP_InsertUpdateRatingReview_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id ),
                            new SqlParameter("itemId ", params_VM.ItemId),
                            new SqlParameter("rating",  params_VM.Rating),
                            new SqlParameter("reviewBody", params_VM.ReviewBody),
                            new SqlParameter("reviewerUserLoginId",params_VM.ReviewerUserLoginId),
                            new SqlParameter("mode",4),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateRatingReview @id,@itemId,@rating,@reviewBody,@reviewerUserLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Advertisement banner
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageAdvertisement_Get<T>(SP_ManageAdvertisement_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageAdvertisement @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Advertisement banner for slider
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageAdvertisement_GetAll<T>(SP_ManageAdvertisement_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageAdvertisement @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Get Business Timing Detail
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessTiming_Get<T>(SP_ManageBusinessTiming_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusiness @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessBannerDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Banner Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentBanner_Get<T>(SP_ManageBusinessContentBanner_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentBanner @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /////Banner Image For Dancing page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateOrder 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentBanner_Get<T>(SP_InsertUpdateBusinessContentBanner_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@subTitle", params_VM.SubTitle),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@buttonText", params_VM.ButtonText),
                            new SqlParameter("@buttonLink", params_VM.ButtonLink),
                            new SqlParameter("@bannerImage", params_VM.BannerImage),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentBanner @id,@userLoginId,@profilePageTypeId,@title,@subTitle, @description, @buttonText, @buttonLink, @bannerImage,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /////About Image For Dancing page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateOrder 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentAbout_Get<T>(SP_InsertUpdateBusinessContentAbout params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@subTitle", params_VM.SubTitle),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@aboutImage", params_VM.AboutImage),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessAboutDetail @id,@userLoginId,@profilePageTypeId,@title,@subTitle, @description, @aboutImage,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessAboutDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-About Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentAbout_Get<T>(SP_ManageBusinessContentAbout params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentAbout @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        ///-- TODO: 31-10-23 Check. Procedure not integrated because of BusinessContentService
        /////Business Content Service Image For Dancing page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateOrder 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentService_Get<T>(SP_InsertUpdateBusinessContentService params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@serviceTitle", params_VM.ServiceTitle),
                            new SqlParameter("@shortDescription", params_VM.ShortDescription),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentService @id,@userLoginId,@serviceTitle, @shortDescription,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }


        /////Business Content Video  For Yoga page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateOrder 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinesContentVideo<T>(SP_InsertUpdateBusinessContentVideoPPCMeta_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@videoDescription", params_VM.VideoDescription),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentVideoPPCMeta @id,@userLoginId,@title, @videoDescription,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessContentVideoPPCMeta
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Banner Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentVideoPPCMeta_GetSingle<T>(SP_ManageBusinessContentVideo_PPCMeta_Param_VM params_VM)
        {
            // Get Video-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentVideoPPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Get Business  content Video PPCMeta  Detail (List) 
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentVideoPPCMeta_GetList<T>(SP_ManageBusinessContentVideo_PPCMeta_Param_VM params_VM)
        {
            // Get BusinessContentVideo-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentVideoPPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }

        /////Business Content Plan  For Yoga page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessContentPlanPPCMeta
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinesContentPlanPPCMeta_Get<T>(SP_InsertUpdateBusinessContentPlanPPCMeta_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@businessPlanTitle", params_VM.BusinessPlanTitle),
                            new SqlParameter("@businessPlanDescription", params_VM.BusinessPlanDescription),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentPlanPPCMeta @id,@userLoginId,@businessPlanTitle, @businessPlanDescription,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  content Plan PPCMeta  Detail 
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessContentPlanPPCMeta_Get<T>(SP_ManageBusinessContentPlan_PPCMeta_Param_VM params_VM)
        {
            // Get BusinessPlan-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentPlan_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  content Plan PPCMeta  Detail (List) 
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentPlanPPCMeta_GetList<T>(SP_ManageBusinessContentPlan_PPCMeta_Param_VM params_VM)
        {
            // Get BusinessPlan-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentPlan_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }


        /////Banner Image For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBanner
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentSportsBanner_Get<T>(SP_InsertUpdateBusinessContentBanner_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@subTitle", params_VM.SubTitle),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@buttonText", params_VM.ButtonText),
                            new SqlParameter("@buttonLink", params_VM.ButtonLink),
                            new SqlParameter("@bannerImage", params_VM.BannerImage),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentSportsBanner @id,@userLoginId,@profilePageTypeId,@title,@subTitle, @description, @buttonText, @buttonLink, @bannerImage,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessBannerDetail(List)
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Banner Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessContentBannerList_Get<T>(SP_ManageBusinessContentBanner_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentBanner @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /////About  Service Image For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateAbout
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentAboutService_Get<T>(SP_InsertUpdateBusinessContentAboutService params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@aboutServicetitle", params_VM.AboutServiceTitle),
                            new SqlParameter("@aboutServiceDescription", params_VM.AboutServiceDescription),
                            new SqlParameter("@aboutServiceIcon", params_VM.AboutServiceIcon),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessAboutServiceDetail @id,@userLoginId,@profilePageTypeId,@aboutServicetitle, @aboutServiceDescription, @aboutServiceIcon,   @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessAboutServiceDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-About Service Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentAboutService_Get<T>(SP_ManageBusinessContentAboutService params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentAboutService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessAboutServiceDetailList
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-About Service Detail List Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessContentAboutService_GetList<T>(SP_ManageBusinessContentAboutService params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentAboutService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }


        /////Event For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateSports
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentEvent_Get<T>(SP_InsertUpdateBusinessContentEvent_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentEventPPCMeta @id,@userLoginId,@profilePageTypeId, @description,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessEventDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Event  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentEvent_Get<T>(SP_ManageBusinessContentEvent_Param_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentEvent_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessTennisDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Tennis Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentTennis_Get<T>(SP_ManageBusinessContentTennis_PPCMeta_Params_VM params_VM)
        {
            // Get Tennis-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentTennis_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /////Tennis Image For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateTennis
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentTennis_Get<T>(SP_InsertUpdateBusinessContentTennis_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("subTitle", params_VM.SubTitle),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("tennisImage", params_VM.TennisImage),
                            new SqlParameter("basicPrice" ,params_VM.BasicPrice),
                            new SqlParameter("commercialPrice" ,params_VM.CommercialPrice),
                            new SqlParameter("otherPrice" ,params_VM.OtherPrice),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("slotId", params_VM.SlotId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentTennis_PPCMeta @id,@userLoginId,@profilePageTypeId,@title,@subTitle, @description, @tennisImage,@basicPrice,@commercialPrice,@otherPrice,@submittedByLoginId,@slotId, @mode", queryParams).FirstOrDefault();
        }


        /////class For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateSports
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentClass_Get<T>(SP_InsertUpdateBusinessContentClass_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title",params_VM.Title),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentClassPPCMeta @id,@userLoginId,@profilePageTypeId,@title, @description,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }
        // <summary>
        /// Call stored-procedure sp_ManageBusinessClassDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Class  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentClass_Get<T>(SP_ManageBusinessContentClass_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentClass_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// Call stored-procedure sp_ManageBusinessTennisDetailList
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Tennis Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessContentTennisDetail_Get<T>(SP_ManageBusinessContentTennis_PPCMeta_Params_VM params_VM)
        {
            // Get Tennis-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentTennis_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        ////
        /////To Add Review Detail  For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateReview
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentReview_Get<T>(SP_InsertUpdateBusinessContentReview_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@reviewImage", params_VM.ReviewImage),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentReviewDetail @id,@userLoginId,@profilePageTypeId, @description, @reviewImage,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessReviewDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Review  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentReview_Get<T>(SP_ManageBusinessContentReview_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentReview_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /////To Add Professonial Detail  For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateProfessonial
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentProfessional_Get<T>(SP_InsertUpdateBusinessContentProfessional params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@professionalTitle", params_VM.ProfessionalTitle),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentProfessionalDetail @id,@userLoginId,@profilePageTypeId,@professionalTitle,@description,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessReviewDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Review  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentProfessional_Get<T>(SP_ManageBusinessContentProfessional_PPCMeta_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentProfessional_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add World Class Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentWorldClass_Get<T>(SP_InsertUpdateBusinessContentWorldClass_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentWorldClassDetail @id,@userLoginId,@profilePageTypeId,@title,@description,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }


        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentWorldClassDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content World Class  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentWorldClass_Get<T>(SP_ManageBusinessContentWorldClass_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentWorldClassDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        //////////////////////////////////////////To Add World Class Program /////////////////////////////////////////////////////
        /// <summary>
        /// To Add Business Content World Class  Program Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentWorldClassProgram_Get<T>(SP_InsertUpdateBusinessContentWorldClassProgram_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("options", params_VM.Options),
                            new SqlParameter("image", params_VM.Image),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessWorldClassProgramDetail @id,@userLoginId,@profilePageTypeId,@title,@options,@image,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentWorldClassProgramDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content World Class Program  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentWorldClassProgram_Get<T>(SP_ManageBusinessContentWorldClass_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentWorldClassProgramDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentWorldClassProgramDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content World Class Program  Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessContentWorldClassProgram_GetList<T>(SP_ManageBusinessContentWorldClass_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentWorldClassProgramDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Add Fitness Detail (Image)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentFitness_Get<T>(SP_InsertUpdateBusinessContentFitness_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("fitnessImage", params_VM.FitnessImage),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentFitness_PPCMeta @id,@userLoginId,@profilePageTypeId,@title,@description, @fitnessImage, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentFitnessPPC_Meta
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content  Fitness   Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentFitness_Get<T>(SP_ManageBusinessContentFitness_PPCMeta_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentFitness_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add Fitness  Movement Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentFitnessMovement_Get<T>(SP_InsertUpdateBusinessContentFitnessMovement_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("requirements", params_VM.Requirements),
                            new SqlParameter("investment", params_VM.Investment),
                            new SqlParameter("inclusions",params_VM.Inclusions),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentFitnessMovement @id,@userLoginId,@profilePageTypeId,@title,@requirements, @investment,@inclusions,@description, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentFitnessMovementDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content  Fitness  Movement Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentFitnessMovement_Get<T>(SP_ManageBusinessContentFitness_PPCMeta_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentFitnessMovementDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /////////////////////////////////////////////////////////////////////Much More Service///////////////////////////////////////////////
        /// <summary>
        /// To Add Much More Service Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentMuchMoreService_Get<T>(SP_InsertUpdateBusinessContentMuchMoreService_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("content", params_VM.Content),
                            new SqlParameter("serviceIcon", params_VM.ServiceIcon),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentMuchMoreService @id,@userLoginId,@profilePageTypeId,@content, @serviceIcon, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentMuchMoreServiceDetail
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content Much More Service Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentMuchMoreService_Get<T>(SP_ManageBusinessContentMuchMoreService_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentMuchMoreServiceDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add Much More Service Detail (Title)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentMuchMoreServiceDetail_Get<T>(SP_InsertUpdateBusinessContentMuchMoreServiceDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec Sp_InsertUpdateBusinessMuchMoreServiceDetail @id,@userLoginId,@profilePageTypeId,@title, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentMuchMoreServiceDetail (List)
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content Much More Service Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessContentMuchMoreServicelstDetails_Get<T>(SP_ManageBusinessContentMuchMoreService_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentMuchMoreServiceDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentFitnessMovementDetail (List)
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content Much More Service Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessContentFitnessMovementList_Get<T>(SP_ManageBusinessContentFitness_PPCMeta_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentFitnessMovementDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentStudioEquipment_PPCMetaDetail 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content Much More Service Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentStudioEquipment_PPCMeta_Get<T>(SP_ManageBusinessContentStudioEquipment_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentStudioEquipment_PPCMetaDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add StudioEquipment_PPCMetaDetail  Detail (Title)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentStudioEquipment_PPCMeta_Detail_Get<T>(SP_InsertUpdateBusinessContentStudioEquipmentPPCMeta_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("subTitle", params_VM.SubTitle),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentStudioEquipment_PPCMetaDetail @id,@userLoginId,@profilePageTypeId,@title, @subTitle,@submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        // <summary>
        /// Call stored-procedure sp_ManageBusinessContentStudioEquipmentDetail 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Content Much More Service Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessContentStudioEquipment_Get<T>(SP_ManageBusinessContentStudioEquipment_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentStudioEquipmentDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add StudioEquipment_PPCMetaDetail  Detail (Title)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentStudioEquipmentDetail_Get<T>(SP_InsertUpdateBusinessContentStudioEquipment_Param_VM params_VM)
        {


            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("equipmentType", params_VM.EquipmentType),
                            new SqlParameter("equipmentValue",params_VM.EquipmentValue),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentStudioEquipmentDetail @id,@userLoginId,@profilePageTypeId,@equipmentType,@equipmentValue,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        /// Get Business  content Studio Equipment PPCMeta  Detail (List) 
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentStudioEquipmentList_Get<T>(SP_ManageBusinessContentStudioEquipment_Param_VM params_VM)
        {
            // Get BusinessContentStudio Equipment-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentStudioEquipmentDetail @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Add AudioDetail_PPCMetaDetail  Detail (Title)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentAudio_PPCMeta_Detail_Get<T>(SP_InsertUpdateBusinessContentPortfolio_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("portfolioImage", params_VM.PortfolioImage),
                            new SqlParameter("audioImage", params_VM.AudioImage),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentAudio_PPCMeta @id,@userLoginId,@profilePageTypeId,@title, @description,@portfolioImage,@audioImage,@submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  content Audio  PPCMeta  Detail 
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessContentAudioDetail_PPCMetaGet<T>(SP_ManageBusinessContentPortfolio_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentAudio_PPCMetaDetail @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add Audio Detail   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentAudioDetail_Get<T>(SP_InsertUpdateBusinessContentAudio_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("artistName", params_VM.ArtistName),
                            new SqlParameter("audioFile", params_VM.AudioFile),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentAudioDetail @id,@userLoginId,@profilePageTypeId,@title, @artistName,@audioFile,@submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  content Audio    Detail  (List)
        /// </summary>
        /// <param name="">BusinessOwnerLogin Id</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentAudioList_Get<T>(SP_ManageBusinessContentPortfolio_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentAudio_PPCMetaDetail @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }


        /// <summary>
        /// To Add  Business Sponsor Detail   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessSponsor_Get<T>(SP_InsertUpdateBusinessSponsor_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("sponsorTitle", params_VM.SponsorTitle),
                            new SqlParameter("sponsorLink",params_VM.SponsorLink),
                            new SqlParameter("sponsorIcon",params_VM.SponsorIcon),
                            new SqlParameter("submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessSponsor @id,@businessOwnerLoginId,@sponsorTitle,@sponsorLink,@sponsorIcon,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Get Business  content Sponsor  - Get Single Record
        /// </summary>
        /// <param name="">SponsorId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessContentSponsor_GetSingle<T>(SP_ManageBusinessContentSponsor_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentSponsors @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  content Sponsor  - Get Single Record
        /// </summary>
        /// <param name="">SponsorId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentSponsor_GetList<T>(SP_ManageBusinessContentSponsor_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentSponsors @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Add  Business Client Detail   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessClient_Get<T>(SP_InsertUpdateBusinessContentClient_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId",params_VM.ProfilePageTypeId),
                            new SqlParameter("name", params_VM.Name),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("clientImage",params_VM.ClientImage),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentClientDetail @id,@userLoginId,@profilePageTypeId,@name,@description,@clientImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  content client    Detail By Id
        /// </summary>
        /// <param name="">ClientId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessContentClientDetail_Get<T>(SP_ManageBusinessContentClientDetail_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentClient_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Get Business  content client    Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">ClientId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentClientDetail_lst_Get<T>(SP_ManageBusinessContentClientDetail_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentClient_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Add  Business Event Company Detail   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentEventCompany_Get<T>(SP_BusinessContentEventCompany_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("profilePageTypeId",params_VM.ProfilePageTypeId),
                            new SqlParameter("title ", params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("image",params_VM.Image),
                            new SqlParameter("eventOptions", params_VM.EventOptions),
                            new SqlParameter("submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentEventCompanyDetail @id,@businessOwnerLoginId,@profilePageTypeId,@title,@description,@image,@eventOptions,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  content Event Company    Detail By BusinessOwnerLoginId 
        /// </summary>
        /// <param name="">EeventId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessContentEventCompanyDetail_Get<T>(SP_ManageBusinessContentEventCompany_Parasm_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentEventCompanyDetail @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// Insert Business-Content-Event images     /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentEventImage_Get<T>(SP_BusinessContentEventImages_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@eventId", params_VM.EventId),
                            new SqlParameter("@eventImage", params_VM.Image),
                            new SqlParameter("@submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec SP_InsertUpdateBusinessContentEventImage @id,@businessOwnerLoginId,@profilePageTypeId,@eventId,@eventImage,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }
        /// <summary>
        /// Get Business  content Event Image    Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">EeventId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessContentEventImageDetail_Get<T>(SP_ManageBusinessContentEventImage_Param_VM params_VM)
        {
            // Get BusinessContent Audio-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("eventId", params_VM.EventId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentEventImageDetail_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@eventId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To AddUpdate Business Course Image Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentCourseImages_Get<T>(SP_InsertUpdateBusinessContentCourseImages_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("courseSignIcon", params_VM.CourseSignIcon),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentCourseDetail @id,@userLoginId,@profilePageTypeId,@title,@courseSignIcon, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  course image    Detail By Id
        /// </summary>
        /// <param name="">ClientId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessCourseImageDetail_Get<T>(SP_ManageBusinessContentCourseImage_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCourseImage_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  course image    Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">CourseId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessCourseImageDetail_GetLst<T>(SP_ManageBusinessContentCourseImage_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCourseImage_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }



        /////////////////////////////////////////////////Curriculum //////////////////////////////////////////
        ///
        /// <summary>
        /// To AddUpdate Business Curriculum Image Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentCurriculum_Get<T>(SP_InsertUpdateBusinessContentCurriculum_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("curriculumOptions", params_VM.CurriculumOptions),
                            new SqlParameter("curriculumImage",params_VM.CurriculumImage),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentCurriculumDetail @id,@userLoginId,@profilePageTypeId,@title,@curriculumOptions,@curriculumImage, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  Curriculum image    Detail By Id
        /// </summary>
        /// <param name="">CurriculumId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessCurriculumDetail_Get<T>(SP_ManageBusinessContentCurriculum_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCurriculum_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  Curriculum image    Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">CurriculumUserLoginId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessCurriculumDetail_GetLst<T>(SP_ManageBusinessContentCurriculum_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCurriculum_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }




        /////////////////////////////////////////////////Education //////////////////////////////////////////
        ///
        /// <summary>
        /// To AddUpdate Business Education Image Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentEducation_Get<T>(SP_InsertUpdateBusinessContentEducation_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("university", params_VM.University),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("universityLogo", params_VM.UniversityLogo),
                            new SqlParameter("universityImage",params_VM.UniversityImage),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentEducationDetail @id,@userLoginId,@university, @submittedByLoginId,@universityLogo, @universityImage,@description,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  Education     Detail By Id
        /// </summary>
        /// <param name="">CurriculumId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessEducationDetail_Get<T>(SP_ManageBusinessContentEducationDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentEducation_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  Education     Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">EducationId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessEducationDetail_GetLst<T>(SP_ManageBusinessContentEducationDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentEducation_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }
        /// <summary>
        /// Get Business  Class     Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">ClassId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessClassDetail_GetLst<T>(SP_ManageClass_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("dayName","0"),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClass @id,@userLoginId,@dayName,@mode", queryParams).ToList();

        }


        /////////////////////////////////////////////////////// Language Detail/////////////////////////
        ////// <summary>
        /// To AddUpdate Business Language Icon Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentLanguage_Get<T>(SP_InsertUpdateBusinessContentLanguage_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("language", params_VM.Language),
                            new SqlParameter("languageIcon", params_VM.LanguageIcon),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentLanguageDetail @id,@userLoginId,@language,@languageIcon, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Get Business  Language Icon    Detail By Id
        /// </summary>
        /// <param name="">LanguageId</param>
        /// <returns>Returns the Table-Row data</returns>
        public T SP_ManageBusinessLanguageIconDetail_Get<T>(SP_ManageBussinessContentLanguage_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentLanguage_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Get Business  Language Icon  Detail By BusinessOwnerLoginId (List)
        /// </summary>
        /// <param name="">BusinessOwnerLoginId</param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageBusinessLanguageIconDetail_GetLst<T>(SP_ManageBussinessContentLanguage_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentLanguage_PPCMeta @id,@businessOwnerLoginId,@UserLoginId,@mode", queryParams).ToList();

        }


        /////////////////////////////////////////////////////// Language Detail/////////////////////////
        ////// <summary>
        /// To AddUpdate Business Language Icon Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentLanguages_Get<T>(SP_InsertUpdateLanguage_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("profilePageTypeId",params_VM.ProfilePageTypeId),
                            new SqlParameter("languageId", params_VM.LanguageId),
                            new SqlParameter("submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentLanguage @id,@businessOwnerLoginId,@profilePageTypeId,@languageId, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }



        /// <summary>
        ///  Language Detail By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> ManageBusinessContentLanguageDetail_GetList<T>(SP_ManageBusinessLanguage_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentLanguageDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }



        //////////////////////////////////////////////////// University Id ////////////////////////////////
        ///
         ////// <summary>
        /// To AddUpdate Business University  Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessUniversity_Get<T>(SP_InsertUpdateUniversityDetail params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("profilePageTypeId",params_VM.ProfilePageTypeId),
                            new SqlParameter("qualification",params_VM.Qualification),
                            new SqlParameter("startDate", params_VM.StartDate),
                            new SqlParameter("endDate", params_VM.EndDate),
                            new SqlParameter("universityName", params_VM.UniversityName),
                            new SqlParameter("universityLogo",params_VM.UniversityLogo),
                            new SqlParameter("submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinesUniversityDetail @id,@businessOwnerLoginId,@profilePageTypeId,@qualification,@startDate,@endDate,@universityName,@universityLogo, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }



        /// <summary>
        ///  University Detail By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> ManageUniversityDetail_GetList<T>(SP_ManageBusinessContentEducationDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUniversityDetail_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        ///  University Detail By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T ManageUniversityDetail_Get<T>(SP_ManageBusinessContentEducationDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUniversityDetail_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }



        //////////////////////////////////////////////////////////////////////// Business University Id saved /////////////////////////////////


        ////// <summary>
        /// To AddUpdate Business UniversityId  Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessUniversityId_Get<T>(SP_InsertUpdateBusinessContentUniversityDetail_PPCMeta params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("profilePageTypeId",params_VM.ProfilePageTypeId),
                            new SqlParameter("universityId",params_VM.UniversityId),
                            new SqlParameter("submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinesUniversityDetail_PPCMeta @id,@profilePageTypeId,@businessOwnerLoginId,@universityId, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }




        /////////////////////////////////////////////////////////////////////// Business Course Category ///////////////////
        ///
          ////// <summary>
        /// To AddUpdate Business Course Category Detail (Crud)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessCourseCategory_Get<T>(SP_InsertUpdateBusinessCourseCategory_Param_Vm params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("courseCategoryImage", params_VM.CourseCategoryImage),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessCourseCategoryDetail @id,@userLoginId,@title,@description,@courseCategoryImage,@submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        ///  Course Category Detail By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessCourseCategory_Get<T>(SP_ManageBusinessCourseCategory_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessCourseCategoryDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }



        /// <summary>
        ///  Course Category Detail By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessCourseCategory_Getlst<T>(SP_ManageBusinessCourseCategory_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessCourseCategoryDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Insert Notice Board 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentNoticeBoard<T>(SP_InsertUpdateBusinessNoticeBoard_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId",params_VM.UserLoginId),
                            new SqlParameter("startDate", params_VM.StartDate),
                             new SqlParameter("description", params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessNoticeBoardDetail @id,@userLoginId,@startDate,@description,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        ///   Notice Board Detail By UserLoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessNoticeBoard_Get<T>(SP_ManageBusinessNoticeBoard_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessNoticeBoardDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        ///   Notice Board Detail By UserLoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessNoticeBoard_GetList<T>(SP_ManageBusinessNoticeBoard_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessNoticeBoardDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call stored-procedure for InsertUpdate InstructorOtherInformation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateInstructorOtherInformation_Get<T>(SP_InsertUpdateInstructorOtherInformation params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec SP_InsertUpdateInstructorOtherInformation @id,@userLoginId,@profilePageTypeId,@title, @description, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure SP_ManageInstructorOtherInforamtion_Get
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">InstructorOtherInforamtion_Get Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageInstructorOtherInforamtion_Get<T>(SP_ManageInstructorOtherInforamtion params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessInstructorOtherInformation @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Call stored-procedure for InsertUpdate SP_InsertUpdateBusinessCourseCategory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentCourseCategory<T>(SP_InsertUpdateBusinessCourseCategory_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentCategoriesDetail @id,@userLoginId,@profilePageTypeId,@title, @description, @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure SP_ManageBusinessContentCourseCategory_Param_VM
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">SP_ManageBusinessContentCourseCategory_Param_VM Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessCourseCategoryDetail_Get<T>(SP_ManageBusinessContentCourseCategory_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCategories_Detail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        ////////////////////////////////////////   To Get Business category List in Visitor-Panel///////////////////////////////////////////////////////

        /// <summary>
        /// Call stored-procedure SP_ManageBusinessContentCourseCategory_Param_VM
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">SP_ManageBusinessContentCourseCategory_Param_VM Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageBusinessCourseCategoryDetail<T>(SP_ManageBusinessContentCourseCategory_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCategories_Detail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }
        ///////////////////////////////////////////////////////////   Acccess Course Detail by businessOwnerLoginId/////////////////////////////////////////



        /// <summary>
        /// Call stored-procedure sp_InsertUpdateAccessCourse 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentAccessCourse_Get<T>(SP_InsertUpdateBusinessContentAccessCourse_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@userLoginId", params_VM.UserLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@title", params_VM.Title),
                            new SqlParameter("@subTitle", params_VM.SubTitle),
                            new SqlParameter("@description", params_VM.Description),
                            new SqlParameter("@courseImage", params_VM.CourseImage),
                            new SqlParameter("@accessCourse", params_VM.AccessCourse),
                            new SqlParameter("@submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentAccessCourseDetail @id,@userLoginId,@profilePageTypeId,@title,@subTitle, @description, @courseImage,@accessCourse,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure SP_ManageBusinessContentAccessCours_Param_VM
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">SP_ManageBusinessContentAccessCours_Param_VM Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageBusinessAccessCourseDetail_Get<T>(SP_ManageBusinessContentAccessCours_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentAccessCourseDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// Call stored-procedure sp_InsertUpdateBusinessContentCategories_PPCMeta 
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateBusinessContentCourceCategory_Get<T>(SP_InsertUpdateBusinessContentCourceCategory_PPCMeta_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("@profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("@courseCategoryId", params_VM.CourseCategoryId),
                            new SqlParameter("@submittedByLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("@mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentCategories_PPCMeta @id,@businessOwnerLoginId,@profilePageTypeId,@courseCategoryId,  @submittedByLoginId, @mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        ///  Course Category Detail List For Visitor or business 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessCourseCategoryDetail_Getlst<T>(SP_ManageBusinessCourseCategory_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentCourseCategoryDetail_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }


        /// <summary>
        /// Call stored-procedure sp_InsertUpdateSuperAdminProfile_Get 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Profile Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_InsertUpdateSuperAdminProfile_Get<T>(SP_InsertUpdateSuperAdminProfile_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("firstName", _Params_VM.FirstName),
                            new SqlParameter("lastName", _Params_VM.LastName),
                            new SqlParameter("email", _Params_VM.Email),
                            new SqlParameter("phoneNumber",_Params_VM.PhoneNumber),
                            new SqlParameter("profileImage", _Params_VM.ProfileImage),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode",_Params_VM.Mode),
                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateSuperAdminProfile @id,@firstName,@lastName,@email,@phoneNumber,@profileImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }


        /// <summary>
        /// Call stored-procedure sp_InsertUpdateSubAdminProfile_Get 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Profile Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_InsertUpdateSubAdminProfile_Get<T>(SP_InsertUpdateSubAdminProfile_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("firstName", _Params_VM.FirstName),
                            new SqlParameter("lastName", _Params_VM.LastName),
                            new SqlParameter("email", _Params_VM.Email),
                            new SqlParameter("phoneNumber",_Params_VM.PhoneNumber),
                            new SqlParameter("profileImage", _Params_VM.ProfileImage),
                            new SqlParameter("submittedByLoginId", "1"),
                            new SqlParameter("mode",_Params_VM.Mode),

                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateSubAdminProfile @id,@firstName,@lastName,@email,@phoneNumber,@profileImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateUserFamilyRelation_Get 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Profile Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_InsertUpdateUserFamilyRelation_Get<T>(SP_InsertUpdateUserFamilyRelation_Params_VM _Params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("user1LoginId", _Params_VM.User1LoginId),
                            new SqlParameter("user2LoginId", _Params_VM.User2LoginId),
                            new SqlParameter("user1Relation_FieldTypeCatalogKey", _Params_VM.User1Relation_FieldTypeCatalogKey),
                            new SqlParameter("user2Relation_FieldTypeCatalogKey", _Params_VM.User2Relation_FieldTypeCatalogKey),
                            new SqlParameter("submittedByLoginId", _Params_VM.SubmittedByLoginId),
                            new SqlParameter("mode",_Params_VM.Mode),
                        };

            var resp = _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateUserFamilyRelation @id,@user1LoginId,@user2LoginId,@user1Relation_FieldTypeCatalogKey,@user2Relation_FieldTypeCatalogKey,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
            return resp;
        }

        /// <summary>
        /// Call stored-procedure sp_ManageUserFamilyRelations -Get Single
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">sp_ManageUserFamilyRelations Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public T SP_ManageUserFamilyRelations_Get<T>(SP_ManageUserFamilyRelations_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("masterId", params_VM.MasterId),
                            new SqlParameter("user1LoginId", params_VM.User1LoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserFamilyRelations @id,@masterId,@user1LoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageUserFamilyRelations -Get List
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">sp_ManageUserFamilyRelations Stored-Procedure Params</param>
        /// <returns>Return Object List</returns>
        public List<T> SP_ManageUserFamilyRelations_GetList<T>(SP_ManageUserFamilyRelations_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("masterId", params_VM.MasterId),
                            new SqlParameter("user1LoginId", params_VM.User1LoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageUserFamilyRelations @id,@masterId,@user1LoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Add The Exam Submission 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>s
        public T SP_InsertUpdateExamFormSubmission<T>(SP_InsertUpdateExamFormSubmission_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("Id", params_VM.Id),
                            new SqlParameter("ExamFormId",  params_VM.ExamFormId),
                            new SqlParameter("SessionYear",  params_VM.SessionYear),
                            new SqlParameter("CandidateProfileImage",  params_VM.CandidateProfileImage),
                            new SqlParameter("Category",  params_VM.Category),
                            new SqlParameter("UserMasterId",  params_VM.UserMasterId),
                            new SqlParameter("CurrentRollNo",  params_VM.CurrentRollNo),
                            new SqlParameter("CandidateName",  params_VM.CandidateName),
                            new SqlParameter("CandidateFather",  params_VM.CandidateFather),
                            new SqlParameter("CandidateMother",  params_VM.CandidateMother),
                            new SqlParameter("PermanentAddress",  params_VM.PermanentAddress),
                            new SqlParameter("PermanentPin",  params_VM.PermanentPin),
                            new SqlParameter("PermanentMobNo",  params_VM.PermanentMobNo),
                            new SqlParameter("PresentAddress",  params_VM.PresentAddress),
                            new SqlParameter("PresentPin",  params_VM.PresentPin),
                            new SqlParameter("PresentMobNo",  params_VM.PresentMobNo),
                            new SqlParameter("Nationality",  params_VM.Nationality),
                            new SqlParameter("AadharCardNo",  params_VM.AadharCardNo),
                            new SqlParameter("DOB",  params_VM.DOB),
                            new SqlParameter("Email",  params_VM.Email),
                            new SqlParameter("EduQualification",  params_VM.EduQualification),
                            new SqlParameter("CurrentClass",  params_VM.CurrentClass),
                            new SqlParameter("CurrentSubject",  params_VM.CurrentSubject),
                            new SqlParameter("CurrentCenterName",  params_VM.CurrentCenterName),
                            new SqlParameter("CurrentCenterCity",  params_VM.CurrentCenterCity),
                            new SqlParameter("PreviousClass",  params_VM.PreviousClass),
                            new SqlParameter("PreviousSubject",  params_VM.PreviousSubject),
                            new SqlParameter("PreviousYear",  params_VM.PreviousYear),
                            new SqlParameter("PreviousRollNo",  params_VM.PreviousRollNo),
                            new SqlParameter("PreviousResult",  params_VM.PreviousResult),
                            new SqlParameter("PreviousCenterName",  params_VM.PreviousCenterName),
                            new SqlParameter("Amount",  params_VM.Amount),
                            new SqlParameter("AmountInWord",  params_VM.AmountInWord),
                            new SqlParameter("NoOfAttached",  params_VM.NoOfAttached),
                            new SqlParameter("CertificateCollectFrom",  params_VM.CertificateCollectFrom),
                            new SqlParameter("CandidateSignature",  params_VM.CandidateSignature),
                            new SqlParameter("CandidateGuradianSignature",  params_VM.CandidateGuradianSignature),
                            new SqlParameter("CandidateGuradianName",  params_VM.CandidateGuradianName),
                            new SqlParameter("BankDraftNo",  params_VM.BankDraftNo),
                            new SqlParameter("BankDraftDate",  params_VM.BankDraftDate),
                            new SqlParameter("PostalOrderNo",  params_VM.PostalOrderNo),
                            new SqlParameter("SuperintendentSignature",  params_VM.SuperintendentSignature),
                            new SqlParameter("SuperintendentName", params_VM.SuperintendentName ),
                            new SqlParameter("SuperintendentPinNo",  params_VM.SuperintendentPinNo),
                            new SqlParameter("SuperintendentPhoneNo",  params_VM.SuperintendentPhoneNo),
                            new SqlParameter("SuperintendentEmail",  params_VM.SuperintendentEmail),
                            new SqlParameter("mode",  params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateExamFormSubmission @Id, @ExamFormId, @SessionYear, @CandidateProfileImage, @Category, @UserMasterId, @CurrentRollNo, @CandidateName, @CandidateFather, @CandidateMother, @PermanentAddress, @PermanentPin, @PermanentMobNo, @PresentAddress, @PresentPin, @PresentMobNo, @Nationality, @AadharCardNo, @DOB, @Email, @EduQualification, @CurrentClass, @CurrentSubject, @CurrentCenterName, @CurrentCenterCity, @PreviousClass, @PreviousSubject, @PreviousYear, @PreviousRollNo, @PreviousResult, @PreviousCenterName, @Amount, @AmountInWord, @NoOfAttached, @CertificateCollectFrom, @CandidateSignature, @CandidateGuradianSignature, @CandidateGuradianName, @BankDraftNo, @BankDraftDate, @PostalOrderNo, @SuperintendentSignature, @SuperintendentName, @SuperintendentPinNo, @SuperintendentPhoneNo, @SuperintendentEmail, @mode ", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessLicenses - Get List
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageBusinessLicenses_GetAll<T>(SP_ManageBusinessLicenses_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("licenseId", params_VM.LicenseId),
                            new SqlParameter("certificateId", params_VM.CertificateId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessLicenses @id,@businessOwnerLoginId,@licenseId,@certificateId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageBusinessLicense - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public T SP_ManageBusinessLicense_Get<T>(SP_ManageBusinessLicenses_Params_VM params_VM)
        {
            return SP_ManageBusinessLicenses_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_GetDetailByMasterId - Get List
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_GetDetailByMasterId_GetAll<T>(SP_GetDetailByMasterId_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("masterId", params_VM.MasterId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetDetailByMasterId @masterId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_GetDetailByMasterId - Get Single
        /// </summary>
        /// <typeparam name="T">Response VM</typeparam>
        /// <param name="params_VM">Stored-Procedure Parameter VM</param>
        /// <returns>Returns Object</returns>
        public T SP_GetDetailByMasterId_Get<T>(SP_GetDetailByMasterId_Params_VM params_VM)
        {
            return SP_GetDetailByMasterId_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateTrainingBooking
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateTrainingBooking_Get<T>(SP_InsertUpdateTrainingBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@orderId", params_VM.OrderId),
                            new SqlParameter("@trainingId", params_VM.TrainingId),
                            new SqlParameter("@studentUserLoginId", params_VM.StudentUserLoginId),
                            new SqlParameter("@trainingStartDate", params_VM.TrainingStartDate),
                            new SqlParameter("@trainingEndDate", params_VM.TrainingEndDate),
                            new SqlParameter("@mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateTrainingBooking @id, @orderId, @trainingId, @studentUserLoginId, @trainingStartDate, @trainingEndDate,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageTrainingBooking --Get List
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public List<T> SP_ManageTrainingBooking_GetAll<T>(SP_ManageTrainingBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("trainingId", params_VM.TrainingId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageTrainingBooking @id,@userLoginId,@trainingId,@mode", queryParamsGetEvent).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageTrainingBooking - Get Single
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_ManageTrainingBooking_Get<T>(SP_ManageTrainingBooking_Params_VM params_VM)
        {
            return SP_ManageTrainingBooking_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// Get Training Booking Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageTrainingBooking_GetByID<T>(SP_ManageTrainingBooking_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("userLoginId", params_VM.UserLoginId),
                    new SqlParameter("trainingId", params_VM.TrainingId),
                    new SqlParameter("mode", params_VM.Mode)
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageTrainingBooking @id,@userLoginId,@trainingId,@mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        /// Call stored-procedure sp_ManageOrder --Get List
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return List Result</returns>
        public List<T> SP_ManageOrder_GetAll<T>(SP_ManageOrder_Params_VM params_VM)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageOrder @id,@userLoginId,@mode", queryParamsGetEvent).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageOrder --Get Single
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_ManageOrder_Get<T>(SP_ManageOrder_Params_VM params_VM)
        {
            return SP_ManageOrder_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_ManagePaymentResponse --Get List
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return List Result</returns>
        public List<T> SP_ManagePaymentResponse_GetAll<T>(SP_ManagePaymentResponse_Params_VM params_VM)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("orderId", params_VM.OrderId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManagePaymentResponse @id,@orderId,@mode", queryParamsGetEvent).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManagePaymentResponse --Get Single
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_ManagePaymentResponse_Get<T>(SP_ManagePaymentResponse_Params_VM params_VM)
        {
            return SP_ManagePaymentResponse_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// To Get Sponsor For SuperAdmin Panel By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageSuperAdminSponsors_Get<T>(SP_ManageSuperAdminSponsor_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageSuperAdminSponsors @id,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call Stored-Procedure sp_ManageSuperAdminSponsors - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Object List</returns>
        public List<T> SP_ManageSuperAdminSponsors_GetAll<T>(SP_ManageSuperAdminSponsor_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageSuperAdminSponsors @id,@mode", queryParams).ToList();

        }

        /// <summary>
        ///  To Add/Update SuperAdmin Sponsor Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateSuperAdminSponsor_Get<T>(SP_InsertUpdateSuperAdminSponsor_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("sponsorTitle", params_VM.SponsorTitle),
                            new SqlParameter("sponsorLink",params_VM.SponsorLink),
                            new SqlParameter("sponsorIcon",params_VM.SponsorIcon),
                            new SqlParameter("submittedByLoginId", params_VM.CreatedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateSuperAdminSponsor @id,@sponsorTitle,@sponsorLink,@sponsorIcon,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        public List<T> SP_ManageBusinessCategory_GetAll<T>(SP_ManageBusinessCategorty_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("lastRecordId", params_VM.LastRecordId),
                            new SqlParameter("recordLimit", params_VM.RecordLimit),
                            new SqlParameter("parentBusinessCategoryId", params_VM.ParentBusinessCategoryId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// business category for find business page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessList_Get<T>(SP_ManageBusinessCategorty_Param_VM params_VM)
        {
            // Get About-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("lastRecordId", params_VM.LastRecordId),
                            new SqlParameter("recordLimit", "12"),
                            new SqlParameter("parentBusinessCategoryId", params_VM.ParentBusinessCategoryId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).ToList();
        }
        /// <summary>
        /// 
        /// <summary>
        /// Call stored-procedure sp_ManageGroup - Get List
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Returns Object List</returns>
        public List<T> SP_ManageGroup_GetAll<T>(SP_ManageGroup_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("userLoginId",params_VM.UserLoginId),
                    new SqlParameter("mode", params_VM.Mode),
                    new SqlParameter("searchKeywords",params_VM.SearchKeywords),
                    new SqlParameter("groupType", params_VM.GroupType)
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageGroup @id,@userLoginId,@mode,@searchKeywords,@groupType", queryParams).ToList();
        }

        /// <summary>
        /// Call stored-procedure sp_ManageGroup - Get Single
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageGroup_Get<T>(SP_ManageGroup_Params_VM params_VM)
        {

            return SP_ManageGroup_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure sp_InsertUpdateUserCertificate
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateUserCertificate_Get<T>(SP_InsertUpdateUserCertificate_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                        new SqlParameter("@id", params_VM.Id),
                        new SqlParameter("@userLoginId", params_VM.UserLoginId),
                        new SqlParameter("@certificateId", params_VM.CertificateId),
                        new SqlParameter("@licenseId", params_VM.LicenseId),
                        new SqlParameter("@itemId", params_VM.ItemId),
                        new SqlParameter("@itemTable", params_VM.ItemTable),
                        new SqlParameter("@certificateNumber", params_VM.CertificateNumber),
                        new SqlParameter("@certificateFile", params_VM.CertificateFile),
                        new SqlParameter("@mode", params_VM.Mode)
                        };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateUserCertificate @id,@userLoginId,@certificateId,@licenseId,@itemId,@itemTable,@certificateNumber,@certificateFile,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// To Add Update the Branch Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T InsertUpdateBusinessBranches_Get<T>(SP_InsertUpdateBusinessBranch_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("branchbusinessLoginId",params_VM.BranchBusinessLoginId),
                            new SqlParameter("name",params_VM.Name),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessBranch @id,@businessOwnerLoginId,@branchbusinessLoginId,@name,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Branches Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessBranchDetail_Get<T>(SP_ManageBusinessBranchDetail_Param_VM params_VM)
        {
            // Get Branch-Record-Detail-By-BranchBusinessLoginId 
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessBranchDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Branches Detail By BusinessOwnerLoginId (visitotPanel)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessBranchesDetaillst_Get<T>(SP_ManageBusinessBranchDetail_Param_VM params_VM)
        {
            // Get Branch-Record-Detail-By-BranchBusinessLoginId 
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessBranchDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }



        /// <summary>
        /// To Add Pause Class Request 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdatePauseClassRequest_Get<T>(SP_InsertUpdatePauseClassRequest_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("classBookingId",params_VM.ClassBookingId),
                            new SqlParameter("pauseStartDate",params_VM.PauseStartDate),
                            new SqlParameter("pauseEndDate",params_VM.PauseEndDate),
                            new SqlParameter("pauseDays",params_VM.PauseDays),
                            new SqlParameter("reason",params_VM.Reason),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("businessReply",params_VM.BusinessReply)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateClassPause @id,@userLoginId,@businessOwnerLoginId,@classBookingId,@pauseStartDate,@pauseEndDate,@pauseDays,@reason,@status,@submittedByLoginId,@mode,@businessReply", queryParams).FirstOrDefault();

        }
        public T SP_ManagePauseClassRequest<T>(SP_ManageClassPauseRequestDetail_Param_VM params_VM)
        {
            // Get Branch-Record-Detail-By-BranchBusinessLoginId 
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassPauseDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        ///  To Add/Update UserContentRecume Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentUserResumeDetail_Get<T>(SP_InsertUpdateBusinessUserContentResume_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("summary",params_VM.Summary),
                            new SqlParameter("languages",params_VM.Languages),
                            new SqlParameter("skills", params_VM.Skills),
                            new SqlParameter("freelance",params_VM.Freelance),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateUserContentResumeDetail @id,@userLoginId,@summary,@languages,@skills,@freelance,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get the User Content Resume Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessContentUserResume<T>(SP_ManageBusinessContentUserResume_Param_VM params_VM)
        {
            // Get UserContentResume-Record-Detail-By-UserLoginId 
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentUserResume @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// To Add/Update Experience Module Detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessUerExperienceDetail_Get<T>(SP_InsertUpdateUserExperience_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("companyName", params_VM.CompanyName),
                            new SqlParameter("startMonth",params_VM.StartMonth),
                            new SqlParameter("startYear", params_VM.StartYear),
                            new SqlParameter("endMonth",params_VM.EndMonth),
                            new SqlParameter("endYear", params_VM.EndYear),
                            new SqlParameter("startDate", params_VM.StartDate),
                            new SqlParameter("endDate", params_VM.EndDate),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateExperienceDetail @id,@userLoginId,@title,@companyName,@startMonth,@startYear,@endMonth,@endYear,@startDate,@endDate,@description,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get the User Experience Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessUserExperienceDetail<T>(SP_ManageBusinessUserExperienceDetail_Param_VM params_VM)
        {
            // Get UserContentResume-Record-Detail-By-UserLoginId 
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessUserExperienceDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Add/Update Education Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public T SP_InsertUpdateBusinessUerEducationDetail_Get<T>(SP_InsertUpdateUserEducation_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("schoolName",params_VM.SchoolName),
                            new SqlParameter("designation", params_VM.Designation),
                            new SqlParameter("startMonth",params_VM.StartMonth),
                            new SqlParameter("startYear", params_VM.StartYear),
                            new SqlParameter("endMonth",params_VM.EndMonth),
                            new SqlParameter("endYear", params_VM.EndYear),
                            new SqlParameter("startDate", params_VM.StartDate),
                            new SqlParameter("endDate", params_VM.EndDate),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("grade", params_VM.Grade),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateEducationDetail @id,@userLoginId,@schoolName,@designation,@startMonth,@startYear,@endMonth,@endYear,@startDate,@endDate,@description,@grade,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To get User Education Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessUserEducationDetail<T>(SP_ManageBusinessUserEducationDetail_Param_VM params_VM)
        {
            // Get UserContentResume-Record-Detail-By-UserLoginId 
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessUserEducationDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get User Education Detail List By UserLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessUserEducationDetail_Getlst<T>(SP_ManageBusinessUserEducationDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessUserEducationDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }
        /// <summary>
        /// To Get User Experience Detail List By UserLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessUserExperienceDetail_Getlst<T>(SP_ManageBusinessUserExperienceDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessUserExperienceDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Get Business Detail (Video/Images ) using the Last Record Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessDetail_GetAll<T>(SP_ManageBusinessDetail_Param_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId", params_VM.LastRecordId),
                            new SqlParameter("recordLimit ", params_VM.RecordLimit),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllBusinessDetail @id,@userLoginId,@lastRecordId,@recordLimit ,@mode", queryParams).ToList();
        }

        /// <summary>
        /// To Get Image/Video Detail Through Pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessUserDetail_GetAll<T>(SP_ManageBusinessUserDetail_Param_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("pageSize", params_VM.PageSize),
                            new SqlParameter("pageNumber ", params_VM.PageNumber),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllBusinessUserDetail_ByPagination @id,@businessOwnerLoginId,@userLoginId,@pageSize,@pageNumber ,@mode", queryParams).ToList();
        }

        /// <summary>
        /// To Insert/Update Classic Dance Technique Detail By UserLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateClassicDanceTechniqueDetail_Get<T>(SP_InsertUpdateClassicDanceTechnique_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("subTitle", params_VM.SubTitle),
                            new SqlParameter("techniqueImage", params_VM.TechniqueImage),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessClassicDanceDetail @id,@userLoginId,@profilePageTypeId,@title,@subTitle,@techniqueImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Get Classic Dance Detail By UserLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public T SP_ManageBusinessClassicDanceTechniqueDetail_Get<T>(SP_ManageBusinessClassicDanceTechnique_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessClassicDanceTechniqueDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Get Business Content Classic Dance Detail By BusinessOwnerLoginId For Viewing Visitor-Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public List<T> SP_ManageBusinessClassicDanceTechniqueDetail_Getlst<T>(SP_ManageBusinessClassicDanceTechnique_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessClassicDanceTechniqueDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }


        /// <summary>
        /// To Insert/Update Classic Dance detail PPCMeta By UserLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinesClassicDanceTechniqueDetail_Get<T>(SP_InsertUpdateBusinessContentClassicDance_PPCMeta_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("techniqueItemList", params_VM.TechniqueItemList),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentClassicDance_PPCMeta @id,@userLoginId,@profilePageTypeId,@title,@description,@techniqueItemList,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add/Update Classic Dance Video Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateClassicDanceVideoDetail_Get<T>(SP_InsertUpdateClassicDanceVideo_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("subTitle",params_VM.SubTitle),
                            new SqlParameter("note", params_VM.Note),
                            new SqlParameter("videoLink", params_VM.VideoLink),
                            new SqlParameter("videoImage", params_VM.VideoImage),
                            new SqlParameter("buttonText", params_VM.ButtonText),
                            new SqlParameter("buttonLink", params_VM.ButtonLink),
                            new SqlParameter("buttonText1", params_VM.ButtonText1),
                            new SqlParameter("buttonLink1", params_VM.ButtonLink1),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentClassicVideoDetail @id,@userLoginId,@profilePageTypeId,@title,@subTitle,@note,@videoLink,@videoImage,@buttonText,buttonLink,@buttonText1,@buttonLink1,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Classic Dance Video Detail by businessOwnerLoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessClassicDanceVideoDetail_Get<T>(SP_ManageBusinessClassicDanceTechnique_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessClassicDanceVideoDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Get branch Detail By Location (city,state)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessClassicDanceBranchDetail_Get<T>(SP_ManageBusinessBranchLocationDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("state",params_VM.State),
                            new SqlParameter("city",params_VM.City),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessBranchLocationDetail @id,@businessOwnerLoginId,@userLoginId,@state,@city,@mode", queryParams).ToList();

        }


        /// <summary>
        /// To Get Classic Dance Profile Detail By LoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageClassicDanceProfile_PPCMeta_Get<T>(SP_ManageClassicDanceProfileDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassicDanceProfile_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// To Add/Update Classic Dance Profile Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateClassicDanceProfileDetail_Get<T>(SP_InsertUpdateClassicDanceProfileDetail_Paarm_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("subTitle",params_VM.SubTitle),
                            new SqlParameter("classicImage", params_VM.ClassicImage),
                            new SqlParameter("image", params_VM.Image),
                            new SqlParameter("scheduleImage", params_VM.ScheduleImage),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateClassicDanceProfileDetail @id,@userLoginId,@profilePageTypeId,@title,@subTitle,@classicImage,@image,@scheduleImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Class Timing Detail by Classdays 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessClassTimingDetail_GetLst<T>(SP_ManageBusinessTimingDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("classDays",params_VM.ClassDays),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessClassTimeDetail @id,@businessOwnerLoginId,@userLoginId,@classDays,@mode", queryParams).ToList();

        }


        /// <summary>
        /// Call Stored Procedure SP_InsertUpdateHomePageFeaturedCardSection
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateHomePageFeaturedCardSection_Get<T>(SP_InsertUpdateHomePageFeaturedCardSection params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("type", params_VM.Type),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("buttonText", params_VM.ButtonText),
                            new SqlParameter("buttonLink", params_VM.ButtonLink),
                            new SqlParameter("thumbnail", params_VM.Thumbnail),
                            new SqlParameter("video", params_VM.Video),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateHomePageFeaturedCardSection @id,@type,@title,@description,@buttonText,@buttonLink,@thumbnail,@video,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageFeaturedCardSection - Get List
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageHomePageFeaturedCardSection_GetAll<T>(SP_ManageHomePageFeaturedCardSection_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("type", params_VM.Type),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageHomePageFeaturedCardSection @id,@type,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageFeaturedCardSection - Get Single
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageHomePageFeaturedCardSection_Get<T>(SP_ManageHomePageFeaturedCardSection_Params_VM params_VM)
        {
            return SP_ManageHomePageFeaturedCardSection_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// Call Stored Procedure sp_InsertUpdateHomePageMultipleItem
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateHomePageMultipleItem_Get<T>(SP_InsertUpdateHomePageMultipleItem_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("type", params_VM.Type),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("link", params_VM.Link),
                            new SqlParameter("thumbnail", params_VM.Thumbnail),
                            new SqlParameter("video", params_VM.Video),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateHomePageMultipleItem @id,@type,@title,@description,@link,@thumbnail,@video,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageMultipleItem - Get List
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageHomePageMultipleItem_GetAll<T>(SP_ManageHomePageMultipleItem_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("type", params_VM.Type),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageHomePageMultipleItem @id,@type,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageMultipleItem - Get Single
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageHomePageMultipleItem_Get<T>(SP_ManageHomePageMultipleItem_Params_VM params_VM)
        {
            return SP_ManageHomePageMultipleItem_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// Call Stored Procedure SP_InsertUpdateHomePageClassCategorySection
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateHomePageClassCategorySection_Get<T>(SP_InsertUpdateHomePageClassCategorySection params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("classCategoryTypeId",  params_VM.ClassCategoryTypeId),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateHomePageClassCategorySection @id,@classCategoryTypeId,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageClassCategorySection - Get List
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageHomePageClassCategorySection_GetAll<T>(SP_ManageHomePageClassCategorySection_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageHomePageClassCategorySection @id,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageClassCategorySection - Get Single
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageHomePageClassCategorySection_Get<T>(SP_ManageHomePageClassCategorySection_Params_VM params_VM)
        {
            return SP_ManageHomePageClassCategorySection_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// Call Stored Procedure sp_InsertUpdateHomePageFeaturedVideo
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateHomePageFeaturedVideo_Get<T>(SP_InsertUpdateHomePageFeaturedVideo_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("thumbnail", params_VM.Thumbnail),
                            new SqlParameter("video", params_VM.Video),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateHomePageFeaturedVideo @id,@title,@description,@thumbnail,@video,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageFeaturedVideo - Get List
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageHomePageFeaturedVideo_GetAll<T>(SP_ManageHomePageFeaturedVideo_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageHomePageFeaturedVideos @id,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageFeaturedVideo - Get Single
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageHomePageFeaturedVideo_Get<T>(SP_ManageHomePageFeaturedVideo_Params_VM params_VM)
        {
            return SP_ManageHomePageFeaturedVideo_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// Call Stored Procedure sp_InsertUpdateHomePageBannerItem
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateHomePageBannerItem_Get<T>(SP_InsertUpdateHomePageBannerItem_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("type",params_VM.Type),
                            new SqlParameter("image", params_VM.Image),
                            new SqlParameter("video", params_VM.Video),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("text", params_VM.Text),
                            new SqlParameter("link", params_VM.Link)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateHomePageBannerItem @id,@type,@image,@video,@status,@submittedByLoginId,@mode,@text,@link", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageBannerItem - Get List
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageHomePageBannerItem_GetAll<T>(SP_ManageHomePageBannerItem_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageHomePageBannerItem @id,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call Stored Procedure sp_ManageHomePageBannerItem - Get Single
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_ManageHomePageBannerItem_Get<T>(SP_ManageHomePageBannerItem_Params_VM params_VM)
        {
            return SP_ManageHomePageBannerItem_GetAll<T>(params_VM).FirstOrDefault();
        }


        public T SP_InsertUpdateMasterProExtraInformation_Get<T>(SP_InsertUpdateMasterProExtraInformation_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",params_VM.Id ),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("pdf", params_VM.Pdf),
                            new SqlParameter("description", params_VM.ShortDescription),
                            new SqlParameter("mode", params_VM.Mode),

            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateMasterProExtraInformationService @id,@userLoginId,@title,@pdf,@description,@mode", queryParams).FirstOrDefault();
        }
        public T SP_ManageMasterProExtraInformation_Get<T>(SP_ManageMasterProExtraInformation_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMasterProExtraInformationService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        public List<T> SP_ManageMasterProExtraInformation_GetAll<T>(SP_ManageMasterProExtraInformation_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMasterProExtraInformationService @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }


        public T SP_InsertUpdateMasterResumeDetails<T>(SP_InsertUpdateMasterpro_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("age", params_VM.Age),
                            new SqlParameter("nationality", params_VM.Nationality),
                             new SqlParameter("uploadCV",params_VM.UploadCV),
                            new SqlParameter("freelance", params_VM.Freelance),
                            new SqlParameter("skype", params_VM.Skype),
                            new SqlParameter("languages",params_VM.Languages),
                            new SqlParameter("mode", params_VM.Mode)

            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateMasterProResume_PPCMeta @id,@userLoginId,@age,@nationality,@uploadCV,@freelance,@skype,@languages,@mode", queryParams).FirstOrDefault();
        }
        public T SP_ManageMasterResumeDetails<T>(SP_ManageMasterProResume_PPCMeta_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                                         new SqlParameter("id", params_VM.Id),
                                         new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                                         new SqlParameter("userLoginId", params_VM.UserLoginId),
                                         new SqlParameter("mode", params_VM.Mode)
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMasterProResume_PPCMeta @id,@businessOwnerLoginId, @userLoginId, @mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// get instructor name for dropdown
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessInstructorDetailsCategory_Get<T>(SP_ManageBusinessCourseCategory_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                              new SqlParameter("lastRecordId", "0"),
                            new SqlParameter("recordLimit", "0"),
                              new SqlParameter("parentBusinessCategoryId", "0"),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessCategory @id,@lastRecordId,@recordLimit,@parentBusinessCategoryId,@mode", queryParams).ToList();

        }


        /// <summary>
        /// Call Stored Procedure sp_GetRecordsBySearch - Get List
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public List<T> SP_GetRecordsBySearch_GetAll<T>(SP_GetRecordsBySearch_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId",  params_VM.UserLoginId),
                            new SqlParameter("menuTag",  params_VM.MenuTag),
                            new SqlParameter("location",  params_VM.Location),
                            new SqlParameter("latitude",  params_VM.Latitude),
                            new SqlParameter("longitude",  params_VM.Longitude),
                            new SqlParameter("itemType",  params_VM.ItemType),
                            new SqlParameter("searchText",  params_VM.SearchText),
                            new SqlParameter("itemMode",  params_VM.ItemMode),
                            new SqlParameter("priceType",  params_VM.PriceType),
                            new SqlParameter("categoryId",  params_VM.CategoryId),
                            new SqlParameter("pageSize",  params_VM.PageSize),
                            new SqlParameter("page",  params_VM.Page),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("startDate", params_VM.StartDate),
                            new SqlParameter("days", params_VM.Days)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetRecordsBySearch @id,@userLoginId,@menuTag,@location,@latitude,@longitude,@itemType,@searchTExt,@itemMode,@priceType,@categoryId,@pageSize,@page,@mode,@startDate,@days", queryParams).ToList();

        }


        /// <summary>
        /// To Add/Update Explore Classic Dance Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public T SP_InsertUpdateBusinessContentExploreClassicDanceDetail_Get<T>(SP_InsertUpdateBusinessExploreDClassicDanceDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessExploreDClassicDanceDetail @id,@userLoginId,@profilePageTypeId,@title,@description,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Explore Classic Dance Detail By UserLoginId  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessExploreClassicDanceDetail<T>(SP_ManageBusinessExploreClassicDanceDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessExploreClassicDanceDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Insert Explore Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentFindMasterProfilDetail_Get<T>(SP_InsertUpdateBusinessFindMasterProfile_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                             new SqlParameter("exploreType",params_VM.ExploreType),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("image",params_VM.Image),
                            new SqlParameter("scheduleLink",params_VM.ScheduleLink),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessFindMasterProfileDetail @id,@userLoginId,@profilePageTypeId,@title,@exploreType,@description,@image,@scheduleLink,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        ///  To Get Bind Find Master Profile Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessFindMasterProfileDetail<T>(SP_ManageBusinessExploreClassicDanceDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("exploreType", params_VM.ExploreType),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessFindMasterProfileDetail @id,@businessOwnerLoginId,@userLoginId,@exploreType,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Get Find Explore Detail List By BusinessOwnerLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessFindMasterProfileDetaillst<T>(SP_ManageBusinessExploreClassicDanceDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                             new SqlParameter("exploreType", params_VM.ExploreType),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessFindMasterProfileDetail @id,@businessOwnerLoginId,@userLoginId,@exploreType,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Add/Update Instructor and MemberShip Banner Detail For Master Profile 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentInstructorBannerMasterProfileDetail_Get<T>(SP_InsertUpdateBusinessContentInstructorBannerDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("bannerType",params_VM.BannerType),
                            new SqlParameter("title",params_VM.Title),
                             new SqlParameter("subTitle",params_VM.SubTitle),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("bannerImage",params_VM.BannerImage),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessMasterProfileBannerDetail @id,@userLoginId,@profilePageTypeId,@bannerType,@title,@subTitle,@description,@bannerImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }


        /// <summary>
        /// To Get Instructor/MemberShip Banner Detail List  By LoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessInstructorBannerMasterProfileDetaillst<T>(SP_ManageBusinessContentInstructorBannerDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessMasterProfileBannerDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Get Instructor/MemberShip  Banner Image  Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessInstructorBannerMasterProfileDetail<T>(SP_ManageBusinessContentInstructorBannerDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessMasterProfileBannerDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add/Update Instructor About Master Profile Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateInstructorAboutMasterProfileDetail_Get<T>(SP_InsertUpdateInstructorAboutMasterProfile_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                             new SqlParameter("subTitle",params_VM.SubTitle),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("image",params_VM.Image),
                            new SqlParameter("buttonLink",params_VM.ButtonLink),
                             new SqlParameter("buttonText",params_VM.ButtonText),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateInstructorMasterProfileAboutDetail @id,@userLoginId,@profilePageTypeId,@title,@subTitle,@description,@image,@buttonLink,@buttonText,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To  Get Instructor About Detail by LoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessInstructorAboutMasterProfileDetail<T>(SP_ManageInstructorAboutMasterProfileDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageInstructorAboutMasterProfileDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Add/Update Term Condition Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateTermConditionDetail_Get<T>(SP_InsertUpdateBusinessTermCondition_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessTermCondition @id,@userLoginId,@profilePageTypeId,@title,@description,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Term Condition Detail By userLoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessTermConditionDetail<T>(SP_ManageBusinessTermConditionDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessTermConditionDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add /Update MemberShip Package Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateMemberShipPackageDetail_Get<T>(SP_InsertUpdateBusinessMemberShipPackage_Param_VM params_VM)
        {
            if (params_VM.PlanTypeId == 2)
            {
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("planTypeId", params_VM.PlanTypeId)
                            };
                return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessAdvanceMemberShipPackageDetail @id,@userLoginId,@profilePageTypeId,@title,@description,@submittedByLoginId,@mode,@planTypeId", queryParams).FirstOrDefault();

            }
            else
            {
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("planTypeId", params_VM.PlanTypeId)
                            };
                return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessMemberShipPackageDetail @id,@userLoginId,@profilePageTypeId,@title,@description,@submittedByLoginId,@mode,@planTypeId", queryParams).FirstOrDefault();
            }

        }

        /// <summary>
        /// To Get MemberShip Package detail by loginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessMemberShipPackageDetail<T>(SP_ManageBusinessMemberShipPackageDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessMemberShipPackageDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add/Update MemberShip Plan Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateMemberShipPlanDetail_Get<T>(SP_InsertUpdateMemberShipPlan_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("image",params_VM.Image),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateMemberShipPlanDetail @id,@userLoginId,@profilePageTypeId,@title,@image,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get MemberShip Plan Detail List By LoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessMemberShipPackageDetailList<T>(SP_ManageBusinessMemberShipPackageDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessMemberShipPackageDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Get All Class Time Table Detail For VisitorPanel (filter using for time table page)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageAllClassTimeTableDetailList<T>(SP_ManageAllClassTimeTableDetail_Param_VM params_VM)
        {
            if (params_VM.ClassDays == null )
            {
                params_VM.ClassDays = "";
            }
            if(params_VM.ClassMode == null)
            {
                params_VM.ClassMode = "";
            }

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("classCategoryTypeId", params_VM.ClassCategoryTypeId),
                            new SqlParameter("instructorLoginId", params_VM.InstructorLoginId),
                            new SqlParameter("classDays",params_VM.ClassDays),
                            new SqlParameter("classMode",params_VM.ClassMode),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetALLClassTimeTableDetail @id,@businessOwnerLoginId,@userLoginId,@classCategoryTypeId,@instructorLoginId,@classDays,@classMode,@mode", queryParams).ToList();

        }


        // Contact Information to all 
        /// <summary>
        /// To Add/Update Contact Information 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdatBusinessContactContactInformationDetail_Get<T>(SP_InsertUpdateBusinessContentContactInformation_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title",params_VM.Title),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessContentContactInformation_PPCMeta @id,@userLoginId,@profilePageTypeId,@title,@description,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        // Contact Information by UserLoginId 
        /// <summary>
        /// To Get Contact Information For Visitor Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessContactInformationDetail<T>(BusinessContentContactInformation_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessContentContactInformation_PPCMeta @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Update ResetPasswordToken UserLogins
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public T SP_InsertUpdateResetPasswordDetail<T>(SP_InsertUpdateResetPasswordDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("resetPasswordToken", params_VM.ResetPasswordToken),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdate_ResetPasswordDetail @id,@resetPasswordToken,@mode", queryParams).FirstOrDefault();

        }

        ///// <summary>
        ///// To Add Update Custom Form Detail 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="params_VM"></param>
        ///// <returns></returns>
        //public T SP_InsertUpdateCustomFormDetail_Get<T>(SP_InsertUpdateCustomForm_Param_VM params_VM)
        //{
        //    SqlParameter[] queryParams = new SqlParameter[] {
        //                    new SqlParameter("id", params_VM.Id),
        //                     new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
        //                     new SqlParameter("customFormName", params_VM.CustomFormName),
        //                     new SqlParameter("status", params_VM.Status),
        //                     new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
        //                    new SqlParameter("mode", params_VM.Mode),
        //                    };

        //    return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateCustomForm @id,@businessOwnerLoginId,@customFormName,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        //}

        /// <summary>
        /// To Insert Custom Form  Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateCustomFormDetail_Get<T>(SP_InsertUpdateCustomForm_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                             new SqlParameter("customFormName", params_VM.CustomFormName),
                             new SqlParameter("status", params_VM.Status),
                             new SqlParameter("customFormId", params_VM.CustomFormId),
                             new SqlParameter("customFormElementName ", params_VM.CustomFormElementName),
                             new SqlParameter("customFormElementType", params_VM.CustomFormElementType),
                             new SqlParameter("customFormElementValue",params_VM.CustomFormElementValue),
                             new SqlParameter("customFormElementPlaceholder", params_VM.CustomFormElementPlaceholder),
                             new SqlParameter("customFormElementStatus",params_VM.CustomFormElementStatus),
                             new SqlParameter("customFormElementId",params_VM.CustomFormElementId),
                             new SqlParameter("customFormElementOptions",params_VM.CustomFormElementOptions),
                             new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateCustomForm @id,@businessOwnerLoginId,@customFormName,@status,@customFormId,@customFormElementName,@customFormElementType,@customFormElementValue,@customFormElementPlaceholder,@customFormElementStatus,@customFormElementId,@customFormElementOptions,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Custom Form Detail List by BusinessOwnerLoginId  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageCustomFormDetail<T>(SP_ManageCustomFormDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCustomFormDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// To Get Custom Form Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageCustomFormDetailById<T>(SP_ManageCustomFormDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCustomFormDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call Stored Procedure SP_InsertUpdateEventCategory_Get
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateEventCategory_Get<T>(SP_InsertUpdateEventCategory_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("categoryName", params_VM.CategoryName),
                            new SqlParameter("status", params_VM.Status),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateEventCategory @id,@categoryName,@status,@mode", queryParams).FirstOrDefault();
        }



        /// <summary>
        /// Call stored-procedure SP_ManageEventCategory_GetAll - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageEventCategory_GetAll<T>(SP_ManageEventCategory_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEventCategory @id,@mode", queryParams).ToList();
        }


        /// <summary>
        /// Call stored-procedure sp_ManageClassCategoryType - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageEventCategory_Get<T>(SP_ManageEventCategory_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("mode", params_VM.Mode),
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEventCategory @id,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call Stored Procedure SP_Insert Update Staff Category_Get
        /// </summary>
        /// <typeparam name="T">Return Type Class</typeparam>
        /// <param name="params_VM">Stored Procedure Params VM</param>
        /// <returns>Returns Object</returns>
        public T SP_InsertUpdateStaffCategory_Get<T>(SP_InsertUpdateStaffCategory_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("Name", params_VM.Name),
                            new SqlParameter("status", params_VM.IsActive),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateStaffCategory @id,@name,@status,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// Call stored-procedure SP_ManageStaffCategory_GetAll - Get List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public List<T> SP_ManageStaffCategories_GetAll<T>(SP_ManageStaffCategories_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageStaffCategories @id,@mode", queryParams).ToList();
        }


        /// <summary>
        /// Call stored-procedure sp_ManageStaffCategoryType - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageStaffCategories_Get<T>(SP_ManageStaffCategories_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("mode", params_VM.Mode),
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageStaffCategories @id,@mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        ///  To Get Follower User Count Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageFollower_Get<T>(SP_ManageToGetFollowerUser_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("followingUserLoginId",params_VM.FollowingUserLoginId),
                            new SqlParameter("favouriteUserLoginId", params_VM.FavouriteUserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageToGetFollowerUser @id,@followingUserLoginId,@favouriteUserLoginId,@mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        /// To Get Class Refer Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageClassReferDetail_Get<T>(SP_ManageClassReferDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassReferDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// To insert Update Class Refer Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateClassReferDetail<T>(SP_InsertUpdateClassReferDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId",params_VM.UserLoginId),
                             new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                              new SqlParameter("title", params_VM.Title),
                               new SqlParameter("description", params_VM.Description),
                                new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateClassReferDetail @id,@userLoginId,@profilePageTypeId,@title,@description,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Add/Update About Detail Through SuperAdmin 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateSuperAdminAboutDetail_Get<T>(SP_InsertUpdateSuperAdminAboutContent_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("userLoginId", params_VM.UserLoginId),
                             new SqlParameter("aboutTitle", params_VM.AboutTitle),
                             new SqlParameter("aboutDescription", params_VM.AboutDescription),
                             new SqlParameter("ourMissionTitle",params_VM.OurMissionTitle),
                             new SqlParameter("ourMissionDescription",params_VM.OurMissionDescription),
                             new SqlParameter("image", params_VM.Image),
                             new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateSuperAdminAboutDetail @id,@userLoginId,@aboutTitle,@aboutDescription,@ourMissionTitle,@ourMissionDescription,@image,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get  About Detail  SuperAdmin 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageSuperAdminAboutDetail_Get<T>(SP_ManageSuperAdminAboutDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageSuperAdminAboutDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Contact Detail In SuperAdmin Panel 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageContactDetail_Get<T>(SP_ManageContactDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageContactDetail @id,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Insert Update Course Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateCourse_Param_Get<T>(SP_InsertUpdateCourse_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[]
             {
                                new SqlParameter("id", params_VM.Id),
                                new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                                new SqlParameter("name", params_VM.Name),
                                new SqlParameter("shortDescription", params_VM.ShortDescription),
                                 new SqlParameter("description", params_VM.Description),
                                new SqlParameter("courseStartDate", params_VM.CourseStartDate),
                                new SqlParameter("courseMode", params_VM.CourseMode),
                                new SqlParameter("duration ", params_VM.Duration),
                                new SqlParameter("price", params_VM.Price),
                                new SqlParameter("onlineCourseLink",  params_VM.OnlineCourseLink),
                                new SqlParameter("courseOfflineAddress",  params_VM.Address),
                                new SqlParameter("courseOfflineCountry",params_VM.Country),
                                new SqlParameter("courseOfflineCity",params_VM.City),
                                new SqlParameter("courseOfflinePinCode",params_VM.Pincode),
                                new SqlParameter("courseOfflineLandMark",params_VM.LandMark),
                                new SqlParameter("courseOfflineState",params_VM.State),
                                new SqlParameter("isPaid",params_VM.IsPaid),
                                new SqlParameter("courseOnlineURLLinkPassword",params_VM.CourseURLLinkPassword),
                                new SqlParameter("instructorLoginId", params_VM.InstructorLoginId),
                                new SqlParameter("courseImage", params_VM.CourseImage),
                                new SqlParameter("courseLocationLatitude",params_VM.Latitude),
                                new SqlParameter("courseLocationLongitude",params_VM.Longitude),
                                new SqlParameter("courseCategoryId", params_VM.CourseCategoryId),
                                new SqlParameter("durationType",params_VM.DurationType),
                                new SqlParameter("howToBookText",params_VM.HowToBookText),
                                new SqlParameter("examId",params_VM.ExamId),
                                new SqlParameter("examType",params_VM.ExamType),
                                new SqlParameter("certificateType",params_VM.CertificateType),
                                new SqlParameter("certificateId",params_VM.CertificateId),
                                new SqlParameter("coursePriceType",params_VM.CoursePriceType),
                                new SqlParameter("groupId",params_VM.GroupId),
                                new SqlParameter("isActive",params_VM.IsActive),
                                new SqlParameter("certificateProfileId",params_VM.CertificateProfileId),
                                new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                                new SqlParameter("mode", params_VM.Mode),
             };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateCourse @id,@businessOwnerLoginId,@name,@shortDescription,@description,@courseStartDate,@courseMode,@duration,@price,@onlineCourseLink,@courseOfflineAddress,@courseOfflineCountry,@courseOfflineCity,@courseOfflinePinCode,@courseOfflineLandMark,@courseOfflineState,@isPaid,@courseOnlineURLLinkPassword,@instructorLoginId,@courseImage,@courseLocationLatitude,@courseLocationLongitude,@courseCategoryId,@durationType,@howToBookText,@examId,@examType,@certificateType,@certificateId,@coursePriceType,@groupId,@isActive,@certificateProfileId,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Course Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageCourseDetail_Get<T>(SP_ManageCourseDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCourseDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Course Detail List 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageCourseDetailList_Get<T>(SP_ManageCourseDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCourseDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }





        /// <summary>
        /// To Add/Update Contact Number Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateContactNumberDetail_Get<T>(SP_InsertUpdateContactNumber_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("location1", params_VM.Location1),
                             new SqlParameter("contactNumber1", params_VM.ContactNumber1),
                            new SqlParameter("location2", params_VM.Location2),
                             new SqlParameter("contactNumber2", params_VM.ContactNumber2),
                              new SqlParameter("location3", params_VM.Location3),
                             new SqlParameter("contactNumber3", params_VM.ContactNumber3),
                              new SqlParameter("location4", params_VM.Location4),
                             new SqlParameter("contactNumber4", params_VM.ContactNumber4),
                              new SqlParameter("location5", params_VM.Location5),
                             new SqlParameter("contactNumber5", params_VM.ContactNumber5),
                              new SqlParameter("location6", params_VM.Location6),
                             new SqlParameter("contactNumber6", params_VM.ContactNumber6),
                              new SqlParameter("location7", params_VM.Location7),
                             new SqlParameter("contactNumber7", params_VM.ContactNumber7),
                              new SqlParameter("location8", params_VM.Location8),
                             new SqlParameter("contactNumber8", params_VM.ContactNumber8),
                             new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateContactNumber @id,@userLoginId,@location1,@contactNumber1,@location2,@contactNumber2,@location3,@contactNumber3,@location4,@contactNumber4,@location5,@contactNumber5,@location6,@contactNumber6,@location7,@contactNumber7,@location8,@contactNumber8,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Contact Number Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageContactNumberDetail_Get<T>(SP_ManageContactNumberDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageContactNumberDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageStaffCategoryType - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageStaffCategory_Get<T>(SP_ManageStaffCategories_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("mode", params_VM.Mode),
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageStaffCategory @id,@mode", queryParams).FirstOrDefault();
        }


        /// <summary>
        /// Get event booking details 
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageEventBooking_GetAll<T>(SP_ManageEventDetails_Params_VM params_VM)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
            new SqlParameter("id", params_VM.Id),
            new SqlParameter("userLoginId",params_VM.UserLoginId),
            new SqlParameter("eventId",params_VM.EventId),
            new SqlParameter("mode", params_VM.Mode)
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEventBooking @id,@userLoginId,@eventId,@mode", queryParamsGetEvent).ToList();
        }

        /// <summary>
        /// get event booking details 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageEventBooking_Get<T>(SP_ManageEventDetails_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
            new SqlParameter("id", params_VM.Id),
            new SqlParameter("userLoginId",params_VM.UserLoginId),
            new SqlParameter("eventId",params_VM.EventId),
            new SqlParameter("mode", params_VM.Mode)
            };
            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEventBooking @id,@userLoginId,@eventId,@mode", queryParamsGetEvent).FirstOrDefault();

        }
        /// <summary>
        /// Get event booking details 
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns>Returns the Table-Row data</returns>
        public List<T> SP_ManageEventDetails_Get<T>(SP_ManageEventDetails_Params_VM params_VM)
        {
            SqlParameter[] queryParamsGetEvent = new SqlParameter[] {
            new SqlParameter("id", params_VM.Id),
            new SqlParameter("userLoginId",params_VM.UserLoginId),
            new SqlParameter("eventId",params_VM.EventId),
            new SqlParameter("mode", params_VM.Mode)
            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageEventBooking @id,@userLoginId,@eventId,@mode", queryParamsGetEvent).ToList();
        }

        public T SP_ManagePlanBooking_GetById<T>(SP_ManagePlanBooking_Params_VM params_VM)
        {
            return SP_ManagePlanBooking_GetAll<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// To Get Business Plan Package Detail Booking List 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManagePlanBooking_GetAll<T>(SP_ManagePlanBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", params_VM.Id),
                    new SqlParameter("userLoginId", params_VM.UserLoginId),
                    new SqlParameter("planId", params_VM.PlanId),
                    new SqlParameter("mode", params_VM.Mode),
                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).ToList();
        }

        /// <summary>
        /// get event booking details by id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageEventBooking_GetById<T>(SP_ManageEventDetails_Params_VM params_VM)
        {
            return SP_ManageEventDetails_Get<T>(params_VM).FirstOrDefault();
        }


        /// <summary>
        /// get plan details list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        //public List<T> SP_ManagePlanBooking_GetAll<T>(SP_ManagePlanBooking_Params_VM params_VM)
        //{
        //    SqlParameter[] queryParams = new SqlParameter[] {
        //                    new SqlParameter("id", params_VM.Id),
        //                    new SqlParameter("userLoginId", params_VM.UserLoginId),
        //                    new SqlParameter("planId", params_VM.PlanId),
        //                    new SqlParameter("mode", params_VM.Mode),
        //                    };

        //    return _dbContext.Database.SqlQuery<T>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).ToList();
        //}


        /// <summary>
        /// get plan details by id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManagePlanBooking_Get<T>(SP_ManagePlanBooking_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                             new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("planId", params_VM.PlanId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).FirstOrDefault();
        }




        /// <summary>
        /// To Get Course Booking Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageCourseBooking_GetByID<T>(SP_ManageCourseBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("courseId", params_VM.CourseId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("joiningDate", params_VM.JoiningDate),
                           new SqlParameter("studentUserLoginId", params_VM.StudentUserLoginId),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCourseBooking @id,@userLoginId,@courseId,@mode,@joiningDate,@studentUserLoginId", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Use In VisitorPanel For Course Booking Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageCourseBooking_GetById<T>(SP_ManageCourseBooking_Params_VM params_VM)
        {
            return SP_ManageCourseBooking_GetAll<T>(params_VM).FirstOrDefault();
        }


        public List<T> SP_ManageCourseBooking_GetAll<T>(SP_ManageCourseBooking_Params_VM params_VM)
        {
            // Get Course-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("courseId", params_VM.CourseId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("joiningDate", params_VM.JoiningDate),
                              new SqlParameter("studentUserLoginId", params_VM.StudentUserLoginId),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageCourseBooking @id,@userLoginId,@courseId,@mode,@joiningDate,@studentUserLoginId", queryParams).ToList();
        }



        /// <summary>
        /// Manage Course Booking Get By Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageCourseBooking_Get<T>(SP_ManageCourseBooking_Params_VM params_VM)
        {
            return SP_ManageCourseBooking_GetAll<T>(params_VM).FirstOrDefault();
        }

        /// <summary>
        /// insert update course Booking 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateCourseBooking_Get<T>(SP_InsertUpdateCourseBooking_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("@id", params_VM.Id),
                            new SqlParameter("@orderId", params_VM.OrderId),
                            new SqlParameter("@courseId", params_VM.CourseId),
                            new SqlParameter("@UserLoginId", params_VM.UserLoginId),
                            new SqlParameter("@courseStartDate", params_VM.CourseStartDate),
                            new SqlParameter("@courseEndDate", params_VM.CourseEndDate),
                            new SqlParameter("@mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateCourseBooking @id,@orderId,@courseId,@UserLoginId,@courseStartDate,@courseEndDate,@mode", queryParams).FirstOrDefault();
        }



        /// <summary>
        /// To Add/Update Education Banner Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateBusinessContentEducationBannerProfileDetail_Get<T>(SP_InsertUpdateEducationBannerDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("bannerType",params_VM.BannerType),
                            new SqlParameter("title",params_VM.Title),
                             new SqlParameter("subTitle",params_VM.SubTitle),
                            new SqlParameter("description",params_VM.Description),
                            new SqlParameter("bannerImage",params_VM.BannerImage),
                            new SqlParameter("buttonText", params_VM.ButtonText),
                            new SqlParameter("buttonLink",params_VM.ButtonLink),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessEducationBannerDetail @id,@userLoginId,@profilePageTypeId,@bannerType,@title,@subTitle,@description,@bannerImage,@buttonText,@buttonLink,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// To Get Education Banner Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessEducationBannerDetail_Get<T>(SP_ManageBusinessContentBannerDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessEducationBannerDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Education Banner Detail List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageBusinessEducationBannerDetailList_Get<T>(SP_ManageBusinessContentBannerDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessEducationBannerDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// Call stored-procedure sp_ManageClassBooking - Get Single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns>Returns Object</returns>
        public T SP_ManageClassBooking_GetById<T>(SP_ManageClassBooking_Params_VM params_VM)
        {
            // Get Class-Booking-Table-Data
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("classId", params_VM.ClassId),
                            new SqlParameter("batchId", params_VM.BatchId),
                            new SqlParameter("mode", params_VM.Mode),
                            new SqlParameter("joiningDate", params_VM.JoiningDate)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassBooking @id,@userLoginId,@classId,@batchId,@mode,@joiningDate", queryParams).FirstOrDefault();
        }


        /// <summary>
        /// To Get Course Detail By Course Id and BusienssOwnerLoginId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_ManageCourseSerachDetail_GetAll<T>(SP_GetBusinessCourseSearch_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                           new SqlParameter("businessOwnerLoginId",params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("name",params_VM.Name),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec GetBusinessCourseDetailForSearch @id,@businessOwnerLoginId,@userLoginId,@name,@mode", queryParams).ToList();
        }

        /// <summary>
        /// To Add/Update Sports Booking Cheaque Detail 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateSportsBookingDetail_Get<T>(SP_InsertUpdateSportsBookingCheaque_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                             new SqlParameter("userLoginId", params_VM.UserLoginId),
                             new SqlParameter("name", params_VM.Name),
                             new SqlParameter("surname", params_VM.SurName),
                             new SqlParameter("email",params_VM.Email),
                             new SqlParameter("phoneNumber",params_VM.PhoneNumber),
                             new SqlParameter("bookedId",params_VM.BookedId),
                             new SqlParameter("department",params_VM.Department),
                             new SqlParameter("apartment",params_VM.Apartment),
                             new SqlParameter("houseNumber",params_VM.PhoneNumber),
                             new SqlParameter("message",params_VM.Message),
                             new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                             new SqlParameter("tennisImage", params_VM.TennisImage),
                             new SqlParameter("tennisTitle", params_VM.TennisTitle),
                             new SqlParameter("selectDate", params_VM.SelectDate),
                             new SqlParameter("roomService", params_VM.RoomService),
                             new SqlParameter("roomTime", params_VM.RoomTime),
                             new SqlParameter("playerCount", params_VM.PlayerCount),
                             new SqlParameter("request", params_VM.Request),
                             new SqlParameter("price", params_VM.Price),
                             new SqlParameter("slotId", params_VM.SlotId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateBusinessSportsBookingDetail @id,@businessOwnerLoginId,@userLoginId,@name,@surname,@email,@phoneNumber,@bookedId,@department,@apartment,@houseNumber,@message,@submittedByLoginId,@tennisImage,@tennisTitle,@selectDate,@roomService,@roomTime,@playerCount,@request,@price,@slotId,@mode", queryParams).FirstOrDefault();
        }


        public T SP_ManageBusinessSportsBookingCheaqueDetail<T>(SP_ManageBusinessSportsBookingCheaqueDetail_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessSportsBookingCheaqueDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// To Get Main Package Detail By Id 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageMainPackage_Get<T>(SP_ManageBusinessPlans_Params_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",params_VM.Id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).FirstOrDefault();
        }

        /// <summary>
        /// To Get Current Main Package Detail By UserLoginId 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param_VM"></param>
        /// <returns></returns>
        public T SP_ManageMainPackageBookingGet<T>(SP_ManageMainPackageBooking_Param_VM param_VM)
        {
            SqlParameter[] queryParamsGetPackage = new SqlParameter[] {
                            new SqlParameter("id", param_VM.Id),
                            new SqlParameter("userLoginId", param_VM.UserLoginId),
                            new SqlParameter("mainPlanId", param_VM.MainPlanId),
                            new SqlParameter("mode", param_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageMainPlanBooking @id,@userLoginId,@mainPlanId,@mode", queryParamsGetPackage).FirstOrDefault();
        }

        /// <summary>
        /// To Add Update Tennis Time Slot 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_InsertUpdateTennisTimeSlot_Get<T>(SP_InsertUpdateTennisAreaTimeSlot_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("slotId", params_VM.SlotId),
                            new SqlParameter("time",params_VM.Time),
                            new SqlParameter("submittedByLoginId", params_VM.SubmittedByLoginId),
                            new SqlParameter("mode", params_VM.Mode)
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_InsertUpdateTennisTimeSlotDetail @id,@userLoginId,@slotId,@time,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

        }

        /// <summary>
        /// to get training details for student panel 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public List<T> SP_GetAllStudentTrainingDetail_GetAll<T>(SP_GetAllTrainingDetailSearch_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit) ,
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllStudentTrainingDetail @id,@userLoginId,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }

        /// <summary>
        /// to get first or default data using  this SP
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageStudentEventDetail_Get<T>(SP_GetAllStudentEventList_Params_VM params_VM)
        {
            return SP_GetAllStudentEventDetail_GetAll<T>(params_VM).FirstOrDefault();
        }
        /// <summary>
        /// to get event details for student panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public List<T> SP_GetAllStudentEventDetail_GetAll<T>(SP_GetAllStudentEventList_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                           new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("lastRecordId",params_VM.LastRecordId),
                            new SqlParameter("recordLimit",params_VM.RecordLimit) ,
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_GetAllStudentEventDetail @id,@userLoginId,@lastRecordId,@recordLimit,@mode", queryParams).ToList();
        }
        /// <summary>
        /// to get class intermediate 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public T SP_ManageClassInterMediate_Get<T>(SP_ManageClassIntermediatesDetail_Params_VM params_VM)
        {
            // Get Training-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageClassInterMediate @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// to get advance membership package details
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageBusinessAdvanceMemberShipPackageDetail<T>(SP_ManageBusinessMemberShipPackageDetail_Param_VM params_VM)
        {

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                             new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageBusinessAdvanceMemberShipPackageDetail @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// to get business timing details
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>

        public List<T> SP_ManageBusinessTennisTiming_GetAll<T>(SP_ManageTennisTiming_Params_VM params_VM)
        {
            // Get Tennis-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("slotId", params_VM.slotId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

              return _dbContext.Database.SqlQuery<T>("exec sp_ManageTennisTimeSlotDetail @id,@businessOwnerLoginId,@userLoginId,@slotId,@mode", queryParams).ToList();

        }

        /// <summary>
        /// to verify otp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_VerifyOtpStudent<T>(SP_InsertUpdateStudent_Params_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[]
                                    {
            new SqlParameter("email", params_VM.Email),
            new SqlParameter("otp",params_VM.OTP),
            new SqlParameter("mode",1),
                                    };

            return _dbContext.Database.SqlQuery<T>("exec sp_VerifyOtpUserRegistration @email,@otp,@mode", queryParams).FirstOrDefault();

        }
        /// <summary>
        /// call sp to get room details for firstor default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params_VM"></param>
        /// <returns></returns>
        public T SP_ManageMasterProfileRoomDetail_GetbyId<T>(SP_ManageBusinessContentTennis_PPCMeta_Params_VM params_VM)
        {

            return SP_ManageMasterProfileRoomDetail_Get<T>(params_VM).FirstOrDefault();
        }
        /// <summary>
        /// Call stored-procedure sp_ManageBusinessTennisDetailList
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="params_VM">Business-Tennis Detail Stored-Procedure Params</param>
        /// <returns>Return Object</returns>
        public List<T> SP_ManageMasterProfileRoomDetail_Get<T>(SP_ManageBusinessContentTennis_PPCMeta_Params_VM params_VM)
        {
            // Get Tennis-Record-Detail-By-Id
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", params_VM.BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("mode", params_VM.Mode),
                            };

            return _dbContext.Database.SqlQuery<T>("exec sp_ManageRoomDetails @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).ToList();

        }
        /////Tennis Image For Sports page
        /// <summary>
        /// Call stored-procedure sp_InsertUpdateTennis
        /// </summary>
        /// <typeparam name="T">Response Type Model</typeparam>
        /// <param name="params_VM">Stored Procedure Parameters ViewModel</param>
        /// <returns>Return Type Model Result</returns>
        public T SP_InsertUpdateMasterProfileRoomDetails_Get<T>(SP_InsertUpdateBusinessContentTennis_Param_VM params_VM)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", params_VM.Id),
                            new SqlParameter("userLoginId", params_VM.UserLoginId),
                            new SqlParameter("profilePageTypeId", params_VM.ProfilePageTypeId),
                            new SqlParameter("title", params_VM.Title),
                            new SqlParameter("subTitle", params_VM.SubTitle),
                            new SqlParameter("description", params_VM.Description),
                            new SqlParameter("tennisImage", params_VM.TennisImage),
                            new SqlParameter("price" ,params_VM.Price),
                            new SqlParameter("submittedByLoginId", params_VM.UserLoginId),
                            new SqlParameter("slotId", params_VM.SlotId),
                            new SqlParameter("mode", params_VM.Mode),

                            };

            return _dbContext.Database.SqlQuery<T>("exec Sp_InsertUpdateMasterProfileRoomDetails @id,@userLoginId,@profilePageTypeId,@title,@subTitle, @description, @tennisImage,@price,  @submittedByLoginId,@slotId, @mode", queryParams).FirstOrDefault();
        }

    }
   
}