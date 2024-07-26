using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Windows.Controls;

namespace MasterZoneMvc.Services
{
    public class GroupService
    {
        private MasterZoneDbContext db;
        private NotificationService notificationService;
        private StoredProcedureRepository storedPorcedureRepository;


        public GroupService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Group Detail by Group-Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Group_VM GetGroupById(long id)
        {
            // Get Group By Id
           return storedPorcedureRepository.SP_ManageGroup_Get<Group_VM>(new SP_ManageGroup_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        public JqueryDataTable_Pagination_Response_VM<Group_Pagination_VM> GetGoupsListByBusinessOwner_Pagination(NameValueCollection httpRequestParams, GroupList_Pagination_SQL_Params_VM _Params_VM)
        {

            _Params_VM.JqueryDataTableParams.draw = (Convert.ToInt32(httpRequestParams["draw"]));
            _Params_VM.JqueryDataTableParams.start = (Convert.ToInt32(httpRequestParams["start"]));
            _Params_VM.JqueryDataTableParams.length = (Convert.ToInt32(httpRequestParams["length"])) == 0 ? 10 : (Convert.ToInt32(httpRequestParams["length"]));
            _Params_VM.JqueryDataTableParams.searchValue = httpRequestParams["search[value]"] ?? "";
            _Params_VM.JqueryDataTableParams.sortColumnIndex = Convert.ToInt32(httpRequestParams["order[0][column]"]);
            _Params_VM.JqueryDataTableParams.sortColumn = "";
            _Params_VM.JqueryDataTableParams.sortOrder = "";
            _Params_VM.JqueryDataTableParams.sortDirection = httpRequestParams["order[0][dir]"] ?? "asc";
            //_Params_VM.GetExpenditure_Params_VM = JsonConvert.DeserializeObject<GetExpenditure_Params_VM>(HttpContext.Current.Request.Params["_Params"]);

            long recordsTotal = 0;

            // optimized code
            string[] expenditureSortColumn = {
                            "CreatedOn",
                            "CreatedOn",
                            "Name",
                            "Description",
                            "CreatedOn"
                        };

            _Params_VM.JqueryDataTableParams.sortColumn = expenditureSortColumn[_Params_VM.JqueryDataTableParams.sortColumnIndex];

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<Group_Pagination_VM> lstGroupRecords = new List<Group_Pagination_VM>();

            //IExpenditureRepository expenditureRepository = new ExpenditureRepository(db);
            //lstExpenditures = expenditureRepository.GetExpendituresList_Pagination(_Params_VM);

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", _Params_VM.Mode)
                            };

            lstGroupRecords = db.Database.SqlQuery<Group_Pagination_VM>("exec sp_ManageGroup_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();


            foreach (var group in lstGroupRecords)
            {
                group.EncryptedUserLoginId = EDClass.Encrypt(group.Id.ToString());
            }



            recordsTotal = lstGroupRecords.Count > 0 ? lstGroupRecords[0].TotalRecords : 0;

            JqueryDataTable_Pagination_Response_VM<Group_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Group_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstGroupRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };
            return jqueryDataTableParamsViewModel;
        }

        /// <summary>
        /// Get All Group Names List By Business
        /// </summary>
        /// <param name="userLoginId">Business User-Login-Id</param>
        /// <returns>List of Groups</returns>
        public List<GroupNameList_VM> GetAllGroupNamesListByBusiness(long userLoginId, string groupType)
        {
            return storedPorcedureRepository.SP_ManageGroup_GetAll<GroupNameList_VM>(new SP_ManageGroup_Params_VM()
            {
                UserLoginId = userLoginId,
                GroupType = groupType,
                Mode = 3
            });
        }
        
        /// <summary>
        /// Get All Group Names List not assigned to any class-batch except batch(passed by Id) By Business
        /// </summary>
        /// <param name="userLoginId">Business User-Login-Id</param>
        /// <param name="batchId">Batch-Id whose groups will be in returned list</param>
        /// <returns>List of Groups</returns>
        public List<GroupNameList_VM> GetAllUnassignedBatchGroupNamesListByBusiness(long userLoginId, long batchId)
        {
            var groupNameList_VM = storedPorcedureRepository.SP_ManageBatch_GetAll<GroupNameList_VM>(new SP_ManageBatch_Params_VM()
            {
                BusinessOwnerLoginId = userLoginId,
                Id = batchId,
                Mode = 6
            });
            return groupNameList_VM;
        }

        /// <summary>
        /// Add User/Member in Group and send Notification
        /// </summary>
        /// <param name="params_VM"></param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel AddMemberInGroup(AddGroupMember_VM params_VM)
        {
            //validate
            if (params_VM.GroupId <= 0)
                return new SPResponseViewModel() { ret = -1, responseMessage = Resources.BusinessPanel.GroupNotExist_ErrorMessage };

            // Insert-Update Group Member
            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("groupId", params_VM.GroupId),
                            new SqlParameter("memberLoginId", params_VM.MemberLoginId),
                            new SqlParameter("userLoginId", params_VM.GroupUserLoginId),
                            new SqlParameter("mode", "1")
                            };

            var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateGroupMember @id,@groupId,@memberLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();

            // Send notification to user if successfully removed
            if (resp.ret == 1)
            {
                var group_VM = GetGroupById(params_VM.GroupId);
                notificationService = new NotificationService(db);

                var notificationResponse = notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                {
                    FromUserLoginId = params_VM.GroupUserLoginId,
                    NotificationText = "You have been added in the group '" + group_VM.Name + "'",
                    NotificationTitle = "Added in Group - " + group_VM.Name,
                    NotificationType = "Custom",
                    NotificationUsersList = params_VM.MemberLoginId.ToString(),
                    Mode = 1
                });
            }

