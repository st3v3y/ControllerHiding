using System.Web.Mvc;
using ControllerHiding.DTO;

namespace ControllerHiding.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var page = RouteData.DataTokens["page"] as Page;
            if (page == null)
            {
                return HttpNotFound();
            }

            return View(page);
        }
    }
}