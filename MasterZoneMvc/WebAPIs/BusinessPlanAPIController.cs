using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.WebAPIs
{
    public class BusinessPlanAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private PlanService planService;
        private StoredProcedureRepository storedProcedureRepository;


        public BusinessPlanAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            planService = new PlanService(db);
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/GetAll")]
        [HttpGet]
        public HttpResponseMessage GetAll()
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
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("planType", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).ToList();

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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/GetById/{id}")]
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
                    apiResponse.message = "Invalid Id";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("planType", "0"),
                            new SqlParameter("mode", "8")
                            };

                var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).FirstOrDefault();

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessNotFound_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
                }

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
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/AddUpdate")]
        public HttpResponseMessage AddUpdateBusinessPlan()
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
                long _BusinessOwnerLoginID = validateResponse.BusinessAdminLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestBusinessPlan_VM requestBusinessPlan_VM = new RequestBusinessPlan_VM();
                requestBusinessPlan_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestBusinessPlan_VM.BusinessPlanDurationTypeId = Convert.ToInt64(HttpRequest.Params["BusinessPlanDurationTypeId"]);
                requestBusinessPlan_VM.Name = HttpRequest.Params["Name"].Trim();
                requestBusinessPlan_VM.Description = HttpUtility.UrlDecode(HttpRequest.Params["Description"]);
                requestBusinessPlan_VM.Price = Convert.ToDecimal(HttpRequest.Params["Price"]);
                requestBusinessPlan_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestBusinessPlan_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                requestBusinessPlan_VM.PlanTypeTitle = Convert.ToInt32(HttpRequest.Params["PlanTypeTitle"]);
                requestBusinessPlan_VM.PlanType = 1;
                requestBusinessPlan_VM.CompareAtPrice = (!String.IsNullOrEmpty(HttpRequest.Params["CompareAtPrice"])) ? Convert.ToDecimal(HttpRequest.Params["CompareAtPrice"]) : 0;
                requestBusinessPlan_VM.DiscountPercent = (!String.IsNullOrEmpty(HttpRequest.Params["DiscountPercent"])) ? Math.Round(Convert.ToDecimal(HttpRequest.Params["DiscountPercent"])) : 0;

                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _PlanImageFile = files["PlanImage"];
                requestBusinessPlan_VM.PlanImage = _PlanImageFile; // for validation
                string _PlanImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousPlanImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestBusinessPlan_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_PlanImageFile != null)
                    {
                        _PlanImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_PlanImageFile);
                    }
                }

                // if Edit Mode
                if (requestBusinessPlan_VM.Mode == 2)
                {

                    BusinessPlanViewModel businessPlan_VM = new BusinessPlanViewModel();

                    //Get Event By Id
                    businessPlan_VM = planService.GetPlanDataById(requestBusinessPlan_VM.Id,requestBusinessPlan_VM.PlanType);
                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        // set current image value
                        _PlanImageFileNameGenerated = businessPlan_VM.PlanImage;
                    }
                    else
                    {
                        // set previous image value
                        _PreviousPlanImageFileName = businessPlan_VM.PlanImage;
                    }
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestBusinessPlan_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginID),
                            new SqlParameter("businessPlanDurationTypeId", requestBusinessPlan_VM.BusinessPlanDurationTypeId),
                            new SqlParameter("name", requestBusinessPlan_VM.Name),
                            new SqlParameter("description", requestBusinessPlan_VM.Description),
                            new SqlParameter("price", requestBusinessPlan_VM.Price),
                            new SqlParameter("status", requestBusinessPlan_VM.Status),
                            new SqlParameter("compareAtPrice", requestBusinessPlan_VM.CompareAtPrice),
                            new SqlParameter("planImage", _PlanImageFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestBusinessPlan_VM.Mode),
                            new SqlParameter("discountPercent", requestBusinessPlan_VM.DiscountPercent),
                            //new SqlParameter("planType", requestBusinessPlan_VM.PlanType),
                            new SqlParameter("planTypeTitle", requestBusinessPlan_VM.PlanTypeTitle)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessPlan @id, @businessOwnerLoginId, @businessPlanDurationTypeId, @name, @description, @price, @status, @compareAtPrice, @planImage, @submittedByLoginId, @mode, @discountPercent,@planTypeTitle", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Plan Image.
                    #region Insert-Update Event Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousPlanImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessPlanImage), _PreviousPlanImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_BusinessPlanImage), _PlanImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_PlanImageFile, FileWithPath);
                    }
                    #endregion
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

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/Delete/{id}")]
        public HttpResponseMessage DeleteBusinessPlan(int id)
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

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginID = validateResponse.BusinessAdminLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginID),
                            new SqlParameter("businessPlanDurationTypeId", "0"),
                            new SqlParameter("name", ""),
                            new SqlParameter("description", ""),
                            new SqlParameter("price", "0"),
                            new SqlParameter("status", "0"),
                            new SqlParameter("compareAtPrice", "0"),
                            new SqlParameter("planImage", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "3"),
                            new SqlParameter("discountPercent", "0"),
                            new SqlParameter("planType", "0"),
                            new SqlParameter("planTypeTitle", "0")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateBusinessPlan @id,@businessOwnerLoginId,@businessPlanDurationTypeId,@name,@description,@price,@status,@compareAtPrice,@planImage,@submittedByLoginId,@mode,@discountPercent,@planType,@planTypeTitle", queryParams).FirstOrDefault();
                //return resp;

                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
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


        // TODO: move api to BusinessPlanDurationType
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlanDurationType/GetAll")]
        [HttpGet]
        public HttpResponseMessage GetAllBusinessPlanDurationTypes()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<BusinessPlanDurationType_VM>("exec sp_ManageBusinessPlanDurationType @id,@mode", queryParams).ToList();

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
        /// Get All Active Business Plans by Business-Owner-Id
        /// </summary>
        /// <param name="businessOwnerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessPlan/GetAllActive/{businessOwnerId}")]
        public HttpResponseMessage GetAllActiveBusinessPlans(long businessOwnerId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                /*if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }*/

                var businessOwner = db.BusinessOwners.Where(bo => bo.Id == businessOwnerId).FirstOrDefault();
                if (businessOwner == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.BusinessNotFound_ErrorMessage;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", businessOwner.UserLoginId),
                            new SqlParameter("mode", "4")
                            };

                var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@mode", queryParams).ToList();

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
        /// Get Active Business Plan Detail for all users. [public]
        /// </summary>
        /// <param name="planId">Business-Plan-Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessPlan/DetialForUser")]
        public HttpResponseMessage GetActivePlanDetialByIdForUser(int planId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (planId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", planId),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("planType", "0"),
                            new SqlParameter("mode", "5")
                            };

                var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@planType,@mode", queryParams).FirstOrDefault();

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.PlanNotFound_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
                }

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
        /// Get All Active Business Plans users. [public] TODO: Remove later just for showing on HomePage
        /// </summary>
        /// <param name="planId">Business-Plan-Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessPlan/GetAllForUser")]
        public HttpResponseMessage GetAllActivePlansForUser()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("mode", "7")
                            };

                var resp = db.Database.SqlQuery<BusinessPlan_VM>("exec sp_ManageBusinessPlans @id,@businessOwnerLoginId,@mode", queryParams).ToList();

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.PlanNotFound_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
                }

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
        /// Get All Active Business Plans users-screen
        /// </summary>
        /// <param name="City">filter by city name</param>
        /// <param name="lastRecordId">Last fetched record Id</param>
        /// <param name="recordLimit">no. of records to fetch</param>
        /// <returns>List of Business Plans</returns>
        [HttpGet]
        [Route("api/BusinessPlan/GetAllBusinessPlansForUser")]
        public HttpResponseMessage GetAllActivePlansForBusiness(string City, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                List<BusinessPlan_VM> response = planService.GetAllBusinessPlan(City, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }

        /// <summary>
        /// Get Coaches About List Detail By Id for Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Class/GetCoachesPackageListById")]
        public HttpResponseMessage GetCoachesPackageListById(long Id, long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                List<CoachesInstructorPackageDetail> response = planService.GetCoachesPackageDetailListById(Id, lastRecordId, recordLimit);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = response;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }

        }

        #region Business Content Plan --------------------------------------------------
        /// <summary>
        /// Add Business Content Plan Detail
        /// </summary>
        /// <returns>If Status 1 to add the vedio, then -1 then occur error</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/AddUpdateBusinessPlanPCCMeta_Detail")]
        public HttpResponseMessage AddUpdatePlans()
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
                BusinessContentPlan_PPCMetaViewModel businessContentPlan_PPCMetaViewModel = new BusinessContentPlan_PPCMetaViewModel();
                businessContentPlan_PPCMetaViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                businessContentPlan_PPCMetaViewModel.BusinessPlanTitle = HttpRequest.Params["BusinessPlanTitle"].Trim();
                businessContentPlan_PPCMetaViewModel.BusinessPlanDescription = HttpRequest.Params["BusinessPlanDescription"].Trim();
                businessContentPlan_PPCMetaViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);



                // Validate infromation passed
                Error_VM error_VM = businessContentPlan_PPCMetaViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateBusinesContentPlanPPCMeta_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentPlanPPCMeta_Params_VM
                {

                    Id = businessContentPlan_PPCMetaViewModel.Id,
                    UserLoginId = _BusinessOwnerLoginId,
                    BusinessPlanTitle = businessContentPlan_PPCMetaViewModel.BusinessPlanTitle,
                    BusinessPlanDescription = businessContentPlan_PPCMetaViewModel.BusinessPlanDescription,
                    Mode = businessContentPlan_PPCMetaViewModel.Mode


                });


                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                // send success response
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
        /// To Get Business Content Plan Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/GetBusinessPlanDetail")]
        public HttpResponseMessage GetBusinessContentPlan()
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


                BusinessContentPlan_PPCMetaDetail_VM resp = new BusinessContentPlan_PPCMetaDetail_VM();

                resp = planService.GetBusinessContentPlanPPCMetaDetail(_BusinessOwnerLoginId);



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
        /// To Get Business Content Plan Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/GetBusinessPlanPPCMeta_Detail")]
        public HttpResponseMessage GetBusinessContentPlanDetail(long businessOwnerLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                BusinessContentPlan_PPCMetaDetail_VM resp = new BusinessContentPlan_PPCMetaDetail_VM();

                resp = planService.GetBusinessContentPlanPPCMetaDetail(businessOwnerLoginId);

                //List<BusinessContentPlan_PPCMetaDetail_VM> businessPlanList = planService.GetBusinessContentPlanDetailList(businessOwnerLoginId);
                List<BusinessPlan_VM> businessPlanList = planService.GetAllActiveBusinessPlans(businessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    BusinessContentPlan = resp,
                    BusinessContentPlanList = businessPlanList,
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
        #endregion -------------------------------------------------------------------



        /// <summary>
        /// get details of plan for my booking 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/BusinessPlan/GetPlanBookingDetails")]
        public HttpResponseMessage GetPlanBookingDetails()
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

                // Get Training-Record-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                             new SqlParameter("planId", "0"),
                            new SqlParameter("mode", "3")
                            };

                var response = db.Database.SqlQuery<PlanBooking_ViewModel>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = response;

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
        /// get plan , order, payment details by id for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/BusinessPlan/GetPlanBookingDetailById")]
        public HttpResponseMessage GetPlanBookingDetailById(long id,int planbookingtype)
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

                // Get plan-Booking-Detail
                var planBookingDetail = planService.GetPlanBookingDetailById(id, _LoginID_Exact,planbookingtype);

                

                    SqlParameter[] queryParams = new SqlParameter[] {
                             new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("planId", "0"),
                            new SqlParameter("mode", 9),
                            };

                   var  permissiondetailsList = db.Database.SqlQuery<PermissionViewModel>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).ToList();



                    //planBookingDetail.Password = EDClass.Decrypt(planBookingDetail.Password);

                    //       SqlParameter[] queryParamsPermissions = new SqlParameter[] {
                    //            new SqlParameter("id", "0"),
                    //        new SqlParameter("userLoginId", planBookingDetail.UserLoginId),
                    //        new SqlParameter("mode", "1")
                    //        };
                    //planBookingDetail.Permissions = db.Database.SqlQuery<PermissionHierarchy_VM>("exec sp_ManageUserPermissions @id, @userLoginId,@mode", queryParamsPermissions).ToList();


                    //var listPermissionHierarchy_VM = planBookingDetail.Permissions.Where(p => p.ParentPermissionId == 0).ToList();

                    //foreach (var permission in listPermissionHierarchy_VM)
                    //{
                    //    permission.SubPermissions = planBookingDetail.Permissions.Where(p => p.ParentPermissionId == permission.Id).ToList();
                    //    foreach (var subPermission in permission.SubPermissions)
                    //    {
                    //        subPermission.SubPermissions = planBookingDetail.Permissions.Where(p => p.ParentPermissionId == subPermission.Id).ToList();
                    //    }
                    //}

                    //planBookingDetail.PermissionsHierarchy = listPermissionHierarchy_VM;


                //// Get plan-Detail  
                //PlanBooking_ViewModel planbookingDetail = new PlanBooking_ViewModel();
                //planbookingDetail = planService.GetClassBookingDetail(planBookingDetail.Id, _LoginID_Exact);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(planBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    //PlanDetail = planbookingDetail,
                    PlanBookingDetail = planBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail,
                    PermissionAllList = permissiondetailsList
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
        /// get details of plan for my booking 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/GetBusinessPlanBookingDetails")]
        public HttpResponseMessage GetBusinessPlanBookingDetails()
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

                // Get Training-Record-Detail-By-Id
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                             new SqlParameter("planId", "0"),
                            new SqlParameter("mode", "5")
                            };

                var response = db.Database.SqlQuery<PlanBooking_ViewModel>("exec sp_ManagePlanBooking @id,@userLoginId,@planId,@mode", queryParams).ToList();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = response;

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
        /// get plan , order, payment details by id for view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/BusinessPlan/GetBusinessPlanBookingDetailById")]
        public HttpResponseMessage GetBusinessPlanBookingDetailById(long id)
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

                // Get plan-Booking-Detail
                var planBookingDetail = planService.GetBusinessPlanBookingDetailById(id, _BusinessOwnerLoginId);

                // Get plan-Detail  
                PlanBooking_ViewModel planbookingDetail = new PlanBooking_ViewModel();
                planbookingDetail = planService.GetBusinessClassBookingDetail(planBookingDetail.Id, _BusinessOwnerLoginId);

                // Get Order Detail
                OrderService orderService = new OrderService(db);
                var orderDetail = orderService.GetOrderDataById(planBookingDetail.OrderId);

                // Get Payment Response
                var paymentResponseDetail = orderService.GetPaymentResponseData(orderDetail.Id);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    PlanDetail = planbookingDetail,
                    PlanBookingDetail = planBookingDetail,
                    OrderDetail = orderDetail,
                    PaymentResponseDetail = paymentResponseDetail
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

        [HttpGet]
        [Route("api/BusinessPlan/GetActivePlanDetailByIdForUser")]
        public HttpResponseMessage GetActivePlanDetailByIdForUser(int planId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                if (planId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",planId),
                            new SqlParameter("userLoginId", "1"),
                            new SqlParameter("mode", "6")
                            };

                var resp = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).FirstOrDefault();

                if (resp == null)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.BusinessPanel.PlanNotFound_ErrorMessage;
                }
                else
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    apiResponse.data = resp;
                }

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