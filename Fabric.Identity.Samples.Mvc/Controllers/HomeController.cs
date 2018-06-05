using System.Web;
using System.Web.Mvc;

namespace Fabric.Identity.Samples.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut("Cookies", "oidc");
            return Redirect("/");
        }
    }
}