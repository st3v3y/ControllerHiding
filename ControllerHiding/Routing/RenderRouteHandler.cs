using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ControllerHiding.Constants;
using ControllerHiding.DTO;
using ControllerHiding.Extensions;
using ControllerHiding.Helper;
using Module = ControllerHiding.DTO.Module;

namespace ControllerHiding.Routing
{
    /// <summary>
    /// In this route handler we gonna catch the names of the hidden controllers/actions and return their routehandler
    /// </summary>
    public class RenderRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpRequestBase = requestContext.HttpContext.Request;

            ValidateRouteData(requestContext);

            string formData = httpRequestBase.Form.Get(KeyConstants.FormData);
            string getData = httpRequestBase.QueryString.Get(KeyConstants.GetData);

            if (string.IsNullOrEmpty(formData) && string.IsNullOrEmpty(getData))
            {
                return new MvcHandler(requestContext);
            }

            string hiddenController = null;
            string hiddenAction = null;
            //string area; //ignore area to make it easier in the beginning
            string identifier = null;

            if (httpRequestBase.RequestType == "POST")
            {
                var hiddenFormData = formData.Decrypt<HiddenFormData>();

                hiddenController = hiddenFormData.Controller;
                hiddenAction = hiddenFormData.Action;
                identifier = hiddenFormData.Identifier;
            }
            else if (requestContext.HttpContext.Request.RequestType != "GET")
            {
                return null;
            }

            requestContext.RouteData.Values["controller"] = hiddenController;
            requestContext.RouteData.Values["action"] = hiddenAction;
            requestContext.RouteData.Values[KeyConstants.Identifier] = identifier;

            return new MvcHandler(requestContext);
        }

        private static void ValidateRouteData(RequestContext requestContext)
        {
            var page = requestContext.RouteData.DataTokens["page"] as Page;
            var urlParams = requestContext.RouteData.DataTokens["params"] as string[];

            if (page == null || urlParams == null || urlParams.Any(string.IsNullOrWhiteSpace))
            {
                SendHttpNotFoundResponse(requestContext);
                return;
            }

            IEnumerable<RouteInformation> usedIdentifierRoutes = GetUsedIdentifierRoutes(page, urlParams);
            if (!ValidateUsedIdentifierRoutes(usedIdentifierRoutes, urlParams.ToList()))
            {
                SendHttpNotFoundResponse(requestContext);
            }
        }

        private static bool ValidateUsedIdentifierRoutes(IEnumerable<RouteInformation> usedIdentifierRoutes, List<string> checkParams)
        {
            foreach (var usedIdentifierRoute in usedIdentifierRoutes)
            {
                var identifierIndex = checkParams.IndexOf(usedIdentifierRoute.Identifier);
                if (identifierIndex == -1)
                {
                    continue;
                }

                checkParams.RemoveRange(identifierIndex, 1);

                var matchingIndex = identifierIndex;
                foreach (var routeMatching in usedIdentifierRoute.RouteMatchings)
                {
                    if (!IsValidRouteMatching(usedIdentifierRoute, routeMatching, matchingIndex, checkParams))
                    {
                        return false;
                    }
                    checkParams.RemoveRange(matchingIndex, 1);
                }
            }

            return checkParams.Count <= 0;
        }

        private static bool IsValidRouteMatching(RouteInformation usedIdentifierRoute, string routeMatching, int matchingIndex, List<string> checkParams)
        {
            var matchingParameter = usedIdentifierRoute.RouteParameters.FirstOrDefault(x => x.Name == routeMatching);
            if (matchingIndex >= checkParams.Count || matchingParameter == null)
            {
                return false;
            }

            var matchingValue = checkParams[matchingIndex];
            if (ValidateDefaults(matchingParameter.ParameterType, usedIdentifierRoute.Defaults, routeMatching, matchingValue))
            {
                return true;
            }

            var typeConverter = TypeDescriptor.GetConverter(matchingParameter.ParameterType);
            if (!typeConverter.CanConvertFrom(typeof (string)) || !typeConverter.IsValid(matchingValue))
            {
                return false;
            }

            object value = typeConverter.ConvertFromString(matchingValue);
            if (!ValidateConstraints(value, usedIdentifierRoute.Constraints, routeMatching))
            {
                return false;
            }

            return true;
        }

        private static bool ValidateDefaults(Type parameterType, Dictionary<string,string> defaults, string routeMatching, string matchingValue)
        {
            var isNullable = Nullable.GetUnderlyingType(parameterType) != null;
            if (!isNullable)
            {
                return false;
            }

            string defaultValue;
            if (!defaults.TryGetValue(routeMatching, out defaultValue) || defaultValue != matchingValue)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateConstraints(object value, Dictionary<string, Func<object, bool>[]> constraints, string routeMatching)
        {
            Func<object, bool>[] paramConstraints = constraints.FirstOrDefault(x => x.Key == routeMatching).Value;
            return paramConstraints == null || paramConstraints.All(validation => validation(value));
        }

        private static IEnumerable<RouteInformation> GetUsedIdentifierRoutes(Page page, string[] urlParams)
        {
            var modulesOnPage = page.ModuleAreas.SelectMany(x => x.Modules);
            var allIdentifiers = modulesOnPage.GroupBy(x => x.Identifier, x => x, (x, y) => new KeyValuePair<string, IEnumerable<Module>>(x, y));
            var usedIdentifiers = allIdentifiers.Where(x => urlParams.Contains(x.Key));

            return usedIdentifiers.Select(x => GetRouteMatches(x.Key, x.Value.ToArray()));
        }

        private static RouteInformation GetRouteMatches(string identifier, Module[] modules)
        {
            ParameterInfo[] parameters;
            Dictionary<string, string> defaults;
            Module module = modules.First(); //TODO: Really just take the first one?!
            Dictionary<string, Func<object, bool>[]> constraints;
            string childRoute = ChildRouteHelper.GetChildRoute(module.Action, module.Controller, out defaults, out parameters, out constraints);
            MatchCollection routeMachings = ChildRouteHelper.GetChildRouteMatchCollection(childRoute);

            string[] childRouteMatches = (from Match routeMaching in routeMachings select routeMaching.Value).ToArray();
            ParameterInfo[] routeParameters = parameters.Where(x => childRouteMatches.Contains(x.Name)).ToArray();

            return new RouteInformation()
            {
                Identifier = identifier,
                RouteMatchings = childRouteMatches,
                Defaults = defaults,
                Modules = modules,
                RouteParameters = routeParameters,
                Constraints = constraints
            };
        }

        internal class RouteInformation
        {
            public string Identifier { get; set; }
            public string[] RouteMatchings { get; set; }
            public Dictionary<string, string> Defaults { get; set; }
            public Module[] Modules { get; set; }
            public ParameterInfo[] RouteParameters { get; set; }
            public Dictionary<string, Func<object, bool>[]> Constraints { get; set; }
        }

        private static void SendHttpNotFoundResponse(RequestContext requestContext)
        {
            requestContext.HttpContext.Response.ClearHeaders();
            requestContext.HttpContext.Response.Clear();

            requestContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            requestContext.HttpContext.Response.SuppressContent = true;
            requestContext.HttpContext.Response.End();
        }
    }

    internal class IdentifierComparer : IEqualityComparer<Module>
    {
        public bool Equals(Module x, Module y)
        {
            return x.Identifier == y.Identifier;
        }

        public int GetHashCode(Module obj)
        {
            return obj.GetHashCode();
        }
    }
}