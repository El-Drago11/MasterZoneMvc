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
    public class ContactService
    {

        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public ContactService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }


        /// <summary>
        ///  To Get Contact Detail By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ContactDetailViewModel GetContactDetail_Get()
        {
            return storedPorcedureRepository.SP_ManageContactDetail_Get<ContactDetailViewModel>(new SP_ManageContactDetail_Param_VM()
            {

                Mode = 1
            });
        }

        /// <summary>
        /// To Get Contact Number Detail 
        /// </summary>
        /// <returns></returns>
        public ContactNumberDetail_VM GetContactNumberDetail_Get()
        {
            return storedPorcedureRepository.SP_ManageContactNumberDetail_Get<ContactNumberDetail_VM>(new SP_ManageContactNumberDetail_Param_VM()
            {

                Mode = 1
            });
        }

    }
}