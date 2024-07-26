using MasterZoneMvc.Common;
using MasterZoneMvc.DAL;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class LoginService
    {
        private MasterZoneDbContext db;
        public LoginService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
        }
        public UserLoginViewModel GetLoginValidationInformation(LoginService_VM _Params)
        {
            SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", "0"),
                    new SqlParameter("email", _Params.Email),
                    new SqlParameter("password", EDClass.Encrypt(_Params.Password)),
                    new SqlParameter("socialLoginId",_Params.SocialLoginId),
                    new SqlParameter("masterId",_Params.MasterId),
                    new SqlParameter("mode", _Params.Mode)
                    };
            var st = EDClass.Encrypt(_Params.Password);
            UserLoginViewModel user = db.Database.SqlQuery<UserLoginViewModel>("exec sp_Login @id,@email,@password,@socialLoginId,@masterId,@mode", queryParams).FirstOrDefault();
            return user;
        }
    }
}