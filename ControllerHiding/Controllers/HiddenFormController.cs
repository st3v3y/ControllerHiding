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
        public ActionResult Index(HiddenFormModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult MyFormAction(HiddenFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToHome();
            }

            //Save model data in DB or so

            return RedirectToHome();
        }

        private ActionResult RedirectToHome()
        {
            return new HomeActionResult();
        }
    }
}