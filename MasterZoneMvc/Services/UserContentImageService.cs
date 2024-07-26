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
    public class UserContentImageService
    {

        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public UserContentImageService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        #region
        /// <summary>
        /// User Content Image Record Detail
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns>Get All User Content Image Detail</returns>
        public List<UserContentImagesDetail_VM> GetAllUserContentImageDetail(long UserLoginId)
        {
            return storedPorcedureRepository.SP_ManageUserContentImage_GetList<UserContentImagesDetail_VM>(new SP_ManageUserContentImage_Params_VM
            {
                UserLoginId = UserLoginId,
                Mode = 1


            });

        }
        #endregion

        #region
        /// <summary>
        /// User Content Image Record Detail By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Get  User Content Image Detail</returns>
        public UserContentImagesDetail_VM GetUserContentImageDetail(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_ManageUserContentImage_GetSingle<UserContentImagesDetail_VM>(new SP_ManageUserContentImage_Params_VM
            {
                Id = Id,
                Mode = 2


            });

        }
        #endregion
        #region
        /// <summary>
        /// User Content Image Record Detail Delete By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> To Delete   User Content Image Detail</returns>
        public SPResponseViewModel DeleteUserContentImage(long Id)
        {
            storedPorcedureRepository = new StoredProcedureRepository(db);

            return storedPorcedureRepository.SP_InsertUpdateUserContentImage<SPResponseViewModel>(new SP_InsertUpdateUserContentImage_Params_VM
            {
                Id = Id,
                Mode = 3


            });

        }
        #endregion

    }
}