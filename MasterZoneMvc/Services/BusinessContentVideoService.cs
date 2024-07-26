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
    public class BusinessContentVideoService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;

        public BusinessContentVideoService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// Insert-Update Business-Content-Video
        /// </summary>
        /// <param name="params_VM">Video Detail Data</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel InsertBusinessContentVideo(SP_InsertUpdateBusinessVideos_Params_VM params_VM)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateBusinessVideos<SPResponseViewModel>(new SP_InsertUpdateBusinessVideos_Params_VM()
            {
                Id = params_VM.Id,
                UserLoginId = params_VM.UserLoginId,
                BusinessContentVideoCategoryId = params_VM.BusinessContentVideoCategoryId,
                SubmittedByLoginId = params_VM.SubmittedByLoginId,
                Thumbnail = params_VM.Thumbnail,
                VideoLink = params_VM.VideoLink,
                VideoTitle = params_VM.VideoTitle,
                Description = params_VM.Description,
                Mode = 1
            });
        }

        /// <summary>
        /// Get All Business-Content-Videos by Business-Owner-Login-Id
        /// </summary>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>List</returns>
        public List<BusinessVideoResponse_VM> GetAllBusinessContentVideos(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessVideos_GetAll<BusinessVideoResponse_VM>(new SP_ManageBusinessVideos_Params_VM()
            {
                UserLoginId = UserLoginId,
                Mode = 1
            });
        }
        
        /// <summary>
        /// Get Random Business-Content-Videos by Business-Owner-Login-Id (Top 4)
        /// </summary>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>List</returns>
        public List<BusinessVideoResponse_VM> GetRandomBusinessContentVideos()
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessVideos_GetAll<BusinessVideoResponse_VM>(new SP_ManageBusinessVideos_Params_VM()
            {
                Mode = 2
            });
        }


        /// <summary>
        /// Delete Business Vedio by Id and Business-Owner-Login-Id
        /// </summary>
        /// <param name="Id">Video-Id</param>
        /// <param name="UserLoginId">Business-Owner-Login-Id</param>
        /// <returns>Status 1 if deleted, else -ve value with message</returns>
        public SPResponseViewModel DeleteBusinessVideo(long Id, long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_InsertUpdateBusinessVideos<SPResponseViewModel>(new SP_InsertUpdateBusinessVideos_Params_VM()
            {
                Id = Id,
                UserLoginId = UserLoginId,
                Mode = 2
            });
        }

        /// <summary>
        /// To Get (Video ) detail By LastRecordedId 
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <param name="lastRecordedId"></param>
        /// <param name="RecordLimit"></param>
        /// <returns></returns>
        public List<BusinessVideoResponse_VM> GetAllBusinessDetaillst(long businessOwnerLoginId, long lastRecordedId, int RecordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessDetail_GetAll<BusinessVideoResponse_VM>(new SP_ManageBusinessDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                LastRecordId = lastRecordedId,
                RecordLimit = RecordLimit,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Business Images Detail By LastRecordedId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="lastRecordedId"></param>
        /// <param name="RecordLimit"></param>
        /// <returns></returns>
        public List<BusinessImageResponse_VM> GetAllBusinessImagesDetaillst(long businessOwnerLoginId, long lastRecordedId, int RecordLimit)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessDetail_GetAll<BusinessImageResponse_VM>(new SP_ManageBusinessDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                LastRecordId = lastRecordedId,
                RecordLimit = RecordLimit,
                Mode = 2
            });
        }


        /// <summary>
        /// To  Get Business Detail ALL Images /Videos Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<BusinessResponseUserDetail> GetAllBusinessUserDetaillst(long businessOwnerLoginId, int pageSize, int pageNumber)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessUserDetail_GetAll<BusinessResponseUserDetail>(new SP_ManageBusinessUserDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Mode = 1
            });
        }
        /// <summary>
        /// to get video details
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<MasterProResponseUserDetail> GetAllVideoListByBusiness(long businessOwnerLoginId, int pageSize, int pageNumber)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessUserDetail_GetAll<MasterProResponseUserDetail>(new SP_ManageBusinessUserDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Mode = 2
            });
        }
        /// <summary>
        /// to get pdf details
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<MasterProResponseUserDetail> GetAllPDFListByBusiness(long businessOwnerLoginId, int pageSize, int pageNumber)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessUserDetail_GetAll<MasterProResponseUserDetail>(new SP_ManageBusinessUserDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Mode = 3
            });
        }

        #region Business-Content-Video-Categories ------------------------------------------


        /// <summary>
        /// Get All active Business-Content-Video Categories 
        /// </summary>
        /// <returns>List</returns>
        public List<BusinessContentVideoCategory_VM> GetAllActiveBusinessVideoCategories()
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessContentVideoCategories_GetAll<BusinessContentVideoCategory_VM>(new SP_ManageBusinessContentVideoCategories_Params_VM()
            {
                Mode = 1
            });
        }


        #endregion Business-Content-Video-Categories ------------------------------------------
        /// <summary>
        /// to get class details   
        /// </summary>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        public List<BusinessAllClassesDetail> GetAllBusinessClassDetails(long UserLoginId)
        {
            storedProcedureRepository = new StoredProcedureRepository(db);
            return storedProcedureRepository.SP_ManageBusinessVideos_GetAll<BusinessAllClassesDetail>(new SP_ManageBusinessVideos_Params_VM()
            {
                UserLoginId = UserLoginId,
                Mode = 3
            });
        }
    }
}