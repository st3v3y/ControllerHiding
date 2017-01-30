using System.Diagnostics.Contracts;
using System.Web.Routing;

namespace ControllerHiding.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static void AddOrSetRouteValue(this RouteValueDictionary routeValueCollection, string routeValueName, object value)
        {
            Contract.Requires(routeValueCollection != null);

            if (routeValueCollection.ContainsKey(routeValueName))
            {
                routeValueCollection[routeValueName] = value;
            }
            else
            {
                routeValueCollection.Add(routeValueName, value);
            }
        }
    }
}