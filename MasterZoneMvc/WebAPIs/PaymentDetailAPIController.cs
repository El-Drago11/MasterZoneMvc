using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.ViewModels;
using Newtonsoft.Json;
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
    public class PaymentDetailAPIController : ApiController
    {
        private MasterZoneDbContext db;
        public PaymentDetailAPIController()
        {
            db = new MasterZoneDbContext();
        }
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/AddUpdateCardDetail")]
        public HttpResponseMessage AddUpdatePayment()
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

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Card_VM card_VM = new Card_VM();
                card_VM.CardNumber = HttpRequest.Params["CardNumber"].Trim();
                card_VM.CardName = HttpRequest.Params["CardName"].Trim();
                card_VM.ExpMonth = Convert.ToInt32(HttpRequest.Params["ExpMonth"]);
                card_VM.ExpYear = Convert.ToInt32(HttpRequest.Params["ExpYear"]);

                // Validate infromation passed
                Error_VM error_VM = card_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringCardObj = JsonConvert.SerializeObject(card_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "CCAvenue"),
                            new SqlParameter("paymentModeDetail",stringCardObj),
                            new SqlParameter("mode","1"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();

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

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/GetById/{id}")]
        public HttpResponseMessage GetPaymentDetail(Int64 Id)
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


                PaymentDetails resp = new PaymentDetails();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", Id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", ""),
                            new SqlParameter("paymentModeDetail",""),
                            new SqlParameter("mode","1"),
                            };

                resp = db.Database.SqlQuery<PaymentDetails>("exec sp_ManagePaymentDetail @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();

                // List = JsonConvert.DeserializeObject<ApiResponseViewModel<Card_VM>>(resp.PaymentModeDetail);
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

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/GetAllPaymentDetails")]
        public HttpResponseMessage GetAllPaymentDetailByUser()
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

                List<PaymentDetails> lst = new List<PaymentDetails>();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", ""),
                            new SqlParameter("paymentModeDetail",""),
                            new SqlParameter("mode","2"),
                            };

                lst = db.Database.SqlQuery<PaymentDetails>("exec sp_ManagePaymentDetail @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).ToList();

                List<UPI_VM> upiList = new List<UPI_VM>();

                List<Paytm_VM> paytmList = new List<Paytm_VM>();
                List<Card_VM> cardsList = new List<Card_VM>();
                foreach (var paymentDetail in lst)
                {
                    if (paymentDetail.PaymentModeType == "UPI")
                    {
                        UPI_VM upi = JsonConvert.DeserializeObject<UPI_VM>(paymentDetail.PaymentModeDetail);
                        upi.PaymentDetailId = paymentDetail.Id;
                        upiList.Add(upi);
                    }
                    if (paymentDetail.PaymentModeType == "Paytm")
                    {
                        Paytm_VM paytm = JsonConvert.DeserializeObject<Paytm_VM>(paymentDetail.PaymentModeDetail);
                        paytm.PaymentDetailId = paymentDetail.Id;
                        paytmList.Add(paytm);
                    }
                    if (paymentDetail.PaymentModeType == "CCAvenue")
                    {
                        Card_VM card = JsonConvert.DeserializeObject<Card_VM>(paymentDetail.PaymentModeDetail);
                        card.PaymentDetailId = paymentDetail.Id;
                        cardsList.Add(card);

                    }
                }
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    UPIList = upiList,
                    CardsList = cardsList,
                    PaytmList = paytmList,
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

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetails/AddUpdateUPIDetail/")]
        public HttpResponseMessage AddUpdateUPIDetail()
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


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                UPI_VM uPI_VM = new UPI_VM();
                uPI_VM.UPIId = HttpRequest.Params["UPIId"].Trim();


                // Validate infromation passed
                Error_VM error_VM = uPI_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringUPIObj = JsonConvert.SerializeObject(uPI_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "UPI"),
                            new SqlParameter("paymentModeDetail",stringUPIObj),
                            new SqlParameter("mode","2"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                //return resp;

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

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/AddUpdatePaytmDetail/")]
        public HttpResponseMessage AddUpdatePaytmDetail()
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


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Paytm_VM paytm_VM = new Paytm_VM();
                paytm_VM.PaytmId = HttpRequest.Params["PaytmId"].Trim();


                // Validate infromation passed
                Error_VM error_VM = paytm_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringPaytmObj = JsonConvert.SerializeObject(paytm_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "Paytm"),
                            new SqlParameter("paymentModeDetail",stringPaytmObj),
                            new SqlParameter("mode","3"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                //return resp;


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

        //Not found - Not in use
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/UPI/GetById")]
        public HttpResponseMessage GetUPI(Int64 Id)
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

                UPI_VM uPI_VM = new UPI_VM();
                PaymentDetailViewModel paymentDetails = new PaymentDetailViewModel();
                // Get Business Profile Data for Image Names
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", Id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "UPI"),
                            new SqlParameter("paymentModeDetail",""),
                            new SqlParameter("mode","3"),
                            };

                paymentDetails = db.Database.SqlQuery<PaymentDetailViewModel>("exec sp_ManagePaymentDetail @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();

                uPI_VM = JsonConvert.DeserializeObject<UPI_VM>(paymentDetails.PaymentModeDetail);

                // List = JsonConvert.DeserializeObject<ApiResponseViewModel<Card_VM>>(resp.PaymentModeDetail);
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = uPI_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        //Not found - Not in use
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Paytm/GetById")]
        public HttpResponseMessage GetPaytmId(Int64 Id)
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

                Paytm_VM paytm_VM = new Paytm_VM();
                PaymentDetailViewModel paymentDetails = new PaymentDetailViewModel();

                // Get Business Profile Data for Image Names
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", Id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "Paytm"),
                            new SqlParameter("paymentModeDetail",""),
                            new SqlParameter("mode","4"),
                            };

                paymentDetails = db.Database.SqlQuery<PaymentDetailViewModel>("exec sp_ManagePaymentDetail @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                paytm_VM = JsonConvert.DeserializeObject<Paytm_VM>(paymentDetails.PaymentModeDetail);
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = paymentDetails;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        //Not found - Not in use
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/EditPaytmDetail/")]
        public HttpResponseMessage EditPaytm()
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


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Paytm_VM paytm_VM = new Paytm_VM();
                paytm_VM.PaytmId = HttpRequest.Params["PaytmId"].Trim();


                // Validate infromation passed
                Error_VM error_VM = paytm_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringPaytmObj = JsonConvert.SerializeObject(paytm_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "Paytm"),
                            new SqlParameter("paymentModeDetail",stringPaytmObj),
                            new SqlParameter("mode","4"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                //return resp;



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

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/CCAvenueGetById/{id}")]
        public HttpResponseMessage GetCardById(Int64 id)
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

                Card_VM card_VM = new Card_VM();
                PaymentDetailViewModel paymentDetails = new PaymentDetailViewModel();

                // Get Business Profile Data for Image Names
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "CCAvenue"),
                            new SqlParameter("paymentModeDetail",""),
                            new SqlParameter("mode","5"),
                            };

                paymentDetails = db.Database.SqlQuery<PaymentDetailViewModel>("exec sp_ManagePaymentDetail @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                if (paymentDetails != null)
                {
                    card_VM = JsonConvert.DeserializeObject<Card_VM>(paymentDetails.PaymentModeDetail);
                    card_VM.PaymentDetailId = paymentDetails.Id;
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    CardsList = card_VM,


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

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PAymentDetail/UpdateCardPaymentDetail/")]
        public HttpResponseMessage UpdateCard()
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


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Card_VM card_VM = new Card_VM();
                card_VM.PaymentDetailId = Convert.ToInt64(HttpRequest.Params["Id"]);
                card_VM.CardNumber = HttpRequest.Params["CardNumber"].Trim();
                card_VM.CardName = HttpRequest.Params["CardName"].Trim();
                card_VM.ExpMonth = Convert.ToInt32(HttpRequest.Params["ExpMonth"]);
                card_VM.ExpYear = Convert.ToInt32(HttpRequest.Params["ExpYear"]);

                // Validate infromation passed
                Error_VM error_VM = card_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringCardObj = JsonConvert.SerializeObject(card_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", card_VM.PaymentDetailId),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "CCAvenue"),
                            new SqlParameter("paymentModeDetail",stringCardObj),
                            new SqlParameter("mode","5"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();

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

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/UpdateUPIById/")]
        public HttpResponseMessage UpdateUPI()
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


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                UPI_VM uPI_VM = new UPI_VM();
                uPI_VM.PaymentDetailId = Convert.ToInt64(HttpRequest.Params["Id"]);
                uPI_VM.UPIId = HttpRequest.Params["UPIId"].Trim();


                // Validate infromation passed
                Error_VM error_VM = uPI_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringUPIObj = JsonConvert.SerializeObject(uPI_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", uPI_VM.PaymentDetailId),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "UPI"),
                            new SqlParameter("paymentModeDetail",stringUPIObj),
                            new SqlParameter("mode","6"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                //return resp;

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

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/PaymentDetail/UpdatePaytm/")]
        public HttpResponseMessage UpdatePaytm()
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


                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                Paytm_VM paytm_VM = new Paytm_VM();
                paytm_VM.PaymentDetailId = Convert.ToInt64(HttpRequest.Params["Id"]);
                paytm_VM.PaytmId = HttpRequest.Params["PaytmId"].Trim();


                // Validate infromation passed
                Error_VM error_VM = paytm_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                //StringContent content = new StringContent(JsonConvert.SerializeObject(card_VM), Encoding.UTF8, "application/json");
                string stringPaytmObj = JsonConvert.SerializeObject(paytm_VM);

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",paytm_VM.PaymentDetailId),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", "Paytm"),
                            new SqlParameter("paymentModeDetail",stringPaytmObj),
                            new SqlParameter("mode","7"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();
                //return resp;

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
        [Route("api/PaymentDetail/DeleteById/{id}")]
        public HttpResponseMessage DeletePaymentById(int id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",id),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("paymentModeType ", ""),
                            new SqlParameter("paymentModeDetail",""),
                            new SqlParameter("mode","8"),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdatePaymentDetails @id,@userLoginId,@paymentModeType,@paymentModeDetail,@mode", queryParams).FirstOrDefault();

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
    }
}