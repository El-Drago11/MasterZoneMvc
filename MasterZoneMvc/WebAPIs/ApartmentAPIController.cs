using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using MasterZoneMvc.Common.Helpers;
using System.Web;
using Newtonsoft.Json;
using MasterZoneMvc.Models.Enum;
using System.Data.SqlClient;
using MasterZoneMvc.Models;
using System.Runtime.InteropServices;

namespace MasterZoneMvc.WebAPIs
{
    public class ApartmentAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private ApartmentService apartmentService;

        public ApartmentAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            apartmentService = new ApartmentService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }


        #region Apartment CRUD ---------------------------------------------------------------------

        /// <summary>
        /// Get Apartment Detail By Id
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>Apartment Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetById")]
        public HttpResponseMessage GetApartmentDetailById(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (apartmentId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Menu By Id
                var resp = apartmentService.GetApartmentById(apartmentId);

               
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
        /// Add or Update Apartment
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/AddUpdate")]
        public HttpResponseMessage AddUpdateApartment()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestApartmentViewModel requestMenu_VM = new RequestApartmentViewModel();
                requestMenu_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestMenu_VM.Name = HttpRequest.Params["Name"].Trim();
                requestMenu_VM.Status = Convert.ToInt32(HttpRequest.Params["Status"]);
                requestMenu_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);
                
                // Validate infromation passed
                Error_VM error_VM = requestMenu_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update Menu Record
                var resp = apartmentService.InsertUpdateApartment(new ViewModels.StoredProcedureParams.SP_InsertUpdateApartment_Params_VM()
                {
                    Id = requestMenu_VM.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    Name = requestMenu_VM.Name,
                    Status = requestMenu_VM.Status,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestMenu_VM.Mode
                });

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
        /// Delete Apartment By Apartment-Id
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>Status 1 if deleted else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Delete")]
        public HttpResponseMessage DeleteApartment(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (apartmentId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Delete Menu by Id
                SPResponseViewModel respDeleteMenu = apartmentService.DeleteApartmentById(apartmentId, _BusinessOwnerLoginId);

                apiResponse.status = respDeleteMenu.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(respDeleteMenu.resourceFileName, respDeleteMenu.resourceKey);
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
        /// Change Apartment Active/Inactive status [automatically swithes to opposite value]
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>Api Resoponse: Status 1 if updated else -ve value with error message.</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/ChangeStatus")]
        public HttpResponseMessage ChangeApartmentStatus(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (apartmentId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Change Apartment Status
                var resp = apartmentService.ChangeApartmentStatusById(apartmentId, _BusinessOwnerLoginId);

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
        /// Get All Apartments data by the Pagination
        /// </summary>
        /// <returns>List of Apartments</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetAllApartmentByPagination")]
        public HttpResponseMessage GetAllLicenseDataTablePagination()
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

                Apartment_Pagination_SQL_Params_VM _Params_VM = new Apartment_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.BusinessOwnerLoginId = _BusinessOwnerLoginId;
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;

                var paginationResponse = apartmentService.GetAllApartments_Pagination(HttpRequestParams, _Params_VM);

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

        #endregion -------------------------------------------------------------------------

        /// <summary>
        /// Get All Active Apartments
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>Apartment Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetAllActiveApartments")]
        public HttpResponseMessage GetAllActiveApartments()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
               

                // Get All Active Apartment list
                var resp = apartmentService.GetAllActiveApartmentsForDropdown(_BusinessOwnerLoginId);

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
        /// Get All Apartment Blocks list
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>Apartment Blocks List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetAllApartmentBlocks")]
        public HttpResponseMessage GetAllApartmentBlocks(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
               

                //// Get Apartment by Id
                //var apartment = apartmentService.GetApartmentById(apartmentId);

                //List<string> blockList = new List<string>();
                //if(apartment != null)
                //{
                //    blockList = apartment.Blocks.Split(',').ToList();
                //}

                // Get All Apartment Blocks
                var apartmentBlocks = apartmentService.GetAllApartmentBlocks(apartmentId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = apartmentBlocks;

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
        /// Get All Apartment Areas
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>Apartment Areas List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetAllApartmentAreas")]
        public HttpResponseMessage GetAllApartmentAreas(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
               
                //// Get Apartment by Id
                //var apartment = apartmentService.GetApartmentById(apartmentId);

                //List<string> areasList = new List<string>();
                //if(apartment != null)
                //{
                //    areasList = apartment.Areas.Split(',').ToList();
                //}

                var apartmentAreas = apartmentService.GetAllApartmentAreas(apartmentId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = apartmentAreas;

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
        /// Book Apartment
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/BookApartment")]
        public HttpResponseMessage BookApartment()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
                
                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestApartmentBooking_VM requestApartmentBooking_VM = new RequestApartmentBooking_VM();
                requestApartmentBooking_VM.ApartmentId = Convert.ToInt64(HttpRequest.Params["ApartmentId"]);
                requestApartmentBooking_VM.BatchId = Convert.ToInt64(HttpRequest.Params["BatchId"]);
                requestApartmentBooking_VM.ApartmentBlockId = Convert.ToInt64(HttpRequest.Params["ApartmentBlockId"]);
                requestApartmentBooking_VM.FlatOrVillaNumber = HttpRequest.Params["FlatOrVillaNumber"].Trim();
                requestApartmentBooking_VM.Phase = HttpRequest.Params["Phase"].Trim();
                requestApartmentBooking_VM.Lane = HttpRequest.Params["Lane"].Trim();
                requestApartmentBooking_VM.OccupantType = HttpRequest.Params["OccupantType"].Trim();
                requestApartmentBooking_VM.ApartmentAreaId = Convert.ToInt64(HttpRequest.Params["ApartmentAreaId"]);
                requestApartmentBooking_VM.Activity = HttpRequest.Params["Activity"].Trim();
                requestApartmentBooking_VM.PersonHasMasterId = Convert.ToInt32(HttpRequest.Params["PersonHasMasterId"].Trim());
                requestApartmentBooking_VM.PersonMasterId = HttpRequest.Params["PersonMasterId"].Trim();
                requestApartmentBooking_VM.PersonFirstName = HttpRequest.Params["PersonFirstName"].Trim();
                requestApartmentBooking_VM.PersonLastName = HttpRequest.Params["PersonLastName"].Trim();
                requestApartmentBooking_VM.PersonEmail = HttpRequest.Params["PersonEmail"].Trim();
                requestApartmentBooking_VM.PersonPhoneNumber = HttpRequest.Params["PersonPhoneNumber"].Trim();
                requestApartmentBooking_VM.PersonGender = Convert.ToInt32(HttpRequest.Params["PersonGender"].Trim());
                requestApartmentBooking_VM.FamilyMemberList = (!string.IsNullOrEmpty(HttpRequest.Params["FamilyMemberList"])) ? JsonConvert.DeserializeObject<List<RequestFamilyMember_VM>>(HttpRequest.Params["FamilyMemberList"]) : new List<RequestFamilyMember_VM>();
                requestApartmentBooking_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _PersonProfileImageFile = files["PersonProfileImage"];
                string _PersonProfileImageFileNameGenerated = ""; //will contains generated file name
                requestApartmentBooking_VM.PersonProfileImageFile = _PersonProfileImageFile;
                if (_PersonProfileImageFile != null)
                    _PersonProfileImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_PersonProfileImageFile);

                // Check and set Family Member sent Images
                if (files.Count > 0 && requestApartmentBooking_VM.FamilyMemberList.Count() > 0)
                {
                    foreach(var familyMember in requestApartmentBooking_VM.FamilyMemberList)
                    {
                        familyMember.ProfileImageFile = files["FamilyMemberImage_"+familyMember.Id];
                        
                        if(familyMember.ProfileImageFile != null) { 
                            familyMember.ProfileImage = fileHelper.GenerateFileNameTimeStamp(familyMember.ProfileImageFile);
                        }
                        else
                        {
                            familyMember.ProfileImage = "";
                        }
                    }
                }

                // Validate infromation passed
                Error_VM error_VM = requestApartmentBooking_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _PersonUserLoginId = 0;
                int _PersonGender = 0;
                // Check and verify MasterId of user and get UserLoginId
                if (requestApartmentBooking_VM.PersonHasMasterId == 1)
                {
                    var dbUserLogin = db.UserLogins.Where(u => u.MasterId == requestApartmentBooking_VM.PersonMasterId && u.IsDeleted == 0).FirstOrDefault();
                    if (dbUserLogin != null)
                    {
                        _PersonUserLoginId = dbUserLogin.Id;
                        _PersonGender = dbUserLogin.Gender;
                    }
                    else
                    {
                        apiResponse.status = -1;
                        apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                    //_PersonUserLoginId = 0;
                }

                // Check and veify MasterId of family members if has then get their UserLoginId & Gender for Relation Management
                foreach (var familyMember in requestApartmentBooking_VM.FamilyMemberList)
                {

                    if (familyMember.FamilyMemberHasMasterId == 1)
                    {
                        var dbUserLogin = db.UserLogins.Where(u => u.MasterId == familyMember.FamilyMemberMasterId && u.IsDeleted == 0).FirstOrDefault();
                        if (dbUserLogin != null)
                        {
                            familyMember.FamilyMemberUserLoginId = dbUserLogin.Id;
                            familyMember.Gender = dbUserLogin.Gender;
                        }
                        else
                        {
                            apiResponse.status = -1;
                            apiResponse.message = Resources.ErrorMessage.InvalidMasterId + $"({familyMember.FamilyMemberMasterId})";
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }
                    }
                }

                // For Email if New Account Created
                List<EmailSender_VM> sendEmailList = new List<EmailSender_VM>();

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        if (requestApartmentBooking_VM.PersonHasMasterId == 0)
                        {
                            // Create Student User account
                            StudentService studentService = new StudentService(db);
                            string tempPassword = "student123#";
                            var resp_CreateStudentUser = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                            {
                                Email = requestApartmentBooking_VM.PersonEmail,
                                Password = EDClass.Encrypt(tempPassword),
                                PhoneNumber = requestApartmentBooking_VM.PersonPhoneNumber,
                                PhoneNumberCountryCode = "+91",
                                RoleId = 3,
                                FirstName = requestApartmentBooking_VM.PersonFirstName,
                                LastName = requestApartmentBooking_VM.PersonLastName,
                                ProfileImage = _PersonProfileImageFileNameGenerated,
                                Gender = requestApartmentBooking_VM.PersonGender,
                                Mode = 1
                            });

                            if (resp_CreateStudentUser.ret <= 0)
                            {
                                transaction.Rollback();
                                apiResponse.status = resp_CreateStudentUser.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp_CreateStudentUser.resourceFileName, resp_CreateStudentUser.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            _PersonUserLoginId = resp_CreateStudentUser.Id;
                            _PersonGender = requestApartmentBooking_VM.PersonGender;
                            requestApartmentBooking_VM.PersonMasterId = resp_CreateStudentUser.MasterId;

                            // Save Family Member Picture if passed
                            if (resp_CreateStudentUser != null && resp_CreateStudentUser.ret == 1)
                            {
                                fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_StudentProfileImage, "", _PersonProfileImageFileNameGenerated, _PersonProfileImageFile);
                            }

                            EmailSender_VM sendEmail_Person = new EmailSender_VM()
                            {
                                ReceiverName = requestApartmentBooking_VM.PersonFirstName + " " + requestApartmentBooking_VM.PersonLastName,
                                Subject = "Registration successful",
                                ToEmail = requestApartmentBooking_VM.PersonEmail,
                                MessageBody = $"You have been successfully registered with Masterzone. Your MasterID is: {resp_CreateStudentUser.MasterId} and Your password is {tempPassword}. Please don't forget to change your password after login."
                            };
                            sendEmailList.Add(sendEmail_Person);

                        }

                        // Create Apartment Booking
                        var resp_bookApartment = apartmentService.InsertUpdateApartmentBooking(new ViewModels.StoredProcedureParams.SP_InsertUpdateApartmentBooking_Params_VM()
                        {
                            BusinessOwnerLoginId = _BusinessOwnerLoginId,
                            UserLoginId = _PersonUserLoginId,
                            BatchId = requestApartmentBooking_VM.BatchId,
                            ApartmentId = requestApartmentBooking_VM.ApartmentId,
                            ApartmentBlockId = requestApartmentBooking_VM.ApartmentBlockId,
                            FlatOrVillaNumber = requestApartmentBooking_VM.FlatOrVillaNumber,
                            Phase = requestApartmentBooking_VM.Phase,
                            Lane = requestApartmentBooking_VM.Lane,
                            ApartmentAreaId = requestApartmentBooking_VM.ApartmentAreaId,
                            Activity = requestApartmentBooking_VM.Activity,
                            MasterId = requestApartmentBooking_VM.PersonMasterId,
                            OccupantType = requestApartmentBooking_VM.OccupantType,
                            SubmittedByLoginId = _LoginId,
                            Mode = 1
                        });

                        if (resp_bookApartment.ret <= 0)
                        {
                            transaction.Rollback();
                            apiResponse.status = resp_bookApartment.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp_bookApartment.resourceFileName, resp_bookApartment.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        #region Insert/Update Family Members Relations ----------------------------
                        // insert all passed family members.
                        FamilyMemberService familyMemberService = new FamilyMemberService(db);
                        foreach (var familyMember in requestApartmentBooking_VM.FamilyMemberList)
                        {
                            // Check if MasterId passed
                            if(familyMember.FamilyMemberHasMasterId == 1)
                            {
                                //Already got data for Family Member while checking Master-Id above.
                            }

                            //Else Register Family Member 
                            if (familyMember.FamilyMemberHasMasterId <= 0)
                            {
                                // Create Student User account
                                StudentService studentService = new StudentService(db);
                                string tempPassword = "student123#";
                                var resp_CreateStudentUser = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                                {
                                    Email = familyMember.Email,
                                    Password = EDClass.Encrypt(tempPassword),
                                    PhoneNumber = "",
                                    PhoneNumberCountryCode = "+91",
                                    RoleId = 3,
                                    FirstName = familyMember.FirstName,
                                    LastName = familyMember.LastName,
                                    ProfileImage = familyMember.ProfileImage,
                                    Gender = familyMember.Gender,
                                    Mode = 1
                                });

                                if (resp_CreateStudentUser.ret <= 0)
                                {
                                    transaction.Rollback();
                                    apiResponse.status = resp_CreateStudentUser.ret;
                                    apiResponse.message = ResourcesHelper.GetResourceValue(resp_CreateStudentUser.resourceFileName, resp_CreateStudentUser.resourceKey);
                                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                }

                                familyMember.FamilyMemberUserLoginId = resp_CreateStudentUser.Id;

                                // Save Family Member Picture if passed
                                if (resp_CreateStudentUser != null && resp_CreateStudentUser.ret == 1)
                                {
                                    fileHelper.InsertOrDeleteFileFromServer(StaticResources.FileUploadPath_StudentProfileImage, "", familyMember.ProfileImage, familyMember.ProfileImageFile);
                                }

                                EmailSender_VM sendEmail_Person = new EmailSender_VM()
                                {
                                    ReceiverName = familyMember.FirstName + " " + familyMember.LastName,
                                    Subject = "Registration successful",
                                    ToEmail = familyMember.Email,
                                    MessageBody = $"You have been successfully registered with Masterzone. Your MasterID is: {resp_CreateStudentUser.MasterId} and Your password is {tempPassword}. Please don't forget to change your password after login."
                                };
                                sendEmailList.Add(sendEmail_Person);
                            }

                            // check gender of both  and add relation to the user
                            UserFamilyRelationService userFamilyRelationService = new UserFamilyRelationService(db);
                            var user1RelationWithUser2_RelationTypeKey = userFamilyRelationService.GetReverseFamilyRelationKeyName(_PersonGender, familyMember.FamilyRelationTypeKey);

                            // Insert Family Member Relation
                            var resp_FM = userFamilyRelationService.InsertUserFamilyRelation(new ViewModels.StoredProcedureParams.SP_InsertUpdateUserFamilyRelation_Params_VM()
                            {
                                User1LoginId = _PersonUserLoginId,
                                User2LoginId = familyMember.FamilyMemberUserLoginId,
                                User1Relation_FieldTypeCatalogKey = user1RelationWithUser2_RelationTypeKey,
                                User2Relation_FieldTypeCatalogKey = familyMember.FamilyRelationTypeKey,
                                SubmittedByLoginId = _PersonUserLoginId
                            });

                            
                            if (resp_FM.ret <= 0) {
                                transaction.Rollback();
                                apiResponse.status = -100;
                                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage + " Something wen't wrong while adding Family Members.";
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
                            }
                        }
                        #endregion ------------------------------------------------------------------------

                        db.SaveChanges(); // Save changes to the database

                        transaction.Commit(); // Commit the transaction if everything is successfull

                        if(requestApartmentBooking_VM.PersonHasMasterId == 0)
                        {
                            //// Send Account Created email with credentials
                            EmailSender emailSender = new EmailSender();
                            foreach(var emailData in sendEmailList)
                            {
                                emailSender.Send(emailData.ReceiverName, emailData.Subject, emailData.ToEmail, emailData.MessageBody, "");
                            }
                        }

                        apiResponse.status = 1;
                        apiResponse.message = ResourcesHelper.GetResourceValue(resp_bookApartment.resourceFileName, resp_bookApartment.resourceKey);
                        apiResponse.data = new { };
                        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions and perform error handling or logging
                        transaction.Rollback(); // Roll back the transaction
                        apiResponse.status = -100;
                        apiResponse.message = Resources.ErrorMessage.DatabaseTransactionFailedErrorMessage;
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
                    }
                } // transaction scope ends

            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get All Apartment-Booking data by Business-Owner with Jquery Pagination [for Business-Admin Panel]
        /// </summary>
        /// <returns>List of Apartment Bookings</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetAllApartmentBookingForBOByPagination")]
        public HttpResponseMessage GetAllApartmentBookingForBOByPagination()
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

                ApartmentBooking_Pagination_SQL_Params_VM _Params_VM = new ApartmentBooking_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessOwnerLoginId = _BusinessOwnerLoginId;

                var paginationResponse = apartmentService.GetAllApartmentBookingsByBusiness_Pagination(HttpRequestParams, _Params_VM);

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
        /// Get Apartment Booking Detail [for Business-Admin Panel]
        /// </summary>
        /// <param name="apartmentBookingId">Apartment-Booking-Id</param>
        /// <returns>Apartment Booking Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetApartmentBookingDetail")]
        public HttpResponseMessage GetApartmentBookingDetail(long apartmentBookingId)
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

                ApartmentBookingDetail_VM apartmentBookingDetail_VM = new ApartmentBookingDetail_VM();

                apartmentBookingDetail_VM = apartmentService.GetApartmentBookingDetailById(apartmentBookingId, _BusinessOwnerLoginId); 

                if(apartmentBookingDetail_VM != null)
                {
                    //// Get All Family Members List
                    //FamilyMemberService familyMemberService = new FamilyMemberService(db);
                    //apartmentBookingDetail_VM.FamilyMembers = familyMemberService.GetAllFamilyMembersByUser(apartmentBookingDetail_VM.UserLoginId);
                    
                    // Get All Family Members List
                    UserFamilyRelationService userFamilyRelationService = new UserFamilyRelationService(db);
                    apartmentBookingDetail_VM.FamilyMembers = userFamilyRelationService.GetAllFamilyMembersByUserLoginId(apartmentBookingDetail_VM.UserLoginId);
                }

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = apartmentBookingDetail_VM;

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
        /// Get Apartment Booking Detail by User's Login Id [for Business-Admin Panel]
        /// </summary>
        /// <param name="userLoginId">User-Login-Id</param>
        /// <returns>Apartment Booking Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetApartmentBookingDetailByUser")]
        public HttpResponseMessage GetApartmentBookingDetailByUser(long userLoginId)
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

                List<ApartmentBookingDetail_VM> apartmentBookingDetail_VM = new List<ApartmentBookingDetail_VM>();

                apartmentBookingDetail_VM = apartmentService.GetAllApartmentBookingDetailsByUserLoginId(userLoginId, _BusinessOwnerLoginId);
                
                // Get All Family Members List
                UserFamilyRelationService userFamilyRelationService = new UserFamilyRelationService(db);
                var userFamilyMembers = userFamilyRelationService.GetAllFamilyMembersByUserLoginId(userLoginId);

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new { 
                    personDetails = apartmentBookingDetail_VM,
                    familyMembers =  userFamilyMembers,
                    apartmentBookingList = apartmentBookingDetail_VM 
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
        /// Get All Apartment Booking Users List by Apartment [for Business-Admin Panel]
        /// </summary>
        /// <param name="apartmentId">Apartment-Id</param>
        /// <returns>Apartment Booking Users List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/GetAllApartmentBookingsByApartmentId")]
        public HttpResponseMessage GetAllApartmentBookingsByApartmentId(long apartmentId)
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

                List<ApartmentBookingUserDetail_VM> apartmentBookingUserList_VM = new List<ApartmentBookingUserDetail_VM>();

                apartmentBookingUserList_VM = apartmentService.GetAllApartmentBookingUserListByApartmentId(apartmentId, _BusinessOwnerLoginId);

                List<long> apartmentBookingBlocks = new List<long>();
                List<ApartmentBlock_VM> apartmentBlockList = new List<ApartmentBlock_VM>();
                if (apartmentBookingUserList_VM.Count > 0)
                {
                    apartmentBookingBlocks = apartmentBookingUserList_VM.Select(ab => ab.ApartmentBlockId).Distinct().ToList();
                    //apartmentBlockList = apartmentBookingUserList_VM.Select(ab => new ApartmentBlock_VM { ApartmentId = ab.ApartmentId, Id = ab.ApartmentBlockId, Name = ab.BlockName }).Distinct().ToList();
                    foreach(var blockId in apartmentBookingBlocks)
                    {
                        apartmentBlockList.Add(apartmentBookingUserList_VM.Where(x => x.ApartmentBlockId == blockId).Select(ab => new ApartmentBlock_VM { ApartmentId = ab.ApartmentId, Id = ab.ApartmentBlockId, Name = ab.BlockName }).FirstOrDefault());
                    }
                }

                //--Create response
                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new {
                    apartmentBookingBlocks = apartmentBlockList,
                    apartmentBookingUserList = apartmentBookingUserList_VM
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

        #region Apartment Block CRUD -----------------------------------------------------------

        /// <summary>
        /// Get All Apartment-Block Detail By Id
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>All Apartment Block Detail List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Block/GetAll")]
        public HttpResponseMessage GetAllApartmentBlocksById(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (apartmentId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get All Apartment Blocks
                var resp = apartmentService.GetAllApartmentBlocks(apartmentId);

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
        /// Get Apartment-Block Detail By Id
        /// </summary>
        /// <param name="id">Apartment-Block Id</param>
        /// <returns>Apartment-Block Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Block/GetById")]
        public HttpResponseMessage GetApartmentBlockDetailById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Apartment-Block By Id
                var resp = apartmentService.GetApartmentBlockById(id);

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
        /// Add or Update Apartment-Block
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Block/AddUpdate")]
        public HttpResponseMessage AddUpdateApartmentBlock()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestApartmentBlock_VM requestMenu_VM = new RequestApartmentBlock_VM();
                requestMenu_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestMenu_VM.ApartmentId = Convert.ToInt64(HttpRequest.Params["ApartmentId"]);
                requestMenu_VM.Name = HttpRequest.Params["Name"].Trim();
                requestMenu_VM.Status = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                requestMenu_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = requestMenu_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update Apartment-Block Record
                var resp = apartmentService.InsertUpdateApartmentBlock(new ViewModels.StoredProcedureParams.SP_InsertUpdateApartmentBlock_Params_VM()
                {
                    Id = requestMenu_VM.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    ApartmentId = requestMenu_VM.ApartmentId,
                    Name = requestMenu_VM.Name,
                    IsActive = requestMenu_VM.Status,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestMenu_VM.Mode
                });

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
        /// Delete Apartment-Block By Block-Id
        /// </summary>
        /// <param name="blockId">Block Id</param>
        /// <returns>Status 1 if deleted else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Block/Delete")]
        public HttpResponseMessage DeleteApartmentBlock(long blockId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (blockId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Delete Apartment-Block by Id
                SPResponseViewModel respDeleteMenu = apartmentService.DeleteApartmentBlockById(blockId, _BusinessOwnerLoginId);

                apiResponse.status = respDeleteMenu.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(respDeleteMenu.resourceFileName, respDeleteMenu.resourceKey);
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
        /// Change Apartment-Block Active/Inactive status [automatically swithes to opposite value]
        /// </summary>
        /// <param name="blockId">Block Id</param>
        /// <returns>Api Resoponse: Status 1 if updated else -ve value with error message.</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Block/ChangeStatus")]
        public HttpResponseMessage ChangeApartmentBlockStatus(long blockId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (blockId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Change Apartment-Block Status
                var resp = apartmentService.ChangeApartmentBlockStatusById(blockId, _BusinessOwnerLoginId);

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


        #endregion -------------------------------------------------------------------------------

        #region Apartment Area CRUD -----------------------------------------------------------

        /// <summary>
        /// Get All Apartment-Area Detail By Id
        /// </summary>
        /// <param name="apartmentId">Apartment Id</param>
        /// <returns>All Apartment Area Detail List</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Area/GetAll")]
        public HttpResponseMessage GetAllApartmentAreasById(long apartmentId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (apartmentId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get All Apartment Areas
                var resp = apartmentService.GetAllApartmentAreas(apartmentId);

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
        /// Get Apartment-Area Detail By Id
        /// </summary>
        /// <param name="id">Apartment-Area Id</param>
        /// <returns>Apartment-Area Detail</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Area/GetById")]
        public HttpResponseMessage GetApartmentAreaDetailById(long id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (id <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Apartment-Area By Id
                var resp = apartmentService.GetApartmentAreaById(id);

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
        /// Add or Update Apartment-Area
        /// </summary>
        /// <returns>Status 1 if created else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Area/AddUpdate")]
        public HttpResponseMessage AddUpdateApartmentArea()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();

            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestApartmentArea_VM requestMenu_VM = new RequestApartmentArea_VM();
                requestMenu_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestMenu_VM.ApartmentId = Convert.ToInt64(HttpRequest.Params["ApartmentId"]);
                requestMenu_VM.Name = HttpRequest.Params["Name"].Trim();
                requestMenu_VM.SubTitle = "Static";
                requestMenu_VM.Description = "Static";
                requestMenu_VM.Price = 230;
                requestMenu_VM.Status = Convert.ToInt32(HttpRequest.Params["IsActive"]);
                requestMenu_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = requestMenu_VM.ValidInformation();
                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Insert-Update Apartment-Area Record
                var resp = apartmentService.InsertUpdateApartmentArea(new ViewModels.StoredProcedureParams.SP_InsertUpdateApartmentArea_Params_VM()
                {
                    Id = requestMenu_VM.Id,
                    BusinessOwnerLoginId = _BusinessOwnerLoginId,
                    ApartmentId = requestMenu_VM.ApartmentId,
                    Name = requestMenu_VM.Name,
                    IsActive = requestMenu_VM.Status,
                    SubmittedByLoginId = _LoginId,
                    Mode = requestMenu_VM.Mode
                });

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
        /// Delete Apartment-Area By Area-Id
        /// </summary>
        /// <param name="areaId">Area Id</param>
        /// <returns>Status 1 if deleted else -ve value with error message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Area/Delete")]
        public HttpResponseMessage DeleteApartmentArea(long areaId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (areaId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Delete Apartment-Area by Id
                SPResponseViewModel respDeleteMenu = apartmentService.DeleteApartmentAreaById(areaId, _BusinessOwnerLoginId);

                apiResponse.status = respDeleteMenu.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(respDeleteMenu.resourceFileName, respDeleteMenu.resourceKey);
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
        /// Change Apartment-Area Active/Inactive status [automatically swithes to opposite value]
        /// </summary>
        /// <param name="areaId">Area Id</param>
        /// <returns>Api Resoponse: Status 1 if updated else -ve value with error message.</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Apartment/Area/ChangeStatus")]
        public HttpResponseMessage ChangeApartmentAreaStatus(long areaId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (areaId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Change Apartment-Area Status
                var resp = apartmentService.ChangeApartmentAreaStatusById(areaId, _BusinessOwnerLoginId);

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


        #endregion -------------------------------------------------------------------------------
    }
}