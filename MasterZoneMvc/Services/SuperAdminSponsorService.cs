using MasterZoneMvc.DAL;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class SuperAdminSponsorService
    {
        private MasterZoneDbContext db;
        private readonly StoredProcedureRepository storedProcedureRepository;
        //private SuperAdminSponsorService superAdminSponsorService;
        public SuperAdminSponsorService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
            //superAdminSponsorService = new SuperAdminSponsorService(db);
        }

        /// <summary>
        /// SuperAdmin Sponsor Detail 
        /// </summary>
        /// <param name=""></param>
        /// <returns>To Get Sponsor Detail</returns>
        public SuperAdminSponsor_VisitorVM GetSuperAdminSponsorDetailById(long Id)
        {

            return storedProcedureRepository.SP_ManageSuperAdminSponsors_Get<SuperAdminSponsor_VisitorVM>(new SP_ManageSuperAdminSponsor_Param_VM()
            {
                Id = Id,
                Mode = 2
            });

        }

        /// <summary>
        ///  To Delete the Sponsor For SuperAdmin Panel
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteSuperAdminSponsor(long Id)
        {


            return storedProcedureRepository.SP_InsertUpdateSuperAdminSponsor_Get<SPResponseViewModel>(new SP_InsertUpdateSuperAdminSponsor_Param_VM()
            {
                Id = Id,
                Mode = 3
            });

        }

        //// <summary>
        /// Get Sponsors List For Home Page (Show on Home Page active)
        /// </summary>
        /// <returns>Sponsors List</returns>
        public List<SuperAdminSponsor_VisitorVM> GetSponsorListForHomePage()
        {
            return storedProcedureRepository.SP_ManageSuperAdminSponsors_GetAll<SuperAdminSponsor_VisitorVM>(new SP_ManageSuperAdminSponsor_Param_VM()
            {
                Mode = 4
            });
        }
    }


}
