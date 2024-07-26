using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class MenuService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        
        public MenuService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(dbContext);
        }

        /// <summary>
        /// Get All Visitor-Menu list for Visitor Panel [Non-Deleted]
        /// </summary>
        /// <returns>All Non-Deleted Menu List</returns>
        public List<VisitorMenu_VM> GetAllVisitorMenuItems()
        {
            List<VisitorMenu_VM> visitorMenu_VMs = new List<VisitorMenu_VM>();
            visitorMenu_VMs = storedProcedureRepository.SP_ManageMenu_GetAll<VisitorMenu_VM>(new ViewModels.StoredProcedureParams.SP_ManageMenu_Params_VM() { Mode = 2});
            return visitorMenu_VMs;
        }

        /// <summary>
        /// Get All Active Visitor-Menu list for Visitor Panel
        /// </summary>
        /// <returns>All Active Menu List</returns>
        public List<VisitorMenu_VM> GetAllActiveVisitorMenuList()
        {
            List<VisitorMenu_VM> visitorMenu_VMs = new List<VisitorMenu_VM>();
            visitorMenu_VMs = storedProcedureRepository.SP_ManageMenu_GetAll<VisitorMenu_VM>(new ViewModels.StoredProcedureParams.SP_ManageMenu_Params_VM() { Mode = 2 });
            visitorMenu_VMs = visitorMenu_VMs.Where(x => x.IsActive == 1).ToList();

            return visitorMenu_VMs;
        }

        /// <summary>
        /// Get All Active Visitor-Menu Tag list for Visitor Panel
        /// </summary>
        /// <returns>All Active Menu Tags List</returns>
        public List<string> GetAllActiveVisitorMenuTagsList()
        {
            List<VisitorMenu_VM> visitorMenu_VMs = new List<VisitorMenu_VM>();
            visitorMenu_VMs = storedProcedureRepository.SP_ManageMenu_GetAll<VisitorMenu_VM>(new ViewModels.StoredProcedureParams.SP_ManageMenu_Params_VM() { Mode = 2 });
            List<string> TagsList = visitorMenu_VMs.Where(x => x.IsActive == 1).Select(x => x.Tag).ToList();

            return TagsList;
        }

        /// <summary>
        /// Get Menu Detail by Id
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Menu Table data</returns>
        public VisitorMenu_VM GetMenuById(long menuId)
        {
            VisitorMenu_VM response = storedProcedureRepository.SP_ManageMenu_Get<VisitorMenu_VM>(new SP_ManageMenu_Params_VM { Id = menuId, Mode = 3});
            return response;
        }

        /// <summary>
        /// Change Menu status [vice-versa automatically]
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Stored-Procedure Response status and message</returns>
        public SPResponseViewModel ChangeMenuStatusById(long menuId)
        {
            SPResponseViewModel response = storedProcedureRepository.SP_ManageMenu_Get<SPResponseViewModel>(new SP_ManageMenu_Params_VM { Id = menuId, Mode = 4});
            return response;
        }
        
        /// <summary>
        /// Delete Menu
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Stored-Procedure Response status and message</returns>
        public SPResponseViewModel DeleteMenuById(long menuId)
        {
            SPResponseViewModel response = storedProcedureRepository.SP_InsertUpdateMenu_Get<SPResponseViewModel>(new SP_InsertUpdateMenu_Params_VM { Id = menuId, Mode = 3});
            return response;
        }

        /// <summary>
        /// Insert-Update Menu Record in Database
        /// </summary>
        /// <param name="params_VM">Stored Procedure Parameters data Obj</param>
        /// <returns>Stored-Procedure Response status and message</returns>
        public SPResponseViewModel InsertUpdateVisitorMenu(SP_InsertUpdateMenu_Params_VM params_VM)
        {
            return storedProcedureRepository.SP_InsertUpdateMenu_Get<SPResponseViewModel>(params_VM);
        }

        /// <summary>
        /// Get All Business/Academies (B2B Business Owners) by Menu Tag filter and location
        /// </summary>
        /// <param name="MenuTag">Menu Tag Name</param>
        /// <param name="CategoryKey"></param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Fetched record Id</param>
        /// <param name="recordLimit">No. of records to fetch next</param>
        /// <returns>List of Business Owners (Academies)</returns>
        public List<BusinessOnwerListByMenuTag_VM> GetAllBusinessAcademiesListByMenutag(FilterRecordByMenu_VM filterRecordByMenu_VM)
        {
            string CategoryKey = StaticResources.BusinessCategory_CategoryKey_B2B;

            return storedProcedureRepository.SP_GetRecordByMenu_GetAll<BusinessOnwerListByMenuTag_VM>(new SP_GetRecordByMenu_Params_VM
            {
                MenuTag = filterRecordByMenu_VM.MenuTag,
                City = filterRecordByMenu_VM.City,
                CategoryKey = CategoryKey,
                LastRecordId = filterRecordByMenu_VM.LastRecordId,
                RecordLimit = filterRecordByMenu_VM.RecordLimit,
                ItemType = "",
                Mode = 1,
                CategorySearch = filterRecordByMenu_VM.CategorySearchValue,
                SearchType = filterRecordByMenu_VM.SearchType,
                SearchValue = filterRecordByMenu_VM.SearchValue
            });
        }

        /// <summary>
        /// Get All Instructors (Individual Business Owners) by Menu Tag filter and location
        /// </summary>
        /// <param name="MenuTag">Menu Tag Name</param>
        /// <param name="CategoryKey"></param>
        /// <param name="ItemType"></param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last fetched record Id</param>
        /// <param name="recordLimit">No. of records to fetch</param>
        /// <returns>List of Instructors</returns>
        public List<InstructorListByMenuTag_VM> GetAllInstructorByMenutag(FilterRecordByMenu_VM filterRecordByMenu_VM)
        {
            string CategoryKey = StaticResources.BusinessCategory_CategoryKey_Instructor;
            string ItemType = "Business";

            var instructorsList = storedProcedureRepository.SP_GetRecordByMenu_GetAll<InstructorListByMenuTag_VM>(new SP_GetRecordByMenu_Params_VM
            {
                UserLoginId = filterRecordByMenu_VM.UserLoginId,
                MenuTag = filterRecordByMenu_VM.MenuTag,
                City = filterRecordByMenu_VM.City,
                CategoryKey = CategoryKey,
                LastRecordId = filterRecordByMenu_VM.LastRecordId,
                RecordLimit = filterRecordByMenu_VM.RecordLimit,
                ItemType = ItemType,
                Mode = 2,
                CategorySearch = filterRecordByMenu_VM.CategorySearchValue,
                SearchType = filterRecordByMenu_VM.SearchType,
                SearchValue = filterRecordByMenu_VM.SearchValue
            });

            foreach (var instructor in instructorsList)
            {
                var certificatesList = storedProcedureRepository.SP_ManageUserCertificates_GetAll<UserCertificationForProfile_VM>(new SP_ManageUserCertificates_Params_VM() { 
                    UserLoginId = instructor.BusinessOwnerLoginId,
                    Mode = 1
                });
                instructor.CertificationList = certificatesList;
            }

            return instructorsList;
        }

        /// <summary>
        /// Get All Event/Activities by Menu Tag filter and location
        /// </summary>
        /// <param name="MenuTag">Menu Tag</param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Fethed record Id</param>
        /// <param name="recordLimit">No. of Records to fetch</param>
        /// <returns>List of Events</returns>
        public List<EventListByMenuTag_VM> GetAllEventActivitiesByMenutag(FilterRecordByMenu_VM filterRecordByMenu_VM)
        {
            return storedProcedureRepository.SP_GetRecordByMenu_GetAll<EventListByMenuTag_VM>(new SP_GetRecordByMenu_Params_VM
            {
                MenuTag = filterRecordByMenu_VM.MenuTag,
                City = filterRecordByMenu_VM.City,
                LastRecordId = filterRecordByMenu_VM.LastRecordId,
                RecordLimit = filterRecordByMenu_VM.RecordLimit,
                ItemType = "",
                CategoryKey = "",
                Mode = 3,
                CategorySearch = filterRecordByMenu_VM.CategorySearchValue,
                SearchType = filterRecordByMenu_VM.SearchType,
                SearchValue = filterRecordByMenu_VM.SearchValue
            });
        }

        /// <summary>
        /// Get All Classes by Menu Tag filter and location
        /// </summary>
        /// <param name="MenuTag">Menu Tag</param>
        /// <param name="City">City Name</param>
        /// <param name="lastRecordId">Last Fethed record Id</param>
        /// <param name="recordLimit">No. of Records to fetch</param>
        /// <returns>List of Events</returns>
        public List<ClassSearchList_VM> GetAllClassesByMenutag(FilterRecordByMenu_VM filterRecordByMenu_VM)
        {
            return storedProcedureRepository.SP_GetRecordByMenu_GetAll<ClassSearchList_VM>(new SP_GetRecordByMenu_Params_VM
            {
                MenuTag = filterRecordByMenu_VM.MenuTag,
                City = filterRecordByMenu_VM.City,
                LastRecordId = filterRecordByMenu_VM.LastRecordId,
                RecordLimit = filterRecordByMenu_VM.RecordLimit,
                ItemType = "",
                CategoryKey = "",
                Mode = 4,
                CategorySearch = filterRecordByMenu_VM.CategorySearchValue,
                SearchType = filterRecordByMenu_VM.SearchType,
                SearchValue = filterRecordByMenu_VM.SearchValue
            });
        }
    }
}