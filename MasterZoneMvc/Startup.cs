using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web.Configuration;

[assembly: OwinStartup(typeof(MasterZoneMvc.Startup))]

namespace MasterZoneMvc
{
    public class Startup
    {
        private string JwtIssuer = WebConfigurationManager.AppSettings["JWTIssuer"];
        private string JwtSecretKey = WebConfigurationManager.AppSettings["JWTSecretKey"];
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            object p = app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = JwtIssuer, //some string, normally web url,  
                        ValidAudience = JwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey))
                    }
                });
        }
    }
}
