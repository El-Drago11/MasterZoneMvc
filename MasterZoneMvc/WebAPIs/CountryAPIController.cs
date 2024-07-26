using MasterZoneMvc.Common.ValidationHelpers;
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
using System.Web.Http;
using System.Windows.Documents;
namespace MasterZoneMvc.WebAPIs
{
    public class CountryAPIController : ApiController
    {
        private MasterZoneDbContext db;

        public CountryAPIController()
        {
            db = new MasterZoneDbContext();
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
        /// --Get All Countries-List-- 
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,Student")]
        [Route("api/Country/GetAllCountriesListData")]
        [HttpGet]
        public HttpResponseMessage GetAllCountriesListData()
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

                List<CountryViewModel> lstCountries = new List<CountryViewModel>();

                //--Get All Countries-List
                SqlParameter[] queryParams_Country = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("mode", "1")
                        };
                lstCountries = db.Database.SqlQuery<CountryViewModel>("exec sp_ManageCountry @id,@mode", queryParams_Country).ToList();

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = lstCountries;

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
        /// Get All States-List by Country-ID--
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,Student")]
        [Route("api/Country/GetAllStatesListByCountry")]
        [HttpGet]
        public HttpResponseMessage GetAllStatesListByCountry(Int64 countryId)
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


                List<StateViewModel> lstStates = new List<StateViewModel>();

                //--Get All States-List By Country
                SqlParameter[] queryParams_State = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("countryId", countryId),
                        new SqlParameter("mode", "1")
                        };
                lstStates = db.Database.SqlQuery<StateViewModel>("exec sp_ManageState @id,@countryId,@mode", queryParams_State).ToList();
                //--Create response
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = lstStates;

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
        /// Get All Cities-List by State-ID
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,Student")]
        [Route("api/Country/GetAllCitiesListByState")]
        [HttpGet]
        public HttpResponseMessage GetAllCitiesListByState(Int64 stateId)
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

                List<CityViewModel> lstCities = new List<CityViewModel>();

                //--Get All Cities-List By State
                SqlParameter[] queryParams_City = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("stateId", stateId),
                        new SqlParameter("mode", "1")
                        };
                lstCities = db.Database.SqlQuery<CityViewModel>("exec sp_ManageCity @id,@stateId,@mode", queryParams_City).ToList();

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = lstCities;

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
        /// Get All States-List
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,Student")]
        [Route("api/Country/GetAllStatesList")]
        [HttpGet]
        public HttpResponseMessage GetAllStatesList()
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

                List<StateViewModel> lstStates = new List<StateViewModel>();

                //--Get All States-List
                SqlParameter[] queryParams_State = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("countryId", "0"),
                        new SqlParameter("mode", "3")
                        };
                lstStates = db.Database.SqlQuery<StateViewModel>("exec sp_ManageState @id,@countryId,@mode", queryParams_State).ToList();

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = lstStates;

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
        /// Get All Cities-List
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,Student")]
        [Route("api/Country/GetAllCitiesList")]
        [HttpGet]
        public HttpResponseMessage GetAllCitiesList()
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

                List<CityViewModel> lstCities = new List<CityViewModel>();

                //--Get All Cities-List
                SqlParameter[] queryParams_City = new SqlParameter[] {
                        new SqlParameter("id", "0"),
                        new SqlParameter("stateId", "0"),
                        new SqlParameter("mode", "3")
                        };
                lstCities = db.Database.SqlQuery<CityViewModel>("exec sp_ManageCity @id,@stateId,@mode", queryParams_City).ToList();

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = lstCities;

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