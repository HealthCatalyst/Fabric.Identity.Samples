using System;
using System.Security.Claims;
using Fabric.Identity.Samples.API.Configuration;
using Fabric.Platform.Bootstrappers.Nancy;
using LibOwin;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Owin;
using Nancy.TinyIoc;
using Serilog;

namespace Fabric.Identity.Samples.API
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly ILogger _logger;
        private readonly IAppConfiguration _appConfig;

        public Bootstrapper(ILogger logger, IAppConfiguration appConfig)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (appConfig == null)
            {
                throw new ArgumentNullException(nameof(appConfig));
            }
            _logger = logger;
            _appConfig = appConfig;
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            var principal = context.GetOwinEnvironment()[OwinConstants.RequestUser] as ClaimsPrincipal;
            context.CurrentUser = principal;
            var appConfig = container.Resolve<IAppConfiguration>();
            container.UseHttpClientFactory(context, appConfig.IdentityServerConfidentialClientSettings);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            pipelines.OnError.AddItemToEndOfPipeline((ctx, ex) =>
            {
                _logger.Error(ex, "Unhandled error on request: @{Url}. Error Message: @{Message}", ctx.Request.Url, ex.Message);
                return ctx.Response;
            });
            container.Register(_appConfig);
        }
    }
}
