using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DelegationController : Controller
    {
        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var identityUrl = "http://localhost:5001";
            var clientId = "sample-api-client";
            var clientSecret = "{replace-me}";

            var discoveryResponse = await DiscoveryClient.GetAsync(identityUrl);
            if (discoveryResponse.IsError)
            {
                throw new Exception($"Could not get discovery document from Fabric.Identity at {identityUrl}. Error is: {discoveryResponse.Error}.");
            }

            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, clientId, clientSecret);
            var accessTokenFromRequest = await HttpContext.GetTokenAsync("access_token");
            var tokenResponse = await tokenClient.RequestCustomGrantAsync("delegation", "fabric/identity.manageresources", new { token = accessTokenFromRequest });

            if (tokenResponse.IsError)
            {
                throw new Exception($"Could not get token for client: {clientId} from authority: {tokenClient.Address}. Error is {tokenResponse.Error}.");
            }

            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{identityUrl}/api/client/{id}");
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                httpRequestMessage.Headers.Add("Accept", "application/json");

                var httpResponse = await httpClient.SendAsync(httpRequestMessage);
                httpResponse.EnsureSuccessStatusCode();

                var content = await httpResponse.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}