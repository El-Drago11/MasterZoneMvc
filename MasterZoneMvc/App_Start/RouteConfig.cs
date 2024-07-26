using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MasterZoneMvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Staff_Routes",
                url: "Staff/{action}/{id}",
                defaults: new { controller = "Business", action = "Dashboard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SubAdmin_Routes",
                url: "SubAdmin/{action}/{id}",
                defaults: new { controller = "SuperAdmin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
