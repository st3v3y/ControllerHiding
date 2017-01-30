using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Routing;
using ControllerHiding.Attributes;
using ControllerHiding.Controllers;

namespace ControllerHiding.Helper
{
    public static class ChildRouteHelper
    {
        public static MatchCollection GetChildRouteMatchCollection(string route)
        {
            return new Regex(@"(?<=\{)[^}]*(?=\})").Matches(route);
        }

        public static string[] GetChildRouteSegments(RouteValueDictionary routeValues, MatchCollection routeMachings, Dictionary<string, string> routeDefaults)
        {
            var routeSegments = new List<string>();
            foreach (Match routeMatch in routeMachings)
            {
                //ToDo: Check if parameter is optional (optional parameters are currently not supported)
                if (!routeValues.ContainsKey(routeMatch.Value))
                {
                    throw new Exception("The route value for \"" + routeMatch.Value + "\" has to be provided!");
                }
                string routeValue = routeValues[routeMatch.Value]?.ToString();
                if (string.IsNullOrEmpty(routeValue))
                {
                    if (!routeDefaults.TryGetValue(routeMatch.Value, out routeValue))
                    {
                        routeValue = "-";
                    }
                }
                routeSegments.Add(routeValue);
            }
            return routeSegments.ToArray();
        }

        public static string GetChildRoute(string action, string controller)
        {
            Dictionary<string, string> defaults;
            ParameterInfo[] parameters;
            Dictionary<string, Func<object, bool>[]> constraints;
            return GetChildRouteInternal(action, controller, false, false, false, out defaults, out parameters, out constraints);
        }

        public static string GetChildRoute(string action, string controller, out Dictionary<string, string> defaults)
        {
            ParameterInfo[] parameters;
            Dictionary<string, Func<object, bool>[]> constraints;
            return GetChildRouteInternal(action, controller, true, false, false, out defaults, out parameters, out constraints);
        }

        public static string GetChildRoute(string action, string controller, out Dictionary<string, string> defaults, out ParameterInfo[] parameters)
        {
            Dictionary<string, Func<object, bool>[]> constraints;
            return GetChildRouteInternal(action, controller, true, true, false, out defaults, out parameters, out constraints);
        }

        public static string GetChildRoute(string action, string controller, out Dictionary<string, string> defaults, out ParameterInfo[] parameters, out Dictionary<string, Func<object,bool>[]> constraints)
        {
            return GetChildRouteInternal(action, controller, true, true, true, out defaults, out parameters, out constraints);
        }

        private static string GetChildRouteInternal(string action, string controller, bool setDefaults, bool setParameters, bool setConstraints, out Dictionary<string, string> defaults, out ParameterInfo[] parameters, out Dictionary<string, Func<object, bool>[]> constraints)
        {
            Type controllerType = ControllerHelper.GetControllerType(controller);
            if (controllerType == null)
            {
                throw new Exception("Invalid controller");
            }

            var methodInfo = controllerType.GetMethod(action);
            var customAttributes = methodInfo.GetCustomAttributes(true);
            var attribute = customAttributes.OfType<ChildRouteAttribute>().FirstOrDefault();

            defaults = new Dictionary<string, string>();
            if (setDefaults)
            {
                defaults = customAttributes.OfType<ChildRouteDefaultAttribute>().ToDictionary(x => x.RouteParamName, y => y.DefaultValue);
            }

            parameters = new ParameterInfo[] {};
            if (setParameters)
            {
                parameters = methodInfo.GetParameters();
            }

            constraints = new Dictionary<string, Func<object, bool>[]>();
            if (setConstraints)
            {
                constraints = customAttributes
                    .OfType<ChildRouteConstraintAttribute>()
                    .GroupBy(x => x.RouteParamName, x => (Func<object, bool>)x.IsValidRouteValue, (key, funcs) => new { key, funcs = funcs.ToArray() })
                    .ToDictionary(x => x.key, x => x.funcs);
            }

            return attribute != null
                ? attribute.Route
                : string.Empty;
        }

    }
}