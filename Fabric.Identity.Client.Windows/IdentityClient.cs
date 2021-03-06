﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Newtonsoft.Json;
using Serilog;

namespace Fabric.Identity.Client.Windows
{
    public class IdentityClient
    {
        private static OidcClient _oidcClient;
        private static string _uriPath = "signin-oidc";

        public async Task RegisterClient(IdentityClientConfiguration configuration, string installerSecret)
        {
            var installerAccessToken = await GetInstallerAccessToken(configuration, installerSecret);

            using (var httpClient = new HttpClient())
     {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installerAccessToken);

                var client = new
                {
                    ClientId = configuration.ClientId,
                    ClientName = configuration.ClientId,
                    RequireConsent = false,
                    AllowedGrantTypes = configuration.AllowedGrantTypes,
                    RedirectUris = new[] { $"{configuration.HostBaseUri}:{configuration.Port}/{_uriPath}" },
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    AllowedScopes = configuration.Scope.Split(' '),
                    RefreshTokenUsage = "ReUse"
                };
                var registrationResponse = await httpClient.PostAsync($"{configuration.Authority}api/client",
                    new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));

                if (!registrationResponse.IsSuccessStatusCode)
                {
                    if (registrationResponse.StatusCode == HttpStatusCode.Conflict)
                    {
                        Console.WriteLine($"Client {configuration.ClientId} already created.");
                        return;
                    }
                    throw new Exception($"Failed to create client {configuration.ClientId}. StatusCode {registrationResponse.StatusCode}, Error: {await registrationResponse.Content.ReadAsStringAsync()}");
                }
                dynamic clientResponse = JsonConvert.DeserializeObject(await registrationResponse.Content.ReadAsStringAsync());
                Console.WriteLine($"Client {configuration.ClientId} Created.");
                Console.WriteLine($"Client Secret: {clientResponse.clientSecret}");

            }
        }

        private static async Task<string> GetInstallerAccessToken(IdentityClientConfiguration configuration, string installerSecret)
        {
            var discoClient = new DiscoveryClient(configuration.Authority);
            var discoveryResponse = await discoClient.GetAsync();
            if (discoveryResponse.IsError)
            {
                throw new Exception($"Couldn't get discovery document: {discoveryResponse.Error}");
            }
            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "fabric-installer", installerSecret, null, AuthenticationStyle.PostValues);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("fabric/identity.manageresources");
            return tokenResponse.AccessToken;
        }

        public async Task<IdentityClientResult> SignIn(IdentityClientConfiguration configuration)

        {

            var port = configuration.Port;
            var browser = new Browser(_uriPath, port, configuration.HostBaseUri);
            string redirectUri = $"{configuration.HostBaseUri}:{port}/{_uriPath}";

            var options = new OidcClientOptions
            {
                Authority = configuration.Authority,
                ClientId = configuration.ClientId,
                RedirectUri = redirectUri,
                Scope = configuration.Scope,
                FilterClaims = false,
                Browser = browser
            };

            var logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            options.LoggerFactory.AddSerilog(logger);

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync(new LoginRequest());

            if (result.IsError)
            {
                Console.WriteLine("There was an error: {0}", result.Error);
                throw new Exception(result.Error);
            }

            return new IdentityClientResult
            {
                IdentityToken = result.IdentityToken,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                AccessTokenExpiration = result.AccessTokenExpiration,
                UserName = result.User?.Identity?.Name
            };
        }
    }
}
