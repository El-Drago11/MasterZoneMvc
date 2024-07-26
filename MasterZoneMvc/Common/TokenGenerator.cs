using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Configuration;

namespace MasterZoneMvc.Common
{
    public class TokenGenerator
    {
        private string JwtIssuer = WebConfigurationManager.AppSettings["JWTIssuer"];
        private string JwtSecretKey = WebConfigurationManager.AppSettings["JWTSecretKey"];

        /// <summary>
        /// Generates Token for user
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="user"></param>
        /// <returns>Generated Token string</returns>
        public string Create_JWT(Int64 _LoginID, string _UserRole, int rememberMe, long _BusinessAdminLoginId = 0)
        {
            string key = JwtSecretKey; //Secret key which will be used later during validation    
            var issuer = JwtIssuer;  //normally this will be your site URL  

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("loginid", _LoginID.ToString()));
            permClaims.Add(new Claim("businessAdminLoginId", _BusinessAdminLoginId.ToString()));

            // Add roles as multiple claims
            // foreach (var role in user.Roles)
            //{
            permClaims.Add(new Claim(ClaimTypes.Role, _UserRole));
            //}

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
        }
    }
}