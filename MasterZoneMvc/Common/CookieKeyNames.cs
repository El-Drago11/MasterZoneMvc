using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common
{
    public static class CookieKeyNames
    {
        public static string CookieNameSuffix = "_MZ"; //Masterzone
        public static string BusinessAdminCookie = "BusinessAdminCookie" + CookieNameSuffix;
        public static string StaffCookie = "StaffCookie" + CookieNameSuffix;
        public static string SubAdminCookie = "SubAdminCookie" + CookieNameSuffix;
        public static string SuperAdminCookie = "SuperAdminCookie" + CookieNameSuffix;
        public static string StudentCookie = "StudentCookie" + CookieNameSuffix;
        public static string SidebarCookie = "SidebarCookie" + CookieNameSuffix;
    }
}   