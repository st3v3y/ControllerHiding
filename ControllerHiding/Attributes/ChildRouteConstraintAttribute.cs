using System;

namespace ControllerHiding.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ChildRouteConstraintAttribute : Attribute
    {
        public string RouteParamName { get; }

        protected ChildRouteConstraintAttribute(string routeParamName)
        {
            RouteParamName = routeParamName;
        }

        public abstract bool IsValidRouteValue(object routeValue);
    }
}