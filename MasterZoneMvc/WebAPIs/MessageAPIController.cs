using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
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
    public class MessageAPIController : ApiController
    {
        private MasterZoneDbContext db;
        public MessageAPIController()
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
        /// Add  Message
        /// </summary>
        /// <returns>Success or Error Message</returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,Student")]
        [Route("api/Message/AddUpdate")]
        public HttpResponseMessage AddUpdateMessge(long toUserLoginId)
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

                long SenderUserLoginId = 0;
                long ReceiverUserLoginId = toUserLoginId;

                if (validateResponse.UserRoleName == "BusinessAdmin" || validateResponse.UserRoleName == "Staff")
                {
                    SenderUserLoginId = _BusinessOwnerLoginId;
                }
                else if (validateResponse.UserRoleName == "Student")
                {
                    SenderUserLoginId = _LoginID_Exact;
                }

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                MessageChat_VM messageChat_VM = new MessageChat_VM();
                messageChat_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                messageChat_VM.SenderUserloginId = Convert.ToInt64(HttpRequest.Params["SenderUserLoginId"]);
                messageChat_VM.ReceiverUserLoginId = Convert.ToInt64(HttpRequest.Params["ReceiverUserLoginId"]);
                messageChat_VM.Messagebody = HttpRequest.Params["Messagebody"].Trim();
                messageChat_VM.SenderStatus = Convert.ToInt32(HttpRequest.Params["SenderStatus"]);
                messageChat_VM.ReceiverStatus = Convert.ToInt32(HttpRequest.Params["ReceiverStatus"]);
                messageChat_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);

                // Validate infromation passed
                Error_VM error_VM = messageChat_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("senderUserLoginId", SenderUserLoginId),
                            new SqlParameter("receiverUserLoginId",ReceiverUserLoginId),
                            new SqlParameter("messagebody", messageChat_VM.Messagebody),
                            new SqlParameter("senderStatus", messageChat_VM.SenderStatus),
                            new SqlParameter("receiverStatus",messageChat_VM.ReceiverStatus),
                            new SqlParameter("submittedByLoginId","1"),
                            new SqlParameter("mode", messageChat_VM.Mode),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateMessage @id,@senderUserLoginId,@receiverUserLoginId,@messagebody,@senderStatus,@receiverStatus,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = resp.responseMessage;
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
        /// Get Message  Detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,Student")]
        [Route("api/Message/MessageChatGetById")]
        public HttpResponseMessage GetById(long id)
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
                
                long SenderUserLoginId = 0;
                long ReceiverUserLoginId = id;
                if (validateResponse.UserRoleName == "BusinessAdmin" || validateResponse.UserRoleName == "Staff")
                {
                    SenderUserLoginId = _BusinessOwnerLoginId;
                }
                else if (validateResponse.UserRoleName == "Student")
                {
                    SenderUserLoginId = _LoginID_Exact;
                }

                // Get Message-Detail-By-BussinessUserLoginId
                List<Message_VM> messageUserDetail = new List<Message_VM>();
                SqlParameter[] queryParam = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("toUserLoginId", ReceiverUserLoginId),
                            new SqlParameter("userLoginId", SenderUserLoginId),
                            new SqlParameter("mode", "1")
                            };

                messageUserDetail = db.Database.SqlQuery<Message_VM>("exec sp_ManageMessage @id,@toUserLoginId,@userLoginId,@mode", queryParam).ToList();


                // Get Sender-User-Login-Detail
                StudentMessage_VM senderUserLogin = new StudentMessage_VM();
                SqlParameter[] queryParam1 = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("userLoginId", SenderUserLoginId),
                            new SqlParameter("mode", "2")
                            };

                senderUserLogin = db.Database.SqlQuery<StudentMessage_VM>("exec sp_ManageMessage @id,@toUserLoginId,@userLoginId,@mode", queryParam1).FirstOrDefault();

                // Get Receiver-User-Login-Id
                StudentMessage_VM recevierUserLogin = new StudentMessage_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("userLoginId", ReceiverUserLoginId),
                            new SqlParameter("mode", "2")
                            };

                recevierUserLogin = db.Database.SqlQuery<StudentMessage_VM>("exec sp_ManageMessage @id,@toUserLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    SenderUserLoginDetail = senderUserLogin,
                    RecevierUserLoginDetail = recevierUserLogin,
                    MessageList = messageUserDetail
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
        /// Get ALL Student  Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Message/StudentListGetById")]
        public HttpResponseMessage GetStudentsById()
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

                // Get Message-Detail-By-BussinessUserLoginId
                List<StudentDetailForMessage_VM> studentDetail = new List<StudentDetailForMessage_VM>();
                SqlParameter[] queryParam = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("toUserLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "4")
                            };

                studentDetail = db.Database.SqlQuery<StudentDetailForMessage_VM>("exec sp_ManageMessage @id,@toUserLoginId,@userLoginId,@mode", queryParam).ToList();

                foreach (var student in studentDetail)
                {
                    student.EncryptedUserLoginId = EDClass.Encrypt(student.UserLoginId.ToString());
                }

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data =
                    new
                    {

                        StudentDetail = studentDetail
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
        [Authorize(Roles = "BusinessAdmin,Staff,Student")]
        [Route("api/Message/MarkAsReadUnread")]
        public HttpResponseMessage ChangeStatusMarkAsRead(RequestMessageReadUnread_VM requestMessageReadUnread_VM)
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

                long SenderUserLoginId = 0;
                if (validateResponse.UserRoleName == "BusinessAdmin" || validateResponse.UserRoleName == "Staff")
                {
                    SenderUserLoginId = _BusinessOwnerLoginId;
                }
                else if (validateResponse.UserRoleName == "Student")
                {
                    SenderUserLoginId = _LoginID_Exact;
                }

                SqlParameter[] queryParam = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("toUserLoginId", requestMessageReadUnread_VM.Id ),
                            new SqlParameter("userLoginId", SenderUserLoginId),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_ManageMessage @id,@toUserLoginId,@userLoginId,@mode", queryParam).FirstOrDefault();

                apiResponse.status = 1;
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
        /// Get ALL Business  Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Message/BusinessListGetById")]
        public HttpResponseMessage GetAllBusinessListById()
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

                // Get Message-Detail-By-BussinessUserLoginId
                List<BusinessDetailForMessages_VM> businessDetail = new List<BusinessDetailForMessages_VM>();
                SqlParameter[] queryParam = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                             new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "5")
                            };

                businessDetail = db.Database.SqlQuery<BusinessDetailForMessages_VM>("exec sp_ManageMessage @id,@businessOwnerLoginId,@userLoginId,@mode", queryParam).ToList();

                foreach (var business in businessDetail)
                {
                    business.EncryptedUserLoginId = EDClass.Encrypt(business.UserLoginId.ToString());
                }

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data =
                    new
                    {
                        BusinessDetail = businessDetail
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
        /// to add or update group messages 
        /// </summary>
        /// <param name="toUserLoginId"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,Student")]
        [Route("api/Message/AddUpdateGroupMessge")]
        public HttpResponseMessage AddUpdateGroupMessge(long toUserLoginId)
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

                long SenderUserLoginId = 0;
                long ReceiverUserLoginId = toUserLoginId;

                if (validateResponse.UserRoleName == "BusinessAdmin" || validateResponse.UserRoleName == "Staff")
                {
                    SenderUserLoginId = _BusinessOwnerLoginId;
                }
                else if (validateResponse.UserRoleName == "Student")
                {
                    SenderUserLoginId = _LoginID_Exact;
                }

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                GroupMessageChat_VM messageChat_VM = new GroupMessageChat_VM();
                messageChat_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                messageChat_VM.SenderUserloginId = Convert.ToInt64(HttpRequest.Params["SenderUserLoginId"]);
                messageChat_VM.ReceiverUserLoginId = Convert.ToInt64(HttpRequest.Params["ReceiverUserLoginId"]);
                messageChat_VM.GroupId = Convert.ToInt64(HttpRequest.Params["ReceiverUserLoginId"]);
                messageChat_VM.Messagebody = HttpRequest.Params["Messagebody"].Trim();
                messageChat_VM.SenderStatus = Convert.ToInt32(HttpRequest.Params["SenderStatus"]);
                messageChat_VM.ReceiverStatus = Convert.ToInt32(HttpRequest.Params["ReceiverStatus"]);
                messageChat_VM.Mode = Convert.ToInt32(HttpRequest.Params["mode"]);

                // Validate infromation passed
                Error_VM error_VM = messageChat_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("senderUserLoginId", SenderUserLoginId),
                            new SqlParameter("receiverUserLoginId",ReceiverUserLoginId),
                            new SqlParameter("messagebody", messageChat_VM.Messagebody),
                            new SqlParameter("senderStatus", messageChat_VM.SenderStatus),
                            new SqlParameter("receiverStatus",messageChat_VM.ReceiverStatus),
                            new SqlParameter("submittedByLoginId","1"),
                            new SqlParameter("mode", messageChat_VM.Mode),
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateGroupMessage @id,@senderUserLoginId,@receiverUserLoginId,@messagebody,@senderStatus,@receiverStatus,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = resp.responseMessage;
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
        ///  to get group list group chat message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Student")]
        [Route("api/Message/GetAllGroupList")]
        public HttpResponseMessage GetAllGroupList(string searchname)
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

                if(!string.IsNullOrEmpty(searchname))
                {
                    List<GroupDetailForMessages_VM> groupDetails = new List<GroupDetailForMessages_VM>();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                             new SqlParameter("toUserLoginId", _BusinessOwnerLoginId),
                             new SqlParameter("searchName", searchname),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "8")
                            };

                    groupDetails = db.Database.SqlQuery<GroupDetailForMessages_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParams).ToList();

                    foreach (var business in groupDetails)
                    {
                        business.EncryptedUserLoginId = EDClass.Encrypt(business.GroupId.ToString());
                    }
                    apiResponse.status = 1;
                    apiResponse.message = Resources.BusinessPanel.Success;
                    apiResponse.data =
                        new
                        {
                            GroupDetails = groupDetails,

                        };
                }
                else
                {
                    // Get Message-Detail-By-BussinessUserLoginId
                    List<GroupDetailForMessages_VM> groupDetail = new List<GroupDetailForMessages_VM>();
                    SqlParameter[] queryParam = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                             new SqlParameter("toUserLoginId", _BusinessOwnerLoginId),
                             new SqlParameter("searchName", "0"),
                            new SqlParameter("userLoginId", _LoginID_Exact),
                            new SqlParameter("mode", "1")
                            };

                    groupDetail = db.Database.SqlQuery<GroupDetailForMessages_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParam).ToList();
                   
                    foreach (var business in groupDetail)
                    {
                        business.EncryptedUserLoginId = EDClass.Encrypt(business.GroupId.ToString());
                    }
                    apiResponse.status = 1;
                    apiResponse.message = Resources.BusinessPanel.Success;
                    apiResponse.data =
                        new
                        {
                            GroupDetail = groupDetail,

                        };
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
        /// to get group details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

       
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,Student")]
        [Route("api/Message/GetGroupDetailsById")]
        public HttpResponseMessage GetGroupDetailsById(long id)
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

                long SenderUserLoginId = 0;
                long ReceiverUserLoginId = id;
                if (validateResponse.UserRoleName == "BusinessAdmin" || validateResponse.UserRoleName == "Staff")
                {
                    SenderUserLoginId = _BusinessOwnerLoginId;
                }
                else if (validateResponse.UserRoleName == "Student")
                {
                    SenderUserLoginId = _LoginID_Exact;
                }

                // Get Message-Detail-By-BussinessUserLoginId
                List<GroupDetailForMessages_VM> messageUserDetail = new List<GroupDetailForMessages_VM>();
                SqlParameter[] queryParam = new SqlParameter[] {
                            new SqlParameter("id","0"),
                            new SqlParameter("toUserLoginId", ReceiverUserLoginId),
                            new SqlParameter("searchName", "0"),
                            new SqlParameter("userLoginId", SenderUserLoginId),
                            new SqlParameter("mode", "7")
                            };

                messageUserDetail = db.Database.SqlQuery<GroupDetailForMessages_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParam).ToList();


                // Get Sender-User-Login-Detail
                GroupMessage_VM senderUserLogin = new GroupMessage_VM();
                SqlParameter[] queryParam1 = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("searchName", "0"),
                            new SqlParameter("userLoginId", SenderUserLoginId),
                            new SqlParameter("mode", "4")
                            };

                senderUserLogin = db.Database.SqlQuery<GroupMessage_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParam1).FirstOrDefault();

                // Get Receiver-User-Login-Id
                GroupMessage_VM recevierUserLogin = new GroupMessage_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("searchName", "0"),
                            new SqlParameter("userLoginId", ReceiverUserLoginId),
                            new SqlParameter("mode", "4")
                            };

                recevierUserLogin = db.Database.SqlQuery<GroupMessage_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParams).FirstOrDefault();

                // to get count of student 
                GroupMessage_VM countstudentgroup = new GroupMessage_VM();
                SqlParameter[] queryParamss = new SqlParameter[] {
                            new SqlParameter("id", ReceiverUserLoginId),
                            new SqlParameter("toUserLoginId", "0"),
                            new SqlParameter("searchName", "0"),
                            new SqlParameter("userLoginId", "0"),
                            new SqlParameter("mode", "9")
                            };

                countstudentgroup = db.Database.SqlQuery<GroupMessage_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParamss).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    SenderUserLoginDetail = senderUserLogin,
                    RecevierUserLoginDetail = recevierUserLogin,
                    CountStudentGroup = countstudentgroup,
                    MessageList = messageUserDetail
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
        /// to get message 0f group chat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Message/GetBusinessGroupDetailsById")]
        public HttpResponseMessage GetBusinessGroupDetailsById(long id)
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

                long SenderUserLoginId = 0;
                long ReceiverUserLoginId = id;
                if (validateResponse.UserRoleName == "BusinessAdmin" || validateResponse.UserRoleName == "Staff")
                {
                    SenderUserLoginId = _BusinessOwnerLoginId;
                }
                else if (validateResponse.UserRoleName == "Student")
                {
                    SenderUserLoginId = _LoginID_Exact;
                }

                // Get Message-Detail-By-BussinessUserLoginId
                List<GroupDetailForMessages_VM> messageUserDetail = new List<GroupDetailForMessages_VM>();
                SqlParameter[] queryParam = new SqlParameter[] {
                    new SqlParameter("id","0"),
                    new SqlParameter("toUserLoginId", ReceiverUserLoginId),
                    new SqlParameter("searchName", "0"),
                    new SqlParameter("userLoginId", SenderUserLoginId),
                    new SqlParameter("mode", "6")
                    };

                messageUserDetail = db.Database.SqlQuery<GroupDetailForMessages_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParam).ToList();


                // Get Sender-User-Login-Detail
                GroupMessage_VM senderUserLogin = new GroupMessage_VM();
                SqlParameter[] queryParam1 = new SqlParameter[] {
                    new SqlParameter("id", "0"),
                    new SqlParameter("toUserLoginId", "0"),
                    new SqlParameter("searchName", "0"),
                    new SqlParameter("userLoginId", SenderUserLoginId),
                    new SqlParameter("mode", "4")
                    };

                senderUserLogin = db.Database.SqlQuery<GroupMessage_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParam1).FirstOrDefault();

                // Get Receiver-User-Login-Id
                GroupMessage_VM recevierUserLogin = new GroupMessage_VM();
                SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", "0"),
                    new SqlParameter("toUserLoginId", "0"),
                    new SqlParameter("searchName", "0"),
                    new SqlParameter("userLoginId", ReceiverUserLoginId),
                    new SqlParameter("mode", "4")
                    };

                recevierUserLogin = db.Database.SqlQuery<GroupMessage_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParams).FirstOrDefault();

                // to get count of student 
                GroupMessage_VM countstudentgroup = new GroupMessage_VM();
                SqlParameter[] queryParamss = new SqlParameter[] {
                    new SqlParameter("id", ReceiverUserLoginId),
                    new SqlParameter("toUserLoginId", "0"),
                    new SqlParameter("searchName", "0"),
                    new SqlParameter("userLoginId", "0"),
                    new SqlParameter("mode", "9")
                    };

                countstudentgroup = db.Database.SqlQuery<GroupMessage_VM>("exec sp_ManageGroupMessage @id,@toUserLoginId,@searchName,@userLoginId,@mode", queryParamss).FirstOrDefault();

                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    SenderUserLoginDetail = senderUserLogin,
                    RecevierUserLoginDetail = recevierUserLogin,
                    MessageList = messageUserDetail,
                    CountStudentGroup = countstudentgroup,
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