using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace MasterZoneMvc.Services
{
    public class HomePageContentService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public HomePageContentService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        #region Home-Page-Featured-Card-Section -------------------------------------------------------
        /// <summary>
        /// Insert or Update the Card-Section by Type
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateHomePageFeaturedCardSection(SP_InsertUpdateHomePageFeaturedCardSection param)
        {
            return storedProcedureRepository.SP_InsertUpdateHomePageFeaturedCardSection_Get<SPResponseViewModel>(new SP_InsertUpdateHomePageFeaturedCardSection()
            {
                Id = param.Id,
                Title = param.Title,
                Description = param.Description,
                Type = param.Type,
                ButtonLink = param.ButtonLink,
                ButtonText = param.ButtonText,
                Video = param.Video,
                Status = param.Status,
                Thumbnail = param.Thumbnail,
                SubmittedByLoginId = param.SubmittedByLoginId,
                Mode = 1,
            });
        }

        /// <summary>
        /// Get HomePage-Featured-Card-Section by Id
        /// </summary>
        /// <param name="id">HomePageFeaturedCardSection-Id</param>
        /// <returns>Object</returns>
        public HomePageFeaturedCardSection_VM GetHomePageFeaturedCardSectionById(long id)
        {
            return storedProcedureRepository.SP_ManageHomePageFeaturedCardSection_Get<HomePageFeaturedCardSection_VM>(new SP_ManageHomePageFeaturedCardSection_Params_VM() { 
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get HomePage-Featured-Card-Section by Type
        /// </summary>
        /// <param name="type">HomePageFeaturedCardSection-Type</param>
        /// <returns>Object</returns>
        public HomePageFeaturedCardSection_VM GetHomePageFeaturedCardSectionByType(string type)
        {
            return storedProcedureRepository.SP_ManageHomePageFeaturedCardSection_Get<HomePageFeaturedCardSection_VM>(new SP_ManageHomePageFeaturedCardSection_Params_VM() { 
                Type = type,
                Mode = 2
            });
        }
        #endregion ----------------------------------------------------------------------------------------




        #region Home-Page-Class-Category-Section -------------------------------------------------------
        /// <summary>
        /// Insert or Update Home-Page-Class-Category-Type Section
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateHomePageClassCategorySection(SP_InsertUpdateHomePageClassCategorySection param)
        {
            return storedProcedureRepository.SP_InsertUpdateHomePageClassCategorySection_Get<SPResponseViewModel>(new SP_InsertUpdateHomePageClassCategorySection()
            {
                Id = param.Id,
                ClassCategoryTypeId = param.ClassCategoryTypeId,
                Status = param.Status,
                SubmittedByLoginId = param.SubmittedByLoginId,
                Mode = param.Mode,
            });
        }

        /// <summary>
        /// Get HomePage-Class-Category-Section Data
        /// </summary>
        /// <returns>Data Object</returns>
        public HomePageClassCategorySection_VM GetHomePageClassCategorySectionData()
        {
            return storedProcedureRepository.SP_ManageHomePageClassCategorySection_Get<HomePageClassCategorySection_VM>(new SP_ManageHomePageClassCategorySection_Params_VM()
            {
                Mode = 1
            });
        }

        /// <summary>
        /// Get HomePage-Class-Category-Section Sub-Categories Data.
        /// </summary>
        /// <returns>Data Object</returns>
        public List<ClassCategoryType_VM> GetHomePageClassCategorySection_SubCategoriesList()
        {
            return storedProcedureRepository.SP_ManageHomePageClassCategorySection_GetAll<ClassCategoryType_VM>(new SP_ManageHomePageClassCategorySection_Params_VM()
            {
                Mode = 2
            });
        }
        #endregion  ------------------------------------------------------------------------------------




        #region Home-Page-Multiple-Items-Section -------------------------------------------------------

        /// <summary>
        /// Insert or Update Home-Page-Multiple-Items Images/Videos
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateHomePageMultipleItem(SP_InsertUpdateHomePageMultipleItem_Params_VM param)
        {
            return storedProcedureRepository.SP_InsertUpdateHomePageMultipleItem_Get<SPResponseViewModel>(new SP_InsertUpdateHomePageMultipleItem_Params_VM()
            {
                Id = param.Id,
                Title = param.Title,
                Description = param.Description,
                Type = param.Type,
                Link = param.Link,
                Video = param.Video,
                Status = param.Status,
                Thumbnail = param.Thumbnail,
                SubmittedByLoginId = param.SubmittedByLoginId,
                Mode = param.Mode,
            });
        }

        /// <summary>
        /// Get HomePage-Multiple-Item by Id
        /// </summary>
        /// <param name="id">HomePageMulitipleItem-Id</param>
        /// <returns>Object</returns>
        public HomePageMultipleItem_VM GetHomePageMultipleItemDataById(long id)
        {
            return storedProcedureRepository.SP_ManageHomePageMultipleItem_Get<HomePageMultipleItem_VM>(new SP_ManageHomePageMultipleItem_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All HomePage-Multiple-Item by Type
        /// </summary>
        /// <param name="type">HomePageMulitipleItem-Type</param>
        /// <returns>Object List</returns>
        public List<HomePageMultipleItem_VM> GetAllHomePageMultipleItemsByType(string type)
        {
            return storedProcedureRepository.SP_ManageHomePageMultipleItem_GetAll<HomePageMultipleItem_VM>(new SP_ManageHomePageMultipleItem_Params_VM()
            {
                Type = type,
                Mode = 2
            });
        }

        #endregion ----------------------------------------------------------------------------------------



        #region Home-Page-Featured-Video -------------------------------------------------------
        /// <summary>
        /// Insert or Update the Featured-Video
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateHomePageFeaturedVideo(SP_InsertUpdateHomePageFeaturedVideo_Params_VM param)
        {
            return storedProcedureRepository.SP_InsertUpdateHomePageFeaturedVideo_Get<SPResponseViewModel>(new SP_InsertUpdateHomePageFeaturedVideo_Params_VM()
            {
                Id = param.Id,
                Title = param.Title,
                Description = param.Description,
                Video = param.Video,
                Status = param.Status,
                Thumbnail = param.Thumbnail,
                SubmittedByLoginId = param.SubmittedByLoginId,
                Mode = param.Mode,
            });
        }

        /// <summary>
        /// Get HomePage-Featured-Video by Id
        /// </summary>
        /// <param name="id">HomePageFeaturedVideo-Id</param>
        /// <returns>Object</returns>
        public HomePageFeaturedVideo_VM GetHomePageFeaturedVideoById(long id)
        {
            return storedProcedureRepository.SP_ManageHomePageFeaturedVideo_Get<HomePageFeaturedVideo_VM>(new SP_ManageHomePageFeaturedVideo_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All HomePage-Featured-Videos 
        /// </summary>
        /// <returns>Object List</returns>
        public List<HomePageFeaturedVideo_VM> GetAllHomePageFeaturedVideos()
        {
            return storedProcedureRepository.SP_ManageHomePageFeaturedVideo_GetAll<HomePageFeaturedVideo_VM>(new SP_ManageHomePageFeaturedVideo_Params_VM()
            {
                Mode = 2
            });
        }
        #endregion ----------------------------------------------------------------------------------------
        
        
        #region Home-Page-Banner-Item -------------------------------------------------------
        /// <summary>
        /// Insert or Update the Home-Page Banner-Item
        /// </summary>
        /// <param name="param">Data</param>
        /// <returns>Returns success or error response</returns>
        public SPResponseViewModel InsertUpdateHomePageBannerItem(SP_InsertUpdateHomePageBannerItem_Params_VM param)
        {
            return storedProcedureRepository.SP_InsertUpdateHomePageBannerItem_Get<SPResponseViewModel>(new SP_InsertUpdateHomePageBannerItem_Params_VM()
            {
                Id = param.Id,
                Type = param.Type,
                Image = param.Image,
                Video = param.Video,
                Status = param.Status,
                SubmittedByLoginId = param.SubmittedByLoginId,
                Text = param.Text,
                Link = param.Link,
                Mode = param.Mode
            });
        }

        /// <summary>
        /// Get HomePage-Banner-Item by Id
        /// </summary>
        /// <param name="id">HomePageBannerItem-Id</param>
        /// <returns>Object</returns>
        public HomePageBannerItem_VM GetHomePageBannerItemById(long id)
        {
            return storedProcedureRepository.SP_ManageHomePageBannerItem_Get<HomePageBannerItem_VM>(new SP_ManageHomePageBannerItem_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All HomePage-Banner-Items 
        /// </summary>
        /// <returns>Object List</returns>
        public List<HomePageBannerItem_VM> GetAllHomePageBannerItems()
        {
            return storedProcedureRepository.SP_ManageHomePageBannerItem_GetAll<HomePageBannerItem_VM>(new SP_ManageHomePageBannerItem_Params_VM()
            {
                Mode = 2
            });
        }
        #endregion ----------------------------------------------------------------------------------------

    }
}