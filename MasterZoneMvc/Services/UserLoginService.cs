using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class UserLoginService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        public UserLoginService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Generate Unique-User-Id
        /// </summary>
        /// <returns>Unique-User-Id</returns>
        public string GenerateRandomUniqueUserId()
        {
            Random random = new Random();
            
            int flag = 0;
            string UniqueId = "";

            while (flag == 0)
            {
                long randNumber = random.Next(StaticResources.UserId_MinValue, StaticResources.UserId_MaxValue);

                UniqueId = randNumber.ToString();

                var user = storedProcedureRepository.SP_ManageUserLogin_Get<UserLogin>(new ViewModels.StoredProcedureParams.SP_ManageUserLogin_Params_VM()
                {
                    UniqueUserId = UniqueId,
                    Mode = 1
                });

                if(user == null)
                {
                    flag = 1;
                }
            }

            return UniqueId;
        }

        /// <summary>
        /// Generate Unique-User-Id With Prefix
        /// </summary>
        /// <param name="prefix">Prefix to User-Id (Defined in Static Resources)</param>
        /// <returns>Unique-User-Id</returns>
        public string GenerateRandomUniqueUserId(string prefix)
        {
            Random random = new Random();
            int flag = 0;
            string UniqueId = "";

            while (flag == 0)
            {
                long randNumber = random.Next(StaticResources.UserId_MinValue, StaticResources.UserId_MaxValue);

                UniqueId = prefix + randNumber.ToString();

                var user = storedProcedureRepository.SP_ManageUserLogin_Get<UserLogin>(new ViewModels.StoredProcedureParams.SP_ManageUserLogin_Params_VM()
                {
                    UniqueUserId = UniqueId,
                    Mode = 1
                });

                if (user == null)
                {
                    flag = 1;
                }
            }

            return UniqueId;
        }

        public UserLogin GetUserLoginData(long userLoginId)
        {
            var user = storedProcedureRepository.SP_ManageUserLogin_Get<UserLogin>(new ViewModels.StoredProcedureParams.SP_ManageUserLogin_Params_VM()
            {
                Id = userLoginId,
                Mode = 4
            });
            return user;
        }
    }
}