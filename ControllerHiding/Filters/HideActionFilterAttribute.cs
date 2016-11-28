using System.Web.Mvc;
using ControllerHiding.Constants;

namespace ControllerHiding.Filters
{
    public class HideActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewData = filterContext.Controller.ViewData;
            var routeValues = filterContext.RouteData.Values;

            object identifier;
            if (!routeValues.TryGetValue(KeyConstants.Identifier, out identifier))
            {
                return;
            }

            viewData.Add(KeyConstants.Identifier, identifier);

            var tempData = filterContext.Controller.TempData;

            object formIdentifier;
            if (!tempData.TryGetValue(KeyConstants.FormIdentifier, out formIdentifier))
            {
                return;
            }

            if (identifier?.ToString() != formIdentifier?.ToString())
            {
                return;
            }

            object modelState;
            if (!tempData.TryGetValue(KeyConstants.ModelState, out modelState))
            {
                return;
            }

            viewData.ModelState.Merge((ModelStateDictionary)modelState);

            object model;
            if (tempData.TryGetValue(KeyConstants.SubModel, out model))
            {
                viewData[KeyConstants.IdentifiedSubModel] = model;
            }

        }
    }
}