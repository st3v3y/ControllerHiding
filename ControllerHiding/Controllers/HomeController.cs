using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            return View();
        }
    }
}