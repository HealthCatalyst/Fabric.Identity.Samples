using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Fabric.Identity.InteractiveConsole
{
    class Program
    {
        private static OidcClient _oidcClient;
        private static string _uriPath = "signin-oidc";

        static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();
            Console.WriteLine("Enter the fabric-installer secret to register the test client.");
            var installerSecret = Console.ReadLine();

            await RegisterClient(configuration, installerSecret);

            Console.WriteLine("Sign in with OIDC");
            Console.WriteLine("Press any key to start the sign in process...");
            Console.ReadKey();
            
            await SignIn(configuration);
            
        }

        private static async Task RegisterClient(AppConfiguration configuration, string installerSecret)
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
                    AllowedGrantTypes = new[] {"hybrid"},
                    RedirectUris = new[] {$"{configuration.HostBaseUri}:{configuration.Port}/{_uriPath}"},
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    AllowedScopes = configuration.Scope.Split(" "),
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

        private static async Task<string> GetInstallerAccessToken(AppConfiguration configuration, string installerSecret)
        {
            var discoClient = new DiscoveryClient(configuration.Authority);
            var discoveryResponse = await discoClient.GetAsync();
            if (discoveryResponse.IsError)
            {
                throw new Exception($"Couldn't get discovery document: {discoveryResponse.Error}");
            }
            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "fabric-installer", installerSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("fabric/identity.manageresources");
            return tokenResponse.AccessToken;
        }

        private static AppConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(config, appConfig);
            return appConfig;
        }

        private static async Task SignIn(AppConfiguration configuration)
        {

            var port = configuration.Port;
            var browser = new Browser(_uriPath, configuration);
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
            }

            Console.WriteLine("Identity Token: {0}", result.IdentityToken);
            Console.WriteLine("Access Token: {0}", result.AccessToken);
            Console.WriteLine("Refresh Token: {0}", result.RefreshToken);

        }
    }
}
