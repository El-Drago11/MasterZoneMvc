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
    public class GroupAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private GroupService groupService;
        private NotificationService notificationService;

        public GroupAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            groupService = new GroupService(db);
            notificationService = new NotificationService(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Add Or Update Group - Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/AddUpdate")]
        public HttpResponseMessage AddUpdateGroup()
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
                RequestGroupViewModel requestGroup_VM = new RequestGroupViewModel();
                requestGroup_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                requestGroup_VM.BusinessOwnerLoginId = 0;
                requestGroup_VM.Name = HttpRequest.Params["Name"].Trim();
                requestGroup_VM.Description = HttpRequest.Params["Description"].Trim();
                requestGroup_VM.GroupType = HttpRequest.Params["GroupType"].Trim();
                requestGroup_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Get Attatched Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _GroupImageFile = files["GroupImage"];
                requestGroup_VM.ProfileImage = _GroupImageFile; // for validation
                string _GroupImageFileNameGenerated = ""; //will contains generated file name
                string _PreviousGroupImageFileName = ""; // will be used to delete file while updating.

                // Validate infromation passed
                Error_VM error_VM = requestGroup_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                if (files.Count > 0)
                {
                    if (_GroupImageFile != null)
                    {
                        _GroupImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_GroupImageFile);
                    }
                }

                // if Edit Mode
                if (requestGroup_VM.Mode == 2)
                {
                    // Get Group By Id
                    var respGetGroup = groupService.GetGroupById(requestGroup_VM.Id);
                    // if no image update then update name of previous image else need to delete previous image
                    if (files.Count <= 0)
                    {
                        _GroupImageFileNameGenerated = respGetGroup.GroupImage;
                    }
                    else
                    {
                        _PreviousGroupImageFileName = respGetGroup.GroupImage;
                    }
                }

                // Insert-Update Group Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", requestGroup_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("name", requestGroup_VM.Name),
                            new SqlParameter("description", requestGroup_VM.Description),
                            new SqlParameter("groupType", requestGroup_VM.GroupType),
                            new SqlParameter("groupImage", _GroupImageFileNameGenerated),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", requestGroup_VM.Mode)
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateGroup @id,@businessOwnerLoginId,@name,@description,@groupType,@groupImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Group Image.
                    #region Insert-Update Group Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousGroupImageFileName))
                        {
                            // remove previous file
                            string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_GroupImage), _PreviousGroupImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveFileWithPath);
                        }

                        // save new file
                        string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_GroupImage), _GroupImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_GroupImageFile, FileWithPath);
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
        /// Get All Groups with Pagination For the Business-Panel [Admin, Staff]
        /// <br/>For Jquery Data-Table Pagination
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/ByBusiness/GetAllByPagination")]
        [HttpPost]
        public HttpResponseMessage GetAllGroupsByBusinessOwnerForDataTablePagination()
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

                GroupList_Pagination_SQL_Params_VM _Params_VM = new GroupList_Pagination_SQL_Params_VM()
                {
                    JqueryDataTableParams = new JqueryDataTableParamsViewModel()
                };

                var HttpRequestParams = HttpContext.Current.Request.Params;

                // Set data
                _Params_VM.Mode = 1;
                _Params_VM.LoginId = _LoginId;
                _Params_VM.BusinessAdminLoginId = _BusinessOwnerLoginId;

                var paginationResponse = groupService.GetGoupsListByBusinessOwner_Pagination(HttpRequestParams, _Params_VM);


                
                //--Create response
                var objResponse = new
                {
                    status = 1,
                    message = "Success",
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
        /// Get Group Data by Group-Id
        /// </summary>
        /// <param name="id">Group Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/GetById")]
        public HttpResponseMessage GetGroupById(long id)
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
                Group_VM group_VM = new Group_VM();

                // Get Group By Id
                group_VM = groupService.GetGroupById(id);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = group_VM;

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
        /// Delete Group Created by Business-Owner
        /// </summary>
        /// <param name="id">Group Id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/Delete")]
        public HttpResponseMessage DeleteGroupById(long id)
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

                // Insert-Update Group Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", id),
                            new SqlParameter("businessOwnerLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("name", ""),
                            new SqlParameter("description", ""),
                            new SqlParameter("groupType", "0"),
                            new SqlParameter("groupImage", ""),
                            new SqlParameter("submittedByLoginId", _LoginId),
                            new SqlParameter("mode", "3")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateGroup @id,@businessOwnerLoginId,@name,@description,@groupType,@groupImage,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

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
        /// Add Member to the Group - Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/AddMember")]
        public HttpResponseMessage AddMemberInGroup(RequestGroupMember_VM requestGroupMember_VM)
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

                //// Insert-Update Group Information
                //SqlParameter[] queryParams = new SqlParameter[] {
                //            new SqlParameter("id", "0"),
                //            new SqlParameter("groupId", requestGroupMember_VM.GroupId),
                //            new SqlParameter("memberLoginId", requestGroupMember_VM.MemberLoginId),
                //            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                //            new SqlParameter("mode", "1")
                //            };

                //var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateGroupMember @id,@groupId,@memberLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                //// Send notification to user if successfully removed
                //if (resp.ret == 1)
                //{
                //    var group_VM = groupService.GetGroupById(requestGroupMember_VM.GroupId);
                //    var notificationResponse = notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                //    {
                //        FromUserLoginId = _BusinessOwnerLoginId,
                //        NotificationText = "You have been added in the group '" + group_VM.Name + "'",
                //        NotificationTitle = "Added in Group - " + group_VM.Name,
                //        NotificationType = "Custom",
                //        NotificationUsersList = requestGroupMember_VM.MemberLoginId.ToString(),
                //        Mode = 1
                //    });
                //}

                var resp = groupService.AddMemberInGroup(new AddGroupMember_VM
                {
                    GroupId = requestGroupMember_VM.GroupId,
                    GroupUserLoginId = _BusinessOwnerLoginId,
                    MemberLoginId = requestGroupMember_VM.MemberLoginId
                });

                // send success response
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
        /// Remove Member from the Group - Business Panel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/RemoveMember")]
        public HttpResponseMessage RemoveMemberFromGroup(RequestGroupMember_VM requestGroupMember_VM)
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

                //// Validate infromation passed
                //Error_VM error_VM = requestGroupMember_VM.ValidInformation();

                //if (!error_VM.Valid)
                //{
                //    apiResponse.status = -1;
                //    apiResponse.message = error_VM.Message;
                //    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                //}

                // Insert-Update Group Information
                SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("groupId", requestGroupMember_VM.GroupId),
                            new SqlParameter("memberLoginId", requestGroupMember_VM.MemberLoginId),
                            new SqlParameter("userLoginId", _BusinessOwnerLoginId),
                            new SqlParameter("mode", "2")
                            };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateGroupMember @id,@groupId,@memberLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

                // Send notification to user if successfully removed
                if (resp.ret == 1)
                {
                    var group_VM = groupService.GetGroupById(requestGroupMember_VM.GroupId);
                    var notificationResponse = notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        FromUserLoginId = _BusinessOwnerLoginId,
                        NotificationText = "You have been removed from the group '" + group_VM.Name + "'",
                        NotificationTitle = "Removed from Group - " + group_VM.Name,
                        NotificationType = "Custom",
                        NotificationUsersList = requestGroupMember_VM.MemberLoginId.ToString(),
                        Mode = 1
                    });
                }

                // send success response
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
        /// Get Group Detials With Members(students) Data by Group-Id
        /// </summary>
        /// <param name="id">Group Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/GetGroupDetialsWithMembers")]
        public HttpResponseMessage GetGroupDetailWithMembersById(long id)
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

                Group_VM group_VM = new Group_VM();
                List<StudentList_ForBusiness_VM> studentList = new List<StudentList_ForBusiness_VM>();

                //// Get Group By Id
                group_VM = groupService.GetGroupById(id);
                if (group_VM != null)
                {
                    // Get Group-Member Details List By Group-Id
                    group_VM.GroupStudents = groupService.GetGroupMemberListByGroupId(id, _BusinessOwnerLoginId);
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = group_VM;

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
        /// Get All Group Names with their Group-Id
        /// </summary>
        /// <returns>List of Group-Names with Group-Id</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/GetAllGroupNamesList")]
        public HttpResponseMessage GetAllGroupNamesList(string groupType)
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
                long _BusinessAdminLoginId = validateResponse.BusinessAdminLoginId;

                List<GroupNameList_VM> groupNameList_VM = new List<GroupNameList_VM>();

                // Get Group By Id
                groupNameList_VM = groupService.GetAllGroupNamesListByBusiness(_BusinessAdminLoginId, groupType);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = groupNameList_VM;

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
        /// Get All Group Names with their Group-Id which are not assigned to any batch(Class-Batch) except the passed Batch.
        /// </summary>
        /// <param name="batchId">Batch-Id for which group must be included in list</param>
        /// <returns>List of Group-Names with Group-Id</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/GetAllUnassignedBatchGroupNamesList")]
        public HttpResponseMessage GetAllUnassignedBatchGroupNamesList(long batchId)
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
                long _BusinessAdminLoginId = validateResponse.BusinessAdminLoginId;

                List<GroupNameList_VM> groupNameList_VM = new List<GroupNameList_VM>();

                // Get Group By Id
                groupNameList_VM = groupService.GetAllUnassignedBatchGroupNamesListByBusiness(_BusinessAdminLoginId, batchId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = groupNameList_VM;

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
        /// To Get Groups Record By UserloginId In Visitor Panel, Mobile App
        /// Class Groups with Busienss and Instructor details of student
        /// </summary>
        /// <param name="lastRecordId">Last fetched record Id</param>
        /// <param name="recordLimit">no. of records to fetch</param>
        /// <returns>All Student-Groups List</returns>
        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("api/Group/GetAllStudentUserGroup")]
        public HttpResponseMessage GetAllGroupList(long lastRecordId, int recordLimit = StaticResources.RecordLimit_Default)
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

                List<UserClassGroupList_VM> response = groupService.GetAllGroupList(_LoginId, lastRecordId, recordLimit);

                List<UserClassGroupBusinessList_VM> userClassGroupByBusiness = new List<UserClassGroupBusinessList_VM>();

                userClassGroupByBusiness = response.GroupBy(x => x.BusinessOwnerLoginId).Select(g => new UserClassGroupBusinessList_VM() { BusinessOwnerId = g.First().BusinessOwnerId, BusinessOwnerLoginId = g.First().BusinessOwnerLoginId, BusinessName = g.First().BusinessName, BusinessLogoWithPath = g.First().BusinessLogoWithPath }).ToList();

                foreach (var business in userClassGroupByBusiness)
                {
                    business.UserClassGroupList = response.Where(g => g.BusinessOwnerLoginId == business.BusinessOwnerLoginId).ToList();
                    business.TotalGroupCount = business.UserClassGroupList.Count;
                }

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = userClassGroupByBusiness;

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
        /// Get All Business Members (Staff and Students) list for Group.
        /// </summary>
        /// <returns>List of Business(Staff & Students) Members</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/GetAllMembersListForGroup")]
        public HttpResponseMessage GetAllMembersListForGroup()
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

                List<GroupMember_ForBusiness_VM> memberList = new List<GroupMember_ForBusiness_VM>();

                // Get Group-Member Details By Group-Id
                memberList = groupService.GetMembersListByBusiness(_BusinessOwnerLoginId);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = memberList;

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
        /// Get All Searched Business Members (Staff and Students) list for Group.
        /// </summary>
        /// <returns>List of Business(Staff & Students) Members</returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/Group/GetAllSearchedMembersListForGroup")]
        public HttpResponseMessage GetAllSearchedMembersListForGroup(string searchKeyword)
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

                List<GroupMember_ForBusiness_VM> memberList = new List<GroupMember_ForBusiness_VM>();

                // Get Group-Member Details By Group-Id
                memberList = groupService.GetSearchedMembersListByBusiness(_BusinessOwnerLoginId, searchKeyword);

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = memberList;

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