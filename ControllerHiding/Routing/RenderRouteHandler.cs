﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ControllerHiding.Routing
{
    /// <summary>
    /// In this route handler we gonna catch the names of the hidden controllers/actions and return their routehandler
    /// </summary>
    public class RenderRouteHandler : IRouteHandler
    {
        public RenderRouteHandler(IControllerFactory getControllerFactory)
        {
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpRequestBase = requestContext.HttpContext.Request;

            if (string.IsNullOrEmpty(httpRequestBase.Form["Controller"]))
            {
                return new MvcHandler(requestContext);
            }

            string hiddenController = null;
            string hiddenAction = null;
            //string area; //ignore area to make it easier in the beginning

            if (httpRequestBase.RequestType == "POST")
            {
                hiddenController = httpRequestBase.Form["Controller"];
                hiddenAction = httpRequestBase.Form["Action"];
            }
            else if (requestContext.HttpContext.Request.RequestType == "GET")
            {
                //later maybe we want to support GET as well
            }
            else
            {
                //we don't want to handle other RequestTypes
                return null;
            }

            requestContext.RouteData.DataTokens["MyHomepageRouteDefinition"] = new RouteDefinition()
            {
                ControllerName = "Home",
                ActionName = "Index",
            };

            requestContext.RouteData.Values["controller"] = hiddenController;
            requestContext.RouteData.Values["action"] = hiddenAction;

            IHttpHandler handler;
            using (RouteTable.Routes.GetReadLock())
            {
                //Route hiddenRoute;
                //var routes = RouteTable.Routes.OfType<Route>()
                //                                    .Where(x => x.Defaults != null &&
                //                                                x.Defaults.ContainsKey("controller") &&
                //                                                x.Defaults["controller"].ToString().Equals(hiddenController, StringComparison.InvariantCultureIgnoreCase) &&
                //                                                x.DataTokens.ContainsKey("area") == false).ToList();

                //// If more than one route is found, find one with a matching action
                //if (routes.Count() > 1)
                //{
                //    hiddenRoute = routes.FirstOrDefault(x =>
                //        x.Defaults["action"] != null &&
                //        x.Defaults["action"].ToString().Equals(hiddenAction, StringComparison.InvariantCultureIgnoreCase));
                //}
                //else
                //{
                //    hiddenRoute = routes.SingleOrDefault();
                //}

                Route hiddenRoute = RouteTable.Routes.OfType<Route>()
                    .SingleOrDefault(x =>
                        x.Defaults != null &&
                        x.Defaults["controller"].ToString() == hiddenController);

                if (hiddenRoute == null)
                {
                    throw new InvalidOperationException("Could not find a controller route for controller: " + hiddenController);
                }

                if (hiddenRoute.DataTokens.ContainsKey("Namespaces"))
                {
                    requestContext.RouteData.DataTokens["Namespaces"] = hiddenRoute.DataTokens["Namespaces"];
                }

                handler = hiddenRoute.RouteHandler.GetHttpHandler(requestContext);
            }

            return handler;
        }


    }
}