using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Fabric.Identity.Samples.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Fabric.Identity.Samples.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "oidc",
                ClientId = "fabric-mvc-sample",
                ClientSecret = "secret",
                RedirectUri = "http://localhost:51627/signin-oidc",
                PostLogoutRedirectUri = "http://localhost:51627/signout-callback-oidc",
                Authority = "http://localhost/identity",
                ResponseType = "code id_token",
                Scope = "openid profile fabric.profile offline_access",
                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",
                RequireHttpsMetadata = false
            });
        }
    }
}