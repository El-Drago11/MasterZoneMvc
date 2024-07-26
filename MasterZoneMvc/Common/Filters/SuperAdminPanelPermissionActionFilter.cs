using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MasterZoneMvc.Common.Filters
{
    public class SuperAdminPanelPermissionActionFilter : ActionFilterAttribute, IActionFilter
    {
        private MasterZoneDbContext db;
        public SuperAdminPanelPermissionActionFilter()
        {
            db = new MasterZoneDbContext();
        }

        private bool ValidateSubAdminCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieKeyNames.SubAdminCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }
        private string GetSubAdminLoginIdFromSubAdminCookie()
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieKeyNames.SubAdminCookie];
            string _LoginId = null;
            //-- if RestaurantLoginId Cookie not null
            if (myCookie != null)
            {
                string JWT_Token = myCookie["UserToken"];

                var stream = JWT_Token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var id = tokenS.Claims.First(claim => claim.Type == "loginid")?.Value;
                _LoginId = id;
            }
            return _LoginId;
        }
        private bool ValidateSuperAdminCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieKeyNames.SuperAdminCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }
        private string GetClaimValueFromCookie(string cookieName, string claimName)
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
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (!ValidateSuperAdminCookie() && !ValidateSubAdminCookie())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "SuperAdmin" }));
                base.OnActionExecuting(filterContext);
                return;
            }

            string UserRole = (ValidateSuperAdminCookie()) ? GetClaimValueFromCookie(CookieKeyNames.SuperAdminCookie, ClaimTypes.Role) : GetClaimValueFromCookie(CookieKeyNames.SubAdminCookie, ClaimTypes.Role);

            if (UserRole == "SuperAdmin")
            {
                PermisssionService permisssionService = new PermisssionService(db);
                filterContext.HttpContext.Items["UserRole"] = UserRole;
                //filterContext.HttpContext.Items["Permissions"] = permisssionService.GetAllPanelPermissionList(StaticResources.PanelType_Business);
                base.OnActionExecuting(filterContext);
                return;
            }
            // else get Sub-Admin Permissions 
            else
            {
                // Get subAdmin login id
                long subAdminLoginId = Convert.ToInt64(GetSubAdminLoginIdFromSubAdminCookie());

                // Retrieve the user's roles and permissions from the database
                PermisssionService permisssionService = new PermisssionService(db);
                var permissions = permisssionService.GetAllUserPermissions(subAdminLoginId);

                // Store the user's permissions in the Front end 
                filterContext.HttpContext.Items["Permissions"] = permissions;

            }
            filterContext.HttpContext.Items["UserRole"] = UserRole;
            base.OnActionExecuting(filterContext);
            return;

        }

    }
}