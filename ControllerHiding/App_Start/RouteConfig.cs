using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using ControllerHiding.Routing;

namespace ControllerHiding
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var defaultRoute = routes.MapRoute(
                "Page",
                "{*path}", //this catches everything
                new { controller = "Home", action = "Index", path = UrlParameter.Optional },
                new { path = new PagePathConstraint() }
            );
            defaultRoute.RouteHandler = new RenderRouteHandler(ControllerBuilder.Current.GetControllerFactory());

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "HiddenFormController",
                url: "{controller}/{action}",
                defaults: new { controller = "HiddenForm", action = "Index" }
            );

            routes.MapRoute(
                name: "AnotherHiddenFormController",
                url: "{controller}/{action}",
                defaults: new { controller = "AnotherHiddenForm", action = "Index" }
            );
        }
    }
}
