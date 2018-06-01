using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
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
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("fabric/identity.manageresources");

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

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
