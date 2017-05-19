using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using Fabric.Platform.Authorization;
using Fabric.Platform.Http;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Fabric.Identity.Samples.Console
{
    class Program
    {
        private static readonly IHttpClientFactory HttpClientFactory = new HttpClientFactory("http://localhost:5001", "fabric-mvcsample", "secret", Guid.NewGuid().ToString(), "authorization-console");
        static void Main(string[] args)
        {

            var permission = args[0];
            var role = args[1];

            AddPermissionToRole(permission, role);
        }

        private static void AddPermissionToRole(string permission, string role)
        {
            var permissionItem = ParseAuthorizationItem(permission);
            var roleItem = ParseAuthorizationItem(role);

            dynamic roleFromApi;
            dynamic permissionFromApi;

            using (var authClient = new AuthorizationClient(HttpClientFactory))
            {
                authClient.SetAccessToken(GetToken("fabric/authorization.read"));
                roleFromApi = authClient.GetRole(roleItem.Grain, roleItem.SecurableItem, roleItem.Name).Result;
                System.Console.WriteLine(JsonConvert.SerializeObject(roleFromApi, Formatting.Indented));

                permissionFromApi = authClient
                    .GetPermission(permissionItem.Grain, permissionItem.SecurableItem, permissionItem.Name).Result;
                System.Console.WriteLine(JsonConvert.SerializeObject(permissionFromApi, Formatting.Indented));
            }

            using (var authClient = new AuthorizationClient(HttpClientFactory))
            {
                authClient.SetAccessToken(GetToken("fabric/authorization.write"));
                var result = authClient.AddPermissionToRole(permissionFromApi[0], roleFromApi[0]).Result;
                System.Console.WriteLine(result ? "success!" : "Failure");
            }
        }

        private static AuthorizationApiItem ParseAuthorizationItem(string item)
        {
            var parts = item.Split('/');
            if(parts.Length != 2) throw new ArgumentException("string parameter must be in the format grain/app.name");
            var subparts = parts[1].Split('.');
            if (parts.Length != 2) throw new ArgumentException("string parameter must be in the format grain/app.name");
            return new AuthorizationApiItem
            {
                Grain = parts[0],
                SecurableItem = subparts[0],
                Name = subparts[1]
            };
        }

        private static string GetToken(string scope)
        {
            var discovery = DiscoveryClient.GetAsync("http://localhost:5001").Result;
            var tokenClient = new TokenClient(discovery.TokenEndpoint, "fabric-mvcsample", "secret");
            var tokenResponse = tokenClient.RequestClientCredentialsAsync(scope).Result;
            if(tokenResponse.IsError) throw new AuthenticationException($"Could not get token for scope: {scope}");
            return tokenResponse.AccessToken;
        }
    }

    public class AuthorizationApiItem
    {
        public string Grain { get; set; }
        public string SecurableItem { get; set; }
        public string Name { get; set; }
    }
}