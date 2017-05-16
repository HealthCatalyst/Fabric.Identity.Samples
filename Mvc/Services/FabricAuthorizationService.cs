using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Fabric.Platform.Http;
using Newtonsoft.Json;

namespace Fabric.Identity.Samples.Mvc.Services
{
    public class FabricAuthorizationService : IFabricAuthorizationService, IDisposable
    {
        private readonly Uri UriBase = new Uri("http://localhost:5004");
        private HttpClient _client;

        public FabricAuthorizationService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory?.CreateWithAccessToken(UriBase, string.Empty) ?? throw new ArgumentNullException(nameof(clientFactory));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetAccessToken(string accessToken)
        {
            //TODO: need to move the access token into the httpclient factory so we can inject it better.
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<dynamic> CreatePermission(dynamic permission)
        {
            var patientPermissionResponse = await _client.PostAsync("/permissions", CreateJsonContent(permission));
            patientPermissionResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject(await patientPermissionResponse.Content.ReadAsStringAsync());
        }

        public async Task<dynamic> CreatRole(dynamic role)
        {
            var roleResponse = await _client.PostAsync("/roles", CreateJsonContent(role));
            roleResponse.EnsureSuccessStatusCode();
            return  JsonConvert.DeserializeObject(await roleResponse.Content.ReadAsStringAsync());
        }

        public async Task AddPermissionToRole(dynamic permission, dynamic role)
        {
            var rolePermissionResponse = await _client.PostAsync($"/roles/{role.id}/permissions",
                CreateJsonContent(new[] { permission }));
            rolePermissionResponse.EnsureSuccessStatusCode();
        }

        public async Task AddRoleToGroup(dynamic role, string groupName)
        {
            var groupRoleResponse = await _client.PostAsync($"/groups/{groupName}/roles", CreateJsonContent(role));
            groupRoleResponse.EnsureSuccessStatusCode();
        }

        public async Task<dynamic> GetGroupByName(string groupName)
        {
            var groupResponse = await _client.GetAsync($"/groups/{groupName}/roles");
            groupResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject(await groupResponse.Content.ReadAsStringAsync());
        }

        private StringContent CreateJsonContent(object model)
        {
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FabricAuthorizationService()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }
    }
}
