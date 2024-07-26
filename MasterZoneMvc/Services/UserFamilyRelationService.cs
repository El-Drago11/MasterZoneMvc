using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MasterZoneMvc.Common;

namespace MasterZoneMvc.Services
{
    public class UserFamilyRelationService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public UserFamilyRelationService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Insert User Family Relation Record 
        /// Mode not required
        /// </summary>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel InsertUserFamilyRelation(SP_InsertUpdateUserFamilyRelation_Params_VM params_VM)
        {
            return storedPorcedureRepository.SP_InsertUpdateUserFamilyRelation_Get<SPResponseViewModel>(new SP_InsertUpdateUserFamilyRelation_Params_VM()
            {
                Id = params_VM.Id,
                User1LoginId = params_VM.User1LoginId,
                User2LoginId = params_VM.User2LoginId,
                User1Relation_FieldTypeCatalogKey = params_VM.User1Relation_FieldTypeCatalogKey,
                User2Relation_FieldTypeCatalogKey = params_VM.User2Relation_FieldTypeCatalogKey,
                SubmittedByLoginId = params_VM.SubmittedByLoginId,
                Mode = 1
            });
        }

        /// <summary>
        /// Get User Relation by the its gender and the relation selected for the other person.
        /// </summary>
        /// <param name="user1Gender">User Gender</param>
        /// <param name="user2FamilyRelationKeyName">Other User Relation with User1(Gender)</param>
        /// <returns>The User1 Relation with User2 Key Name, if not found then returns empty string</returns>
        public string GetReverseFamilyRelationKeyName(int user1Gender, string user2FamilyRelationKeyName)
        {
            List<UserRelationManagement_VM> AllRelations = new List<UserRelationManagement_VM>
            {
                // For Gender 1
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Father_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Son_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Mother_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Son_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_GrandFather_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandSon_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_GrandMother_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandSon_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Wife_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Husband_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Uncle_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Nephew_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Aunt_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Nephew_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Brother_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Brother_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_SisterInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_BrotherInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Sister_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Brother_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_BrotherInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_BrotherInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Son_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Father_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_DaughterInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_FatherInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Daughter_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Father_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_SonInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_FatherInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_GrandSon_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandFather_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Nephew_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Uncle_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Niece_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Uncle_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_GrandDaughter_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandFather_KeyName },
                new UserRelationManagement_VM() { User1Gender = 1, User2_FamilyRelationKey = StaticResources.FRT_Cousin_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Cousin_KeyName },

                // For Gender 2
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Father_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Daughter_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Mother_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Daughter_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_GrandFather_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandDaughter_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_GrandMother_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandDaughter_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Husband_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Wife_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Uncle_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Niece_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Aunt_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Niece_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Brother_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Sister_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_SisterInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_SisterInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Sister_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Sister_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_BrotherInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_SisterInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Son_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Mother_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_DaughterInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_MotherInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Daughter_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Mother_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_SonInLaw_KeyName, User1_FamilyRelationKey = StaticResources.FRT_MotherInLaw_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_GrandSon_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandMother_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Nephew_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Aunt_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Niece_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Aunt_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_GrandDaughter_KeyName, User1_FamilyRelationKey = StaticResources.FRT_GrandMother_KeyName },
                new UserRelationManagement_VM() { User1Gender = 2, User2_FamilyRelationKey = StaticResources.FRT_Cousin_KeyName, User1_FamilyRelationKey = StaticResources.FRT_Cousin_KeyName },

            };

            UserRelationManagement_VM user1Relation = AllRelations.Where(x => x.User1Gender == user1Gender && x.User2_FamilyRelationKey == user2FamilyRelationKeyName).FirstOrDefault();
            if (user1Relation == null)
                return "";
            return user1Relation.User1_FamilyRelationKey;
        }
        
        /// <summary>
        /// Get User Family Members List by User's MasterId
        /// </summary>
        /// <param name="MasterId">User Master Id whose member list to fetch</param>
        /// <returns>Family Members List</returns>
        public List<UserFamilyMemberRelation_VM> GetAllFamilyMembersByUserMasterId(string MasterId)
        {
            return storedPorcedureRepository.SP_ManageUserFamilyRelations_GetList<UserFamilyMemberRelation_VM>(new PageTemplateViewModels.PageTemplateStoreProcedureViewModel.SP_ManageUserFamilyRelations_Param_VM()
            {
                MasterId = MasterId,
                Mode = 1
            });
        }
        
        /// <summary>
        /// Get User Family Members List by User's Login-Id
        /// </summary>
        /// <param name="userLoginId">User Login Id whose member list to fetch</param>
        /// <returns>Family Members List</returns>
        public List<UserFamilyMemberRelation_VM> GetAllFamilyMembersByUserLoginId(long userLoginId)
        {
            return storedPorcedureRepository.SP_ManageUserFamilyRelations_GetList<UserFamilyMemberRelation_VM>(new PageTemplateViewModels.PageTemplateStoreProcedureViewModel.SP_ManageUserFamilyRelations_Param_VM()
            {
                User1LoginId = userLoginId,
                Mode = 2
            });
        }

    }
}