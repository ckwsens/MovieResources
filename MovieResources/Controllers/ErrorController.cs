using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/NotAdmin/
        public ActionResult NotAdmin()
        {
            return View();
        }

        //
        // GET: /Error/NotFound/
        public ActionResult NotFound()
        {
            return View();
        }
    }
}