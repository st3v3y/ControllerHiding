using System.Web.Mvc;
using ControllerHiding.Constants;
using ControllerHiding.Routing;

namespace ControllerHiding.Controllers
{
    public abstract class BaseHideController<TModel> : Controller
        where TModel : class, new()
    {
        protected TModel Model => _model;

        private ViewDataDictionary ParentViewData => ControllerContext.ParentActionViewContext.ViewData;

        private TModel _model = new TModel();
        private string _routeIdentifier;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ControllerContext.IsChildAction)
            {
                return;
            }

            _routeIdentifier = GetRouteIdentifier();

            if (string.IsNullOrEmpty(_routeIdentifier))
            {
                return;
            }

            ViewData.Add(KeyConstants.Identifier, _routeIdentifier);

            string formIdentifier = GetFormIdentifier();

            if (!IsCorrespondingIdentifier(formIdentifier))
            {
                return;
            }

            if (!ParentViewData.ModelState.IsValid)
            {
                ViewData.ModelState.Merge(ParentViewData.ModelState);
            }

            _model = GetModelFromParentViewData() ?? new TModel();

            filterContext.ActionParameters["model"] = _model;
        }

        private bool IsCorrespondingIdentifier(string formIdentifier)
        {
            return !string.IsNullOrEmpty(formIdentifier) && _routeIdentifier.Equals(formIdentifier);
        }

        private string GetRouteIdentifier()
        {
            object identifier;
            return RouteData.Values.TryGetValue(KeyConstants.Identifier, out identifier)
                ? identifier.ToString()
                : string.Empty;
        }

        private string GetFormIdentifier()
        {
            object formIdentifier;
            return ParentViewData.TryGetValue(KeyConstants.FormIdentifier, out formIdentifier) 
                ? formIdentifier.ToString()
                : null;
        }

        private TModel GetModelFromParentViewData()
        {
            object model;
            if (ParentViewData.TryGetValue(KeyConstants.SubModel, out model))
            {
                return model as TModel;
            }
            return null;
        }

        protected ActionResult RedirectToHome(object model = null)
        {
            ViewData[KeyConstants.SubModel] = model;
            
            return new HomeActionResult();
        }
    }
}