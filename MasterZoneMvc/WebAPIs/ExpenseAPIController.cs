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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using static MasterZoneMvc.ViewModels.ExpenseViewModel;

namespace MasterZoneMvc.WebAPIs
{
    public class ExpenseAPIController : ApiController
    {

        private MasterZoneDbContext db;
        private ExpenseService expenseService;
        public ExpenseAPIController()
        {
            db = new MasterZoneDbContext();
            expenseService = new ExpenseService(db);
           
        }
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

       

        // <summary>
        /// To Add Expense Detail By Staff
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Expense/AddUpdate")]
        public HttpResponseMessage AddUpdateExpense()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                ExpenseViewModel expenseViewModel = new ExpenseViewModel();
                expenseViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);             
                expenseViewModel.ExpenseAmount = Convert.ToDecimal(HttpRequest.Params["ExpenseAmount"]);
                expenseViewModel.ExpenseDescription = HttpRequest.Params["ExpenseDescription"].Trim();
                expenseViewModel.ExpenseDate = HttpRequest.Params["ExpenseDate"].Trim();
                expenseViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                if (validateResponse.UserRoleName == "Staff")
                {
                    expenseViewModel.UserLoginId = _LoginId;
                }
                else
                {
                    expenseViewModel.UserLoginId = Convert.ToInt64(HttpRequest.Params["UserLoginId"]);
                }
                
                

                // Validate infromation passed
                Error_VM error_VM = expenseViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
               

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", expenseViewModel.Id),
                            new SqlParameter("userLoginId", expenseViewModel.UserLoginId),
                            new SqlParameter("expenseAmount", expenseViewModel.ExpenseAmount),
                            new SqlParameter("expenseDescription", expenseViewModel.ExpenseDescription),
                            new SqlParameter("expenseDate", expenseViewModel.ExpenseDate),
                            new SqlParameter("remarks",""),
                            new SqlParameter("status","0"),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", expenseViewModel.Mode),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateExpense @id,@userLoginId,@expenseAmount,@expenseDescription,@expenseDate,@remarks,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

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
        /// Get All Expense with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Expense/GetAllByPagination")]

        public HttpResponseMessage GetAllExpenseDataTablePagination()
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

                Expense_Pagination_SQL_Params_VM _Params_VM = new Expense_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = expenseService.GetExpenseList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get SubAdmin Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Expense/ExpenseDetailGetById")]
        public HttpResponseMessage GetExpenseById(long id)
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


                // Get Class By Id

                SqlParameter[] queryParamsGetExpense = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<ExpenseList_VM>("exec sp_ManageExpense @id,@userLoginId,@businessOwnerLoginId,@mode", queryParamsGetExpense).FirstOrDefault();

               
                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }

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
        /// To Delete the Expense Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Expense/ExpenseDeleteById/{id}")]
        public HttpResponseMessage DeleteExpenseById(long id)
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


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", ""),
                            new SqlParameter("expenseAmount","0"),
                            new SqlParameter("expenseDescription",""),
                            new SqlParameter("expenseDate",""),
                            new SqlParameter("remarks",""),
                            new SqlParameter("status","0"),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode",3),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateExpense @id,@userLoginId,@expenseAmount,@expenseDescription,@expenseDate,@remarks,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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

        /// <summary>
        /// Get All Expense with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Expense/GetAllBusinessExpenseByPagination")]

        public HttpResponseMessage GetAllBusinessExpenseDataTablePagination()
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

                BusinessExpense_Pagination_SQL_Params_VM _Params_VM = new BusinessExpense_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = expenseService.GetBusinessExpenseList_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get SubAdmin Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Expense/BusinessExpenseDetailGetById")]
        public HttpResponseMessage GetBusinessExpenseById(long id)
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


                // Get Class By Id

                SqlParameter[] queryParamsGetExpense = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<BusinessExpenseList_VM>("exec sp_ManageExpense @id,@userLoginId,@businessOwnerLoginId,@mode", queryParamsGetExpense).FirstOrDefault();


                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessCategory_ErrorMessage;
                }

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


        // <summary>
        /// To Update Business Expense Detail By Staff
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin")]
        [Route("api/Expense/BusinessUpdateExpense")]
        public HttpResponseMessage BusinessUpdateExpense()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                BusinessExpense_ViewModel businessexpenseViewModel = new BusinessExpense_ViewModel();
                businessexpenseViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessexpenseViewModel.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                businessexpenseViewModel.Remarks = HttpRequest.Params["Remarks"];

             
                // Validate infromation passed
                Error_VM error_VM = businessexpenseViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", businessexpenseViewModel.Id),
                            new SqlParameter("userLoginId", businessexpenseViewModel.UserLoginId),
                            new SqlParameter("expenseAmount", "0"),
                            new SqlParameter("expenseDescription",""),
                            new SqlParameter("expenseDate", ""),
                            new SqlParameter("remarks",businessexpenseViewModel.Remarks),
                            new SqlParameter("status",businessexpenseViewModel.Status),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", 4),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateExpense @id,@userLoginId,@expenseAmount,@expenseDescription,@expenseDate,@remarks,@status,@submittedByLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

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


        
    }
}