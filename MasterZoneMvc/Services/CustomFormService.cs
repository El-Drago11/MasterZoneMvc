using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Data.SqlClient;

namespace MasterZoneMvc.Services
{
    public class CustomFormService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public CustomFormService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }


        /// <summary>
        /// To Get Custom Form Detail By BusinessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<CustomFormDetail_VM> GetAllCustomFormDetail(long businessOwnerLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageCustomFormDetail<CustomFormDetail_VM>(new SP_ManageCustomFormDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }


        /// <summary>
        ///  To Delete Custom Form Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        public SPResponseViewModel DeleteCustomFormDetail(long Id)
        {
            return storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(new SP_InsertUpdateCustomForm_Param_VM()
            {
                Id = Id,
                Mode = 4
            });
        }


        /// <summary>
        /// To Get Custom Form Name Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public CustomFormDetail_VM GetAllCustomFormNameDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageCustomFormDetailById<CustomFormDetail_VM>(new SP_ManageCustomFormDetail_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }

        /// <summary>
        /// To Get Custom Form Detail by Id (List)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<CustomFormDetail_ViewModel> GetAllCustomFormDetailById(long Id)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageCustomFormDetail<CustomFormDetail_ViewModel>(new SP_ManageCustomFormDetail_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }

        /// <summary>
        /// To Assign The form to Business Owners
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="customformId"></param>
        /// <returns></returns>

        public SPResponseViewModel AssignBusinessCustomForm(long businessOwnerLoginId, long customformId, long userLoginId)
        {
            return storedProcedureRepository.SP_ManageCustomFormDetailById<SPResponseViewModel>(new SP_ManageCustomFormDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Id = customformId,
                UserLoginId = userLoginId,
                Mode = 4
            });
        }


        /// <summary>
        /// To Un Assign the Form To Business Owners
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="customformId"></param>
        /// <returns></returns>
        public SPResponseViewModel UnAssignBusinessCustomForm(long businessOwnerLoginId, long customformId, long userLoginId)
        {
            return storedProcedureRepository.SP_ManageCustomFormDetailById<SPResponseViewModel>(new SP_ManageCustomFormDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Id = customformId,
                UserLoginId = userLoginId,
                Mode = 5
            });
        }

        /// <summary>
        /// To Get All Custom Form Detail By Super Admin Panel in Transform Page
        /// </summary>
        /// <param name="customfornId"></param>
        /// <returns></returns>
        public List<BusinessSearch_ForSuperAdmin_VM> GetAllAssignedBusinessOwnersToCustomFormById(long customfornId)
        {
            return storedProcedureRepository.SP_ManageCustomFormDetail<BusinessSearch_ForSuperAdmin_VM>(new SP_ManageCustomFormDetail_Param_VM()
            {
                Id = customfornId,
                Mode = 6
            });
        }
        /// <summary>
        /// To Get the business Owner Detail To transfer the custom form  
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessSearch_ForSuperAdmin_VM> GetAllAssignedBusinessOwnersCustomFormList(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageCustomFormDetail<BusinessSearch_ForSuperAdmin_VM>(new SP_ManageCustomFormDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 7
            });
        }

        /// <summary>
        /// To Get Business Detail To Transfer Form By Business Owners 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessSearch_ForSuperAdmin_VM> GetAllAssignedBusinessOwnersTransferFormToAnotherBusinessList(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageCustomFormDetail<BusinessSearch_ForSuperAdmin_VM>(new SP_ManageCustomFormDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 8
            });
        }

    }
}