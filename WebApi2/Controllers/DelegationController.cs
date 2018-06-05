using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using IdentityModel.Client;

namespace WebApi2.Controllers
{
    [Authorize]
    public class DelegationController : ApiController
    {
        // GET api/<controller>/5
        public async Task<string> Get(string id)
        {
            var identityUrl = "https://{fabric-identity-url}";
            var tokenEndpoint = $"{identityUrl}/connect/token";
            var clientId = "sample-api-client";
            var clientSecret = "{replace-me}";

            var tokenClient = new TokenClient(tokenEndpoint, clientId, clientSecret);
            var accessTokenFromRequest = HttpContext.Current?.Request.Headers["Authorization"].Replace("Bearer ", string.Empty);
            var tokenResponse = await tokenClient.RequestCustomGrantAsync("delegation", "fabric/identity.manageresources", new { token = accessTokenFromRequest });

            if (tokenResponse.IsError)
            {
                throw new Exception($"Could not get token for client: {clientId} from authority: {tokenEndpoint}. Error is {tokenResponse.Error}.");
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