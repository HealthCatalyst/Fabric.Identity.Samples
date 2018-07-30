using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Fabric.Identity.Client.Windows;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Fabric.Identity.InteractiveConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();
            Console.WriteLine("Enter the fabric-installer secret to register the test client.");
            var installerSecret = Console.ReadLine();

            var identityClient = new IdentityClient();

            await identityClient.RegisterClient(configuration, installerSecret);

            Console.WriteLine("Sign in with OIDC");
            Console.WriteLine("Press any key to start the sign in process...");
            Console.ReadKey();
            
            await identityClient.SignIn(configuration);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }

        private static IdentityClientConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(config, appConfig);
            return new IdentityClientConfiguration
            {
                HostBaseUri = appConfig.HostBaseUri,
                Authority = appConfig.Authority,
                ClientId = appConfig.ClientId,
                Port = appConfig.Port,
                Scope = appConfig.Scope
            };
        }

    }
}
