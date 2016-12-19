using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControllerHiding.Models;

namespace ControllerHiding.Controllers
{
    public class StandardFormController : Controller
    {
        // GET: StandardForm
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MyFormAction(StandardFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            //Save model data in DB or so

            return RedirectToAction("Index");
        }
    }
}