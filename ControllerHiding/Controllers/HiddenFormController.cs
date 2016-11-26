using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControllerHiding.Models;
using ControllerHiding.Routing;

namespace ControllerHiding.Controllers
{
    public class HiddenFormController : Controller
    {
        public ActionResult Index()
        {
            object modelState;
            if (TempData.TryGetValue("ModelState", out modelState))
            {
                ViewData.ModelState.Merge((ModelStateDictionary)modelState);
            }

            object viewDataModel;
            var model = new HiddenFormModel();
            if (TempData.TryGetValue("MyModel", out viewDataModel))
            {
                model = viewDataModel as HiddenFormModel;
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult MyFormAction(HiddenFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToHome(model);
            }

            //Save model data in DB or so

            return RedirectToHome();
        }

        private ActionResult RedirectToHome(object model = null)
        {
            if (model != null)
            {
                ViewData["MyModel"] = model;
            }

            return new HomeActionResult();
        }
    }
}