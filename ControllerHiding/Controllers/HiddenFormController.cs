﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControllerHiding.Models;

namespace ControllerHiding.Controllers
{
    public class HiddenFormController : BaseHideController<HiddenFormModel>
    {
        protected override ActionResult Index(HiddenFormModel model)
        {
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
    }
}