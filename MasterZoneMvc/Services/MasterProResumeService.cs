using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
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
    public class MasterProResumeService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        public MasterProResumeService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        public MasterProResume_ViewModel SP_MasterProDetails_Get(long BusinessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageMasterResumeDetails<MasterProResume_ViewModel>(new SP_ManageMasterProResume_PPCMeta_Params_VM()
            {

                UserLoginId = BusinessLoginId,
                Mode = 2
            });
        }

        public MasterProResume_ViewModel SP_MasterProDetails_GetAll(long BusinessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_ManageMasterResumeDetails<MasterProResume_ViewModel>(new SP_ManageMasterProResume_PPCMeta_Params_VM()
            {

                UserLoginId = BusinessLoginId,
                Mode = 1
            });
        }

     

        public MasterProResume_PPCMeta_VM SP_InsertMasterResumeDetails(long BusinessLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);

            return storedProcedureRepository.SP_InsertUpdateMasterResumeDetails<MasterProResume_PPCMeta_VM>(new SP_InsertUpdateMasterpro_Params_VM()
            {
                UserLoginId = BusinessLoginId,
                Mode = 1
            });
        }

        




        //public MasterProResume_PPCMeta_VM SP_UpdateMasterProDetails(long BusinessLoginId)
        //{
        //    storedProcedureRepository = new StoredProcedureRepository(db);
        //    return storedProcedureRepository.SP_InsertUpdateMasterResumeDetails<MasterProResume_PPCMeta_VM>(new SP_ManageMasterProResumeDetails()
        //    {
        //        BusinessOwnerLoginId = BusinessLoginId,
        //        Mode = 2
        //    });

        //}

        //public MasterProResume_PPCMeta_VM SP_DeleteMasterProDetails(long BusinessLoginId)
        //{
        //    storedProcedureRepository = new StoredProcedureRepository(db);
        //    return storedProcedureRepository.SP_InsertUpdateMasterResumeDetails<MasterProResume_PPCMeta_VM>(new SP_ManageMasterProResumeDetails()
        //    {
        //        BusinessOwnerLoginId = BusinessLoginId,
        //        Mode = 3
        //    });

        //}

    }
}

