//Copyright 2017 HealthCatalyst

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Fabric.Identity.Samples.MvcCore.Services;
using Fabric.Identity.Samples.MvcCore.Configuration;
using Fabric.Platform.Bootstrappers.AspNetCoreMvc;
using Fabric.Platform.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Fabric.Identity.Samples.MvcCore
{
    public class Startup
    {
        private IAppConfiguration _appConfig;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _appConfig = new AppConfiguration();
            Configuration.Bind(_appConfig);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(_appConfig);
            services.AddScoped<IFabricAuthorizationService, FabricAuthorizationService>();
            services.AddHttpClientFactory(_appConfig.IdentityServerConfidentialClientSettings);

// Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var idServerSettings = _appConfig.IdentityServerConfidentialClientSettings;

            var levelSwitch = new LoggingLevelSwitch();
            var logger = LogFactory.CreateLogger(levelSwitch, _appConfig.ElasticSearchSettings,
                idServerSettings.ClientId);

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
                Scope =
                {
                    "openid",
                    "profile",
                    "fabric.profile",
                    //"patientapi",
                    "fabric/authorization.read",
                    "fabric/authorization.write",
                    "offline_access"
                },

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
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}