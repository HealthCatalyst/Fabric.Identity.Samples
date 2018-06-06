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
                ClientId = "{replace-me}",
                ClientSecret = "{replace-me}",
                RedirectUri = "{app_root}/signin-oidc",
                PostLogoutRedirectUri = "{app_root}/signout-callback-oidc",
                Authority = "{identity_url}",
                ResponseType = "code id_token",
                Scope = "openid profile fabric.profile offline_access",
                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",
                RequireHttpsMetadata = false
            });
        }
    }
}