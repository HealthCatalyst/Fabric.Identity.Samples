using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Fabric.Identity.Samples.Mvc.Configuration;
using Fabric.Platform.Http;
using Fabric.Platform.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Serilog.Core;

namespace Fabric.Identity.Samples.Mvc
{
    public class Startup
    {
        private IAppConfiguration _appConfig;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            _appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(Configuration, _appConfig);
            var idServerSettings = _appConfig.IdentityServerConfidentialClientSettings;
            Func<IServiceProvider, IHttpClientFactory> httpClientFactorySupplier = p => new HttpClientFactory(idServerSettings.Authority, idServerSettings.ClientId,
                idServerSettings.ClientSecret, "", "");
            // Add framework services.
            services.AddScoped<IHttpClientFactory>(httpClientFactorySupplier);
            services.AddMvc();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            var idServerSettings = _appConfig.IdentityServerConfidentialClientSettings;

            var levelSwitch = new LoggingLevelSwitch();
            var logger = LogFactory.CreateLogger(levelSwitch, _appConfig.ElasticSearchSettings, idServerSettings.ClientId);

            loggerFactory.AddSerilog(logger);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });


            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = idServerSettings.Authority,
                RequireHttpsMetadata = false,

                ClientId = idServerSettings.ClientId,
                ClientSecret = idServerSettings.ClientSecret,

                ResponseType = "code id_token",
                Scope = { "openid", "profile", "fabric.profile", "patientapi", "offline_access"},

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseOwin()
                .UseFabricLoggingAndMonitoring(logger, () => Task.FromResult(true), levelSwitch);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });



        }
    }
}
