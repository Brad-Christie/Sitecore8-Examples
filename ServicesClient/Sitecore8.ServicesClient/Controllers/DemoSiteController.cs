using System.Web.Mvc;

namespace Sitecore8.ServicesClient.Controllers
{
    //
    // GET: /Sitecore8-ServicesClient/
    public class DemoSiteController : Controller
    {
        //
        // GET: /
        // GET: /Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Custom
        public ActionResult Custom()
        {
            return View();
        }

        //
        // GET: /Default
        public ActionResult Default()
        {
            return View();
        }
    }
}