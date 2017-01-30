using System;

namespace ControllerHiding.Attributes
{
    public class ChildRouteDefaultAttribute : Attribute
    {
        public string RouteParamName { get; }
        public string DefaultValue { get; }

        public ChildRouteDefaultAttribute(string routeParamName, string defaultValue)
        {
            RouteParamName = routeParamName;
            DefaultValue = defaultValue;
        }
    }
}