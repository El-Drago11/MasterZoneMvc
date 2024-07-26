using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class UserContentVedioService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public UserContentVedioService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// User Content Video Record Detail
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns>Get All User Content Video Detail</returns>
        public List<UserContentVideo_VM> GetAllUserContentVedioDetail(long UserLoginId)
        {
            return storedPorcedureRepository.SP_ManageUserContentVideos_GetList<UserContentVideo_VM>(new SP_ManageUserContentVideos_Param_VM
            {
                UserLoginId = UserLoginId,
                Mode = 1


            });

        }

        /// <summary>
        /// User Content Video Record Detail By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Get  User Content Video Detail</returns>
        public UserContentVideo_VM GetUserContentVedioDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageUserContentVideo_GetSingle<UserContentVideo_VM>(new SP_ManageUserContentVideos_Param_VM
            {
                Id = Id,
                Mode = 2


            });

        }

        /// <summary>
        /// User Content Video Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   User Content Video Detail</returns>
        public SPResponseViewModel DeleteUserContentVedio(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateUserContentVideos<SPResponseViewModel>(new SP_InsertUpdateUserContentVideo_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }
       

    }
}