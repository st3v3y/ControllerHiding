using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControllerHiding.Constants;

namespace ControllerHiding.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (RouteData.DataTokens["page"] == null)
            {
                return HttpNotFound();
            }

            PrepareTempDataForHiddenControllers();

            return View();
        }

        private void PrepareTempDataForHiddenControllers()
        {
            object subModel;
            if (ViewData.TryGetValue(KeyConstants.SubModel, out subModel))
            {
                TempData[KeyConstants.SubModel] = subModel;
            }

            object formIdentifier;
            if (ViewData.TryGetValue(KeyConstants.FormIdentifier, out formIdentifier))
            {
                TempData[KeyConstants.FormIdentifier] = formIdentifier;
            }

            if (!ViewData.ModelState.IsValid)
            {
                TempData[KeyConstants.ModelState] = ViewData.ModelState;
            }
        }
    }
}