using System.Threading.Tasks;
using Fabric.Identity.Samples.API.Configuration;
using Fabric.Platform.Auth;
using Fabric.Platform.Bootstrappers.Nancy;
using Fabric.Platform.Http;
using Fabric.Platform.Logging;
using LibOwin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using Serilog.Core;

namespace Fabric.Identity.Samples.API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private const string DefaultCorsPolicy = "default";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(env.ContentRootPath);

            _config = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebEncoders();
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy(DefaultCorsPolicy, policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(_config, appConfig);
            var idServerSettings = appConfig.IdentityServerConfidentialClientSettings;

            var levelSwitch = new LoggingLevelSwitch();
            var logger = LogFactory.CreateLogger(levelSwitch, appConfig.ElasticSearchSettings, idServerSettings.ClientId);
            
            app.UseCors(DefaultCorsPolicy);
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = idServerSettings.Authority,
                RequireHttpsMetadata = false,

                ApiName = idServerSettings.ClientId
            });
            app.UseOwin()
                .UseFabricLoggingAndMonitoring(logger, () => Task.FromResult(true), levelSwitch)
                .UseAuthPlatform(idServerSettings.Scopes)
                .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(logger, appConfig));
        }
    }
}
