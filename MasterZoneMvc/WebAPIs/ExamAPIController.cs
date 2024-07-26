using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using MasterZoneMvc.Common.Helpers;
using System.Data.SqlClient;
using System.Web;
using System.Globalization;
using MasterZoneMvc.Services;
using System.IO;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.WebAPIs
{
    public class ExamAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private ExamService examService;
        private ExamFormSubmissionService examFormSubmitService;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;


        public ExamAPIController()
        {
            db = new MasterZoneDbContext();
            examService = new ExamService(db);
            examFormSubmitService = new ExamFormSubmissionService(db);
            fileHelper = new FileHelper();
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
        /// Add update exam form 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Exam/AddUpdateExamForm")]
        public HttpResponseMessage AddUpdateExamForm()
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
                RequestExamFrom_VM requestExamFrom_VM = new RequestExamFrom_VM();
                requestExamFrom_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestExamFrom_VM.Title = HttpRequest.Params["Title"].Trim();
                requestExamFrom_VM.EstablishedYear = HttpRequest.Params["EstablishedYear"];
                requestExamFrom_VM.StartDate = HttpRequest.Params["StartDate"];
                requestExamFrom_VM.EndDate = HttpRequest.Params["EndDate"];
                requestExamFrom_VM.SecretaryNumber = HttpRequest.Params["SecretaryNumber"];
                requestExamFrom_VM.RegistrarOfficeNumber = HttpRequest.Params["RegistrarNumber"];
                requestExamFrom_VM.WebsiteLink = HttpRequest.Params["WebsiteLink"];
                requestExamFrom_VM.Email = HttpRequest.Params["Email"];
                requestExamFrom_VM.ImportantInstruction = HttpUtility.UrlDecode(HttpRequest.Params["ImportantInstruction"]);
                requestExamFrom_VM.BusinessMasterId = HttpRequest.Params["BusinessMasterId"];
                requestExamFrom_VM.BusinessId = Convert.ToInt64(HttpRequest.Params["BusinessId"]);
                requestExamFrom_VM.CenterNo = Convert.ToInt64(HttpRequest.Params["CenterNo"]);
                requestExamFrom_VM.NameWithAddress = HttpRequest.Params["NameWithAddress"];
                requestExamFrom_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                
                requestExamFrom_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _ExamFormLogoFile = files["ExamFormLogo"];
                requestExamFrom_VM.ExamFormLogo = _ExamFormLogoFile; // for validation
                string _ExamFormLogoFileNameGenerated = ""; //will contains generated file name
                string _PreviousExamFormLogoFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestExamFrom_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_ExamFormLogoFile != null)
                    {
                        _ExamFormLogoFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_ExamFormLogoFile);
                    }
                }

                // if Edit Mode
                if (requestExamFrom_VM.Mode == 2)
                {

                    ExamFormResponse_VM examFormResponse = new ExamFormResponse_VM();

                    // Get Exam Form data By ExamFormId and BusinessId and display at Business/Exam

                    examFormResponse = examService.GetExamFormByExamFormId(requestExamFrom_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _ExamFormLogoFileNameGenerated = examFormResponse.ExamFormLogo;
                    }
                    else
                    {
                        _PreviousExamFormLogoFileName = examFormResponse.ExamFormLogo;
                    }
                }



                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id",  requestExamFrom_VM.Id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("title", requestExamFrom_VM.Title),
                            new SqlParameter("establishedYear",requestExamFrom_VM.EstablishedYear),
                            new SqlParameter("startDate",requestExamFrom_VM.StartDate),
                            new SqlParameter("endDate",requestExamFrom_VM.EndDate),
                            new SqlParameter("secretaryNumber",requestExamFrom_VM.SecretaryNumber),
                            new SqlParameter("registrarNumber",requestExamFrom_VM.RegistrarOfficeNumber),
                            new SqlParameter("websiteLink",requestExamFrom_VM.WebsiteLink),
                            new SqlParameter("email",requestExamFrom_VM.Email),
                            new SqlParameter("importantInstruction",requestExamFrom_VM.ImportantInstruction),
                            new SqlParameter("examFormLogo",_ExamFormLogoFileNameGenerated),
                            new SqlParameter("nameWithAddress",requestExamFrom_VM.NameWithAddress),
                            new SqlParameter("businessMasterId",requestExamFrom_VM.BusinessMasterId),
                            new SqlParameter("businessId", requestExamFrom_VM.BusinessId),
                            new SqlParameter("centerNo", requestExamFrom_VM.CenterNo),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("status", requestExamFrom_VM.Status),
                            new SqlParameter("mode", requestExamFrom_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateExamForm @id,@userLoginId,@title, @establishedYear,@startDate,@endDate,@secretaryNumber,@registrarNumber,@websiteLink,@email,@importantInstruction,@examFormLogo,@nameWithAddress,@businessMasterId ,@businessId,@centerNo,@submittedByLoginId,@status,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Event Image.
                    #region Insert-Update Event Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousExamFormLogoFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ExamFormLogo), _PreviousExamFormLogoFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_ExamFormLogo), _ExamFormLogoFileNameGenerated);
                        fileHelper.SaveUploadedFile(_ExamFormLogoFile, FileWithPath);
                    }
                    #endregion
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
        /// Get all exam form list by BusinessId and display at Business/Exam
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Exam/GetExamFormByBusinessId")]
        public HttpResponseMessage GetExamFormByBusinessId()
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

                List<ExamFormResponse_VM> examFormResponse_VMs = new List<ExamFormResponse_VM>();

                examFormResponse_VMs = examService.GetExamFormByBusinessId(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = examFormResponse_VMs;

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
        /// get exam form data by ExamFormId when user want to Edit the Exam Form 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Exam/GetEditExamFormById")]
        public HttpResponseMessage GetEditExamFormById(long id)
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

                ExamFormResponse_VM examFormResponse_VM = new ExamFormResponse_VM();

                // Get Exam Form By ExamFormId, BusinessId and display at Business/EditExam
                examFormResponse_VM = examService.GetExamFormByExamFormId(id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = examFormResponse_VM;

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
        /// Delete exam from data by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Exam/DeleteExamFormById")]
        public HttpResponseMessage DeleteExamFormById(long id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("title",""),
                            new SqlParameter("establishedYear",""),
                            new SqlParameter("startDate",""),
                            new SqlParameter("endDate",""),
                            new SqlParameter("secretaryNumber",""),
                            new SqlParameter("registrarNumber",""),
                            new SqlParameter("websiteLink",""),
                            new SqlParameter("email",""),
                            new SqlParameter("importantInstruction",""),
                            new SqlParameter("examFormLogo",""),
                            new SqlParameter("nameWithAddress",""),
                            new SqlParameter("businessMasterId",""),
                            new SqlParameter("businessId", "0"),
                            new SqlParameter("centerNo", "0"),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("status", "0"),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateExamForm @id,@userLoginId,@title, @establishedYear,@startDate,@endDate,@secretaryNumber,@registrarNumber,@websiteLink,@email,@importantInstruction,@examFormLogo,@nameWithAddress,@businessMasterId ,@businessId,@centerNo,@submittedByLoginId,@status,@mode", queryParams).FirstOrDefault();

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
        /// update exam form status when user edit the from
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Exam/ChangeStatus/{id}")]
        public HttpResponseMessage UpdateExamFormStatus(int id)
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

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("title",""),
                            new SqlParameter("establishedYear",""),
                            new SqlParameter("startDate",""),
                            new SqlParameter("endDate",""),
                            new SqlParameter("secretaryNumber",""),
                            new SqlParameter("registrarNumber",""),
                            new SqlParameter("websiteLink",""),
                            new SqlParameter("email",""),
                            new SqlParameter("importantInstruction",""),
                            new SqlParameter("examFormLogo",""),
                            new SqlParameter("nameWithAddress",""),
                            new SqlParameter("businessMasterId",""),
                            new SqlParameter("businessId", "0"),
                            new SqlParameter("centerNo", "0"),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("status", "0"),
                            new SqlParameter("mode", "4")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateExamForm @id,@userLoginId,@title, @establishedYear,@startDate,@endDate,@secretaryNumber,@registrarNumber,@websiteLink,@email,@importantInstruction,@examFormLogo,@nameWithAddress,@businessMasterId ,@businessId,@centerNo,@submittedByLoginId,@status,@mode", queryParams).FirstOrDefault();

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

        /// <summary>
        /// Get All exam form list which is active and display at Home/Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Exam/GetAllExamFormList")]
        public HttpResponseMessage GetAllExamFormList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                List<ExamFormResponse_VM> examFormResponse_VMs = new List<ExamFormResponse_VM>();
                examFormResponse_VMs = examService.GetAllExamFormList();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = examFormResponse_VMs;

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
        /// get exam form data by ExamFormId and display at Home/SubmitExamForm?examId=2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Exam/GetExamFormForVistor")]
        public HttpResponseMessage GetExamFormForVistor(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {

                ExamFormResponse_VM examFormResponse_VM = new ExamFormResponse_VM();

                // Get Exam Form By Id
                examFormResponse_VM = examService.GetExamFormByExamFormId(id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = examFormResponse_VM;

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
        /// Visitor/Student Submit the exam form application 
        /// </summary>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Route("api/Exam/SubmitExamForm")]
        public HttpResponseMessage SubmitExamForm()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestSubmitExamForm_VM requestSubmitExamForm_VM = new RequestSubmitExamForm_VM();
                requestSubmitExamForm_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestSubmitExamForm_VM.ExamFormId = Convert.ToInt64(HttpRequest.Params["ExamFormId"]);
                requestSubmitExamForm_VM.SessionYear = HttpRequest.Params["SessionYear"];
                requestSubmitExamForm_VM.Category = HttpRequest.Params["Category"];
                requestSubmitExamForm_VM.UserMasterId = HttpRequest.Params["UserMasterId"];
                //requestSubmitExamForm_VM.CurrentRollNo = HttpRequest.Params["CurrentRollNo"];
                requestSubmitExamForm_VM.CurrentRollNo = "";
                requestSubmitExamForm_VM.CandidateName = HttpRequest.Params["CandidateName"];
                requestSubmitExamForm_VM.CandidateFather = HttpRequest.Params["CandidateFather"];
                requestSubmitExamForm_VM.CandidateMother = HttpRequest.Params["CandidateMother"];
                requestSubmitExamForm_VM.PermanentAddress = HttpRequest.Params["PermanentAddress"];
                requestSubmitExamForm_VM.PermanentPin = (string.IsNullOrEmpty(HttpRequest.Params["PermanentPin"])) ? -1 : Convert.ToInt64(HttpRequest.Params["PermanentPin"]);
                requestSubmitExamForm_VM.PermanentMobNo = HttpRequest.Params["PermanentMobNo"];
                requestSubmitExamForm_VM.PresentAddress = HttpRequest.Params["PresentAddress"];
                requestSubmitExamForm_VM.PresentPin = (string.IsNullOrEmpty(HttpRequest.Params["PresentPin"])) ? -1 : Convert.ToInt64(HttpRequest.Params["PresentPin"]);
                requestSubmitExamForm_VM.PresentMobNo = HttpRequest.Params["PresentMobNo"];
                requestSubmitExamForm_VM.Nationality = HttpRequest.Params["Nationality"];
                requestSubmitExamForm_VM.AadharCardNo = (string.IsNullOrEmpty(HttpRequest.Params["AadharCardNo"])) ? -1 : Convert.ToInt64(HttpRequest.Params["AadharCardNo"]);
                //requestSubmitExamForm_VM.DOB = (string.IsNullOrEmpty(HttpRequest.Params["DOB"])) ? DateTime.MinValue : Convert.ToDateTime(HttpRequest.Params["DOB"]);
                string dobString = HttpRequest.Params["DOB"];
                DateTime dob;

                if (string.IsNullOrEmpty(dobString))
                {
                    requestSubmitExamForm_VM.DOB = DateTime.MinValue;
                }
                else if (DateTime.TryParseExact(dobString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
                {
                    requestSubmitExamForm_VM.DOB = dob;
                }
                requestSubmitExamForm_VM.Email = HttpRequest.Params["Email"];
                requestSubmitExamForm_VM.EduQualification = HttpRequest.Params["EduQualification"];
                requestSubmitExamForm_VM.CurrentClass = HttpRequest.Params["CurrentClass"];
                requestSubmitExamForm_VM.CurrentSubject = HttpRequest.Params["CurrentSubject"];
                requestSubmitExamForm_VM.CurrentCenterName = HttpRequest.Params["CurrentCenterName"];
                requestSubmitExamForm_VM.CurrentCenterCity = HttpRequest.Params["CurrentCenterCity"];
                requestSubmitExamForm_VM.PreviousClass = HttpRequest.Params["PreviousClass"];
                requestSubmitExamForm_VM.PreviousSubject = HttpRequest.Params["PreviousSubject"];
                requestSubmitExamForm_VM.PreviousYear = (string.IsNullOrEmpty(HttpRequest.Params["PreviousYear"])) ? -1 : Convert.ToInt32(HttpRequest.Params["PreviousYear"]);
                requestSubmitExamForm_VM.PreviousRollNo = (string.IsNullOrEmpty(HttpRequest.Params["PreviousRollNo"])) ? -1 : Convert.ToInt64(HttpRequest.Params["PreviousRollNo"]);
                requestSubmitExamForm_VM.PreviousResult = (string.IsNullOrEmpty(HttpRequest.Params["PreviousResult"])) ? -1 : Convert.ToInt32(HttpRequest.Params["PreviousResult"]);
                requestSubmitExamForm_VM.PreviousCenterName = HttpRequest.Params["PreviousCenterName"];
                requestSubmitExamForm_VM.Amount = (string.IsNullOrEmpty(HttpRequest.Params["Amount"])) ? -1 : Convert.ToInt32(HttpRequest.Params["Amount"]);
                requestSubmitExamForm_VM.AmountInWord = HttpRequest.Params["AmountInWord"];
                requestSubmitExamForm_VM.NoOfAttached = (string.IsNullOrEmpty(HttpRequest.Params["NoOfAttached"])) ? -1 : Convert.ToInt32(HttpRequest.Params["NoOfAttached"]);
                requestSubmitExamForm_VM.CertificateCollectFrom = HttpRequest.Params["CertificateCollectFrom"];
                requestSubmitExamForm_VM.CandidateGuradianName = HttpRequest.Params["CandidateGuradianName"];
                requestSubmitExamForm_VM.BankDraftNo = ""; //HttpRequest.Params["BankDraftNo"];
                requestSubmitExamForm_VM.BankDraftDate = ""; // HttpRequest.Params["BankDraftDate"];
                requestSubmitExamForm_VM.PostalOrderNo = ""; //HttpRequest.Params["PostalOrderNo"];
                requestSubmitExamForm_VM.SuperintendentName = ""; //HttpRequest.Params["SuperintendentName"];
                requestSubmitExamForm_VM.SuperintendentPinNo = ""; // HttpRequest.Params["SuperintendentPinNo"];
                requestSubmitExamForm_VM.SuperintendentPhoneNo = ""; //HttpRequest.Params["SuperintendentPhoneNo"];
                requestSubmitExamForm_VM.SuperintendentEmail = ""; //HttpRequest.Params["SuperintendentEmail"];
                requestSubmitExamForm_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);


                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _CandidateProfileImage = files["CandidateProfileImage"];
                HttpPostedFile _CandidateSignature = files["CandidateSignature"];
                HttpPostedFile _GuradianSignature = files["CandidateGuradianSignature"];
                HttpPostedFile _SuperintendentSignature = files["SuperintendentSignature"];
                requestSubmitExamForm_VM.CandidateProfileImage = _CandidateProfileImage; // for validation
                requestSubmitExamForm_VM.CandidateSignature = _CandidateSignature; // for validation
                requestSubmitExamForm_VM.CandidateGuradianSignature = _GuradianSignature; // for validation
                requestSubmitExamForm_VM.SuperintendentSignature = _SuperintendentSignature; // for validation
                string _CandidateProfileImageFileNameGenerated = "";
                string _CandidateSignatureFileNameGenerated = "";
                string _PreviousCandidateProfileImageFileName = "";
                string _PreviousCandidateSignatureFileName = "";

                string _GuradianSignatureFileNameGenerated = "";
                string _SuperintendentSignatureFileNameGenerated = "";
                string _PreviousGuradianSignatureFileName = "";
                string _PreviousSuperintendentSignatureFileName = "";


                // Validate infromation passed
                Error_VM error_VM = requestSubmitExamForm_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (files.Count > 0)
                {
                    if (_CandidateProfileImage != null && _CandidateSignature != null)
                    {
                        _CandidateProfileImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CandidateProfileImage);
                        _CandidateSignatureFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_CandidateSignature);

                        _GuradianSignatureFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_GuradianSignature);
                        //_SuperintendentSignatureFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_SuperintendentSignature);
                    }
                }

                // if Edit Mode
                if (requestSubmitExamForm_VM.Mode == 2 && requestSubmitExamForm_VM.Id > 0)
                {

                    ExamFormSubmissionResponse_VM examFormResponse = new ExamFormSubmissionResponse_VM();

                    // Get Submited Exam Form data By Id

                    examFormResponse = examFormSubmitService.GetExamFormSubmissionImage(requestSubmitExamForm_VM.Id);

                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _CandidateProfileImageFileNameGenerated = examFormResponse.CandidateProfileImage;
                        _CandidateSignatureFileNameGenerated = examFormResponse.CandidateSignature;
                        _GuradianSignatureFileNameGenerated = examFormResponse.CandidateGuradianSignature;
                        //_SuperintendentSignatureFileNameGenerated = examFormResponse.SuperintendentSignature;
                    }
                    else
                    {
                        _PreviousCandidateProfileImageFileName = examFormResponse.CandidateProfileImage;
                        _PreviousCandidateSignatureFileName = examFormResponse.CandidateSignature;
                        _PreviousGuradianSignatureFileName = examFormResponse.CandidateGuradianSignature;
                        //_PreviousSuperintendentSignatureFileName = examFormResponse.SuperintendentSignature;
                    }
                }



                var resp = storedProcedureRepository.SP_InsertUpdateExamFormSubmission<SPResponseViewModel>(new SP_InsertUpdateExamFormSubmission_Param_VM
                {

                    Id = requestSubmitExamForm_VM.Id,
                    ExamFormId = requestSubmitExamForm_VM.ExamFormId,
                    SessionYear = requestSubmitExamForm_VM.SessionYear,
                    CandidateProfileImage = _CandidateProfileImageFileNameGenerated,
                    Category = requestSubmitExamForm_VM.Category,
                    UserMasterId = requestSubmitExamForm_VM.UserMasterId,
                    CurrentRollNo = requestSubmitExamForm_VM.CurrentRollNo,
                    CandidateName = requestSubmitExamForm_VM.CandidateName,
                    CandidateFather = requestSubmitExamForm_VM.CandidateFather,
                    CandidateMother = requestSubmitExamForm_VM.CandidateMother,
                    PermanentAddress = requestSubmitExamForm_VM.PermanentAddress,
                    PermanentPin = requestSubmitExamForm_VM.PermanentPin,
                    PermanentMobNo = requestSubmitExamForm_VM.PermanentMobNo,
                    PresentAddress = requestSubmitExamForm_VM.PresentAddress,
                    PresentPin = requestSubmitExamForm_VM.PresentPin,
                    PresentMobNo = requestSubmitExamForm_VM.PresentMobNo,
                    Nationality = requestSubmitExamForm_VM.Nationality,
                    AadharCardNo = requestSubmitExamForm_VM.AadharCardNo,
                    DOB = requestSubmitExamForm_VM.DOB,
                    Email = requestSubmitExamForm_VM.Email,
                    EduQualification = requestSubmitExamForm_VM.EduQualification,
                    CurrentClass = requestSubmitExamForm_VM.CurrentClass,
                    CurrentSubject = requestSubmitExamForm_VM.CurrentSubject,
                    CurrentCenterName = requestSubmitExamForm_VM.CurrentCenterName,
                    CurrentCenterCity = requestSubmitExamForm_VM.CurrentCenterCity,
                    PreviousClass = requestSubmitExamForm_VM.PreviousClass,
                    PreviousSubject = requestSubmitExamForm_VM.PreviousSubject,
                    PreviousYear = requestSubmitExamForm_VM.PreviousYear,
                    PreviousRollNo = requestSubmitExamForm_VM.PreviousRollNo,
                    PreviousResult = requestSubmitExamForm_VM.PreviousResult,
                    PreviousCenterName = requestSubmitExamForm_VM.PreviousCenterName,
                    Amount = requestSubmitExamForm_VM.Amount,
                    AmountInWord = requestSubmitExamForm_VM.AmountInWord,
                    NoOfAttached = requestSubmitExamForm_VM.NoOfAttached,
                    CertificateCollectFrom = requestSubmitExamForm_VM.CertificateCollectFrom,
                    CandidateSignature = _CandidateSignatureFileNameGenerated,
                    CandidateGuradianSignature = _GuradianSignatureFileNameGenerated,
                    CandidateGuradianName = requestSubmitExamForm_VM.CandidateGuradianName,
                    BankDraftNo = requestSubmitExamForm_VM.BankDraftNo,
                    BankDraftDate = requestSubmitExamForm_VM.BankDraftDate,
                    PostalOrderNo = requestSubmitExamForm_VM.PostalOrderNo,
                    SuperintendentSignature = _SuperintendentSignatureFileNameGenerated,
                    SuperintendentName = requestSubmitExamForm_VM.SuperintendentName,
                    SuperintendentPinNo = requestSubmitExamForm_VM.SuperintendentPinNo,
                    SuperintendentPhoneNo = requestSubmitExamForm_VM.SuperintendentPhoneNo,
                    SuperintendentEmail = requestSubmitExamForm_VM.SuperintendentEmail,
                    Mode = requestSubmitExamForm_VM.Mode,

                });
                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Event Image.
                    #region Insert-Update Event Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousCandidateProfileImageFileName) && !String.IsNullOrEmpty(_PreviousCandidateSignatureFileName) && !String.IsNullOrEmpty(_PreviousGuradianSignatureFileName) && !String.IsNullOrEmpty(_PreviousSuperintendentSignatureFileName))
                        {
                            // remove previous file
                            string RemoveProfileFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormProfileImage), _PreviousCandidateProfileImageFileName);

                            string RemoveSignatureFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormSignature), _PreviousCandidateSignatureFileName);
                            string RemoveGuradianSignatureFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormSignature), _PreviousGuradianSignatureFileName);

                            //string RemoveSuperintendentSignatureFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormSignature), _PreviousSuperintendentSignatureFileName);

                            fileHelper.DeleteAttachedFileFromServer(RemoveProfileFileWithPath);
                            fileHelper.DeleteAttachedFileFromServer(RemoveSignatureFileWithPath);
                            fileHelper.DeleteAttachedFileFromServer(RemoveGuradianSignatureFileWithPath);
                            //fileHelper.DeleteAttachedFileFromServer(RemoveSuperintendentSignatureFileWithPath);
                        }

                        // save new file
                        string ProfileFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormProfileImage), _CandidateProfileImageFileNameGenerated);

                        string SignatureFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormSignature), _CandidateSignatureFileNameGenerated);

                        string GuradianSignatureFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormSignature), _GuradianSignatureFileNameGenerated);

                        //string SuperintendentSignatureFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SubmitExamFormSignature), _SuperintendentSignatureFileNameGenerated);

                        fileHelper.SaveUploadedFile(_CandidateProfileImage, ProfileFileWithPath);
                        fileHelper.SaveUploadedFile(_CandidateSignature, SignatureFileWithPath);
                        fileHelper.SaveUploadedFile(_GuradianSignature, GuradianSignatureFileWithPath);
                        //fileHelper.SaveUploadedFile(_SuperintendentSignature, SuperintendentSignatureFileWithPath);
                    }
                    #endregion
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
                //apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                apiResponse.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get the All submited Exam Form by visitor 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Exam/GetSubmitExamForm")]
        public HttpResponseMessage GetSubmitExamFormList(long Id = 0, string UserMasterId = "", long ExamFormId = 0, long BusinessId = 0, long BusinessMasterId = 0)
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
                RequestSubmitExamForm_VM requestSubmitExamForm_VM = new RequestSubmitExamForm_VM();
                requestSubmitExamForm_VM.Id = Id;
                requestSubmitExamForm_VM.UserMasterId = UserMasterId;
                requestSubmitExamForm_VM.ExamFormId = ExamFormId;
                requestSubmitExamForm_VM.BusinessId = BusinessId;
                requestSubmitExamForm_VM.BusinessMasterId = BusinessMasterId;

                dynamic examFormResponse_VMs;
                if (requestSubmitExamForm_VM.Id == 0 && requestSubmitExamForm_VM.UserMasterId == "" && requestSubmitExamForm_VM.ExamFormId == 0 && requestSubmitExamForm_VM.BusinessId == 0 && requestSubmitExamForm_VM.BusinessMasterId == 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = "If Mode is 1 then must to pass Id otherwise change the mode 1 to 2";
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (requestSubmitExamForm_VM.Id == 0)
                {
                    examFormResponse_VMs = examFormSubmitService.GetExamFormSubmissionList(requestSubmitExamForm_VM.UserMasterId, requestSubmitExamForm_VM.ExamFormId, requestSubmitExamForm_VM.BusinessId, requestSubmitExamForm_VM.BusinessMasterId);
                }
                else
                {
                    examFormResponse_VMs = examFormSubmitService.GetExamFormSubmissionById(requestSubmitExamForm_VM.Id);
                }


                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = examFormResponse_VMs;

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
        /// Visitor/Student Submit the exam form application 
        /// </summary>
        /// <returns>Success or error response</returns>
        [HttpPost]
        [Route("api/Exam/SubmitRollNumberExamForm")]
        public HttpResponseMessage SubmitRollNumberForm()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestSubmitExamForm_VM requestSubmitExamForm_VM = new RequestSubmitExamForm_VM();
                requestSubmitExamForm_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestSubmitExamForm_VM.ExamFormId = Convert.ToInt64(HttpRequest.Params["ExamFormId"]);
                requestSubmitExamForm_VM.SessionYear = HttpRequest.Params["SessionYear"];
                requestSubmitExamForm_VM.Category = HttpRequest.Params["Category"];
                requestSubmitExamForm_VM.UserMasterId = HttpRequest.Params["UserMasterId"];
                requestSubmitExamForm_VM.CurrentRollNo = HttpRequest.Params["CurrentRollNo"];
                requestSubmitExamForm_VM.CandidateName = HttpRequest.Params["CandidateName"];
                requestSubmitExamForm_VM.CandidateFather = HttpRequest.Params["CandidateFather"];
                requestSubmitExamForm_VM.CandidateMother = HttpRequest.Params["CandidateMother"];
                requestSubmitExamForm_VM.PermanentAddress = HttpRequest.Params["PermanentAddress"];
                requestSubmitExamForm_VM.PermanentPin = (string.IsNullOrEmpty(HttpRequest.Params["PermanentPin"])) ? -1 : Convert.ToInt64(HttpRequest.Params["PermanentPin"]);
                requestSubmitExamForm_VM.PermanentMobNo = HttpRequest.Params["PermanentMobNo"];
                requestSubmitExamForm_VM.PresentAddress = HttpRequest.Params["PresentAddress"];
                requestSubmitExamForm_VM.PresentPin = (string.IsNullOrEmpty(HttpRequest.Params["PresentPin"])) ? -1 : Convert.ToInt64(HttpRequest.Params["PresentPin"]);
                requestSubmitExamForm_VM.PresentMobNo = HttpRequest.Params["PresentMobNo"];
                requestSubmitExamForm_VM.Nationality = HttpRequest.Params["Nationality"];
                requestSubmitExamForm_VM.AadharCardNo = (string.IsNullOrEmpty(HttpRequest.Params["AadharCardNo"])) ? -1 : Convert.ToInt64(HttpRequest.Params["AadharCardNo"]);
                requestSubmitExamForm_VM.DOB = (string.IsNullOrEmpty(HttpRequest.Params["DOB"])) ? DateTime.MinValue : Convert.ToDateTime(HttpRequest.Params["DOB"]);
                requestSubmitExamForm_VM.Email = HttpRequest.Params["Email"];
                requestSubmitExamForm_VM.EduQualification = HttpRequest.Params["EduQualification"];
                requestSubmitExamForm_VM.CurrentClass = HttpRequest.Params["CurrentClass"];
                requestSubmitExamForm_VM.CurrentSubject = HttpRequest.Params["CurrentSubject"];
                requestSubmitExamForm_VM.CurrentCenterName = HttpRequest.Params["CurrentCenterName"];
                requestSubmitExamForm_VM.CurrentCenterCity = HttpRequest.Params["CurrentCenterCity"];
                requestSubmitExamForm_VM.PreviousClass = HttpRequest.Params["PreviousClass"];
                requestSubmitExamForm_VM.PreviousSubject = HttpRequest.Params["PreviousSubject"];
                requestSubmitExamForm_VM.PreviousYear = (string.IsNullOrEmpty(HttpRequest.Params["PreviousYear"])) ? -1 : Convert.ToInt32(HttpRequest.Params["PreviousYear"]);
                requestSubmitExamForm_VM.PreviousRollNo = (string.IsNullOrEmpty(HttpRequest.Params["PreviousRollNo"])) ? -1 : Convert.ToInt64(HttpRequest.Params["PreviousRollNo"]);
                requestSubmitExamForm_VM.PreviousResult = (string.IsNullOrEmpty(HttpRequest.Params["PreviousResult"])) ? -1 : Convert.ToInt32(HttpRequest.Params["PreviousResult"]);
                requestSubmitExamForm_VM.PreviousCenterName = HttpRequest.Params["PreviousCenterName"];
                requestSubmitExamForm_VM.Amount = (string.IsNullOrEmpty(HttpRequest.Params["Amount"])) ? -1 : Convert.ToInt32(HttpRequest.Params["Amount"]);
                requestSubmitExamForm_VM.AmountInWord = HttpRequest.Params["AmountInWord"];
                requestSubmitExamForm_VM.NoOfAttached = (string.IsNullOrEmpty(HttpRequest.Params["NoOfAttached"])) ? -1 : Convert.ToInt32(HttpRequest.Params["NoOfAttached"]);
                requestSubmitExamForm_VM.CertificateCollectFrom = HttpRequest.Params["CertificateCollectFrom"];
                requestSubmitExamForm_VM.CandidateGuradianName = HttpRequest.Params["CandidateGuradianName"];
                requestSubmitExamForm_VM.BankDraftNo = ""; //HttpRequest.Params["BankDraftNo"];
                requestSubmitExamForm_VM.BankDraftDate = ""; // HttpRequest.Params["BankDraftDate"];
                requestSubmitExamForm_VM.PostalOrderNo = ""; //HttpRequest.Params["PostalOrderNo"];
                requestSubmitExamForm_VM.SuperintendentName = ""; //HttpRequest.Params["SuperintendentName"];
                requestSubmitExamForm_VM.SuperintendentPinNo = ""; // HttpRequest.Params["SuperintendentPinNo"];
                requestSubmitExamForm_VM.SuperintendentPhoneNo = ""; //HttpRequest.Params["SuperintendentPhoneNo"];
                requestSubmitExamForm_VM.SuperintendentEmail = ""; //HttpRequest.Params["SuperintendentEmail"];
                requestSubmitExamForm_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);




                // Validate infromation passed
                Error_VM error_VM = requestSubmitExamForm_VM.ValidRollNumberInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                var resp = storedProcedureRepository.SP_InsertUpdateExamFormSubmission<SPResponseViewModel>(new SP_InsertUpdateExamFormSubmission_Param_VM

                {

                    Id = requestSubmitExamForm_VM.Id,
                    CurrentRollNo = requestSubmitExamForm_VM.CurrentRollNo,
                    Mode = requestSubmitExamForm_VM.Mode,

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

    }
}