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

            return View();
        }

    }
}