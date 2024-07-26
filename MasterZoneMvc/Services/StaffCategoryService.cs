using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class StaffCategoryService
    {
        private MasterZoneDbContext db;
        private string _SiteURL = ConfigurationManager.AppSettings["SiteURL"];
        private StoredProcedureRepository storedProcedureRepository;


        public StaffCategoryService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }



        /// <summary>
        /// Insert or Update Insert Update Staff Category
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateStaffCategory(SP_InsertUpdateStaffCategory_Params_VM param)
        {
            return storedProcedureRepository.SP_InsertUpdateStaffCategory_Get<SPResponseViewModel>(param);
        }



        /// <summary>
        /// Get staff Category detail  
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<StaffCategory_VM> Get_ManageStaffCategoryDetail(long id)
        {
            return storedProcedureRepository.SP_ManageStaffCategories_GetAll<StaffCategory_VM>(new ViewModels.StoredProcedureParams.SP_ManageStaffCategories_Params_VM()
            {
                Id = id,
                Mode = 2
            });
        }

        /// <summary>
        /// Get Staff Category ById-Type Detail By Id
        /// </summary>
        /// <param name="id">Staff-Category-Type Id</param>
        /// <returns>Detail</returns>
        public StaffCategory_VM GetStaffCategoryById(long id)
        {
            return storedProcedureRepository.SP_ManageStaffCategories_Get<StaffCategory_VM>(new ViewModels.StoredProcedureParams.SP_ManageStaffCategories_Params_VM()
            {
                Id = id,
                Mode = 3
            });
        }


        /// <summary>
        /// delete Staff category
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>

        public SPResponseViewModel DeleteStaffCategory(long id)
        {
            return storedProcedureRepository.SP_InsertUpdateStaffCategory_Get<SPResponseViewModel>(new SP_InsertUpdateStaffCategory_Params_VM()
            {
                Id = id,
                Mode = 3,
            });
        }


        /// <summary>
        /// Change Status Staff-Category-Type By Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SPResponseViewModel ChangeStatusStaffCategoryType(long id)
        {
            return storedProcedureRepository.SP_InsertUpdateStaffCategory_Get<SPResponseViewModel>(new SP_InsertUpdateStaffCategory_Params_VM()
            {
                Id = id,
                Mode = 4
            });
        }
    }
}