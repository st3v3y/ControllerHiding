using System.Web.Mvc;
using ControllerHiding.Controllers.Base;
using ControllerHiding.Models;

namespace ControllerHiding.Controllers
{
    public class AnotherHiddenFormController : BaseHideController
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
                return View(model);
            }

            //Save model data in DB or so

            return RedirectToAction("Success", new AnotherHiddenFormSuccessModel()
            {
                Name = model.Name,
                Street = model.Street
            });
        }

        public ActionResult Success(AnotherHiddenFormSuccessModel model)
        {
            return View("Success", model);
        }
    }
}