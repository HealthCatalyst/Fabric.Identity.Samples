using System;
using System.Security.Claims;
using LibOwin;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Owin;
using Nancy.TinyIoc;

namespace Fabric.Identity.Samples.API
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            var principal = context.GetOwinEnvironment()[OwinConstants.RequestUser] as ClaimsPrincipal;
            
            context.CurrentUser = principal;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            pipelines.OnError.AddItemToEndOfPipeline((ctx, ex) =>
            {
                Console.WriteLine($"Unhandled error on request: {ctx.Request.Url}. Error Message: {ex.Message}");
                return ctx.Response;
            });
        }
    }
}
