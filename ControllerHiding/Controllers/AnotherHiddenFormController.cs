using System.Web.Mvc;
using ControllerHiding.Models;

namespace ControllerHiding.Controllers
{
    public class AnotherHiddenFormController : BaseHideController<AnotherHiddenFormModel>
    {
        public ActionResult Index()
        {
            return View(Model);
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