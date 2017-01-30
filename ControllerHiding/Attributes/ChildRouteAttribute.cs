using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ControllerHiding.Constants;
using ControllerHiding.Helper;

namespace ControllerHiding.Attributes
{
    public class ChildRouteAttribute : ActionFilterAttribute, IActionFilter
    {
        public readonly string Route;

        public ChildRouteAttribute(string route)
        {
            Route = route;
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            object identifierObj;
            if (!filterContext.RouteData.Values.TryGetValue(KeyConstants.Identifier, out identifierObj))
            {
                return;
            }

            object routeParamsObj;
            if (!filterContext.ParentActionViewContext.RouteData.DataTokens.TryGetValue("params", out routeParamsObj) || !(routeParamsObj is string[]))
            {
                return;
            }

            var routeParams = (string[]) routeParamsObj;
            var identifier = identifierObj.ToString();

            int identifierIndex = Array.FindIndex(routeParams, x => x == identifier);
            if (identifierIndex == -1)
            {
                return;
            }

            MatchCollection routeMatchCollection = ChildRouteHelper.GetChildRouteMatchCollection(Route);
            int nextParamIndex = identifierIndex + 1;
            foreach (Match routeMatch in routeMatchCollection)
            {
                var parameterDescriptor = filterContext.ActionDescriptor.GetParameters().FirstOrDefault(x => x.ParameterName == routeMatch.Value);
                if (parameterDescriptor == null || routeParams.Length <= nextParamIndex)
                {
                    return;
                }
                var routeParam = routeParams[nextParamIndex];
                try {
                    var parameterValue = TypeDescriptor.GetConverter(parameterDescriptor.ParameterType).ConvertFromInvariantString(routeParam);
                    filterContext.ActionParameters[routeMatch.Value] = parameterValue;
                }
                catch (Exception){}
                nextParamIndex++;
            }
        }
    }
}