using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class RegisterAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private StudentService studentService;
        private BusinessOwnerService businessOwnerService;

        public RegisterAPIController()
        {
            db = new MasterZoneDbContext();
        }
        [HttpPost]
        [Route("api/Register/Business")]
        public HttpResponseMessage BusinessRegister()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;

                RegisterBusinessAdminViewModel registerBusinessAdminViewModel = new RegisterBusinessAdminViewModel();
                registerBusinessAdminViewModel.BusinessCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessCategoryId"]);
                registerBusinessAdminViewModel.BusinessSubCategoryId = Convert.ToInt64(HttpRequest.Params["BusinessSubCategoryId"]);
                registerBusinessAdminViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                registerBusinessAdminViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                registerBusinessAdminViewModel.Email = HttpRequest.Params["Email"].Trim();
                registerBusinessAdminViewModel.Password = HttpRequest.Params["Password"].Trim();
                registerBusinessAdminViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                registerBusinessAdminViewModel.BusinessName = HttpRequest.Params["BusinessName"].Trim();

                // Validate infromation passed
                Error_VM error_VM = registerBusinessAdminViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                businessOwnerService = new BusinessOwnerService(db);
                var resp = businessOwnerService.InsertUpdateBusinessOwner(new ViewModels.StoredProcedureParams.SP_InsertUpdateBusinessOwner_Params_VM()
                {
                    BusinessName = registerBusinessAdminViewModel.BusinessName,
                    Email = registerBusinessAdminViewModel.Email,
                    Password = EDClass.Encrypt(registerBusinessAdminViewModel.Password),
                    PhoneNumber = registerBusinessAdminViewModel.PhoneNumber,
                    PhoneNumberCountryCode = "+91",
                    FirstName = registerBusinessAdminViewModel.FirstName,
                    LastName = registerBusinessAdminViewModel.LastName,
                    BusinessCategoryId = registerBusinessAdminViewModel.BusinessCategoryId,
                    BusinessSubCategoryId = registerBusinessAdminViewModel.BusinessSubCategoryId,
                    Mode = 1
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                EmailSender emailSender = new EmailSender();
                emailSender.Send(registerBusinessAdminViewModel.FirstName + " " + registerBusinessAdminViewModel.LastName, "Registration successful", registerBusinessAdminViewModel.Email, "You have been successfully registered as business owner with Masterzone. Your MasterID is: " + resp.MasterId, "");

                TokenGenerator tokenGenerator = new TokenGenerator();
                var _JWT_User_Token = tokenGenerator.Create_JWT(resp.Id, "BusinessAdmin", 0);

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { token = _JWT_User_Token };
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
        [Route("api/Register/Student")]
        public HttpResponseMessage StudentRegister()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                StudentRegisterViewModel studentregisterViewModel = new StudentRegisterViewModel();

                studentregisterViewModel.Email = HttpRequest.Params["Email"].Trim();
                studentregisterViewModel.Password = HttpRequest.Params["Password"].Trim();
                studentregisterViewModel.FirstName = HttpRequest.Params["FirstName"].Trim();
                studentregisterViewModel.LastName = HttpRequest.Params["LastName"].Trim();
                studentregisterViewModel.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                studentregisterViewModel.Gender = Convert.ToInt32( HttpRequest.Params["PhoneNumber"].Trim());
                studentregisterViewModel.PhoneNumber_CountryCode = HttpRequest.Params["PhoneNumberCountryCode"].Trim();


                // Validate infromation passed
                Error_VM error_VM = studentregisterViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if(String.IsNullOrEmpty(studentregisterViewModel.PhoneNumber_CountryCode))
                {
                    studentregisterViewModel.PhoneNumber_CountryCode = "+91";
                }

                ApiResponseViewModel<UserLoginViewModel> apiResponseViewModel = new ApiResponseViewModel<UserLoginViewModel>();

                studentService = new StudentService(db);
                var resp = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                {
                    Email = studentregisterViewModel.Email,
                    Password = EDClass.Encrypt(studentregisterViewModel.Password),
                    PhoneNumber = studentregisterViewModel.PhoneNumber,
                    PhoneNumberCountryCode = "+91",
                    RoleId = 3,
                    FirstName = studentregisterViewModel.FirstName,
                    LastName = studentregisterViewModel.LastName,
                    Mode = 1
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                EmailSender emailSender = new EmailSender();
                emailSender.Send(studentregisterViewModel.FirstName + " " + studentregisterViewModel.LastName, "Registration successful", studentregisterViewModel.Email, "You have been successfully registered with Masterzone. Your MasterID is: " + resp.MasterId, "");

                TokenGenerator tokenGenerator = new TokenGenerator();
                var _JWT_User_Token = tokenGenerator.Create_JWT(resp.Id, "Student", 0);

                apiResponse.status = 1;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { token = _JWT_User_Token };
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