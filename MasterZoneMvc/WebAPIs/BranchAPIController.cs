using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
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
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.PageTemplateViewModels;
using Org.BouncyCastle.Ocsp;
using Microsoft.Extensions.Logging;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class BranchAPIController : ApiController
    {


        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        private BranchService branchService;
        private BusinessOwnerService businessOwnerService;


        public BranchAPIController ()
        {
            db = new MasterZoneDbContext ();
            branchService = new BranchService (db);
            storedProcedureRepository = new StoredProcedureRepository(db);
            businessOwnerService = new BusinessOwnerService(db);
        }
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// To Add/Update Branch Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
       
        [Route("api/Business/AddUpdateBranches")]
        public HttpResponseMessage AddUpdateBranches(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                var HttpRequest = HttpContext.Current.Request;
                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
                var _RegMode = 0;

                UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                long respId = 0; // Nullable long

                RegisterBusinessAdminViewModel registerBusinessAdminViewModel = new RegisterBusinessAdminViewModel();
                registerBusinessAdminViewModel.BusinessCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessCategoryId"]);
                registerBusinessAdminViewModel.BusinessSubCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessSubCategoryId"]);
                registerBusinessAdminViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                registerBusinessAdminViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                registerBusinessAdminViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                registerBusinessAdminViewModel.Email = HttpRequest.Params["Email"].Trim();
                registerBusinessAdminViewModel.Password = HttpRequest.Params["Password"].Trim();

                

                BranchesViewModel branchesViewModel = new BranchesViewModel();
                branchesViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                branchesViewModel.BranchBusinessLoginId = respId; // Use the null coalescing operator to provide a default value if respId is null
                branchesViewModel.Name = HttpRequest.Params["Name"];
                branchesViewModel.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                branchesViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                Error_VM error_VM = branchesViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (id == 0)
                        {
                            _RegMode = 1;

                            var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                            {
                                BusinessName = branchesViewModel.Name,
                                Email = registerBusinessAdminViewModel.Email,
                                Password = EDClass.Encrypt(registerBusinessAdminViewModel.Password),
                                PhoneNumber = registerBusinessAdminViewModel.PhoneNumber,
                                PhoneNumberCountryCode = "+91",
                                FirstName = registerBusinessAdminViewModel.FirstName,
                                LastName = registerBusinessAdminViewModel.LastName,
                                BusinessCategoryId = registerBusinessAdminViewModel.BusinessCategoryId,
                                BusinessSubCategoryId = registerBusinessAdminViewModel.BusinessSubCategoryId,
                                Mode = _RegMode
                            });
                            respId = resp.Id; // Nullable type
                            if (resp.ret <= 0)
                            {
                                apiResponse.status = resp.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }
                        }
                        else if (id > 0)
                        {
                            _RegMode = 5;
                            var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                            {
                                Id = id,
                                BusinessName = branchesViewModel.Name,
                                Email = registerBusinessAdminViewModel.Email,
                                PhoneNumber = registerBusinessAdminViewModel.PhoneNumber,
                                PhoneNumberCountryCode = "+91",
                                FirstName = registerBusinessAdminViewModel.FirstName,
                                LastName = registerBusinessAdminViewModel.LastName,
                                Mode = _RegMode
                            });

                            respId = resp.Id; // Nullable type
                            if (resp.ret <= 0)
                            {
                                apiResponse.status = resp.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }
                        }

                        var resp1 = storedProcedureRepository.InsertUpdateBusinessBranches_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessBranch_Param_VM()
                        {
                            BusinessOwnerLoginId = _BusinessOwnerLoginId,
                            BranchBusinessLoginId = respId,
                            Name = branchesViewModel.Name,
                            Status = branchesViewModel.Status,
                            SubmittedByLoginId = _LoginID_Exact,
                            Mode = branchesViewModel.Mode
                        });

                        if (resp1.ret <= 0)
                        {
                            apiResponse.status = resp1.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp1.resourceFileName, resp1.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        db.SaveChanges(); // Save changes to the database

                        transaction.Commit(); // Commit the transaction if everything is successful

                        apiResponse.status = 1;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp1.resourceFileName, resp1.resourceKey);
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
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = "Internal Server Error!";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// To Get Branches View Detail For Pagination 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBranchesDetailByPagination")]
        public HttpResponseMessage GetAllBusinessBranchesDataTablePagination()
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

                BranchDetail_Pagination_SQL_Params_VM _Params_VM = new BranchDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = branchService.GetBranchList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Branch Detail By Id
        /// </summary>
        /// <param name="branchbusinessLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetBranchDetailBybusinessLoginId")]
        public HttpResponseMessage GetBranchDetailByUserLoginId(long branchbusinessLoginId)
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


                BranchDetail_VM resp = new BranchDetail_VM();

                resp = branchService.GetBranchDetailByBranchbusinessLoginId(branchbusinessLoginId);

                resp.Password = EDClass.Decrypt(resp.Password);
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// To Change  Branch Status  by Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/ChangeStatus")]
        public HttpResponseMessage ChangeStatusbranchbusiness(long Id)
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

                var resp = branchService.ChangeStatusBranchDetail(Id);

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
        /// To Delete the Business and branch detail by branchbusinessLoginId
        /// </summary>
        /// <param name="branchBusinessLoginId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/DeleteBranchDetailById")]
        public HttpResponseMessage DeleteBranchDetailById(long branchBusinessLoginId)
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

              
                       
                        // Delete By Id
                        var resp = branchService.DeleteBranchDetail(branchBusinessLoginId);


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
        /// To Get the View Detail of Staff by branchbusinessLoginId 
        /// </summary>
        /// <param name="branchBusinessLoginId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBranchStaffDetailByPagination")]
        public HttpResponseMessage GetAllBusinessStaffDataTablePagination(long branchBusinessLoginId)
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

                BranchStaffDetail_Pagination_SQL_Params_VM _Params_VM = new BranchStaffDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = branchBusinessLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = branchService.GetBranchStaffList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To get Event Detail For BusinessOwner
        /// </summary>
        /// <param name="branchBusinessLoginId"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBranchEventDetailByPagination")]
        public HttpResponseMessage GetAllBusinessEventDataTablePagination(long branchBusinessLoginId)
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

                BranchEventDetail_Pagination_SQL_Params_VM _Params_VM = new BranchEventDetail_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = branchBusinessLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = branchService.GetBranchEventList_Pagination(HttpRequestParams, _Params_VM);

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


        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBranchStudentDetailByPagination")]
        public HttpResponseMessage GetAllBusinessStudentDataTablePagination(long branchBusinessLoginId)
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

                StudentList_Pagination_SQL_Params_VM _Params_VM = new StudentList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = branchBusinessLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = branchService.GetBranchStudentList_Pagination(HttpRequestParams, _Params_VM);


                

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
        /// Get All Payment/Transaction with Pagination For the Business 
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Business/GetAllBranchPaymentsForBusinessByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllBusinessPaymentDataTablePagination(long branchBusinessLoginId)
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

                BranchPaymentsList_Pagination_SQL_Params_VM _Params_VM = new BranchPaymentsList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = branchBusinessLoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = branchService.GetBranchPaymentList_Pagination(HttpRequestParams, _Params_VM);

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
        /// To Get Business Branches Detail List (classic dance)
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBranchesDetailForVisitorPanel")]
        public HttpResponseMessage GetBranchesDetailForVisitorPanel(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                //long _LoginID_Exact = validateResponse.UserLoginId;
                //long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;


               

               List<BusinessBranchesDetailForVisitorPanel> resp = branchService.GetAllBranchesDetailList(businessOwnerLoginId);
               BusinessBranchesDetailForVisitorPanel businessLogo = branchService.GetAllBranchesDetailListsforBusinessName(businessOwnerLoginId);

                
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BranchDetails = resp,
                    BranchListforBusinessLogo = businessLogo,
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
        /// To Get All Branches Detail By Location 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Business/GetBranchesDetailLocationForVisitorPanel")]
        public HttpResponseMessage GetBranchesDetailLocationForVisitorPanel( string state, string city, long businessownerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                

                List<BusinessBranchesDetailForVisitorPanel> resp = branchService.GetAllLocationBranchesDetailList(state,city,businessownerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
    }

}