using System.Linq;
using ControllerHiding.Attributes;
using ControllerHiding.Repositories;

namespace ControllerHiding.CustomAttributes
{
    public class ChildRouteBlogEntryYearConstraintAttribute : ChildRouteConstraintAttribute
    {
        public ChildRouteBlogEntryYearConstraintAttribute(string routeParamName) 
            : base(routeParamName)
        {
        }

        public override bool IsValidRouteValue(object routeValue)
        {
            var value = routeValue as int?;
            if (value == null)
            {
                return false;
            }

            return new BlogEntryRepository().GetAllBlogEntries().Any(x => x.Date.Year == value.Value);
        }
    }
}