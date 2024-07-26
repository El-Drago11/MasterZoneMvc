using MasterZoneMvc.DAL;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;

namespace MasterZoneMvc.Services
{
    public class MasterProfileService
    {

        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public MasterProfileService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }



        /// <summary>
        /// Explore Classic Dance Detail
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessExploreClassicDanceDetail_VM GetExploreClassicDanceDetail_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessExploreClassicDanceDetail<BusinessExploreClassicDanceDetail_VM>(new SP_ManageBusinessExploreClassicDanceDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }



        /// <summary>
        /// To Get Find  For Master Profile Detail 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        public BussinessContentFindMasterProfilelDetail_VM GetFindMasterProfileDetail_Get(long Id)
        {
            return storedPorcedureRepository.SP_ManageBusinessFindMasterProfileDetail<BussinessContentFindMasterProfilelDetail_VM>(new SP_ManageBusinessExploreClassicDanceDetail_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }




        /// <summary>
        /// To Get View Detail For Find Explore Detail List
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BussinessContentFindMasterProfilelDetail_VM> GetFindMasterProfileDetailList_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessFindMasterProfileDetaillst<BussinessContentFindMasterProfilelDetail_VM>(new SP_ManageBusinessExploreClassicDanceDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Delete Business Find Explore Detail 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBusinessContentFindExploreDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdateBusinessContentFindMasterProfilDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessFindMasterProfile_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }





        /// <summary>
        /// To get Find Explore Profile Detail For VisitorPanel List Show Through Explore Type 
        /// </summary>
        /// <param name="exploreType"></param>
        /// <returns></returns>
        public List<BussinessContentFindMasterProfilelDetail_VM> Get_FindMasterProfileDetailList(string exploreType)
        {
            return storedPorcedureRepository.SP_ManageBusinessFindMasterProfileDetaillst<BussinessContentFindMasterProfilelDetail_VM>(new SP_ManageBusinessExploreClassicDanceDetail_Param_VM()
            {
                ExploreType = exploreType,
                Mode = 3
            });
        }





        /// <summary>
        /// To Get All Classes Detail List
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <param name="lastRecordLimit"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<Class_VM> GetAllClassDetail_lst(long BusinessOwnerLoginId, long categoryTypeId, long lastRecordLimit, int recordLimit)
        {
            return storedPorcedureRepository.SP_ManageBusinessDetail_GetAll<Class_VM>(new SP_ManageBusinessDetail_Param_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Id = categoryTypeId,
                LastRecordId = lastRecordLimit,
                RecordLimit = recordLimit,
                Mode = 3
            });
        }




        /// <summary>
        /// To Show All Class Category Type Detail List 
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <param name="lastRecordLimit"></param>
        /// <param name="recordLimit"></param>
        /// <returns></returns>
        public List<Class_VM> GetAllShowClassCategoryDetail_lst(long BusinessOwnerLoginId, long lastRecordLimit, int recordLimit)
        {
            return storedPorcedureRepository.SP_ManageBusinessDetail_GetAll<Class_VM>(new SP_ManageBusinessDetail_Param_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                LastRecordId = lastRecordLimit,
                RecordLimit = recordLimit,
                Mode = 4
            });
        }



        /// <summary>
        /// To Get Instructor/MemberShip Banner Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public BusinessContentInstructorBannerDetail_VM GetInstructorBannerMasterProfileDetail(long Id)
        {
            return storedPorcedureRepository.SP_ManageBusinessInstructorBannerMasterProfileDetail<BusinessContentInstructorBannerDetail_VM>(new SP_ManageBusinessContentInstructorBannerDetail_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }



        /// <summary>
        /// To Get  Instructor/MemberShip Banner Detail List 
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessContentInstructorBannerDetail_VM> GetInstructorBannerMasterProfileDetail_Get(long BusinessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessInstructorBannerMasterProfileDetaillst<BusinessContentInstructorBannerDetail_VM>(new SP_ManageBusinessContentInstructorBannerDetail_Param_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 1
            });
        }




        /// <summary>
        /// To Delete Instructor Banner Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBusinessContentInstructorBannerDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdateBusinessContentInstructorBannerMasterProfileDetail_Get<SPResponseViewModel>(new SP_InsertUpdateBusinessContentInstructorBannerDetail_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }



        /// <summary>
        /// To Get Instructor About Master Profile Detail By LoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>

        public InstructorMasterProfileAboutDetail_VM GetInstructorAboutMasterProfileDetail_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessInstructorAboutMasterProfileDetail<InstructorMasterProfileAboutDetail_VM>(new SP_ManageInstructorAboutMasterProfileDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Term Condition Detail  By UserloginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessContentTermConditionDetail_VM GetTermConditionDetail_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessTermConditionDetail<BusinessContentTermConditionDetail_VM>(new SP_ManageBusinessTermConditionDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To get MemberShip Package Detail By LoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessMemberShipPackageDetail_VM GetMemberShipPackageDetail_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessMemberShipPackageDetail<BusinessMemberShipPackageDetail_VM>(new SP_ManageBusinessMemberShipPackageDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// To Get Member Ship Plan Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        public BusinessMemberShipPlanDetail_VM GetMemberShipPlanDetail_Get(long Id)
        {
            return storedPorcedureRepository.SP_ManageBusinessMemberShipPackageDetail<BusinessMemberShipPlanDetail_VM>(new SP_ManageBusinessMemberShipPackageDetail_Param_VM()
            {
                Id = Id,
                Mode = 2
            });
        }


        /// <summary>
        /// To Delete Member Ship Plan Detail By Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SPResponseViewModel DeleteBusinessContentMemberShipPlanDetail(long Id)
        {
            return storedPorcedureRepository.SP_InsertUpdateMemberShipPlanDetail_Get<SPResponseViewModel>(new SP_InsertUpdateMemberShipPlan_Param_VM()
            {
                Id = Id,
                Mode = 3
            });
        }


        /// <summary>
        /// To Get MemberShip Plan Detail List By BusinessOwnerLoginId
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<BusinessMemberShipPlanDetail_VM> GetMemberShipPlanDetailList_Get(long BusinessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessMemberShipPackageDetailList<BusinessMemberShipPlanDetail_VM>(new SP_ManageBusinessMemberShipPackageDetail_Param_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 3
            });
        }


        /// <summary>
        /// To Get Class Instructor Detail List 
        /// </summary>
        /// <param name="BusinessOwnerLoginId"></param>
        /// <returns></returns>
        public List<InstructorList_VM> GetClassInstructorDetailList_Get(long BusinessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageClass_GetAll<InstructorList_VM>(new SP_ManageClass_Params_VM()
            {
                UserLoginId = BusinessOwnerLoginId,
                Mode = 21
            });
        }
        /// <summary>
        /// to get advance content member ship
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessMemberShipPackageDetail_VM GetAdvanceMemberShipPackageDetail_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessAdvanceMemberShipPackageDetail<BusinessMemberShipPackageDetail_VM>(new SP_ManageBusinessMemberShipPackageDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }
        /// <summary>
        /// for profile page 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public BusinessMemberShipPackageDetail_VM GetAdvanceMemberShipPackage_Get(long businessOwnerLoginId)
        {
            return storedPorcedureRepository.SP_ManageBusinessAdvanceMemberShipPackageDetail<BusinessMemberShipPackageDetail_VM>(new SP_ManageBusinessMemberShipPackageDetail_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 2
            });
        }

    }
}