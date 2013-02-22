using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace misechko.com
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Read",
               url: "Read/{*contentKey}",
               defaults: new { controller = "Read", action = "Index", contentKey = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "PressCenter",
               url: "Press/{submenu}",
               defaults: new { controller = "PressCenter", action = "Index", submenu = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "About",
               url: "About/{submenu}",
               defaults: new { controller = "About", action = "Index", submenu = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Practicies",
                url: "Practicies/{practice}",
                defaults: new { controller = "Practicies", action = "Index", practice = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Industries",
                url: "Industries/{industry}",
                defaults: new { controller = "Industries", action = "Index", industry = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("NotFound", "{*url}",
                new { controller = "Error", action = "Http404" });
        }
    }
}