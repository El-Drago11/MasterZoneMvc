using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class ItemFeatureService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository StoredProcedureRepository;

        public ItemFeatureService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            StoredProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Insert Item Features 
        /// </summary>
        /// <param name="params_VM">Stored Procedure Parameters</param>
        /// <returns>Response stauts +ve if created</returns>
        public SPResponseViewModel InsertItemFeatures(SP_InsertUpdateItemFeatures_Params_VM params_VM)
        {
            return StoredProcedureRepository.SP_InsertUpdateItemFeatures<SPResponseViewModel>(new SP_InsertUpdateItemFeatures_Params_VM() { 
                Id = params_VM.Id,
                ItemId = params_VM.ItemId,
                ItemType = params_VM.ItemType,
                FeatureId = params_VM.FeatureId,
                IsLimited = params_VM.IsLimited,
                Limit = params_VM.Limit,
                Mode = 1
            });
        }

        /// <summary>
        /// Delete all Item-Features of particular Item by ItemId
        /// </summary>
        /// <param name="itemId">Item Id</param>
        /// <param name="itemType">Item Type</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel DeleteItemFeatures(long itemId, string itemType)
        {
            return StoredProcedureRepository.SP_InsertUpdateItemFeatures<SPResponseViewModel>(new SP_InsertUpdateItemFeatures_Params_VM()
            {
                ItemId = itemId,
                ItemType = itemType,
                Mode = 2
            });
        }

        /// <summary>
        /// Get all Item-Features of particular Item by ItemId
        /// </summary>
        /// <param name="itemId">Item Id</param>
        /// <param name="itemType">Item Type</param>
        /// <returns>Success or error response</returns>
        public List<ItemFeatureViewModel> GetItemFeatures(long itemId, string itemType)
        {
            return StoredProcedureRepository.SP_ManageItemFeatures_GetAll<ItemFeatureViewModel>(new SP_ManageItemFeatures_Params_VM()
            {
                ItemId = itemId,
                ItemType = itemType,
                Mode = 1
            });
        }
    }
}