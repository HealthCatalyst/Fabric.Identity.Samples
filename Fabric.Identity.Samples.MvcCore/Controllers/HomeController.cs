using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Identity.Samples.MvcCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Claims()
        {
            ViewBag.Message = "Claims";

            var cp = (ClaimsPrincipal)User;
            await AddTokensToClaims(cp);
            ViewData["access_token"] = cp.FindFirst("access_token").Value;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyIdentityClient()
        {
            var cp = (ClaimsPrincipal)User;
            await AddTokensToClaims(cp);

            var token = cp.FindFirst("access_token").Value;

            var client = new HttpClient();
            client.SetBearerToken(token);

            var result = await client.GetStringAsync("{identity-url}/api/client/{client-id}");
            ViewBag.Json = result;
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
        }

        private async Task AddTokensToClaims(ClaimsPrincipal user)
        {
            var oidcInfo = await Request.HttpContext.Authentication.GetAuthenticateInfoAsync("oidc");
            var accessToken = oidcInfo.Properties.Items[".Token.access_token"];
            var refreshToken = oidcInfo.Properties.Items[".Token.refresh_token"];
            var idToken = oidcInfo.Properties.Items[".Token.id_token"];
            var expiresAt = oidcInfo.Properties.Items[".Token.expires_at"];
            var oidcIdentity = user.Identities.First(i => i.AuthenticationType == "AuthenticationTypes.Federation");

            oidcIdentity.AddClaim(new Claim("access_token", accessToken));
            oidcIdentity.AddClaim(new Claim("refresh_token", refreshToken));
            oidcIdentity.AddClaim(new Claim("id_token", idToken));
            oidcIdentity.AddClaim(new Claim("expires_at", expiresAt));
        }
    }
}