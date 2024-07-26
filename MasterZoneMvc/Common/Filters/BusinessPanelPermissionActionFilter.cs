using MasterZoneMvc.DAL;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
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
    public class BusinessPanelPermissionActionFilter : ActionFilterAttribute, IActionFilter
    {
        private MasterZoneDbContext db;
        public BusinessPanelPermissionActionFilter()
        {
            db = new MasterZoneDbContext();
        }

        #region private members
        private bool ValidateStaffCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieKeyNames.StaffCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }

        private string GetStaffLoginIdFromStaffCookie()
        {
            HttpCookie myCookie_Customer = HttpContext.Current.Request.Cookies[CookieKeyNames.StaffCookie];
            string _LoginId = null;
            //-- if RestaurantLoginId Cookie not null
            if (myCookie_Customer != null)
            {
                string JWT_Token = myCookie_Customer["UserToken"];

                var stream = JWT_Token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var id = tokenS.Claims.First(claim => claim.Type == "loginid")?.Value;
                _LoginId = id;
            }
            return _LoginId;
        }
        private bool ValidateBusinessAdminCookie()
        {
            bool _isValid = false;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieKeyNames.BusinessAdminCookie];

            if (myCookie != null)
            {
                _isValid = true;
            }

            return _isValid;
        }
        private string GetLoginIdFromBusinessAdminCookie()
        {
            HttpCookie myCookie_Customer = HttpContext.Current.Request.Cookies[CookieKeyNames.BusinessAdminCookie];
            string _LoginId = null;
            //-- if Cookie not null
            if (myCookie_Customer != null)
            {
                string JWT_Token = myCookie_Customer["UserToken"];

                var stream = JWT_Token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var id = tokenS.Claims.First(claim => claim.Type == "loginid")?.Value;
                _LoginId = id;
            }
            return _LoginId;
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

        #endregion

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Commented: Because creating issue on gettingStaffToken in common ChangePasswordFile.
            // So move staffToken Code to another controller or make check in custom action methods
            //// if invalid login cookie then return
           
            if (!ValidateStaffCookie() && !ValidateBusinessAdminCookie())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Business" }));
                base.OnActionExecuting(filterContext);
                return;
            }

            string UserRole = (ValidateBusinessAdminCookie()) ? GetClaimValueFromCookie(CookieKeyNames.BusinessAdminCookie, ClaimTypes.Role) : GetClaimValueFromCookie(CookieKeyNames.StaffCookie, ClaimTypes.Role);

            if (UserRole == "BusinessAdmin")
            {
                PermisssionService permisssionService = new PermisssionService(db);
                filterContext.HttpContext.Items["UserRole"] = UserRole;
                filterContext.HttpContext.Items["Permissions"] = permisssionService.GetAllPanelPermissionList(StaticResources.PanelType_Business);

                BusinessProfileTypeKey_VM ProfileTypeKeyResponse = new BusinessProfileTypeKey_VM();
                BusinessOwnerService businessOwnerService = new BusinessOwnerService(db);
                ProfileTypeKeyResponse = businessOwnerService.GetBusinessProfileTypeDetailsById(Convert.ToInt64(GetLoginIdFromBusinessAdminCookie()));

                filterContext.HttpContext.Items["ProfilePageTypeKey"] = ProfileTypeKeyResponse.Key;

                base.OnActionExecuting(filterContext);
                return;
            }

            // else get staff permissions 
            else
            {
                // Get staff login id
                long staffLoginId = Convert.ToInt64(GetStaffLoginIdFromStaffCookie());

                // Retrieve the user's roles and permissions from the database
                PermisssionService permisssionService = new PermisssionService(db);
                var permissions = permisssionService.GetAllUserPermissions(staffLoginId);

                // Store the user's permissions in the Front end 
                filterContext.HttpContext.Items["Permissions"] = permissions;

                BusinessProfileTypeKey_VM ProfileTypeKeyResponse = new BusinessProfileTypeKey_VM();
                BusinessOwnerService businessOwnerService = new BusinessOwnerService(db);
                ProfileTypeKeyResponse = businessOwnerService.GetBusinessProfileTypeDetailsById(Convert.ToInt64(GetClaimValueFromCookie(CookieKeyNames.StaffCookie, "businessAdminLoginId")));

                filterContext.HttpContext.Items["ProfilePageTypeKey"] = ProfileTypeKeyResponse.Key;
            }

            filterContext.HttpContext.Items["UserRole"] = UserRole;
            base.OnActionExecuting(filterContext);
            return;

        }
    }
}