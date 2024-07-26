using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
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
using MasterZoneMvc.Services;
using MasterZoneMvc.Common.ValidationHelpers;
using System.Numerics;
using MasterZoneMvc.Repository;

namespace MasterZoneMvc.WebAPIs
{
    public class TransferPackageAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private TransferPackageService transferPackageService;
        private StoredProcedureRepository storedProcedureRepository;
        public TransferPackageAPIController()
        {
            db = new MasterZoneDbContext();
            transferPackageService = new TransferPackageService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Validate Logged-in user. 
        /// </summary>
        /// <returns></returns>
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);

            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Get All Follower-Students List linked with the BusinessOnwer
        /// </summary>
        /// <returns>List of All-Business-Students</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/TransferPackage/GetAllFollowerStudents")]
        public HttpResponseMessage GetAllFollowerStudentListByBusiness()
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


                List<TransferPackageStudentList_VM> resp = new List<TransferPackageStudentList_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "1"),

                            };

                resp = db.Database.SqlQuery<TransferPackageStudentList_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/TransferPackage/AddPackage")]
        public HttpResponseMessage AddUpdateTransferPackage()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                TransferStudentPackage_VM transferStudentPackage_VM = new TransferStudentPackage_VM();
                transferStudentPackage_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                transferStudentPackage_VM.TransferType = Convert.ToInt32(HttpRequest.Params["TransferType"]);
                if (transferStudentPackage_VM.TransferType == 1)
                {
                    transferStudentPackage_VM.TransferType = Convert.ToInt32(HttpRequest.Params["TransferType"]);
                    transferStudentPackage_VM.TransferStudent = Convert.ToInt64(HttpRequest.Params["TransferStudent"]);
                }


                transferStudentPackage_VM.TransferReason = HttpRequest.Params["TransferReason"].Trim();
                transferStudentPackage_VM.TransferStartDate = HttpRequest.Params["TransferStartDate"].Trim();
                transferStudentPackage_VM.PlanId = 1; // Convert.ToInt64(HttpRequest.Params["PlanId"]);
                transferStudentPackage_VM.PlanBookingId = Convert.ToInt64(HttpRequest.Params["PlanBookingId"]);


                if (transferStudentPackage_VM.TransferType == 2)
                {
                    transferStudentPackage_VM.TransferCity = HttpRequest.Params["TransferCity"].Trim();
                    transferStudentPackage_VM.TransferState = HttpRequest.Params["TransferState"].Trim();
                    transferStudentPackage_VM.TransferBusinessUserLoginId = Convert.ToInt64(HttpRequest.Params["TransferStudent"]);
                }

                else if (transferStudentPackage_VM.TransferType == 1)
                {
                    transferStudentPackage_VM.TransferCity = string.Empty;
                    transferStudentPackage_VM.TransferState = string.Empty;

                }

                // Validate infromation passed
                Error_VM error_VM = transferStudentPackage_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                //var _transferPackageDetails = transferPackageService.GetAllPendingOrAcceptedStudentTransferPackageRequestList(_LoginID_Exact);
                //var _transferPackageDetails = transferPackageService.GetAllPendingStudentTransferPackageRequestList(_LoginID_Exact);

                //if (_transferPackageDetails.Count() > 0)
                //{
                //    apiResponse.status = -1;
                //    apiResponse.message = Resources.VisitorPanel.TransferPackageAllreadyRequestErrorMesssgae;
                //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                //}

                SqlParameter[] queryParams_VM = new SqlParameter[]
                     {
                    new SqlParameter("id",transferStudentPackage_VM.Id),
                    new SqlParameter("businessOwnerLoginId",transferStudentPackage_VM.TransferBusinessUserLoginId),
                    new SqlParameter("transferFromUserloginId",_LoginID_Exact),
                    new SqlParameter("transferToUserLoginId",transferStudentPackage_VM.TransferStudent),
                    new SqlParameter("transferDate",transferStudentPackage_VM.TransferStartDate),
                    new SqlParameter("transferReason",transferStudentPackage_VM.TransferReason),
                    new SqlParameter("transferType",transferStudentPackage_VM.TransferType),
                    new SqlParameter("transferCity",transferStudentPackage_VM.TransferCity),
                    new SqlParameter("transferState",transferStudentPackage_VM.TransferState),
                    new SqlParameter("notificationMessage","1"),
                    new SqlParameter("packageId",transferStudentPackage_VM.PlanId),
                    new SqlParameter("rejectionReason","0"),
                    new SqlParameter("transferStatus","0"),
                    new SqlParameter("mode","1"),
                    new SqlParameter("planBookingId",transferStudentPackage_VM.PlanBookingId)
                     };

                var resps = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTransferPackage @id,@businessOwnerLoginId,@transferFromUserloginId,@transferToUserLoginId ,@transferDate,@transferReason,@transferType,@transferCity,@transferState ,@notificationMessage ,@packageId,@transferStatus,@rejectionReason,@mode,@planBookingId", queryParams_VM).FirstOrDefault();


                if (resps.ret <= 0)
                {
                    apiResponse.status = resps.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                apiResponse.status = resps.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
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
        /// Get All PlanBookings List linked with the BusinessOnwer
        /// </summary>
        /// <returns>List of All-Business-PlanBookings</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/TransferPackage/GetAllPlanBookings")]
        public HttpResponseMessage GetAllPlanBookingsListByBusiness()
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


                List<TransferPackagePlanBooking_VM> resp = new List<TransferPackagePlanBooking_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "2"),

                            };

                resp = db.Database.SqlQuery<TransferPackagePlanBooking_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

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
        /// Get All Business
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/TransferPackage/GetAllFollowerBusiness")]
        public HttpResponseMessage GetAllPlanBusinessListByBusiness()
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


                List<TransferPackageBusiness_VM> resp = new List<TransferPackageBusiness_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "3"),

                            };

                resp = db.Database.SqlQuery<TransferPackageBusiness_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

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
        /// Get All Transfer-Package List with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/TransferPackage/GetAllPackageByPagination")]

        public HttpResponseMessage GetAllPackageDataTablePagination()
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

                TransferPackage_Pagination_SQL_Params_VM _Params_VM = new TransferPackage_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = transferPackageService.GetAllTransferPackageListByBusinessOwner_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Package View Detail  For the BusinessOnwer
        /// </summary>
        /// <returns> Package View Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/TransferPackage/GetPackageDetailById")]
        public HttpResponseMessage GetAllPackageDetailByBusiness(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }


                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("searchid", ""),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "4"),

                            };

                var packageViewDetail_VM = db.Database.SqlQuery<PackageViewDetail_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = packageViewDetail_VM;

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
        /// To Update TransferStatus for Business-Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/TransferPackage/UpdatePackageStatus")]
        public HttpResponseMessage UpdateTransferPackage()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                PackageStatus_VM packageStatus_VM = new PackageStatus_VM();
                packageStatus_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                packageStatus_VM.TransferStatus = Convert.ToInt32(HttpRequest.Params["TransferStatus"]);
                packageStatus_VM.RejectionReason = string.Empty;
                if (packageStatus_VM.TransferStatus == 2)
                {
                    packageStatus_VM.RejectionReason = HttpRequest.Params["RejectionReason"].Trim();

                }

                // Validate infromation passed
                Error_VM error_VM = packageStatus_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _transferPackageDetail = transferPackageService.GetTransferPackageDetail(packageStatus_VM.Id);
                
                // return if Transfer-Package not found
                if( _transferPackageDetail == null )
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.NotFound;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // If Same business transfer and accepted then transfer the package
                if(_transferPackageDetail.TransferType == 1 && packageStatus_VM.TransferStatus == 1)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // update status
                            SqlParameter[] queryParams_VM = new SqlParameter[]
                            {
                                new SqlParameter("id",packageStatus_VM.Id),
                                new SqlParameter("businessOwnerLoginId",_BusinessOwnerLoginId),
                                new SqlParameter("transferFromUserloginId","0"),
                                new SqlParameter("transferToUserLoginId",""),
                                new SqlParameter("transferDate",""),
                                new SqlParameter("transferReason",""),
                                new SqlParameter("transferType",""),
                                new SqlParameter("transferCity",""),
                                new SqlParameter("transferState",""),
                                new SqlParameter("notificationMessage","1"),
                                new SqlParameter("packageId","0"),
                                new SqlParameter("transferStatus",packageStatus_VM.TransferStatus),
                                new SqlParameter("rejectionReason",packageStatus_VM.RejectionReason),
                                new SqlParameter("mode", "2"),
                                new SqlParameter("planBookingId", "0")

                                     };

                            var resps = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTransferPackage @id,@businessOwnerLoginId,@transferFromUserloginId,@transferToUserLoginId ,@transferDate,@transferReason,@transferType,@transferCity,@transferState ,@notificationMessage ,@packageId,@transferStatus,@rejectionReason,@mode,@planBookingId", queryParams_VM).FirstOrDefault();

                            if (resps.ret <= 0)
                            {
                                apiResponse.status = resps.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // transfer package 
                            SqlParameter[] queryParamsTransferPackage_VM = new SqlParameter[]
                            {
                                new SqlParameter("id",packageStatus_VM.Id),
                                new SqlParameter("businessOwnerLoginId",_BusinessOwnerLoginId),
                                new SqlParameter("transferFromUserloginId","0"),
                                new SqlParameter("transferToUserLoginId",""),
                                new SqlParameter("transferDate",""),
                                new SqlParameter("transferReason",""),
                                new SqlParameter("transferType",""),
                                new SqlParameter("transferCity",""),
                                new SqlParameter("transferState",""),
                                new SqlParameter("notificationMessage","0"),
                                new SqlParameter("packageId","0"),
                                new SqlParameter("transferStatus","0"),
                                new SqlParameter("rejectionReason",""),
                                new SqlParameter("mode", "3"),
                                new SqlParameter("planBookingId", "0")

                                     }; 

                            var respTransferPackage = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTransferPackage @id,@businessOwnerLoginId,@transferFromUserloginId,@transferToUserLoginId ,@transferDate,@transferReason,@transferType,@transferCity,@transferState ,@notificationMessage ,@packageId,@transferStatus,@rejectionReason,@mode,@planBookingId", queryParamsTransferPackage_VM).FirstOrDefault();

                            if (respTransferPackage.ret <= 0)
                            {
                                apiResponse.status = resps.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // Link Student with Business
                            BusinessStudentService businessStudentService = new BusinessStudentService(db);
                            var respBusinessStudentLinking = businessStudentService.AddStudentLinkWithBusinessOwner(_transferPackageDetail.BusinessOwnerLoginId, _transferPackageDetail.TransferToUserLoginId);


                            db.SaveChanges(); // Save changes to the database

                            transaction.Commit(); // Commit the transaction if everything is successful

                            apiResponse.status = 1;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                            apiResponse.data = new { };
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions and perform error handling or logging
                            transaction.Rollback(); // Roll back the transaction
                            apiResponse.status = -100;
                            apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                    }
                }
                else
                {

                    SqlParameter[] queryParams_VM = new SqlParameter[]
                     {
                        new SqlParameter("id",packageStatus_VM.Id),
                        new SqlParameter("businessOwnerLoginId",_BusinessOwnerLoginId),
                        new SqlParameter("transferFromUserloginId","0"),
                        new SqlParameter("transferToUserLoginId",""),
                        new SqlParameter("transferDate",""),
                        new SqlParameter("transferReason",""),
                        new SqlParameter("transferType",""),
                        new SqlParameter("transferCity",""),
                        new SqlParameter("transferState",""),
                        new SqlParameter("notificationMessage","1"),
                        new SqlParameter("packageId","0"),
                        new SqlParameter("transferStatus",packageStatus_VM.TransferStatus),
                        new SqlParameter("rejectionReason",packageStatus_VM.RejectionReason),
                        new SqlParameter("mode", "2"),
                        new SqlParameter("planBookingId", "0")

                     };

                    var resps = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateTransferPackage @id,@businessOwnerLoginId,@transferFromUserloginId,@transferToUserLoginId ,@transferDate,@transferReason,@transferType,@transferCity,@transferState ,@notificationMessage ,@packageId,@transferStatus,@rejectionReason,@mode,@planBookingId", queryParams_VM).FirstOrDefault();

                    if (resps.ret <= 0)
                    {
                        apiResponse.status = resps.ret;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }

                    apiResponse.status = 1;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                    apiResponse.data = new { };
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// To Notification  Transfer for Business-Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/TransferPackage/AddNotification/{id}")]
        public HttpResponseMessage NotificationTransferPackage(long id)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                NotificationTransferViewModel notificationTransferViewModel = new NotificationTransferViewModel();
                notificationTransferViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                notificationTransferViewModel.TransferSenderId = HttpRequest.Params["TransferSenderId"];
                notificationTransferViewModel.MessageNotification = HttpRequest.Params["MessageNotification"];


                // Validate infromation passed
                Error_VM error_VM = notificationTransferViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                SqlParameter[] queryParams_VM = new SqlParameter[]
                     {
                    new SqlParameter("id",notificationTransferViewModel.Id),
                    new SqlParameter("transferrequestedId",id),
                    new SqlParameter("transferSenderId",notificationTransferViewModel.TransferSenderId),
                    new SqlParameter("notificationMessage",notificationTransferViewModel.MessageNotification),
                    new SqlParameter("mode", "1")

                     };

                var resps = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateNotificationTransfer @id,@transferrequestedId,@transferSenderId,@notificationMessage,@mode", queryParams_VM).FirstOrDefault();


                if (resps.ret <= 0)
                {
                    apiResponse.status = resps.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
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
        /// Get All Notification Detail  User Student with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/TransferPackage/GetAllPackageStudentNotificationDetailByPagination")]

        public HttpResponseMessage GetAllPackageStudentDataTablePagination()
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

                NotificationDetail_Pagination_SQL_Params_VM _Params_VM = new NotificationDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = transferPackageService.GetAllTransferPackageListByStudentUser_Pagination(HttpRequestParams, _Params_VM);

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
        /// to get user or student  basic details  by  master id
        /// </summary>
        /// <param name="searchid"></param>
        /// <returns></returns>
        [Route("api/TransferPackage/GetUserdetailsByMasterId")]
        public HttpResponseMessage GetUserdetailsByMasterId(string searchid)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            if (searchid == null)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.NoDataFound;
                return Request.CreateResponse(apiResponse);
            }

            try
            {
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("searchid", searchid),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "9"),

                            };

                var lst = db.Database.SqlQuery<TransferPackageStudentList_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).FirstOrDefault();



                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    DetailsList = lst,
                };

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
        /// to get business   basic details  by  master id
        /// </summary>
        /// <param name="searchid"></param>
        /// <returns></returns>
        [Route("api/TransferPackage/GetBusinessdetailsByMasterId")]
        public HttpResponseMessage GetBusinessdetailsByMasterId(string searchid)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            if (searchid == null)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.NoDataFound;
                return Request.CreateResponse(apiResponse);
            }

            try
            {
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("searchid", searchid),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "10"),

                            };

                var lst = db.Database.SqlQuery<TransferPackageStudentList_VM>("exec sp_ManageTransferPackage @id,@searchid,@businessOwnerId,@userLoginId,@mode", queryParams).FirstOrDefault();


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    DetailsList = lst,
                };

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