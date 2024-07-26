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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class StudentAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private EmailSender emailSender;
        private StudentService studentService;
        private StudentFavouriteService studentFavouriteService;
        private CertificateService certificateService;

        public StudentAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            emailSender = new EmailSender();
            studentService = new StudentService(db);
            certificateService = new CertificateService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetBasicProfileDetail")]
        public HttpResponseMessage GetBasicProfileDetailByStudent()
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

                BasicProfileDetail_VM basicProfileDetail_VM = new BasicProfileDetail_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;
                //}

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
        [Route("api/Student/GetProfileDetail")]
        public HttpResponseMessage GetProfileDetail()
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

                StudentProfileSetting_VM resp = new StudentProfileSetting_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "2")
                            };

                resp = db.Database.SqlQuery<StudentProfileSetting_VM>("exec  sp_ManageStudent @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;
                //}

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
        [Route("api/Home/Profile/AddUpdateProfile")]
        public HttpResponseMessage AddUpdateStudentProfile()
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

                // --Get User - Login Detail
                //UserLogin _UserLogin = db.UserLogins.Where(ul => ul.Id == _LoginID_Exact).FirstOrDefault();

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                RequestStudentProfile_VM requestStudentProfile_VM = new RequestStudentProfile_VM();
                //requestStudentProfile_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestStudentProfile_VM.FirstName = HttpRequest.Params["FirstName"].Trim();
                requestStudentProfile_VM.LastName = HttpRequest.Params["LastName"].Trim();
                requestStudentProfile_VM.Email = HttpRequest.Params["Email"].Trim();
                requestStudentProfile_VM.PhoneNumber = HttpRequest.Params["PhoneNumber"].Trim();
                // requestStudentProfile_VM.Mode = 1;

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _StudentImageFile = files["ProfileImage"]; // change name
                requestStudentProfile_VM.ProfileImage = _StudentImageFile;

                string _StudentImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousProfileImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestStudentProfile_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Get Business Profile Data for Image Names

                StudentProfileSetting_VM resp = new StudentProfileSetting_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "2")
                            };

                resp = db.Database.SqlQuery<StudentProfileSetting_VM>("exec  sp_ManageStudent @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                if (_StudentImageFile != null && files.Count > 0)
                {
                    _StudentImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_StudentImageFile);

                    _PreviousProfileImageFileName = resp.ProfileImage;
                }
                else
                {
                    _StudentImageFileNameGenerated = resp.ProfileImage;
                }

                studentService = new StudentService(db);
                var resps = studentService.InsertUpdateStudent(new ViewModels.StoredProcedureParams.SP_InsertUpdateStudent_Params_VM()
                {
                    Id = _LoginID_Exact,
                    Email = requestStudentProfile_VM.Email,
                    PhoneNumber = requestStudentProfile_VM.PhoneNumber,
                    PhoneNumberCountryCode = "+91",
                    RoleId = 3,
                    FirstName = requestStudentProfile_VM.FirstName,
                    LastName = requestStudentProfile_VM.LastName,
                    ProfileImage = _StudentImageFileNameGenerated,
                    Mode = 2
                });

                if (resps.ret <= 0)
                {
                    apiResponse.status = resps.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resps.resourceFileName, resps.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                // Update Category Image.
                #region Insert-Update [Business Profile Image] on Server
                if (files.Count > 0)
                {
                    // if business Profile Image passed then Add-Update Profile Image
                    if (_StudentImageFile != null)
                    {
                        if (!String.IsNullOrEmpty(_PreviousProfileImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(""), _PreviousProfileImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string fileWithPathImage = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_StudentProfileImage), _StudentImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_StudentImageFile, fileWithPathImage);
                    }


                }


                #endregion

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
        /// Get All Business Owners List with which Student is linked
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetBusinessOnwers")]
        public HttpResponseMessage GetBusinessOnwersListByStudent()
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

                List<BusinessOnwerList_ForStudent_VM> resp = new List<BusinessOnwerList_ForStudent_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerId", "0"),
                            new SqlParameter("studentId", "0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", "2"),
                            new SqlParameter("searchKeywords", "")
                            };

                resp = db.Database.SqlQuery<BusinessOnwerList_ForStudent_VM>("exec sp_ManageBusinessStudents @id,@businessOwnerId,@studentId,@userLoginId,@mode,@searchKeywords", queryParams).ToList();

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
        /// Add or Remove User(passed user Login Id) to Favourites 
        /// </summary>
        /// <param name="favouriteUserLoginId">User to Favorite User-Login-Id</param>
        /// <returns>Status 1 if added, status 2 if removed</returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Student/AddOrRemoveStudentFavourites")]
        public HttpResponseMessage AddOrRemoveStudentFavourites(long favouriteUserLoginId)
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


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentLoginId",_LoginID_Exact),
                            new SqlParameter("favouriteUserLoginId", favouriteUserLoginId),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = resp.ret;
                apiResponse.message = resp.responseMessage;
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
        /// Get All List For Business Favourite 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllListBusinessFavourites")]
        public HttpResponseMessage GetAllListBusinessFavourites()
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

                List<FavouriteBusiness_VM> favourite_Business_VM = new List<FavouriteBusiness_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentLoginId",_LoginID_Exact),
                            new SqlParameter("favouriteUserLoginId", "0"),
                            new SqlParameter("mode", "3")
                            };

                favourite_Business_VM = db.Database.SqlQuery<FavouriteBusiness_VM>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", queryParams).ToList();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = favourite_Business_VM;
                //}

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
        /// Get All List For Business Favourite 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllListFavouriteBusinessInstructors")]
        public HttpResponseMessage GetAllListFavouriteBusinessInstructors()
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

                List<FavouriteBusiness_VM> favourite_Business_VM = new List<FavouriteBusiness_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentLoginId",_LoginID_Exact),
                            new SqlParameter("favouriteUserLoginId", "0"),
                            new SqlParameter("mode", "8")
                            };

                favourite_Business_VM = db.Database.SqlQuery<FavouriteBusiness_VM>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", queryParams).ToList();

                // Get All user certifications of instructor
                foreach (var instructor in favourite_Business_VM)
                {
                    instructor.Certifications = certificateService.GetAllUserCertificates(instructor.FavouriteUserLoginId);
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = favourite_Business_VM;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        // TODO: Check if AddOrRemove Api can be used instead of this. 2023-06-13
        /// <summary> 
        ///  Business Remove From Favourite list 
        /// </summary>
        /// <param name="favouriteUserLoginId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [Route("api/Student/RemoveBusinessFavourites")]
        public HttpResponseMessage RemoveBusinessFavourites(long favouriteUserLoginId)
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


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentLoginId",_LoginID_Exact),
                            new SqlParameter("favouriteUserLoginId", favouriteUserLoginId),
                            new SqlParameter("mode", "4")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", queryParams).FirstOrDefault();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = Resources.VisitorPanel.RemovedFavourites_ErrorMessage;
                apiResponse.data = resp;
                //}

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
        /// Get All List of Favourite-Business-Instructors only 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllFavouritesInstructorList")]
        public HttpResponseMessage GetAllFavouritesInstructorList()
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

                List<FavouriteInstructor_VM> favourite_Instructor_VM = new List<FavouriteInstructor_VM>();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("studentLoginId",_LoginID_Exact),
                            new SqlParameter("favouriteUserLoginId", "0"),
                            new SqlParameter("mode", "6")
                            };

                favourite_Instructor_VM = db.Database.SqlQuery<FavouriteInstructor_VM>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", queryParams).ToList();
                //return resp;

                //if (resp.Count > 0)
                //{
                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = favourite_Instructor_VM;
                //}

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
        /// To Get Active Course Record By UserloginId In Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllActiveCourse")]
        public HttpResponseMessage GetAllActiveCourseList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<ClassBookingList_VM> response = studentService.GetActiveCourseList(_LoginId, lastRecordId, recordLimit);

                foreach (var business in response)
                {
                    // Encrypting the BusinessOwnerLoginId
                    // string encryptedId = EDClass.Encrypt(business.BusinessOwnerLoginId.ToString());
                    // Assuming there's another property to store the encrypted value
                    business.EncryptedUserLoginId = EDClass.Encrypt(business.BusinessOwnerLoginId.ToString());
                }

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
        /// To Get Ended Course Record By UserloginId In Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllEndedCourse")]
        public HttpResponseMessage GetAllEndedCourseList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<ClassBookingList_VM> response = studentService.GetEndedCourseList(_LoginId, lastRecordId, recordLimit);

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
        /// To Get Ended Course Record By UserloginId In Visitor Panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllUserFavouriteInstructorList")]
        public HttpResponseMessage GetAllFavouriteInstructorList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                studentFavouriteService = new StudentFavouriteService(db);
                List<UserInstructor_VM> response = studentFavouriteService.GetUserFavouriteInstructorList(_LoginId, lastRecordId, recordLimit);

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
        /// To Add or Remove Followers For all Role's
        /// </summary>
        /// <param name="FollowerUserLoginId">User to Follow User-Login-Id</param>
        /// <returns>Status 1 if added, status 2 if removed</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Student,SuperAdmin,SubAdmin,Staff")]
        [Route("api/Student/AddOrRemoveStudentFollowing")]
        public HttpResponseMessage AddOrRemoveStudentFollowing(long FollowerUserLoginId)
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


                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("followerUserLoginId",_LoginID_Exact),
                            new SqlParameter("followingUserLoginId", FollowerUserLoginId),
                            new SqlParameter("mode", "1")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageFollowingUser @id,@followerUserLoginId,@followingUserLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = resp.ret;
                apiResponse.message = resp.responseMessage;
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
        /// Get status detail if Logged in user Follows or Is favourite of other User(passed user id) For all Role's
        /// </summary>
        /// <param name="toUserLoginId"></param>
        /// <returns></returns>
        [HttpGet]
       // [Authorize(Roles = "BusinessAdmin,Student,SuperAdmin,SubAdmin,Staff")]
        [Route("api/User/GetUserFavouriteFollowStatus")]
        public HttpResponseMessage GetUserFavouriteFollowStatus(long toUserLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                //if (validateResponse.ApiResponse_VM.status < 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                //}

                long _LoginID_Exact = validateResponse.UserLoginId;

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("followerUserLoginId",_LoginID_Exact),
                            new SqlParameter("followingUserLoginId", toUserLoginId),
                            new SqlParameter("mode", "2")
                            };

                var isFollowing = db.Database.SqlQuery<int>("exec sp_ManageFollowingUser @id,@followerUserLoginId,@followingUserLoginId,@mode", queryParams).FirstOrDefault();

                SqlParameter[] favourite_queryParams = new SqlParameter[] {
                    new SqlParameter("id", "0"),
                    new SqlParameter("studentLoginId",_LoginID_Exact),
                    new SqlParameter("favouriteUserLoginId", toUserLoginId),
                    new SqlParameter("mode", "7")
                    };

                var isFavourite = db.Database.SqlQuery<int>("exec sp_ManageStudentFavourite @id,@studentLoginId,@favouriteUserLoginId,@mode", favourite_queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    IsFavourite = isFavourite,
                    IsFollowing = isFollowing
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
        /// To Get Business Training Detail By UserLoginId 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetBusinessTrainingDetail")]
        public HttpResponseMessage GetBusinessTrainingDetail()
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


                List<StudentBusinessTrainingDetail_VM> studentBusinessTrainingDetail = studentService.GetStudentBusinessDetail(_LoginID_Exact);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = studentBusinessTrainingDetail;



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
        /// To Get Business  Detail By BusinessLoginId 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessLoginId"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetBusinessTrainingDetailById")]
        public HttpResponseMessage GetBusinessDetailById(long Id)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }
                long BusinessLoginId = 0;
                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                if (Id <= 0)
                {
                    StudentBusinessTrainingDetail_VM data = studentService.GetStudentBusinessDetail(_LoginID_Exact).FirstOrDefault();
                    BusinessLoginId = data.UserLoginId;
                }
                else
                {
                    BusinessLoginId = Id;
                }
                StudentBusinessTrainingDetail_VM details = new StudentBusinessTrainingDetail_VM();

                details = studentService.GetStudentBusinessDetailByBusinessId(_LoginID_Exact, BusinessLoginId);
                List<StudentTrainingDetail_VM> studentBusinessTrainingDetail = studentService.GetStudentBusinessTrainingDetail(_LoginID_Exact, BusinessLoginId);
                StudentTrainingDetail_VM studentRandomlyTrainingDetail = new StudentTrainingDetail_VM();

                studentRandomlyTrainingDetail = studentService.GetTrainingDetailrandomly(BusinessLoginId);

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    StudentTrainingDetails = studentBusinessTrainingDetail,
                    details = details,
                    RandomlyStudentTrainingDetail = studentRandomlyTrainingDetail,
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
        /// Get User Family Members by its Family Members, if Master Id exists
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns>Check Master Id and Return Family Members</returns>
        [HttpGet]
        [Authorize(Roles = "Student,BusinessAdmin,Staff")]
        [Route("api/Student/GetUserFamilyMembersByMasterId")]
        public HttpResponseMessage GetUserFamilyMembersByMasterId(string masterId)
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
                List<UserFamilyMemberRelation_VM> familyMembers = new List<UserFamilyMemberRelation_VM>();
                int IsMasterIdExists = 0;

                var dbUserLogin = db.UserLogins.Where(u => u.MasterId == masterId && u.IsDeleted == 0).FirstOrDefault();
                if (dbUserLogin != null)
                {
                    IsMasterIdExists = 1;
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                    UserFamilyRelationService userFamilyRelationService = new UserFamilyRelationService(db);
                    familyMembers = userFamilyRelationService.GetAllFamilyMembersByUserMasterId(masterId);
                }
                else
                {
                    IsMasterIdExists = 0;
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                }

                apiResponse.data = familyMembers;

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
        /// Get User Basic Data by its masterId, if Master Id exists
        /// </summary>
        /// <param name="masterId">Student Master-Id</param>
        /// <returns>User Data</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Student/GetUserBasicDataByMasterId")]
        public HttpResponseMessage GetUserBasicDataByMasterId(string masterId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                if (string.IsNullOrEmpty(masterId))
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                int IsMasterIdExists = 0;

                var studentDetail = studentService.GetStudentBasicDetailDataByMasterId(masterId);
                if (studentDetail != null)
                {
                    IsMasterIdExists = 1;
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                }
                else
                {
                    IsMasterIdExists = 0;
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                }

                apiResponse.data = studentDetail;

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
        /// Get User Data by userLoginId
        /// </summary>
        /// <param name="userLoginId">User-Login-Id</param>
        /// <returns>User Data</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Student/GetUserBasicDataById")]
        public HttpResponseMessage GetUserBasicDataById(long userLoginId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                if (userLoginId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidIdErrorMessage;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                long _LoginId = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                var studentDetail = studentService.GetStudentBasicDetailDataByLoginId(userLoginId);
                if (studentDetail != null)
                {
                    apiResponse.status = 1;
                    apiResponse.message = "success";
                }
                else
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidMasterId;
                }

                apiResponse.data = studentDetail;

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
        /// get plan date for Show in student panel side bar page 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetPlanDetailsForDate")]
        public HttpResponseMessage GetPlanDetailsForDate()
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

                List<PlanBooking_ViewModel> response = studentService.GetPlanDetailForDate(_LoginId);

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
        /// To get training list for student panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllActiveTrainingList")]
        public HttpResponseMessage GetAllActiveTrainingList(long lastRecordId, int recordLimit = StaticResources.RecordLimitTraining_Default)
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

                List<AvailableTraining_VM> response = studentService.GetActiveTrainingList(_LoginId, lastRecordId, recordLimit);

                //foreach (var business in response)
                //{
                //    // Encrypting the BusinessOwnerLoginId
                //    // string encryptedId = EDClass.Encrypt(business.BusinessOwnerLoginId.ToString());
                //    // Assuming there's another property to store the encrypted value
                //    business.EncryptedUserLoginId = EDClass.Encrypt(business.BusinessOwnerLoginId.ToString());
                //}

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
        ///  to get endend training list
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllEndedTrainingList")]
        public HttpResponseMessage GetAllEndedTrainingList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<AvailableTraining_VM> response = studentService.GetEndedTrainingList(_LoginId, lastRecordId, recordLimit);

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
        /// To get event list for student panel
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllActiveEventList")]
        public HttpResponseMessage GetAllActiveEventList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<EventBookingDetail_VM> response = studentService.GetActiveEventList(_LoginId, lastRecordId, recordLimit);
                List<EventBookingDetail_VM> eventguestDetails = studentService.GetEventDetailsGuestList(_LoginId, lastRecordId, recordLimit);
                List<EventBookingDetail_VM> eventsponserDetails = studentService.GetEventSponsersList(_LoginId, lastRecordId, recordLimit);
                EventBookingDetail_VM eventCountTicketDetails = studentService.GetEventTicketCount(_LoginId);

                //foreach (var business in response)
                //{
                //    // Encrypting the BusinessOwnerLoginId
                //    // string encryptedId = EDClass.Encrypt(business.BusinessOwnerLoginId.ToString());
                //    // Assuming there's another property to store the encrypted value
                //    business.EncryptedUserLoginId = EDClass.Encrypt(business.UserLoginId.ToString());
                //}

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new
                {
                    Eventdetails = response,
                    EventGuestDetails = eventguestDetails,
                    EventSponserDetails = eventsponserDetails,
                    EventCountTicket = eventCountTicketDetails

                };

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
        ///  to get endend event list
        /// </summary>
        /// <param name="lastRecordId"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetAllEndedEventList")]
        public HttpResponseMessage GetAllEndedEventList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<EventBookingDetail_VM> response = studentService.GetEndendEventList(_LoginId, lastRecordId, recordLimit);

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
        /// only to get masterId for showing in form (sport-booking)
        /// </summary>
        /// <param name="classDays"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Student/GetMasterIdForSportBooking")]
        public HttpResponseMessage GetMasterIdForSportBooking()
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

                MainPlan_VM resp = new MainPlan_VM();

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("userLoginId", _LoginId),
                            new SqlParameter("mode", 7)
                            };

                resp = db.Database.SqlQuery<MainPlan_VM>("exec sp_ManageMainPlans @id,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;


                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
        }


    }
}