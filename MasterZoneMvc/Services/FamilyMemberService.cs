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
    public class FamilyMemberService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedPorcedureRepository;

        public FamilyMemberService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedPorcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Insert or Update Family Member Record
        /// </summary>
        /// <param name="params_VM">Stored-Procedure Params</param>
        /// <returns>Success or error response</returns>
        public SPResponseViewModel InsertUpdateFamilyMember(SP_InsertUpdateFamilyMember_Params_VM params_VM)
        {
            return storedPorcedureRepository.SP_InsertUpdateFamilyMember_Get<SPResponseViewModel>(new SP_InsertUpdateFamilyMember_Params_VM()
            {
                Id = params_VM.Id,
                UserLoginId = params_VM.UserLoginId,
                FirstName = params_VM.FirstName,
                LastName = params_VM.LastName,
                Gender = params_VM.Gender,
                ProfileImage = params_VM.ProfileImage,
                Relation = params_VM.Relation,
                Mode = params_VM.Mode
            });
        }

        /// <summary>
        /// Get All Family Members List by User (User-Login-Id)
        /// </summary>
        /// <param name="userLoginId">User-Login-Id</param>
        /// <returns>List of Family Members</returns>
        public List<FamilyMember_VM> GetAllFamilyMembersByUser(long userLoginId)
        {
            return storedPorcedureRepository.SP_ManageFamilyMember_GetAll<FamilyMember_VM>(new SP_ManageFamilyMember_Params_VM()
            {
                UserLoginId = userLoginId,
                Mode = 1
            });
        }

    }
}