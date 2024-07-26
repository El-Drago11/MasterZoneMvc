using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class FieldTypeCatalogService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public FieldTypeCatalogService(MasterZoneDbContext dbContext) {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get All Active Field-Type List by KeyName
        /// </summary>
        /// <param name="keyName">Key Name for which linked list of items to fetch</param>
        /// <returns>List of linked Items/types</returns>
        public List<FieldTypeCatalogViewModel> GetAllActiveFieldTypeCatologByKey(string keyName)
        {
            return storedPorcedureRepository.SP_ManageFieldTypeCatalog_GetAll<FieldTypeCatalogViewModel>(new ViewModels.StoredProcedureParams.SP_ManageFieldTypeCatalog_Params_VM()
            {
                KeyName = keyName,
                Mode = 1
            });
        }
    }
}