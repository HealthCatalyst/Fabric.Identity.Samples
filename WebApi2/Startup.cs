using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin;
using Owin;
using WebApi2;

[assembly: OwinStartup(typeof(Startup))]

namespace WebApi2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "http://localhost:5001",
                RequiredScopes = new[] {"sample-api"}
            });


            app.UseWebApi(WebApiConfig.Register());
        }
    }
}