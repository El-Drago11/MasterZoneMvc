using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common
{
    public class CookieHelper
    {
        public void RemoveOrExpireLoginCookies(HttpResponseBase Response)
        {
            DateTime expireTime = DateTime.Now.AddMonths(-1);
            
            //Remove User/Student login cookie
            HttpCookie myCookieStudentCookie = new HttpCookie(CookieKeyNames.StudentCookie);
            myCookieStudentCookie.Expires = expireTime;
            Response.Cookies.Add(myCookieStudentCookie);

            //Remove Business Admin login cookie
            HttpCookie myCookieBusinessAdminn = new HttpCookie(CookieKeyNames.BusinessAdminCookie);
            myCookieBusinessAdminn.Expires = expireTime;
            Response.Cookies.Add(myCookieBusinessAdminn);

            //Remove Staff login cookie
            HttpCookie myCookieStaff = new HttpCookie(CookieKeyNames.StaffCookie);
            myCookieStaff.Expires = expireTime;
            Response.Cookies.Add(myCookieStaff);

            //Remove SubAdmin login cookie
            HttpCookie myCookieSubAdmin = new HttpCookie(CookieKeyNames.SubAdminCookie);
            myCookieSubAdmin.Expires = expireTime;
            Response.Cookies.Add(myCookieSubAdmin);

            //Remove SuperAdmin login cookie
            HttpCookie myCookieSuperAdmin = new HttpCookie(CookieKeyNames.SuperAdminCookie);
            myCookieSuperAdmin.Expires = expireTime;
            Response.Cookies.Add(myCookieSuperAdmin);
        }

        /// <summary>
        /// Reset Sidebar cookie value for the selected link
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="value"></param>
        public void ResetSidebarCookie(HttpResponseBase Response, string value)
        {
            //Remove Sidebar cookie detail
            HttpCookie myCookie = new HttpCookie(CookieKeyNames.SidebarCookie);
            myCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(myCookie);

            HttpCookie myCookie_Sidebar = new HttpCookie(CookieKeyNames.SidebarCookie);
            myCookie_Sidebar["SelectedLink"] = value;
            myCookie_Sidebar.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Add(myCookie_Sidebar);
        }

        /// <summary>
        /// Get Claim value from JWT-Token by Cookie name and the claim name
        /// </summary>
        /// <param name="cookieName">Cookie Name</param>
        /// <param name="claimName">Claim Name</param>
        /// <returns>Claim Value if found else null</returns>
        public string GetClaimValueFromCookie(string cookieName, string claimName)
        {
            HttpCookie myCookie_Customer = HttpContext.Current.Request.Cookies[cookieName];
            string _Value = null;

            //-- if Cookie not null
            if (myCookie_Customer != null)
            {
                string JWT_Token = myCookie_Customer["UserToken"];

                var stream = JWT_Token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var claimValue = tokenS.Claims.First(claim => claim.Type == claimName)?.Value;
                _Value = claimValue;
            }
            return _Value;
        }
    }
}