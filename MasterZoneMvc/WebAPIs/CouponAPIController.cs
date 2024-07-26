using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class CouponAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private CouponService couponService;
        private PlanService planService;
        private EventService eventService;
        private ClassService classService;
        private TrainingService trainingService;

        public CouponAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            couponService = new CouponService(db);
            planService = new PlanService(db);
            eventService = new EventService(db);
            classService = new ClassService(db);
            trainingService = new TrainingService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// To add Coupon by business 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Coupon/AddUpdate")]
        public HttpResponseMessage AddUpdateCoupon()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _UserLoginId = validateResponse.BusinessAdminLoginId;


                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                const int length = 10;
                var random = new Random();
                string code = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                Coupon_VM discount_VM = new Coupon_VM();
                discount_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                discount_VM.Name = HttpRequest.Params["Name"].Trim();
                discount_VM.Description = HttpRequest.Params["Description"].Trim();
                discount_VM.StartDate = HttpRequest.Params["StartDate"].Trim();
                discount_VM.EndDate = HttpRequest.Params["EndDate"].Trim();
                discount_VM.IsFixedAmount = Convert.ToInt32(HttpRequest.Params["IsFixedAmount"]);
                discount_VM.DiscountValue = Convert.ToDecimal(HttpRequest.Params["DiscountValue"]);
                discount_VM.DiscountFor = Convert.ToInt32(HttpRequest.Params["DiscountFor"]);
                discount_VM.SelectedStudent = HttpRequest.Params["SelectedStudent"].Trim();
                discount_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);


                // Validate infromation passed
                Error_VM error_VM = discount_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SPResponseViewModel resp = new SPResponseViewModel();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", discount_VM.Id),
                            new SqlParameter("userLoginId ", _UserLoginId),
                            new SqlParameter("name",discount_VM.Name),
                            new SqlParameter("description",discount_VM.Description),
                            new SqlParameter("code ",code),
                            new SqlParameter("startDate  ",discount_VM.StartDate),
                            new SqlParameter("endDate  ",discount_VM.EndDate),
                            new SqlParameter("isfixedAmount   ",discount_VM.IsFixedAmount),
                            new SqlParameter("discountValue  ",discount_VM.DiscountValue),
                            new SqlParameter("totalUsed   ",""),
                            new SqlParameter("discountFor", discount_VM.DiscountFor),
                            new SqlParameter("selectedStudent", discount_VM.SelectedStudent),
                            new SqlParameter("submittedByLoginId",_LoginID_Exact),
                            new SqlParameter("mode",discount_VM.Mode),
                            };

                resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateCoupon @id,@userLoginId,@name,@description,@code,@startDate,@endDate,@isfixedAmount,@discountValue,@totalUsed,@discountFor,@selectedStudent,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get All Coupon with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Coupon/GetAllByPagination")]
        public HttpResponseMessage GetAllDiscountByBusinessOwnerForDataTablePagination()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                CouponList_Pagination_SQL_Params_VM _Params_VM = new CouponList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = couponService.GetCouponsListByBusinessOwner_Pagination(HttpRequestParams, _Params_VM);

                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = Resources.BusinessPanel.Success,
                    draw = paginationResponse.draw,
                    data = paginationResponse.data,
                    recordsTotal = paginationResponse.recordsTotal,
                    recordsFiltered = paginationResponse.recordsFiltered
                };

                return Request.CreateResponse(HttpStatusCode.OK, objResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }

        }

        /// <summary>
        /// Get Coupon  Detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Coupon/GetById/{id}")]
        public HttpResponseMessage GetById(int id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Get Notification-Record-Detail 
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                             new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                             new SqlParameter("couponCode",""),
                            new SqlParameter("mode", "1")
                            };

                var responseDiscount = db.Database.SqlQuery<Coupon_VM>("exec sp_ManageCoupon @id,@userLoginId,@businessOwnerLoginId,@couponCode,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = responseDiscount;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        //[HttpGet]
        //[Authorize(Roles = "BusinessAdmin")]
        //[Route("api/Discount/GetAllDiscount")]
        //public HttpResponseMessage GetByDetailId()
        //{
        //    ApiResponse_VM apiResponse = new ApiResponse_VM();
        //    try
        //    {
        //        var validateResponse = ValidateLoggedInUser();
        //        if (validateResponse.ApiResponse_VM.status < 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
        //        }

        //        long _LoginID_Exact = validateResponse.UserLoginId;
        //        long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

        //        List<Coupon_VM> discount_VMs = new List<Coupon_VM>();
        //        // Get Notification-Record-Detail 
        //        SqlParameter[] queryParams = new SqlParameter[] {
        //                    new SqlParameter("id","0"),
        //                    new SqlParameter("userLoginId", _LoginID_Exact),
        //                    new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
        //                    new SqlParameter("couponCode", ""),
        //                    new SqlParameter("mode", "2")
        //                    };

        //        var resp = db.Database.SqlQuery<Coupon_VM>("exec sp_ManageCoupon @id,@userLoginId,@businessOwnerLoginId,@couponCode,@mode", queryParams).ToList();

        //        apiResponse.status = 1;
        //        apiResponse.message = Resources.BusinessPanel.Success;
        //        apiResponse.data = resp;

        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.status = -500;
        //        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}

        /// <summary>
        /// To delete the Coupon Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Coupon/DeleteById/{id}")]
        public HttpResponseMessage DeleteDiscountById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _UserLoginId = validateResponse.BusinessAdminLoginId;

                // Insert-Update Group Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId ", _UserLoginId),
                            new SqlParameter("name",""),
                            new SqlParameter("description",""),
                            new SqlParameter("code ",""),
                            new SqlParameter("startDate  ",""),
                            new SqlParameter("endDate  ",""),
                            new SqlParameter("isfixedAmount   ",""),
                            new SqlParameter("discountValue  ","0"),
                            new SqlParameter("totalUsed   ",""),
                            new SqlParameter("discountFor", ""),
                            new SqlParameter("selectedStudent", ""),
                            new SqlParameter("submittedByLoginId",_LoginId),
                            new SqlParameter("mode","3"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateCoupon @id,@userLoginId,@name,@description,@code,@startDate,@endDate,@isfixedAmount,@discountValue,@totalUsed,@discountFor,@selectedStudent,@submittedByLoginId,@mode", queryParams).FirstOrDefault();


                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        //[HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        //[Route("api/Group/GetById")]
        //public HttpResponseMessage GetDiscountById(long id)
        //{
        //    ApiResponse_VM apiResponse = new ApiResponse_VM();
        //    try
        //    {
        //        var validateResponse = ValidateLoggedInUser();
        //        if (validateResponse.ApiResponse_VM.status < 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
        //        }

        //        long _LoginId = validateResponse.UserLoginId;
        //        CouponDetailWithStudent_VM discount_VM = new CouponDetailWithStudent_VM();

        //        // Get Group By Id
        //        discount_VM = couponService.GetCouponDetailWithStudentById(id);

        //        apiResponse.status = 1;
        //        apiResponse.message = Resources.BusinessPanel.Success;
        //        apiResponse.data = discount_VM;

        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.status = -500;
        //        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}

        /// <summary>
        /// Get Coupon Detials With students Data by Coupon-Id
        /// </summary>
        /// <param name="id">Coupon Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Coupon/GetCouponDetialsByStudentDetail")]
        public HttpResponseMessage GetDiscountDetailWithById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                CouponDetailWithStudent_VM discount_VM = new CouponDetailWithStudent_VM();
                List<StudentList_ForBusiness_VM> studentList = new List<StudentList_ForBusiness_VM>();

                // Get Group By Id
                SqlParameter[] queryParamsGetGroup = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("couponCode",""),
                            new SqlParameter("mode", "3")

                            }; 

                discount_VM = db.Database.SqlQuery<CouponDetailWithStudent_VM>("exec sp_ManageCoupon @id,@userLoginId,@businessOwnerLoginId,@couponCode,@mode", queryParamsGetGroup).FirstOrDefault();
                discount_VM.DiscountStudent = new List<StudentList_ForBusiness_VM>();
                
                if (discount_VM != null && discount_VM.DiscountFor == 2)
                {
                    // Get Group-Member Details By Group-Id
                    SqlParameter[] queryParamsGetGroupMembers = new SqlParameter[] {
                            new SqlParameter("id", id),
                              new SqlParameter("studentList",discount_VM.SelectedStudents),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "1")
                            };

                    discount_VM.DiscountStudent = db.Database.SqlQuery<StudentList_ForBusiness_VM>("exec sp_GetStudentListById @id,@studentList,@userLoginId,@mode", queryParamsGetGroupMembers).ToList();
                }

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = discount_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get All  Users Coupons Detail For Student
        /// </summary>
        /// <returns>List of Users Coupons Available for Student</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Coupon/GetByAll")]
        public HttpResponseMessage GetByAllCoupon()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }


                long _LoginID_Exact = validateResponse.UserLoginId;
                //long _UserLoginId = validateResponse.BusinessAdminLoginId;

                List<CouponDetailForStudent_VM> couponStudent_VM = new List<CouponDetailForStudent_VM>();
                // Get Notification-Record-Detail 
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentUserLoginId ", _LoginID_Exact),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

                couponStudent_VM = db.Database.SqlQuery<CouponDetailForStudent_VM>("exec sp_GetAllCouponsByStudent @id,@studentUserLoginId,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = couponStudent_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Verify Coupon if its available to be used and has not been used before
        /// </summary>
        /// <param name="couponCode">Coupon Code Value</param>
        /// <param name="itemOwnerLoginId">Item Owner-Login-Id (Certificate creator, Plan creator-Login-Id)</param>
        /// <returns>List of Coupons Available for Student</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Coupon/Verify")]
        public HttpResponseMessage VerifyCoupon(string couponCode, long itemId, string itemType)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                if (String.IsNullOrEmpty(couponCode))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.CouponCodeRequired;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long itemOwnerLoginId = 0;
                string[] itemTypes = { "plan", "certificate", "event", "class", "training" };
                
                itemType = itemType.ToLower();

                if (!itemTypes.Contains(itemType))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.InvalidOrderItemType_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    if (itemType == "plan")
                    {
                        var planData = planService.GetPlanDataById(itemId,0);
                        itemOwnerLoginId = planData.BusinessOwnerLoginId;
                    }
                    else if(itemType == "event")
                    {
                        var eventData = eventService.GetEventDataById(itemId);
                        itemOwnerLoginId = eventData.UserLoginId;
                    }
                    else if (itemType == "class")
                    {
                        var classData = classService.GetClassDataByID(itemId);
                        itemOwnerLoginId = classData.BusinessOwnerLoginId;
                    }
                    else if (itemType == "training")
                    {
                        var trainingData = trainingService.GetTrainingDataById(itemId);
                        itemOwnerLoginId = trainingData.UserLoginId;
                    }
                    // check for other types here
                }

                // Get Coupon Code Detail By Coupon Code
                var couponDetail = couponService.GetCouponDetailByCouponCode(couponCode, itemOwnerLoginId);
                DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (couponDetail == null)
                {
                    //coupon not found
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.InvalidCoupon;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else if (_couponEndDate < DateTime.UtcNow.Date)
                {
                    //coupon has been expired
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get User Consumption Detail for same Coupon if it has been used
                var couponConsumption = couponService.GetUserCouponConsumption(_LoginID_Exact, couponDetail.Id);
                if (couponConsumption != null)
                {
                    // coupon code already used
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Availabe to use
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = couponDetail;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Verify Student Coupon if its available to be used and has not been used before
        /// </summary>
        /// <param name="couponCode">Coupon Code Value</param>
        /// <param name="itemOwnerLoginId">Item Owner-Login-Id (Certificate creator, Plan creator-Login-Id)</param>
        /// <returns>List of Coupons Available for Student</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Coupon/VerifyStudentCoupon")]
        public HttpResponseMessage VerifyCoupon(long studentLoginId, string couponCode, long itemId, string itemType)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                if (String.IsNullOrEmpty(couponCode))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.CouponCodeRequired;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long itemOwnerLoginId = 0;
                string[] itemTypes = { "plan", "certificate", "event", "class", "training" };

                itemType = itemType.ToLower();

                if (!itemTypes.Contains(itemType))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.InvalidOrderItemType_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    if (itemType == "plan")
                    {
                        var planData = planService.GetPlanDataById(itemId,0);
                        itemOwnerLoginId = planData.BusinessOwnerLoginId;
                    }
                    else if (itemType == "event")
                    {
                        var eventData = eventService.GetEventDataById(itemId);
                        itemOwnerLoginId = eventData.UserLoginId;
                    }
                    if (itemType == "class")
                    {
                        var classData = classService.GetClassDataByID(itemId);
                        itemOwnerLoginId = classData.BusinessOwnerLoginId;
                    }
                    if (itemType == "training")
                    {
                        var trainingData = trainingService.GetTrainingDataById(itemId);
                        itemOwnerLoginId = trainingData.UserLoginId;
                    }
                    // check for other types here
                }

                // Get Coupon Code Detail By Coupon Code
                var couponDetail = couponService.GetCouponDetailByCouponCode(couponCode, itemOwnerLoginId);
                DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (couponDetail == null)
                {
                    //coupon not found
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.InvalidCoupon;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                else if (_couponEndDate < DateTime.UtcNow.Date)
                {
                    //coupon has been expired
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get User Consumption Detail for same Coupon if it has been used
                var couponConsumption = couponService.GetUserCouponConsumption(studentLoginId, couponDetail.Id);
                if (couponConsumption != null)
                {
                    // coupon code already used
                    apiResponse.status = -1;
                    apiResponse.message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Availabe to use
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = couponDetail;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }
    }
}