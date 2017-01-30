using System.Web.Mvc;
using ControllerHiding.Controllers.Base;
using ControllerHiding.Models;

namespace ControllerHiding.Controllers
{
    public class HiddenFormController : BaseHideController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MyFormAction(HiddenFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Save model data in DB or so

            return RedirectToAction("Index");
        }
    }
}