            return resp;
        }

        public List<UserClassGroupList_VM> GetAllGroupList(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedPorcedureRepository.SP_GetAllStudentGroupRecord<UserClassGroupList_VM>(new SP_GetAllStudentGroupRecord_Params_VM
            {

                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 1
            });

        }

        /// <summary>
        /// Get All Group-Members Table Data Records by Group-Id
        /// </summary>
        /// <param name="id">Group-Id</param>
        /// <returns>GroupMembers Table Records Only</returns>
        public List<GroupMember> GetGroupMemberTableDataById(long id)
        {
            // Get Group-Members By Id
            return storedPorcedureRepository.SP_ManageGroup_GetAll<GroupMember>(new SP_ManageGroup_Params_VM()
            {
                Id = id,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Group-Member List by Group-Id
        /// </summary>
        /// <param name="groupId">Group-Id</param>
        /// <param name="businessOwnerLoginId">Business-Owner-Login-Id</param>
        /// <returns>Group Member List</returns>
        public List<GroupMember_ForBusiness_VM> GetGroupMemberListByGroupId(long groupId, long businessOwnerLoginId)
        {
            // Get Group By Id
            return storedPorcedureRepository.SP_ManageGroup_GetAll<GroupMember_ForBusiness_VM>(new SP_ManageGroup_Params_VM()
            {
                Id = groupId,
                UserLoginId= businessOwnerLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// Get Members List (Staff and Students) by Business for group 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns>Members list</returns>
        public List<GroupMember_ForBusiness_VM> GetMembersListByBusiness(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageGroup_GetAll<GroupMember_ForBusiness_VM>(new SP_ManageGroup_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 5
            });
        }

        /// <summary>
        /// Get Searched Members List (Staff and Students) by Business for group 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="searchKeywords">Search Keywords entered</param>
        /// <returns>Members list</returns>
        public List<GroupMember_ForBusiness_VM> GetSearchedMembersListByBusiness(long businessOwnerLoginId, string searchKeywords)
        {
            return storedPorcedureRepository.SP_ManageGroup_GetAll<GroupMember_ForBusiness_VM>(new SP_ManageGroup_Params_VM()
            {
                UserLoginId = businessOwnerLoginId,
                SearchKeywords = searchKeywords,
                Mode = 6
            });
        }

    }
}