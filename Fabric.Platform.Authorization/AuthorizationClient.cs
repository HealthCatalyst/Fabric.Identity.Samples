using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Fabric.Platform.Http;
using Newtonsoft.Json;

namespace Fabric.Platform.Authorization
{
    public class AuthorizationClient : IAuthorizationClient, IDisposable
    {
        private readonly Uri _uriBase = new Uri("http://localhost:5004");
        private HttpClient _client;

        public AuthorizationClient(IHttpClientFactory clientFactory)
        {
            _client = clientFactory?.CreateWithAccessToken(_uriBase, string.Empty) ?? throw new ArgumentNullException(nameof(clientFactory));
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

        public async Task<dynamic> GetPermission(string grain, string securableItem, string name)
        {
            var permissionResponse = await _client.GetAsync($"/permissions/{grain}/{securableItem}/{name}");
            permissionResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject(await permissionResponse.Content.ReadAsStringAsync());
        }

        public async Task<dynamic> CreatRole(dynamic role)
        {
            var roleResponse = await _client.PostAsync("/roles", CreateJsonContent(role));
            roleResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject(await roleResponse.Content.ReadAsStringAsync());
        }

        public async Task<dynamic> GetRole(string grain, string securableItem, string name)
        {
            var roleResponse = await _client.GetAsync($"/roles/{grain}/{securableItem}/{name}");
            roleResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject(await roleResponse.Content.ReadAsStringAsync());
        }

        public async Task<bool> AddPermissionToRole(dynamic permission, dynamic role)
        {
            var rolePermissionResponse = await _client.PostAsync($"/roles/{role.id}/permissions", CreateJsonContent(new [] {permission}));
            rolePermissionResponse.EnsureSuccessStatusCode();
            return rolePermissionResponse.IsSuccessStatusCode;
        }

        public async Task<bool> AddRoleToGroup(dynamic role, string groupName)
        {
            var groupUrlStub = WebUtility.UrlEncode(groupName);
            var groupRoleResponse = await _client.PostAsync($"/groups/{groupUrlStub}/roles", CreateJsonContent(role));
            groupRoleResponse.EnsureSuccessStatusCode();
            return groupRoleResponse.IsSuccessStatusCode;
        }

        public async Task<dynamic> GetGroupByName(string groupName)
        {
            var groupUrlStub = WebUtility.UrlEncode(groupName);
            var groupResponse = await _client.GetAsync($"/groups/{groupUrlStub}/roles");
            groupResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject(await groupResponse.Content.ReadAsStringAsync());
        }

        public async Task<UserPermissions> GetPermissionsForUser(string grain, string securableItem)
        {
            var permissionResponse = await _client.GetAsync($"/user/permissions?grain={grain}&securableItem={securableItem}");
            permissionResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<UserPermissions>(await permissionResponse.Content.ReadAsStringAsync());
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

        ~AuthorizationClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
    public class UserPermissions
    {
        public IEnumerable<string> Permissions { get; set; }
        public string RequestedGrain { get; set; }
        public string RequestedSecurableItem { get; set; }
    }
}
