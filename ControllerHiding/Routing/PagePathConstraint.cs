using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ControllerHiding.Routing
{
    public class PagePathConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            route.DataTokens.Clear();

            if (values[parameterName] == null)
            {
                return true;
            }

            var pagePath = values[parameterName].ToString();

            string[] pageSlugs = pagePath.Split('/');
            if (pageSlugs.Length == 1 && pageSlugs[0] == string.Empty)
            {
                route.DataTokens.Add("page", "Home");
                route.DataTokens.Add("path", pagePath);

                return true; //default
            }

            if (!PageExtists(pageSlugs))
            {
                return true;
            }

            //We can save same information for later, if we want to
            route.DataTokens.Add("page", pageSlugs[0]);
            route.DataTokens.Add("path", pagePath);

            return true;
        }

        /// <summary>
        /// Here we are checking, if the called "page" exists. Normally, we would check against the database
        /// </summary>
        /// <param name="urlSegments">Splittet URL segments</param>
        /// <returns></returns>
        private static bool PageExtists(string[] urlSegments)
        {
            if (urlSegments[0] == "MyHomepage")
            {
                if (urlSegments.Length == 1 || urlSegments[1] == string.Empty || urlSegments[1] == "MyAction")
                {
                    return true; 
                }
            }
            return false;
        }
    }
}