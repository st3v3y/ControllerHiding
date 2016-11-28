using System.Web.Mvc;
using ControllerHiding.Filters;
using ControllerHiding.Routing;

namespace ControllerHiding.Controllers
{
    [HideActionFilter]
    public abstract class BaseHideController<TModel> : Controller
        where TModel : class, new()
    {
        public ActionResult Index()
        {
            object viewDataModel;
            var model = new TModel();
            object isPostedModel;
            if (ViewData.TryGetValue("IsPostedModel", out isPostedModel) && isPostedModel is bool && (bool)isPostedModel && TempData.TryGetValue("MyModel", out viewDataModel))
            {
                model = viewDataModel as TModel;
                return Index(model);
            }

            return Index(model);
        }

        protected abstract ActionResult Index(TModel model);

        protected ActionResult RedirectToHome(object model = null)
        {
            if (model != null)
            {
                ViewData["MyModel"] = model;
            }
            return new HomeActionResult();
        }
    }
}