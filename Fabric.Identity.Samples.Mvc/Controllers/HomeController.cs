using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Fabric.Identity.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";

            var cp = (ClaimsPrincipal)User;
            ViewData["access_token"] = cp.FindFirst("access_token").Value;

            return View();
        }

        [Authorize]
        public async Task<ActionResult> MyIdentityClient()
        {
            var token = (User as ClaimsPrincipal)?.FindFirst("access_token").Value;

            var client = new HttpClient();
            client.SetBearerToken(token);

            var result = await client.GetStringAsync("{identity-url}/api/client/{client-id}");
            ViewBag.Json = result;
            return View();
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}