using System.Web.Mvc;
using ControllerHiding.Constants;
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
            var model = new TModel();
            object viewDataModel;
            if (ViewData.TryGetValue(KeyConstants.IdentifiedSubModel, out viewDataModel))
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
                ViewData[KeyConstants.SubModel] = model;
            }
            return new HomeActionResult();
        }
    }
}