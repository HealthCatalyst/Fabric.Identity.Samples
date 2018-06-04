using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel.Client;

namespace WebApi2.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public async Task<string> Get(string id)
        {
            var identityUrl = "https://{fabric-identity-url}";
            var tokenEndpoint = $"{identityUrl}/connect/token";
            var clientId = "sample-api-client";
            var clientSecret = "{replace-me}";
            
            var tokenClient = new TokenClient(tokenEndpoint, clientId, clientSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("fabric/identity.manageresources");

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

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
