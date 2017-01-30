using System.Web.Mvc;
using ControllerHiding.Constants;

namespace ControllerHiding.Controllers.Base
{
    public abstract class BaseHideController : ChildController
    {
        private ViewDataDictionary ParentViewData => ControllerContext.ParentActionViewContext.ViewData;

        protected string RouteIdentifier => RouteData.GetRequiredString("Identifier");
        protected object Model => RouteData.Values["Model"];

        protected bool IsFormSubmitted
        {
            get
            {
                bool formSubmittedValue;
                return bool.TryParse(RouteData.Values["IsFormSubmitted"]?.ToString(), out formSubmittedValue) && formSubmittedValue;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ControllerContext.IsChildAction)
            {
                return;
            }
            ViewData.Add(KeyConstants.Identifier, RouteIdentifier);

            if (!IsFormSubmitted)
            {
                return;
            }

            if (!ParentViewData.ModelState.IsValid)
            {
                ViewData.ModelState.Merge(ParentViewData.ModelState);
            }

            filterContext.ActionParameters["model"] = Model;
        }
    }
}