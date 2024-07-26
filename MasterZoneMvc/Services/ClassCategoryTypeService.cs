using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class ClassCategoryTypeService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public ClassCategoryTypeService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Add or Update Class-Category-Type Data
        /// </summary>
        /// <param name="model">Data</param>
        /// <returns>Response with message</returns>
        public SPResponseViewModel AddUpdate(SP_InsertUpdateClassCategoryType_Params_VM model) {
            return storedPorcedureRepository.SP_InsertUpdateClassCategoryType_Get<SPResponseViewModel>(model);
        }

        /// <summary>
        /// Delete Class-Category-Type By Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteClassCategoryType(long id)
        {
            return storedPorcedureRepository.SP_InsertUpdateClassCategoryType_Get<SPResponseViewModel>(new SP_InsertUpdateClassCategoryType_Params_VM()
            {
                Id = id,
                Mode = 3
            });
        }
        
        /// <summary>
        /// Change Status Class-Category-Type By Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SPResponseViewModel ChangeStatusClassCategoryType(long id)
        {
            return storedPorcedureRepository.SP_InsertUpdateClassCategoryType_Get<SPResponseViewModel>(new SP_InsertUpdateClassCategoryType_Params_VM()
            {
                Id = id,
                Mode = 4
            });
        }

        /// <summary>
        /// Get All Class-Category-Types
        /// </summary>
        /// <param name="businessCategoryId">Business-Category-Id (Sub Category)</param>
        /// <returns>All Active Class-Category-Types</returns>
        public List<ClassCategoryType_VM> GetAllClassCategoryTypes()
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Mode = 1
            });
        }

        /// <summary>
        /// Get All Class-Category-Types By Business-Category-Id
        /// </summary>
        /// <param name="businessCategoryId">Business-Category-Id (Sub Category)</param>
        /// <returns>All Active Class-Category-Types</returns>
        public List<ClassCategoryType_VM> GetAllClassCategoryTypesByBusinessCategory(long businessCategoryId)
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                BusinessCategoryId = businessCategoryId,
                Mode = 2
            });
        }

        /// <summary>
        /// Get All Active Class-Category-Types By Business-Category-Id
        /// </summary>
        /// <param name="businessCategoryId">Business-Category-Id (Sub Category)</param>
        /// <returns>All Active Class-Category-Types</returns>
        public List<ClassCategoryType_VM> GetAllActiveClassCategoryTypesByBusinessCategory(long businessCategoryId)
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                BusinessCategoryId = businessCategoryId,
                Mode = 3
            });
        }

        /// <summary>
        /// Get Class-Category-Type Detail By Id
        /// </summary>
        /// <param name="id">Class-Category-Type Id</param>
        /// <returns>Detail</returns>
        public ClassCategoryType_VM GetClassCategoryTypeById(long id)
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_Get<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Id = id,
                Mode = 4
            });
        }

        /// <summary>
        /// Get Class Category detail  
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<ClassCategoryType_VM> Get_ManageClasssCategoryDetail(long Id)
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Id = Id,
                Mode = 5
            });
        }

        /// <summary>
        ///  To Get All ParentBusinessCategories list 
        /// </summary>
        /// <returns></returns>
        public List<ClassCategoryType_VM> GetAllParentClassCategoryList()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new SP_ManageClassCategoryType_Params_VM()
            {
                Mode = 6
            });
        }


        /// <summary>
        ///  To Get All Active Parent-Class-Category-Types List
        /// </summary>
        /// <returns>Active Parent Class Categories</returns>
        public List<ClassCategoryType_VM> GetAllActiveParentClassCategoryList()
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            var parentCategories = storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new SP_ManageClassCategoryType_Params_VM()
            {
                Mode = 6
            });

            return parentCategories.Where(c => c.IsActive == 1).ToList();
        }

        /// <summary>
        /// Get All Active Sub Class-Category-Types By Parent-Class-Category-Type-Id
        /// </summary>
        /// <param name="classCategoryTypeId">Parent Class-Category-Type-Id</param>
        /// <returns>All Active Sub Class-Category-Types</returns>
        public List<ClassCategoryType_VM> GetAllActiveClassCategoryTypesByParentCategory(long classCategoryTypeId)
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Id = classCategoryTypeId,
                Mode = 7
            });
        }
        
        /// <summary>
        /// Get All Sub Class-Category-Types By Parent-Class-Category-Type-Id
        /// </summary>
        /// <param name="classCategoryTypeId">Parent Class-Category-Type-Id</param>
        /// <returns>All Sub Class-Category-Types</returns>
        public List<ClassCategoryType_VM> GetAllSubClassCategoryTypesByParentCategory(long classCategoryTypeId)
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Id = classCategoryTypeId,
                Mode = 8
            });
        }

        /// <summary>
        /// Get All Sub Class-Category-Types For Home-Page
        /// </summary>
        /// <returns>All Sub Class-Category-Types</returns>
        public List<ClassCategoryType_VM> GetAllSubClassCategoryTypesForHomePage()
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Mode = 9
            });
        }

        /// <summary>
        /// dropdown list for  frontpage 
        /// </summary>
        /// <returns></returns>
        public List<ClassCategoryType_VM> Get_ManageClasssCategoryDetailDropdown()
        {
            return storedPorcedureRepository.SP_ManageClassCategoryType_GetAll<ClassCategoryType_VM>(new ViewModels.StoredProcedureParams.SP_ManageClassCategoryType_Params_VM()
            {
                Mode = 1
            });
        }

    }
}