using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ControllerHiding.Attributes;
using ControllerHiding.Controllers.Base;
using ControllerHiding.DTO;
using ControllerHiding.Helper;

namespace ControllerHiding.Extensions
{
    public static class UrlExtensions
    {
        public static string LinkToPage(this UrlHelper urlhelper, Page page)
        {
            var currentBaseUrl = GetCurrentBaseUri(urlhelper.RequestContext.HttpContext.Request);
            return new Uri(currentBaseUrl, page.RoutePath).ToString();
        }

        public static string ChildAction(this UrlHelper urlhelper, string action, string identifier, object routeValues)
        {
            var currentController = urlhelper.RequestContext.RouteData.Values["controller"]?.ToString();
            return ChildAction(urlhelper, action, currentController, identifier, (RouteValueDictionary)HtmlHelper.ObjectToDictionary(routeValues));
        }

        public static string ChildAction(this UrlHelper urlhelper, string action, string controller, string identifier, object routeValues)
        {
            return ChildAction(urlhelper, action, controller, identifier, (RouteValueDictionary)HtmlHelper.ObjectToDictionary(routeValues));
        }

        public static string ChildAction(this UrlHelper urlhelper, string action, string controller, string identifier, RouteValueDictionary routeValues)
        {
            return GenerateUrl(urlhelper.RequestContext.HttpContext.Request.Url, action, controller, identifier, routeValues);
        }

        public static string ChildAction(this UrlHelper urlhelper, string action, string identifier, Page page, object routeValues)
        {
            var currentController = urlhelper.RequestContext.RouteData.Values["controller"]?.ToString();
            return ChildAction(urlhelper, action, currentController, identifier, page, (RouteValueDictionary)HtmlHelper.ObjectToDictionary(routeValues));
        }

        public static string ChildAction(this UrlHelper urlhelper, string action, string controller, string identifier, Page page, object routeValues)
        {
            return ChildAction(urlhelper, action, controller, identifier, page, (RouteValueDictionary)HtmlHelper.ObjectToDictionary(routeValues));
        }

        public static string ChildAction(this UrlHelper urlhelper, string action, string controller, string identifier, Page page, RouteValueDictionary routeValues)
        {
            var currentBaseUrl = GetCurrentBaseUri(urlhelper.RequestContext.HttpContext.Request);
            var url = new Uri(currentBaseUrl, page.RoutePath);

            return GenerateUrl(url, action, controller, identifier, routeValues);
        }

        private static Uri GetCurrentBaseUri(HttpRequestBase httpRequestBase)
        {
            Contract.Requires(httpRequestBase != null);
            Contract.Requires(httpRequestBase.Url != null);
            Contract.Requires(httpRequestBase.ApplicationPath != null);

            return new Uri(httpRequestBase.Url.Scheme + "://" + httpRequestBase.Url.Authority + httpRequestBase.ApplicationPath.TrimEnd('/') + "/");
        }

        private static string GenerateUrl(Uri uri, string action, string controller, string identifier, RouteValueDictionary routeValues)
        {
            Contract.Requires(uri != null);
            
            var absolutePath = uri.AbsolutePath;
            Dictionary<string, string> routeDefaults;
            string childRoute = ChildRouteHelper.GetChildRoute(action, controller, out routeDefaults);

            MatchCollection routeMachings = ChildRouteHelper.GetChildRouteMatchCollection(childRoute);
            string[] childRouteSegments = ChildRouteHelper.GetChildRouteSegments(routeValues, routeMachings, routeDefaults);

            var segmentIndex = GetIdentifierSegmentIndex(uri, identifier);
            if (segmentIndex == -1)
            {
                var childRoutePath = string.Join("/", childRouteSegments);
                absolutePath += $"/{identifier}/{childRoutePath}";
            }
            else
            {
                var newSegments = uri.Segments;
                int routeValueCount = 0;
                for (int i = segmentIndex + 1; i < segmentIndex + routeMachings.Count + 1; i++)
                {
                    string lastSign = newSegments[i].LastOrDefault().ToString();
                    if (lastSign != "/")
                    {
                        lastSign = "";
                    }
                    newSegments[i] = childRouteSegments[routeValueCount] + lastSign;
                    routeValueCount++;
                }
                absolutePath = string.Join("", newSegments);
            }

            return absolutePath + uri?.Query;
        }

        private static int GetIdentifierSegmentIndex(Uri uri, string identifier)
        {
            var segments = uri.Segments
                .Select(x => x.Replace("/", ""))
                .ToArray();
            return Array.IndexOf(segments, identifier);
        }
    }
}