using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class BranchService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public BranchService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// To Get the Branch detail Pagination For (BusinessPanel)
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BranchDetail_Pagination_VM> GetBranchList_Pagination(NameValueCollection httpRequestParams, BranchDetail_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<BranchDetail_Pagination_VM> lstRecords = new List<BranchDetail_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<BranchDetail_Pagination_VM>("exec sp_ManageBusinessBranches_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<BranchDetail_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BranchDetail_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// To Chnage the branch status 
        /// </summary>
        /// <param name="branchBusinessLoginId"></param>
        /// <returns></returns>
        public SPResponseViewModel ChangeStatusBranchDetail(long Id)
        {
            return storedPorcedureRepository.InsertUpdateBusinessBranches_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessBranch_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }

        /// <summary>
        /// To Delete Branch detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBranchDetail(long branchBusinessLoginId)
        {
            return storedPorcedureRepository.InsertUpdateBusinessBranches_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessBranch_Param_VM()
            {
                BranchBusinessLoginId = branchBusinessLoginId,
                Mode = 4
            });
        }


        /// <summary>
        /// To get Single branch detail by BranchBusinessLoginId
        /// </summary>
        /// <param name="branchBusinessLoginId"></param>
        /// <returns></returns>
        public BranchDetail_VM GetBranchDetailByBranchbusinessLoginId(long branchBusinessLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessBranchDetail_Get<BranchDetail_VM>(new SP_ManageBusinessBranchDetail_Param_VM()
            {
                BusinessOwnerLoginId = branchBusinessLoginId,
                Mode = 1
            });
        }


        /// <summary>
        /// To Get View for Business  Branches Staff detail 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BranchStaffDetail_Pagination> GetBranchStaffList_Pagination(NameValueCollection httpRequestParams, BranchStaffDetail_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<BranchStaffDetail_Pagination> lstRecords = new List<BranchStaffDetail_Pagination>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<BranchStaffDetail_Pagination>("exec sp_GetAllBusinesBranchStaff_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<BranchStaffDetail_Pagination> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BranchStaffDetail_Pagination>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// To Get View Detail For Branch Event 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<EventBranch_Pagination_VM> GetBranchEventList_Pagination(NameValueCollection httpRequestParams, BranchEventDetail_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<EventBranch_Pagination_VM> lstRecords = new List<EventBranch_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<EventBranch_Pagination_VM>("exec sp_GetAllBusinesBranchEvent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;


            JqueryDataTable_Pagination_Response_VM<EventBranch_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<EventBranch_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }



        /// <summary>
        ///  To Get Branch Student Pagination By Business Owner 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<BranchStudentDetail_VM> GetBranchStudentList_Pagination(NameValueCollection httpRequestParams, StudentList_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<BranchStudentDetail_VM> lstRecords = new List<BranchStudentDetail_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<BranchStudentDetail_VM>("exec sp_ManageBusinessBranchStudent_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;
            for(var i = 0; i<lstRecords.Count;i++)
            {
                List<BusinessClassDetailList_VM> businessclassDetail = GetAllStudentClassesList(lstRecords[i].UserLoginId);
                lstRecords[i].ClassNameList = businessclassDetail;
            }


            JqueryDataTable_Pagination_Response_VM<BranchStudentDetail_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<BranchStudentDetail_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }


        /// <summary>
        /// To Get Student Transaction detail By Branch LoginId 
        /// </summary>
        /// <param name="httpRequestParams"></param>
        /// <param name="_Params_VM"></param>
        /// <returns></returns>
        public JqueryDataTable_Pagination_Response_VM<Manage_BranchStudentPayment_Pagination_VM> GetBranchPaymentList_Pagination(NameValueCollection httpRequestParams, BranchPaymentsList_Pagination_SQL_Params_VM _Params_VM)
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


            _Params_VM.JqueryDataTableParams.sortColumn = (_Params_VM.JqueryDataTableParams.sortColumnIndex > 0) ? httpRequestParams["columns[" + _Params_VM.JqueryDataTableParams.sortColumnIndex + "][name]"] : "CreatedOn";

            if (_Params_VM.JqueryDataTableParams.sortDirection == "asc")
                _Params_VM.JqueryDataTableParams.sortOrder = "ASC";
            else
                _Params_VM.JqueryDataTableParams.sortOrder = "DESC";


            List<Manage_BranchStudentPayment_Pagination_VM> lstRecords = new List<Manage_BranchStudentPayment_Pagination_VM>();

            SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", _Params_VM.Id),
                            new SqlParameter("businessOwnerLoginId", _Params_VM.BusinessAdminLoginId),
                            new SqlParameter("userLoginId", _Params_VM.LoginId),
                            new SqlParameter("start", _Params_VM.JqueryDataTableParams.start),
                            new SqlParameter("searchFilter", _Params_VM.JqueryDataTableParams.searchValue),
                            new SqlParameter("pageSize", _Params_VM.JqueryDataTableParams.length),
                            new SqlParameter("sorting", _Params_VM.JqueryDataTableParams.sortColumn),
                            new SqlParameter("sortOrder", _Params_VM.JqueryDataTableParams.sortOrder),
                            new SqlParameter("mode", "1")
                            };

            lstRecords = db.Database.SqlQuery<Manage_BranchStudentPayment_Pagination_VM>("exec sp_ManageBranchTransactionPayment_Pagination @id,@businessOwnerLoginId,@userLoginId,@start,@searchFilter,@pageSize,@sorting,@sortOrder,@mode", queryParams).ToList();

            recordsTotal = lstRecords.Count > 0 ? lstRecords[0].TotalRecords : 0;



            JqueryDataTable_Pagination_Response_VM<Manage_BranchStudentPayment_Pagination_VM> jqueryDataTableParamsViewModel = new JqueryDataTable_Pagination_Response_VM<Manage_BranchStudentPayment_Pagination_VM>()
            {
                draw = _Params_VM.JqueryDataTableParams.draw,
                data = lstRecords,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal
            };

            return jqueryDataTableParamsViewModel;
        }



        /// <summary>
        /// To Get Student Class Detail List
        /// </summary>
        /// <param name="studentLoginId">Student-User-Login-Id</param>
        /// <returns></returns>
        public List<BusinessClassDetailList_VM> GetAllStudentClassesList(long studentLoginId)
        {
            return storedPorcedureRepository.SP_ManageClass_GetAll<BusinessClassDetailList_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = studentLoginId,
                Mode = 18
            });
        }


        /// <summary>
        /// To Get Branches Detail For Visitor Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessBranchesDetailForVisitorPanel> GetAllBranchesDetailList(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessBranchesDetaillst_Get<BusinessBranchesDetailForVisitorPanel>(new SP_ManageBusinessBranchDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }

        /// <summary>
        /// To Get Branches Detail For Visitor Panel 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessBranchesDetailForVisitorPanel GetAllBranchesDetailListsforBusinessName(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessBranchDetail_Get<BusinessBranchesDetailForVisitorPanel>(new SP_ManageBusinessBranchDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 4
            });
        }

        /// <summary>
        /// To Get Business Classic Dance Detail By Location and UserLoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="State"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        public List<BusinessBranchesDetailForVisitorPanel> GetAllLocationBranchesDetailList(string State,string City, long businessownerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessClassicDanceBranchDetail_Get<BusinessBranchesDetailForVisitorPanel>(new SP_ManageBusinessBranchLocationDetail_Param_VM()
            {
              BusinessOwnerLoginId = businessownerLoginId,
                State = State,
                City = City,
                Mode = 1
            });
        }
    }
}