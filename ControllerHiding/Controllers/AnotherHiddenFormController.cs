using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControllerHiding.Models;

namespace ControllerHiding.Controllers
{
    public class AnotherHiddenFormController : BaseHideController<AnotherHiddenFormModel>
    {
        protected override ActionResult Index(AnotherHiddenFormModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult MyFormAction(AnotherHiddenFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToHome(model);
            }

            //Save model data in DB or so

            return RedirectToHome();
        }
    }
}